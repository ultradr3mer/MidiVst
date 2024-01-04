using Smx.Vst.Parameter;
using Smx.Vst.Util;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Smx.Vst.ViewModels
{
  internal class EnvelopeViewModel : INotifyPropertyChanged
  {
    public static EnvelopeLinkableItem DefaultItem = new EnvelopeLinkableItem()
    {
      LabelLong = "Link Parameter"
    };

    private readonly int i;
    private readonly EnvelopeParameterContainer item;
    private readonly Dictionary<int, SmxParameterManager> modManagers;
    private readonly BindingList<EnvelopeLinkViewModel> unasignedEnvelopeLinkVms;

    public EnvelopeViewModel(EnvelopeParameterContainer item, Dictionary<int, SmxParameterManager> modManagers, BindingList<EnvelopeLinkViewModel> unasignedEnvelopeLinkVms, int i)
    {
      this.AttackVm = new DailViewModel(item.AttackMgr);
      this.DecayVm = new DailViewModel(item.DecayMgr);
      this.SustainVm = new DailViewModel(item.SustainMgr);
      this.ReleaseVm = new DailViewModel(item.ReleaseMgr);

      this.EnvelopeLinkableItems = new[] { DefaultItem }.Concat(modManagers.Values.Select(m => new EnvelopeLinkableItem()
      {
        ModPara = m.ModPara,
        LabelLong = m.ParameterInfo.Label,
        LabelShort = m.ParameterInfo.ShortLabel,
        TargetId = m.ModParaIndex
      })).ToList();

      this.SelectedEnvelopeLink = DefaultItem;

      this.EnvelopeName = $"Env#{i}";
      this.item = item;
      this.modManagers = modManagers;
      this.unasignedEnvelopeLinkVms = unasignedEnvelopeLinkVms;
      this.AddLinkCommand = new DelegateCommand(AddLinkCommandExecute, AddLinkCommandCanExecute);

      this.PropertyChanged += EnvelopeViewModel_PropertyChanged;

      this.i = i;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand AddLinkCommand { get; }

    public DailViewModel AttackVm { get; }

    public DailViewModel DecayVm { get; }

    public List<EnvelopeLinkableItem> EnvelopeLinkableItems { get; set; } = new List<EnvelopeLinkableItem>();

    public BindingList<EnvelopeLinkViewModel> EnvelopeLinkViewModels { get; set; } = new BindingList<EnvelopeLinkViewModel>();

    public string EnvelopeName { get; }

    public DailViewModel ReleaseVm { get; }

    public EnvelopeLinkableItem SelectedEnvelopeLink { get; set; }

    public DailViewModel SustainVm { get; }

    public void Link(EnvelopeLinkViewModel vm, int targetId, string? labelLong, string? labelShort)
    {
      vm.Link(this.i, targetId, labelLong, labelShort, this.Unlink);
      vm.Parameter.TargetModPara = modManagers[targetId].ModPara;
      this.EnvelopeLinkViewModels.Add(vm);
      this.item.Parameter.Links.Add(vm.Parameter);
    }

    public void Unlink(EnvelopeLinkViewModel vm)
    {
      this.item.Parameter.Links.Remove(vm.Parameter);
      this.EnvelopeLinkViewModels.Remove(vm);
      unasignedEnvelopeLinkVms.Add(vm);
    }

    private bool AddLinkCommandCanExecute(object arg)
    {
      return this.SelectedEnvelopeLink != null &&
        this.SelectedEnvelopeLink != DefaultItem;
    }

    private void AddLinkCommandExecute(object obj)
    {
      if (unasignedEnvelopeLinkVms.Count == 0)
        return;

      var vm = unasignedEnvelopeLinkVms.First();
      unasignedEnvelopeLinkVms.Remove(vm);
      var selectedLink = this.SelectedEnvelopeLink;
      Link(vm, selectedLink.TargetId, selectedLink.LabelLong, selectedLink.LabelShort);
    }

    private void EnvelopeViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(SelectedEnvelopeLink))
      {
        AddLinkCommand.CanExecute(null);
      }
    }

    public class EnvelopeLinkableItem
    {
      public string? LabelLong { get; set; }
      public string? LabelShort { get; set; }
      public ModPara ModPara { get; set; }
      public int TargetId { get; set; }
    }
  }
}