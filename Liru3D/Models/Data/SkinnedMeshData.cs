namespace Liru3D.Models.Data
{
    public struct SkinnedMeshData
    {
        #region Properties
        public string Name { get; }

        public SkinnedVertex[] Vertices { get; }

        public int VertexCount => Vertices.Length;

        public int[] Indices { get; }

        public int IndexCount => Indices.Length;
        #endregion

        #region Constructors
        public SkinnedMeshData(string name, SkinnedVertex[] vertices, int[] indices)
        {
            Name = name;
            Vertices = vertices;
            Indices = indices;
        }
        #endregion
    }
}