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
    public class Character
    {
        public Body Body { get; protected set; }


        private InputManager _inputManager;
        private BulletManager _bulletManager;
        private Texture2D _texture;
        private World _world;
        private Color _spriteStatusColor;
        private Vector2 _unitSize;
        private Vector2 _origin;
        private string _textureAsset;
        private uint _unitSpeed;
        private uint _unitJumpHieght;

        public Character(World world, InputDevice inputDevice, Vector2 StartingPos, string textureAsset, Vector2 unitSize, uint unitSpeed, uint unitJumpHieght)
        {
            _world = world;
            _textureAsset = textureAsset;
            _unitSize = unitSize;
            _origin = new Vector2(_unitSize.X / 2, _unitSize.Y / 2);
            _unitSpeed = unitSpeed;
            _unitJumpHieght = unitJumpHieght;
            _inputManager = new InputManager(inputDevice, _unitSize, _unitSpeed, _unitJumpHieght);
            _bulletManager = new BulletManager(_world);
            _spriteStatusColor = Color.White;

            Body = _world.CreateRectangle(_unitSize.X, _unitSize.Y, 1, StartingPos, 0, BodyType.Dynamic);
        }

        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>(_textureAsset);
            _bulletManager.LoadContent(content);
        }

        public void Update(GameTime time)
        {
            _inputManager.Update(time);
            Body.LinearVelocity = _inputManager.Velocity;
            Body.Rotation = 0;

            _bulletManager.Update();

            if (_inputManager.Shoot && _inputManager.Target.X > Body.Position.X + _origin.X)
            {
                Body.LinearVelocity -= _bulletManager.SpawnBullet(Body.Position + new Vector2(_origin.X + 8, 0), _inputManager.Target);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_texture is null) throw new InvalidOperationException("Character's texture mustt be loaded with LoadContent() to render");

            spriteBatch.Draw(_texture, Body.Position, null, _spriteStatusColor, Body.Rotation, _origin, 1, SpriteEffects.None, 0);
            _bulletManager.Draw(spriteBatch);
        }
    }
}
