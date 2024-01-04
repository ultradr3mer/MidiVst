using System.ComponentModel;
using VstNetAudioPlugin;

namespace Smx.Vst.ViewModels
{
  internal class PluginViewModel
  {
    public BindingList<KnobViewModel> GeneratorVms { get; set; } = new BindingList<KnobViewModel>();
    public BindingList<DailViewModel> GeneralVms { get; set; } = new BindingList<DailViewModel>();
    public BindingList<FilterViewModel> FilterVms { get; set; } = new BindingList<FilterViewModel>();
    public BindingList<EnvelopeLinkViewModel> UnasignedEnvelopeLinkVms { get; set; } = new BindingList<EnvelopeLinkViewModel>();
    public BindingList<EnvelopeViewModel> EnvelopeVms { get; set; } = new BindingList<EnvelopeViewModel>();

    internal void InitializeParameters(PluginParameters parameters)
    {
      foreach (var item in parameters.SmxParameters.GenParameterContainer)
      {
        GeneratorVms.Add(new KnobViewModel(item));
      }

      foreach (var item in parameters.SmxParameters.GeneralParameter)
      {
        GeneralVms.Add(new DailViewModel(item));
      }

      int filterNr = 0;
      foreach (var item in parameters.SmxParameters.FilterManagerAry)
      {
        FilterVms.Add(new FilterViewModel(item, filterNr++));
      }

      int envNr = 0;
      foreach (var item in parameters.SmxParameters.EnvelopeManagerAry)
      {
        EnvelopeVms.Add(new EnvelopeViewModel(item, parameters.SmxParameters.ModParaManager, UnasignedEnvelopeLinkVms, envNr++));
      }

      int linkNr = 0;
      foreach (var item in parameters.SmxParameters.EnvelopeLinkManagerAry)
      {
        var vm = new EnvelopeLinkViewModel(item, linkNr++);
        int linkEnvNr = item.Parameter.EnvelopeNr;

        if (linkEnvNr == -1) {
          UnasignedEnvelopeLinkVms.Add(vm);
          continue;
        }

        int targetId = item.Parameter.EnvelopeNr;
        var paraMgr = parameters.SmxParameters.ModParaManager[targetId];
        var envVm = EnvelopeVms[linkEnvNr];
        envVm.Link(vm, targetId, paraMgr.ParameterInfo.Label, paraMgr.ParameterInfo.ShortLabel);
      }
    }
  }
}