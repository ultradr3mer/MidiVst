using Jacobi.Vst.Plugin.Framework;
using Smx.Vst.Util;
using System;

namespace Smx.Vst.Smx
{
  internal class Filter
  {
    public class FilterParameter
    {
      public FilterParameter(VstParameterManager modeMgrs,
                             VstParameterManager cutoffMgr,
                             VstParameterManager dryWetMgr)
      {
        ModeMgrs = modeMgrs;
        CutoffMgr = cutoffMgr;
        DryWetMgr = dryWetMgr;
      }

      public VstParameterManager CutoffMgr { get; private set; }
      public VstParameterManager DryWetMgr { get; private set; }
      public VstParameterManager ModeMgrs { get; private set; }
    }

    private double buf0;

    private double buf1;
    private readonly FilterParameter parameters;

    //private double cutoff;

    //private double feedbackMount;

    //private double dryWet;

    //private Mode mode;

    public Filter(FilterParameter parameters)
    {
      this.parameters = parameters;
    }

    public enum Mode
    {
      None,
      Lowpass,
      Highpass,
      Bandpass
    }

    public double Process(double inputValue)
    {
      return AudioMath.Mix(inputValue, ProcessInternal(inputValue), parameters.DryWetMgr.CurrentValue);
    }

    private double ProcessInternal(double inputValue)
    {
      var cutoff = Math.Pow(parameters.CutoffMgr.CurrentValue, 4.0);
      //buf0 += cutoff * (inputValue - buf0 + feedbackAmount * (buf0 - buf1));
      buf0 += cutoff * (inputValue - buf0);
      buf1 += cutoff * (buf0 - buf1);
      switch ((Mode)parameters.ModeMgrs.CurrentValue)
      {
        case Mode.Lowpass:
          return buf1;

        case Mode.Highpass:
          return inputValue - buf0;

        case Mode.Bandpass:
          return buf0 - buf1;

        default:
          return 0.0;
      }
    }
  }
}