using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace Liru3D.Animations
{
    /// <summary> Hey I'm Laura from Lovebirb, and you're watching the bone channel. This class holds collections of scale, rotation, and position frames for a specific bone. </summary>
    [DebuggerDisplay("Channel for {BoneName} bone with {Scales.Count} scales, {Rotations.Count} rotations, and {Positions.Count} positions")]
    public class BoneChannel
    {
        #region Properties
        /// <summary> The name of the bone that this animation represents. </summary>
        public string BoneName { get; }

        public ChannelComponent<Vector3> Scales { get; }

        public ChannelComponent<Quaternion> Rotations { get; }

        public ChannelComponent<Vector3> Positions { get; }
        #endregion

        #region Constructors
        public BoneChannel(string boneName, IReadOnlyList<Keyframe<Vector3>> scaleFrames, IReadOnlyList<Keyframe<Quaternion>> rotationFrames, IReadOnlyList<Keyframe<Vector3>> positionFrames)
        {
            // Set the name.
            BoneName = boneName;
            
            // Create the channels.
            Scales = new ChannelComponent<Vector3>(scaleFrames);
            Rotations = new ChannelComponent<Quaternion>(rotationFrames);
            Positions = new ChannelComponent<Vector3>(positionFrames);
        }
        #endregion
    }
}