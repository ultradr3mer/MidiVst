using Smx.Vst.Util;
using System;
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
      this.ShortLabel = manager.ParameterInfo.ShortLabel;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsPressed { get; set; }

    public double NormalizedValue
    {
      get
      {
        var targetScale = this.manager.Max - this.manager.Min;
        return (Value - this.manager.Min) / targetScale;
      }
      set
      {
        var targetScale = this.manager.Max - this.manager.Min;
        this.Value = value * targetScale + this.manager.Min;
        if (manager.IsInteger || manager.IsSwitch)
          this.Value = (int)this.Value;
      }
    }

    public string ShortLabel { get; }
    public double Value { get; set; }
    public string? ValueString { get; set; }

    private void DailViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(Value))
      {
        ValueString = Value.ToString("0.00");
        manager.CurrentValue = (float)this.Value;
        this?.PropertyChanged(this, new PropertyChangedEventArgs(nameof(NormalizedValue)));
      }
    }
  }
}