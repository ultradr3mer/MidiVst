using Jacobi.Vst.Plugin.Framework;
using System;
using System.ComponentModel;

namespace Smx.Vst.Util
{
  internal class SmxParameterManager : INotifyPropertyChanged
  {
    private VstParameterManager innerManager;
    private Action<float>? updateAction;

    public SmxParameterManager(VstParameterInfo paramInfo, Action<float>? updateAction = null, float min = 0, float max = 1)
    {
      this.innerManager = paramInfo
                        .Normalize()
                        .ToManager();

      this.updateAction = updateAction;
      Min = min;
      Max = max;

      this.innerManager.PropertyChanged += InnerManager_PropertyChanged;
      this.updateAction?.Invoke(paramInfo.DefaultValue);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public float CurrentValue
    {
      get => this.innerManager.CurrentValue;
      set
      {
        if(value == CurrentValue) 
          return;
        
        if (this.innerManager.ActiveParameter != null)
        {
          this.innerManager.ActiveParameter.Value = (float)value;
        }
      }
    }

    public VstParameterInfo ParameterInfo { get => this.innerManager.ParameterInfo; }

    public float Min { get; }
    public float Max { get; }

    private void InnerManager_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(VstParameterManager.CurrentValue))
      {
        updateAction?.Invoke(this.innerManager.CurrentValue);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentValue)));
      }
    }
  }
}