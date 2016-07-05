using System;
using System.Threading;
using SharpDX;
using SharpDX.Windows;
using SharpDX.Direct3D12;
using SharpDX.DXGI;

using Device = SharpDX.Direct3D12.Device;
using Resource = SharpDX.Direct3D12.Resource;
using HVR.Common.Objects.Game.Level;

using HVR.Common.Interfaces;
using HVR.Common.Helpers;

namespace HVR.Renderer.DX12 {
    public class MainRenderWindow : IRenderer {
        private LevelContainerItem _level;
        private ConfigHelper _cfgHelper;

        private Resource vertexBuffer; 
        private VertexBufferView vertexBufferView;
        private RootSignature rootSignature;
        private PipelineState pipelineState;
        private ViewportF viewport;
        private Rectangle scissorRect;
        
        public void Initialize(RenderForm form, Adapter selectedAdapter, LevelContainerItem level, ConfigHelper cfgHelper) {
            _level = level;
            _cfgHelper = cfgHelper;

            LoadPipeline(form, selectedAdapter);
            LoadAssets();
        }

        private void LoadPipeline(RenderForm form, Adapter selectedAdapter) {
            int width = form.ClientSize.Width;
            int height = form.ClientSize.Height;

            viewport.Width = width;
            viewport.Height = height;
            viewport.MaxDepth = 1.0f;

            scissorRect.Right = width;
            scissorRect.Bottom = height;

#if DEBUG
            // Enable the D3D12 debug layer.
            {
                DebugInterface.Get().EnableDebugLayer();
            }
#endif
            device = new Device(selectedAdapter, SharpDX.Direct3D.FeatureLevel.Level_11_0);
            using (var factory = new Factory4()) {
                // Describe and create the command queue.
                CommandQueueDescription queueDesc = new CommandQueueDescription(CommandListType.Direct);
                commandQueue = device.CreateCommandQueue(queueDesc);


                // Describe and create the swap chain.
                SwapChainDescription swapChainDesc = new SwapChainDescription() {
                    BufferCount = FrameCount,
                    ModeDescription = new ModeDescription(width, height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                    Usage = Usage.RenderTargetOutput,
                    SwapEffect = SwapEffect.FlipDiscard,
                    OutputHandle = form.Handle,
                    //Flags = SwapChainFlags.None,
                    SampleDescription = new SampleDescription(_cfgHelper.GetConfigOption(Common.Enums.ConfigOptions.SELECTED_MULTISAMPLE_VALUE), 0),
                    IsWindowed = true
                };

                SwapChain tempSwapChain = new SwapChain(factory, commandQueue, swapChainDesc);
                swapChain = tempSwapChain.QueryInterface<SwapChain3>();
                tempSwapChain.Dispose();
                frameIndex = swapChain.CurrentBackBufferIndex;
            }

            // Create descriptor heaps.
            // Describe and create a render target view (RTV) descriptor heap.
            DescriptorHeapDescription rtvHeapDesc = new DescriptorHeapDescription() {
                DescriptorCount = FrameCount,
                Flags = DescriptorHeapFlags.None,
                Type = DescriptorHeapType.RenderTargetView
            };

            renderTargetViewHeap = device.CreateDescriptorHeap(rtvHeapDesc);

            rtvDescriptorSize = device.GetDescriptorHandleIncrementSize(DescriptorHeapType.RenderTargetView);

            // Create frame resources.
            CpuDescriptorHandle rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
            for (int n = 0; n < FrameCount; n++) {
                renderTargets[n] = swapChain.GetBackBuffer<Resource>(n);
                device.CreateRenderTargetView(renderTargets[n], null, rtvHandle);
                rtvHandle += rtvDescriptorSize;
            }

            commandAllocator = device.CreateCommandAllocator(CommandListType.Direct);
        }

        private void LoadAssets() {
            var rootSignatureDesc = new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout);
            rootSignature = device.CreateRootSignature(rootSignatureDesc.Serialize());
            
            var vertexShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(PathHelper.GetPath(Common.Enums.ResourceTypes.Shaders, "vs_color.hlsl"), "VSMain", "vs_5_0", SharpDX.D3DCompiler.ShaderFlags.Debug));
            var pixelShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(PathHelper.GetPath(Common.Enums.ResourceTypes.Shaders, "ps_color.hlsl"), "PSMain", "ps_5_0", SharpDX.D3DCompiler.ShaderFlags.Debug));

            var inputElementDescs = new[]             {
                     new InputElement("POSITION",0,Format.R32G32B32_Float,0,0),
                     new InputElement("COLOR",0,Format.R32G32B32A32_Float,12,0)
             };

            // Describe and create the graphics pipeline state object (PSO). 
             var psoDesc = new GraphicsPipelineStateDescription()
             {
                                 InputLayout = new InputLayoutDescription(inputElementDescs), 
                 RootSignature = rootSignature, 
                 VertexShader = vertexShader, 
                 PixelShader = pixelShader, 
                 RasterizerState = RasterizerStateDescription.Default(), 
                 BlendState = BlendStateDescription.Default(), 
                 DepthStencilFormat = SharpDX.DXGI.Format.D32_Float, 
                 DepthStencilState = new DepthStencilStateDescription() { IsDepthEnabled = false, IsStencilEnabled = false }, 
                 SampleMask = int.MaxValue, 
                 PrimitiveTopologyType = PrimitiveTopologyType.Triangle, 
                 RenderTargetCount = 1, 
                 Flags = PipelineStateFlags.None, 
                 SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0), 
                 StreamOutput = new StreamOutputDescription()
             };

            psoDesc.RenderTargetFormats[0] = SharpDX.DXGI.Format.R8G8B8A8_UNorm;
            pipelineState = device.CreateGraphicsPipelineState(psoDesc);

            commandList = device.CreateCommandList(CommandListType.Direct, commandAllocator, pipelineState);
            
            int vertexBufferSize = Utilities.SizeOf(_level.Geometry);
            
            vertexBuffer = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(vertexBufferSize), ResourceStates.GenericRead);

            IntPtr pVertexDataBegin = vertexBuffer.Map(0);
            Utilities.Write(pVertexDataBegin, _level.Geometry, 0, _level.Geometry.Length);
            vertexBuffer.Unmap(0);

            vertexBufferView = new VertexBufferView();
            vertexBufferView.BufferLocation = vertexBuffer.GPUVirtualAddress;
            vertexBufferView.StrideInBytes = Utilities.SizeOf<LevelGeometryItem>();
            vertexBufferView.SizeInBytes = vertexBufferSize;
            
            commandList.Close();
            
            fence = device.CreateFence(0, FenceFlags.None);
            fenceValue = 1;
            
            fenceEvent = new AutoResetEvent(false);
        }

        private void PopulateCommandList() {
            // Command list allocators can only be reset when the associated 
            // command lists have finished execution on the GPU; apps should use 
            // fences to determine GPU execution progress.
            commandAllocator.Reset();

            // However, when ExecuteCommandList() is called on a particular command 
            // list, that command list can then be reset at any time and must be before 
            // re-recording.
            commandList.Reset(commandAllocator, pipelineState);

            commandList.SetGraphicsRootSignature(rootSignature);
            commandList.SetViewport(viewport);
            commandList.SetScissorRectangles(scissorRect);
            
            // Indicate that the back buffer will be used as a render target.
            commandList.ResourceBarrierTransition(renderTargets[frameIndex], ResourceStates.Present,
                ResourceStates.RenderTarget);

            var rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
            rtvHandle += frameIndex * rtvDescriptorSize;
            commandList.SetRenderTargets(rtvHandle, null);

            commandList.ClearRenderTargetView(rtvHandle, new Color4(0, 0.0F, 0.0f, 1), 0, null);

            commandList.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            commandList.SetVertexBuffer(0, vertexBufferView);
            commandList.DrawInstanced(3, 1, 0, 0);


            commandList.ResourceBarrierTransition(renderTargets[frameIndex], ResourceStates.RenderTarget, ResourceStates.Present);

            commandList.Close();
        }

        /// <summary> 
        /// Wait the previous command list to finish executing. 
        /// </summary> 
        private void WaitForPreviousFrame() {
            // WAITING FOR THE FRAME TO COMPLETE BEFORE CONTINUING IS NOT BEST PRACTICE. 
            // This is code implemented as such for simplicity. 

            int fence = fenceValue;
            commandQueue.Signal(this.fence, fence);
            fenceValue++;

            // Wait until the previous frame is finished.
            if (this.fence.CompletedValue < fence) {
                this.fence.SetEventOnCompletion(fence, fenceEvent.SafeWaitHandle.DangerousGetHandle());
                fenceEvent.WaitOne();
            }

            frameIndex = swapChain.CurrentBackBufferIndex;
        }

        public void Update() {
        }

        public void Render() {
            // Record all the commands we need to render the scene into the command list.
            PopulateCommandList();

            // Execute the command list.
            commandQueue.ExecuteCommandList(commandList);

            // Present the frame.
            swapChain.Present(1, 0);

            WaitForPreviousFrame();
        }

        public void Dispose() {
            // Wait for the GPU to be done with all resources.
            WaitForPreviousFrame();

            //release all resources
            foreach (var target in renderTargets) {
                target.Dispose();
            }
            commandAllocator.Dispose();
            commandQueue.Dispose();
            renderTargetViewHeap.Dispose();
            commandList.Dispose();
            fence.Dispose();
            swapChain.Dispose();
            device.Dispose();
        }

        const int FrameCount = 2;

        // Pipeline objects.
        private SwapChain3 swapChain;
        private Device device;
        private Resource[] renderTargets = new Resource[FrameCount];

        private CommandAllocator commandAllocator;
        private CommandQueue commandQueue;
        private DescriptorHeap renderTargetViewHeap;

        private GraphicsCommandList commandList;
        private int rtvDescriptorSize;

        // Synchronization objects.
        private int frameIndex;
        private AutoResetEvent fenceEvent;

        private Fence fence;
        private int fenceValue;
    }
}