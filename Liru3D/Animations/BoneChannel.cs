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
        /// <summary> The name of the bone that this channel is for. </summary>
        public string BoneName { get; }

        /// <summary> The scales channel for the bone. </summary>
        public ChannelComponent<Vector3> Scales { get; }

        /// <summary> The rotations channel for the bone. </summary>
        public ChannelComponent<Quaternion> Rotations { get; }

        /// <summary> The positions channel for the bone. </summary>
        public ChannelComponent<Vector3> Positions { get; }
        #endregion

        #region Constructors
        /// <summary> Creates a new channel with the given animation parameters. </summary>
        /// <param name="boneName"> The name of the bone. </param>
        /// <param name="scaleFrames"> The scale frames. </param>
        /// <param name="rotationFrames"> The rotation frames. </param>
        /// <param name="positionFrames"> The position frames. </param>
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