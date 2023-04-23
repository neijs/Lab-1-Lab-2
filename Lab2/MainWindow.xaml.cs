using System.Drawing;
using System.Windows;
using Microsoft.Win32;

namespace SpringLab2
{
    public partial class MainWindow : Window
    {
        ViewData viewData = new();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = viewData;
        }

        private void CanExecuteFromFile(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (viewData["NGrid"] == null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void ExecutedFromFile(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new()
            {
                Filter = "JSON Files | *.json",
                Title = "Please pick a JSON file for RawData initialization"
            };
            bool? success = fileDialog.ShowDialog();
            if (success == true)
            {
                string path = fileDialog.FileName;

                viewData.Load(path);
                if (viewData.RawData!.boundA >= viewData.RawData!.boundB)
                {
                    viewData.RawData = null;
                    MessageBox.Show("Chosen file has invalid interpolation boundaries");
                    return;                
                }
                if (viewData.RawData!.nodeQnt < 2)
                {
                    viewData.RawData = null;
                    MessageBox.Show("Chosen file has invalid interpolation nodes amount");
                    return;
                }
                viewData.ExecuteSplinesFF();

                rd_lb.ItemsSource = viewData.NodeValue;
                sd_lb.ItemsSource = viewData.SplineData!.spline;
                integ_tb.Text = viewData.SplineData.integValue.ToString();
            }
            else
            {
                MessageBox.Show("You didn't select anything...");
            }
        }

        private void CanExecuteFromControls(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (viewData["NodeQnt"] == null && viewData["BoundA"] == null && viewData["NGrid"] == null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void ExecutedFromControls(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            viewData.ExecuteSplinesFC();

            rd_lb.ItemsSource = viewData.NodeValue;
            sd_lb.ItemsSource = viewData.SplineData!.spline;
            integ_tb.Text = viewData.SplineData.integValue.ToString("f3");
        }

        private void CommandBinding_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (viewData["NodeQnt"] == null && viewData["BoundA"] == null && viewData["NGrid"] == null && viewData.RawData != null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void CommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            SaveFileDialog fileDialog = new()
            {
                Filter = "JSON Files | *.json",
                Title = "Please pick a JSON file for saving your data"
            };
            bool? success = fileDialog.ShowDialog();
            if (success == true)
            {
                string path = fileDialog.FileName;
                viewData.Save(path);
            }
            else
            {
                MessageBox.Show("You didn't select anything...");
            }
        }

        private void CanExecuteCharts(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (viewData.SplineData != null && viewData["NodeQnt"] == null && viewData["BoundA"] == null && viewData["NGrid"] == null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void ExecutedCharts(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            ChartsWindow chartWindow = new();

            double[] rdataX = viewData.RawData!.nodes;
            double[] rdataY = viewData.RawData!.values;
            chartWindow.rd_chart.Plot.AddScatter(rdataX, rdataY, Color.Green, markerSize: 10, lineWidth: 0);
            chartWindow.rd_chart.Refresh();

            double[] sdataX = new double[viewData.NGrid];
            double[] sdataY = new double[viewData.NGrid];

            for (int i = 0; i < viewData.NGrid; ++i)
            {
                sdataX[i] = viewData.SplineData!.spline[i].coord;
                sdataY[i] = viewData.SplineData!.spline[i].value;
            }
            chartWindow.sd_chart.Plot.AddScatter(sdataX, sdataY, Color.Red, markerSize: 0, lineWidth: 1);
            chartWindow.sd_chart.Refresh();

            chartWindow.Show();
        }
    }
}
