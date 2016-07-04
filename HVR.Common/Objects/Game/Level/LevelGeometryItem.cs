using SharpDX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVR.Common.Objects.Game.Level {
    public class LevelGeometryItem {
        public List<LevelVertexItem> Vertices { get; set; }

        public Color Color { get; set; }
    }
}
