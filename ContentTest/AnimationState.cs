using Assimp;
using Liru3D.Animations;
using Liru3D.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace ContentTest
{
    public class AnimationState : Game
    {
        #region Constants
        private const Keys nextAnimationKey = Keys.Right;

        private const Keys previousAnimationKey = Keys.Left;

        private const Keys togglePlaybackKey = Keys.Space;

        private const Keys toggleLoopingKey = Keys.L;

        private const Keys toggleMeshKey = Keys.M;

        private const Keys toggleSkeletonKey = Keys.B;

        private const Keys toggleBoundsKey = Keys.N;

        private const Keys toggleHelpKey = Keys.H;

        private const Keys randomisePlayPositionKey = Keys.R;
        #endregion

        #region Fields
        private Random random = new Random();

        private SkinnedModel character;

        private Camera camera;

        private Model arrowModel;

        private Model boundingSphereModel;

        private AnimationPlayer animationPlayer;

        private Texture2D texture;

        private KeyboardState lastKeyboardState;

        private int currentAnimationIndex = 0;

        private bool drawMesh = true;

        private bool drawSkeleton = true;

        private bool drawBoundingSpheres = false;

        private bool drawHelp = true;
        #endregion

        #region Constructors
        public AnimationState()
        {
            new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        #endregion

        #region Initialisation Functions
        protected override void Initialize() => base.Initialize();

        protected override void LoadContent()
        {
            // Load the font for the camera.
            SpriteFont font = Content.Load<SpriteFont>("DebugFont");

            // Create the camera.
            camera = new Camera(GraphicsDevice, font);

            // Get the models and textures.
            arrowModel = Content.Load<Model>("Arrow");
            boundingSphereModel = Content.Load<Model>("BoundingSphere");
            texture = Content.Load<Texture2D>("MaleTexture");

            // Load the skinned character model.
            character = Content.Load<SkinnedModel>("Male");

            // Create an animation player and set its animation.
            animationPlayer = new AnimationPlayer(character) { Animation = character.Animations[currentAnimationIndex] };
        }
        #endregion

        #region Update Functions
        protected override void Update(GameTime gameTime)
        {
            // Get the current state of the keyboard.
            KeyboardState currentKeyboardState = Keyboard.GetState();

            // Handle the player quitting via escape.
            if (currentKeyboardState.IsKeyDown(Keys.Escape))
                Exit();

            // Handle the player toggling playback of animations.
            if (currentKeyboardState.IsKeyDown(togglePlaybackKey) && lastKeyboardState.IsKeyUp(togglePlaybackKey))
                animationPlayer.IsPlaying = !animationPlayer.IsPlaying;

            // Handle the player toggling looping.
            if (currentKeyboardState.IsKeyDown(toggleLoopingKey) && lastKeyboardState.IsKeyUp(toggleLoopingKey))
                animationPlayer.IsLooping = !animationPlayer.IsLooping;

            // Handle the player switching the current animation.
            if (currentKeyboardState.IsKeyDown(nextAnimationKey) && lastKeyboardState.IsKeyUp(nextAnimationKey))
            {
                currentAnimationIndex = (currentAnimationIndex + 1 >= character.AnimationCount) ? 0 : currentAnimationIndex + 1;
                animationPlayer.Animation = character.Animations[currentAnimationIndex];
            }
            else if (currentKeyboardState.IsKeyDown(previousAnimationKey) && lastKeyboardState.IsKeyUp(previousAnimationKey))
            {
                currentAnimationIndex = (currentAnimationIndex - 1 < 0) ? character.AnimationCount - 1 : currentAnimationIndex - 1;
                animationPlayer.Animation = character.Animations[currentAnimationIndex];
            }

            // Handle draw toggles.
            if (currentKeyboardState.IsKeyDown(toggleMeshKey) && lastKeyboardState.IsKeyUp(toggleMeshKey))
                drawMesh = !drawMesh;
            if (currentKeyboardState.IsKeyDown(toggleSkeletonKey) && lastKeyboardState.IsKeyUp(toggleSkeletonKey))
                drawSkeleton = !drawSkeleton;
            if (currentKeyboardState.IsKeyDown(toggleHelpKey) && lastKeyboardState.IsKeyUp(toggleHelpKey))
                drawHelp = !drawHelp;
            if (currentKeyboardState.IsKeyDown(toggleBoundsKey) && lastKeyboardState.IsKeyUp(toggleBoundsKey))
                drawBoundingSpheres = !drawBoundingSpheres;

            // Handle randomisation.
            if (currentKeyboardState.IsKeyDown(randomisePlayPositionKey) && lastKeyboardState.IsKeyUp(randomisePlayPositionKey))
            {
                animationPlayer.CurrentTick = random.Next(0, animationPlayer.Animation.DurationInTicks);
                animationPlayer.IsPlaying = true;
            }

            // Update the animation.
            animationPlayer.Update(gameTime);

            // Update the camera.
            camera.Update(gameTime);

            // Set the last keyboard state to the current state.
            lastKeyboardState = currentKeyboardState;

            // Update the base state.
            base.Update(gameTime);
        }
        #endregion

        #region Draw Functions
        protected override void Draw(GameTime gameTime)
        {
            // Start the camera.
            camera.Begin();

            // Set the texture of the skinned effect to that of the model to be rendered.
            camera.SkinnedEffect.Texture = texture;

            // Draw the character in its current animation.
            if (drawMesh)
                camera.Draw(animationPlayer, Matrix.Identity);

            // Draw the skeleton.
            if (drawSkeleton)
                for (int i = 0; i < character.BoneCount; i++)
                {
                    // Draw the bone as an arrow.
                    camera.Draw(arrowModel, animationPlayer.ModelSpaceTransforms[i]);

                    // Draw the name of the bone.
                    camera.DrawString(character.Bones[i].Name, animationPlayer.ModelSpaceTransforms[i].Translation);
                }

            // Draw the bounding spheres.
            if (drawBoundingSpheres)
            {
                // Set the parameters on the bounding spheres.
                foreach (ModelMesh mesh in boundingSphereModel.Meshes)
                    foreach (Effect effect in mesh.Effects)
                        effect.Parameters["DiffuseColor"].SetValue(new Color(Color.Green, 0.5f).ToVector4());

                // Draw the bounding spheres on each mesh.
                foreach (SkinnedMesh mesh in animationPlayer.Model.Meshes)
                    camera.Draw(boundingSphereModel, Matrix.CreateScale(mesh.BoundingSphere.Radius) * Matrix.CreateTranslation(mesh.BoundingSphere.Center));
            }

            // Draw the help.
            if (drawHelp)
                camera.DrawString(
                    $"Right click + drag to move camera, scroll to zoom\n" +
                    $"{(animationPlayer.IsPlaying ? "Stop" : "Start")} playback: {togglePlaybackKey}\n" +
                    $"Turn looping {(animationPlayer.IsLooping ? "off" : "on")}: {toggleLoopingKey}\n" +
                    $"Randomise play position: {randomisePlayPositionKey}\n" +
                    $"Next/previous animation: {nextAnimationKey}/{previousAnimationKey}\n" +
                    $"Toggle mesh: {toggleMeshKey}\n" +
                    $"Toggle bones: {toggleSkeletonKey}\n" +
                    $"Toggle bounds: {toggleBoundsKey}\n" +
                    $"Toggle help: {toggleHelpKey}\n" +
                    $"Data:\n" +
                    $"Time: {animationPlayer.CurrentTime:F2}/{animationPlayer.Animation.DurationInSeconds:F2}\n" +
                    $"Frames: {animationPlayer.CurrentWholeTick}/{animationPlayer.Animation.DurationInTicks}", new Vector2(5));

            // End the camera's drawing.
            camera.End();

            // Draw the base state.
            base.Draw(gameTime);
        }
        #endregion
    }
}
