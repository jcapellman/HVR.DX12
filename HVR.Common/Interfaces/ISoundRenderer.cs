using HVR.Common.Objects.Global;

namespace HVR.Common.Interfaces {
    public interface ISoundRenderer {
        void Init();

        void DeInit();

        void Play(SoundItem soundItem, bool loop = false);
    }
}