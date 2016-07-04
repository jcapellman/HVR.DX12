using HVR.Common.Objects.Global;
using System;

namespace HVR.Common.Interfaces {
    public interface ISoundRenderer : IDisposable {
        void Play(SoundItem soundItem, bool loop = false);
    }
}