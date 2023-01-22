using System.IO;
using Util;
using SFML.Window;
using SFML.Graphics;
using System.Threading;
using System.Numerics;
using SFML.System;

namespace Particles
{
    internal static class Program
    {

        const double dt = .001;
        const int particle_count = 14;
        const int computation_multiplier = 5;

        static void Main(string[] args)
        {
            RenderWindow window = new RenderWindow(new VideoMode(800, 800), "test 1");
            window.SetFramerateLimit(60);

            List<Particle> particles = new List<Particle>();
            
            string file = "./particle{0}.txt";

            // add all of the particles
            var rand = new Random();

            for (int i = 0; i < particle_count; i++)
            {
                particles.Add(new Particle(new Vec2(rand.NextDouble() * 4 - 2, rand.NextDouble() * 4 - 2), rand.NextDouble() * 2 - 1));
            }

            List<List<string>> data = new List<List<string>>();
            List<CircleShape> circles = new List<CircleShape>();
            // sets up data variable and circles.
            for(int i = 0; i < particle_count; i++)
            {
                data.Add(new List<string>());
                circles.Add(new CircleShape(1.0f));
                if (particles[i].q >= 0)
                {
                    circles[i].FillColor = Color.Red;
                } else
                {
                    circles[i].FillColor = Color.Blue;
                }
            }

            // updates the particles and writes out some of the data
            /*
            for (int i = 0; i < 100; i++)
            {
                Particle.UpdateParticleListForces(particles);
                Particle.UpdateParticleListPositions(particles, dt);
                
                if(i % 4 == 0)
                {
                    for (int j = 0; j < particle_count; j++)
                    {
                        data[j].Add(particles[j].GetPos().ToDesmosString());
                    }
                }
            }*/

            // write all the data to the files
            for (int j = 0; j < particle_count; j++)
            {
                File.WriteAllLines(String.Format(file, j), data[j].ToArray());
            }

            window.Closed += delegate(object? sender, EventArgs e)
            {
                window.Close();
            };

            while (window.IsOpen)
            {
                window.DispatchEvents();
                window.Clear(new Color(10, 10, 10));

                for (int k = 0; k < computation_multiplier; k++)
                {
                    Particle.UpdateParticleListForces(particles);
                    Particle.UpdateParticleListPositions(particles, dt);
                }

                // update and draw circles
                int i = 0;
                foreach(var circle in circles)
                {
                    circle.Position = ((particles[i].GetPos() * 200) + 400).Vec2V2f();
                    i++;

                    window.Draw(circle);
                }

                window.Display();
            }
        }
    }
}