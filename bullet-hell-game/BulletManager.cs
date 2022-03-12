using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using System.Collections;
using Microsoft.Xna.Framework.Audio;

namespace bullet_hell_game
{
    public class BulletManager
    {
        public uint Score { get; private set; }

        private World _world;
        private Texture2D _texture;
        private ArrayList _bullets;
        private SoundEffect _shootSound;
        private Game1 _game;

        public BulletManager(World world, Game1 game)
        {
            _world = world;
            _bullets = new ArrayList();
            _game = game;
        }

        public Vector2 SpawnBullet(Vector2 startingPosition, Vector2 targetPosition)
        {
            var velocity = new Vector2();
            _bullets.Add(new Bullet(_world, startingPosition, targetPosition, _texture, _game, IncermentScore, out velocity));
            _shootSound.Play();

            return velocity;
        }

        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("bullet");
            _shootSound = content.Load<SoundEffect>("shoot");
        }


        public void Update()
        {
            ArrayList bullets = new ArrayList();

            foreach (Bullet bullet in _bullets)
            {
                bullet.Update();
                if (!bullet.Contact)
                {
                    bullets.Add(bullet);
                }
            }

            _bullets = bullets;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Bullet bullet in _bullets)
                bullet.Draw(spriteBatch);
        }

        public uint IncermentScore()
        {
            return Score++;
        }
    }

    class Bullet
    {
        Body Body;
        Vector2 TargetVelocity;
        public bool Contact;

        private Vector2 _unitSize = new Vector2(8, 4);
        private Vector2 _origin = new Vector2(4, 2);
        private float _unitSpeed = -600000;
        private Texture2D _texture;
        private Func<uint> _incermentScore;
        private Game1 _game;


        public Bullet(World world, Vector2 startingPosition, Vector2 targetPosition, Texture2D texture, Game1 game, Func<uint> incermentScore, out Vector2 velocity)
        {
            float x = startingPosition.X - targetPosition.X;
            float y = startingPosition.Y - targetPosition.Y;
            float rotation = (float)Math.Atan2(x, y);
            TargetVelocity = new Vector2(x * _unitSpeed, y * _unitSpeed);

            Body = world.CreateRectangle(_unitSize.X, _unitSize.Y, 1, startingPosition, rotation, BodyType.Dynamic);
            Body.LinearVelocity = TargetVelocity;
            Body.OnCollision += CollisionHandler;

            _game = game;
            _incermentScore = incermentScore;
            _texture = texture;
            velocity = TargetVelocity;
        }

        public void Update()
        {
            Body.LinearVelocity = TargetVelocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Body.Position, null, Color.White, Body.Rotation, _origin, 1, SpriteEffects.None, 0);
        }

        bool CollisionHandler(Fixture fixture, Fixture other, Contact contact)
        {
            if (!Contact && _game.GetTargetFixtures().Contains(other))
            {
                _game.PlaceExposion(Body.Position);
                _incermentScore();
            }


            Contact = true;
            return true;
        }
    }
}
