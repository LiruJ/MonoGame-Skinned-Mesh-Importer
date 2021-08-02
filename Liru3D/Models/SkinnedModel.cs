using Liru3D.Animations;
using Liru3D.Models.Data;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Liru3D.Models
{
    /// <summary> Contains a collection of meshes and animations. </summary>
    /// <remarks> Note that no changes can be made to a model, mesh, or bone at runtime. </remarks>
    public class SkinnedModel
    {
        #region Properties
        /// <summary> The collection of meshes. </summary>
        public List<SkinnedMesh> Meshes { get; }

        public List<Animation> Animations { get; }

        public int AnimationCount => Animations != null ? Animations.Count : 0;

        public IReadOnlyList<Bone> Bones { get; }

        public int BoneCount => Bones.Count;
        #endregion

        #region Constructors
        public SkinnedModel(List<SkinnedMesh> meshes, List<Animation> animations, IReadOnlyList<Bone> bones)
        {
            Meshes = meshes ?? throw new System.ArgumentNullException(nameof(meshes));
            Animations = animations;
            Bones = bones;
        }
        #endregion

        #region Creation Functions
        public static SkinnedModel CreateFrom(GraphicsDevice graphicsDevice, SkinnedModelData data)
        {
            // Create the mesh and bone collections.
            List<SkinnedMesh> meshes = new List<SkinnedMesh>(data.MeshCount);
            Bone[] bones = new Bone[data.BoneCount];

            SkinnedModel model = new SkinnedModel(meshes, data.Animations, bones);
            
            for (int meshIndex = 0; meshIndex < data.Meshes.Count; meshIndex++)
                meshes.Add(SkinnedMesh.CreateFrom(graphicsDevice, data.Meshes[meshIndex]));

            for (int boneIndex = 0; boneIndex < data.BoneCount; boneIndex++)
                bones[boneIndex] = Bone.CreateFrom(model, data.Bones[boneIndex]);

            return model;
        }
        #endregion
    }
}