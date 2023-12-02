using Jacobi.Vst.Plugin.Framework;
using Jacobi.Vst.Samples.MidiNoteSampler.Data;
using Jacobi.Vst.Samples.MidiNoteSampler.Smx;
using Jacobi.Vst.Samples.MidiNoteSampler.UI;
using Jacobi.Vst.Samples.MidiNoteSampler.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
      //InitializeComponent();

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
      //checkedListBox1.Items.Clear();
      //foreach (var generator in GeneratorList.List)
      //{
      //  checkedListBox1.Items.Insert(generator.Index, generator);
      //}

      //if (parameters == null)
      //  return false;

      //this.parameters = parameters;

      //BindParameter(GetParameterByName(parameters, SmxParameters.SawParameterName), trackBar1);


      //foreach (var generator in GeneratorList.List)
      //{
      //  this.checkedListBox1.SetItemChecked(generator.Index, GetParameterByName(parameters, generator.ParameterName).CurrentValue == 1.0);
      //}

      //BindParameter(parameters[1], label2, trackBar2, label6);
      //BindParameter(parameters[2], label3, trackBar3, label7);
      //BindParameter(parameters[3], label4, trackBar4, label8);

      return true;
    }

    //private static VstParameterManager GetParameterByName(IList<VstParameterManager> parameters, string name)
    //{
    //  return parameters.FirstOrDefault(p => p.ParameterInfo.Name == name);
    //}

    //private void BindParameter(VstParameterManager paramMgr, TrackBar trackBar)
    //{
    //  //label.Text = paramMgr.ParameterInfo.Name;
    //  //shortLabel.Te6xt = paramMgr.ParameterInfo.ShortLabel;

    //  var factor = InitTrackBar(trackBar, paramMgr.ParameterInfo);
    //  var paramState = new ParameterControlStateLinear(paramMgr, factor);

    //  this.FmModulation.Checked = GetParameterByName(parameters, SmxParameters.FmModParameterName).CurrentValue == 1;

    //  // use databinding for VstParameter/Manager changed notifications.
    //  trackBar.DataBindings.Add("Value", paramState, nameof(ParameterControlStateLinear.Value), false, DataSourceUpdateMode.OnPropertyChanged);
    //  //trackBar.ValueChanged += TrackBar_ValueChanged;
    //  trackBar.Tag = paramState;
    //}

    private float InitTrackBar(TrackBar trackBar, VstParameterInfo parameterInfo)
    {
      // A multiplication factor to convert floats (0.0-1.0) 
      // to an integer range for the TrackBar to work with.
      float factor = trackBar.Maximum - trackBar.Minimum;

      //if (parameterInfo.IsSwitch)
      //{
      //  trackBar.Minimum = 0;
      //  trackBar.Maximum = 1;
      //  trackBar.LargeChange = 1;
      //  trackBar.SmallChange = 1;
      //  return factor;
      //}

      //if (parameterInfo.IsStepIntegerValid)
      //{
      //  trackBar.LargeChange = parameterInfo.LargeStepInteger;
      //  trackBar.SmallChange = parameterInfo.StepInteger;
      //}
      //else if (parameterInfo.IsStepFloatValid)
      //{
      //  factor = 1 / parameterInfo.StepFloat;
      //  trackBar.LargeChange = (int)(parameterInfo.LargeStepFloat * factor);
      //  trackBar.SmallChange = (int)(parameterInfo.StepFloat * factor);
      //}

      //if (parameterInfo.IsMinMaxIntegerValid)
      //{
      //  trackBar.Minimum = (int)(parameterInfo.MinInteger * factor);
      //  trackBar.Maximum = (int)(parameterInfo.MaxInteger * factor);
      //}
      //else
      //{
      //  trackBar.Minimum = 0;
      //  trackBar.Maximum = (int)factor;
      //}

      return factor;
    }

    //private void TrackBar_ValueChanged(object? sender, System.EventArgs e)
    //{
    //  var trackBar = (TrackBar?)sender;
    //  var paramState = (ParameterControlState?)trackBar?.Tag;

    //  if (trackBar != null &&
    //      paramState?.ParameterManager.ActiveParameter != null)
    //  {
    //    paramState.ParameterManager.ActiveParameter.Value =
    //        trackBar.Value / paramState.ValueFactor;
    //  }
    //}

    internal void ProcessIdle()
    {
      // TODO: short idle processing here
    }

    ///// <summary>
    ///// This class converts the parameter value range to a compatible (integer) TrackBar value range.
    ///// </summary>
    //private sealed class ParameterControlStateLinear
    //{
    //  public ParameterControlStateLinear(VstParameterManager parameterManager, float valueFactor)
    //  {
    //    ParameterManager = parameterManager;
    //    ValueFactor = valueFactor;
    //  }

    //  public VstParameterManager ParameterManager { get; }
    //  public float ValueFactor { get; }

    //  public int Value
    //  {
    //    get
    //    {
    //      return (int)(ParameterManager.CurrentValue * ValueFactor);
    //    }

    //    set
    //    {
    //      if (ParameterManager.ActiveParameter != null)
    //      {
    //        ParameterManager.ActiveParameter.Value = value / ValueFactor;
    //      }
    //    }
    //  }
    //}

    //private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
    //{
    //}

    //private void checkedListBox1_SelectedIndexChanged(object sender, System.EventArgs e)
    //{
    //  foreach (var gen in GeneratorList.List)
    //  {
    //    VstParameter active;
    //    if ((active = GetParameterByName(parameters, gen.ParameterName).ActiveParameter) != null)
    //    {
    //      active.Value = this.checkedListBox1.CheckedItems.Contains(gen) ? 1 : 0;
    //    }
    //  }
    //}

    ////private void trackBar1_ValueChanged(object sender, System.EventArgs e)
    ////{
    ////  SmxParameters.SawMgr.CurrentValue = Math.Pow((double)this.trackBar1.Value / this.trackBar1.Maximum, 1.0 / 3.0);
    ////}

    //private void attack_ValueChanged(object sender, EventArgs e)
    //{
    //  SmxParameters.Attack = (double)this.attack.Value / this.attack.Maximum;
    //}

    //private void release_ValueChanged(object sender, EventArgs e)
    //{
    //  SmxParameters.Release = (double)this.release.Value / this.release.Maximum;
    //}

    //private void FmModulation_CheckedChanged(object sender, EventArgs e)
    //{
    //  SaveSetByName(parameters, SmxParameters.FmModParameterName, this.FmModulation.Checked ? 1 : 0);
    //}

    //private void SaveSetByName(IList<VstParameterManager> parameters, string name, float v)
    //{
    //  VstParameter active;
    //  if ((active = GetParameterByName(parameters, name).ActiveParameter) != null)
    //  {
    //    active.Value = v;
    //  }
    //}

    //private void pow_ValueChanged(object sender, EventArgs e)
    //{
    //  //var normalized = this.pow.Value - this.pow.Maximum / 2.0;
    //  //var mult = Math.Pow(16, 1.0 / this.pow.Maximum * 2.0);
    //  //SmxParameters.Pow = Math.Pow(mult, normalized);
    //  //Debug.WriteLine(SmxParameters.Pow);
    //}
  }
}
