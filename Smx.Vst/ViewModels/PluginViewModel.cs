using Jacobi.Vst.Plugin.Framework;
using Smx.Vst.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using VstNetAudioPlugin;

namespace Smx.Vst.ViewModels
{
  internal class PluginViewModel
  {
    public BindingList<KnobViewModel> GeneralKnobs { get; set; } = new BindingList<KnobViewModel>();
    public BindingList<KnobViewModel> GeneratorKnobs { get; set; } = new BindingList<KnobViewModel>();

    internal void InitializeParameters(PluginParameters parameters)
    {
      foreach (var item in parameters.SmxParameters.GeneralParameter)
      {
        GeneralKnobs.Add(new KnobViewModel(item));
      }

      foreach (var item in parameters.SmxParameters.GenParameterContainer)
      {
        GeneratorKnobs.Add(new KnobViewModel(item));
      }
    }
  }
}