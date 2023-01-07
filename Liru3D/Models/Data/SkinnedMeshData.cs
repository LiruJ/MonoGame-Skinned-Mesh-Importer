using Microsoft.Xna.Framework;
using System.Linq;

namespace Liru3D.Models.Data
{
    /// <summary> Represents the data of a single skinned mesh residing within RAM (has not been uploaded to the graphics device). </summary>
    public readonly struct SkinnedMeshData
    {
        #region Properties
        /// <summary> The name of the mesh. </summary>
        public string Name { get; }

        /// <summary> The collection of vertices. Each vertex within this collection holds multiple pieces of data, see <see cref="SkinnedVertex"/> for more. </summary>
        public SkinnedVertex[] Vertices { get; }

        /// <summary> The number of vertices in this data. </summary>
        public int VertexCount => Vertices == null ? 0 : Vertices.Length;

        /// <summary> The collection of indices. </summary>
        public int[] Indices { get; }

        /// <summary> The number of indices in this data. </summary>
        public int IndexCount => Indices == null ? 0 : Indices.Length;
        #endregion

        #region Constructors
        /// <summary> Creates a new data with the given name and collections. </summary>
        /// <param name="name"> The name of the mesh. </param>
        /// <param name="vertices"> The collection of vertices. </param>
        /// <param name="indices"> The collection of indices. </param>
        public SkinnedMeshData(string name, SkinnedVertex[] vertices, int[] indices)
        {
            Name = name;
            Vertices = vertices;
            Indices = indices;
        }
        #endregion

        #region Bounding Functions
        /// <summary> Calculates a bounding sphere for the data's vertices. </summary>
        /// <returns> The calculated bounding sphere. </returns>
        public BoundingSphere CalculateBoundingSphere() => VertexCount == 0 ? new BoundingSphere() : BoundingSphere.CreateFromPoints(Vertices.Select(v => v.Position));
        #endregion
    }
}