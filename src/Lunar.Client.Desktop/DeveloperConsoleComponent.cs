using Lunar.Client.Utilities;
using Lunar.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunar.Client
{
    internal sealed class DeveloperConsoleComponent : DrawableGameComponent
    {
        private const int MaxOutputLines = 200;
        private const int MaxHistoryEntries = 100;
        private const int MaxInputLength = 256;
        private const int DisplayLineCount = 14;

        private readonly CommandInterpreter _interpreter;
        private readonly List<string> _outputLines = new List<string>();
        private readonly List<string> _commandHistory = new List<string>();

        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private Texture2D _pixel;
        private KeyboardState _previousKeyboardState;
        private string _input = string.Empty;
        private int _historyIndex = -1;

        public bool IsOpen { get; private set; }
        public Color FontColor { get; set; } = Color.Wheat;

        public DeveloperConsoleComponent(Game game, CommandInterpreter interpreter)
            : base(game)
        {
            _interpreter = interpreter;
        }

        public override void Initialize()
        {
            _previousKeyboardState = Lunar.Client.Utilities.Input.Input.Keyboard;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
            _font = Game.Content.LoadAsset<SpriteFont>(Constants.FILEPATH_GFX + "Fonts/chatFont");
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            _pixel?.Dispose();
            _spriteBatch?.Dispose();
            base.UnloadContent();
        }

        public void ToggleOpenClose()
        {
            IsOpen = !IsOpen;
            if (IsOpen)
                _historyIndex = -1;
        }

        public void AppendLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return;

            _outputLines.Add(line);
            if (_outputLines.Count > MaxOutputLines)
                _outputLines.RemoveAt(0);
        }

        public void Clear()
        {
            _outputLines.Clear();
        }

        public override void Update(GameTime gameTime)
        {
            var keyboardState = Lunar.Client.Utilities.Input.Input.Keyboard;

            if (IsOpen)
            {
                HandleSpecialKeys(keyboardState);
                HandleCharacterInput(keyboardState);
            }

            _previousKeyboardState = keyboardState;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!IsOpen || _font == null)
                return;

            var viewport = GraphicsDevice.Viewport;
            var width = viewport.Width;
            var height = Math.Max(220, viewport.Height / 3);
            var background = new Rectangle(0, 0, width, height);
            var inputY = height - 28;

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            _spriteBatch.Draw(_pixel, background, Color.Black * 0.80f);
            _spriteBatch.Draw(_pixel, new Rectangle(0, inputY - 4, width, 2), Color.Gray * 0.6f);

            int start = Math.Max(0, _outputLines.Count - DisplayLineCount);
            for (int i = start; i < _outputLines.Count; i++)
            {
                int lineIndex = i - start;
                _spriteBatch.DrawString(_font, _outputLines[i], new Vector2(8, 8 + (lineIndex * 14)), FontColor);
            }

            _spriteBatch.DrawString(_font, $"> {_input}_", new Vector2(8, inputY), Color.White);
            _spriteBatch.End();
        }

        private void HandleSpecialKeys(KeyboardState keyboardState)
        {
            if (IsNewKeyPress(keyboardState, Keys.Enter))
            {
                SubmitInput();
                return;
            }

            if (IsNewKeyPress(keyboardState, Keys.Back) && _input.Length > 0)
            {
                _input = _input[..^1];
            }

            if (IsNewKeyPress(keyboardState, Keys.Tab))
            {
                bool forward = !(keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift));
                _input = _interpreter.Autocomplete(_input, forward);
            }

            if (IsNewKeyPress(keyboardState, Keys.Up))
            {
                if (_commandHistory.Count == 0)
                    return;

                _historyIndex = Math.Min(_commandHistory.Count - 1, _historyIndex + 1);
                _input = _commandHistory[_commandHistory.Count - 1 - _historyIndex];
            }

            if (IsNewKeyPress(keyboardState, Keys.Down))
            {
                if (_commandHistory.Count == 0 || _historyIndex < 0)
                    return;

                _historyIndex--;
                _input = _historyIndex < 0
                    ? string.Empty
                    : _commandHistory[_commandHistory.Count - 1 - _historyIndex];
            }
        }

        private void SubmitInput()
        {
            var line = _input.Trim();
            if (string.IsNullOrEmpty(line))
            {
                _input = string.Empty;
                return;
            }

            AppendLine($"> {line}");
            _commandHistory.Add(line);
            if (_commandHistory.Count > MaxHistoryEntries)
                _commandHistory.RemoveAt(0);

            _historyIndex = -1;
            bool clearRequested = _interpreter.Execute(line, AppendLine);
            if (clearRequested)
                Clear();

            _input = string.Empty;
        }

        private void HandleCharacterInput(KeyboardState keyboardState)
        {
            bool shift = keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift);

            foreach (var key in keyboardState.GetPressedKeys())
            {
                if (!IsNewKeyPress(keyboardState, key))
                    continue;

                if (TryTranslateKey(key, shift, out char c) && _input.Length < MaxInputLength)
                    _input += c;
            }
        }

        private bool IsNewKeyPress(KeyboardState keyboardState, Keys key) =>
            keyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);

        private static bool TryTranslateKey(Keys key, bool shift, out char c)
        {
            c = '\0';

            if (key >= Keys.A && key <= Keys.Z)
            {
                char baseChar = (char)('a' + (key - Keys.A));
                c = shift ? char.ToUpperInvariant(baseChar) : baseChar;
                return true;
            }

            if (key >= Keys.D0 && key <= Keys.D9)
            {
                char[] normal = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                char[] shifted = { ')', '!', '@', '#', '$', '%', '^', '&', '*', '(' };
                int idx = key - Keys.D0;
                c = shift ? shifted[idx] : normal[idx];
                return true;
            }

            if (key >= Keys.NumPad0 && key <= Keys.NumPad9)
            {
                c = (char)('0' + (key - Keys.NumPad0));
                return true;
            }

            switch (key)
            {
                case Keys.Space: c = ' '; return true;
                case Keys.OemPeriod: c = shift ? '>' : '.'; return true;
                case Keys.OemComma: c = shift ? '<' : ','; return true;
                case Keys.OemMinus: c = shift ? '_' : '-'; return true;
                case Keys.OemPlus: c = shift ? '+' : '='; return true;
                case Keys.OemQuestion: c = shift ? '?' : '/'; return true;
                case Keys.OemSemicolon: c = shift ? ':' : ';'; return true;
                case Keys.OemQuotes: c = shift ? '"' : '\''; return true;
                case Keys.OemOpenBrackets: c = shift ? '{' : '['; return true;
                case Keys.OemCloseBrackets: c = shift ? '}' : ']'; return true;
                case Keys.OemPipe: c = shift ? '|' : '\\'; return true;
                case Keys.OemTilde: c = shift ? '~' : '`'; return true;
                default: return false;
            }
        }
    }
}
