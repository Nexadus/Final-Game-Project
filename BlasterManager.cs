using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace BQM
{
    class BlasterManager
    {
        #region BlasterManager Variables

        public List<Blaster> projectiles;
        Texture2D projectile;
        KeyboardState current;

        int timePerShot = 15;
        int timePerFrame = 50;
        int timeElapsed = 0;
        int timeSinceShot = 0;

        #endregion

        #region Blaster Movement Variables

        float up    = (3 * MathHelper.Pi) / 2,
              down  = MathHelper.PiOver2,
              left  = MathHelper.Pi,
              right = 0f,
              upR   = 7 * MathHelper.PiOver4,
              downR = MathHelper.PiOver4,
              upL   = 5 * MathHelper.PiOver4,
              downL = 3 * MathHelper.PiOver4;

        #endregion

        #region BlasterManager Methods

        public BlasterManager(Texture2D img) {
            projectile = img;
            projectiles = new List<Blaster>();
        }

        public void addProjectile(float a, Vector2 pos, Direction face) {
            float direction = handleDirection(face);
            projectiles.Add(new Blaster(projectile, a, pos, face, direction));
        }

        void collision(Blaster b) {
            projectiles.Remove(b);
        }

        float handleDirection(Direction face) {
            float angle = 0f;
            switch (face)
            {
                case (Direction.Up):
                    angle = up;
                    break;
                case (Direction.Down):
                    angle = down;
                    break;
                case (Direction.Left):
                    angle = left;
                    break;
                case (Direction.Right):
                    angle = right;
                    break;
                case (Direction.UpL):
                    angle = upL;
                    break;
                case (Direction.UpR):
                    angle = upR;
                    break;
                case (Direction.DownL):
                    angle = downL;
                    break;
                case (Direction.DownR):
                    angle = downR;
                    break;
            }
            return angle;
        }

        #endregion

        #region XNA Blaster Methods

        public void Update(GameTime gameTime, SpriteBatch spriteBatch, float angle, Vector2 pos, Direction face, bool currentAction) {
            current = Keyboard.GetState();
            timeElapsed += gameTime.ElapsedGameTime.Milliseconds;
            if( timeElapsed > timePerFrame ) {
                timeSinceShot++;
                if(current.IsKeyDown(Keys.Space) && (timeSinceShot > timePerShot )) {
                    timeSinceShot = 0;
                    float direction = handleDirection(face);
                    projectiles.Add(new Blaster(projectile, angle, pos, face, direction));
                }
                for(int i = 0; i < projectiles.Count; i++) {
                    projectiles[i].Update(gameTime, currentAction);
                    if (projectiles[i].Status == GameObjectStatus.Dead)
                        collision(projectiles[i]);
                }
                
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach(Blaster b in projectiles) {
                b.Draw(gameTime, spriteBatch);
            }
        }

        #endregion

    }
}
