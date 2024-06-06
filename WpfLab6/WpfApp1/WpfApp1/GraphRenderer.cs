using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using RpnLogic;

namespace WpfApp1
{
    public class CanvasRenderer
    {
        private Canvas _canvasGraph;
        private Ellipse _selectedPoint;
        private TextBlock _coordinateText;
        private List<(double x, double y, double canvasX, double canvasY)> _graphPoints;
        private DispatcherTimer _coordinateTextTimer;
        private bool _isCoordinateTextVisible;

        public CanvasRenderer(Canvas canvasGraph)
        {
            _canvasGraph = canvasGraph;
            _graphPoints = new List<(double, double, double, double)>();
            _coordinateTextTimer = new DispatcherTimer();
            _coordinateTextTimer.Interval = TimeSpan.FromSeconds(0.6);
            _coordinateTextTimer.Tick += CoordinateTextTimer_Tick;
            _isCoordinateTextVisible = false;
        }


        public void DrawFunctionGraph(string expression, double start, double end, double scale, double step)
        {
            _canvasGraph.Children.Clear();

            DrawCoordinateSystem(start, end, scale, step);

            List<(double x, double y)> points = new List<(double, double)>();
            RpnCalculator calculator = new RpnCalculator();
            double x = start;
            while (x <= end)
            {
                double y = calculator.CalculateWithVariable(expression, x);
                if (!double.IsInfinity(y) && !double.IsNaN(y))
                {
                    points.Add((x, y));
                }
                x += step;
            }

            DrawGraphPoints(points, scale);
            DrawGraphLines(points, scale);
        }

        private void DrawCoordinateSystem(double start, double end, double scale, double step)
        {
            //Hbcetv ось X
            double xStart = 0;
            double xEnd = _canvasGraph.Width;
            double yStart = _canvasGraph.Height / 2;
            double yEnd = _canvasGraph.Height / 2;
            DrawLine(xStart, xEnd, yStart, yEnd, Brushes.Black, 2);

            //Рисуем ось Y
            xStart = _canvasGraph.Width / 2;
            xEnd = _canvasGraph.Width / 2;
            yStart = 0;
            yEnd = _canvasGraph.Height;
            DrawLine(xStart, xEnd, yStart, yEnd, Brushes.Black, 2);

            //Деления
            double x = start;
            double y = 0;
            while (x <= end)
            {
                DrawAxisTick(x, y, scale);
                x += step;
            }

            y = start;
            x = 0;
            while (y <= end)
            {
                DrawAxisTick(x, y, scale);
                y += step;
            }
        }

        private void DrawAxisTick(double x, double y, double scale)
        {
            double canvasX = (x * scale) + _canvasGraph.Width / 2;
            double canvasY = (_canvasGraph.Height / 2) - (y * scale);
            Ellipse point = new Ellipse
            {
                Width = 6.5,
                Height = 6.5,
                Fill = Brushes.Red
            };
            Canvas.SetLeft(point, canvasX - point.Width / 2);
            Canvas.SetTop(point, canvasY - point.Height / 2);
            _canvasGraph.Children.Add(point);
        }

        private void DrawGraphPoints(List<(double x, double y)> points, double scale)
        {
            _graphPoints.Clear();
            foreach ((double x, double y) in points)
            {
                double canvasX = (x * scale) + _canvasGraph.Width / 2;
                double canvasY = (_canvasGraph.Height / 2) - (y * scale);
                Ellipse point = new Ellipse
                {
                    Width = 7,
                    Height = 7,
                    Fill = Brushes.Blue
                };
                Canvas.SetLeft(point, canvasX - point.Width / 2);
                Canvas.SetTop(point, canvasY - point.Height / 2);
                point.MouseEnter += GraphPoint_MouseEnter;
                point.MouseLeave += GraphPoint_MouseLeave;
                _canvasGraph.Children.Add(point);
                _graphPoints.Add((x, y, canvasX, canvasY));
            }
        }

        private void DrawGraphLines(List<(double x, double y)> points, double scale)
        {
            for (int i = 1; i < points.Count; i++)
            {
                double x1 = (points[i - 1].x * scale) + _canvasGraph.Width / 2;
                double y1 = (_canvasGraph.Height / 2) - (points[i - 1].y * scale);
                double x2 = (points[i].x * scale) + _canvasGraph.Width / 2;
                double y2 = (_canvasGraph.Height / 2) - (points[i].y * scale);
                DrawLine(x1, x2, y1, y2, Brushes.Blue, 2);
            }
        }

        private void DrawLine(double x1, double x2, double y1, double y2, Brush color, double thickness)
        {
            Line line = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = color,
                StrokeThickness = thickness
            };
            _canvasGraph.Children.Add(line);
        }

        private void GraphPoint_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Ellipse point = sender as Ellipse;
            if (point != null)
            {
                _selectedPoint = point;
                double pointCanvasX = Canvas.GetLeft(point) + point.Width / 2;
                double pointCanvasY = Canvas.GetTop(point) + point.Height / 2;
                var pointInfo = _graphPoints.Find(p => p.canvasX == pointCanvasX && p.canvasY == pointCanvasY);
                double x = pointInfo.x;
                double y = pointInfo.y;

                if (_coordinateText == null)
                {
                    _coordinateText = new TextBlock
                    {
                        Foreground = Brushes.White,
                        Background = Brushes.Black,
                        Padding = new Thickness(5)
                    };
                    _canvasGraph.Children.Add(_coordinateText);
                }

                _coordinateText.Text = $"x: {x:F2}, y: {y:F2}";
                Canvas.SetLeft(_coordinateText, pointInfo.canvasX - point.Width / 2);
                Canvas.SetTop(_coordinateText, pointInfo.canvasY - point.Height / 2 - 20);
                _isCoordinateTextVisible = true;
                _coordinateTextTimer.Start();
            }
        }

        private void GraphPoint_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_selectedPoint != null)
            {
                if (_isCoordinateTextVisible)
                {
                    _coordinateTextTimer.Start();
                }
                else
                {
                    _canvasGraph.Children.Remove(_coordinateText);
                    _coordinateText = null;
                }
                _selectedPoint = null;
            }
        }
        private void CoordinateTextTimer_Tick(object sender, EventArgs e)
        {
            _coordinateTextTimer.Stop();
            _isCoordinateTextVisible = false;
            _canvasGraph.Children.Remove(_coordinateText);
            _coordinateText = null;
        }

    }
}