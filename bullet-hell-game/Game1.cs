using System;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

using bullet_hell_game.Particles;

namespace bullet_hell_game
{
    public enum Scene
    {
        TitleScreen,
        Game
    }

    public class Game1 : Game
    {
        private const string START_MESSAGE = "Press [Enter] to Continue";
        private const string EXIT_MESSAGE = "Press [ESC] to Quit";
        private const string SHOOT_MESSAGE = "Aim with mouse, left click to shoot.";
        private const int MAX_EXPLOSIONS = 128;
        private const float GRAVITY = 8048;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private World _world;
        private TiledMap _tiledMap;
        private TiledMapRenderer _tiledMapRenderer;
        private Scene _currentScene;

        private KeyboardState _currKeyboardState;
        private KeyboardState _lastKeyboardState;

        private ExplosionParticleSystem _explosionParticleSystem;

        private BHTitleSprite _bhTitleSprite;
        private SpriteFont _mechanical;
        private GenericBGSprite[] _background;
        private Character _character;
        private Target _target;
        private StaticFGSprite[] _fowardGround;
        private Song _music;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _world = new World
            {
                Gravity = new Vector2(0, GRAVITY)
            };
        }

        public FixtureCollection GetTargetFixtures() => _target.GetFixtures();
        public void PlaceExposion(Vector2 where) => _explosionParticleSystem.PlaceExplosion(where);

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _currentScene = Scene.TitleScreen;
            _bhTitleSprite = new BHTitleSprite();

            _background = new GenericBGSprite[]
            {
                new GenericBGSprite("parallax-mountain-bg", new Vector2(0,0)),
                new GenericBGSprite("parallax-mountain-montain-far", new Vector2(0,0)),
                new GenericBGSprite("parallax-mountain-mountains", new Vector2(0,0)),
                new GenericBGSprite("parallax-mountain-trees", new Vector2(0,0)),
                new GenericBGSprite("parallax-mountain-foreground-trees", new Vector2(0,0)),
            };

            _fowardGround = new StaticFGSprite[]
            {
            };

            _target = new Target(_world, new Vector2(GraphicsDevice.Viewport.Width / 2, 64), new Vector2(32, 32), "target");


            var top = 0;
            var bottom = GraphicsDevice.Viewport.Height;
            var left = 0;
            var right = GraphicsDevice.Viewport.Width;

            var edges = new Body[]
            {
                _world.CreateEdge(new Vector2(left, top), new Vector2(right, top)),
                _world.CreateEdge(new Vector2(left, top), new Vector2(left, bottom)),
                _world.CreateEdge(new Vector2(left, bottom), new Vector2(right, bottom)),
                _world.CreateEdge(new Vector2(right, top), new Vector2(right, bottom))
            };

            foreach (var edge in edges)
            {
                edge.BodyType = BodyType.Static;
            }

            _explosionParticleSystem = new ExplosionParticleSystem(this, MAX_EXPLOSIONS);
            Components.Add(_explosionParticleSystem);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _tiledMap = Content.Load<TiledMap>("map-01");
            _tiledMapRenderer = new TiledMapRenderer(GraphicsDevice, _tiledMap);
            CreateMapBodies();

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            TiledMapRectangleObject spawn = (TiledMapRectangleObject)_tiledMap.GetLayer<TiledMapObjectLayer>("spawns").Objects.GetValue(0);

            Vector2 spawnPoint = spawn.Position;

            _character = new Character(this, _world, InputDevice.Keyboard, spawn.Position, "red-player", new Vector2(32, 32), 3, 2);

            _bhTitleSprite.LoadContent(Content);

            foreach (GenericBGSprite bgSprite in _background)
            {
                bgSprite.LoadContent(Content);
            }

            foreach (StaticFGSprite fgSprite in _fowardGround)
            {
                fgSprite.LoadContent(Content);
            }



            _target.LoadContent(Content);
            _music = Content.Load<Song>("into-the-night-20928");
            _character.LoadContent(Content);

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_music);

            _mechanical = Content.Load<SpriteFont>("mechanical");
        }

        protected override void Update(GameTime gameTime)
        {
            _lastKeyboardState = _currKeyboardState;
            _currKeyboardState = Keyboard.GetState();

            _tiledMapRenderer.Update(gameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || _currKeyboardState.IsKeyDown(Keys.Escape))
                Exit();

            if (_currentScene == Scene.TitleScreen && _currKeyboardState.IsKeyDown(Keys.Enter) && _lastKeyboardState.IsKeyUp(Keys.Enter))
                _currentScene = Scene.Game;

            _character.Update(gameTime);

            _target.Update();

            _world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            DrawBackground();

            _spriteBatch.Begin();

            switch (_currentScene)
            {
                case Scene.TitleScreen:
                    DrawTitleScreen(gameTime);
                    break;
                case Scene.Game:
                    DrawFowardground();
                    _tiledMapRenderer.Draw();
                    break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawBackground()
        {
            _spriteBatch.Begin();
            _background[0].Draw(_spriteBatch, 3);
            _background[1].Draw(_spriteBatch, 3);
            _spriteBatch.End();

            float characterX = MathHelper.Clamp(_character.Body.Position.X, 0, 1632);
            float offset = -characterX;
            Matrix transform;

            for (int i = 2; i < _background.Length; i++)
            {
                transform = Matrix.CreateTranslation(offset * ((float)i / _background.Length), 0, 0);
                _spriteBatch.Begin(transformMatrix: transform);
                _background[i].Draw(_spriteBatch, 3);
                _spriteBatch.End();
            }
        }

        private void DrawFowardground()
        {
            foreach (StaticFGSprite fgSprite in _fowardGround)
            {
                fgSprite.Draw(_spriteBatch);
            }


            _character.Draw(_spriteBatch);
            _target.Draw(_spriteBatch);

            _spriteBatch.DrawString(_mechanical, _character.BulletManager.Score.ToString(), Vector2.Zero, Color.White);
            _spriteBatch.DrawString(_mechanical, SHOOT_MESSAGE, new Vector2(GraphicsDevice.Viewport.Width / 2 - GetOffset(SHOOT_MESSAGE), GraphicsDevice.Viewport.Height - 26), Color.White);

        }

        private void DrawTitleScreen(GameTime time)
        {
            _bhTitleSprite.Draw(time, _spriteBatch, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            _spriteBatch.DrawString(
                _mechanical,
                START_MESSAGE,
                new Vector2(GraphicsDevice.Viewport.Width / 2 - GetOffset(START_MESSAGE), (GraphicsDevice.Viewport.Height / 8) * 6),
                Color.White);

            _spriteBatch.DrawString(
                _mechanical,
                EXIT_MESSAGE,
                new Vector2(GraphicsDevice.Viewport.Width / 2 - GetOffset(EXIT_MESSAGE), (GraphicsDevice.Viewport.Height / 8) * 7),
                Color.White);
        }

        private float GetOffset(string message)
        {
            return _mechanical.MeasureString(message).X / 2;
        }

        private void CreateMapBodies()
        {
            TiledMapTileLayer layer = _tiledMap.GetLayer<TiledMapTileLayer>("ground");

            int uX = layer.TileWidth;
            int uY = layer.TileHeight;

            foreach (TiledMapTile tile in layer.Tiles)
            {
                if (!tile.IsBlank)
                {
                    _world.CreateRectangle(
                        uX,
                        uY,
                        1,
                        new Vector2(tile.X * uX + uX / 2, tile.Y * uY + uY / 2),
                        0,
                        BodyType.Static);

                }
            }
        }
    }
}
