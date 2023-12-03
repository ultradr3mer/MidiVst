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
  internal class GeneratorViewModel : KnobViewModel
  {
    private KnobViewModel phaseViewModel;

    public GeneratorViewModel(VstParameterManager item) : base(item)
    {
      this.Label = this.Label.Substring("Generator ".Length);
      this.PropertyChanged += GeneratorViewModel_PropertyChanged;
    }

    private void GeneratorViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(this.PhaseValue))
      {
        this.phaseViewModel.Value = this.PhaseValue;
      }
    }

    public double PhaseValue { get; set; }

    public void SetPhaseParameter(VstParameterManager phase)
    {
      var vm = new KnobViewModel(phase);
      this.phaseViewModel = vm;
      this.phaseViewModel.PropertyChanged += PhaseViewModel_PropertyChanged;
    }

    private void PhaseViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(KnobViewModel.Value))
      {
        this.PhaseValue = this.phaseViewModel.Value;
      }
    }
  }
}
