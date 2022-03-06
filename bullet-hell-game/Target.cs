using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace bullet_hell_game
{
    class Target
    {
        public Body Body { get; protected set; }
        public bool Contact;
        public int Score = 0;

        private Texture2D _texture;
        private Color _spiteColor;
        private Vector2 _unitSize;
        private Vector2 _origin;
        private Vector2 _velocity;
        private string _textureAsset;
        private Vector2 _path;

        public Target(World world, Vector2 position, Vector2 unitSize, string textureAsset)
        {
            _unitSize = unitSize;
            _origin = new Vector2(_unitSize.X / 2, _unitSize.Y / 2);
            _textureAsset = textureAsset;
            _spiteColor = Color.White;

            Body = world.CreateRectangle(_unitSize.X, _unitSize.Y, 1, position, 0, BodyType.Dynamic);
            _path = new Vector2(32, -world.Gravity.Y);
            Body.LinearVelocity = _path;


            Body.OnCollision += CollisionHandler;
        }

        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>(_textureAsset);
        }

        public void Update()
        {
            if (Contact)
            {
                Body.LinearVelocity = -Body.LinearVelocity * 2;
                _path = Body.LinearVelocity * 3;
                Contact = false;
            }
            else
            {
                Body.LinearVelocity = _path;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Body.Position, new Rectangle(0, 0, (int)_unitSize.X, (int)_unitSize.Y), _spiteColor, Body.Rotation, _origin, Vector2.One, SpriteEffects.None, 0f);
        }

        bool CollisionHandler(Fixture fixture, Fixture other, Contact contact)
        {
            Contact = true;
            Score++;
            return true;
        }
    }
}
