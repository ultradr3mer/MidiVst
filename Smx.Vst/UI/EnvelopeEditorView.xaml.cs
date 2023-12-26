using Smx.Vst.ViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Smx.Vst.UI
{
  /// <summary>
  /// Interaction logic for EnvelopeEditorView.xaml
  /// </summary>
  public partial class EnvelopeEditorView : UserControl
  {
    private EnvelopeViewModel viewModel;

    public EnvelopeEditorView()
    {
      InitializeComponent();

      this.DataContextChanged += FilterEditorView_DataContextChanged;
    }

    private void FilterEditorView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      this.viewModel = (EnvelopeViewModel)e.NewValue;
      this.viewModel.EnvelopeLinkViewModels.ListChanged += EnvelopeLinkViewModels_ListChanged; ;
      ExpandCollapse();
    }

    private void EnvelopeLinkViewModels_ListChanged(object? sender, System.ComponentModel.ListChangedEventArgs e)
    {
      this.ExpandCollapse();
    }

    private void ExpandCollapse()
    {
      var hasLinks = this.viewModel.EnvelopeLinkViewModels.Any();
      this.Height = hasLinks ? 225 : 40;
    }
  }
}
