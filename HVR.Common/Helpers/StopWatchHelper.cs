﻿using System.Diagnostics;

namespace HVR.Common.Helpers {
    public class StopWatchHelper {
        private Stopwatch _stopwatch;
        private double _lastUpdate;

        public StopWatchHelper() {
            _stopwatch = new Stopwatch();
        }

        public void Start() {
            _stopwatch.Start();
            _lastUpdate = 0;
        }

        public void Stop() {
            _stopwatch.Stop();
        }

        public double Update() {
            double now = ElapseTime;
            double updateTime = now - _lastUpdate;
            _lastUpdate = now;
            return updateTime;
        }

        public double ElapseTime =>_stopwatch.ElapsedMilliseconds * 0.001;
    }
}