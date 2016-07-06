using System.Windows;

using HVR.ViewModels;

using MahApps.Metro.Controls;

namespace HVR {
    public partial class MainWindow : MetroWindow {
        private MainWindowViewModel viewModel => (MainWindowViewModel)DataContext;
        
        public MainWindow() {
            InitializeComponent();

            DataContext = new MainWindowViewModel();

            viewModel.LoadData();
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e) {
            viewModel.SaveConfig();

            System.Windows.Application.Current.Shutdown();
        }

        private void btnLaunch_Click(object sender, RoutedEventArgs e) {
            viewModel.SaveConfig();
            
            Hide();

            var mainLoop = new Helpers.MainLoopHelper();

            mainLoop.OnClose += MainLoop_OnClose;

            mainLoop.RunLoop(viewModel.GetMainLoopTransportItem());
        }

        private void MainLoop_OnClose(object sender, System.EventArgs e) {
            Show();
        }
    }
}