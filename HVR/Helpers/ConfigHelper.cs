using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

using HVR.Enums;

namespace HVR.Helpers {
    public class ConfigHelper {
        private Dictionary<ConfigOptions, object> _configData;

        private string configFileName => $"{Common.Constants.DEFAULT_GAME_MOD}/{Common.Constants.CONFIG_FILENAME}";

        public ConfigHelper() {
            _configData = new Dictionary<ConfigOptions, object>();
            
            if (!File.Exists(configFileName)) {
                return;
            }

            var fileStr = File.ReadAllText(configFileName);

            _configData = JsonConvert.DeserializeObject<Dictionary<ConfigOptions, object>>(fileStr);
        }

        public void SetConfigOption(ConfigOptions configOption, object val) {
            _configData[configOption] = val;
        }

        private dynamic GetDefaultConfigOption(ConfigOptions configOption) {
            switch (configOption) {
                case ConfigOptions.SELECTED_ADAPTER:
                    return string.Empty;
                case ConfigOptions.SELECTED_FULLSCREEN:
                    return false;
                case ConfigOptions.SELECTED_RESOLUTION:
                    return "1280x720";
                case ConfigOptions.SELECTED_MULTISAMPLE_VALUE:
                    return 4;
            }

            return null;
        }

        public void WriteConfig() {
            File.WriteAllText(configFileName, JsonConvert.SerializeObject(_configData));
        }

        public dynamic GetConfigOption(ConfigOptions configOption) {
            if (!_configData.ContainsKey(configOption)) {
                return GetDefaultConfigOption(configOption);
            }

            switch (configOption) {
                case ConfigOptions.SELECTED_RESOLUTION:
                    return _configData[configOption];
                case ConfigOptions.SELECTED_FULLSCREEN:
                    return Convert.ToBoolean(_configData[configOption]);
                case ConfigOptions.SELECTED_MULTISAMPLE_VALUE:
                    return Convert.ToInt32(_configData[configOption]);
            }

            return null;
        }
    }
}