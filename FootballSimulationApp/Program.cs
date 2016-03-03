using System;
using System.Drawing;
using System.Windows.Forms;

namespace FootballSimulationApp
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var settings = Properties.Settings.Default;
            var drawingStrategy = new SimulationDrawingStrategy(
                Color.FromName(settings.LineColor),
                Color.FromName(settings.BallColor),
                new[] {Color.FromName(settings.Team1Color), Color.FromName(settings.Team2Color)},
                SystemFonts.CaptionFont);
            var form = new MainForm(
                new FixedTimeStepSimulationLoop(settings.TargetElapsedTime, settings.MaxElapsedTime),
                drawingStrategy)
            {BackColor = Color.FromName(settings.PitchColor)};

            Application.Run(form);
        }
    }
}