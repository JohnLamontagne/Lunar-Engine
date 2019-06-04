using System;
using System.Windows.Forms;
using Lunar.Editor;

namespace Example
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SuiteForm());
        }
    }
}
