using System;
using System.Diagnostics;

namespace Liru3D.Animations
{
    /// <summary> Holds a time and a <typeparamref name="T"/> value. </summary>
    [DebuggerDisplay("Index: {Index} Tick: {TickTime} Value: {Value}")]
    public struct Keyframe<T> : IComparable<Keyframe<T>> where T : struct
    {
        #region Properties
        /// <summary> The index of this frame. </summary>
        public int Index { get; }

        /// <summary> The time of this frame in ticks. </summary>
        public int TickTime { get; }

        /// <summary> The value (rotation, scale, or position) of this frame. </summary>
        public T Value { get; }
        #endregion

        #region Constructors
        /// <summary> Creates a new keyframe with the given values. </summary>
        /// <param name="index"> The index of this frame within its collection. </param>
        /// <param name="tickTime"> The time at which this keyframe exists. </param>
        /// <param name="value"> The value of this keyframe. </param>
        public Keyframe(int index, int tickTime, T value)
        {
            TickTime = tickTime;
            Value = value;
            Index = index;
        }
        #endregion

        #region Comparison Functions
        /// <summary> Compares the <see cref="TickTime"/> of this keyframe to that of the given keyframe. </summary>
        /// <param name="other"> The keyframe to compare against. </param>
        /// <returns> The result of <see cref="int.CompareTo(int)"/>. </returns>
        public int CompareTo(Keyframe<T> other) => TickTime.CompareTo(other.TickTime);
        #endregion
    }
}