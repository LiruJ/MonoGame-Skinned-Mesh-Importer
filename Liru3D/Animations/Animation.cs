using System.Collections.Generic;
using System.Diagnostics;

namespace Liru3D.Animations
{
    /// <summary> A collection of keyframes creating a single animation. </summary>
    /// <remarks> Note that no changes can be made to an animation at runtime. Instances of this class are loaded through assets and an AnimationPlayer instance handles runtime changes. </remarks>
    [DebuggerDisplay("{Name} with {ChannelCount} channels taking {DurationInSeconds} seconds.")]
    public class Animation
    {
        #region Properties
        /// <summary> The name of this animation. </summary>
        public string Name { get; }

        /// <summary> How many ticks (frames) long this animation is. </summary>
        public int DurationInTicks { get; }

        /// <summary> How many seconds long this animation is. </summary>
        public float DurationInSeconds => (float)DurationInTicks / TicksPerSecond;

        /// <summary> How many ticks (frames) per second this animation plays at at 100% speed. </summary>
        public int TicksPerSecond { get; }

        /// <summary> The collection of bone channels keyed by bone name. </summary>
        public IReadOnlyDictionary<string, BoneChannel> ChannelsByBoneName { get; }

        /// <summary> The number of bone channels in this animation. </summary>
        public int ChannelCount => ChannelsByBoneName.Count;
        #endregion

        #region Constructors
        /// <summary> Creates a new animation with the given data. </summary>
        /// <param name="name"> The name of the animation. </param>
        /// <param name="ticksPerSecond"> The playback speed of the animation in ticks. </param>
        /// <param name="durationInTicks"> How long the animation is in ticks. </param>
        /// <param name="channelsByBoneName"> The collection of bone channels. </param>
        public Animation(string name, int ticksPerSecond, int durationInTicks, IReadOnlyDictionary<string, BoneChannel> channelsByBoneName)
        {
            TicksPerSecond = ticksPerSecond;
            ChannelsByBoneName = channelsByBoneName;
            Name = name;
            DurationInTicks = durationInTicks;
        }
        #endregion
    }
}