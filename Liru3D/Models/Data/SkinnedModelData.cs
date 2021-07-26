using Liru3D.Animations;
using System.Collections.Generic;

namespace Liru3D.Models.Data
{
    public struct SkinnedModelData
    {
        #region Properties
        /// <summary> The collection of meshes. </summary>
        public IReadOnlyList<SkinnedMeshData> Meshes { get; }

        public int MeshCount => Meshes.Count;

        public List<Animation> Animations { get; }

        public int AnimationCount => Animations.Count;

        public IReadOnlyList<BoneData> Bones { get; }

        public int BoneCount => Bones.Count;
        #endregion

        #region Constructors
        public SkinnedModelData(IReadOnlyList<SkinnedMeshData> meshes, List<Animation> animations, IReadOnlyList<BoneData> bones)
        {
            Meshes = meshes ?? throw new System.ArgumentNullException(nameof(meshes));
            Animations = animations ?? throw new System.ArgumentNullException(nameof(animations));
            Bones = bones;
        }
        #endregion
    }
}
