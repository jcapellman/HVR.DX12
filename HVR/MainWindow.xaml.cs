using System.Windows;

using SharpDX.Windows;
using HVR.ViewModels;

namespace HVR {
    public partial class MainWindow : Window {
        private MainWindowViewModel viewModel => (MainWindowViewModel)DataContext;

        public MainWindow() {
            InitializeComponent();

            DataContext = new MainWindowViewModel();

            viewModel.LoadData();
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void btnLaunch_Click(object sender, RoutedEventArgs e) {
            this.Hide();

            var form = new RenderForm("HorrorVR") {
                Width = viewModel.SelectedScreenResolution.Width,
                Height = viewModel.SelectedScreenResolution.Height,
                IsFullscreen = viewModel.IsFullscreen,
                ShowIcon = false
            };

            form.Show();

            using (var app = new MainRenderWindow()) {
                app.Initialize(form);

                using (var loop = new RenderLoop(form)) {
                    while (loop.NextFrame()) {
                        app.Update();
                        app.Render();
                    }
                }
            }
        }
    }
}