using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace BQM
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Player player, playerTwo;
        Camera camera;
        BlasterManager blaster;
        EnemyManager enemyManager;
        Thread playerTh, enemyTh;
        static int points = 0;
        bool twoP = false;

        KeyboardState current, previous;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            /*graphics.PreferredBackBufferHeight = 400;
            graphics.PreferredBackBufferWidth = 400;
            graphics.ApplyChanges();
           */
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            camera = new Camera(GraphicsDevice.Viewport, Vector2.Zero, Content.Load<SpriteFont>(@"SpriteFont1"));
            base.Initialize();
            twoP = false;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //Player 1 is 48/70
            //Player 2 is 52/80
            player = new Player(Content.Load<Texture2D>(@"Player1/Player 1"), Content.Load<Texture2D>(@"Player1/P1Shoot"));
            playerTwo = new Player(Content.Load<Texture2D>(@"Player1/P2"), Content.Load<Texture2D>(@"Player1/P1Shoot"));
            // TODO: use this.Content to load your game content here
            blaster = new BlasterManager(Content.Load<Texture2D>(@"Weapons/blast"));
            enemyManager = new EnemyManager(Content.Load<Texture2D>(@"Enemy/ZOMBIE-B"), Content.Load<Texture2D>(@"Enemy/ZOMBIE-G"),
                                            Content.Load<Texture2D>(@"Enemy/ZOMBIE-B SPAWN"), Content.Load<Texture2D>(@"Enemy/ZOMBIE-G SPAWN"),
                                            Content.Load<Texture2D>(@"Enemy/Death"));

        
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            previous = current;
            current = Keyboard.GetState();
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            camera.Update(gameTime, current, previous);
            camera.Follow(player, 0);
            player.Update(gameTime);
            if(twoP)
                playerTwo.Update(gameTime);
            blaster.Update(gameTime, spriteBatch, player.Rotation, player.Position, player.ObjectDirection, player.AmIMoving);
            enemyManager.Update(gameTime, player.Position);
            checkProjectiles(blaster, enemyManager);
            if(player.Status != GameObjectStatus.Invulnerable)
                checkYourself(player, enemyManager);
            // TODO: Add your update logic here
            
            camera.Score += points;
            points = 0;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.Transform);
            player.Draw(gameTime, spriteBatch);
            if(twoP)
                playerTwo.Draw(gameTime, spriteBatch);
            blaster.Draw(gameTime, spriteBatch);
            enemyManager.Draw(gameTime, spriteBatch);
            camera.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
        static public bool collision(Rectangle rA, Color [] dA, Rectangle rB, Color [] dB) {
            int top = Math.Max(rA.Top, rB.Top);
            int bottom = Math.Min(rA.Bottom, rB.Bottom);
            int left = Math.Max(rA.Left, rB.Left);
            int right = Math.Min(rA.Right, rB.Right);

            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    Color colorA = dA[(x - rA.Left) + (y - rA.Top) * rA.Width];
                    Color colorB = dB[(x - rB.Left) + (y - rB.Top) * rB.Width];

                    if (colorA.A != 0 && colorB.A != 0)
                        return true;
                }
            }
            return false;
        }

        static public bool intersects(Rectangle a, Rectangle b)
        {
            return (a.Right > b.Left && a.Left < b.Right &&
                    a.Bottom > b.Bottom && a.Top < b.Bottom);
        }

        static void checkProjectiles(BlasterManager bm, EnemyManager em) {
            for(int i = 0; i < em.enemies.Count; i++) {
                Enemy e = em.enemies[i];
                if (em.enemies[i].Status == GameObjectStatus.Invulnerable)
                    break;
                Rectangle enemy = em.enemies[i].boundary();
                for(int j = 0; j < bm.projectiles.Count; j++) {                    
                    Blaster b = bm.projectiles[j];
                    Rectangle projectile = b.boundary();
                    if( intersects(enemy,projectile) ) {
                        if(!(e.Status == GameObjectStatus.Dead)) {
                            if (collision(projectile, b.SpriteData, enemy, e.SpriteData))
                            {                            
                                e.Status = GameObjectStatus.Dead;
                                b.Status = GameObjectStatus.Dead;
                                points += 50;
                            }
                        }
                    }

                }
            }
        }
        static void checkYourself(Player p, EnemyManager em)
        {
            Rectangle me = p.boundary();
            for(int i = 0; i < em.enemies.Count; i++) {
                Enemy e = em.enemies[i];
                Rectangle enemy = e.boundary();
                if( intersects(enemy, me) ) {
                    if (!(e.Status == GameObjectStatus.Dead)) {
                        if (collision(me, p.SpriteData, enemy, e.SpriteData)) {
                            points -= 200;
                            p.Status = GameObjectStatus.Invulnerable;
                        }
                    }
                }
            }
        }
    }
}
