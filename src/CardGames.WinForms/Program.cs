using System;
using System.Windows.Forms;

namespace CardGames.WinForms
{
    /// <summary>
    /// Main program entry point for the Windows Forms CardGames application
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}