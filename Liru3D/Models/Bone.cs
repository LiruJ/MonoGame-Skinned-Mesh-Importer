using Liru3D.Models.Data;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace Liru3D.Models
{
    /// <summary> Represents a single bone of a model. </summary>
    [DebuggerDisplay("{Name} ({Index})")]
    public class Bone
    {
        #region Properties
        /// <summary> The name of this bone. </summary>
        public string Name { get; }

        /// <summary> The index of this bone. </summary>
        public int Index { get; }

        /// <summary> The parent of this bone, or <c>null</c> if this is the root bone. </summary>
        public Bone Parent { get; }

        /// <summary> <c>true</c> if <see cref="Parent"/> is <c>null</c>, otherwise; <c>false</c>. </summary>
        public bool HasParent => Parent != null;

        /// <summary> Converts model-space orientations into bone-space orientations. </summary>
        /// <remarks> 
        /// Basically, as the bone moves around, this helps keep track of how much the bone has moved from its default position. 
        /// If the bone is at its default position, this will be <see cref="Matrix.Identity"/> (or pretty close to it).
        /// </remarks>
        public Matrix Offset { get; }

        /// <summary> The transform of the bone relative to its parent. If this bone has no parent, then it is relative to the model. </summary>
        public Matrix LocalTransform { get; }
        #endregion

        #region Constructors
        private Bone(string name, int index, Bone parent, Matrix offset, Matrix localTransform)
        {
            // Set the data.
            Name = name;
            Index = index;
            Parent = parent;
            Offset = offset;
            LocalTransform = localTransform;
        }
        #endregion

        #region Creation Functions
        /// <summary> Creates and returns a bone created for the given <paramref name="model"/> and from the given <paramref name="data"/>. </summary>
        /// <param name="model"> The model that this bone belongs to. </param>
        /// <param name="data"> The bone's data. </param>
        /// <returns> The created bone. </returns>
        public static Bone CreateFrom(SkinnedModel model, BoneData data)
        {
            // Get the parent bone.
            Bone parentBone = data.ParentIndex >= 0 && data.ParentIndex < model.BoneCount ? model.Bones[data.ParentIndex] : null;

            // Ensure the index is correct.
            if (data.Index < 0 || data.Index >= model.BoneCount)
                throw new ArgumentException($"Bone {data.Name} has an index of {data.Index}, when there are only {model.BoneCount} bones total.");

            // Create and return the bone.
            return new Bone(data.Name, data.Index, parentBone, data.Offset, data.LocalTransform);
        }
        #endregion
    }
}