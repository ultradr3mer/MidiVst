using Smx.Vst.Util;
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
    private double fill = 0.6;
    private double fillStart;
    private Point mouseStart;
    private DailViewModel viewModel;

    public RotaryDail()
    {
      InitializeComponent();
      this.viewModel = new DailViewModel();
      this.DataContext = this.viewModel;

      this.MouseDown += Button_MouseDown;
      this.MouseUp += Button_MouseUp;
      this.MouseMove += Button_MouseMove;
    }


    private void Button_MouseDown(object sender, MouseButtonEventArgs e)
    {
      this.viewModel.IsPressed = true;
      this.mouseStart = Mouse.GetPosition(this);
      this.fillStart = this.fill;
    }

    private void Button_MouseMove(object sender, MouseEventArgs e)
    {
      if (this.viewModel.IsPressed)
      {
        double mouseDelta = Mouse.GetPosition(this).Y - this.mouseStart.Y;
        this.fill = Math.Clamp(fillStart - mouseDelta / 100.0, 0.0, 1.0);
        this.UpdateGeometry();
      }
    }

    private void Button_MouseUp(object sender, MouseButtonEventArgs e)
    {
      this.viewModel.IsPressed = false;
      this.mouseStart = new Point(0, 0);
      this.fillStart = 0;
    }

    private void Path_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.UpdateGeometry();
    }

    private void UpdateGeometry()
    {
      double rad = Math.PI * 2.0 * Math.Clamp(fill, 0.001,0.999);

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
          isLargeArc: fill > 0.5f,
          sweepDirection: SweepDirection.Clockwise,
          isStroked: true,
          isSmoothJoin: false
          );
      }
      geometry.Freeze();
      myPath.Data = geometry;
    }

    private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
    {

    }
  }
}