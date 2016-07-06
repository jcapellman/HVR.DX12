using System;
using System.Collections.Generic;
using System.IO;

using HVR.Common.Interfaces;
using HVR.Common.Objects.Global;

using SharpDX.XAudio2;
using SharpDX.Multimedia;

namespace HVR.SoundRenderer.DX12 {
    public class DX12SoundRenderer : ISoundRenderer {
        private XAudio2 _xaudio2;
        private MasteringVoice _masteringVoice;

        private Dictionary<string, AudioWrapper> _cachedSounds;

        public struct AudioWrapper {
            public AudioBuffer Buffer { get; set; }
            public WaveFormat Format { get; internal set; }
            public uint[] PacketInfo { get; internal set; }
        };

        public DX12SoundRenderer() {
            _xaudio2 = new XAudio2();
            _masteringVoice = new MasteringVoice(_xaudio2);

            _cachedSounds = new Dictionary<string, AudioWrapper>();
        }

        public void ClearCache() {
            _cachedSounds.Clear();
        }

        private AudioWrapper getAudioWrapper(string fileName) {
            var stream = new SoundStream(File.OpenRead(fileName));

            var waveFormat = stream.Format;

            var buffer = new AudioBuffer {
                Stream = stream.ToDataStream(),
                AudioBytes = (int)stream.Length,
                Flags = BufferFlags.EndOfStream
            };

            return new AudioWrapper {
                Buffer = buffer,
                Format = waveFormat,
                PacketInfo = stream.DecodedPacketsInfo
            };
        }

        public void Play(SoundItem soundItem, bool cached = true, bool loop = false) {
            AudioWrapper wrapper;

            if (_cachedSounds.ContainsKey(soundItem.FileName)) {
                wrapper = _cachedSounds[soundItem.FileName];
            } else {
                wrapper = getAudioWrapper(soundItem.FileName);

                _cachedSounds.Add(soundItem.FileName, wrapper);
            }

            var sourceVoice = new SourceVoice(_xaudio2, wrapper.Format, false);
            
            sourceVoice.SubmitSourceBuffer(wrapper.Buffer, wrapper.PacketInfo);
            
            sourceVoice.Start();
        }

        void IDisposable.Dispose() {
            _masteringVoice.Dispose();
            
            _xaudio2.Dispose();
        }
    }
}