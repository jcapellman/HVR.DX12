using System.Collections.Generic;

namespace HVR.Objects.Game.Level {
    public class LevelContainerItem {
        public string Title { get; set; }

        public List<LevelGeometryItem> Geometry { get; set; }
    }
}