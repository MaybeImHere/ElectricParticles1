# Particles
Simulates particles that attract and repel with an electrostatic force. There is also a very light force that slowly pushes particles back towards the center of the board.

Note that to change simulation parameters, you have to recompile (currently).

- Simulation parameters:
	- Program.cs
		- `dt`: Timestep of the simulation. Closer to 0 means more accurate simulation but more computationally expensive.
		- `particle_count`: Number of particles to be generated.
		- `computation_multiplier`: How many times the particles are updated every frame.
	- Particle.cs
		- `near_strength`: How strong the electrostatic force is at small distances. This is to prevent `1/(R^2)` from blowing up to near infinity at small ranges by making the expression `1/(near_strength + R^2)`. Closer to zero = stronger.
		- `K`: Coulomb's constant, how strong the electrostatic force is. Higher = stronger.
		- `force_field_strength`: How strong the force field is. The force field is a weak force that gently pushes particles back toward the center of the board. Higher = stronger.
		- `vel_damping`: How strongly velocity is dampened. 1 = No damping, 0 = complete damping. The current velocity is multiplied by this value every physics tick.