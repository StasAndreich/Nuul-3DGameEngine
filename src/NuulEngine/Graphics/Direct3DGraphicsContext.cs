using System;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;

namespace NuulEngine.Graphics.Infrastructure
{
    internal sealed class Direct3DGraphicsContext : IDisposable
    {
        private bool _isDisposed;

        private bool _isFullScreen;

        private DepthStencilView _depthStencilView;

        private SharpDX.Direct3D11.Device _device;

        private readonly RenderForm _renderForm;

        private RenderTargetView _renderTargetView;

        private SwapChain _swapChain;

        private SwapChainDescription _swapChainDescription;

        private Texture2D _backBuffer;

        private Texture2D _depthStencilBuffer;

        private Texture2DDescription _depthStencilBufferDescription;

        public Direct3DGraphicsContext(RenderForm renderForm)
        {
            _renderForm = renderForm;
            var sampleDescription = new SampleDescription(
                count: 4,
                quality: (int) StandardMultisampleQualityLevels.StandardMultisamplePattern);

            _swapChainDescription = new SwapChainDescription
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(
                    width: _renderForm.Width,
                    height: _renderForm.Height,
                    refreshRate: new Rational(60, 1),
                    format: Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = renderForm.Handle,
                SampleDescription = sampleDescription,
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput,
            };

            SharpDX.Direct3D11.Device.CreateWithSwapChain(
                DriverType.Hardware,
                DeviceCreationFlags.BgraSupport,
                _swapChainDescription,
                out _device,
                out _swapChain);

            SetDeviceRasterizerState();
            MakeAssosiationWithRenderForm();

            _depthStencilBufferDescription = new Texture2DDescription
            {
                Format = Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = _renderForm.ClientSize.Width,
                Height = _renderForm.ClientSize.Height,
                SampleDescription = sampleDescription,
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
            };
        }

        public bool IsFullScreen
        {
            get => _isFullScreen;
            set
            {
                if (value != _isFullScreen)
                {
                    _isFullScreen = value;
                    _swapChain.SetFullscreenState(_isFullScreen, null);
                }
            }
        }

        public SharpDX.Direct3D11.Device Device { get => _device; }

        public DeviceContext DeviceContext { get => _device.ImmediateContext; }

        public SwapChain SwapChain { get => _swapChain; }

        public void ClearBuffers(Color backgroundColor)
        {
            _device.ImmediateContext.ClearDepthStencilView(
                depthStencilViewRef: _depthStencilView,
                clearFlags: DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil,
                depth: 1f,
                stencil: 0);

            _device.ImmediateContext.ClearRenderTargetView(
                renderTargetViewRef: _renderTargetView,
                colorRGBA: backgroundColor);
        }

        public void Resize()
        {
            Utilities.Dispose(ref _depthStencilView);
            Utilities.Dispose(ref _depthStencilBuffer);
            Utilities.Dispose(ref _renderTargetView);
            Utilities.Dispose(ref _backBuffer);

            _swapChain.ResizeBuffers(
                bufferCount: _swapChainDescription.BufferCount,
                width: _renderForm.ClientSize.Width,
                height: _renderForm.ClientSize.Height,
                newFormat: Format.Unknown,
                swapChainFlags: SwapChainFlags.None);

            _backBuffer = SharpDX.Direct3D11.Resource
                .FromSwapChain<Texture2D>(_swapChain, 0);
            _renderTargetView = new RenderTargetView(_device, _backBuffer);

            _depthStencilBufferDescription.Width = _renderForm.ClientSize.Width;
            _depthStencilBufferDescription.Height = _renderForm.ClientSize.Height;
            _depthStencilBuffer = new Texture2D(_device, _depthStencilBufferDescription);
            _depthStencilView = new DepthStencilView(_device, _depthStencilBuffer);

            _device.ImmediateContext.Rasterizer.SetViewport(
                new Viewport(
                    x: 0,
                    y: 0,
                    width: _renderForm.ClientSize.Width,
                    height: _renderForm.ClientSize.Height, 
                    minDepth: 0f,
                    maxDepth: 1f));

            _device.ImmediateContext.OutputMerger.SetTargets(_depthStencilView, _renderTargetView);
        }

        private void MakeAssosiationWithRenderForm()
        {
            _swapChain.GetParent<Factory>()
                .MakeWindowAssociation(_renderForm.Handle, WindowAssociationFlags.IgnoreAll);
        }

        private void SetDeviceRasterizerState()
        {
            var rasterizerStateDescription = new RasterizerStateDescription
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.Back,
                IsFrontCounterClockwise = true,
                IsMultisampleEnabled = true,
                IsAntialiasedLineEnabled = true,
                IsDepthClipEnabled = true,
            };

            _device.ImmediateContext.Rasterizer.State =
                new RasterizerState(_device, rasterizerStateDescription);
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Utilities.Dispose(ref _depthStencilView);
                    Utilities.Dispose(ref _depthStencilBuffer);
                    Utilities.Dispose(ref _renderTargetView);
                    Utilities.Dispose(ref _backBuffer);
                    Utilities.Dispose(ref _swapChain);
                    Utilities.Dispose(ref _device);
                }

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
