using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace bullet_hell_game
{
    /// <summary>
    /// Generic Sprite class for loading unchanging background elements
    /// </summary>
    class GenericBGSprite
    {
        private string _asset;
        private Vector2 _position;
        private Texture2D _texture;

        /// <summary>
        /// Construct a new GenericBGSprite
        /// </summary>
        /// <param name="assest">Texture asset name</param>
        /// <param name="position">Texture position</param>
        public GenericBGSprite(string assest, Vector2 position)
        {
            _asset = assest;
            _position = position;
        }

        /// <summary>
        /// Load content of the texutre into the ContentManager
        /// </summary>
        /// <param name="content">Current ContentManager</param>
        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>(_asset);
        }

        /// <summary>
        /// Draw the sprite
        /// </summary>
        /// <param name="spriteBatch">Current SpriteBatch</param>
        public void Draw(SpriteBatch spriteBatch, int width, int height, float xScale, float yScale)
        {
            spriteBatch.Draw(_texture, _position, new Rectangle(0, 0, width, height), Color.White, 0.0f, new Vector2(0, 0), new Vector2(xScale, yScale), SpriteEffects.None, 0.0f);
        }
    }
}
