using Jacobi.Vst.Core;
using Smx.Vst.Data;
using Smx.Vst.Parameter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using VstNetAudioPlugin;

namespace Smx.Vst.Smx
{
  internal class NativeEngineHost
  {
    private readonly SmxParameters parameters;

    private EngineParameter engineParameter;
    private Dictionary<short,int> keys = new Dictionary<short,int>();

    private AudioEngine nativeEngine;
    private long processedSamples = 0;
    private long runtimeTicks = 0;
    private Stopwatch sw = new Stopwatch();
    private int keyNumber;

    public NativeEngineHost(PluginParameters parameters)
    {
      this.parameters = parameters.SmxParameters;

      engineParameter = this.parameters.GeneralParameter.EngineParameter;
      engineParameter.EnvelopeLinks = this.parameters.EnvelopeLinkManagerAry.Select(o => o.Parameter).ToList();
      engineParameter.ModParameter = this.parameters.ModParaManager.Select(o => o.ModPara).ToList();
      nativeEngine = new AudioEngine(engineParameter);
      
      foreach (var container in this.parameters.FilterManagerAry)
      {
        engineParameter.ActiveFilter.Add(container.FilterParamer);
      }

      engineParameter.ActiveEnvelopes = this.parameters.EnvelopeManagerAry.Select(m => m.Parameter).ToList();
    }

    public bool IsPlaying => this.keys.Any() || this.nativeEngine.GetHasActiveKeys();

    internal void Generate(float sampleRate, VstAudioBuffer[] outChannels)
    {
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
    }

    internal void ProcessNoteOffEvent(byte v)
    {
      keys.Remove(v);
    }

    internal void ProcessNoteOnEvent(byte v)
    {
      unchecked
      {
        keys[v] = keyNumber++;
      }
    }
  }
}