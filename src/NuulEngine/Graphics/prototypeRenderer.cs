using System;
using SharpDX.Windows;
using NuulEngine.Graphics.Infrastructure;
using NuulEngine.Graphics.Cameras;

namespace NuulEngine.Graphics
{
    internal sealed class prototypeRenderer
    {
        private RenderForm _renderForm;
        // Not init.
        private Camera _camera;

        private Direct3DGraphicsContext _directX3DGraphics;

        private Direct2DGraphicsContext _directX2DGraphics;

        private GraphicsRenderer _graphicsRenderer;

        public event EventHandler SwapChainResized;

        public event EventHandler SwapChainResizing;

        public prototypeRenderer(RenderForm renderForm)
        {
            _renderForm = renderForm;
            _renderForm.UserResized += OnRenderFormResized;
            _directX3DGraphics = new Direct3DGraphicsContext(_renderForm);
            _graphicsRenderer = new GraphicsRenderer(_directX3DGraphics);
        }

        public void RunRenderLoop(RenderLoop.RenderCallback renderCallback)
        {
            RenderLoop.Run(_renderForm, renderCallback);
        }

        public void RenderScene()
        {
            //Matrix viewMatrix = _camera.GetViewMatrix();
            //Matrix projectionMatrix = _camera.GetPojectionMatrix();

            //_graphicsRenderer.UpdatePerFrameConstantBuffers(_timeHelper.Time);

            //_illumination.eyePosition = _camera.Position;
            //_graphicsRenderer.UpdateIllumination(_illumination);

            //_graphicsRenderer.BeginRender();

            //for (int i = 0; i < _scene.Meshes.Count; i++)
            //{
            //    MeshObject mesh = _scene.Meshes[i];
            //    _graphicsRenderer.UpdatePerObjectConstantBuffers(
            //        mesh.GetWorldMatrix(), viewMatrix, projectionMatrix,
            //        (mesh == _cube ? 1 : 0), mesh.ObjectType);
            //    _graphicsRenderer.RenderMeshObject(mesh);
            //}

            //_headUpDisplay.Render(_timeHelper.FPS, _timeHelper.Time, _timeHelper.DeltaT, false);

            //_graphicsRenderer.EndRender();
        }

        private void OnRenderFormResized(object sender, EventArgs args)
        {
            SwapChainResizing?.Invoke(this, null);
            _directX3DGraphics.Resize();
            _camera.AspectRatio = _renderForm.ClientSize.Width
                / (float)_renderForm.ClientSize.Height;
            SwapChainResized?.Invoke(this, null);
        }
    }
}
