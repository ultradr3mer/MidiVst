using Jacobi.Vst.Plugin.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.Vst.Util
{
  internal class SmxParameterManager
  {
    private VstParameterManager innerManager;

    public SmxParameterManager(VstParameterManager innerManager)
    {
      this.innerManager = innerManager;
      this.innerManager.PropertyChanged += InnerManager_PropertyChanged;
    }

    public float CurrentValue { get => this.innerManager.CurrentValue; }

    public event EventHandler<ValueChangedEventArgs> ValueChanged;

    private void InnerManager_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(VstParameterManager.CurrentValue))
      {
        this.ValueChanged?.Invoke(this, new ValueChangedEventArgs(this.innerManager.CurrentValue));
      }
    }

    public class ValueChangedEventArgs : EventArgs
    {
      public ValueChangedEventArgs(float newValue)
      {
        this.NewValue = newValue;
      }

      public float NewValue { get; }
    }
  }
}