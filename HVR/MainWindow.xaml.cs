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

            ccMain.OnHide += CcMain_OnHide;
            this.KeyUp += MainWindow_KeyUp;
        }

        private void CcMain_OnHide(object sender, System.EventArgs e) {
            ccMain.Visibility = Visibility.Collapsed;
        }

        private void MainWindow_KeyUp(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key != System.Windows.Input.Key.OemTilde) {
                return;
            }

            if (ccMain.Visibility == Visibility.Collapsed) {
                ccMain.Visibility = Visibility.Visible;
            } else {
                ccMain.Visibility = Visibility.Collapsed;
            }
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e) {
            viewModel.SaveConfig();

            Application.Current.Shutdown();
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