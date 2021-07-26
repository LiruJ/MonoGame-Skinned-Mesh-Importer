using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Liru3D.Models.Data
{
    [DebuggerDisplay("{Name}")]
    public struct BoneData
    {
        #region Properties
        public string Name { get; set; }

        public int Index { get; set; }

        public int ParentIndex { get; set; }

        /// <summary> The offset of the bone, used when rendering. </summary>
        public Matrix Offset { get; set; }

        /// <summary> The transform of the bone relative to its parent. If this bone has no parent, then it is relative to the model. </summary>
        public Matrix LocalTransform { get; set; }
        #endregion
    }
}
