using Jacobi.Vst.Plugin.Framework;
using Smx.Vst.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.Vst.Data
{
  internal class GenParameterContainer : ParameterContainer
  {
    public GenParameterContainer(VstParameterCategory paramCategory) : base(paramCategory)
    {
      foreach (var gen in GeneratorList.List)
      {
        CreateSwitch(name: gen.ParameterName,
                    label: gen.ParameterLabel,
                    shortLabel: gen.DisplayName,
                    defaultValue: gen.Factor == 1.0f);
      }
    }
  }
}