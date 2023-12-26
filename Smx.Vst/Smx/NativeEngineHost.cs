using Jacobi.Vst.Core;
using Smx.Vst.Data;
using Smx.Vst.Parameter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VstNetAudioPlugin;

namespace Smx.Vst.Smx
{
  internal class NativeEngineHost
  {
    private readonly SmxParameters parameters;

    private EngineParameter engineParameter;
    private HashSet<short> keys = new HashSet<short>();

    private AudioEngine nativeEngine;
    private long processedSamples = 0;
    private long runtimeTicks = 0;
    private Stopwatch sw = new Stopwatch();

    public NativeEngineHost(PluginParameters parameters)
    {
      this.parameters = parameters.SmxParameters;

      engineParameter = this.parameters.GeneralParameter.EngineParameter;
      nativeEngine = new AudioEngine(engineParameter);

      foreach (var container in this.parameters.FilterManagerAry)
      {
        engineParameter.ActiveFilter.Add(new Filter(container.FilterParamer));
      }
    }

    public bool IsPlaying => this.keys.Any() || this.engineParameter.ActiveKeys.Any();

    internal void Generate(float sampleRate, VstAudioBuffer[] outChannels)
    {
      sw.Restart();

      engineParameter.SampleRate = sampleRate;
      engineParameter.ActiveGenerators = GeneratorList.List.Where(g => parameters.GenParameterContainer[g.Index].CurrentValue == 1)
                                  .OfType<GeneratorParameter>()
                                  .ToList();
      engineParameter.MinGenFactor = engineParameter.ActiveGenerators.Any() ?
         (float)engineParameter.ActiveGenerators.Min(g => g.Factor)
         : 0.0f;

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
  }
}