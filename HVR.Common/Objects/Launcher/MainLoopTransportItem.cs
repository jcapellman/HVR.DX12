using SharpDX.DXGI;

namespace HVR.Common.Objects.Launcher {
    public class MainLoopTransportItem {
        public int Width { get; set; }

        public int Height { get; set; }

        public bool IsFullScreen { get; set; }
        
        public Common.Objects.Game.Level.LevelContainerItem Level { get; set; }

        public Common.Helpers.ConfigHelper CfgHelper { get; set; }

        public Adapter DXAdapter { get; set; }
    }
}