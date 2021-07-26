using Liru3D.Models.Data;
using Microsoft.Xna.Framework.Graphics;

namespace Liru3D.Models
{
    public class SkinnedMesh
    {
        #region Dependencies
        private readonly GraphicsDevice graphicsDevice;
        #endregion

        #region Properties
        public string Name { get; }

        public VertexBuffer VertexBuffer { get; }

        public IndexBuffer IndexBuffer { get; }
        #endregion

        #region Constructors
        private SkinnedMesh(GraphicsDevice graphicsDevice, VertexBuffer vertexBuffer, IndexBuffer indexBuffer, string name)
        {
            this.graphicsDevice = graphicsDevice ?? throw new System.ArgumentNullException(nameof(graphicsDevice));
            VertexBuffer = vertexBuffer ?? throw new System.ArgumentNullException(nameof(vertexBuffer));
            IndexBuffer = indexBuffer ?? throw new System.ArgumentNullException(nameof(indexBuffer));
            Name = name;
        }
        #endregion

        #region Creation Functions
        public static SkinnedMesh CreateFrom(GraphicsDevice graphicsDevice, SkinnedMeshData data)
        {
            // Create a vertex buffer.
            VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice, SkinnedVertex.VertexDeclaration, data.Vertices.Length * SkinnedVertex.VertexDeclaration.VertexStride, BufferUsage.WriteOnly);
            vertexBuffer.SetData(0, data.Vertices, 0, data.Vertices.Length, SkinnedVertex.VertexDeclaration.VertexStride);

            // Create an index buffer.
            IndexBuffer indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, data.Indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(data.Indices);

            // Create the skinned mesh using the created data.
            SkinnedMesh skinnedMesh = new SkinnedMesh(graphicsDevice, vertexBuffer, indexBuffer, data.Name);

            // Return the created mesh.
            return skinnedMesh;
        }
        #endregion

        #region Draw Functions
        public void Draw()
        {
            graphicsDevice.SetVertexBuffer(VertexBuffer);
            graphicsDevice.Indices = IndexBuffer;
            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, VertexBuffer.VertexCount);
        }
        #endregion
    }
}
