using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

using HVR.Enums;

namespace HVR.Helpers {
    public class ConfigHelper {
        private Dictionary<ConfigOptions, object> _configData;

        public ConfigHelper() {
            _configData = new Dictionary<ConfigOptions, object>();
            
            if (!File.Exists(Common.Constants.CONFIG_FILENAME)) {
                return;
            }

            var fileStr = File.ReadAllText(Common.Constants.CONFIG_FILENAME);

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
            }

            return null;
        }

        public void WriteConfig() {
            File.WriteAllText(Common.Constants.CONFIG_FILENAME, JsonConvert.SerializeObject(_configData));
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
            }

            return null;
        }
    }
}