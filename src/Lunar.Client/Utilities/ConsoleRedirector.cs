using System.IO;
using System.Text;
using QuakeConsole;

namespace Lunar.Client.Utilities
{
    public class ConsoleRedirector : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;

        private ConsoleComponent _consoleComponent;

        public ConsoleRedirector(ConsoleComponent consoleComponent)
        {
            _consoleComponent = consoleComponent;
        }


        public override void Write(string value)
        {
            _consoleComponent.Output.Append(value);
        }

        public override void WriteLine(string value)
        {
            _consoleComponent.Output.Append(value);
        }
    }
}
