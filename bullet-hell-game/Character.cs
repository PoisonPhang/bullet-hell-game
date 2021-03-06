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
        public BulletManager BulletManager { get; set; }

        private InputManager _inputManager;
        private Texture2D _texture;
        private Game1 _game;
        private World _world;
        private Color _spriteStatusColor;
        private Vector2 _unitSize;
        private Vector2 _origin;
        private string _textureAsset;
        private uint _unitSpeed;
        private uint _unitJumpHieght;

        public Character(Game1 game, World world, InputDevice inputDevice, Vector2 startingPos, string textureAsset, Vector2 unitSize, uint unitSpeed, uint unitJumpHieght)
        {
            _game = game;
            _world = world;
            _textureAsset = textureAsset;
            _unitSize = unitSize;
            _origin = new Vector2(_unitSize.X / 2, _unitSize.Y / 2);
            _unitSpeed = unitSpeed;
            _unitJumpHieght = unitJumpHieght;
            _inputManager = new InputManager(inputDevice, _unitSize, _unitSpeed, _unitJumpHieght);
            BulletManager = new BulletManager(_world, game);
            _spriteStatusColor = Color.White;
            startingPos = new Vector2(startingPos.X, startingPos.Y - (_unitSize.Y / 2));

            Body = _world.CreateRectangle(_unitSize.X, _unitSize.Y, 1, startingPos, 0, BodyType.Dynamic);
        }

        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>(_textureAsset);
            BulletManager.LoadContent(content);
        }

        public void Update(GameTime time)
        {
            _inputManager.Update(time);
            Body.LinearVelocity = _inputManager.Velocity;
            Body.Rotation = 0;

            BulletManager.Update();

            if (_inputManager.Shoot && _inputManager.Target.X > Body.Position.X + _origin.X)
            {
                Body.LinearVelocity -= BulletManager.SpawnBullet(Body.Position + new Vector2(_origin.X + 8, 0), _inputManager.Target);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_texture is null) throw new InvalidOperationException("Character's texture mustt be loaded with LoadContent() to render");

            spriteBatch.Draw(_texture, Body.Position, null, _spriteStatusColor, Body.Rotation, _origin, 1, SpriteEffects.None, 0);
            BulletManager.Draw(spriteBatch);
        }
    }
}
