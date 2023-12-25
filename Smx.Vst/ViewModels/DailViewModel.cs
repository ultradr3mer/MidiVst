using Smx.Vst.Util;
using System.ComponentModel;

namespace Smx.Vst.ViewModels
{
  internal class DailViewModel : INotifyPropertyChanged
  {
    private SmxParameterManager manager;

    public DailViewModel(SmxParameterManager manager)
    {
      this.manager = manager;
      PropertyChanged += DailViewModel_PropertyChanged;

      this.Value = manager.CurrentValue;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsPressed { get; set; }

    public double Value { get; set; }

    public string? ValueString { get; set; }

    private void DailViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(Value))
      {
        ValueString = Value.ToString("0.00");
        manager.CurrentValue = (float)this.Value;
      }
    }
  }
}