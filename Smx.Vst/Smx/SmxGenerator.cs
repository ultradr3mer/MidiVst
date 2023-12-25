using Jacobi.Vst.Core;
using Smx.Vst.Collections;
using Smx.Vst.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Markup;
using VstNetAudioPlugin;

namespace Smx.Vst.Smx
{
  internal class SmxGenerator
  {
    private const double a4Frequency = 440.0;
    private static readonly double multiplyer = Math.Pow(2, 1.0 / 12.0);
    private static readonly Dictionary<int, float> noteFrequencies = Enumerable.Range(0, 127).ToDictionary(x => x, x => (float)(a4Frequency * Math.Pow(multiplyer, x - 69)));
    private readonly SmxParameters parameters;
    private readonly double PiBy2 = Math.PI * 2.0;
    private Dictionary<byte, KeyData> keyDataDict = new Dictionary<byte, KeyData>();
    private HashSet<byte> keys = new HashSet<byte>();

    //private List<Filter> filterList = new List<Filter>();

    private AudioEngine nativeEngine;
    private EngineParameter nativeParameter;
    private long processedSamples = 0;
    private long runtimeTicks = 0;
    private Stopwatch sw = new Stopwatch();
    //private Dictionary<short, List<Filter>> channelFilterDict = new Dictionary<short, List<Filter>>();

    public SmxGenerator(PluginParameters parameters)
    {
      this.parameters = parameters.SmxParameters;

      //foreach (var filterParameter in this.parameters.FilterParameterAry)
      //{
      //  filterList.Add(new Filter(filterParameter));
      //}

      nativeParameter = new EngineParameter();
      nativeEngine = new AudioEngine(nativeParameter);
    }

    public bool IsPlaying => keys.Any() || keyDataDict.Any();

    internal void Generate(float sampleRate, VstAudioBuffer[] outChannels)
    {
      //nativeParameter.FilterCount = (int)parameters.FilterCountMgr.CurrentValue;
      nativeParameter.FmMod = parameters.FmModMgr.CurrentValue == 1;
      nativeParameter.InitialDetune = parameters.IniDetMgr.CurrentValue;
      nativeParameter.InitialDetuneAcceleration = parameters.InTuAcMgr.CurrentValue;
      nativeParameter.InitialDetuneFriction = parameters.InTuFrMgr.CurrentValue;
      nativeParameter.Tune = parameters.TuneMgr.CurrentValue;
      nativeParameter.Pow = parameters.PowMgr.CurrentValue;
      nativeParameter.VoiceCount = (int)parameters.VoiceCountMgr.CurrentValue;
      nativeParameter.VoiceSpread = parameters.VoiceSpreadMgr.CurrentValue;
      nativeParameter.VoiceDetune = parameters.VoiceDetuneMgr.CurrentValue;
      nativeParameter.UniDetune = parameters.UniDetMgr.CurrentValue;
      nativeParameter.UniPan = parameters.UniPanMgr.CurrentValue;
      nativeParameter.SawAmount = parameters.SawMgr.CurrentValue;

      sw.Restart();

      int length = outChannels[0].SampleCount;
      for (int sampleIndex = 0; sampleIndex < length; sampleIndex++)
      {
        foreach (var key in keys)
        {
          if (!keyDataDict.TryGetValue(key, out KeyData keyActuation))
          {
            keyActuation = new KeyData()
            {
              Actuation = 0,
              Detune = (float)Math.Pow(parameters.IniDetMgr.CurrentValue * 2.0f, 2.0),
              KeyFrequency = SmxGenerator.noteFrequencies[key],
            };
            keyDataDict[key] = keyActuation;
          }

          if (keyActuation.Actuation < 1)
          {
            if (parameters.AttackMgr.CurrentValue == 0)
            {
              keyActuation.Actuation = 1;
            }
            else
            {
              keyActuation.Actuation = (float)Math.Min(keyActuation.Actuation + 1.0 / parameters.AttackMgr.CurrentValue / sampleRate, 1.0);
            }
          }
        }

        foreach (var item in keyDataDict)
        {
          var data = item.Value;
          if (data.Detune != 1.0 || data.DetuneVec != 0.0)
          {
            var pull = (1.0f - data.Detune) * parameters.InTuAcMgr.CurrentValue * 100000f;
            var scaledFriction = parameters.InTuFrMgr.CurrentValue / sampleRate * 1000f;
            data.DetuneVec = (data.DetuneVec + pull / sampleRate) * (1.0f - scaledFriction);
            data.Detune += data.DetuneVec / sampleRate;
            data.Time += data.Detune / sampleRate;
          }
          else
          {
            data.Time += 1.0f / sampleRate;
          }

          if (keys.Contains(item.Key))
          {
            continue;
          }

          float keyActuation;
          if (parameters.ReleaseMgr.CurrentValue == 0)
          {
            keyActuation = 0;
          }
          else
          {
            keyActuation = (float)Math.Max(item.Value.Actuation - 1.0 / parameters.ReleaseMgr.CurrentValue / sampleRate, 0.0);
          }

          if (keyActuation <= 0)
          {
            keyDataDict.Remove(item.Key);
          }
          else
          {
            keyDataDict[item.Key].Actuation = keyActuation;
          }
        }

        var gens = GeneratorList.List.Where(g => parameters.GenMgrs[g.Index].CurrentValue == 1)
                                      .ToList();

        double value = 0.0;
        if (gens.Any())
        {
          nativeParameter.ActiveGenerators = gens.OfType<GeneratorParameter>().ToList();
          nativeParameter.MinGenFactor = (float)nativeParameter.ActiveGenerators.Min(g => g.Factor);

          value = keyDataDict.Sum(entry => this.GenerateNote(entry.Value));
        }

        //int filterCount = (int)parameters.FilterCountMgr.CurrentValue;
        //foreach (var item in filterList.Take(filterCount))
        //{
        //  value = item.Process(value);
        //}

        short channelnNr = 0;
        foreach (var channel in outChannels)
        {
          channel[sampleIndex] = (float)value;
          channelnNr++;
        }

        sw.Stop();
        runtimeTicks += sw.ElapsedTicks;
        processedSamples += length;

        if (processedSamples >= sampleRate)
        {
          var processedTicks = (long)(length / sampleRate * TimeSpan.TicksPerSecond);
          Debug.WriteLine($"Runtime {runtimeTicks / processedTicks * 100:0.00}% {(float)runtimeTicks / (float)TimeSpan.TicksPerMillisecond:0.000}ms for {processedSamples} samples");
          runtimeTicks = 0;
          processedSamples = 0;
        }
      }
    }

    internal void ProcessNoteOffEvent(byte v)
    {
      keys.Remove(v);
    }

    internal void ProcessNoteOnEvent(byte v)
    {
      keys.Add(v);
    }

    private double GenerateNote(KeyData data)
    {
      var voiceCount = (int)parameters.VoiceCountMgr.CurrentValue;

      var noteSample = Enumerable.Range(0, voiceCount).Sum(v => this.nativeEngine.GenerateVoice(data, v));

      return noteSample;
    }

    private double GenerateVoice(byte key, KeyData data, List<GeneratorList.GeneratorItem> gens, int v)
    {
      var shiftPerVoice = parameters.VoiceSpreadMgr.CurrentValue
                      / noteFrequencies[key]
                      / gens.Min(o => o.Factor);
      var voiceTime = data.Time
                * (1.0 + v / 10.0 * parameters.VoiceDetuneMgr.CurrentValue)
                + shiftPerVoice * v;

      int shiftNr = 0;
      double CalcTime(GeneratorParameter genPara)
      {
        return voiceTime * noteFrequencies[key] * 4.0 * parameters.TuneMgr.CurrentValue * genPara.Factor
               * (1.0 + shiftNr / 100.0 * parameters.UniDetMgr.CurrentValue)
               + parameters.UniPanMgr.CurrentValue * shiftNr++ / gens.Count();
      }

      return (data.Actuation * ((parameters.FmModMgr.CurrentValue == 1)
                      ? gens.Aggregate(1.0, (a, g) => a * 1.5 * AudioEngine.Wave(parameters.SawMgr.CurrentValue, CalcTime(g), parameters.PowMgr.CurrentValue))
                      : gens.Sum(g => AudioEngine.Wave(parameters.SawMgr.CurrentValue, CalcTime(g), parameters.PowMgr.CurrentValue))));
    }

    private double Wave(double saw, double t)
    {
      t = t % 1;
      double segment13_length = (1.0 - saw) / 4.0;
      double segment2_length = (1.0 + saw) / 2.0;

      if (t < segment13_length)
      {
        t = t / (1 - saw);
      }
      else if (t < segment13_length + segment2_length)
      {
        t = (t - segment13_length) / (1.0 + saw) + 1.0 / 4.0;
      }
      else
      {
        t = 1 - (1 - t) / (1 - saw);
      }

      double sin_wave = Math.Sin(2 * Math.PI * t);

      double saw_wave = t < 1.0 / 4.0 ? t * 4.0 :
        t < 3.0 / 4.0 ? 1 - (t - 1.0 / 4.0) * 4.0 :
        -4.0 + 4.0 * t;

      double combined = (1 - saw) * sin_wave + saw * saw_wave;

      return Math.Pow(Math.Abs(combined), parameters.PowMgr.CurrentValue) * Math.Sign(combined);
    }
  }
}