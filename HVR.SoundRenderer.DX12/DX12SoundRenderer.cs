using System.IO;

using HVR.Common.Interfaces;
using HVR.Common.Objects.Global;

using SharpDX.XAudio2;
using SharpDX.Multimedia;
using System;
using System.Collections.Generic;

namespace HVR.SoundRenderer.DX12 {
    public class DX12SoundRenderer : ISoundRenderer {
        private XAudio2 _xaudio2;
        private MasteringVoice _masteringVoice;

        private Dictionary<string, SourceVoice> _cachedSounds;

        public DX12SoundRenderer() {
            _xaudio2 = new XAudio2();
            _masteringVoice = new MasteringVoice(_xaudio2);

            _cachedSounds = new Dictionary<string, SourceVoice>();
        }

        public void ClearCache() {
            _cachedSounds.Clear();
        }

        public void Play(SoundItem soundItem, bool cached = true, bool loop = false) {
            if (_cachedSounds.ContainsKey(soundItem.FileName)) {
                _cachedSounds[soundItem.FileName].Start();
                return;
            }

            var stream = new SoundStream(File.OpenRead(soundItem.FileName));

            var waveFormat = stream.Format;
            var buffer = new AudioBuffer {
                Stream = stream.ToDataStream(),
                AudioBytes = (int)stream.Length,
                Flags = BufferFlags.EndOfStream
            };

            var sourceVoice = new SourceVoice(_xaudio2, waveFormat, false);

            sourceVoice.SubmitSourceBuffer(buffer, stream.DecodedPacketsInfo);
            
            sourceVoice.Start();

            _cachedSounds.Add(soundItem.FileName, sourceVoice);
        }

        void IDisposable.Dispose() {
            _masteringVoice.Dispose();
            _xaudio2.Dispose();
        }
    }
}