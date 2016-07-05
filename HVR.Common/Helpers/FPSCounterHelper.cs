using System;

using SharpDX.Windows;

namespace HVR.Common.Helpers {
    public class FPSCounterHelper {
        private StopWatchHelper _stopWatcherHelper = new StopWatchHelper();

        private float _frameAccumulator;
        private int _frameCount;
        private float _frameDelta;
        private float _framePerSecond;

        public void Start() {
            _stopWatcherHelper.Start();
        }

        public void Update() {
            _frameDelta = (float)_stopWatcherHelper.Update();
        }

        public void Calculate(ref RenderForm form) {
            _frameAccumulator += _frameDelta;
            ++_frameCount;
            if (_frameAccumulator >= 1.0f) {
                _framePerSecond = _frameCount / _frameAccumulator;

                form.Text = Common.Constants.GAME_NAME + " - FPS: " + Math.Round(_framePerSecond, 0);
                _frameAccumulator = 0.0f;
                _frameCount = 0;
            }
        }
    }
}