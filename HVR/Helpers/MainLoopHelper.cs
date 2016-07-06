using System;

using HVR.Common.Objects.Launcher;

using SharpDX.Windows;
using SharpDX.DirectInput;

namespace HVR.Helpers {
    public class MainLoopHelper {
        private RenderForm _form;
        public event EventHandler OnClose;
        private InputHandler _inputHandler;

        public void RunLoop(MainLoopTransportItem mainLoopItem) {
            _inputHandler = new InputHandler();

            _form = new RenderForm(Common.Constants.GAME_NAME) {
                Width = mainLoopItem.Width,
                Height = mainLoopItem.Height,
                IsFullscreen = mainLoopItem.IsFullScreen,
                ShowIcon = false
            };
            
            _form.Show();

            using (var app = new HVR.Renderer.DX12.MainRenderWindow()) {
                app.OnClose += App_OnClose;
                app.Initialize(ref _form, mainLoopItem.DXAdapter, mainLoopItem.Level, mainLoopItem.CfgHelper);

                using (var loop = new RenderLoop(_form)) {
                    while (loop.NextFrame()) {
                        HandleInput(_inputHandler.CheckInput());

                        app.Update();
                        app.Render();
                    }
                }
            }
        }

        private void HandleInput(KeyboardUpdate[] pressedKeys) {
            if (pressedKeys.Length == 0) {
                return;
            }

            foreach (var key in pressedKeys) {
                switch (key.Key) {
                    case Key.Escape:
                        _form.Close();

                        OnClose(this, null);
                        break;
                }
            }
        }

        private void App_OnClose(object sender, EventArgs e) {
            OnClose(this, null);
        }
    }
}