using Smx.Vst.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Smx.Vst.UI
{
  /// <summary>
  /// Interaction logic for FilterEditorView.xaml
  /// </summary>
  public partial class FilterEditorView : UserControl
  {
    private FilterViewModel viewModel;

    public FilterEditorView()
    {
      InitializeComponent();

      this.DataContextChanged += FilterEditorView_DataContextChanged;
    }

    private void FilterEditorView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      this.viewModel = (FilterViewModel)e.NewValue;
      this.viewModel.PropertyChanged += ViewModel_PropertyChanged;
      ExpandCollapse();
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(FilterViewModel.SelectedMode))
      {
        this.ExpandCollapse();
      }
    }

    private void ExpandCollapse()
    {
      var mode = this.viewModel.SelectedMode?.Mode;
      this.Height = (mode == FilterMode.None) ? 40 : 130;
    }
  }
}
