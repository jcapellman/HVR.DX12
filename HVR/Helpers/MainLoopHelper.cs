using System;
using System.Collections.Generic;
using System.IO;

using HVR.Common.Enums;
using HVR.Common.Objects.Global;
using HVR.Common.Objects.Launcher;
using HVR.Common.Helpers;
using HVR.Common.Objects.Game.Global;

using HVR.SoundRenderer.DX12;

using Newtonsoft.Json;

using SharpDX.Windows;
using SharpDX.DirectInput;

namespace HVR.Helpers {
    public class MainLoopHelper {
        private RenderForm _form;
        public event EventHandler OnClose;
        private InputHandler _inputHandler;
        private DX12SoundRenderer _sndRenderer = new DX12SoundRenderer();

        private Dictionary<CHARACTER_EVENT, SoundItem> _baseSounds;

        private void LoadBaseResources() {
            _baseSounds = new Dictionary<CHARACTER_EVENT, SoundItem>();

            var baseItems = JsonConvert.DeserializeObject<List<BaseResourceItem>>(File.ReadAllText(PathHelper.GetPath(ResourceTypes.Base, "BaseItems.json")));

            foreach (var item in baseItems) {
                var fileName = PathHelper.GetPath(item.ResourceType, item.RelativePath);

                switch (item.ResourceType) {
                    case ResourceTypes.Sounds:
                        _baseSounds.Add(item.CharacterEvent, new SoundItem {
                            FileName = fileName
                        });
                        break;
                }
            }
        }

        public void RunLoop(MainLoopTransportItem mainLoopItem) {
            _inputHandler = new InputHandler();

            LoadBaseResources();

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
                    case Key.W:
                        _sndRenderer.Play(_baseSounds[CHARACTER_EVENT.WALK_FORWARD]);
                        break;
                    case Key.A:
                        _sndRenderer.Play(_baseSounds[CHARACTER_EVENT.WALK_LEFT]);
                        break;
                    case Key.D:
                        _sndRenderer.Play(_baseSounds[CHARACTER_EVENT.WALK_RIGHT]);
                        break;
                    case Key.S:
                        _sndRenderer.Play(_baseSounds[CHARACTER_EVENT.WALK_BACKWARDS]);
                        break;
                }
            }
        }

        private void App_OnClose(object sender, EventArgs e) {
            OnClose(this, null);
        }
    }
}