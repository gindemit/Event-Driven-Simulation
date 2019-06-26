using UnityEngine;

namespace Assets._Source.Model
{
    public class Particle
    {
        private static readonly double Infinity = double.PositiveInfinity;

        internal double _rx { get; private set; } //TODO: to Vector2
        internal double _ry { get; private set; }
        private double _vx, _vy; // velocity
        private int _count; // number of collisions so far
        internal double _radius { get; } // radius
        private readonly double _mass; // mass
        private readonly Color _color; // color


        /// <summary>
        /// Initializes a particle with the specified position, velocity, radius, mass, and color.
        /// </summary>
        /// <param name="rx">x-coordinate of position</param>
        /// <param name="ry">y-coordinate of position</param>
        /// <param name="vx">x-coordinate of velocity</param>
        /// <param name="vy">y-coordinate of velocity</param>
        /// <param name="radius">the radius</param>
        /// <param name="mass">the mass</param>
        /// <param name="color">the color</param>
        public Particle(double rx, double ry, double vx, double vy, double radius, double mass, Color color)
        {
            _vx = vx;
            _vy = vy;
            _rx = rx;
            _ry = ry;
            _radius = radius;
            _mass = mass;
            _color = color;
        }

        /// <summary>
        /// Initializes a particle with a random position and velocity.
        /// The position is uniform in the unit box; the velocity in
        /// either direciton is chosen uniformly at random.
        /// </summary>
        public Particle(System.Random random)
        {
            _rx = random.NextDoubleRange(0.0, 1.0);
            _ry = random.NextDoubleRange(0.0f, 1.0f);
            _vx = random.NextDoubleRange(-0.005f, 0.005f);
            _vy = random.NextDoubleRange(-0.005f, 0.005f);
            _radius = 0.02;
            _mass = 0.5;
            _color = Color.black;
        }

        /// <summary>
        /// Moves this particle in a straight line (based on its velocity)
        /// for the specified amount of time.
        /// </summary>
        /// <param name="dt">dt the amount of time</param>
        public void Move(double dt)
        {
            _rx += _vx * dt;
            _ry += _vy * dt;
        }

        /// <summary>
        /// Returns the number of collisions involving this particle with
        /// vertical walls, horizontal walls, or other particles.
        /// This is equal to the number of calls to <code>BounceOff()</code>
        /// <code>BounceOffVerticalWall()</code> and <code>BounceOffHorizontalWall()</code>
        /// </summary>
        /// <returns>
        /// the number of collisions involving this particle with
        /// vertical walls, horizontal walls, or other particles
        /// </returns>
        public int GetCount()
        {
            return _count;
        }

        /// <summary>
        /// Returns the amount of time for this particle to collide with the specified
        /// particle, assuming no interening collisions.
        /// </summary>
        /// <param name="that">the other particle</param>
        /// <returns>
        /// the amount of time for this particle to collide with the specified
        /// particle, assuming no interening collisions;
        /// <code>Infinity</code> if the particles will not collide
        /// </returns>
        public double TimeToHit(Particle that)
        {
            if (this == that) return Infinity;
            double dx = that._rx - _rx;
            double dy = that._ry - _ry;
            double dvx = that._vx - _vx;
            double dvy = that._vy - _vy;
            double dvdr = dx * dvx + dy * dvy;
            if (dvdr > 0) return Infinity;
            double dvdv = dvx * dvx + dvy * dvy;
            if (dvdv == 0) return Infinity;
            double drdr = dx * dx + dy * dy;
            double sigma = _radius + that._radius;
            double d = (dvdr * dvdr) - dvdv * (drdr - sigma * sigma);
            // if (drdr < sigma*sigma) StdOut.println("overlapping particles");
            if (d < 0) return Infinity;
            return -(dvdr + System.Math.Sqrt(d)) / dvdv;
        }

        /// <summary>
        /// Returns the amount of time for this particle to collide with a vertical
        /// wall, assuming no interening collisions.
        /// </summary>
        /// <returns>
        /// the amount of time for this particle to collide with a vertical wall,
        /// assuming no interening collisions;
        /// <code>Infinity</code> if the particle will not collide with a vertical wall
        /// </returns>
        public double TimeToHitVerticalWall()
        {
            if (_vx > 0) return (1.0 - _rx - _radius) / _vx;
            else if (_vx < 0) return (_radius - _rx) / _vx;
            else return Infinity;
        }


        /// <summary>
        /// Returns the amount of time for this particle to collide with a horizontal
        /// wall, assuming no interening collisions.
        /// </summary>
        /// <returns>
        /// the amount of time for this particle to collide with a horizontal wall,
        /// assuming no interening collisions;
        /// <code>Infinity</code> if the particle will not collide with a horizontal wall
        /// </returns>
        public double TimeToHitHorizontalWall()
        {
            if (_vy > 0) return (1.0 - _ry - _radius) / _vy;
            else if (_vy < 0) return (_radius - _ry) / _vy;
            else return Infinity;
        }

        /// <summary>
        /// Updates the velocities of this particle and the specified particle according
        /// to the laws of elastic collision. Assumes that the particles are colliding
        /// at this instant.
        /// </summary>
        /// <param name="that">the other particle</param>
        public void BounceOff(Particle that)
        {
            double dx = that._rx - _rx;
            double dy = that._ry - _ry;
            double dvx = that._vx - _vx;
            double dvy = that._vy - _vy;
            double dvdr = dx * dvx + dy * dvy; // dv dot dr
            double dist = _radius + that._radius; // distance between particle centers at collison

            // magnitude of normal force
            double magnitude = 2 * _mass * that._mass * dvdr / ((_mass + that._mass) * dist);

            // normal force, and in x and y directions
            double fx = magnitude * dx / dist;
            double fy = magnitude * dy / dist;

            // update velocities according to normal force
            _vx += fx / _mass;
            _vy += fy / _mass;
            that._vx -= fx / that._mass;
            that._vy -= fy / that._mass;

            // update collision counts
            _count++;
            that._count++;
        }

        /// <summary>
        /// Updates the velocity of this particle upon collision with a vertical
        /// wall (by reflecting the velocity in the x direction).
        /// Assumes that the particle is colliding with a vertical wall at this instant.
        /// </summary>
        public void BounceOffVerticalWall()
        {
            _vx = -_vx;
            _count++;
        }

        /// <summary>
        /// Updates the velocity of this particle upon collision with a horizontal
        /// wall (by reflecting the velocity in the y direction).
        /// Assumes that the particle is colliding with a horizontal wall at this instant.
        /// </summary>
        public void BounceOffHorizontalWall()
        {
            _vy = -_vy;
            _count++;
        }

        /// <summary>
        /// Returns the kinetic energy of this particle.
        /// The kinetic energy is given by the formula 1/2 m v^2,
        /// here m is the mass of this particle and v is its velocity.
        /// </summary>
        /// <returns>the kinetic energy of this particle</returns>

        public double KineticEnergy()
        {
            return 0.5 * _mass * (_vx * _vx + _vy * _vy);
        }
    }
}