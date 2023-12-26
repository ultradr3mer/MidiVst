using Smx.Vst.Util;
using Smx.Vst.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Smx.Vst.UI
{
    /// <summary>
    /// Interaction logic for RotaryDail.xaml
    /// </summary>
    public partial class RotaryDail : UserControl
  {
    private double valueStart;
    private Point mouseStart;
    private DailViewModel viewModel;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public RotaryDail()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
      InitializeComponent();

      this.MouseDown += Dail_MouseDown;
      this.MouseUp += Dail_MouseUp;
      this.MouseMove += Dail_MouseMove;

      this.DataContextChanged += RotaryDail_DataContextChanged;
    }

    private void RotaryDail_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      this.viewModel = e.NewValue as DailViewModel;
      this.UpdateGeometry();
    }

    private void Dail_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (this.viewModel == null)
        return;

      this.viewModel.IsPressed = true;
      this.mouseStart = Mouse.GetPosition(this);
      this.valueStart = this.viewModel.NormalizedValue;
      Mouse.Capture(this);
    }

    private void Dail_MouseMove(object sender, MouseEventArgs e)
    {
      if(this.viewModel == null) 
        return;

      if (this.viewModel.IsPressed == true)
      {
        double mouseDelta = Mouse.GetPosition(this).Y - this.mouseStart.Y;
        var value = Math.Clamp(valueStart - mouseDelta / 100.0, 0.0, 1.0);
        this.viewModel.NormalizedValue = value;
        this.UpdateGeometry();
      }
    }

    private void Dail_MouseUp(object sender, MouseButtonEventArgs e)
    {
      if (this.viewModel == null)
        return;

      this.viewModel.IsPressed = false;
      this.mouseStart = new Point(0, 0);
      this.valueStart = 0;
      Mouse.Captured?.ReleaseMouseCapture();
    }

    private void Path_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.UpdateGeometry();
    }

    private void UpdateGeometry()
    {
      if (this.viewModel == null)
        return;

      double rad = Math.PI * 2.0 * Math.Clamp(this.viewModel.NormalizedValue, 0.001,0.999);

      double radius = OuterEllipse.ActualWidth / 2.0;

      Path myPath = this.FillState;
      StreamGeometry geometry = new StreamGeometry();
      geometry.FillRule = FillRule.EvenOdd;

      using (StreamGeometryContext ctx = geometry.Open())
      {
        ctx.BeginFigure(new Point(0, 0), true /* is filled */, true /* is closed */);
        ctx.LineTo(new Point(0, radius), true /* is stroked */, false /* is smooth join */);
        ctx.ArcTo(new Point(-Math.Sin(rad) * radius, Math.Cos(rad) * radius),
          new Size(radius, radius),
          rotationAngle: 0,
          isLargeArc: this.viewModel.NormalizedValue > 0.5f,
          sweepDirection: SweepDirection.Clockwise,
          isStroked: true,
          isSmoothJoin: false
          );
      }

      geometry.Freeze();
      myPath.Data = geometry;
    }
  }
}