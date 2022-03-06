using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;


namespace bullet_hell_game
{
    public class StaticFGSprite
    {
        public Body Body { get; protected set; }

        private Texture2D _texture;
        private Color _spiteColor;
        private Vector2 _unitSize;
        private Vector2 _origin;
        private string _textureAsset;

        public StaticFGSprite(World world, Vector2 position, Vector2 unitSize, string textureAsset)
        {
            _unitSize = unitSize;
            _origin = new Vector2(_unitSize.X / 2, _unitSize.Y / 2);
            _textureAsset = textureAsset;
            _spiteColor = Color.White;

            Body = world.CreateRectangle(_unitSize.X, _unitSize.Y, 1, position, 0, BodyType.Static);
        }

        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>(_textureAsset);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_texture is null) throw new InvalidOperationException("Texture must be loaded with LoadContent()");

            spriteBatch.Draw(_texture, Body.Position, new Rectangle(0, 0, (int)_unitSize.X, (int)_unitSize.Y), _spiteColor, Body.Rotation, _origin, Vector2.One, SpriteEffects.None, 0f);
        }

    }
}
