using Jacobi.Vst.Plugin.Framework;
using Smx.Vst.Util;
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
    private SmxParameterManager mgr;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Label { get; protected set; }
    public double Value { get; set; }
    public Visibility CheckboxVisibility { get; }
    public Visibility SliderVisibility { get; }
    public bool IsValue { get => Value == 1; set => Value = value ? 1 : 0; }
    public double MinValue { get; private set; }
    public double MaxValue { get; private set; }
    public bool IsInteger { get; private set; }
    public double StepSize { get; private set; }

    public KnobViewModel(SmxParameterManager item)
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

      this.MaxValue = mgr.Max;
      this.MinValue = mgr.Min;
    }

    private void KnobViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(Value))
      {
        this.mgr.CurrentValue = (float)this.Value;
      }
    }

    private void Item_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(SmxParameterManager.CurrentValue))
      {
        this.Value = mgr.CurrentValue;
      }
    }
  }
}
