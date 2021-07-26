using Microsoft.Xna.Framework;

namespace Liru3D.Animations
{
    public class BoneChannelPlayer
    {
        #region Dependencies
        private readonly AnimationPlayer animationPlayer;
        #endregion

        #region Fields
        private Keyframe<Vector3> currentScaleFrame;

        private Keyframe<Quaternion> currentRotationFrame;

        private Keyframe<Vector3> currentPositionFrame;
        #endregion

        #region Backing Fields
        private BoneChannel channel;
        #endregion

        #region Properties
        public Vector3 InterpolatedScale { get; private set; } = Vector3.One;

        public Quaternion InterpolatedRotation { get; private set; } = Quaternion.Identity;

        public Vector3 InterpolatedPosition { get; private set; } = Vector3.Zero;

        public Matrix InterpolatedTransform { get; private set; } = Matrix.Identity;

        public BoneChannel Channel
        {
            get => channel;
            set
            {
                // Set the channel.
                channel = value;

                // Start on the first frame of the channel.
                ResetToFirstFrame();
            }
        }
        #endregion

        #region Constructors
        public BoneChannelPlayer(AnimationPlayer animationPlayer)
        {
            // Set the player.
            this.animationPlayer = animationPlayer ?? throw new System.ArgumentNullException(nameof(animationPlayer));
        }
        #endregion

        #region Frame Functions
        /// <summary> Handles smooth stepping between frames, and updating the current frame. </summary>
        public void Update()
        {
            if (animationPlayer.PlaybackDirection == 0) return;

            // Handle scale.
            if (Channel.Scales.Count > 1)
            {
                channel.Scales.CalculateInterpolatedFrameData(animationPlayer, ref currentScaleFrame, out Vector3 nextValue, out float tweenScalar);
                InterpolatedScale = Vector3.SmoothStep(currentScaleFrame.Value, nextValue, tweenScalar);
            }

            // Handle rotation.
            if (Channel.Rotations.Count > 1)
            {
                channel.Rotations.CalculateInterpolatedFrameData(animationPlayer, ref currentRotationFrame, out Quaternion nextValue, out float tweenScalar);
                InterpolatedRotation = Quaternion.Slerp(currentRotationFrame.Value, nextValue, tweenScalar);
            }

            // Handle translation.
            if (Channel.Positions.Count > 1)
            {
                channel.Positions.CalculateInterpolatedFrameData(animationPlayer, ref currentPositionFrame, out Vector3 nextValue, out float tweenScalar);
                InterpolatedPosition = Vector3.SmoothStep(currentPositionFrame.Value, nextValue, tweenScalar);
            }

            // Create the final interpolated transform.
            InterpolatedTransform = Matrix.CreateScale(InterpolatedScale) * Matrix.CreateFromQuaternion(InterpolatedRotation) * Matrix.CreateTranslation(InterpolatedPosition);
        }

        /// <summary> Resets this channel so that it is set to the first frame. </summary>
        public void ResetToFirstFrame()
        {
            // Set the current frames to the very first ones, or identity if there is no channel.
            currentScaleFrame = Channel == null ? new Keyframe<Vector3>(0, 0, Vector3.One) : Channel.Scales.Keyframes[0];
            currentRotationFrame = Channel == null ? new Keyframe<Quaternion>(0, 0, Quaternion.Identity) : Channel.Rotations.Keyframes[0];
            currentPositionFrame = Channel == null ?  new Keyframe<Vector3>(0, 0, Vector3.Zero) : Channel.Positions.Keyframes[0];

            // Set the interpolated transform to that of the first frame.
            InterpolatedScale = currentScaleFrame.Value;
            InterpolatedRotation = currentRotationFrame.Value;
            InterpolatedPosition = currentPositionFrame.Value;
            InterpolatedTransform = Matrix.CreateScale(InterpolatedScale) * Matrix.CreateFromQuaternion(InterpolatedRotation) * Matrix.CreateTranslation(InterpolatedPosition);
        }
        #endregion
    }
}
