using HVR.Common.Enums;

namespace HVR.Common.Objects.Editor {
    public class MapObjectsContainerItem {
        public MapObjectTypes MapObjectType { get; set; }

        public MapGeometryTypes MapGeometryType { get; set; }

        public string Description { get; set; }

        public string PreviewImage { get; set; }
    }
}