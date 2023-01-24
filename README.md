# Particles
Simulates particles that attract and repel with an electrostatic force. There is also a very light force that slowly pushes particles back towards the center of the board called the force field.

To change simulation parameters, modify options.txt (Run the program once if options.txt doesn't exist and follow the command line instructions.)

- Simulation parameters:
	- `sim_parameters`:
		- `dt`: Timestep of the simulation. Closer to 0 means more accurate simulation but more computationally expensive.
		- `particle_count`: Number of particles to be generated.
		- `computation_multiplier`: How many times the particles are updated every frame.
	- `particle_parameters`:
		- `NearStrength`: How strong the electrostatic force is at small distances. This is to prevent `1/(R^2)` from blowing up to near infinity at small ranges by making the expression for the electrostatic force porportional to `1/(NearStrength + R^2)`. Closer to zero = stronger.
		- `K`: Coulomb's constant, how strong the electrostatic force is. Higher = stronger.
		- `ForceFieldStrength`: How strong the force field is. The force field is a weak force that gently pushes particles back toward the center of the board. Higher = stronger.
		- `VelDamping`: How strongly velocity is dampened. 1 = No damping, 0 = complete damping. The current velocity is multiplied by this value every physics tick.
	- `sim_render_parameters`:
		- `WindowWidthPxl`: Width of the window in pixels.
		- `WindowHeightPxl`: Height of the window in pixels.
		- `XMin`: Like on a graphing calculator, the minimum x-value to be plotted.
		- `XMax`: The maximum x-value to be plotted.
		- `YMin`: The minimum y-value to be plotted.
		- `YMax`: The maximum y-value to be plotted.