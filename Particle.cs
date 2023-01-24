using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace Particles
{
    internal class Particle
    {
        public Vec2 Pos { get; protected set; }
        Vec2 vel;
        Vec2 force;

        // currently unused
        const double mass = 1;

#pragma warning disable IDE1006 // Naming Styles
        public double q
#pragma warning restore IDE1006 // Naming Styles
        {
            get;
            protected set;
        }

        public Particle(Vec2 pos)
        {
            this.Pos = pos;
            vel = new Vec2();
            force = new Vec2();
            q = 1;
        }

        public Particle(Vec2 pos, double charge)
        {
            this.Pos = pos;
            vel = new Vec2();
            force = new Vec2();
            q = charge;
        }

        public double GetForceMag(Particle other_particle, ParticleParameters p_params)
        {
            return p_params.K * Math.Abs(q * other_particle.q) / (p_params.NearStrength + other_particle.Pos.Dist(Pos) * other_particle.Pos.Dist(Pos));
        }

        // Updates the force this particle is to feel with another particle.
        // also updates other particle.
        public void UpdateForce(Particle other_particle, ParticleParameters p_params)
        {
            // make sure opposite charges attract, and like charges repel.
            if(Math.Sign(other_particle.q) == Math.Sign(q))
            {
                double force_mag = GetForceMag(other_particle, p_params);
                force += -new Vec2(
                    force_mag * Math.Cos(Pos.GetTheta(other_particle.Pos)), 
                    force_mag * Math.Sin(Pos.GetTheta(other_particle.Pos))
                );

                other_particle.force += -force;
            } else
            {
                double force_mag = GetForceMag(other_particle, p_params);
                force += new Vec2(
                    force_mag * Math.Cos(Pos.GetTheta(other_particle.Pos)),
                    force_mag * Math.Sin(Pos.GetTheta(other_particle.Pos))
                );

                other_particle.force += -force;
            }
        }

        // Pushes particles towards the center of the board
        public void AddBoundaryForce(ParticleParameters p_params)
        {
            var theta = Pos.GetTheta(Vec2.Zero);

            force += new Vec2(Math.Cos(theta), Math.Sin(theta)) * p_params.ForceFieldStrength * Math.Cbrt(Pos.Dist(Vec2.Zero));
        }

        public void UpdatePosition(ParticleParameters p_params, SimParameters s_params)
        {
            double dt = s_params.dt;
            Vec2 acc = force / mass;
            Pos = Pos + vel * dt + acc * dt * dt * 0.5;
            vel += acc * dt;
            vel *= p_params.VelDamping;
        }

        public static void UpdateParticleListForces(List<Particle> particles, ParticleParameters p_params)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].force = new Vec2();
                particles[i].AddBoundaryForce(p_params);
            }

            for (int i = 0; i < particles.Count; i++)
            {
                for (int j = i + 1; j < particles.Count; j++)
                {
                    particles[i].UpdateForce(particles[j], p_params);
                }
            }
        }

        public static void UpdateParticleListPositions(List<Particle> particles,
                                                       ParticleParameters p_params,
                                                       SimParameters s_params)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].UpdatePosition(p_params, s_params);
            }
        }
    }
}
