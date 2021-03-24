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

        private DeviceContext _deviceContext;

        private Factory _factory;

        private RasterizerState _rasterizerState;

        private RasterizerStateDescription _rasterizerStateDescription;

        private readonly RenderForm _renderForm;

        private RenderTargetView _renderTargetView;

        private SampleDescription _sampleDescription;

        private SwapChain _swapChain;

        private SwapChainDescription _swapChainDescription;

        private Texture2D _backBuffer;

        private Texture2D _depthStencilBuffer;

        private Texture2DDescription _depthStencilBufferDescription;

        public Direct3DGraphicsContext(RenderForm renderForm)
        {
            _renderForm = renderForm;
            _sampleDescription = new SampleDescription(
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
                SampleDescription = _sampleDescription,
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput,
            };

            SharpDX.Direct3D11.Device.CreateWithSwapChain(
                DriverType.Hardware,
                DeviceCreationFlags.BgraSupport,
                _swapChainDescription,
                out _device,
                out _swapChain);

            _deviceContext = _device.ImmediateContext;

            _rasterizerStateDescription = new RasterizerStateDescription
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.Back,
                IsFrontCounterClockwise = true,
                IsMultisampleEnabled = true,
                IsAntialiasedLineEnabled = true,
                IsDepthClipEnabled = true,
            };

            _rasterizerState = new RasterizerState(_device, _rasterizerStateDescription);
            _deviceContext.Rasterizer.State = _rasterizerState;

            _factory = _swapChain.GetParent<Factory>();
            _factory.MakeWindowAssociation(_renderForm.Handle, WindowAssociationFlags.IgnoreAll);

            _depthStencilBufferDescription = new Texture2DDescription
            {
                Format = Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = _renderForm.ClientSize.Width,
                Height = _renderForm.ClientSize.Height,
                SampleDescription = _sampleDescription,
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

        public DeviceContext DeviceContext { get => _deviceContext; }

        public SwapChain SwapChain { get => _swapChain; }

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

            _backBuffer = Texture2D.FromSwapChain<Texture2D>(_swapChain, 0);

            _renderTargetView = new RenderTargetView(_device, _backBuffer);

            _depthStencilBufferDescription.Width = _renderForm.ClientSize.Width;
            _depthStencilBufferDescription.Height = _renderForm.ClientSize.Height;
            _depthStencilBuffer = new Texture2D(_device, _depthStencilBufferDescription);
            _depthStencilView = new DepthStencilView(_device, _depthStencilBuffer);

            _deviceContext.Rasterizer.SetViewport(
                new Viewport(
                    x: 0,
                    y: 0,
                    width: _renderForm.ClientSize.Width,
                    height: _renderForm.ClientSize.Height, 
                    minDepth: 0f,
                    maxDepth: 1f));

            _deviceContext.OutputMerger.SetTargets(_depthStencilView, _renderTargetView);
        }

        public void ClearBuffers(Color backgroundColor)
        {
            _deviceContext.ClearDepthStencilView(
                depthStencilViewRef: _depthStencilView,
                clearFlags: DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil,
                depth: 1f,
                stencil: 0);

            _deviceContext.ClearRenderTargetView(
                renderTargetViewRef: _renderTargetView,
                colorRGBA: backgroundColor);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Utilities.Dispose(ref _depthStencilView);
                    Utilities.Dispose(ref _depthStencilBuffer);
                    Utilities.Dispose(ref _renderTargetView);
                    Utilities.Dispose(ref _backBuffer);
                    Utilities.Dispose(ref _factory);
                    Utilities.Dispose(ref _rasterizerState);
                    Utilities.Dispose(ref _deviceContext);
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
