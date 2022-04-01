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

        private Texture2D _texture;
        private Color _spiteColor;
        private Vector2 _unitSize;
        private int _unitSpeed = 2;
        private Vector2 _origin;
        private Vector2 _velocity;
        private string _textureAsset;
        private Random _random;

        public Target(World world, Vector2 position, Vector2 unitSize, string textureAsset)
        {
            _unitSize = unitSize;
            _origin = new Vector2(_unitSize.X / 2, _unitSize.Y / 2);
            _textureAsset = textureAsset;
            _spiteColor = Color.White;

            Body = world.CreateRectangle(_unitSize.X, _unitSize.Y, 1, position, 0, BodyType.Dynamic);
            _velocity = new Vector2(_unitSize.X * 3, _unitSize.Y);
            Body.LinearVelocity = _velocity;
            Body.IgnoreGravity = true;
            _random = new Random();


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
                _velocity = new Vector2(
                    _random.Next(
                        (int)-_unitSize.X * _unitSpeed,
                        (int)_unitSize.X * _unitSpeed
                    ),
                    _random.Next(
                        (int)-_unitSize.Y * _unitSpeed,
                        (int)_unitSize.Y * _unitSpeed
                    )
                );
                _spiteColor = Color.Red;
                _unitSpeed *= 2;
            }
            else
            {
                _spiteColor = Color.White;
            }

            Body.LinearVelocity += _velocity;
            Contact = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Body.Position, new Rectangle(0, 0, (int)_unitSize.X, (int)_unitSize.Y), _spiteColor, Body.Rotation, _origin, Vector2.One, SpriteEffects.None, 0f);
        }

        public FixtureCollection GetFixtures()
        {
            return Body.FixtureList;
        }

        bool CollisionHandler(Fixture fixture, Fixture other, Contact contact)
        {
            Contact = true;
            return true;
        }
    }
}
