using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVR.Objects.Game.Level {
    public class LevelGeometryItem {
        public List<LevelVertexItem> Vertices { get; set; }

        public Color4 Color { get; set; }
    }
}
