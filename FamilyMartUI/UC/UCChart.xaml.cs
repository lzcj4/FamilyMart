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
using FamilyMartUI.Common;

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
        private const double lineWidth = 2;
        private const double arrowWidth = 3;
        private const double flagHeight = 10;
        private const double flagOffset = 2;

        private double radius = 3;

        int[] xPoints = { 1, 2, 3, 4, 5, 6, 7 };
        int[] yPoints = { 10, 20, 30, 40, 50, 60, 70 };
        private string[] titles = { "进", "销", "废" };
        private double[][] dataArray = new double[][]{ new double[] { 15, 23, 56, 34, 20, 44, 2 }, 
                                          new double[]{ 3, 60, 29, 23, 56, 34, 23 },
                                          new double[]{ 63, 12, 69, 3, 32, 45, 33 }};
        DateTime[] xPointsDates = { };
        private string fontFace = "Segoe UI";

        Pen[] linePens = { new Pen(Brushes.Green, lineWidth), new Pen(Brushes.Blue, lineWidth), new Pen(Brushes.Red, lineWidth), };

        public void SetXYAxisAndData(DateTime[] xDates, int[] yPoints, double[][] data)
        {
            this.xPointsDates = xDates;
            this.xPoints = xDates.Select(item => item.Day).ToArray();
            this.yPoints = yPoints;
            this.dataArray = data;
            this.InvalidateVisual();
        }

        private FormattedText GetFormattedText(string str)
        {
            return GetFormattedText(str, Brushes.Black);
        }

        private FormattedText GetFormattedText(string str, Brush brush)
        {
            FormattedText formatText = new FormattedText(str, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                                                         new Typeface(fontFace), 18, brush);
            return formatText;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            if (xPoints.IsNullOrEmpty() || yPoints.IsNullOrEmpty() ||
                dataArray.IsNullOrEmpty() || xPointsDates.IsNullOrEmpty())
            {
                return;
            }

            Pen pen = new Pen(Brushes.Black, lineWidth);
            Pen arrowPen = new Pen(Brushes.Black, arrowWidth);

            FormattedText xMaxFT = GetFormattedText(xPoints.Max().ToString());
            FormattedText yMaxFT = GetFormattedText(yPoints.Max().ToString());
            FormattedText tMaxFT = GetFormattedText(titles.Max().ToString());

            double xOffset = 2 * yMaxFT.Width + flagOffset;
            double yOffset = xMaxFT.Height + flagOffset;
            double canvasWidth = this.ActualWidth - xOffset - tMaxFT.Width;
            double canvasHeight = this.ActualHeight - yOffset - flagHeight;

            double xStep = canvasWidth / xPoints.Length;
            double yStep = canvasHeight / yPoints.Length;
            int j = 1;
            dc.DrawLine(pen, new Point(xOffset, canvasHeight), new Point(xOffset + canvasWidth, canvasHeight));
            FormattedText ftStart = GetFormattedText("0");

            dc.DrawText(ftStart, new Point(xOffset, canvasHeight + flagOffset));
            //X-order flag
            for (int i = 0; i < xPoints.Length; i++, j++)
            {
                double startX = xOffset + j * xStep;
                DateTime itemDate = xPointsDates[i];
                FormattedText ft = GetFormattedText(xPoints[i].ToString(), 
                                                   (itemDate.DayOfWeek == DayOfWeek.Sunday || 
                                                    itemDate.DayOfWeek == DayOfWeek.Saturday) ? Brushes.Red : Brushes.Black);
                if (i == xPoints.Length - 1)
                {
                    //arrow
                    dc.DrawLine(arrowPen, new Point(startX - flagHeight * 2, canvasHeight - flagHeight),
                                new Point(startX, canvasHeight));
                    dc.DrawText(ft, new Point(startX - ft.Width, canvasHeight + flagOffset));
                }
                else
                {
                    dc.DrawLine(pen, new Point(startX, canvasHeight), new Point(startX, canvasHeight - flagHeight));
                    dc.DrawText(ft, new Point(startX - ft.Width / 2, canvasHeight + flagOffset));
                }
            }

            dc.DrawLine(pen, new Point(xOffset, canvasHeight), new Point(xOffset, flagHeight));
            j = 1;
            //Y-order flag
            for (int i = 0; i < yPoints.Length; i++, j++)
            {
                double startY = j * yStep - flagHeight;
                FormattedText ft = GetFormattedText(yPoints[i].ToString());
                if (i == yPoints.Length - 1)
                {
                    dc.DrawLine(arrowPen, new Point(xOffset + flagHeight, flagHeight * 3),
                                new Point(xOffset, flagHeight));
                    dc.DrawText(ft, new Point(yMaxFT.Width / 2, canvasHeight - startY - ft.Height /2));
                }
                else
                {
                    dc.DrawLine(pen, new Point(xOffset, canvasHeight - startY), new Point(xOffset + flagHeight, canvasHeight - startY));
                    dc.DrawText(ft, new Point(yMaxFT.Width / 2, canvasHeight - startY - ft.Height / 2));
                }

            }

            //divide to the every value step
            yStep = canvasHeight / yPoints.Max();
            for (int i = 0; i <= dataArray.GetUpperBound(0); i++)
            {
                //Line
                j = 1;
                Pen currentPen = linePens[i];
                Point lastPoint = new Point(xOffset, canvasHeight);

                for (int z = 0; z <= dataArray[0].GetUpperBound(0); z++, j++)
                {
                    //Point newPoint = new Point(xOffset + j * xStep - radius, canvasHeight + flagHeight - dataArray[i][z] * yStep - radius);
                    Point newPoint = new Point(xOffset + j * xStep - radius, canvasHeight - dataArray[i][z] * yStep - radius);
                    dc.DrawLine(currentPen, lastPoint, newPoint);
                    dc.DrawEllipse(currentPen.Brush, currentPen, newPoint, radius, radius);
                    lastPoint = newPoint;
                }
            }

            double yMiddle = canvasHeight / 2;

            for (int i = 0; i < titles.Length; i++)
            {
                FormattedText ft = GetFormattedText(titles[i], linePens[i].Brush);
                if (i == 0)
                    dc.DrawText(ft, new Point(xOffset + canvasWidth, yMiddle - 4 * flagHeight));
                if (i == 1)
                    dc.DrawText(ft, new Point(xOffset + canvasWidth, yMiddle));
                if (i == 2)
                    dc.DrawText(ft, new Point(xOffset + canvasWidth, yMiddle + 4 * flagHeight));
            }
        }

    }
}
