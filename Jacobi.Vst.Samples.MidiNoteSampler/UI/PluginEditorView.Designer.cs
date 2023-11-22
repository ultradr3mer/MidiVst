namespace VstNetAudioPlugin.UI
{
  partial class PluginEditorView
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      checkedListBox1 = new System.Windows.Forms.CheckedListBox();
      trackBar1 = new System.Windows.Forms.TrackBar();
      FmModulation = new System.Windows.Forms.CheckBox();
      attack = new System.Windows.Forms.TrackBar();
      release = new System.Windows.Forms.TrackBar();
      trackBar2 = new System.Windows.Forms.TrackBar();
      ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
      ((System.ComponentModel.ISupportInitialize)attack).BeginInit();
      ((System.ComponentModel.ISupportInitialize)release).BeginInit();
      ((System.ComponentModel.ISupportInitialize)trackBar2).BeginInit();
      SuspendLayout();
      // 
      // checkedListBox1
      // 
      checkedListBox1.CheckOnClick = true;
      checkedListBox1.FormattingEnabled = true;
      checkedListBox1.Location = new System.Drawing.Point(0, 0);
      checkedListBox1.Name = "checkedListBox1";
      checkedListBox1.Size = new System.Drawing.Size(180, 220);
      checkedListBox1.TabIndex = 0;
      checkedListBox1.ItemCheck += checkedListBox1_ItemCheck;
      checkedListBox1.SelectedIndexChanged += checkedListBox1_SelectedIndexChanged;
      // 
      // trackBar1
      // 
      trackBar1.Location = new System.Drawing.Point(186, 3);
      trackBar1.Maximum = 100;
      trackBar1.Name = "trackBar1";
      trackBar1.Size = new System.Drawing.Size(161, 45);
      trackBar1.TabIndex = 1;
      trackBar1.ValueChanged += trackBar1_ValueChanged;
      // 
      // FmModulation
      // 
      FmModulation.AutoSize = true;
      FmModulation.Location = new System.Drawing.Point(186, 43);
      FmModulation.Name = "FmModulation";
      FmModulation.Size = new System.Drawing.Size(105, 19);
      FmModulation.TabIndex = 2;
      FmModulation.Text = "FmModulation";
      FmModulation.UseVisualStyleBackColor = true;
      FmModulation.CheckedChanged += FmModulation_CheckedChanged;
      // 
      // attack
      // 
      attack.Location = new System.Drawing.Point(186, 119);
      attack.Maximum = 100;
      attack.Name = "attack";
      attack.Size = new System.Drawing.Size(161, 45);
      attack.TabIndex = 3;
      attack.ValueChanged += attack_ValueChanged;
      // 
      // release
      // 
      release.Location = new System.Drawing.Point(186, 170);
      release.Maximum = 100;
      release.Name = "release";
      release.Size = new System.Drawing.Size(161, 45);
      release.TabIndex = 4;
      release.ValueChanged += release_ValueChanged;
      // 
      // trackBar2
      // 
      trackBar2.Location = new System.Drawing.Point(186, 68);
      trackBar2.Maximum = 100;
      trackBar2.Name = "trackBar2";
      trackBar2.Size = new System.Drawing.Size(161, 45);
      trackBar2.TabIndex = 5;
      trackBar2.Value = 50;
      // 
      // PluginEditorView
      // 
      AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      Controls.Add(trackBar2);
      Controls.Add(release);
      Controls.Add(attack);
      Controls.Add(FmModulation);
      Controls.Add(trackBar1);
      Controls.Add(checkedListBox1);
      Margin = new System.Windows.Forms.Padding(4);
      Name = "PluginEditorView";
      Size = new System.Drawing.Size(351, 232);
      ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
      ((System.ComponentModel.ISupportInitialize)attack).EndInit();
      ((System.ComponentModel.ISupportInitialize)release).EndInit();
      ((System.ComponentModel.ISupportInitialize)trackBar2).EndInit();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private System.Windows.Forms.CheckedListBox checkedListBox1;
    private System.Windows.Forms.TrackBar trackBar1;
    private System.Windows.Forms.CheckBox FmModulation;
    private System.Windows.Forms.TrackBar attack;
    private System.Windows.Forms.TrackBar release;
    private System.Windows.Forms.TrackBar trackBar2;
  }
}
