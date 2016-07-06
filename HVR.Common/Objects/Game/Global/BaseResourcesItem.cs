using HVR.Common.Enums;

namespace HVR.Common.Objects.Game.Global {
    public class BaseResourceItem {
        public ResourceTypes ResourceType { get; set; }
        
        public string RelativePath { get; set; }

        public CHARACTER_EVENT CharacterEvent { get; set; }
    }
}