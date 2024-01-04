using Smx.Vst.Parameter;
using Smx.Vst.Util;
using System;

namespace Smx.Vst.ViewModels
{
  internal class EnvelopeLinkViewModel
  {
    private EnvelopeLinkParameterContainer item;
    private Action<EnvelopeLinkViewModel> unlinkCallback;

    public EnvelopeLinkParameter Parameter { get => this.item.Parameter;  }

    public EnvelopeLinkViewModel(EnvelopeLinkParameterContainer item)
    {
      this.item = item;
      this.AmountVm = new DailViewModel(item.AmmountMgr);
      this.RemoveLinkCommand = new DelegateCommand(this.RemoveLinkCommandExecute);
    }

    public DailViewModel AmountVm { get; }
    public string? LabelLong { get; private set; }
    public string? LabelShort { get; private set; }
    public DelegateCommand RemoveLinkCommand { get; }

    public void Link(int envelopeId, int targetId, string? labelLong, string? labelShort, Action<EnvelopeLinkViewModel> unlinkCallback)
    {
      this.item.EnvelopeMgr.CurrentValue = envelopeId;
      this.item.TargetIdMgr.CurrentValue = targetId;
      this.LabelLong = labelLong;
      this.LabelShort = labelShort + ":";

      this.unlinkCallback = unlinkCallback;
    }

    private void RemoveLinkCommandExecute(object obj)
    {
      this.item.EnvelopeMgr.CurrentValue = -1;
      this.item.TargetIdMgr.CurrentValue = -1;
      this.AmountVm.Value = 0.5;
      this.LabelLong = "";
      this.LabelShort = "";

      this.unlinkCallback?.Invoke(this);
    }
  }
}