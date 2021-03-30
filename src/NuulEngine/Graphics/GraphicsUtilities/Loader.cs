using System;
using System.Collections.Generic;
using Assimp.Configs;
using NuulEngine.Graphics.Infrastructure;
using NuulEngine.Graphics.Infrastructure.Structs;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.WIC;

namespace NuulEngine.Graphics.GraphicsUtilities
{
    internal sealed class Loader : IDisposable
    {
        private readonly Direct3DGraphicsContext _direct3DGraphicsContext;

        private readonly SampleDescription _sampleDescription;

        private readonly SamplerState _linearSampler;
        
        private readonly Texture _defaultTexture;

        private readonly Material _defaultMaterial;
        
        private bool _isDisposed;

        private ImagingFactory _imagingFactory;

        public Loader(Direct3DGraphicsContext directX3DGraphicsContext, SamplerState pointSampler, SamplerState linearSampler)
        {
            _direct3DGraphicsContext = directX3DGraphicsContext;
            _imagingFactory = new ImagingFactory();
            _linearSampler = linearSampler;
            _sampleDescription = new SampleDescription(1, 0);
            _defaultTexture = LoadTextureFromFile(@"Assets\white.png", true, pointSampler);
            _defaultMaterial = new Material(_defaultTexture, Vector3.Zero, Vector3.One, Vector3.One, Vector3.One, 1);
        }

        public List<MeshObject> LoadMeshFromFile(string path, Vector4 startPosition, Vector3 startRotation)
        {
            var importer = new Assimp.AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            var fileScene = importer.ImportFile(path,
                Assimp.PostProcessPreset.TargetRealTimeMaximumQuality |
                Assimp.PostProcessSteps.JoinIdenticalVertices |
                Assimp.PostProcessSteps.ImproveCacheLocality |
                Assimp.PostProcessSteps.OptimizeMeshes |
                Assimp.PostProcessSteps.OptimizeGraph |
                Assimp.PostProcessSteps.RemoveRedundantMaterials |
                Assimp.PostProcessSteps.Triangulate);

            var meshes = fileScene.Meshes;
            var fileMaterials = fileScene.Materials;

            var indices = new List<uint>();
            var vertices = new List<VertexData>();
            var meshList = new List<MeshObject>();
            var materials = new List<Material>();

            foreach (var material in fileMaterials)
            {
                var emissive = Vector3.Zero;
                var ambient = Vector3.One;
                var diffuse = Vector3.One;
                var specular = Vector3.One;

                if (material.GetMaterialTexture(Assimp.TextureType.Diffuse, 0, out Assimp.TextureSlot slot))
                {
                    materials.Add(new Material(
                        LoadTextureFromFile(slot.FilePath, true, _linearSampler, 4),
                        emissive, ambient, diffuse, specular, 1));
                }
                else
                {
                    materials.Add(new Material(_defaultTexture, emissive, ambient, diffuse, specular, 1));
                }
            }

            if (materials.Count == 0)
            {
                materials.Add(_defaultMaterial);
            }

            foreach (var mesh in meshes)
            {
                vertices.Clear();
                indices.Clear();
                for (int i = 0, j = 0; i < mesh.Vertices.Count && j < mesh.TextureCoordinateChannels[0].Count; i++, j++)
                {
                    var vert = mesh.Vertices[i];
                    var tex = mesh.TextureCoordinateChannels[0][j];
                    var normals = mesh.Normals[i];

                    vertices.Add(new VertexData
                    {
                        position = new Vector4(vert.X, vert.Y, -vert.Z, 1),
                        texCoord = new Vector2(tex.X, 1 - tex.Y),
                        normal = new Vector4(normals.X, normals.Y, normals.Z, 1.0f),
                        color = Vector4.One,
                    });
                }

                foreach (var index in mesh.GetIndices())
                {
                    indices.Add((uint)index);
                }

                meshList.Add(new MeshObject(_direct3DGraphicsContext.Device, startPosition,
                    startRotation.X, startRotation.Y, startRotation.Z,
                    new Mesh(vertices.ToArray(), indices.ToArray(), PrimitiveTopology.TriangleList),
                    materials[mesh.MaterialIndex]));
            }

            return meshList;
        }

        public Texture LoadTextureFromFile(string fileName, bool generateMips, SamplerState samplerState, int mipLevels = -1)
        {
            BitmapFrameDecode bitmapFirstFrame = BitmapFrameDecode(fileName, _imagingFactory);

            FormatConverter formatConverter = new FormatConverter(_imagingFactory);
            formatConverter.Initialize(bitmapFirstFrame, PixelFormat.Format32bppRGBA,
                BitmapDitherType.None, null, 0.0f, BitmapPaletteType.Custom);

            int stride = formatConverter.Size.Width * 4;
            DataStream buffer = new DataStream(
                formatConverter.Size.Height * stride, true, true);
            formatConverter.CopyPixels(stride, buffer);

            int width = formatConverter.Size.Width;
            int height = formatConverter.Size.Height;

            Texture2DDescription texture2DDescription = new Texture2DDescription()
            {
                Width = width,
                Height = height,
                MipLevels = (generateMips ? 0 : 1),
                ArraySize = 1,
                Format = Format.R8G8B8A8_UNorm,
                SampleDescription = _sampleDescription,
                Usage = ResourceUsage.Default,
                BindFlags = (
                    generateMips ?
                    BindFlags.ShaderResource | BindFlags.RenderTarget :
                    BindFlags.ShaderResource
                    ),
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = (
                   generateMips ?
                   ResourceOptionFlags.GenerateMipMaps :
                   ResourceOptionFlags.None
                   )
            };

            Texture2D textureObject;

            if (generateMips)
                textureObject = new Texture2D(_direct3DGraphicsContext.Device, texture2DDescription);
            else
            {
                DataRectangle dataRectangle = new DataRectangle(buffer.DataPointer, stride);
                textureObject = new Texture2D(_direct3DGraphicsContext.Device, texture2DDescription, dataRectangle);
            }

            ShaderResourceViewDescription shaderResourceViewDescription =
                new ShaderResourceViewDescription()
                {
                    Dimension = ShaderResourceViewDimension.Texture2D,
                    Format = Format.R8G8B8A8_UNorm,
                    Texture2D = new ShaderResourceViewDescription.Texture2DResource
                    {
                        MostDetailedMip = 0,
                        MipLevels = (generateMips ? mipLevels : 1)
                    }
                };
            ShaderResourceView shaderResourceView =
                new ShaderResourceView(_direct3DGraphicsContext.Device, textureObject, shaderResourceViewDescription);

            if (generateMips)
            {
                DataBox dataBox = new DataBox(buffer.DataPointer, stride, 1);
                _direct3DGraphicsContext.DeviceContext.UpdateSubresource(dataBox, textureObject, 0);
                _direct3DGraphicsContext.DeviceContext.GenerateMips(shaderResourceView);
            }

            Utilities.Dispose(ref formatConverter);

            return new Texture(textureObject, width, height, shaderResourceView, samplerState);
        }

        private BitmapFrameDecode BitmapFrameDecode(string fileName, ImagingFactory imagingFactory)
        {
            var decoder = new BitmapDecoder(imagingFactory, fileName, DecodeOptions.CacheOnDemand);
            BitmapFrameDecode bitmapFirstFrame = decoder.GetFrame(0);
            Utilities.Dispose(ref decoder);
            return bitmapFirstFrame;
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Utilities.Dispose(ref _imagingFactory);
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
