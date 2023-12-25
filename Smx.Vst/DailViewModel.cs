using System.ComponentModel;

namespace Smx.Vst
{
  internal class DailViewModel : INotifyPropertyChanged
  {
    public DailViewModel()
    {
    }

    public bool IsPressed { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;
  }
}