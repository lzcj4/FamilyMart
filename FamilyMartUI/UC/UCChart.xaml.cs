using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace FamilyMartUI.UC
{
    /// <summary>
    /// Interaction logic for UCChart.xaml
    /// </summary>
    public partial class UCChart : UserControl
    {
        public UCChart()
        {
            InitializeComponent();
        }
    }

    public class MyCanvas : Canvas
    {
        private double lineWidth = 2;
        private double arrowWidth = 3;
        private double flagHeight = 10;

        private double radius = 2;

        int[] xPoints = { 1, 2, 3, 4, 5, 6, 7 };
        int[] yPoints = { 10, 20, 30, 40, 50, 60, 70 };

        private int[,] zPoints = new int[,] { { 15, 23, 56, 34, 20, 67, 2 }, 
                                          { 3, 60, 29, 23, 70, 34, 23 },
                                          { 63, 12, 69, 3, 70, 45, 33 }};

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            Pen pen = new Pen(Brushes.Black, lineWidth);
            Pen arrowPen = new Pen(Brushes.Black, arrowWidth);

            int xMax = xPoints.Max();
            int yMax = yPoints.Max();
            FormattedText xMaxFT = new FormattedText(xMax.ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Klavika"), 20, Brushes.Black);
            FormattedText yMaxFT = new FormattedText(yPoints.ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Klavika"), 20, Brushes.Black);

            double xOffset = yMaxFT.Width + 10;
            double yOffset = xMaxFT.Height + 10;
            double canvasWidth = this.ActualWidth;
            double canvasHeight = this.ActualHeight;

            double xStep = canvasWidth / xPoints.Length;
            double yStep = canvasHeight / yPoints.Length;
            int j = 1;
            dc.DrawLine(pen, new Point(xOffset, canvasHeight - yOffset), new Point(canvasWidth, canvasHeight - yOffset));
            //X-order lable
            for (int i = 0; i < xPoints.Length; i++, j++)
            {
                if (i == xPoints.Length - 1)
                {
                    //arrow
                    dc.DrawLine(arrowPen, new Point(j * xStep - flagHeight * 2, canvasHeight - 2 * flagHeight), new Point(j * xStep, canvasHeight));
                }
                else
                {
                    dc.DrawLine(pen, new Point(j * xStep, canvasHeight), new Point(j * xStep, canvasHeight - flagHeight));
                }

                FormattedText ft = new FormattedText(xPoints[i].ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Klavika"), 20, Brushes.Black);
                dc.DrawText(ft, new Point(j * xStep - ft.Width / 2, canvasHeight + 20));
            }

            dc.DrawLine(pen, new Point(0, canvasHeight), new Point(0, 0));
            j = 1;
            //Y-order lable
            for (int i = 0; i < yPoints.Length; i++, j++)
            {
                if (i == yPoints.Length - 1)
                {
                    dc.DrawLine(arrowPen, new Point(flagHeight * 2, canvasHeight - j * yStep + flagHeight * 2), new Point(0, canvasHeight - j * yStep));
                }
                else
                {
                    dc.DrawLine(pen, new Point(0, canvasHeight - j * yStep), new Point(flagHeight, canvasHeight - j * yStep));
                }

                FormattedText ft = new FormattedText(yPoints[i].ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Klavika"), 20, Brushes.Black);
                dc.DrawText(ft, new Point(0 - 30, canvasHeight - j * yStep + ft.Height / 2));
            }

            yStep = canvasHeight / yPoints.Max();
            Pen[] linePens = { new Pen(Brushes.Red, lineWidth), new Pen(Brushes.Green, lineWidth), new Pen(Brushes.Blue, lineWidth), };
            for (int i = 0; i <= zPoints.GetUpperBound(0); i++)
            {
                //Line
                j = 1;
                Pen currentPen = linePens[i];
                Point lastPoint = new Point(0, canvasHeight);

                for (int z = 0; z <= zPoints.GetUpperBound(1); z++, j++)
                {
                    Point newPoint = new Point(j * xStep, zPoints[i, z] * yStep);
                    dc.DrawLine(currentPen, lastPoint, newPoint);
                    dc.DrawEllipse(Brushes.Black, currentPen, newPoint, radius, radius);
                    lastPoint = newPoint;
                }
            }

        }

    }
}
