﻿using SharpDX.DXGI;

namespace HVR.Objects.Launcher {
    public class AdapterListingItem {
        public Adapter DXAdapter { get; set; }

        public string Display => DXAdapter.Description.Description.Replace("\0", "");
    }
}