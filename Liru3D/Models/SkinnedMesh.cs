using Liru3D.Models.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Liru3D.Models
{
    /// <summary> A single mesh of a <see cref="SkinnedModel"/>. </summary>
    public class SkinnedMesh
    {
        #region Dependencies
        private readonly GraphicsDevice graphicsDevice;
        #endregion

        #region Properties
        /// <summary> The name of the mesh. </summary>
        public string Name { get; private set; }

        /// <summary> The vertex buffer object that contains the vertex data of this mesh. </summary>
        public VertexBuffer VertexBuffer { get; }

        /// <summary> The index buffer object that contains the index data of this mesh. </summary>
        public IndexBuffer IndexBuffer { get; }

        /// <summary> The bounding sphere of the mesh without any animations applied. </summary>
        public BoundingSphere BoundingSphere { get; private set; }
        #endregion

        #region Constructors
        private SkinnedMesh(GraphicsDevice graphicsDevice, VertexBuffer vertexBuffer, IndexBuffer indexBuffer, BoundingSphere boundingSphere, string name)
        {
            this.graphicsDevice = graphicsDevice ?? throw new System.ArgumentNullException(nameof(graphicsDevice));
            VertexBuffer = vertexBuffer ?? throw new System.ArgumentNullException(nameof(vertexBuffer));
            IndexBuffer = indexBuffer ?? throw new System.ArgumentNullException(nameof(indexBuffer));
            BoundingSphere = boundingSphere;
            Name = name;
        }
        #endregion

        #region Data Functions
        /// <summary>
        /// Updates this mesh's data from the given data.
        /// </summary>
        /// <param name="data"> The data object holding the new data to use. </param>
        public void UpdateDataFrom(SkinnedMeshData data)
        {
            // Set the data from the given data object.
            Name = data.Name ?? Name;
            if (data.VertexCount > 0)
            {
                VertexBuffer.SetData(data.Vertices);
                BoundingSphere = data.CalculateBoundingSphere();
            }
            if (data.IndexCount > 0) IndexBuffer.SetData(data.Indices);
        }
        #endregion

        #region Creation Functions
        /// <summary> Creates and returns a new skinned mesh from the given <paramref name="data"/>, uploaded onto the given <paramref name="graphicsDevice"/>. </summary>
        /// <param name="graphicsDevice"> The graphics device onto which the mesh will be uploaded. </param>
        /// <param name="data"> The mesh data. </param>
        /// <returns> The created skinned mesh. </returns>
        public static SkinnedMesh CreateFrom(GraphicsDevice graphicsDevice, SkinnedMeshData data)
        {
            // Create a vertex buffer.
            VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice, SkinnedVertex.VertexDeclaration, data.Vertices.Length * SkinnedVertex.VertexDeclaration.VertexStride, BufferUsage.None);
            vertexBuffer.SetData(0, data.Vertices, 0, data.Vertices.Length, SkinnedVertex.VertexDeclaration.VertexStride);

            // Create an index buffer.
            IndexBuffer indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, data.Indices.Length, BufferUsage.None);
            indexBuffer.SetData(data.Indices);

            // Create the skinned mesh using the created data.
            SkinnedMesh skinnedMesh = new SkinnedMesh(graphicsDevice, vertexBuffer, indexBuffer, data.CalculateBoundingSphere(), data.Name);

            // Return the created mesh.
            return skinnedMesh;
        }
        #endregion

        #region Draw Functions
        /// <summary> Draws this mesh. </summary>
        public void Draw()
        {
            graphicsDevice.SetVertexBuffer(VertexBuffer);
            graphicsDevice.Indices = IndexBuffer;
            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, VertexBuffer.VertexCount);
        }
        #endregion
    }
}