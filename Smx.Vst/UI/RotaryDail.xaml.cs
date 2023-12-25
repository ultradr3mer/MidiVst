using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Smx.Vst.UI
{
  /// <summary>
  /// Interaction logic for RotaryDail.xaml
  /// </summary>
  public partial class RotaryDail : UserControl
  {
    private bool isPressed;
    private Point mouseStart;
    private double fillStart;
    double fill = 0.6;

    public RotaryDail()
    {
      InitializeComponent();
    }

    private void Path_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.UpdateGeometry();
    }

    private void UpdateGeometry()
    {
      double rad = Math.PI * 2.0 * fill;

      double radius = OuterEllipse.ActualWidth / 2.0;

      Path myPath = this.FillState;
      StreamGeometry geometry = new StreamGeometry();
      geometry.FillRule = FillRule.EvenOdd;

      using (StreamGeometryContext ctx = geometry.Open())
      {
        ctx.BeginFigure(new Point(0, 0), true /* is filled */, true /* is closed */);
        ctx.LineTo(new Point(0, radius), true /* is stroked */, false /* is smooth join */);
        ctx.ArcTo(new Point(Math.Sin(rad) * radius, -Math.Cos(rad) * radius),
          new Size(radius, radius),
          rotationAngle: 0,
          isLargeArc: false,
          sweepDirection: SweepDirection.Clockwise,
          isStroked: true,
          isSmoothJoin: false
          );
      }
      geometry.Freeze();
      myPath.Data = geometry;
    }

    private void UserControl_Initialized(object sender, EventArgs e)
    {
      
    }

    private void MainGrid_MouseDown(object sender, MouseButtonEventArgs e)
    {

    }

    private void Button_MouseDown(object sender, MouseButtonEventArgs e)
    {
      this.isPressed = true;
      this.mouseStart = Mouse.GetPosition(this);
      this.fillStart = this.fill;
    }

    private void Button_MouseUp(object sender, MouseButtonEventArgs e)
    {
      this.isPressed = false;
      this.mouseStart = new Point(0, 0);
      this.fillStart = 0;
    }

    private void Button_MouseMove(object sender, MouseEventArgs e)
    {
      if (this.isPressed)
      {
        double mouseDelta = Mouse.GetPosition(this).Y - this.mouseStart.Y;
        this.fill = fillStart - mouseDelta / 100.0;
        this.UpdateGeometry();
      }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
  
    }
  }
}
