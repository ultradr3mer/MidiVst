using Jacobi.Vst.Plugin.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.Vst.Data
{
  internal class FilterParameterContainer : ParameterContainer
  {
    public FilterParameterContainer(VstParameterCategory paramCategory, int i) : base(paramCategory)
    {
      this.FilterParamer = new FilterParameter();

      CreateInteger(name: "FltrMd" + i,
                    label: "Filter Mode " + i,
                    shortLabel: "Fil.Md." + i,
                    min: (int)FilterMode.LowpassMix,
                    max: (int)FilterMode.BandpassAdd,
                    defaultValue: (int)FilterMode.None,
                    updateAction: v => this.FilterParamer.Mode = v);
      CreateFloat(name: "FltrCt" + i,
                  label: "Filter Cutoff " + i,
                  shortLabel: "Fil.Ct." + i,
                  defaultValue: 0.5f,
                  updateAction: v => this.FilterParamer.Cutoff = v);
      CreateFloat(name: "FltrWA" + i,
                  label: "Filter Wet Amt" + i,
                  shortLabel: "Fl.W.A." + i,
                  defaultValue: 1.0f,
                  updateAction: v => this.FilterParamer.WetAmt = v);
      CreateFloat(name: "FltrRe" + i,
                  label: "Filter Resonance " + i,
                  shortLabel: "Fl.F.b." + i,
                  updateAction: v => this.FilterParamer.Resonance = v);
    }

    public FilterParameter FilterParamer { get; }
  }
}