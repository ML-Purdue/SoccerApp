using System;
using System.Drawing;
using System.Windows.Forms;
using FootballSimulation;

namespace FootballSimulationApp
{
    internal sealed partial class MainForm : Form
    {
        private readonly ISimulationDrawingStrategy _drawingStrategy;
        private readonly ISimulationLoop _gameLoop;

        private bool _paused;
        private bool _resizing;
        private Simulation _simulation;

        public MainForm(ISimulationLoop gameLoop, ISimulationDrawingStrategy drawingStrategy)
        {
            InitializeComponent();

            Resize += (s, a) => Invalidate();
            ResizeBegin += (s, a) => _resizing = true;
            ResizeEnd += (s, a) => _resizing = false;

            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);

            _drawingStrategy = drawingStrategy;
            _gameLoop = gameLoop;
            _gameLoop.SetUpdate(t => { if (!_paused && !_resizing) _simulation.Simulate((float) t.TotalSeconds); });
            _gameLoop.SetDraw(t => Invalidate());

            Application.Idle += Application_Idle;
        }

        private static float ScaleToFit(SizeF size, float width, float height)
            => size.Width/size.Height >= width/height ? width/size.Width : height/size.Height;

        protected override void Dispose(bool disposing)
        {
            Application.Idle -= Application_Idle;
            base.Dispose(disposing);
        }

        private void OnFirstNewGame()
        {
            pauseToolStripMenuItem.Enabled = true;
            restartToolStripMenuItem.Enabled = true;
            Paint += MainForm_Paint;
            _gameLoop.Start();
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            while (!NativeMethods.IsMessageAvailable)
                _gameLoop.Tick();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            var height = (float)(ClientSize.Height - menuStrip.Height);
            var s = 0.9f*ScaleToFit(_simulation.PitchBounds.Size, ClientSize.Width, height);

            e.Graphics.Clear(BackColor);
            e.Graphics.TranslateTransform(ClientSize.Width/2f, height/2 + menuStrip.Height);
            e.Graphics.ScaleTransform(s, s);

            _drawingStrategy.Draw(e.Graphics, _simulation);
        }
        
        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _simulation = SimulationFactory.Create2V2Simulation();
            if (!_gameLoop.IsRunning)
                OnFirstNewGame();
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _paused = !_paused;
            pauseToolStripMenuItem.Text = _paused ? "Continue" : "Pause";
            Text = "Football Simulation"; // _simulation.Name != string.Empty ? _simulation.Name : assemblyTitle;
            if (_paused) Text += " (Paused)";
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e) { }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Close();

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var aboutBox = new AboutBox())
                aboutBox.ShowDialog(this);
        }
    }
}