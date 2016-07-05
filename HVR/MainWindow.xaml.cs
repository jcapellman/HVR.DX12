using System.Windows;

using SharpDX.Windows;
using HVR.ViewModels;
using HVR.Common.Helpers;

using MahApps.Metro.Controls;
using HVR.Common.Objects.Global;
using HVR.SoundRenderer.DX12;

namespace HVR {
    public partial class MainWindow : MetroWindow {
        private MainWindowViewModel viewModel => (MainWindowViewModel)DataContext;
        private DX12SoundRenderer sndRenderer = new DX12SoundRenderer();

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

            using (var app = new HVR.Renderer.DX12.MainRenderWindow()) {
                app.Initialize(form, viewModel.SelectedAdapter.DXAdapter, level, App.CfgHelper);

                using (var loop = new RenderLoop(form)) {
                    while (loop.NextFrame()) {
                        app.Update();
                        app.Render();
                    }
                }
            }
        }

        private void btnLaunch_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) {
            var sndItem = new SoundItem {
                FileName = Common.Helpers.PathHelper.GetPath(Common.Enums.ResourceTypes.Sounds, "Atmospheric/breathing.wav")
            };
            
            sndRenderer.Play(sndItem);
        }
    }
}