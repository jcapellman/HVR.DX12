using System.Windows;

namespace HVR.Editor {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void MiExit_OnClick(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }
    }
}