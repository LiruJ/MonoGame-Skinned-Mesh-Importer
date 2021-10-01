using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Liru3D.Models
{
    /// <summary> Represents a 3D point with bone weight data to be used for skinned models. </summary>
    public struct SkinnedVertex : IVertexType
    {
        #region Backing Fields
        /// <summary> The declaration of a single vertex used when uploading vertex data to the GPU. </summary>
        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Byte4, VertexElementUsage.BlendIndices, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(32, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(44, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
            );
        #endregion

        #region Properties
        /// <summary> The layout of this vertex data, compatible with the default MonoGame SkinnedEffect. </summary>
        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        /// <summary> The position of the vertex itself. </summary>
        public Vector3 Position { get; set; }

        /// <summary> The packed bone indices that affect this vertex's position. </summary>
        public Byte4 BlendIndices { get; set; }

        /// <summary> The amount that each bone affects the final position of this vertex. </summary>
        public Vector4 BlendWeights { get; set; }

        /// <summary> The normal direction of this vertex. </summary>
        public Vector3 Normal { get; set; }

        /// <summary> The Texture co-ordinate. </summary>
        public Vector2 UV { get; set; }
        #endregion

        #region Weight Functions
        /// <summary> Calculates the total number of non-zero weights of the <see cref="BlendWeights"/>, which is the total number of bones influencing this vertex. </summary>
        /// <returns> The calculated number of bones which influence this vertex. </returns>
        public int CalculateBoneCount()
        {
            // Count the number of non-zero weights and return the result.
            int boneCount = 0;
            if (BlendWeights.X != 0) boneCount++;
            if (BlendWeights.Y != 0) boneCount++;
            if (BlendWeights.Z != 0) boneCount++;
            if (BlendWeights.W != 0) boneCount++;
            return boneCount;
        }

        /// <summary> Sets the next value of <see cref="BlendIndices"/> and <see cref="BlendWeights"/> to the given values. </summary>
        /// <param name="boneIndex"> The index of the next bone influence. </param>
        /// <param name="weight"> The weight of the next bone influence. </param>
        public void SetNextWeight(int boneIndex, float weight)
        {
            // Unpack the indices and copy the weights.
            Vector4 boneIndices = BlendIndices.ToVector4();
            Vector4 boneWeights = BlendWeights;

            // Calculate the bone count.
            int boneCount = CalculateBoneCount();

            // Set the index and weight.
            switch (boneCount)
            {
                case 0: boneIndices.X = boneIndex; boneWeights.X = weight; break;
                case 1: boneIndices.Y = boneIndex; boneWeights.Y = weight; break;
                case 2: boneIndices.Z = boneIndex; boneWeights.Z = weight; break;
                case 3: boneIndices.W = boneIndex; boneWeights.W = weight; break;
                default: throw new System.Exception("Cannot use more than 4 bones per vertex.");
            }

            // Set the blend indices and weights.
            BlendIndices = new Byte4(boneIndices);
            BlendWeights = boneWeights;
        }
        #endregion
    }
}