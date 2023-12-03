using Jacobi.Vst.Plugin.Framework;
using Smx.Vst.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.Vst.ViewModels
{
  internal class PluginViewModel
  {
    public BindingList<KnobViewModel> GeneratorKnobs { get; set; } = new BindingList<KnobViewModel>();
    public BindingList<KnobViewModel> GeneralKnobs { get; set; } = new BindingList<KnobViewModel>();

    internal void InitializeParameters(IList<VstParameterManager> parameters)
    {
      foreach (var item in parameters)
      {
        if (!GeneratorList.List.Any(g => g.ParameterName == item.ParameterInfo.Name) && !GeneratorList.List.Any(g => g.PhaseParameterName == item.ParameterInfo.Name))
        {
          GeneralKnobs.Add(new KnobViewModel(item));
        }
      }

      foreach (var item in GeneratorList.List) {
        var genKnob = new GeneratorViewModel(parameters.First(p => p.ParameterInfo.Name == item.ParameterName));
        genKnob.SetPhaseParameter(parameters.First(p => p.ParameterInfo.Name == item.PhaseParameterName));
        GeneratorKnobs.Add(genKnob);
      }
    }
  }
}
