﻿using Jacobi.Vst.Core;
using Smx.Vst.Collections;
using Smx.Vst.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;
using VstNetAudioPlugin;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

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

    private Dictionary<short, Ringbuffer<float>> decayBuffer = new Dictionary<short, Ringbuffer<float>>();
    private Dictionary<short, MembraneData> ChannelMembrane = new Dictionary<short, MembraneData>();

    public SmxGenerator(PluginParameters parameters)
    {
      this.parameters = parameters.SmxParameters;
    }

    public bool IsPlaying => keys.Any() || keyDataDict.Any();

    internal void Generate(float sampleRate, VstAudioBuffer[] outChannels)
    {
      int length = outChannels[0].SampleCount;
      for (int i = 0; i < length; i++)
      {
        foreach (var key in keys)
        {
          if (!keyDataDict.TryGetValue(key, out KeyData keyActuation))
          {
            keyActuation = new KeyData() { Actuation = 0, Detune = (float)Math.Pow(parameters.IniDetMgr.CurrentValue * 2.0f, 2.0) };
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

        var shifts = GeneratorList.List.Where(g => parameters.GenMgrs[g.Index].CurrentValue == 1)
                                      .ToList();

        var voiceCount = (int)parameters.VoiceCountMgr.CurrentValue;
        short channelnNr = 0;
        foreach (var channel in outChannels)
        {
          var newValue = (float)keyDataDict.Sum(entry =>
          {
            if (!shifts.Any())
            {
              return 0.0f;
            }

            int shiftNr = 0;
            double CalcTime(GeneratorList.GeneratorItem gen)
            {
              return entry.Value.Time * noteFrequencies[entry.Key] * 2.0 * gen.Factor * (1.0 + shiftNr / 100.0 * parameters.UniDetMgr.CurrentValue)
                     + parameters.UniPanMgr.CurrentValue * shiftNr++ / shifts.Count;
            }

            var noteSample = (float)(entry.Value.Actuation * ((parameters.FmModMgr.CurrentValue == 1) 
                            ? shifts.Aggregate(1.0, (a, s) => a * 1.5 * Wave(parameters.SawMgr.CurrentValue, CalcTime(s)))
                            : shifts.Sum(s => Wave(parameters.SawMgr.CurrentValue, CalcTime(s)))));

            if (voiceCount <= 1 || parameters.VoiceSpreadMgr.CurrentValue <= 0.0001f)
            {
              return noteSample;
            }

            var buffer = entry.Value.VoiceBuffer;
            if (buffer == null)
            {
              entry.Value.VoiceBuffer = buffer = new Ringbuffer<float>((int)(sampleRate
                                                                          / noteFrequencies[entry.Key]
                                                                          / shifts.Min(o => o.Factor)));
            }

            buffer.Add(noteSample);
            var totalSpread = (buffer.Size() * parameters.VoiceSpreadMgr.CurrentValue) - 1.0f;
            double combinedSample = 0;
            for (int voiceIndex = 0; voiceIndex <= voiceCount; voiceIndex++)
            {
              int sampleIndex = (int)(voiceIndex * totalSpread / voiceCount);
              if (buffer.Count() <= sampleIndex)
                continue;

              float value = buffer.GetFromTop(sampleIndex);
              combinedSample += value;
            }

            return combinedSample / voiceCount;
          }
          );

          channel[i] = newValue;

          channelnNr++;
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

    class KeyData
    {
      public float Actuation { get; set; }
      public float Detune { get; set; }
      public float DetuneVec { get; set; }
      public double Time { get; set; }
      public Ringbuffer<float> VoiceBuffer { get; internal set; }
    }

    class MembraneData
    {
      public float Position { get; set; }
      public float Vector { get; set; }
    }
  }
}