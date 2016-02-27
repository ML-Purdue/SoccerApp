using System;
using System.Drawing;
using System.Windows.Forms;
using FootballSimulation;

namespace FootballSimulationApp
{
    internal sealed partial class MainForm : Form
    {
        private readonly Color _clearColor = Color.Green;

        private readonly ISimulationDrawingStrategy _drawingStrategy =
            new SimulationDrawingStrategy(Color.White, Color.Black, new[] {Color.OrangeRed, Color.Blue});

        private readonly FixedTimeStepGameLoop _gameLoop;
        private readonly Simulation _simulation = SimulationFactory.Create2V2Simulation();
        private Bitmap _backBuffer;

        public MainForm()
        {
            InitializeComponent();

            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);

            _gameLoop = new FixedTimeStepGameLoop(
                TimeSpan.FromTicks(TimeSpan.TicksPerSecond/60),
                TimeSpan.FromTicks(TimeSpan.TicksPerSecond/10),
                t => _simulation.Simulate((float)t.TotalSeconds),
                t => { Invalidate(); });

            Application.Idle += (sender, e) =>
            {
                while (!NativeMethods.IsMessageAvailable)
                    _gameLoop.Tick();
            };
        }

        private static float ScaleToFit(SizeF size, float width, float height)
            => size.Width / size.Height >= width / height ? width / size.Width : height / size.Height;

        protected override void Dispose(bool disposing)
        {
            _backBuffer?.Dispose();
            base.Dispose(disposing);
        }
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            _backBuffer = new Bitmap(ClientSize.Width, ClientSize.Height);
            _gameLoop.Start();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            _backBuffer?.Dispose();
            _backBuffer = new Bitmap(ClientSize.Width, ClientSize.Height);
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            using (var g = Graphics.FromImage(_backBuffer))
            {
                var clientHeight = ClientSize.Height - menuStrip.ClientSize.Height;
                var s = 0.9f*ScaleToFit(_simulation.PitchBounds.Size, ClientSize.Width, ClientSize.Height);

                g.Clear(_clearColor);
                g.TranslateTransform(ClientSize.Width/2f, clientHeight/2f + menuStrip.ClientSize.Height);
                g.ScaleTransform(s, s);

                _drawingStrategy.Draw(g, _simulation);
            }

            e.Graphics.DrawImageUnscaled(_backBuffer, Point.Empty);
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aboutBox = new AboutBox();
            aboutBox.Show();
            aboutBox.Closed += (s, a) => aboutBox.Dispose();
        }
    }
}