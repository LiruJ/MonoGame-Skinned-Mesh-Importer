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

        /// <summary> The collection of animations loaded from this model. </summary>
        public List<Animation> Animations { get; }

        /// <summary> Gets the number of animations that exist in this model. </summary>
        public int AnimationCount => Animations != null ? Animations.Count : 0;

        /// <summary> Gets this model's <see cref="Bone"/>s. </summary>
        public IReadOnlyList<Bone> Bones { get; }

        /// <summary> Gets the number of bones that this model has. </summary>
        public int BoneCount => Bones.Count;
        #endregion

        #region Constructors
        /// <summary> Creates a skinned model from the given data. </summary>
        /// <param name="meshes"> The meshes. This cannot be null. </param>
        /// <param name="animations"> The animations, this may be null. </param>
        /// <param name="bones"> The bones. This cannot be null. </param>
        public SkinnedModel(List<SkinnedMesh> meshes, List<Animation> animations, IReadOnlyList<Bone> bones)
        {
            Meshes = meshes ?? throw new System.ArgumentNullException(nameof(meshes));
            Animations = animations;
            Bones = bones ?? throw new System.ArgumentNullException(nameof(bones));
        }
        #endregion

        #region Creation Functions
        /// <summary> Creates and returns a skinned model from the given <paramref name="data"/> and uploaded onto the given <paramref name="graphicsDevice"/>. </summary>
        /// <param name="graphicsDevice"> The <see cref="GraphicsDevice"/> where the model should exist. </param>
        /// <param name="data"> The model data. </param>
        /// <returns> The loaded skinned model. </returns>
        public static SkinnedModel CreateFrom(GraphicsDevice graphicsDevice, SkinnedModelData data)
        {
            // Create the mesh and bone collections.
            List<SkinnedMesh> meshes = new List<SkinnedMesh>(data.MeshCount);
            Bone[] bones = new Bone[data.BoneCount];

            // Create the model with references to the collections.
            SkinnedModel model = new SkinnedModel(meshes, data.Animations, bones);
            
            // Populate the collections.
            for (int meshIndex = 0; meshIndex < data.Meshes.Count; meshIndex++)
                meshes.Add(SkinnedMesh.CreateFrom(graphicsDevice, data.Meshes[meshIndex]));

            for (int boneIndex = 0; boneIndex < data.BoneCount; boneIndex++)
                bones[boneIndex] = Bone.CreateFrom(model, data.Bones[boneIndex]);

            // Return the created and initialised model.
            return model;
        }
        #endregion
    }
}