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

        private readonly GameLoop _gameLoop;
        private readonly Simulation _simulation = SimulationFactory.Create2V2Simulation();
        private Bitmap _backBuffer;

        public MainForm()
        {
            InitializeComponent();

            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);

            _gameLoop = new GameLoop(
                TimeSpan.FromTicks(TimeSpan.TicksPerSecond/60),
                TimeSpan.FromTicks(TimeSpan.TicksPerSecond/10),
                t => _simulation.Simulate((float)t.TotalSeconds),
                t => { Invalidate(); });

            Application.Idle += (sender, e) =>
            {
                while (!IsMessageAvailable)
                    _gameLoop.OnTick();
            };
        }

        private static bool IsMessageAvailable
        {
            get
            {
                NativeMethods.Message message;
                return NativeMethods.PeekMessage(out message, IntPtr.Zero, 0, 0, 0);
            }
        }

        protected override void Dispose(bool disposing)
        {
            _backBuffer.Dispose();
            base.Dispose(disposing);
        }
        
        private void TransformGraphics(Graphics g)
        {
            var pitchBounds = _simulation.PitchBounds;
            var innerAspect = pitchBounds.Width / pitchBounds.Height;
            var outerAspect = (float)ClientSize.Width / ClientSize.Height;
            var s = 0.9f * (innerAspect >= outerAspect
                ? ClientSize.Width / pitchBounds.Width
                : ClientSize.Height / pitchBounds.Height);

            g.TranslateTransform(ClientSize.Width/2f, ClientSize.Height/2f);
            g.ScaleTransform(s, s);
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
                g.Clear(_clearColor);
                TransformGraphics(g);
                _drawingStrategy.Draw(g, _simulation);
            }

            e.Graphics.DrawImageUnscaled(_backBuffer, Point.Empty);
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) => new AboutBox().Show();
    }
}