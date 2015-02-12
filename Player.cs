using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace BQM
{
    class Player : GameObject
    {

        #region Player Game Data

        //Each player has 20 frames.
        const int imageNumber = 20;
        const int shootingImgX = 5;
        const int shootingImgY = 4;
        const float invulnerable = 3000;
        //Current and previos keyboard 
        KeyboardState current, previous;
        Texture2D shootingSprite;
        Rectangle sourceRect, shootingRectangle;
        public int life = 10;
        float blinkTime = 0;
        float timer = 0f;
        float playerSpeed = 5.0f;
        float interval = 55f;
        bool amIMoving = false, amIShooting = false;
        bool modelVisibility = true;
        public bool AmIMoving
        {
            get { return amIMoving; }
            set { amIMoving = value; }
        }

        #endregion

        #region Player Initialized Data

        public Player(Texture2D sprite, Texture2D shoot) {
            //GameObject gets spriteSheet
            Sprite = sprite;
            shootingSprite = shoot;
            //Sets the data for collision
            Width = sprite.Width / imageNumber;
            Height = sprite.Height;
            rectangle = new Rectangle(0, 0, Width, Height);
            shootingRectangle = new Rectangle(0, 0, shootingSprite.Width / shootingImgX, shootingSprite.Height / shootingImgY); 
            SetSpriteData();
            //Current frame
            FrameX = 1;
            FrameY = 1;
            //Default Object Direction
            this.ObjectDirection = Direction.Down;
        }

        #endregion

        #region Movement Functions

        public void HandleMovement(GameTime gameTime) {
           
            int width = this.Width;
            int height = this.Height;

            sourceRect = new Rectangle(FrameX * width, 0, width, height);

            if(pressingWSAD()) {
                //Down
                if (FrameX > 0 && FrameX < 5)
                    FrameX = 0;
                //Right
                if (FrameX > 5 && FrameX < 10)
                    FrameX = 5;
                //Up
                if (FrameX > 10 && FrameX < 15)
                    FrameX = 10;
                
                //Left
                if (FrameX > 15 && FrameX < 20)
                    FrameX = 15;
            }
            rectangle.X = FrameX * this.Width;
        }

        public void AnimateDown(GameTime gameTime) {
            if (current != previous) {
                FrameX = 1;
                
            }
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if(timer > interval) {
                FrameX++;
                if (FrameX > 4) {
                    FrameX = 0;
                }
                timer = 0f;
            }
            rectangle.X = FrameX * this.Width;
        }
        public void AnimateRight(GameTime gameTime) {
            if (current != previous)
                FrameX = 6;
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer > interval)
            {
                FrameX++;
                if (FrameX > 9)
                    FrameX = 5;
                timer = 0f;
            }
        }
        public void AnimateUp(GameTime gameTime) {
            if (current != previous)
                FrameX = 11;
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer > interval)
            {
                FrameX++;
                if (FrameX > 14)
                    FrameX = 10;
                timer = 0f;
            }
        }
        public void AnimateLeft(GameTime gameTime) {
            if (current != previous)
                FrameX = 16;
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer > interval)
            {
                FrameX++;
                if (FrameX > 19)
                    FrameX = 15;
                timer = 0f;
            }
        }
        public bool pressingWSAD() {
            if (current.IsKeyDown(Keys.W))
                return false;
            if (current.IsKeyDown(Keys.S))
                return false;
            if (current.IsKeyDown(Keys.A))
                return false;
            if (current.IsKeyDown(Keys.D))
                return false;
            return true;
            

        }

        #endregion

        #region Shooting Functions

        private void handleShooting(GameTime gameTime) {
            if(pressingWSAD()) {
                switch(ObjectDirection) {
                    case(Direction.Up):
                        FrameX = 0; FrameY = 1;
                        break;

                    case(Direction.Down):
                        FrameX = 0; FrameY = 0;
                        break;
                    case (Direction.Left):
                        FrameX = 0; FrameY = 2;
                        break;
                    case (Direction.Right):
                        FrameX = 0; FrameY = 3;
                        break;
                }
            }
            else {
                switch(ObjectDirection) {
                    case (Direction.Up):
                        AnimateShootingUp(gameTime);
                        break;
                    case (Direction.Down):
                        AnimateShootingDown(gameTime);
                        break;
                    case (Direction.Left):
                        AnimateShootingLeft(gameTime);
                        break;
                    case (Direction.Right):
                        AnimateShootingRight(gameTime);
                        break;
                }
            }
            shootingRectangle.X = FrameX * shootingSprite.Width;
            shootingRectangle.Y = FrameY * shootingSprite.Height;
        }

        private void AnimateShootingUp(GameTime gameTime)
        {

        }
        private void AnimateShootingDown(GameTime gameTime)
        {

        }
        private void AnimateShootingLeft(GameTime gameTime)
        {

        }
        private void AnimateShootingRight(GameTime gameTime)
        {

        }

        #endregion

        #region XNA Player Methods

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            previous = current;
            current = Keyboard.GetState();
            #region Movement 

            Vector2 movement = Vector2.Zero;
            Vector2 otherMovement = Vector2.Zero;
            //Is Up OR W pressed?.
            
            if (current.IsKeyDown(Keys.W)) {
                amIMoving = true;
                this.ObjectDirection = Direction.Up;

                movement = MovingPosition(playerSpeed);
                AnimateUp(gameTime);
                //Is Left/Right or A/D pressed while Up is pressed?
                if (current.IsKeyDown(Keys.A))
                {
                    this.ObjectDirection = Direction.UpL;
                    movement = MovingPosition(movement, playerSpeed);
                }
                else if (current.IsKeyDown(Keys.D))
                {
                    this.ObjectDirection = Direction.UpR;
                    movement = MovingPosition(movement, playerSpeed);
                }
            }
            //Then is Down or S pressed?
            else if (current.IsKeyDown(Keys.S)) {
                amIMoving = true;
                this.ObjectDirection = Direction.Down;
                movement = MovingPosition(playerSpeed);
                AnimateDown(gameTime);
                //Is Left/Right or A/D pressed while Down is pressed?
                if (current.IsKeyDown(Keys.A))
                {
                    this.ObjectDirection = Direction.DownL;
                    movement = MovingPosition(movement, playerSpeed);
                }
                if (current.IsKeyDown(Keys.D))
                {
                    this.ObjectDirection = Direction.DownR;
                    movement = MovingPosition(movement, playerSpeed);
                }
            }
            /* The logic here gets funky
             * If you are pressing Up+Left/Right or Down+Left/Right In the above statement.. the former will be moved to the 
             * bottom methods.
             * Without these methods the player would get stuck and cease to attack.
             * */
            else if (current.IsKeyDown(Keys.A)){
                amIMoving = true;
                this.ObjectDirection = Direction.Left;
                movement = new Vector2((float)(playerSpeed * Math.Cos(this.Rotation + Math.PI)),
                                       (float)(playerSpeed * Math.Sin(this.Rotation + Math.PI)));
                
                AnimateLeft(gameTime);
            }
            else if(current.IsKeyDown(Keys.D)) {
                amIMoving = true;
                this.ObjectDirection = Direction.Right;
                movement = new Vector2((float)(playerSpeed * Math.Cos(this.Rotation)),
                                      (float)(playerSpeed * Math.Sin(this.Rotation)));
                AnimateRight(gameTime);
            }
            else {
                amIMoving = false;
            }

            /*if (current.IsKeyDown(Keys.Space))
            {
                amIShooting = true;
            }
            else
                amIShooting = false;

             */
            #endregion 


            //Applying movement vector
            this.Position = Vector2.Add(this.Position, movement);
            //Handles idle animation
            if(!amIShooting)
                HandleMovement(gameTime);
            else if (amIShooting) {
                //handleShooting(gameTime);
            }

            if (Status == GameObjectStatus.Invulnerable && blinkTime < invulnerable)
            {
                modelVisibility = !modelVisibility;

                blinkTime += gameTime.ElapsedGameTime.Milliseconds;
            }
            else if ( blinkTime > invulnerable) {
                Status = GameObjectStatus.Alive;
                blinkTime = 0;
            }
        }

        //Draws the Sprite
        public void Draw(GameTime gameTime, SpriteBatch spriteBach) {
            //Makes sures spritebatch is not null

            if(modelVisibility) {
                //makes sure the sprite isnt null
                if (this.Sprite != null) {
                    if(amIShooting)
                        spriteBach.Draw(shootingSprite, this.Position, shootingRectangle, Color.White, this.Rotation, this.Origin, 1.0f, SpriteEffects.None, 1.0f);
                    else
                        spriteBach.Draw(this.Sprite, this.Position, sourceRect, Color.White, this.Rotation, this.Origin, 1.0f, SpriteEffects.None, 1.0f);
                }
            }
        }

        #endregion
    }
}
