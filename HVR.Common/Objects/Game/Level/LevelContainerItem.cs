using System.Collections.Generic;

namespace HVR.Common.Objects.Game.Level {
    public class LevelContainerItem {
        public string Title { get; set; }

        public List<LevelGeometryItem> Geometry { get; set; }
    }
}