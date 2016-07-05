using System.IO;
using System.Runtime.InteropServices;

using SharpDX.Direct3D12;
using Device = SharpDX.Direct3D12.Device;
using SharpDX.DXGI;

using HVR.Common.Helpers;

namespace HVR.Renderer.DX12.Helpers {
    public class TextureLoader {
        private SharpDX.Direct3D12.Resource texture;

        const int TextureWidth = 256;
        const int TextureHeight = 256;
        const int TexturePixelSize = 4;	// The number of bytes used to represent a pixel in the texture.
        
        public const int ComponentMappingMask = 0x7;

        public const int ComponentMappingShift = 3;

        public const int ComponentMappingAlwaysSetBitAvoidingZeromemMistakes = (1 << (ComponentMappingShift * 4));

        public int ComponentMapping(int src0, int src1, int src2, int src3) {

            return ((((src0) & ComponentMappingMask) |
            (((src1) & ComponentMappingMask) << ComponentMappingShift) |
                                                                (((src2) & ComponentMappingMask) << (ComponentMappingShift * 2)) |
                                                                (((src3) & ComponentMappingMask) << (ComponentMappingShift * 3)) |
                                                                ComponentMappingAlwaysSetBitAvoidingZeromemMistakes));
        }

        public int DefaultComponentMapping() => ComponentMapping(0, 1, 2, 3);
    
        public int ComponentMapping(int ComponentToExtract, int Mapping) => ((Mapping >> (ComponentMappingShift * ComponentToExtract) & ComponentMappingMask));

        private long GetRequiredIntermediateSize(Device device, SharpDX.Direct3D12.Resource destinationResource, int firstSubresource, int NumSubresources) {
            var desc = destinationResource.Description;
            long requiredSize;
            device.GetCopyableFootprints(ref desc, firstSubresource, NumSubresources, 0, null, null, null, out requiredSize);
            return requiredSize;
        }

        public void LoadTexture(string textureName, ref Device device, ref GraphicsCommandList commandList, ref DescriptorHeap shaderRenderViewHeap) {
                
            var textureDesc = ResourceDescription.Texture2D(Format.R8G8B8A8_UNorm, TextureWidth, TextureHeight);
            texture = device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination);

            long uploadBufferSize = GetRequiredIntermediateSize(device, this.texture, 0, 1);

            var textureUploadHeap = device.CreateCommittedResource(new HeapProperties(CpuPageProperty.WriteBack, MemoryPool.L0), HeapFlags.None, ResourceDescription.Texture2D(Format.R8G8B8A8_UNorm, TextureWidth, TextureHeight), ResourceStates.GenericRead);

            var textureData = File.ReadAllBytes(PathHelper.GetPath(Common.Enums.ResourceTypes.Textures, textureName));

            var handle = GCHandle.Alloc(textureData, GCHandleType.Pinned);
            var ptr = Marshal.UnsafeAddrOfPinnedArrayElement(textureData, 0);
            //   textureUploadHeap.WriteToSubresource(0, null, ptr, TexturePixelSize * TextureWidth, textureData.Length);
            handle.Free();

            commandList.CopyTextureRegion(new TextureCopyLocation(texture, 0), 0, 0, 0, new TextureCopyLocation(textureUploadHeap, 0), null);

            commandList.ResourceBarrierTransition(this.texture, ResourceStates.CopyDestination, ResourceStates.PixelShaderResource);

            // Describe and create a SRV for the texture.
            var srvDesc = new ShaderResourceViewDescription {
                Shader4ComponentMapping = DefaultComponentMapping(),
                Format = textureDesc.Format,
                Dimension = ShaderResourceViewDimension.Texture2D,
                Texture2D = { MipLevels = 1 },
            };

            device.CreateShaderResourceView(this.texture, srvDesc, shaderRenderViewHeap.CPUDescriptorHandleForHeapStart);

            textureUploadHeap.Dispose();
        }
    }
}