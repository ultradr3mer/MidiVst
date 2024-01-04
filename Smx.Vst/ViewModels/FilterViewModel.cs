using Smx.Vst.Parameter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Smx.Vst.ViewModels
{
    internal class FilterViewModel : INotifyPropertyChanged
  {
    private FilterParameterContainer parameterContainer;

    public event PropertyChangedEventHandler? PropertyChanged;

    public FilterViewModel(FilterParameterContainer item, int i)
    {
      DailViewModelCut = new DailViewModel(item.FltrCtMgr);
      DailViewModelWetAmt = new DailViewModel(item.FltrWAMgr);
      DailViewModelRes = new DailViewModel(item.FltrReMgr);
      DailViewModelCycles = new DailViewModel(item.FltrCyclesMgr);

      FilterModes = Enum.GetValues(typeof(FilterMode))
                    .OfType<FilterMode>()
                    .Select(m => new FilterModeItem() { Mode = m, Name = m.ToString() })
                    .ToList();

      this.SelectedMode = FilterModes.FirstOrDefault(f => (int)f.Mode == (int)item.FltrMdMgr.CurrentValue);
      this.FilterName = $"Fltr#{i}";
      this.parameterContainer = item;

      this.PropertyChanged += FilterViewModel_PropertyChanged;
    }

    private void FilterViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      if(e.PropertyName == nameof(SelectedMode))
      {
        parameterContainer.FltrMdMgr.CurrentValue = (int)this.SelectedMode.Mode;
      }
    }

    public DailViewModel DailViewModelCut { get; set; }
    public DailViewModel DailViewModelRes { get; set; }
    public DailViewModel DailViewModelWetAmt { get; set; }
    public DailViewModel DailViewModelCycles { get; set; }
    public List<FilterModeItem> FilterModes { get; }
    public FilterModeItem? SelectedMode { get; set; }
    public string FilterName { get; }

    public class FilterModeItem
    {
      public FilterMode Mode { get; set; }
      public string? Name { get; set; }
    }
  }
}