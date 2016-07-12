using HVR.Common.Enums;

namespace HVR.Common.Objects.Game.Level {
    public class LevelItem {
        public MapObjectTypes MapObjectType { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public string TextureName { get; set; }
    }
}