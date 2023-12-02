using Jacobi.Vst.Plugin.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Smx.Vst.ViewModels
{
  internal class KnobViewModel : INotifyPropertyChanged
  {
    private VstParameterManager mgr;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Label { get; private set; }
    public string ValueString { get; private set; }
    public double Value { get; set; }
    public Visibility CheckboxVisibility { get; }
    public Visibility SliderVisibility { get; }
    public bool IsValue { get => Value == 1; set => Value = value ? 1 : 0; }

    public KnobViewModel(VstParameterManager item)
    {
      this.mgr = item;
      this.Label = item.ParameterInfo.Label;
      this.CheckboxVisibility = item.ParameterInfo.IsSwitch ? Visibility.Visible : Visibility.Hidden;
      this.SliderVisibility = item.ParameterInfo.IsSwitch ? Visibility.Hidden : Visibility.Visible;
      item.PropertyChanged += Item_PropertyChanged;
      this.PropertyChanged += KnobViewModel_PropertyChanged;
      this.Value = item.CurrentValue;
    }

    private void KnobViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(Value))
      {
        if(mgr.ActiveParameter != null) { 
          mgr.ActiveParameter.Value = (float)this.Value;
        }
        this.ValueString = this.Value.ToString();
      }
    }

    private void Item_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      this.Value = mgr.CurrentValue;
    }
  }
}
