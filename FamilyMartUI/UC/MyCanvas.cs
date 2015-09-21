﻿using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FamilyMartUI.Common;


namespace FamilyMartUI.UC
{
    public class MyCanvas : Canvas
    {
        private const double lineWidth = 1.5;
        private const double pointRadius = 2;
        private const double flagHeight = 10;
        private const double offset = 5;
        private const string fontFace = "Segoe UI";

        private double yChartPadding = 20;
        private double xChartPadding = 20;
        int[] xPoints = { 1, 2, 3, 4, 5, 6, 7 };
        int[] yPoints = { 10, 20, 30, 40, 50, 60, 70 };
        double[][] dataArray = new double[][]{ new double[] { 15, 23, 56, 34, 20, 44, 2 }, 
                                          new double[]{ 3, 60, 29, 23, 56, 34, 23 },
                                          new double[]{ 63, 12, 69, 3, 32, 45, 33 }};
        DateTime[] xPointsDates = { DateTime.Now, DateTime.Now.AddDays(1),
                                      DateTime.Now.AddDays(2), DateTime.Now.AddDays(3),
                                      DateTime.Now.AddDays(4), DateTime.Now.AddDays(5),
                                      DateTime.Now.AddDays(6) };
        string[] titles = { "进", "销", "废" };
        Pen[] linePens = { new Pen(Brushes.Green, lineWidth), new Pen(Brushes.Blue, lineWidth), new Pen(Brushes.Red, lineWidth), };

        public void SetXYAxisAndData(DateTime[] xDates, int[] yPoints, double[][] data)
        {
            this.xPointsDates = xDates;
            this.xPoints = xDates.Select(item => item.Day).ToArray();
            this.yPoints = yPoints;
            this.dataArray = data;
            this.xChartPadding = 20;
            this.yChartPadding = 20;
            this.InvalidateVisual();
        }

        public void SetTitleAndBrushes(string[] items, Brush[] brushes)
        {
            titles = items;
            linePens = brushes.Select(item => new Pen(item, lineWidth)).ToArray();
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

            Pen blackPen = new Pen(Brushes.Black, lineWidth);
            Pen grayPen = new Pen(Brushes.LightGray, 1);

            FormattedText xMaxFT = GetFormattedText(xPoints.Max().ToString());
            FormattedText yMaxFT = GetFormattedText(yPoints.Max().ToString());
            FormattedText tMaxFT = GetFormattedText(titles.Max().ToString());

            xChartPadding = yMaxFT.Width > xChartPadding ? yMaxFT.Width + offset : xChartPadding;
            yChartPadding = xMaxFT.Height > yChartPadding ? xMaxFT.Height : yChartPadding;

            double canvasWidth = this.ActualWidth - xChartPadding * 2;
            double canvasHeight = this.ActualHeight - yChartPadding * 2;

            double xStep = canvasWidth / xPoints.Length;
            int j = 1;
            dc.DrawLine(blackPen, new Point(xChartPadding, canvasHeight + yChartPadding),
                                  new Point(xChartPadding + canvasWidth, canvasHeight + yChartPadding));
            FormattedText ftStart = GetFormattedText("0");
            dc.DrawText(ftStart, new Point(yChartPadding, canvasHeight + yChartPadding - flagHeight / 4));

            //X-order flag
            for (int i = 0; i < xPoints.Length; i++, j++)
            {
                double startX = xChartPadding + j * xStep;
                DateTime itemDate = xPointsDates[i];
                FormattedText ft = GetFormattedText(xPoints[i].ToString(),
                                                   (itemDate.DayOfWeek == DayOfWeek.Sunday ||
                                                    itemDate.DayOfWeek == DayOfWeek.Saturday) ? Brushes.Red : Brushes.Black);
                dc.DrawLine(blackPen, new Point(startX, canvasHeight + yChartPadding),
                                      new Point(startX, canvasHeight + yChartPadding - flagHeight));
                dc.DrawText(ft, new Point(startX - ft.Width / 2, canvasHeight + yChartPadding - flagHeight / 4));
            }

            dc.DrawLine(blackPen, new Point(xChartPadding, canvasHeight + yChartPadding),
                                  new Point(xChartPadding, yChartPadding));
            j = 1;

            double yStep = canvasHeight / yPoints.Length;
            //Y-order flag
            for (int i = 0; i < yPoints.Length; i++, j++)
            {
                double startY = j * yStep;
                FormattedText ft = GetFormattedText(yPoints[i].ToString());
                dc.DrawLine(grayPen, new Point(xChartPadding, canvasHeight + yChartPadding - startY), new Point(canvasWidth + xChartPadding, canvasHeight + yChartPadding - startY));
                dc.DrawText(ft, new Point(0, canvasHeight + yChartPadding - startY - ft.Height / 2));
            }

            //divide to the every value step
            yStep = canvasHeight / yPoints.Max();
            for (int i = 0; i <= dataArray.GetUpperBound(0); i++)
            {
                //Line
                j = 1;
                Pen currentPen = linePens[i];
                Point lastPoint = new Point(xChartPadding, canvasHeight + yChartPadding);

                for (int z = 0; z <= dataArray[0].GetUpperBound(0); z++, j++)
                {
                    Point newPoint = new Point(xChartPadding + j * xStep,
                                               canvasHeight + yChartPadding - dataArray[i][z] * yStep);
                    dc.DrawLine(currentPen, lastPoint, newPoint);
                    dc.DrawEllipse(currentPen.Brush, currentPen, newPoint, pointRadius, pointRadius);
                    lastPoint = newPoint;
                }
            }

            double yMiddle = this.ActualHeight / 2;

            int middleValue = (int)Math.Ceiling(titles.Length * 1.0 / 2);
            for (int i = 0; i < titles.Length; i++)
            {
                FormattedText ft = GetFormattedText(titles[i], linePens[i].Brush);
                dc.DrawText(ft, new Point(xChartPadding + canvasWidth + offset, yMiddle - 2 * yChartPadding * (--middleValue)));
            }
        }

    }
}