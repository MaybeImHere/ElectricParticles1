using System.IO;
using Util;
using SFML.Window;
using SFML.Graphics;
using System.Threading;
using System.Numerics;
using SFML.System;
using Newtonsoft.Json;

namespace Particles
{
    internal struct Parameters
    {
        [JsonProperty(Required = Required.Always)]
        public SimParameters sim_parameters;
        [JsonProperty(Required = Required.Always)]
        public ParticleParameters particle_parameters;
        [JsonProperty(Required = Required.Always)]
        public SimRenderParameters sim_render_parameters;
    }

    public struct SimParameters
    {
        [JsonProperty(Required = Required.Always)]
        public double dt;
        [JsonProperty(Required = Required.Always)]
        public int particle_count;
        [JsonProperty(Required = Required.Always)]
        public int computation_multiplier;
    }

    public struct ParticleParameters
    {
        [JsonProperty(Required = Required.Always)]
        public double NearStrength { set; get; }
        [JsonProperty(Required = Required.Always)]
        public double K { set; get; }
        [JsonProperty(Required = Required.Always)]
        public double ForceFieldStrength { set; get; }
        [JsonProperty(Required = Required.Always)]
        public double VelDamping { set; get; }
    }

    public struct SimRenderParameters
    {
        [JsonProperty(Required = Required.Always)]
        public uint WindowWidthPxl { set; get; }
        [JsonProperty(Required = Required.Always)]
        public uint WindowHeightPxl { set; get; }
        [JsonProperty(Required = Required.Always)]
        public double XMin { set; get; }
        [JsonProperty(Required = Required.Always)]
        public double XMax { set; get; }
        [JsonProperty(Required = Required.Always)]
        public double YMin { set; get; }
        [JsonProperty(Required = Required.Always)]
        public int YMax { set; get; }
    }

    internal static class Program
    {

        // captures errors and prints them out
        static void WriteStringToFile(string file, string str)
        {
            try
            {
                File.WriteAllText(file, str);
                Console.WriteLine("File written.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to write to {1}: {0}", e, file);
            }
        }

        static void SerializeAndWrite<T>(string file, T obj)
        {
            string output = JsonConvert.SerializeObject(obj, Formatting.Indented);
            WriteStringToFile(file, output);
        }

        static bool PromptYN()
        {
            string? line = Console.ReadLine();
            if (line != null)
            {
                if (line[0] == 'y' || line[0] == 'Y')
                {
                    return true;
                }
            }

            return false;
        }

        static string? ReadFile(string file)
        {
            try
            {
                string text = File.ReadAllText(file);
                Console.WriteLine("{0} read!", file);
                return text;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Error reading {0}: File not found.", file);
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Error reading {0}: Directory not found.", file);
            }
            catch (IOException)
            {
                Console.WriteLine("Error reading {0}: Unable to read data.", file);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Error reading {0}: Unauthorized.", file);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading {1}: {0}.", e, file);
            }

            return null;
        }

        static void Exit()
        {
            Console.WriteLine("Exiting...");
            Thread.Sleep(3000);
            Environment.Exit(0);
        }

        static void Main()
        {
            Parameters default_params = new()
            {
                sim_parameters = new SimParameters
                {
                    dt = .0002,
                    particle_count = 80,
                    computation_multiplier = 8
                },

                particle_parameters = new ParticleParameters
                {
                    NearStrength = .005,
                    K = 2.0,
                    ForceFieldStrength = 2.0,
                    VelDamping = 0.99991
                },

                sim_render_parameters = new SimRenderParameters
                {
                    WindowWidthPxl = 800,
                    WindowHeightPxl = 800,
                    XMin = -4,
                    XMax = 4,
                    YMin = -4,
                    YMax = 4
                }
            };

            // read options.txt and parse.
            Parameters sim_params = default_params;

            if(!File.Exists("./options.txt"))
            {
                Console.WriteLine("WARNING: options.txt not found! Using default parameters.");
                Console.WriteLine("Write default parameters to file? (y/n)");

                if(PromptYN())
                {
                    SerializeAndWrite("./options.txt", default_params);
                } else
                {
                    Console.WriteLine("File not written.");
                }
            } else
            {
                try
                {
                    string? file_data = ReadFile("./options.txt");
                    if(file_data == null)
                    {
                        return;
                    }

                    Parameters temp = JsonConvert.DeserializeObject<Parameters>(file_data);
                    Console.WriteLine("Parameters successfully read!");
                    sim_params = temp;
                } catch (JsonSerializationException)
                {
                    Console.WriteLine("Error reading options.txt: Malformed JSON. Revert to default parameters? (n = exit) (y/n)");
                    if(PromptYN())
                    {
                        Console.WriteLine("Write default parameters to file? (y/n)");
                        if (PromptYN())
                        {
                            SerializeAndWrite("./options.txt", default_params);
                        } else
                        {
                            Console.WriteLine("File not written.");
                        }
                    } else
                    {
                        Exit();
                    }
                }
            }
            // end reading options.txt

            // set up window and inital variables
            RenderWindow window = new(new VideoMode(
                sim_params.sim_render_parameters.WindowWidthPxl,
                sim_params.sim_render_parameters.WindowHeightPxl
                ), "Particle Sim V1");
            window.SetFramerateLimit(60);

            List<Particle> particles = new();
            
            // add all of the particles
            var rand = new Random();

            for (int i = 0; i < sim_params.sim_parameters.particle_count; i++)
            {
                double x_diff = sim_params.sim_render_parameters.XMax - sim_params.sim_render_parameters.XMin;
                double y_diff = sim_params.sim_render_parameters.YMax - sim_params.sim_render_parameters.YMin;

                particles.Add(
                    new Particle(
                        new Vec2(
                            rand.NextDouble() * x_diff - sim_params.sim_render_parameters.XMax,
                            rand.NextDouble() * y_diff - sim_params.sim_render_parameters.YMax
                        ),
                    rand.Next(2) * 2 - 1
                    )
                );
            }

            List<List<string>> data = new();
            List<CircleShape> circles = new();

            // sets up data variable and circles.
            float circle_size = (float)((sim_params.sim_render_parameters.XMax - sim_params.sim_render_parameters.XMin)) * 2;
            circle_size /= (sim_params.sim_render_parameters.WindowWidthPxl + sim_params.sim_render_parameters.WindowHeightPxl);
            for (int i = 0; i < sim_params.sim_parameters.particle_count; i++)
            {
                data.Add(new List<string>());
                circles.Add(new CircleShape(circle_size));
                if (particles[i].q >= 0)
                {
                    circles[i].FillColor = Color.Red;
                } else
                {
                    circles[i].FillColor = Color.Blue;
                }
            }

            window.Closed += delegate(object? sender, EventArgs e)
            {
                window.Close();
            };

            {
                double x_diff = sim_params.sim_render_parameters.XMax - sim_params.sim_render_parameters.XMin;
                double y_diff = sim_params.sim_render_parameters.YMax - sim_params.sim_render_parameters.YMin;

                double x_avg = (sim_params.sim_render_parameters.XMax + sim_params.sim_render_parameters.XMin) / 2;
                double y_avg = (sim_params.sim_render_parameters.YMax + sim_params.sim_render_parameters.YMin) / 2;

                View v = new(new Vector2f((float)x_avg, (float)y_avg), new Vector2f((float)x_diff, (float)y_diff));
                window.SetView(v);
            }

            while (window.IsOpen)
            {
                window.DispatchEvents();
                window.Clear(new Color(10, 10, 10));

                for (int k = 0; k < sim_params.sim_parameters.computation_multiplier; k++)
                {
                    Particle.UpdateParticleListForces(particles, sim_params.particle_parameters);
                    Particle.UpdateParticleListPositions(particles, sim_params.particle_parameters, sim_params.sim_parameters);
                }

                // update and draw circles
                int i = 0;
                foreach(var circle in circles)
                {
                    circle.Position = particles[i].Pos.Vec2V2f();
                    i++;

                    window.Draw(circle);
                }

                window.Display();
            }
        }
    }
}