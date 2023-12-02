using Jacobi.Vst.Plugin.Framework;
using Jacobi.Vst.Samples.MidiNoteSampler.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jacobi.Vst.Samples.MidiNoteSampler.ViewModels
{
  internal class PluginViewModel
  {
    public BindingList<KnobViewModel> GeneratorKnobs { get; set; } = new BindingList<KnobViewModel>();
    public BindingList<KnobViewModel> GeneralKnobs { get; set; } = new BindingList<KnobViewModel>();

    internal void InitializeParameters(IList<VstParameterManager> parameters)
    {
      foreach (var item in parameters)
      {
        if (GeneratorList.List.Any(g => g.ParameterName == item.ParameterInfo.Name))
        {
          GeneratorKnobs.Add(new KnobViewModel(item));
        }
        else
        {
          GeneralKnobs.Add(new KnobViewModel(item));
        }
      }
    }
  }
}
