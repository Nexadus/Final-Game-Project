using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BQM
{
    public enum GameObjectStatus {
        Alive,
        Invulnerable,
        Dead
    }
    //Might be used
    public enum Direction
    {
        Up, Down, Left, Right, 
        UpL, UpR, DownL, DownR
    };
    class GameObject
    {
        #region GameObject Data

        GameObjectStatus status;
        public GameObjectStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        Direction direction;
        public Direction ObjectDirection {
            get { return direction; }
            set { direction = value; }
        }

        #endregion

        #region GameObject Graphic Data

        int height;
        public int Height {
            get { return height; }
            set { height = value; }
        }
        int width;
        public int Width {
            get { return width; }
            set { width = value; }
        }
        Texture2D sprite;
        public Color[] SpriteData {
            get;
            private set;
        }

        public Texture2D Sprite
        {
            get { return sprite; }
            set
            {
                //Calculate matrix of sprite as soon as its set
                sprite = value;
            }
        }
        int currentFrameX = 0;
        public int FrameX {
            get { return currentFrameX; }
            set { currentFrameX = value; }
        }
        int currentFrameY = 0;
        public int FrameY
        {
            get { return currentFrameY; }
            set { currentFrameY = value; }
        }
        public void SetSpriteData() {;
            SpriteData = new Color[Width * Height];
            Sprite.GetData(0, new Rectangle(FrameX * Width, FrameY * Height, Width, Height),
                           SpriteData, FrameX * FrameY, Width * Height);
        }
        /// <summary>
        /// Object's rectangle will be used for collision
        /// </summary>
        public Rectangle rectangle;
        
        /// <summary>
        /// Transform amtrix for correct rectangle collision per-pixel collision
        /// </summary>
        public Matrix Transform {
            get;
            private set;
        }

        /// <summary>
        /// Origin property used for the center of the game object
        /// </summary>
        public Vector2 Origin {
            get {
                return new Vector2(Width , Height ) * 0.5f;
            }
        }

        public Rectangle boundary()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
        }
        
        /// <summary>
        /// Opacity and alpha of the object
        /// </summary>
        float opacity = 1.0f;
        float Alpha {
            get { return opacity; }
        }

        Color color = Color.White;
        protected Color Color {
            get {
                return color * opacity;
            }
            set { color = value; }
        }

        #endregion

        #region Physics Data

        /// <summary>
        /// Location of the game object in the game world
        /// </summary>
        Vector2 position = Vector2.Zero;
        public Vector2 Position {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Object's velocity to update the position
        /// </summary>
        Vector2 velocity = Vector2.Zero;
        public Vector2 Velocity {
            get { return velocity; }
            set { velocity = value; }
        }

        /// <summary>
        /// Object's acceleration to updat the velocity and speed
        /// </summary>
        Vector2 acceleration = Vector2.Zero;
        public Vector2 Acceleration {
            get { return acceleration; }
            set { acceleration = value; }
        }

        /// <summary>
        /// Where the object is looking at (may not use)
        /// </summary>
        float rotation = 0f;
        public float Rotation {
            get { return rotation; }
            set { rotation = value; }
        }

        float speed = 0.0f;
        public float Speed {
            get { return speed; }
            set { speed = value; }
        }

        #endregion

        #region Moving Physics Data

        public Vector2 MovingPosition(float speed) {
            switch(ObjectDirection) {
                case(Direction.Up):
                    return new Vector2((float)(speed * Math.Cos(this.Rotation - (MathHelper.Pi / 2))),
                                       (float)(speed * Math.Sin(this.Rotation - (MathHelper.Pi / 2))));
                case(Direction.Down):
                    return new Vector2((float)(speed * Math.Cos(this.Rotation + (MathHelper.Pi / 2))),
                                       (float)(speed * Math.Sin(this.Rotation + (MathHelper.Pi / 2))));
                case(Direction.Left):
                    return new Vector2((float)(speed * Math.Cos(this.Rotation + MathHelper.Pi)),
                                       (float)(speed * Math.Sin(this.Rotation + MathHelper.Pi)));
                case(Direction.Right):
                    return new Vector2((float)(speed * Math.Cos(this.Rotation)),
                                       (float)(speed * Math.Sin(this.Rotation)));
                case(Direction.UpL):
                    return MovingPosition(
                            new Vector2((float)(speed * Math.Cos(this.Rotation - (MathHelper.Pi / 2))),
                                        (float)(speed * Math.Sin(this.Rotation - (MathHelper.Pi / 2)))), speed);
                case(Direction.UpR):
                    return MovingPosition(
                            new Vector2((float)(speed * Math.Cos(this.Rotation - (MathHelper.Pi / 2))),
                                        (float)(speed * Math.Sin(this.Rotation - (MathHelper.Pi / 2)))), speed);
                case(Direction.DownL):
                    return MovingPosition(
                            new Vector2((float)(speed * Math.Cos(this.Rotation + (MathHelper.Pi / 2))),
                                        (float)(speed * Math.Sin(this.Rotation + (MathHelper.Pi / 2)))), speed);
                case(Direction.DownR):
                    return MovingPosition(
                            new Vector2((float)(speed * Math.Cos(this.Rotation + (MathHelper.Pi / 2))),
                                        (float)(speed * Math.Sin(this.Rotation + (MathHelper.Pi / 2)))), speed);
                default:
                    return Vector2.Zero;

            }
          
        }

        public Vector2 MovingPosition(Vector2 movement, float speed) {
            if((ObjectDirection == Direction.UpL) || (ObjectDirection ==  Direction.DownL))
                return Vector2.Add(movement, new Vector2(
                            (float)(speed * Math.Cos(this.Rotation + MathHelper.Pi)),
                            (float)(speed * Math.Sin(this.Rotation + MathHelper.Pi))));
            if((ObjectDirection == Direction.UpR) || (ObjectDirection == Direction.DownR))
                return Vector2.Add(movement, new Vector2(
                            (float)(speed * Math.Cos(this.Rotation)),
                            (float)(speed * Math.Sin(this.Rotation))));
            return Vector2.Zero;
        }

        #endregion

        #region Death Data

        /// <summary>
        /// 0.0 being fully alive, 1.0 being fully dead
        /// </summary>
        TimeSpan dieTime = TimeSpan.Zero;
        public TimeSpan DieTime {
            get { return dieTime; }
            set { dieTime = value; }
        }

        float diePercent = 0.0f;
        bool collided = false;

        #endregion

        #region Initialization Methods

        public virtual void Initialize() {
            if (!(status == GameObjectStatus.Alive))
                status = GameObjectStatus.Alive;
        }

        #endregion

        #region XNA Methods

        /// <summary>
        /// Update the game object
        /// </summary>
        /// <param name="gameTime">XNA GameTime object from the screen</param>
        public virtual void Update(GameTime gameTime) {
            if (status == GameObjectStatus.Alive) {
                //velocity += Vector2.Multiply(acceleration, (float)gameTime.ElapsedGameTime.TotalSeconds);
                //position += Vector2.Multiply(velocity, (float)gameTime.ElapsedGameTime.TotalSeconds);
                CalculateMatrix();
                CalculateBoundingRectangle();
            }
            else if ( status == GameObjectStatus.Dead ) {
                //Dead(gameTime);
            }
        }


        /// <summary>
        /// Positions will be changed so the transform matrix must be calculated 
        /// depending on rotation, scale and position. Is done during the game loop
        /// </summary>
        private void CalculateMatrix() {
            Transform =
                Matrix.CreateTranslation(new Vector3(-Origin, 0)) *
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateScale(1.0f) *
                Matrix.CreateTranslation(new Vector3(position, 0));
        }

        private void CalculateBoundingRectangle() {
            if ( sprite != null ) {
                rectangle = new Rectangle(0, 0, sprite.Width, sprite.Height);
                Vector2 leftTop     = Vector2.Transform(new Vector2(rectangle.Left, rectangle.Top), Transform),
                        rightTop    = Vector2.Transform(new Vector2(rectangle.Right, rectangle.Top), Transform),
                        leftBottom  = Vector2.Transform(new Vector2(rectangle.Left, rectangle.Bottom), Transform),
                        rightBottom = Vector2.Transform(new Vector2(rectangle.Right, rectangle.Bottom), Transform),
                        min         = Vector2.Min(Vector2.Min(leftTop, rightTop), Vector2.Min(leftBottom, rightBottom)),
                        max         = Vector2.Max(Vector2.Max(leftTop, rightTop), Vector2.Max(leftBottom, rightBottom));

                rectangle = new Rectangle((int)min.X, (int)min.Y,
                                          (int)(max.X - min.X),
                                          (int)(max.Y - min.Y));
            }
        }


        #endregion
    }
}
