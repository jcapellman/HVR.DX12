using System.Windows;
using System.Windows.Forms;

using SharpDX.Windows;

using HVR.ViewModels;
using HVR.Common.Helpers;
using HVR.SoundRenderer.DX12;
using HVR.Common.Objects.Global;

using MahApps.Metro.Controls;

namespace HVR {
    public partial class MainWindow : MetroWindow {
        private MainWindowViewModel viewModel => (MainWindowViewModel)DataContext;
        private DX12SoundRenderer sndRenderer = new DX12SoundRenderer();

        private SoundItem sndItem = new SoundItem {
            FileName = Common.Helpers.PathHelper.GetPath(Common.Enums.ResourceTypes.Sounds, "Character/Footstep01.wav")
        };

        private RenderForm _form;

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
            
            var level = LevelLoaderHelper.GetLevel("e1m1.lvl");

            if (level == null) {
                System.Windows.MessageBox.Show("Level could not be loaded");
                return;
            }

            Hide();

            _form = new RenderForm(Common.Constants.GAME_NAME) {
                Width = viewModel.SelectedScreenResolution.Width,
                Height = viewModel.SelectedScreenResolution.Height,
                IsFullscreen = viewModel.IsFullscreen,
                ShowIcon = false
            };

            _form.KeyUp += Form_KeyUp;
            _form.Show();

            using (var app = new HVR.Renderer.DX12.MainRenderWindow()) {
                app.OnClose += App_OnClose;
                app.Initialize(ref _form, viewModel.SelectedAdapter.DXAdapter, level, App.CfgHelper);

                using (var loop = new RenderLoop(_form)) {
                    while (loop.NextFrame()) {
                        app.Update();
                        app.Render();
                    }
                }
            }
        }

        private void Form_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.W:
                case Keys.S:
                case Keys.A:
                case Keys.D:
                    sndRenderer.Play(sndItem);
                    break;
                case Keys.Escape:
                    _form.Close();
                    break;
            }
        }

        private void App_OnClose(object sender, System.EventArgs e) {
            Show();
        }

        private void btnLaunch_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) {
            //
            
            //sndRenderer.Play(sndItem);
        }
    }
}