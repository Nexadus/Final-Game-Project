using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace BQM
{
    class Blaster : GameObject
    {

        #region Variables

        /// <summary>
        /// Current frame thats being used
        /// </summary>
        int currentFrame;
        public int CurrentFrame {
            get;
            private set;
        }
        Direction facing;
        /// <summary>
        /// Maximum amount of time it lives before it dies
        /// </summary>
        int life;
        /// <summary>
        /// Timer to keep count of animation
        /// </summary>
        float timer;
        /// <summary>
        /// Image number of sprites
        /// </summary>
        int imageNumbr = 5;
        /// <summary>
        /// Speed to determine the projectile
        /// </summary>
        float speed = 7.0f;
        /// <summary>
        /// Interval for animation changes
        /// </summary>
        float interval = 40f;
        /// <summary>
        /// Angle of animation being displayed
        /// </summary>
        float direction;
        bool currentAction;


        #endregion

        #region Blaster Initializing

        /// <summary>
        /// Creates a Blaster class, which contains the sprite, its position, direction and where its facing
        /// </summary>
        /// <param name="sprite">Sprite image</param>
        /// <param name="ang">Angle in case of camera rotation</param>
        /// <param name="pos">Position</param>
        /// <param name="face">Projectile is facing</param>
        /// <param name="dir">Direction its going</param>
        public Blaster(Texture2D sprite, float ang, Vector2 pos, Direction face, float dir) {
            //GameObject gets sprite
            this.Sprite = sprite;
            //Sets the data for width and height
            this.Width = sprite.Width/imageNumbr;
            this.Height = sprite.Height;
            //Sets sprite data
            this.SetSpriteData();
            //Current frame
            currentFrame = 0;
            rectangle = new Rectangle(currentFrame * this.Width, 0, this.Width, this.Height);
            //Relative to the characters position;
            this.Position = pos;
            //Angle (IF Character rotated)
            this.Rotation = ang;
            life = 60;
            //Default characters face
            facing = face;
            direction = dir;

        }

        #endregion

        #region Blaster Animation

        public void AnimateShot(float gameTime) {
            timer += gameTime;
            if(timer > interval) {
                timer = 0;
                currentFrame++;
            }
            rectangle.X = currentFrame * this.Width;
            if (currentFrame == imageNumbr)
                currentFrame = 2;
        }

        #endregion

        #region Collision Blaster

        #endregion

        public void Dead(GameTime gameTime) {
            //Display animation
        }

        #region XNA Blaster Methods

        public void Update(GameTime gameTime, bool currentAction)
        {

            life--;
            if (life == 0) 
                Status = GameObjectStatus.Dead;
            AnimateShot(gameTime.ElapsedGameTime.Milliseconds);
            Vector2 movement = Vector2.Zero;
            if (currentAction)
                speed = 9.0f;
            if (facing == Direction.Up) {
                movement = new Vector2((float)(speed * Math.Cos(this.Rotation - (Math.PI / 2))),
                                       (float)(speed * Math.Sin(this.Rotation - (Math.PI / 2))));
            }
            else if (facing == Direction.Down) {
                
                movement = new Vector2((float)(speed * Math.Cos(this.Rotation + (Math.PI / 2))),
                                       (float)(speed * Math.Sin(this.Rotation + (Math.PI / 2))));
                
            }
            else if (facing == Direction.Left) {
                
                movement = new Vector2((float)(speed * Math.Cos(this.Rotation + Math.PI)),
                                       (float)(speed * Math.Sin(this.Rotation + Math.PI)));
                
            }
            else if (facing == Direction.Right)
            {
                movement = new Vector2((float)(speed * Math.Cos(this.Rotation)),
                                      (float)(speed * Math.Sin(this.Rotation)));                
            }
            else if (facing == Direction.UpL)
            {
                
                movement = new Vector2((float)(speed * Math.Cos(this.Rotation - (Math.PI / 2))),
                                       (float)(speed * Math.Sin(this.Rotation - (Math.PI / 2))));
                movement = Vector2.Add(movement, new Vector2((float)(speed * Math.Cos(this.Rotation + Math.PI)),
                                           (float)(speed * Math.Sin(this.Rotation + Math.PI))));
            }
            else if (facing == Direction.UpR)
            {
                movement = new Vector2((float)(speed * Math.Cos(this.Rotation - (Math.PI / 2))),
                                      (float)(speed * Math.Sin(this.Rotation - (Math.PI / 2))));
                movement = Vector2.Add(movement, new Vector2((float)(speed * Math.Cos(this.Rotation)),
                                         (float)(speed * Math.Sin(this.Rotation))));
            }
            else if (facing == Direction.DownL)
            {
                
                movement = new Vector2((float)(speed * Math.Cos(this.Rotation + (Math.PI / 2))),
                                       (float)(speed * Math.Sin(this.Rotation + (Math.PI / 2))));
                movement = Vector2.Add(movement, new Vector2((float)(speed * Math.Cos(this.Rotation + Math.PI)),
                                          (float)(speed * Math.Sin(this.Rotation + Math.PI))));
            }
            else if (facing == Direction.DownR)
            {
                
                movement = new Vector2((float)(speed * Math.Cos(this.Rotation + (Math.PI / 2))),
                                      (float)(speed * Math.Sin(this.Rotation + (Math.PI / 2))));
                movement = Vector2.Add(movement, new Vector2((float)(speed * Math.Cos(this.Rotation)),
                                          (float)(speed * Math.Sin(this.Rotation))));                
            }

            this.Position = Vector2.Add(this.Position, movement);

            

            speed = 7.0f;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBach)
        {
              spriteBach.Draw(this.Sprite, this.Position, this.rectangle, Color.White, direction, this.Origin, 1.0f, SpriteEffects.None, 0.0f);

        }

        #endregion 



    }
}
