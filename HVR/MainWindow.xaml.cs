using System.Windows;

using SharpDX.Windows;
using HVR.ViewModels;
using HVR.Helpers;

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

            Application.Current.Shutdown();
        }

        private void btnLaunch_Click(object sender, RoutedEventArgs e) {
            viewModel.SaveConfig();
            
            var level = LevelLoaderHelper.GetLevel("e1m1.lvl");

            if (level == null) {
                MessageBox.Show("Level could not be loaded");
                return;
            }

            Hide();

            var form = new RenderForm(Common.Constants.GAME_NAME) {
                Width = viewModel.SelectedScreenResolution.Width,
                Height = viewModel.SelectedScreenResolution.Height,
                IsFullscreen = viewModel.IsFullscreen,
                ShowIcon = false
            };

            form.Show();

            using (var app = new MainRenderWindow()) {
                app.Initialize(form, viewModel.SelectedAdapter.DXAdapter, level);

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