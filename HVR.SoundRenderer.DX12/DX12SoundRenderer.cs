﻿using System.IO;

using HVR.Common.Interfaces;
using HVR.Common.Objects.Global;

using SharpDX.XAudio2;
using SharpDX.Multimedia;

namespace HVR.SoundRenderer.DX12 {
    public class DX12SoundRenderer : ISoundRenderer {
        private XAudio2 _xaudio2;
        private MasteringVoice _masteringVoice;

        public void Init() {
            _xaudio2 = new XAudio2();
            _masteringVoice = new MasteringVoice(_xaudio2);
        }

        public void DeInit() {
            _masteringVoice.Dispose();
            _xaudio2.Dispose();
        }

        public void Play(SoundItem soundItem, bool loop = false) {
            var stream = new SoundStream(File.OpenRead(soundItem.FileName));

            var waveFormat = stream.Format;
            var buffer = new AudioBuffer {
                Stream = stream.ToDataStream(),
                AudioBytes = (int)stream.Length,
                Flags = BufferFlags.EndOfStream
            };

            var sourceVoice = new SourceVoice(_xaudio2, waveFormat, true);

            sourceVoice.SubmitSourceBuffer(buffer, stream.DecodedPacketsInfo);
            sourceVoice.Start();
        }
    }
}