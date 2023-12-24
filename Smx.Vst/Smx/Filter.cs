using Jacobi.Vst.Plugin.Framework;
using Smx.Vst.Util;
using System;

namespace Smx.Vst.Smx
{
  internal class Filter
  {
    private readonly FilterParameter parameters;

    private double buf0;

    private double buf1;

    public Filter(FilterParameter parameters)
    {
      this.parameters = parameters;
    }

    public enum Mode
    {
      None,
      LowpassMix,
      HighpassMix,
      BandpassMix,
      LowpassAdd,
      HighpassAdd,
      BandpassAdd,
    }

    public double Process(double inputValue)
    {
      var mode = (Mode)parameters.ModeMgrs.CurrentValue;
      var wet = ProcessInternal(inputValue, mode);
      var wetAmt = parameters.DryWetMgr.CurrentValue;
      return mode >= Mode.LowpassAdd ? (inputValue + wet * wetAmt)
        : mode >= Mode.LowpassMix ? AudioMath.Mix(inputValue, wet, wetAmt)
        : inputValue;
    }

    private double CalcFeedbackAmmount(double cutoff)
    {
      var feedback = (double)parameters.ResonanceMgr.CurrentValue;
      feedback += feedback / (1.0 - AudioMath.Clamp(cutoff, 0.99, 0.01));
      return feedback;
    }

    private double ProcessInternal(double inputValue, Mode mode)
    {
      var cutoff = Math.Pow(parameters.CutoffMgr.CurrentValue, 4.0);
      double feedback = CalcFeedbackAmmount(cutoff);

      buf0 += cutoff * (inputValue - buf0 + feedback * (buf0 - buf1));
      buf1 += cutoff * (buf0 - buf1);
      switch (mode)
      {
        case Mode.LowpassMix:
        case Mode.LowpassAdd:
          return buf1;

        case Mode.HighpassMix:
        case Mode.HighpassAdd:
          return inputValue - buf0;

        case Mode.BandpassMix:
        case Mode.BandpassAdd:
          return buf0 - buf1;

        default: 
          return inputValue;
      }
    }

    public class FilterParameter
    {
      public FilterParameter(VstParameterManager modeMgrs,
                             VstParameterManager cutoffMgr,
                             VstParameterManager dryWetMgr,
                             VstParameterManager resonanceMgr)
      {
        ModeMgrs = modeMgrs;
        CutoffMgr = cutoffMgr;
        DryWetMgr = dryWetMgr;
        ResonanceMgr = resonanceMgr;
      }

      public VstParameterManager CutoffMgr { get; }
      public VstParameterManager DryWetMgr { get; }
      public VstParameterManager ModeMgrs { get; }
      public VstParameterManager ResonanceMgr { get; }
    }
  }
}