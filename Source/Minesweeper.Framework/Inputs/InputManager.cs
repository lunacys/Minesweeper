using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Minesweeper.Framework.Inputs
{
    public static class InputManager
    {
        /// <summary>
        /// Gets current mouse cursor position relative to the game window
        /// </summary>
        public static Vector2 MousePosition { get; private set; }

        /// <summary>
        /// Gets current mouse cursor velocity
        /// </summary>
        public static Vector2 MouseVelocity { get; private set; }

        /// <summary>
        /// Gets current mouse cursor rectangle. The size is 1 by 1 pixels
        /// </summary>
        public static RectangleF MouseRectangle
            => new RectangleF(MousePosition, new Size2(1, 1));

        public static KeyboardState CurrentKeyboardState => _keyboardState;
        public static KeyboardState OldKeyboardState => _oldKeyboardState;

        public static MouseState CurrentMouseState => _mouseState;
        public static MouseState OldMouseState => _oldMouseState;

        public static GamePadState CurrentGamePadState => _gamePadState;
        public static GamePadState OldGamePadState => _oldGamePadState;

        private static KeyboardState _keyboardState, _oldKeyboardState;
        private static MouseState _mouseState, _oldMouseState;
        private static GamePadState _gamePadState, _oldGamePadState;
        private static Vector2 _oldMousePosition;

        public static void Update(GameTime gameTime)
        {
            _oldKeyboardState = _keyboardState;
            _oldMouseState = _mouseState;
            _oldGamePadState = _gamePadState;
            _oldMousePosition = _oldMouseState.Position.ToVector2();

            _keyboardState = Keyboard.GetState();
            _mouseState = Mouse.GetState();
            _gamePadState = GamePad.GetState(PlayerIndex.One); // TODO: Add handling player index (not only the first player)
            MousePosition = _mouseState.Position.ToVector2();

            MouseVelocity = MousePosition - _oldMousePosition;
        }

        /// <summary>
        /// Gets whether was keyboard key pressed once
        /// </summary>
        /// <param name="key">Keyboard <see cref="Keys"/> to be handled</param>
        /// <returns>true if key was pressed, false otherwise</returns>
        public static bool WasKeyPressed(Keys key)
        {
            return _oldKeyboardState.IsKeyUp(key)
                   && _keyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Gets whether keyboard key was released after being pressed.
        /// </summary>
        /// <param name="key">Keyboard <see cref="Keys"/> to be handled</param>
        /// <returns>true if key was released, false otherwise</returns>
        public static bool WasKeyReleased(Keys key)
        {
            return _oldKeyboardState.IsKeyDown(key)
                   && _keyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Gets whether given key is currently being pressed.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsKeyDown(Keys key)
        {
            return _keyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Gets whether given key is currently being not pressed.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsKeyUp(Keys key)
        {
            return _keyboardState.IsKeyUp(key);
        }

        public static bool WasMouseButtonPressed(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return _oldMouseState.LeftButton == ButtonState.Released
                           && _mouseState.LeftButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return _oldMouseState.RightButton == ButtonState.Released
                           && _mouseState.RightButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return _oldMouseState.MiddleButton == ButtonState.Released
                           && _mouseState.MiddleButton == ButtonState.Pressed;
                case MouseButton.X1:
                    return _oldMouseState.XButton1 == ButtonState.Released
                           && _mouseState.XButton1 == ButtonState.Pressed;
                case MouseButton.X2:
                    return _oldMouseState.XButton2 == ButtonState.Released
                           && _mouseState.XButton2 == ButtonState.Pressed;
            }
            return false;
        }

        /// <summary>
        /// Gets whether a mouse button was released after a press
        /// </summary>
        /// <param name="button"><see cref="MouseButton"/></param>
        /// <returns>Whether a mouse button was released after a press</returns>
        public static bool WasMouseButtonReleased(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return _oldMouseState.LeftButton == ButtonState.Pressed
                           && _mouseState.LeftButton == ButtonState.Released;
                case MouseButton.Right:
                    return _oldMouseState.RightButton == ButtonState.Pressed
                           && _mouseState.RightButton == ButtonState.Released;
                case MouseButton.Middle:
                    return _oldMouseState.MiddleButton == ButtonState.Pressed
                           && _mouseState.MiddleButton == ButtonState.Released;
                case MouseButton.X1:
                    return _oldMouseState.XButton1 == ButtonState.Pressed
                           && _mouseState.XButton1 == ButtonState.Released;
                case MouseButton.X2:
                    return _oldMouseState.XButton2 == ButtonState.Pressed
                           && _mouseState.XButton2 == ButtonState.Released;
            }
            return false;
        }

        public static bool IsMouseButtonDown(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return _mouseState.LeftButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return _mouseState.RightButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return _mouseState.MiddleButton == ButtonState.Pressed;
                case MouseButton.X1:
                    return _mouseState.XButton1 == ButtonState.Pressed;
                case MouseButton.X2:
                    return _mouseState.XButton2 == ButtonState.Pressed;
            }
            return false;
        }

        public static bool IsMouseButtonUp(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return _mouseState.LeftButton == ButtonState.Released;
                case MouseButton.Right:
                    return _mouseState.RightButton == ButtonState.Released;
                case MouseButton.Middle:
                    return _mouseState.MiddleButton == ButtonState.Released;
                case MouseButton.X1:
                    return _mouseState.XButton1 == ButtonState.Released;
                case MouseButton.X2:
                    return _mouseState.XButton2 == ButtonState.Released;
            }
            return false;
        }

        public static bool IsGamePadConnected => _gamePadState.IsConnected;

        public static bool IsButtonDown(Buttons button)
        {
            return _gamePadState.IsButtonDown(button);
        }

        public static bool IsButtonUp(Buttons button)
        {
            return _gamePadState.IsButtonUp(button);
        }

        public static bool WasButtonPressed(Buttons button) =>
            _gamePadState.IsButtonDown(button) && _oldGamePadState.IsButtonUp(button);

        public static bool WasButtonReleased(Buttons button) =>
            _gamePadState.IsButtonUp(button) && _oldGamePadState.IsButtonDown(button);

        public static bool IsCtrlDown() => IsKeyDown(Keys.LeftControl) || IsKeyDown(Keys.RightControl);
        public static bool IsAltDown() => IsKeyDown(Keys.LeftAlt) || IsKeyDown(Keys.RightAlt);
        public static bool IsShiftDown() => IsKeyDown(Keys.LeftShift) || IsKeyDown(Keys.RightShift);
        
        public static bool WasWheelScrolled()
        {
            return _mouseState.ScrollWheelValue != _oldMouseState.ScrollWheelValue;
        }

        public static bool WasWheelScrolledUp()
        {
            return _mouseState.ScrollWheelValue > _oldMouseState.ScrollWheelValue;
        }

        public static bool WasWheelScrolledDown()
        {
            return _mouseState.ScrollWheelValue < _oldMouseState.ScrollWheelValue;
        }
    }
}