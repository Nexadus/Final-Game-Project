using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace BQM
{
    class EnemyManager
    {
        #region EnemyManager Variables

        const int MaxEnemiesOnScreen = 7;
        const int SpawnRange = 200;
        const int MinTimer = 2000;
        const int MaxTimer = 200000;
        public List<Enemy> enemies;
        Texture2D zombieB, zombieBSpawn, zombieG, zombieGSpawn, deathSprite;

        static Random randTimer;
        float timer, timeLeft;

        bool Attacked = false;

        #endregion

        #region EnemyManager Methods

        public EnemyManager(Texture2D imgB, Texture2D imgG, Texture2D spawnB, Texture2D spawnG, Texture2D death) {
            zombieB = imgB;
            zombieG = imgG;
            zombieBSpawn = spawnB;
            zombieGSpawn = spawnG;
            deathSprite = death;
            enemies = new List<Enemy>();
            randTimer = new Random();
        }

        public void addEnemy(Vector2 playerPos) {
            enemies.Add(gender(randomSpawn(playerPos)));
        }

        void collision(Enemy e, float time) {
            e.Dead(time);
            enemies.Remove(e);
        }

        Vector2 randomSpawn(Vector2 playerPos) {
            Random rand = new Random();
            int Y = rand.Next((int)playerPos.X - SpawnRange, Math.Abs((int)playerPos.X - SpawnRange) );
            int X = rand.Next((int)playerPos.Y - SpawnRange, Math.Abs((int)playerPos.Y - SpawnRange));
            return new Vector2(X, Y);            
        }

        void checkSpawn(Vector2 playerPos) {
            int temp = randTimer.Next(MaxTimer);
            if(enemies.Count < MaxEnemiesOnScreen) {
                if(timer > temp) {
                    timer = 0;
                    addEnemy(playerPos);
                }
            }
        }

        Enemy gender(Vector2 playerPos) {
            return (randTimer.Next(100) % 2) == 0 ? new Enemy(zombieB, zombieBSpawn, deathSprite, playerPos) 
                : new Enemy(zombieG, zombieGSpawn, deathSprite, playerPos);
        }

        #endregion

        #region EnemyManager Movement



        #endregion

        #region XNA EnemyManager

        public void Update(GameTime gameTime, Vector2 playerPos) {
            timer += gameTime.ElapsedGameTime.Milliseconds;

            checkSpawn(playerPos);

            for(int i = 0; i < enemies.Count; i++) {
                enemies[i].Update(gameTime, playerPos);
                if (enemies[i].fullyDead)
                    collision(enemies[i], timer);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            foreach(Enemy e in enemies) {
                e.Draw(gameTime, spriteBatch);
            }
        }

        #endregion
    }
}
