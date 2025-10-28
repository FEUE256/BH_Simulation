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

        public MainWindow()
        {
            InitializeComponent();

            simulation = new BlackHoleSimulation(Width: 800, Height: 600, particleCount: 200);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(16); // ~60 FPS
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            simulation.Update();

            simulationCanvas.Children.Clear();

            // Rita partiklar
            foreach (var p in simulation.Particles)
            {
                Ellipse ellipse = new Ellipse
                {
                    Width = 4,
                    Height = 4,
                    Fill = Brushes.White
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
        private double G = 1000; // gravitationskonstant, justera för effekt

        public BlackHoleSimulation(double Width, double Height, int particleCount)
        {
            width = Width;
            height = Height;
            BlackHoleX = Width / 2;
            BlackHoleY = Height / 2;

            Particles = new List<Particle>();
            for (int i = 0; i < particleCount; i++)
            {
                Particles.Add(new Particle
                {
                    X = rnd.NextDouble() * width,
                    Y = rnd.NextDouble() * height,
                    VX = 0,
                    VY = 0
                });
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

                // Undvik division med 0
                if (distance < 1) distance = 1;

                // Gravitationseffekt
                double force = G / distanceSquared;
                double ax = force * dx / distance;
                double ay = force * dy / distance;

                p.VX += ax;
                p.VY += ay;

                p.X += p.VX;
                p.Y += p.VY;
            }
        }
    }
}
