using Liru3D.Animations;
using System.Collections.Generic;

namespace Liru3D.Models.Data
{
    /// <summary> Represents the raw data of a model, residing within RAM (not yet uploaded to the graphics device). </summary>
    public struct SkinnedModelData
    {
        #region Properties
        /// <summary> The collection of mesh data. </summary>
        public IReadOnlyList<SkinnedMeshData> Meshes { get; }

        /// <summary> The number of meshes in this data. </summary>
        public int MeshCount => Meshes.Count;

        /// <summary> The collection of animations. </summary>
        public List<Animation> Animations { get; }

        /// <summary> The number of animations in this data. </summary>
        public int AnimationCount => Animations.Count;

        /// <summary> The collection of bone data. </summary>
        public IReadOnlyList<BoneData> Bones { get; }

        /// <summary> The number of bones in this data. </summary>
        public int BoneCount => Bones.Count;
        #endregion

        #region Constructors
        /// <summary> Creates a new model data with the given collections. </summary>
        /// <param name="meshes"> The collection of mesh data. </param>
        /// <param name="animations"> The collection of animations. </param>
        /// <param name="bones"> The collection of bones. </param>
        public SkinnedModelData(IReadOnlyList<SkinnedMeshData> meshes, List<Animation> animations, IReadOnlyList<BoneData> bones)
        {
            Meshes = meshes ?? throw new System.ArgumentNullException(nameof(meshes));
            Animations = animations ?? throw new System.ArgumentNullException(nameof(animations));
            Bones = bones;
        }
        #endregion
    }
}