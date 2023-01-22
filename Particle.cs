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
        Vec2 pos;
        Vec2 vel;
        Vec2 force;

        double mass = 1;

        public double q
        {
            get;
            protected set;
        }

        // closer to zero makes force stronger at close ranges. 0 = infinite force at 0 radius.
        const double near_strength = .005;
        const double K = 2.0;
        const double force_field_strength = 2.0;
        const double vel_damping = 0.99991;

        public Particle(Vec2 pos)
        {
            this.pos = pos;
            vel = new Vec2();
            force = new Vec2();
            q = 1;
        }

        public Particle(Vec2 pos, double charge)
        {
            this.pos = pos;
            vel = new Vec2();
            force = new Vec2();
            q = charge;
        }

        public double GetForceMag(Particle other_particle)
        {
            return K * Math.Abs(q * other_particle.q) / (near_strength + other_particle.pos.Dist(pos) * other_particle.pos.Dist(pos));
        }

        // Updates the force this particle is to feel with another particle.
        // also updates other particle.
        public void UpdateForce(Particle other_particle)
        {
            // make sure opposite charges attract, and like charges repel.
            if(Math.Sign(other_particle.q) == Math.Sign(q))
            {
                double force_mag = GetForceMag(other_particle);
                force += -new Vec2(
                    force_mag * Math.Cos(pos.GetTheta(other_particle.pos)), 
                    force_mag * Math.Sin(pos.GetTheta(other_particle.pos))
                );

                other_particle.force += -force;
            } else
            {
                double force_mag = GetForceMag(other_particle);
                force += new Vec2(
                    force_mag * Math.Cos(pos.GetTheta(other_particle.pos)),
                    force_mag * Math.Sin(pos.GetTheta(other_particle.pos))
                );

                other_particle.force += -force;
            }
        }

        // Pushes particles towards the center of the board
        public void AddBoundaryForce()
        {
            var theta = pos.GetTheta(Vec2.Zero);

            force += new Vec2(Math.Cos(theta), Math.Sin(theta)) * force_field_strength * Math.Cbrt(pos.Dist(Vec2.Zero));
        }

        public void UpdatePosition(double dt)
        {
            Vec2 acc = force / mass;
            pos = pos + vel * dt + acc * dt * dt * 0.5;
            vel = vel + acc * dt;
            vel *= vel_damping;
        }

        public static void UpdateParticleListForces(List<Particle> particles)
        {
            for (int i = 0; i < particles.LongCount(); i++)
            {
                particles[i].force = new Vec2();
                particles[i].AddBoundaryForce();
            }

            for (int i = 0; i < particles.LongCount(); i++)
            {
                for (int j = i + 1; j < particles.LongCount(); j++)
                {
                    particles[i].UpdateForce(particles[j]);
                }
            }
        }

        public static void UpdateParticleListPositions(List<Particle> particles, double dt)
        {
            for (int i = 0; i < particles.LongCount(); i++)
            {
                particles[i].UpdatePosition(dt);
            }
        }

        public Vec2 GetPos()
        {
            return pos;
        }
    }
}
