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

    public bool IsPlaying => this.keys.Any();

    internal void Generate(float sampleRate, VstAudioBuffer[] outChannels)
    {
      foreach (var channel in outChannels)
      {
        for (int i = 0; i < channel.SampleCount; i++)
        {
          double time = (sampleIndex + i) / sampleRate * 60.0;

          channel[i] = this.keys.Sum(x => (float)Math.Sin(time * noteFrequencies[x] / Pi2));
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
  }
}
