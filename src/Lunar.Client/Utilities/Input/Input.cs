/** Copyright 2018 John Lamontagne https://www.rpgorigin.com

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Lunar.Client.GUI.Widgets;

namespace Lunar.Client.Utilities.Input
{
    /// <summary>
    /// The single source of keyboard and mouse state for the client. Gameplay and GUI code read
    /// <see cref="Keyboard"/> and <see cref="Mouse"/> instead of MonoGame's static device queries, so
    /// that test automation can inject input that flows through exactly the same code paths a player's
    /// hardware does. <see cref="Update"/> is called once per frame, first thing, by the game loop.
    ///
    /// Virtual input is additive for the keyboard (a key is down if hardware or automation holds it)
    /// and replaces the hardware mouse once automation has positioned it.
    /// </summary>
    public static class Input
    {
        private static readonly object Gate = new object();

        private static readonly HashSet<Keys> HeldKeys = new HashSet<Keys>();
        private static readonly Dictionary<Keys, int> KeyReleaseCountdown = new Dictionary<Keys, int>();
        private static readonly Queue<char> PendingCharacters = new Queue<char>();

        private static bool _virtualMouseActive;
        private static Point _virtualMousePosition;
        private static ButtonState _virtualLeft, _virtualRight, _virtualMiddle;
        private static int _leftReleaseCountdown = -1, _rightReleaseCountdown = -1, _middleReleaseCountdown = -1;

        public static KeyboardState Keyboard { get; private set; }

        public static MouseState Mouse { get; private set; }

        /// <summary>Frames processed so far; lets automation wait for "at least one more frame".</summary>
        public static long Frame { get; private set; }

        /// <summary>Samples hardware, applies virtual input, and delivers injected text. Game thread only.</summary>
        public static void Update()
        {
            var hardwareKeyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            var hardwareMouse = Microsoft.Xna.Framework.Input.Mouse.GetState();

            char[] characters;

            lock (Gate)
            {
                Frame++;

                Keyboard = HeldKeys.Count == 0
                    ? hardwareKeyboard
                    : new KeyboardState(hardwareKeyboard.GetPressedKeys().Concat(HeldKeys).Distinct().ToArray());

                // Expire timed key presses only after this frame's state has observed them, so a
                // one-frame tap is visible for exactly one Update.
                foreach (var key in KeyReleaseCountdown.Keys.ToList())
                {
                    if (--KeyReleaseCountdown[key] <= 0)
                    {
                        KeyReleaseCountdown.Remove(key);
                        HeldKeys.Remove(key);
                    }
                }

                if (_virtualMouseActive)
                {
                    Mouse = new MouseState(_virtualMousePosition.X, _virtualMousePosition.Y, hardwareMouse.ScrollWheelValue,
                        _virtualLeft, _virtualMiddle, _virtualRight, ButtonState.Released, ButtonState.Released);

                    // Expire timed button presses after this frame has observed them.
                    if (_leftReleaseCountdown > 0 && --_leftReleaseCountdown == 0) _virtualLeft = ButtonState.Released;
                    if (_rightReleaseCountdown > 0 && --_rightReleaseCountdown == 0) _virtualRight = ButtonState.Released;
                    if (_middleReleaseCountdown > 0 && --_middleReleaseCountdown == 0) _virtualMiddle = ButtonState.Released;
                }
                else
                {
                    Mouse = hardwareMouse;
                }

                characters = PendingCharacters.ToArray();
                PendingCharacters.Clear();
            }

            foreach (var c in characters)
                EventInput.InjectCharacter(c);
        }

        /// <summary>Automation entry points. Thread-safe; take effect on the next <see cref="Update"/>.</summary>
        public static class Virtual
        {
            public static void KeyDown(Keys key)
            {
                lock (Gate) { HeldKeys.Add(key); KeyReleaseCountdown.Remove(key); }
            }

            public static void KeyUp(Keys key)
            {
                lock (Gate) { HeldKeys.Remove(key); KeyReleaseCountdown.Remove(key); }
            }

            /// <summary>Holds a key for <paramref name="frames"/> frames (default 1) then releases it.</summary>
            public static void Tap(Keys key, int frames = 1)
            {
                lock (Gate) { HeldKeys.Add(key); KeyReleaseCountdown[key] = frames < 1 ? 1 : frames; }
            }

            public static void MouseMove(int x, int y)
            {
                lock (Gate) { _virtualMouseActive = true; _virtualMousePosition = new Point(x, y); }
            }

            public static void MouseButton(MouseButtons button, bool down)
            {
                lock (Gate)
                {
                    _virtualMouseActive = true;
                    var state = down ? ButtonState.Pressed : ButtonState.Released;
                    switch (button)
                    {
                        case MouseButtons.Left: _virtualLeft = state; _leftReleaseCountdown = -1; break;
                        case MouseButtons.Right: _virtualRight = state; _rightReleaseCountdown = -1; break;
                    }
                }
            }

            /// <summary>Moves to (x, y), presses the button for one frame, then releases it.</summary>
            public static void Click(int x, int y, MouseButtons button = MouseButtons.Left)
            {
                lock (Gate)
                {
                    _virtualMouseActive = true;
                    _virtualMousePosition = new Point(x, y);
                    switch (button)
                    {
                        case MouseButtons.Left: _virtualLeft = ButtonState.Pressed; _leftReleaseCountdown = 1; break;
                        case MouseButtons.Right: _virtualRight = ButtonState.Pressed; _rightReleaseCountdown = 1; break;
                    }
                }
            }

            /// <summary>Queues text to be delivered as typed characters on the next frame.</summary>
            public static void Text(string text)
            {
                if (string.IsNullOrEmpty(text)) return;
                lock (Gate) { foreach (var c in text) PendingCharacters.Enqueue(c); }
            }

            /// <summary>Releases every virtual key and button and hands the mouse back to hardware.</summary>
            public static void Reset()
            {
                lock (Gate)
                {
                    HeldKeys.Clear();
                    KeyReleaseCountdown.Clear();
                    PendingCharacters.Clear();
                    _virtualMouseActive = false;
                    _virtualLeft = _virtualRight = _virtualMiddle = ButtonState.Released;
                    _leftReleaseCountdown = _rightReleaseCountdown = _middleReleaseCountdown = -1;
                }
            }
        }
    }
}
