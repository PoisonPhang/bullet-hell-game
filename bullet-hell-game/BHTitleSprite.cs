using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;


namespace bullet_hell_game
{
    class BHTitleSprite
    {
        private Texture2D _texture;
        private const int TEXTURE_UNIT_WIDTH = 258;
        private const int TEXTURE_UNIT_HEIGHT = 64;
        private const int TEXTURE_FRAMES = 4 - 1;
        private double _animationTimer;
        private short _animationFrame;

        /// <summary>
        /// Loads the sprite texture using the provided ContentManager
        /// </summary>
        /// <param name="content">The ContentManager to load with</param>
        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("BH_Title");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time">The GameTime</param>
        public void Update(GameTime time)
        {
            // NOOP
        }

        /// <summary>
        /// Draws the sprite using the supplied SpriteBatch
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">The spritebatch to render with</param>
        public void Draw(GameTime time, SpriteBatch spriteBatch, int windowWidth, int windowHeight)
        {
            _animationTimer += time.ElapsedGameTime.TotalSeconds;

            if (_animationTimer > 0.7)
            {
                _animationFrame++;
                if (_animationFrame > TEXTURE_FRAMES) _animationFrame = 0;
                _animationTimer = 0;
            }

            var source = new Rectangle(_animationFrame * TEXTURE_UNIT_WIDTH, 0, TEXTURE_UNIT_WIDTH, TEXTURE_UNIT_HEIGHT);

            spriteBatch.Draw(_texture, new Vector2((windowWidth / 2) - (TEXTURE_UNIT_WIDTH / 2), windowHeight / 6), source, Color.White);
        }
    }
}
