using HVR.Common.Enums;

namespace HVR.Common.Helpers {
    public class PathHelper {
        public static string GetPath(ResourceTypes resourceType, string relativePath, string gameMod = Common.Constants.DEFAULT_GAME_MOD) =>
            gameMod + "/" + (resourceType == ResourceTypes.Base ? "" : $"{resourceType}/") + relativePath;
    }
}