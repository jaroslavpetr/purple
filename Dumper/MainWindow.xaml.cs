using System.Windows;
using ServerCore;
using ServerCore.Reports;

namespace Dumper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        #region Events

        //Button starts generation report
        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            GenerateReport();
        }

     
        #endregion

        #region Code

        //Generate report based on config data from app.config file
        private void GenerateReport() {

            var reporter = new clsReporter();
            reporter.AllServers = ConfigHelp.getAllConnections();
            reporter.StartDate = ConfigHelp.getStartDate();
            reporter.StopDate = ConfigHelp.getStopDate();
            reporter.ExcludedGroup = ConfigHelp.getExcludedGroup();
            reporter.ExcludedUser = ConfigHelp.getExcludedUser();

            reporter.CreateReport();
            MessageBox.Show("Done");
        }
        #endregion

    }
}
