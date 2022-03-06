using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

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

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Scene _currentScene;
        private World _world;

        private KeyboardState _currKeyboardState;
        private KeyboardState _lastKeyboardState;

        private BHTitleSprite _bhTitleSprite;
        private SpriteFont _mechanical;
        private GenericBGSprite[] _background;
        private Character _character;
        private Target target;
        private StaticFGSprite[] _fowardGround;
        private Song music;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _world = new World
            {
                Gravity = new Vector2(0, 16384)
            };
        }

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
                new StaticFGSprite(_world, new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height - 16), new Vector2(800, 16), "RedRectangle"),
                //new StaticFGSprite(_world, new Vector2(GraphicsDevice.Viewport.Width - 400, GraphicsDevice.Viewport.Height - 400), new Vector2(400, 16), "RedRectangle"),
                new StaticFGSprite(_world, new Vector2(0, GraphicsDevice.Viewport.Height - 128), new Vector2(400, 16), "RedRectangle"),
            };

            target = new Target(_world, new Vector2(GraphicsDevice.Viewport.Width / 2, 64), new Vector2(32, 32), "target");

            _character = new Character(_world, InputDevice.Keyboard, new Vector2(GraphicsDevice.Viewport.Width / 2 + 64, GraphicsDevice.Viewport.Height - 192), "green-rectangle", new Vector2(64, 128), 3, 2);

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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _bhTitleSprite.LoadContent(Content);

            foreach (GenericBGSprite bgSprite in _background)
            {
                bgSprite.LoadContent(Content);
            }

            foreach (StaticFGSprite fgSprite in _fowardGround)
            {
                fgSprite.LoadContent(Content);
            }


            target.LoadContent(Content);
            music = Content.Load<Song>("into-the-night-20928");
            _character.LoadContent(Content);

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(music);

            _mechanical = Content.Load<SpriteFont>("mechanical");
        }

        protected override void Update(GameTime gameTime)
        {
            _lastKeyboardState = _currKeyboardState;
            _currKeyboardState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || _currKeyboardState.IsKeyDown(Keys.Escape))
                Exit();

            if (_currentScene == Scene.TitleScreen && _currKeyboardState.IsKeyDown(Keys.Enter) && _lastKeyboardState.IsKeyUp(Keys.Enter))
                _currentScene = Scene.Game;


            _character.Update(gameTime);

            target.Update();

            _world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            _spriteBatch.Begin();

            int i = 0;
            float xScale;
            foreach (GenericBGSprite bgSprite in _background)
            {
                if (i < 2)
                    xScale = 3;
                else
                    xScale = 1.5f;

                bgSprite.Draw(_spriteBatch, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, xScale, 3);

                i++;
            }

            switch (_currentScene)
            {
                case Scene.TitleScreen:
                    DrawTitleScreen(gameTime);
                    break;
                case Scene.Game:
                    foreach (StaticFGSprite fgSprite in _fowardGround)
                    {
                        fgSprite.Draw(_spriteBatch);
                    }
                    _character.Draw(_spriteBatch);
                    target.Draw(_spriteBatch);
                    break;
            }

            _spriteBatch.DrawString(_mechanical, target.Score.ToString(), Vector2.Zero, Color.White);


            _spriteBatch.End();

            base.Draw(gameTime);
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
    }
}
