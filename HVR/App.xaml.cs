using System.Windows;

using HVR.Common.Helpers;

namespace HVR {
    public partial class App : Application {
        public static ConfigHelper CfgHelper;

        public App() {
            CfgHelper = new ConfigHelper();
        }
    }
}