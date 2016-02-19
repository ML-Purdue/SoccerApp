using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using FootballSimulation;

namespace FootballSimulationApp
{
    internal partial class MainForm : Form
    {
        private readonly Color _clearColor = Color.Green;
        private readonly ISimulationDrawingStrategy _drawingStrategy =
            new SimulationDrawingStrategy(Color.White, Color.Black, new [] {Color.OrangeRed, Color.Blue});
        private readonly TimeSpan _maxElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond/10);
        private readonly Simulation _simulation;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly TimeSpan _targetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond/60);

        private TimeSpan _accumulatedTime;
        private Bitmap _backBuffer;
        private TimeSpan _lastTime;

        public MainForm()
        {
            InitializeComponent();

            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer, true);

            Application.Idle += OnIdle;

            const float w = 1000;
            const float h = 500;
            const float goalW = w/20;
            const float goalH = h/4;
            const float mass = 1;
            const float radius = 20;
            const float maxForce = 100;
            const float maxSpeed = 100;

            var team1Players = new PointMass[5];
            var team2Players = new PointMass[5];

            for (var j = 0; j < 5; j++)
            {
                team1Players[j] = new PointMass(mass, radius, maxForce, maxSpeed,
                    new Vector2(-w/4 + w/2, -h/4 + j*h/8), Vector2.Zero);
                team2Players[j] = new PointMass(mass, radius, maxForce, maxSpeed,
                    new Vector2(-w/4, -h/4 + j*h/8), Vector2.Zero);
            }

            var pitch = new RectangleF(-w/2, -h/2, w, h);
            var team1Goal = new RectangleF(-w/2 - goalW, -goalH/2, goalW, goalH);
            var team2Goal = new RectangleF(w/2, -goalH/2, goalW, goalH);
            var team1 = new Team(NullTeamStrategy.Instance, new ReadOnlyCollection<PointMass>(team1Players), team1Goal);
            var team2 = new Team(NullTeamStrategy.Instance, new ReadOnlyCollection<PointMass>(team2Players), team2Goal);
            var ball = new PointMass(1, 5, 100, 100, Vector2.Zero, Vector2.Zero);

            _simulation = new Simulation(new ReadOnlyCollection<Team>(new[] {team1, team2}), ball, pitch, 0.05f);
        }

        private static bool IsMessageAvailable
        {
            get
            {
                NativeMethods.Message message;
                return NativeMethods.PeekMessage(out message, IntPtr.Zero, 0, 0, 0);
            }
        }

        private void Update(TimeSpan elapsedTime) => _simulation.Simulate((float) elapsedTime.TotalSeconds);

        private void Draw(Graphics g)
        {
            g.Clear(_clearColor);
            TransformGraphics(g);
            _drawingStrategy.Draw(g, _simulation);
        }

        private void TransformGraphics(Graphics g)
        {
            var s = GetScalingFactor();
            g.TranslateTransform(ClientSize.Width/2f, ClientSize.Height/2f);
            g.ScaleTransform(s, s);
        }

        private float GetScalingFactor()
        {
            var pitchBounds = _simulation.PitchBounds;
            var innerAspect = pitchBounds.Width/pitchBounds.Height;
            var outerAspect = (float) ClientSize.Width/ClientSize.Height;

            return 0.9f*(innerAspect >= outerAspect
                ? ClientSize.Width/pitchBounds.Width
                : ClientSize.Height/pitchBounds.Height);
        }

        private void OnIdle(object sender, EventArgs e)
        {
            while (!IsMessageAvailable)
                OnTick();
        }

        private TimeSpan GetElapsedTime()
        {
            var currentTime = _stopwatch.Elapsed;
            var elapsedTime = currentTime - _lastTime;

            _lastTime = currentTime;
            return elapsedTime > _maxElapsedTime ? _maxElapsedTime : elapsedTime;
        }

        private void OnTick()
        {
            if ((_accumulatedTime += GetElapsedTime()) < _targetElapsedTime)
                return;

            Update(_targetElapsedTime);
            while ((_accumulatedTime -= _targetElapsedTime) >= _targetElapsedTime)
                Update(_targetElapsedTime);

            Invalidate();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _backBuffer = new Bitmap(ClientSize.Width, ClientSize.Height);
            _stopwatch.Start();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            _backBuffer?.Dispose();
            _backBuffer = new Bitmap(ClientSize.Width, ClientSize.Height);
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            using (var g = Graphics.FromImage(_backBuffer))
                Draw(g);

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