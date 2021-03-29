using System;
using System.Collections.Generic;
using NuulEngine.Graphics.GraphicsUtilities;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.WIC;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;

using Direct2DFactory = SharpDX.Direct2D1.Factory;
using WriteFactory = SharpDX.DirectWrite.Factory;
using Direct2DBitmap = SharpDX.Direct2D1.Bitmap;
using Direct2DPixelFormat = SharpDX.Direct2D1.PixelFormat;
using Direct2DAlphaMode = SharpDX.Direct2D1.AlphaMode;
using Direct2DTextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode;
using WicPixelFormat = SharpDX.WIC.PixelFormat;
using Direct2DBitmapInterpolationMode = SharpDX.Direct2D1.BitmapInterpolationMode;

namespace NuulEngine.Graphics.Infrastructure
{
    internal sealed class Direct2DGraphicsContext : IDisposable
    {
        private readonly Direct3DGraphicsContext _direct3DGraphicsContext;

        private Direct2DFactory _factory;
        private WriteFactory _writeFactory;
        private ImagingFactory _imagingFactory;

        private RenderTargetProperties _renderTargetProperties;
        private RenderTarget _renderTarget;
        private RawRectangleF _renderTargetClientRectangle;

        private readonly Dictionary<string, TextFormat> _textFormats =
            new Dictionary<string, TextFormat>();

        private readonly Dictionary<string, RawColor4> _solidColorBrushColors =
            new Dictionary<string, RawColor4>();

        private readonly Dictionary<string, SolidColorBrush> _solidColorBrushes =
            new Dictionary<string, SolidColorBrush>();

        private readonly Dictionary<string, BitmapFrameDecode> _decodedBitmapFirstFrames =
            new Dictionary<string, BitmapFrameDecode>();

        private readonly Dictionary<string, Direct2DBitmap> _bitmaps =
            new Dictionary<string, Direct2DBitmap>();

        public Direct2DGraphicsContext(Game game, Direct3DGraphicsContext direct3DGraphicsContext)
        {
            _direct3DGraphicsContext = direct3DGraphicsContext;

            game.SwapChainResizing += Game_SwapChainResizing;
            game.SwapChainResized += Game_SwapChainResized;

            _factory = new Direct2DFactory();
            _writeFactory = new WriteFactory();
            _imagingFactory = new ImagingFactory();

            _renderTargetProperties = new RenderTargetProperties(
                type: RenderTargetType.Hardware,
                pixelFormat: new Direct2DPixelFormat(
                    Format.Unknown,
                    Direct2DAlphaMode.Premultiplied),
                dpiX: 0,
                dpiY: 0,
                usage: RenderTargetUsage.None,
                minLevel: FeatureLevel.Level_10
            );
        }

        public RawRectangleF RenderTargetClientRectangle { get => _renderTargetClientRectangle; }

        public void AddTextFormat(string formatName, string fontFamilyName, FontWeight fontWeight,
            FontStyle fontStyle, FontStretch fontStretch, float fontSize,
            TextAlignment textAlignment, ParagraphAlignment paragraphAlignment)
        {
            var textFormat = new TextFormat(_writeFactory, fontFamilyName, fontWeight,
                fontStyle, fontStretch, fontSize)
            {
                TextAlignment = textAlignment,
                ParagraphAlignment = paragraphAlignment,
            };
            _textFormats.Add(formatName, textFormat);
        }

        public void AddBitmap(string bitmapName, BitmapFrameDecode bitmapFrame)
        {
            _decodedBitmapFirstFrames.Add(bitmapName, bitmapFrame);
        }

        private void AddBitmap(string bitmapName)
        {
            BitmapFrameDecode decodedBitmapFirstFrame = _decodedBitmapFirstFrames[bitmapName];

            var imageFormatConverter = new FormatConverter(_imagingFactory);
            imageFormatConverter.Initialize(
                decodedBitmapFirstFrame,
                WicPixelFormat.Format32bppPRGBA,
                BitmapDitherType.Ordered4x4, null, 0.0, BitmapPaletteType.Custom);
            var bitmap = Direct2DBitmap.FromWicBitmap(_renderTarget, imageFormatConverter);

            _bitmaps.Add(bitmapName, bitmap);
            Utilities.Dispose(ref imageFormatConverter);
        }

        public void AddSolidBrushColor(string brushName, RawColor4 color)
        {
            _solidColorBrushColors.Add(brushName, color);
        }

        private void AddSolidColorBrush(string brushName)
        {
            var brush = new SolidColorBrush(_renderTarget, _solidColorBrushColors[brushName]);
            _solidColorBrushes.Add(brushName, brush);
        }

        private void Game_SwapChainResizing(object sender, EventArgs e)
        {
            DisposeUtilities.DisposeDictionaryElements(_bitmaps);
            DisposeUtilities.DisposeDictionaryElements(_solidColorBrushes);
            Utilities.Dispose(ref _renderTarget);
        }

        private void Game_SwapChainResized(object sender, EventArgs e)
        {
            Surface surface = _direct3DGraphicsContext.BackBuffer.QueryInterface<Surface>();
            _renderTarget = new RenderTarget(_factory, surface, _renderTargetProperties)
            {
                AntialiasMode = AntialiasMode.PerPrimitive,
                TextAntialiasMode = Direct2DTextAntialiasMode.Cleartype,
            };
            Utilities.Dispose(ref surface);

            _renderTargetClientRectangle = new RawRectangleF(
                left: 0,
                top: 0,
                right: _renderTarget.Size.Width,
                bottom: _renderTarget.Size.Height);

            foreach (var brushName in _solidColorBrushColors.Keys)
            {
                AddSolidColorBrush(brushName);
            }

            foreach (var bimapName in _decodedBitmapFirstFrames.Keys)
            {
                AddBitmap(bimapName);
            }
        }

        public void BeginDraw()
        {
            _renderTarget.BeginDraw();
        }

        public void DrawText(string text, string formatName, RawRectangleF layoutRectangle, string brushName)
        {
            _renderTarget.Transform = Matrix3x2.Identity;
            _renderTarget.DrawText(text, _textFormats[formatName], layoutRectangle, _solidColorBrushes[brushName]);
        }

        public void DrawBitmap(string bitmapName, Matrix3x2 transformMatrix, float opacity,
            Direct2DBitmapInterpolationMode interpolationMode)
        {
            _renderTarget.Transform = transformMatrix;
            _renderTarget.DrawBitmap(_bitmaps[bitmapName], opacity, interpolationMode);
        }

        public void EndDraw()
        {
            _renderTarget.EndDraw();
        }

        public void Dispose()
        {
            DisposeUtilities.DisposeDictionaryElements(_bitmaps);
            DisposeUtilities.DisposeDictionaryElements(_decodedBitmapFirstFrames);
            DisposeUtilities.DisposeDictionaryElements(_solidColorBrushes);
            DisposeUtilities.DisposeDictionaryElements(_textFormats);
            Utilities.Dispose(ref _renderTarget);
            Utilities.Dispose(ref _imagingFactory);
            Utilities.Dispose(ref _writeFactory);
            Utilities.Dispose(ref _factory);
        }
    }
}
