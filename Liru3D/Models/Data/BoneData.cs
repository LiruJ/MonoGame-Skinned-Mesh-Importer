using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Liru3D.Models.Data
{
    /// <summary> Represents all the data of a bone loaded from a 3D model. </summary>
    [DebuggerDisplay("{Name}")]
    public struct BoneData
    {
        #region Properties
        /// <summary> The name of the bone. </summary>
        public string Name { get; set; }

        /// <summary> the index of this bone within the model. </summary>
        public int Index { get; set; }

        /// <summary> The index of this bone's parent bone within the model. </summary>
        public int ParentIndex { get; set; }

        /// <summary> The offset of the bone, used when rendering. </summary>
        public Matrix Offset { get; set; }

        /// <summary> The transform of the bone relative to its parent. If this bone has no parent, then it is relative to the model. </summary>
        public Matrix LocalTransform { get; set; }
        #endregion
    }
}
