using System;
using System.Collections.Generic;

namespace Liru3D.Animations
{
    /// <summary> Holds a collection of <see cref="Keyframe{T}"/>s and handles their retrieval in a performant way suited to interpolation. </summary>
    /// <typeparam name="T"> The type of value that is stored in each keyframe. </typeparam>
    public class ChannelComponent<T> where T : struct
    {
        #region Properties
        /// <summary> Every keyframe in this channel. </summary>
        public IReadOnlyList<Keyframe<T>> Keyframes { get; }

        /// <summary> Gets the amount of keyframes in this channel. </summary>
        public int Count => Keyframes.Count;
        #endregion

        #region Constructors
        /// <summary> Creates a new channel wrapping the given <paramref name="keyframes"/>. </summary>
        /// <param name="keyframes"> The read only collection of keyframes. </param>
        public ChannelComponent(IReadOnlyList<Keyframe<T>> keyframes) => Keyframes = keyframes ?? throw new ArgumentNullException(nameof(keyframes));
        #endregion

        #region Frame Functions
        /// <summary> Calculates the current frame of a playback along with some tweening data. </summary>
        /// <param name="animationPlayer"> The player that is currently playing this channel. </param>
        /// <param name="currentFrame"> The reference to the current frame of the playback. This will be automatically updated to the current frame of playback based on the <see cref="AnimationPlayer.CurrentTime"/>. </param>
        /// <param name="nextValue"> The value of the frame directly after the current one, used for tweening. </param>
        /// <param name="tweenScalar"> The value between <c>0</c> and <c>1</c> which is used to lerp between the <paramref name="currentFrame"/> and <paramref name="nextValue"/>. </param>
        public void CalculateInterpolatedFrameData(AnimationPlayer animationPlayer, ref Keyframe<T> currentFrame, out T nextValue, out float tweenScalar)
        {
            // Prevent infinite looping. This should never happen, but better safe than sorry.
            if (Count <= 1) throw new Exception("Cannot lerp with only one frame!");

            // Get the tick distance between the tick time of the current frame and the current tick of the animation.
            int tickDistance = measureFrameDistance(animationPlayer, currentFrame.TickTime, animationPlayer.CurrentWholeTick);

            // Assume that the next frame is simply the one directly after the current.
            // In most cases, this is fine. However, if the playback is over 1, then frames may be skipped.
            Keyframe<T> nextFrame = getFrameWrapped(currentFrame.Index, animationPlayer.PlaybackDirection);

            // Keep track of the distance between the current frame and the next frame.
            int currentDistance = measureFrameDistance(animationPlayer, currentFrame.TickTime, nextFrame.TickTime);

            // Keep moving along the animation until the frames before and after the current time are found.
            while (tickDistance >= currentDistance)
            {
                // Move along the animation.
                currentFrame = nextFrame;
                nextFrame = getFrameWrapped(currentFrame.Index, animationPlayer.PlaybackDirection);

                // Keep track of the distance between the current frame and next frame, and add it onto the existing distance.
                currentDistance += measureFrameDistance(animationPlayer, currentFrame.TickTime, nextFrame.TickTime);
            }

            // Handle correcting the current time of the animation player.
            // This is just due to how distances are calculated and avoids a nasty bug where it considers the entire animation to have played in a single frame.
            float wrappedCurrentTime = animationPlayer.CurrentTick;
            if (animationPlayer.PlaybackDirection == -1 && animationPlayer.CurrentTick == 0) wrappedCurrentTime = animationPlayer.Animation.DurationInTicks;
            else if (animationPlayer.PlaybackDirection == 1 && animationPlayer.CurrentTick == animationPlayer.Animation.DurationInTicks) wrappedCurrentTime = 0;

            // Calculate the scalar using the distance between the current time of the animation and the current frame, and the current frame and next frame.
            tweenScalar = measureFrameDistance(animationPlayer, currentFrame.TickTime, wrappedCurrentTime) / measureFrameDistance(animationPlayer, currentFrame.TickTime, nextFrame.TickTime);

            // Set the next value to that of the next frame's value.
            nextValue = nextFrame.Value;
        }

        /// <summary> Gets the next frame in the sequence based on the <paramref name="playbackDirection"/> and handles wrapping so that a valid frame is always returned. </summary>
        /// <param name="index"> The index of the frame from which the next frame should be found. </param>
        /// <param name="playbackDirection"> The direction of the animation's playback. </param>
        /// <returns> The next frame in the sequence. </returns>
        private Keyframe<T> getFrameWrapped(int index, int playbackDirection)
            => playbackDirection == -1
                ? index - 1 < 0 ? Keyframes[Keyframes.Count - 1] : Keyframes[index - 1]
                : index + 1 >= Count ? Keyframes[0] : Keyframes[index + 1];

        /// <summary> Measures the distance between two given ticks, taking into account wrapping and playback direction, and including fractional ticks. </summary>
        /// <param name="animationPlayer"></param>
        /// <param name="firstTick"> The tick that comes "first" in the playback. For reverse playback, this tick is usually closer to the end of the animation. </param>
        /// <param name="secondTick"> The tick that comes "second" in the playback. </param>
        /// <returns> The distance in ticks between the two frames. </returns>
        private float measureFrameDistance(AnimationPlayer animationPlayer, float firstTick, float secondTick)
            => animationPlayer.PlaybackDirection == -1
                ? secondTick > firstTick ? animationPlayer.Animation.DurationInTicks - secondTick + firstTick : firstTick - secondTick
                : secondTick < firstTick ? animationPlayer.Animation.DurationInTicks - firstTick + secondTick : secondTick - firstTick;

        /// <summary> Measures the distance between two given ticks, taking into account wrapping and playback direction. </summary>
        /// <param name="animationPlayer"></param>
        /// <param name="firstTick"> The tick that comes "first" in the playback. For reverse playback, this tick is usually closer to the end of the animation. </param>
        /// <param name="secondTick"> The tick that comes "second" in the playback. </param>
        /// <returns> The distance in ticks between the two frames. </returns>
        private int measureFrameDistance(AnimationPlayer animationPlayer, int firstTick, int secondTick) => (int)measureFrameDistance(animationPlayer, (float)firstTick, secondTick);
        #endregion
    }
}