using Liru3D.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Liru3D.Animations
{
    /// <summary> Stores a collection of transforms for a <see cref="SkinnedModel"/> and manipulates them by playing an <see cref="Animation"/>. </summary>
    public class AnimationPlayer
    {
        #region Backing Fields
        private readonly Matrix[] modelSpaceTransforms;

        private readonly Matrix[] boneSpaceTransforms;

        /// <summary> Keeps track of each bone's current frame. </summary>
        private readonly BoneChannelPlayer[] channelPlayers;

        private Animation animation;

        private float currentTime;

        private bool isPlaying = false;
        #endregion

        #region Properties
        public SkinnedModel Model { get; }

        public Animation Animation
        {
            get => animation;
            set
            {
                // Do nothing if there is no change.
                if (Animation == value) return;

                // Set the animation.
                animation = value;

                // Get the channel from the name of the indexed bones. This ensures that the collection of current frames is in the same order as the bones.
                for (int i = 0; i < Model.BoneCount; i++)
                    channelPlayers[i].Channel = animation?.ChannelsByBoneName[Model.Bones[i].Name];

                // Reset the time.
                CurrentTime = 0f;

                // Stop playback.
                IsPlaying = false;

                // Update the transforms so that the animation is in its first frame.
                updateTransforms();
            }
        }

        /// <summary> The current time of this animation in seconds. </summary>
        public float CurrentTime
        {
            get => currentTime;
            set
            {
                currentTime = value;
                CurrentTick = Animation.TicksPerSecond * CurrentTime;
            }
        }

        public int CurrentWholeTick => (int)(PlaybackDirection == 1 ? Math.Floor(CurrentTick) : Math.Ceiling(CurrentTick));

        /// <summary> The current tick or "frame" of the animation. </summary>
        public float CurrentTick { get; set; }

        /// <summary> Gets the direction of playback, which is <c>1</c> when playing forward, <c>-1</c> when playing backward, and <c>0</c> when <see cref="PlaybackSpeed"/> is <c>0</c>. </summary>
        /// <remarks> Note that this does not take <see cref="IsPlaying"/> into account, only <see cref="PlaybackDirection"/>. </remarks>
        public int PlaybackDirection => Math.Sign(PlaybackSpeed);

        public float PlaybackSpeed { get; set; } = 1f;

        public bool IsPlaying
        {
            get => isPlaying;
            set
            {
                // Do nothing if no change is given.
                if (isPlaying == value) return;

                // If there is no animation, do nothing.
                if (Animation == null) return;

                // Set the value.
                isPlaying = value;

                // If playback has started but the animation is finished, restart it.
                if (isPlaying && (CurrentTime >= animation.DurationInSeconds || CurrentTime <= 0f))
                    CurrentTime = PlaybackDirection == 1 ? 0f : Animation.DurationInSeconds;
            }
        }

        public bool IsLooping { get; set; }

        /// <summary> The transform of each bone relative to the mesh. </summary>
        /// <remarks>
        /// This can be used to position objects around the bones, for example; a character holding an item. 
        /// This collection is created in the same order as the <see cref="SkinnedModel.Bones"/> collection.
        /// </remarks>
        public IReadOnlyList<Matrix> ModelSpaceTransforms => modelSpaceTransforms;

        /// <summary> The transform of each bone in bone-space. </summary>
        /// <remarks>
        /// This is uploaded to the GPU in order to draw the skinned mesh.
        /// This collection is created in the same order as the <see cref="SkinnedModel.Bones"/> collection.
        /// </remarks>
        public IReadOnlyList<Matrix> BoneSpaceTransforms => boneSpaceTransforms;
        #endregion

        #region Constructors
        public AnimationPlayer(SkinnedModel model)
        {
            // Set the mesh.
            Model = model ?? throw new ArgumentNullException(nameof(model));

            // Set up the transform arrays.
            modelSpaceTransforms = new Matrix[model.BoneCount];
            boneSpaceTransforms = new Matrix[model.BoneCount];

            // Set up the current frames.
            channelPlayers = new BoneChannelPlayer[model.BoneCount];
            for (int i = 0; i < Model.BoneCount; i++)
                channelPlayers[i] = new BoneChannelPlayer(this);
        }
        #endregion

        #region Update Functions
        public void Update(GameTime gameTime)
        {
            // If there is no animation or the animation is not playing, do nothing.
            if (!IsPlaying || Animation == null) return;

            // Update the current frame, which handles interpolation and frame changes of all channels.
            for (int i = 0; i < Model.BoneCount; i++)
                channelPlayers[i].Update();

            // Update the transform collections for model and bone space.
            updateTransforms();

            // Update the current time.
            CurrentTime += (float)gameTime.ElapsedGameTime.TotalSeconds * PlaybackSpeed;

            // Handle the playback going out of bounds, either starting over or stopping playback.
            if (CurrentTime >= animation.DurationInSeconds || CurrentTime <= 0f)
            {
                if (IsLooping) CurrentTime -= Animation.DurationInSeconds * PlaybackDirection;
                else IsPlaying = false;
            }
        }
        #endregion

        #region Animation Functions
        private void initialiseAnimation()
        {
            // Get the channel from the name of the indexed bones. This ensures that the collection of current frames is in the same order as the bones.
            for (int i = 0; i < Model.BoneCount; i++)
                channelPlayers[i].Channel = animation?.ChannelsByBoneName[Model.Bones[i].Name];

            // Reset the time.
            CurrentTime = 0f;

            // Update the transforms so that the animation is in its first frame.
            updateTransforms();
        }
        #endregion

        #region Bone Functions
        public void SetEffectBones(SkinnedEffect skinnedEffect) => skinnedEffect.SetBoneTransforms(boneSpaceTransforms);

        public void SetEffectBones(Action<Matrix[]> setFunction) => setFunction?.Invoke(boneSpaceTransforms);

        private void updateTransforms()
        {
            // Update each bone's transform.
            for (int i = 0; i < Model.BoneCount; i++)
            {
                // Get the current bone.
                Bone bone = Model.Bones[i];

                // Get the local interpolated transform of the bone.
                Matrix boneLocalTransform = channelPlayers[i].InterpolatedTransform;

                // Set the transforms.
                modelSpaceTransforms[i] = bone.HasParent ? boneLocalTransform * modelSpaceTransforms[bone.Parent.Index] : boneLocalTransform;
                boneSpaceTransforms[i] = bone.Offset * modelSpaceTransforms[i];
            }
        }
        #endregion
    }
}