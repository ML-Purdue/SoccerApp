using System;
using System.Drawing;
using System.Windows.Forms;
using FootballSimulation;

namespace FootballSimulationApp
{
    internal sealed partial class MainForm : Form
    {
        private readonly Color _clearColor = Color.Green;
        private readonly ISimulationDrawingStrategy _drawingStrategy;
        private readonly FixedTimeStepGameLoop _gameLoop;
        private readonly Simulation _simulation;
        private Bitmap _backBuffer = new Bitmap(1, 1);

        public MainForm()
        {
            InitializeComponent();

            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);

            _simulation = SimulationFactory.Create2V2Simulation();
            _drawingStrategy = new SimulationDrawingStrategy(Color.White, Color.Black,
                new[] {Color.OrangeRed, Color.Blue}, new Font("Arial", 8));
            _gameLoop = new FixedTimeStepGameLoop(
                TimeSpan.FromTicks(TimeSpan.TicksPerSecond/60),
                TimeSpan.FromTicks(TimeSpan.TicksPerSecond/10),
                t => _simulation.Simulate((float) t.TotalSeconds),
                t => Invalidate());

            Application.Idle += Application_Idle;
        }

        private int ClientWidth => ClientSize.Width;

        private int ClientHeight => ClientSize.Height;

        private static float ScaleToFit(SizeF size, float width, float height)
            => size.Width/size.Height >= width/height ? width/size.Width : height/size.Height;

        protected override void Dispose(bool disposing)
        {
            Application.Idle -= Application_Idle;
            _backBuffer.Dispose();
            _drawingStrategy.Dispose();
            base.Dispose(disposing);
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            while (!NativeMethods.IsMessageAvailable)
                _gameLoop.Tick();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _backBuffer = new Bitmap(ClientWidth, ClientHeight);
            _gameLoop.Start();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            _backBuffer.Dispose();
            _backBuffer = new Bitmap(ClientWidth, ClientHeight);
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            using (var g = Graphics.FromImage(_backBuffer))
            {
                var clientHeight = ClientHeight - menuStrip.Height;
                var s = 0.9f*ScaleToFit(_simulation.PitchBounds.Size, ClientWidth, ClientHeight);

                g.Clear(_clearColor);
                g.TranslateTransform(ClientWidth/2f, clientHeight/2f + menuStrip.Height);
                g.ScaleTransform(s, s);

                _drawingStrategy.Draw(g, _simulation);
            }

            e.Graphics.DrawImageUnscaled(_backBuffer, Point.Empty);
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Close();

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aboutBox = new AboutBox();
            aboutBox.Show();
            aboutBox.Closed += (s, a) => aboutBox.Dispose();
        }
    }
}