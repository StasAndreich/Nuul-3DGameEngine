using System.Collections.Generic;
using NuulEngine.Graphics.Infrastructure;
using NuulEngine.Graphics.Infrastructure.Structs;
using SharpDX;
using SharpDX.Direct3D;

namespace NuulEngine.Graphics.GraphicsUtilities
{
    internal sealed class Loader
    {
        public IEnumerable<Mesh> LoadMeshesFromFile(string fileName)
        {
            var importer = new Assimp.AssimpContext();
            Assimp.Scene scene = importer.ImportFile(fileName,
                Assimp.PostProcessPreset.TargetRealTimeMaximumQuality);
            List<Assimp.Mesh> meshes = scene.Meshes;

            var result = new List<Mesh>();

            foreach (Assimp.Mesh mesh in meshes)
            {
                var indices = new List<uint>();
                var vertices = new List<VertexDataStruct>();

                for (int i = 0, j = 0;
                    i < mesh.Vertices.Count
                        && j < mesh.TextureCoordinateChannels[0].Count;
                    i++, j++)
                {
                    var vertex = mesh.Vertices[i];
                    var texCoords = mesh.TextureCoordinateChannels[0][j];
                    var normals = mesh.Normals[i];

                    vertices.Add(new VertexDataStruct
                    {
                        position = new Vector4(vertex.X, vertex.Y, vertex.Z, 1),
                        texCoord = new Vector2(texCoords.X, texCoords.Z),
                        normal = new Vector4(normals.X, normals.Y, normals.Z, 1.0f),
                        color = Vector4.One
                    });
                }

                foreach (var index in mesh.GetIndices())
                {
                    indices.Add((uint)index);
                }

                result.Add(new Mesh(
                    vertices: vertices.ToArray(),
                    indices: indices.ToArray(),
                    primitiveTopology: PrimitiveTopology.TriangleList));
            }

            return result;
        }
    }
}
