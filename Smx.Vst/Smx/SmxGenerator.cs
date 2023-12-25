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

    //private Dictionary<short, KeyData> keyDataDict = new Dictionary<short, KeyData>();
    private HashSet<short> keys = new HashSet<short>();

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

    public bool IsPlaying => this.keys.Any() || this.nativeParameter.ActiveKeys.Any();

    internal void Generate(float sampleRate, VstAudioBuffer[] outChannels)
    {
      sw.Restart();
      //nativeParameter.FilterCount = (int)parameters.FilterCountMgr.CurrentValue;
      nativeParameter.SampleRate = sampleRate;
      nativeParameter.Attack = parameters.AttackMgr.CurrentValue;
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
      nativeParameter.ActiveGenerators = GeneratorList.List.Where(g => parameters.GenMgrs[g.Index].CurrentValue == 1)
                                        .OfType<GeneratorParameter>()
                                        .ToList();
      nativeParameter.MinGenFactor = (float)nativeParameter.ActiveGenerators.Min(g => g.Factor);

      int length = outChannels[0].SampleCount;
      unsafe
      {
        int channelCount = outChannels.Length;
        var bufferAry = new float*[channelCount];
        for (int i = 0; i < channelCount; i++)
        {
          bufferAry[i] = ((IDirectBufferAccess32)outChannels[i]).Buffer;
        }

        nativeEngine.Run(keys, length, bufferAry);
      }

      sw.Stop();
      runtimeTicks += sw.ElapsedTicks;
      processedSamples += length;

      if (processedSamples >= sampleRate)
      {
        var processedTicks = (long)(processedSamples * TimeSpan.TicksPerSecond / sampleRate);
        Debug.WriteLine($"Runtime {runtimeTicks / processedTicks * 100:0.00}% {(float)runtimeTicks / (float)TimeSpan.TicksPerSecond}ms for {processedSamples} samples");
        runtimeTicks = 0;
        processedSamples = 0;
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