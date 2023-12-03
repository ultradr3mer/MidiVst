using Jacobi.Vst.Plugin.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Smx.Vst.ViewModels
{
  internal class KnobViewModel : INotifyPropertyChanged
  {
    private VstParameterManager mgr;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Label { get; protected set; }
    public double Value { get; set; }
    public Visibility CheckboxVisibility { get; }
    public Visibility SliderVisibility { get; }
    public bool IsValue { get => Value == 1; set => Value = value ? 1 : 0; }
    public int MinValue { get; private set; }
    public int MaxValue { get; private set; }
    public bool IsInteger { get; private set; }
    public double StepSize { get; private set; }

    public KnobViewModel(VstParameterManager item)
    {
      this.mgr = item;
      this.Label = item.ParameterInfo.Label;
      this.CheckboxVisibility = item.ParameterInfo.IsSwitch ? Visibility.Visible : Visibility.Hidden;
      this.SliderVisibility = item.ParameterInfo.IsSwitch ? Visibility.Hidden : Visibility.Visible;
      item.PropertyChanged += Item_PropertyChanged;
      this.InitSlider(item.ParameterInfo);
      this.Value = item.CurrentValue;
      this.PropertyChanged += KnobViewModel_PropertyChanged;
      this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(this.Value)));
    }

    private void InitSlider(VstParameterInfo parameterInfo)
    {
      if (parameterInfo.IsSwitch)
      {
        this.MinValue = 0;
        this.MaxValue = 1;
        this.IsInteger = false;
        return;
      }

      if (parameterInfo.IsStepIntegerValid)
      {
        this.IsInteger = true;
        this.StepSize = parameterInfo.StepInteger;
      }
      else if (parameterInfo.IsStepFloatValid)
      {
        this.IsInteger = false;
        this.StepSize = parameterInfo.StepFloat;
      }

      if (parameterInfo.IsMinMaxIntegerValid)
      {
        this.MinValue = parameterInfo.MinInteger;
        this.MaxValue = parameterInfo.MaxInteger;
      }
      else 
      {
        this.MinValue = 0;
        this.MaxValue = 1;
      }
    }

    private void KnobViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(Value))
      {
        if (mgr.ActiveParameter != null)
        {
          mgr.ActiveParameter.Value = (float)this.Value;
        }
      }
    }

    private void Item_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      this.Value = mgr.CurrentValue;
    }
  }
}
