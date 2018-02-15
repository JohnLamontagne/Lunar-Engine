using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Lunar.Client.Utilities.Input
{
    public class CharacterEventArgs : EventArgs
    {
        private readonly char character;

        public CharacterEventArgs(char character)
        {
            this.character = character;
        }

        public char Character
        {
            get { return character; }
        }
    }

    public class KeyEventArgs : EventArgs
    {
        private Keys keyCode;

        public KeyEventArgs(Keys keyCode)
        {
            this.keyCode = keyCode;
        }

        public Keys KeyCode
        {
            get { return keyCode; }
        }
    }

    public delegate void CharEnteredHandler(object sender, CharacterEventArgs e);

    public delegate void KeyEventHandler(object sender, KeyEventArgs e);

    public static class EventInput
    {
        /// <summary>
        /// Event raised when a character has been entered.
        /// </summary>
        public static event CharEnteredHandler CharEntered;

        /// <summary>
        /// Event raised when a key has been pressed down. May fire multiple times due to keyboard repeat.
        /// </summary>
        public static event KeyEventHandler KeyDown;

        /// <summary>
        /// Event raised when a key has been released.
        /// </summary>
        public static event KeyEventHandler KeyUp;

        /// Initialize the TextInput with the given GameWindow.
        /// </summary>
        /// <param name="window">The XNA window to which text input should be linked.</param>
        public static void Initialize(GameWindow window)
        {
            window.TextInput += Window_TextInput;
           
            
        }

        private static void Window_TextInput(object sender, TextInputEventArgs e)
        {
            if (CharEntered != null)
                CharEntered(null, new CharacterEventArgs(e.Character));
        }
    }
}