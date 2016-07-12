using HVR.Editor.ViewModels;

using System.Windows;

namespace HVR.Editor {
    public partial class MainWindow : Window {
        private MainModel viewModel => (MainModel)DataContext;

        public MainWindow() {
            InitializeComponent();

            DataContext = new MainModel();

            viewModel.LoadModel();
        }

        private void MiExit_OnClick(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }
    }
}