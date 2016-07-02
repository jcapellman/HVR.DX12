using System.IO;

using Newtonsoft.Json;

using HVR.Objects.Game.Level;

namespace HVR.Helpers {
    public class LevelLoaderHelper {
        public static LevelContainerItem GetLevel(string levelFileName, string selectedGameMod = Common.Constants.DEFAULT_GAME_MOD) {
            var path = Path.Combine(selectedGameMod, "Levels/" + levelFileName);

            if (!File.Exists(path)) {
                return null;
            }

            var fileStr = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<LevelContainerItem>(fileStr);
        }
    }
}