using Microsoft.Xaml.Behaviors.Core;
using Smx.Vst.Parameter;
using Smx.Vst.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
      var selectedLink = this.SelectedEnvelopeLink;
      vm.Link(this.i, selectedLink.TargetId, selectedLink.LabelLong, selectedLink.LabelShort);
      this.EnvelopeLinkViewModels.Add(vm);
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