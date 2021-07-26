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

        public int DurationInTicks { get; }

        public float DurationInSeconds => (float)DurationInTicks / TicksPerSecond;

        public int TicksPerSecond { get; }

        public IReadOnlyDictionary<string, BoneChannel> ChannelsByBoneName { get; }

        public int ChannelCount => ChannelsByBoneName.Count;
        #endregion

        #region Constructors
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