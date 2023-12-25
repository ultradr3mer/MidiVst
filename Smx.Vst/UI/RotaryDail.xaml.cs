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
    public RotaryDail()
    {
      InitializeComponent();
    }

    private void Path_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      double fill = 0.6;
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
  }
}
