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
    public BindingList<FilterViewModel> FilterKnobs { get; set; } = new BindingList<FilterViewModel>();

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

      int filterNr = 0;
      foreach (var item in parameters.SmxParameters.FilterManagerAry)
      {
        FilterKnobs.Add(new FilterViewModel(item, filterNr++));
      }
    }
  }
}