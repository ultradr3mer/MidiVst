using Jacobi.Vst.Core;
using Jacobi.Vst.Plugin.Framework;
using Jacobi.Vst.Plugin.Framework.Plugin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VstNetAudioPlugin.Dsp;

namespace Jacobi.Vst.Samples.MidiNoteSampler
{
  internal class Generator
  {
    private const double Pi2 = Math.PI * 2.0;
    private const double a4Index = 69;
    private const double a4Frequency = 440.0;
    private static readonly double multiplyer = (double)Math.Pow(2, 1.0 / 12.0);
    private static readonly Dictionary<int, float> noteFrequencies = Enumerable.Range(0, 127).ToDictionary(x => x, x => (float)(a4Frequency * Math.Pow(multiplyer, x - 69)));

    public long sampleIndex = 0;

    private HashSet<byte> keys = new HashSet<byte>();
    private Dictionary<byte, float> actuationDictionary = new Dictionary<byte, float>();

    public bool IsPlaying => keys.Any() || this.actuationDictionary.Any();

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
            if (DelayParameters.Attack == 0)
            {
              actuationDictionary[key] = 1;
            }
            else
            {
              actuationDictionary[key] = (float)Math.Min(keyActuation + 1.0 / DelayParameters.Attack / sampleRate, 1.0);
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
          if (DelayParameters.Release == 0)
          {
            keyActuation = 0;
          }
          else
          {
            keyActuation = (float)Math.Max(item.Value - 1.0 / DelayParameters.Release / sampleRate, 0.0);
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

        double time = (sampleIndex + i) / sampleRate;
        foreach (var channel in outChannels)
        {
          channel[i] = (float)this.actuationDictionary.Sum(actuation => 
            actuation.Value * (DelayParameters.FmMod ? DelayParameters.Notes.Aggregate<int, double>(1.0, (a, o) => a * 1.5 * Wave(DelayParameters.Saw, time * noteFrequencies[actuation.Key] * o / 12.0)) :
                                                       DelayParameters.Notes.Sum(o => Wave(DelayParameters.Saw, time * noteFrequencies[actuation.Key] * o / 12.0))));
        }
      }

      sampleIndex += outChannels.FirstOrDefault()?.SampleCount ?? 0;
    }

    static double Wave(double saw, double t)
    {
      t = t % 1;
      double segment13_length = (1.0 - saw) / 4.0;
      double segment2_length = (1.0 + saw) / 2.0;

      if (t < segment13_length)
      {
        t = t / (1 - saw);
      }
      else if (t < (segment13_length + segment2_length))
      {
        t = (t - segment13_length) / (1.0 + saw) + (1.0 / 4.0);
      }
      else
      {
        t = 1 - (1 - t) / (1 - saw);
      }

      double sin_wave = Math.Sin(2 * Math.PI * t);

      double saw_wave = t < (1.0 / 4.0) ? t * 4.0 :
        t < (3.0 / 4.0) ? 1 - (t - (1.0 / 4.0)) * 4.0 :
        (-4.0 + (4.0 * t));

      double combined = (1 - saw) * sin_wave + saw * saw_wave;

      return Math.Pow(Math.Abs(combined),0.5)*Math.Sign(combined);
    }

    internal void ProcessNoteOffEvent(byte v)
    {
      keys.Remove(v);
    }

    internal void ProcessNoteOnEvent(byte v)
    {
      keys.Add(v);
    }
  }
}
