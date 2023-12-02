using Jacobi.Vst.Plugin.Framework;
using Smx.Vst.UI;
using Smx.Vst.ViewModels;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace VstNetAudioPlugin.UI
{
  public partial class PluginEditorView : UserControl
  {
    private IList<VstParameterManager> parameters;
    private PluginViewModel viewModel;

    public PluginEditorView()
    {
      ElementHost host = new ElementHost();
      host.Dock = DockStyle.Fill;

      this.viewModel = new PluginViewModel();
      var view = new InnerPluginView() { DataContext = viewModel };
      host.Child = view;

      this.Height = 400;
      this.Width = 800;
      this.Controls.Add(host);
    }

    internal bool InitializeParameters(IList<VstParameterManager> parameters)
    {
      this.viewModel.InitializeParameters(parameters);

      return true;
    }

    internal void ProcessIdle()
    {
      // TODO: short idle processing here
    }
  }
}