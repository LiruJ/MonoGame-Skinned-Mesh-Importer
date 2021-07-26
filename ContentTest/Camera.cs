using Liru3D.Animations;
using Liru3D.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentTest
{
    public class Camera
    {
        #region Constants
        private const float rotationSpeed = 2f;

        private const float scrollSpeed = 0.001f;
        #endregion

        #region Dependencies
        private readonly GraphicsDevice graphics;
        #endregion

        #region Fields
        /// <summary> The state of the mouse in the last frame, used to find deltas. </summary>
        private MouseState lastMouseState;

        /// <summary> The font used to draw text onto the screen. </summary>
        private SpriteFont font;

        /// <summary> The spriteBatch used to draw 2D elements onto the screen. </summary>
        private SpriteBatch spriteBatch;
        #endregion

        #region Properties
        public float Distance { get; set; } = 4f;

        public float RotationY { get; set; } = MathHelper.ToRadians(45);

        public float RotationX { get; set; } = MathHelper.ToRadians(-15);

        /// <summary> The projection matrix of the camera. </summary>
        public Matrix Projection { get; }

        /// <summary> The orientation of the camera in world space. Use <see cref="InvertedView"/> for drawing instead. </summary>
        public Matrix View { get; private set; }

        /// <summary> The inverted view matrix, passed into draw functions. </summary>
        public Matrix InvertedView { get; private set; }

        public SkinnedEffect SkinnedEffect { get; }
        #endregion

        #region Constructors
        public Camera(GraphicsDevice graphics, SpriteFont font)
        {
            // Set the graphics device.
            this.graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));

            // Initialise the projection matrix.
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60f), graphics.Viewport.Width / (float)graphics.Viewport.Height, 0.01f, 1000f);

            // Create the skinned effect for the model.
            SkinnedEffect = new SkinnedEffect(graphics);

            // Enable the lighting for the effect.
            SkinnedEffect.EnableDefaultLighting();

            // Initialise the 2D fields.
            this.font = font ?? throw new ArgumentNullException(nameof(font));
            spriteBatch = new SpriteBatch(graphics);
        }
        #endregion

        #region Update Functions
        public void Update(GameTime gameTime)
        {
            // Get the current state of the mouse.
            MouseState currentMouseState = Mouse.GetState();

            // Calculate the difference in scroll values between this frame and last frame.
            int scrollDelta = currentMouseState.ScrollWheelValue - lastMouseState.ScrollWheelValue;

            // Modify the zoom level of the camera.
            // Note that the scrollDelta is in "detents", which is usually 120 per "click" of the mouse wheel. This is huge, so it needs to be scaled down to something reasonable with the scrollSpeed.
            Distance = Math.Max(0, Distance - (scrollDelta * scrollSpeed));

            // Handle rotation if the right mouse button is pressed.
            if (currentMouseState.RightButton == ButtonState.Pressed)
            {
                // Calculate the difference in mouse positions between this frame and last frame.
                Point mouseDelta = currentMouseState.Position - lastMouseState.Position;

                // Set the rotations of the camera.
                // Note that this may look incorrect since RotationX is using the Y and vice versa, but here the RotationY is the rotation around the Y axis which is the horizontal rotation.
                // The delta is scaled based on the size of the screen to make it more uniform and consistent.
                RotationX -= (mouseDelta.Y / (float)graphics.Viewport.Height) * rotationSpeed;
                RotationY -= (mouseDelta.X / (float)graphics.Viewport.Width) * rotationSpeed;

                // Set the cursor to show that the camera is being moved.
                Mouse.SetCursor(MouseCursor.SizeAll);

                // Move the mouse back to where it was and replace the current mouse state with it. This locks the cursor in place when the user is rotating the view.
                Mouse.SetPosition(lastMouseState.X, lastMouseState.Y);
                currentMouseState = Mouse.GetState();
            }
            // Otherwise; if the user is not moving the camera, reset the mouse cursor.
            else Mouse.SetCursor(MouseCursor.Arrow);

            // Create the view and inverted view matrices. This is done every frame regardless of any changes. This is inefficient, but it's a demo project so you know.
            View = Matrix.CreateTranslation(0, 0, Distance) * Matrix.CreateRotationX(RotationX) * Matrix.CreateRotationY(RotationY) * Matrix.CreateTranslation(0, 1, 0);
            InvertedView = Matrix.Invert(View);

            // Set the last mouse state to the current one.
            lastMouseState = currentMouseState;
        }
        #endregion

        #region Draw Functions
        public void Begin()
        {
            // Clear the screen.
            graphics.Clear(Color.CornflowerBlue);

            // Set the depth and sample state.
            graphics.DepthStencilState = DepthStencilState.Default;
            graphics.SamplerStates[0] = SamplerState.PointWrap;

            // Set the matrices of the effect.
            SkinnedEffect.View = InvertedView;
            SkinnedEffect.Projection = Projection;

            // Begin the spritebatch.
            spriteBatch.Begin();
        }

        public void End() => spriteBatch.End();

        public void Draw(AnimationPlayer animationPlayer, Matrix world)
        {
            // Set the world matrix of the effect.
            SkinnedEffect.World = world;

            foreach (SkinnedMesh mesh in animationPlayer.Model.Meshes)
            {
                // Set the bones of the effect. This has to be done for each mesh, as the internal state of the effect changes with every application.
                animationPlayer.SetEffectBones(SkinnedEffect);

                // Apply the first pass of the effect.
                // This should really loop over each pass and apply, but the default SkinnedEffect only has one pass, and I don't want to test loads of random things for a pointless feature.
                SkinnedEffect.CurrentTechnique.Passes[0].Apply();

                // Draw the mesh.
                mesh.Draw();
            }
        }

        public void Draw(Model model, Matrix world)
        {
            model.Draw(world, InvertedView, Projection);
        }

        public void DrawString(string text, Vector2 screenPosition) => spriteBatch.DrawString(font, text, screenPosition, Color.Black);

        public void DrawString(string text, Vector3 worldPosition)
        {
            Vector3 screenSpace = graphics.Viewport.Project(worldPosition, Projection, InvertedView, Matrix.Identity);

            spriteBatch.DrawString(font, text, new Vector2(screenSpace.X, screenSpace.Y), Color.Black);
        }
        #endregion
    }
}