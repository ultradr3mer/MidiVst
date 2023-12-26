using Smx.Vst.Parameter;

namespace Smx.Vst.ViewModels
{
  internal class EnvelopeLinkViewModel
  {
    private EnvelopeLinkParameterContainer item;

    public DailViewModel AmountVm { get; }

    private int v;

    public EnvelopeLinkViewModel(EnvelopeLinkParameterContainer item, int v)
    {
      this.item = item;
      this.AmountVm = new DailViewModel(item.AmmountMgr);
      this.v = v;
    }

    public string? LabelLong { get; private set; }
    public string? LabelShort { get; private set; }

    public void Link(int envelopeId, int targetId, string? labelLong, string? labelShort)
    {
      this.item.EnvelopeMgr.CurrentValue = envelopeId;
      this.item.TargetIdMgr.CurrentValue = targetId;
      this.item.AmmountMgr.CurrentValue = 0.5f;
      this.LabelLong = labelLong;
      this.LabelShort = labelShort;
    }
  }
}