// MainWindow.xaml.cs
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BlackHoleSimulationApp
{
    public partial class MainWindow : Window
    {
        private BlackHoleSimulation simulation;
        private DispatcherTimer timer;
        private Random rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();

            simulation = new BlackHoleSimulation(Width: 800, Height: 600, particleCount: 1500);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(16); // ~60 FPS
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            simulation.Update();

            simulationCanvas.Children.Clear();

            // Rita galaxbakgrund med gradient
            LinearGradientBrush galaxyBackground = new LinearGradientBrush();
            galaxyBackground.GradientStops.Add(new GradientStop(Colors.Black, 0));
            galaxyBackground.GradientStops.Add(new GradientStop(Colors.DarkBlue, 0.5));
            galaxyBackground.GradientStops.Add(new GradientStop(Colors.MidnightBlue, 1));
            simulationCanvas.Background = galaxyBackground;

            // Rita attraction rings
            for (int i = 1; i <= 4; i++)
            {
                Ellipse ring = new Ellipse
                {
                    Width = i * 60,
                    Height = i * 60,
                    Stroke = Brushes.DarkGray,
                    StrokeThickness = 0.5,
                    Opacity = 0.3
                };
                Canvas.SetLeft(ring, simulation.BlackHoleX - ring.Width / 2);
                Canvas.SetTop(ring, simulation.BlackHoleY - ring.Height / 2);
                simulationCanvas.Children.Add(ring);
            }

            // Rita partiklar
            foreach (var p in simulation.Particles)
            {
                Ellipse ellipse = new Ellipse
                {
                    Width = 2,
                    Height = 2,
                    Fill = Brushes.White,
                    Opacity = 0.8
                };

                Canvas.SetLeft(ellipse, p.X);
                Canvas.SetTop(ellipse, p.Y);
                simulationCanvas.Children.Add(ellipse);
            }

            // Rita svart hål
            Ellipse blackHole = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = Brushes.Black
            };
            Canvas.SetLeft(blackHole, simulation.BlackHoleX - 10);
            Canvas.SetTop(blackHole, simulation.BlackHoleY - 10);
            simulationCanvas.Children.Add(blackHole);
        }
    }

    public class Particle
    {
        public double X;
        public double Y;
        public double VX;
        public double VY;
    }

    public class BlackHoleSimulation
    {
        public double BlackHoleX;
        public double BlackHoleY;
        private double width;
        private double height;
        private Random rnd = new Random();
        public List<Particle> Particles { get; private set; }
        private double G = 800; // gravitationskonstant
        private double damping = 0.995; // lite friktion för stabilitet

        public BlackHoleSimulation(double Width, double Height, int particleCount)
        {
            width = Width;
            height = Height;
            BlackHoleX = Width / 2;
            BlackHoleY = Height / 2;

            Particles = new List<Particle>();
            for (int i = 0; i < particleCount; i++)
            {
                double angle = rnd.NextDouble() * 2 * Math.PI;
                double radius = rnd.NextDouble() * Math.Min(width, height) / 2;
                double x = BlackHoleX + Math.Cos(angle) * radius;
                double y = BlackHoleY + Math.Sin(angle) * radius;

                // initiera med lite orbital hastighet
                double speed = Math.Sqrt(G / radius) * 0.7;
                double vx = -Math.Sin(angle) * speed;
                double vy = Math.Cos(angle) * speed;

                Particles.Add(new Particle { X = x, Y = y, VX = vx, VY = vy });
            }
        }

        public void Update()
        {
            foreach (var p in Particles)
            {
                double dx = BlackHoleX - p.X;
                double dy = BlackHoleY - p.Y;
                double distanceSquared = dx * dx + dy * dy;
                double distance = Math.Sqrt(distanceSquared);

                if (distance < 5) distance = 5;

                double force = G / distanceSquared;
                double ax = force * dx / distance;
                double ay = force * dy / distance;

                p.VX += ax;
                p.VY += ay;

                // Dämpa hastighet lite för stabilitet
                p.VX *= damping;
                p.VY *= damping;

                p.X += p.VX;
                p.Y += p.VY;

                // Wrap-around så att partiklar aldrig "försvinner"
                if (p.X < 0) p.X += width;
                if (p.X > width) p.X -= width;
                if (p.Y < 0) p.Y += height;
                if (p.Y > height) p.Y -= height;
            }
        }
    }
}
