using Jacobi.Vst.Core;
using Smx.Vst.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using VstNetAudioPlugin;

namespace Smx.Vst.Smx
{
  internal class SmxGenerator
  {
    public long sampleIndex = 0;
    private const double a4Frequency = 440.0;
    private static readonly double multiplyer = Math.Pow(2, 1.0 / 12.0);
    private static readonly Dictionary<int, float> noteFrequencies = Enumerable.Range(0, 127).ToDictionary(x => x, x => (float)(a4Frequency * Math.Pow(multiplyer, x - 69)));
    private readonly SmxParameters parameters;
    private Dictionary<byte, float> actuationDictionary = new Dictionary<byte, float>();
    private HashSet<byte> keys = new HashSet<byte>();

    public SmxGenerator(PluginParameters parameters)
    {
      this.parameters = parameters.SmxParameters;
    }

    public bool IsPlaying => keys.Any() || actuationDictionary.Any();

    internal void Generate(float sampleRate, VstAudioBuffer[] outChannels)
    {
      int length = outChannels[0].SampleCount;
      for (int i = 0; i < length; i++)
      {
        foreach (var key in keys)
        {
          if (!actuationDictionary.TryGetValue(key, out float keyActuation))
          {
            keyActuation = 0;
          }

          if (keyActuation < 1)
          {
            if (parameters.AttackMgr.CurrentValue == 0)
            {
              actuationDictionary[key] = 1;
            }
            else
            {
              actuationDictionary[key] = (float)Math.Min(keyActuation + 1.0 / parameters.AttackMgr.CurrentValue / sampleRate, 1.0);
            }
          }
        }

        foreach (var item in actuationDictionary)
        {
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
            keyActuation = (float)Math.Max(item.Value - 1.0 / parameters.ReleaseMgr.CurrentValue / sampleRate, 0.0);
          }

          if (keyActuation <= 0)
          {
            actuationDictionary.Remove(item.Key);
          }
          else
          {
            actuationDictionary[item.Key] = keyActuation;
          }
        }

        var notes = GeneratorList.List.Where(g => parameters.GenMgrs[g.Index].CurrentValue == 1)
                                      .Select(o => o.Factor).ToList();

        double time = (sampleIndex + i) / sampleRate;
        foreach (var channel in outChannels)
        {
          channel[i] = (float)actuationDictionary.Sum(actuation =>
            actuation.Value * ((parameters.FmModMgr.CurrentValue == 1) ? notes.Aggregate(1.0, (a, note) => a * 1.5 * Wave(parameters.SawMgr.CurrentValue, time * noteFrequencies[actuation.Key] * note)) :
                                                       notes.Sum(note => Wave(parameters.SawMgr.CurrentValue, time * noteFrequencies[actuation.Key] * note))));
        }
      }

      sampleIndex += outChannels.FirstOrDefault()?.SampleCount ?? 0;
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
  }
}