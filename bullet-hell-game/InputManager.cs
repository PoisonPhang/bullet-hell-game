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
    /// Used to track which input device the `InputManager` should get input
    /// from
    /// </summary>
    public enum InputDevice
    {
        Gamepad0,
        Gamepad1,
        Keyboard,
    }

    /// <summary>
    /// Get's input applies it to player movement
    /// </summary>
    public class InputManager
    {
        /// <summary>
        /// Input Device being used to control the player
        /// </summary>
        public InputDevice InputDevice { get; private set; }
        /// <summary>
        /// Direction and Speed
        /// </summary>
        public Vector2 Velocity { get; private set; }

        public bool Shoot;
        public Vector2 Target;



        private Vector2 _unitSize;
        private uint _unitSpeedMax;
        private float _unitSpeed;
        private float _unitJump;
        private float _unitMaxJumpHieght;

        private KeyboardState _currentKeyboardState;
        private KeyboardState _priorKeyboardState;
        private MouseState _currentMouseState;
        private MouseState _priorMouseState;
        private GamePadState _currentGamePadState;
        private GamePadState _priorGamePadState;

        public InputManager(InputDevice inputDevice, Vector2 unitSize, uint unitSpeedMax, float unitMaxJumpHieght)
        {
            InputDevice = inputDevice;
            _unitSize = unitSize;
            _unitSpeedMax = unitSpeedMax;
            _unitMaxJumpHieght = -unitMaxJumpHieght * _unitSize.Y * 4.5f;
        }

        public void Update(GameTime time)
        {
            switch (InputDevice)
            {
                case InputDevice.Keyboard:
                    UpdateKeyboard(time);
                    break;
                case InputDevice.Gamepad0:
                    // TODO: GamePad movement
                    break;
                case InputDevice.Gamepad1:
                    // TODO: GamePad movement
                    break;
            }
        }

        private void UpdateKeyboard(GameTime time)
        {
            _priorKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            _priorMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            if (_currentKeyboardState.IsKeyDown(Keys.A) || _currentKeyboardState.IsKeyDown(Keys.D))
                _unitSpeed = (float)Math.Clamp(_unitSpeed + 0.1, 0, _unitSpeedMax);
            else
                _unitSpeed = (float)Math.Clamp(_unitSpeed - 0.1, 0, _unitSpeedMax);

            float speed = _unitSize.X * _unitSpeed;
            int direction;

            if (_currentKeyboardState.IsKeyDown(Keys.A) && _currentKeyboardState.IsKeyDown(Keys.D))
                direction = 0;
            else if (_currentKeyboardState.IsKeyDown(Keys.A))
                direction = -1;
            else if (_currentKeyboardState.IsKeyDown(Keys.D))
                direction = 1;
            else
                direction = 0;

            if (_currentKeyboardState.IsKeyDown(Keys.Space) && _priorKeyboardState.IsKeyUp(Keys.Space))
                _unitJump = _unitMaxJumpHieght;
            else
                _unitJump = (float)Math.Clamp(_unitJump + _unitSize.Y * 0.16f, _unitMaxJumpHieght, 0);

            if (_currentMouseState.LeftButton == ButtonState.Pressed && _priorMouseState.LeftButton == ButtonState.Released)
            {

                Shoot = true;
                Target = _currentMouseState.Position.ToVector2();
            }
            else
                Shoot = false;

            Velocity = new Vector2(direction * speed, _unitJump);
        }
    }
}
