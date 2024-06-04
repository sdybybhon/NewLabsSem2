using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using RpnLogic;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private CanvasRenderer _canvasRenderer;
        private Canvas _canvasGraph;

        public MainWindow()
        {
            InitializeComponent();
            _canvasGraph = CanvasGraph;
            _canvasRenderer = new CanvasRenderer(_canvasGraph);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string expression = txtboxInput.Text;
            double start = double.Parse(txtboxStart.Text);
            double end = double.Parse(txtboxEnd.Text);
            double scale = double.Parse(txtboxScale.Text);
            double step = double.Parse(txtboxStep.Text);

            _canvasRenderer.DrawFunctionGraph(expression, start, end, scale, step);
        }
    }
}