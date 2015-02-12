using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;


namespace BQM
{
    class Enemy : GameObject
    {
        public enum EnemyState {
            Wander = 0,
            ChasePlayer,
            AttackPlayer,
            Spawn,
            Dead
        }

        #region Enemy Game Data
        static Random rand = new Random();
        const int changeWanderPath = 1000;
        const int numOfImagesX = 6;
        const int numOfImagesY = 3;
        const int deathImages = 10;
        int changingPaths = 100;
        int currentFrame;
        int imageNumber = 5;
        float timer = 0f, walkTimer = 0f;
        float enemySpeed = 2.5f;
        float spawnTimer = 0, deathInterval = 80f, interval = 250f, walkInterval = 250f;
        float chaseVector, range = 200, stopRange = 20;
        bool spawnFlag = false;
        public bool fullyDead = false;
        Texture2D spawn, death;
        Rectangle spawnBox, deathBox;
        EnemyState status;
        

        #endregion

        #region Enemy Initialized Data

        public Enemy(Texture2D sprite, Texture2D spawnSprite, Texture2D deathSprite, Vector2 pos) {
            //GameObject gets Enemy Spritesheet for death, spawn
            Sprite = sprite;
            spawn = spawnSprite;
            death = deathSprite;
            //Rectangles used for death and spawn animations
            spawnBox = new Rectangle(0, 0, spawn.Width / imageNumber, spawn.Height); 
            deathBox = new Rectangle(0, 0, death.Width / deathImages, death.Height);
            //Sets the data for collision
            Width = sprite.Width / numOfImagesX;
            Height = sprite.Height / numOfImagesY;
            this.SetSpriteData();
            //Current Frame, only used for walking
            FrameX = 0;
            FrameY = 0;
            this.ObjectDirection = Direction.Down;
            Position = pos;
            //Rectangle set
            rectangle = new Rectangle(currentFrame * Width, 0, Width, Height);
            //Sets status to state and invulnerable when attacking it spawning
            status = EnemyState.Spawn;
            Status = GameObjectStatus.Invulnerable;
        }

        #endregion

        #region Movement Animation Functions

        void Spawn(float gameTime) {
            spawnTimer += gameTime;
            if (spawnTimer > interval && currentFrame != 5)
            {
                spawnTimer = 0;
                currentFrame++;
            }
            else if (currentFrame >= 4) {
                spawnFlag = true;
                spawnTimer = 0;
            }
            spawnBox.X = currentFrame * spawnBox.Width;
        }

        public void Dead(float gameTime) {
            if (spawnTimer > deathInterval && currentFrame < 11)
            {
                spawnTimer = 0;
                currentFrame++;
            }
            else if (currentFrame >= 11)
                fullyDead = true;
            spawnTimer += gameTime;
            deathBox.X = currentFrame * (death.Width / deathImages);            
        }

        void AnimateDown() {
            FrameY = 0;
            if (FrameX > 5) FrameX = 0;
            if (walkTimer > walkInterval)
            {
                FrameX++;
                if (FrameX > 5)
                    FrameX = 0;
                walkTimer = 0f;
            }
            rectangle.X = FrameX * Width;
            rectangle.Y = FrameY * Height;
           
        }
        void AnimateLeft() {
            FrameY = 1;
            if (FrameX > 2) FrameX = 0;
            if (walkTimer > walkInterval)
            {
                FrameX++;
                if (FrameX > 2)
                    FrameX = 0;
                walkTimer = 0;
            }
            rectangle.X = FrameX * Width;
            rectangle.Y = FrameY * Height;
        }

        void AnimateRight() {
            FrameY = 1;
            if (FrameX < 2)
                FrameX = 3;
            if (walkTimer > walkInterval)
            {
                FrameX++;
                if (FrameX > 5)
                    FrameX = 3;
                walkTimer = 0;
            }
            rectangle.X = FrameX * Width;
            rectangle.Y = FrameY * Height;
        }

        void AnimateUp() {
            FrameY = 2;
            if (FrameX > 3) FrameX = 0;
            if (walkTimer > walkInterval)
            {
                FrameX++;
                if (FrameX > 3)
                    FrameX = 0;
                walkTimer = 0;
            }
            rectangle.X = FrameX * Width;
            rectangle.Y = FrameY * Height;
        }
        #endregion

        #region Enemy AI functions

        private void MoveTo(Vector2 dest) {
            Vector2 movement = Vector2.Zero;
            ObjectDirection = DirectionOfTarget(dest);
            movement = MovingPosition(enemySpeed);
            Position = Vector2.Add(Position, movement);
            
        }

        private void Wander(float gameTime) {
            timer += gameTime;

            if(timer > changeWanderPath) {
                timer = 0;
                int pick = rand.Next(0, 7);
                ObjectDirection = pickDirection(pick);
                changingPaths = 100;
            }
            if (changingPaths > 0) {
                changingPaths--;                
            }
            if ( changingPaths < 0 ) {
                timer = 0;
            }
            else {
                Animate();
                Position = Vector2.Add(Position, MovingPosition(enemySpeed));
            }

                
        }

        private Direction DirectionOfTarget(Vector2 dest) {
            float angle = calculateAngle(Position, dest);
            if(  angle <= 22.5 && angle >= 337.5 ) {
                AnimateRight();
                return Direction.Right;
            }
            if( angle < 67.5 && angle > 22.5 ) {
                AnimateRight();
                return Direction.UpR;
            }
            if (angle < 112.5 && angle > 67.5)
            {
                AnimateUp();
                return Direction.Up;
            }
            if (angle < 157.5 && angle > 112.5)
            {
                AnimateUp();
                return Direction.UpL;
            }
            if (angle < 202.5 && angle > 157.5)
            {
                AnimateLeft();
                return Direction.Left;
            }
            if (angle < 247.5 && angle > 202.5)
            {
                AnimateLeft();
                return Direction.DownL;
            }
            if (angle < 292.5 && angle > 202.5)
            {
                AnimateDown();
                return Direction.Down;
            }
            if (angle < 337.5 && angle > 292.5)
            {
                AnimateDown();
                return Direction.DownR;
            }            

            return ObjectDirection;
           
        }
        private float isInRange(Vector2 pos) {
            //calculates range of player
            Vector2 temp = Position - pos;
            return (float)temp.Length();
        }
        private float calculateAngle(Vector2 enemy, Vector2 player) {
            //Calculates both vectors using Atan2
            double calculateVectors = Math.Atan2( player.Y - enemy.Y , player.X - enemy.X );
            //Determines the angle
            float angle = MathHelper.ToDegrees((float)calculateVectors);
            //Result is [0, 360)
            return (360 - angle) % 360;
            
        }
        private void Animate() {
            if (ObjectDirection == Direction.Up || ObjectDirection == Direction.UpR)
                AnimateUp();
            if (ObjectDirection == Direction.Left || ObjectDirection == Direction.UpL)
                AnimateLeft();
            if (ObjectDirection == Direction.DownL || ObjectDirection == Direction.Down)
                AnimateDown();
            if (ObjectDirection == Direction.DownR || ObjectDirection == Direction.Right)
                AnimateRight();
        }
        private Direction pickDirection(int rand) {
            switch(rand) {
                case 0:
                    AnimateUp();
                    return Direction.Up;
                case 1:
                    AnimateDown();
                    return Direction.Down;
                case 2:
                    AnimateLeft();
                    return Direction.Left;
                case 3:
                    AnimateRight();
                    return Direction.Right;
                case 4:
                    AnimateUp();
                    return Direction.UpL;
                case 5:
                    AnimateUp();
                    return Direction.UpR;
                case 6:
                    AnimateLeft();
                    return Direction.DownL;
                case 7:
                    AnimateRight();
                    return Direction.DownR;
            }
            return ObjectDirection;
        }

        #endregion

        #region XNA Enemy Method

        public void Update(GameTime gameTime, Vector2 playerPos) {
            //base.Update(gameTime);
            //Calculate the chase vector every cycle
            walkTimer += gameTime.ElapsedGameTime.Milliseconds;
            if(status == EnemyState.Spawn) {
                Spawn(gameTime.ElapsedGameTime.Milliseconds);
                if (spawnFlag) {
                    Status = GameObjectStatus.Alive;
                    status = EnemyState.Wander;
                    currentFrame = 0;
                }
                
            }
            else if ( Status == GameObjectStatus.Dead ) {
                Dead(gameTime.ElapsedGameTime.Milliseconds);
            }
            else {
                chaseVector = Math.Abs(isInRange(playerPos));
                if (chaseVector < range && chaseVector > stopRange) {
                    enemySpeed = 2.5f;
                    MoveTo(playerPos);
                }
                else {
                    enemySpeed = 1.0f;
                    Wander(gameTime.ElapsedGameTime.Milliseconds);
                }
            }
            //if (rangeOfAggression.Intersects(playerRect))
            //    MoveTo(playerPos);
            
            /*
            switch(status) {
                case(EnemyState.Wander):
                    if (distanceToPlayer < distanceToAggro)
                        status = EnemyState.ChasePlayer;
                    else
                        Wander(gameTime);
                    break;
                case(EnemyState.ChasePlayer):
                    if (distanceToPlayer > distanceToAggro)
                    {
                        status = EnemyState.Wander;
                        Wander(gameTime);
                    }
                    else
                        ChasePlayer(gameTime, playerPos);
                    break;
            }
             * */
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBach)
        {
            if(status == EnemyState.Spawn)
                spriteBach.Draw(spawn, this.Position, spawnBox, Color.White, this.Rotation, this.Origin, 1.0f, SpriteEffects.None, 0.0f);
            else if (Status == GameObjectStatus.Dead)
                spriteBach.Draw(death, this.Position, deathBox, Color.White, this.Rotation, this.Origin, 1.0f, SpriteEffects.None, 0.0f);
            else
                spriteBach.Draw(Sprite, this.Position, rectangle, Color.White, this.Rotation, this.Origin, 1.0f, SpriteEffects.None, 0.0f);


        }

        #endregion
    }
}
