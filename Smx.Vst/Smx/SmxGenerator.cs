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


    private AudioEngine nativeEngine;
    private EngineParameter nativeParameter;
    private long processedSamples = 0;
    private long runtimeTicks = 0;
    private Stopwatch sw = new Stopwatch();
    //private Dictionary<short, List<Filter>> channelFilterDict = new Dictionary<short, List<Filter>>();

    public SmxGenerator(PluginParameters parameters)
    {
      this.parameters = parameters.SmxParameters;


      nativeParameter = new EngineParameter();
      nativeEngine = new AudioEngine(nativeParameter);
      
      foreach (var filterParameter in this.parameters.FilterParameterAry)
      {
        nativeParameter.ActiveFilter.Add(new Filter(filterParameter));
      }

    }

    public bool IsPlaying => this.keys.Any() || this.nativeParameter.ActiveKeys.Any();

    internal void Generate(float sampleRate, VstAudioBuffer[] outChannels)
    {
      sw.Restart();
      nativeParameter.FilterCount = (int)parameters.FilterCountMgr.CurrentValue;
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

      //nativeParameter.ActiveFilter = parameters.FilterParameterAry
      //  .Take((int)parameters.FilterCountMgr.CurrentValue)
      //  .Select(container => new FilterParameter()
      //  {
      //    Cutoff = container.CutoffMgr.CurrentValue,
      //    Resonance = container.ResonanceMgr.CurrentValue,
      //    WetAmt = container.DryWetMgr.CurrentValue,
      //    Mode = (int)container.ModeMgrs.CurrentValue
      //  }).ToList();

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