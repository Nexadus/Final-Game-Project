using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BQM
{
    class Camera
    {
        #region CameraGetSetVariables

        //Viewport the camera will use to hold dimensions and etc
        public Viewport View {
            get;
            private set;
        }
        //Position of the center of camera
        public Vector2 Position {
            get;
            private set;
        }
        //Copy of the position when we begin to rumble the screen
        public Vector2 SavedPosition {
            get;
            private set;
        }
        //Center of focus point for the Camera
        public Vector2 FocusPoint {
            get;
            private set;
        }
        //Zoom scalar (1.0f = 100% zoom level)
        public float Zoom {
            get;
            private set;
        }
        public float Rotation {
            get;
            private set;
        }
        //Used to copy the old rotation when we shake
        public float SavedRotation {
            get;
            private set;
        }
        public float PositionShakeAmount {
            get;
            private set;
        }
        //Intensity to shake the camera in terms of Rotation
        public float RotationShakeAmount {
            get;
            private set;
        }
        //Maximum amount of time the shake will last
        public float MaxShakeTime {
            get;
            private set;
        }
        //Camera transform matrix
        public Matrix Transform {
            get;
            private set;
        }
        //Object we are following
        public Player Source {
            get;
            private set;
        }
        //Used to matching the rotation of the object
        public float SourceRotationOffset {
            get;
            private set;
        }
        #endregion

        #region Score

        private Vector2 scorePos = new Vector2();
        private Vector2 scorePosTwo = new Vector2();

        public SpriteFont Font { get; set; }

        public int Score { get; set; }

        #endregion

        #region CameraVariables

        TimeSpan shaketimer;
        Random random;

        #endregion

        #region CameraFunctions
        /// <summary>
        /// Initialize a Camera Object
        /// </summary>
        /// <param name="view">Viewport the camera will use and hold dimensions on</param>
        /// <param name="position">Position of the camera will be</param>
        public Camera(Viewport view, Vector2 position, SpriteFont sp) {
            View = view;
            Position = position;
            Zoom = 1.0f;
            Rotation = 0;
            random = new Random();
            FocusPoint = new Vector2(view.Width / 2, view.Height / 2);
            Font = sp;
        }
        /// <summary>
        /// Initialize a Camera Object (2)
        /// </summary>
        /// <param name="view">Viewport the camera will use and hold dimensions on</param>
        /// <param name="position">Position of the camera will be</param>
        /// <param name="focus">Where to point the center of the camera</param>
        /// <param name="zoom">Default Zoom</param>
        /// <param name="rotation">How much should be rotated by default</param>
        public Camera(Viewport view, Vector2 position, Vector2 focus, float zoom, float rotation) {
            View = view;
            Position = position;
            Zoom = zoom;
            Rotation = rotation;
            random = new Random();
            FocusPoint = focus;
            
        }

        public void Update(GameTime gameTime, KeyboardState current, KeyboardState previous) {
            if ( shaketimer.TotalSeconds > 0 ) {
                //Perform a camera shake

                /* We want to restore the saved positions and rotations
                 * so we do not go far from the center point
                 * */
                FocusPoint = SavedPosition;
                Rotation = SavedRotation;

                /* We want to subtract elapsed time with shaketimer
                 * If it is still above 0. We continue camera shake
                 * Otherwise loop is done and will go to else
                 * */
                shaketimer = shaketimer.Subtract(gameTime.ElapsedGameTime);
                if(shaketimer.TotalSeconds > 0) {
                    FocusPoint += new Vector2(
                        (float)((random.NextDouble() * 2) - 1) * PositionShakeAmount,
                        (float)((random.NextDouble() * 2) - 1) * PositionShakeAmount);
                    Rotation += (float)((random.NextDouble() * 2) - 1) * RotationShakeAmount;                                   
                }
            }
            else {
                /* Create a transofrm matrix through pos, scale, rot and translation to the focus point
                 * We use Math.Pow on the zoom to speed up or slow down the zoom, both X and Y will have 
                 * the same zoom levels. So no stretching 
                 */

                Vector2 objectPosition = Source != null ? Source.Position : Position;
                float objectRotation = Source != null ? Source.Rotation : Rotation;
                float deltaRotation = Source != null ? SourceRotationOffset : 0.0f;

                Transform = Matrix.CreateTranslation(
                    new Vector3(-objectPosition, 0)) *
                        Matrix.CreateScale(new Vector3(
                          (float)Math.Pow(Zoom, 10), (float)Math.Pow(Zoom, 10), 0)) *
                        Matrix.CreateRotationZ(-objectRotation + deltaRotation) *
                        Matrix.CreateTranslation(new Vector3(FocusPoint.X, FocusPoint.Y, 0));
            }
            
        }
        /// <summary>
        /// Perform a camera shake
        /// </summary>
        /// <param name="shakeTime">The amount of time to shake the camera</param>
        /// <param name="positionAmount">Amount in terms of position</param>
        /// <param name="rotationAmount">Amount in terms of rotation</param>
        public void Shake(float shakeTime, float positionAmount, float rotationAmount) {
            if (shaketimer.TotalSeconds <= 0) {
                MaxShakeTime = shakeTime;
                shaketimer = TimeSpan.FromSeconds(MaxShakeTime);
                PositionShakeAmount = positionAmount;
                RotationShakeAmount = rotationAmount;

                SavedPosition = FocusPoint;
                SavedRotation = Rotation;
            }
        }
        /// <summary>
        /// Camera points to the player and follows them
        /// </summary>
        /// <param name="source">Player object we follow</param>
        /// <param name="rotationOffSet">How much we spin camera</param>
        public void Follow(Player source, float rotationOffSet) {
            Source = source;
            scorePos = source.Position;
            scorePos -= new Vector2(-235, 235);
            scorePosTwo -= new Vector2(-235, -235);
            SourceRotationOffset = rotationOffSet;
        }

        /// <summary>
        /// Resets the camera to default values
        /// </summary>
        private void Reset() {
            Position = Vector2.Zero;
            Rotation = 0;
            Zoom = 1.0f;
            shaketimer = TimeSpan.FromSeconds(0);
            Source = null;
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.DrawString(Font, Score.ToString(), scorePos, Color.White);
        }

        #endregion
    }
}
