using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

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

            var document = new XmlDocument();
            document.Load("Settings.xml");

            var targetFramesPerSecond = GetElementInnerTextAsInteger(document, "TargetFramesPerSecond");
            var maxFrameLag = GetElementInnerTextAsInteger(document, "MaxFrameLag");
            var fontSize = GetElementInnerTextAsInteger(document, "FontSize");
            var pitchColor = GetElementInnerText(document, "PitchColor");
            var lineColor = GetElementInnerText(document, "LineColor");
            var ballColor = GetElementInnerText(document, "BallColor");
            var teamColors = from n in document.GetElementsByTagName("TeamColors")[0].ChildNodes.Cast<XmlNode>()
                select n.InnerText;

            using (var font = new Font(SystemFonts.CaptionFont.Name, fontSize))
            using (
                var drawingStrategy = new SimulationDrawingStrategy(Color.FromName(lineColor), Color.FromName(ballColor),
                    (from n in teamColors select Color.FromName(n)).ToList(), font))
            using (
                var form = new MainForm(
                    new FixedTimeStepSimulationLoop(
                        TimeSpan.FromTicks(TimeSpan.TicksPerSecond/targetFramesPerSecond),
                        TimeSpan.FromTicks(TimeSpan.TicksPerSecond/targetFramesPerSecond*maxFrameLag)),
                    drawingStrategy))
            {
                form.BackColor = Color.FromName(pitchColor);
                Application.Run(form);
            }
        }

        private static string GetElementInnerText(XmlDocument document, string name)
            => document.GetElementsByTagName(name)[0].InnerText;

        private static int GetElementInnerTextAsInteger(XmlDocument document, string name)
            => int.Parse(GetElementInnerText(document, name));
    }
}