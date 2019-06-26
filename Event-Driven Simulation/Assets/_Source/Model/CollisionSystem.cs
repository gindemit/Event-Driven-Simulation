using System;


namespace Assets._Source.Model
{
    public class CollisionSystem
    {
        private MinPQ<Event> _pq; // the priority queue
        private double _t = 0.0; // simulation clock time
        private Particle[] _particles; // the array of particles

        /// <summary>
        /// Initializes a system with the specified collection of particles.
        /// The individual particles will be mutated during the simulation.
        /// </summary>
        /// <param name="particles">the array of particles</param>
        public CollisionSystem(Particle[] particles)
        {
            _particles = (Particle[])particles.Clone(); // defensive copy
        }

        /// <summary>
        /// Simulates the system of particles for the specified amount of time.
        /// </summary>
        /// <param name="limit">the amount of time</param>
        public void Simulate(double limit, double hz)
        {
            if (_pq == null)
            {
                Start(limit);
            }

            if (_pq.IsEmpty())
            {
                return;
            }

            // get impending event, discard if invalidated
            Event e = _pq.DelMin();
            if (!e.IsValid())
                return;
            Particle a = e.A;
            Particle b = e.B;

            // physical collision, so update positions, and then simulation clock
            for (int i = 0; i < _particles.Length; i++)
                _particles[i].Move(e.Time - _t);
            _t = e.Time;

            // process event
            if (a != null && b != null) a.BounceOff(b);              // particle-particle collision
            else if (a != null && b == null) a.BounceOffVerticalWall();   // particle-wall collision
            else if (a == null && b != null) b.BounceOffHorizontalWall(); // particle-wall collision
            else if (a == null && b == null) Redraw(limit, hz);               // redraw event

            // update the priority queue with new collisions involving a or b
            Predict(a, limit);
            Predict(b, limit);
        }
        private void Start(double limit)
        {
            // initialize PQ with collision events and redraw event
            _pq = new MinPQ<Event>();
            for (int i = 0; i < _particles.Length; i++)
            {
                Predict(_particles[i], limit);
            }
            _pq.Insert(new Event(0, null, null)); // redraw event
        }
        // updates priority queue with all new events for particle a
        private void Predict(Particle a, double limit)
        {
            if (a == null) return;

            // particle-particle collisions
            for (int i = 0; i < _particles.Length; i++)
            {
                double dt = a.TimeToHit(_particles[i]);
                if (_t + dt <= limit)
                    _pq.Insert(new Event(_t + dt, a, _particles[i]));
            }

            // particle-wall collisions
            double dtX = a.TimeToHitVerticalWall();
            double dtY = a.TimeToHitHorizontalWall();
            if (_t + dtX <= limit) _pq.Insert(new Event(_t + dtX, a, null));
            if (_t + dtY <= limit) _pq.Insert(new Event(_t + dtY, null, a));
        }

        // redraw all particles
        private void Redraw(double limit, double hz)
        {
            //StdDraw.clear();
            //for (int i = 0; i < particles.length; i++)
            //{
            //    particles[i].draw();
            //}
            //StdDraw.show();
            //StdDraw.pause(20);
            if (_t < limit)
            {
                _pq.Insert(new Event(_t + 1.0 / hz, null, null));
            }
        }


        /***************************************************************************
         *  An event during a particle collision simulation. Each event contains
         *  the time at which it will occur (assuming no supervening actions)
         *  and the particles a and b involved.
         *
         *    -  a and b both null:      redraw event
         *    -  a null, b not null:     collision with vertical wall
         *    -  a not null, b null:     collision with horizontal wall
         *    -  a and b both not null:  binary collision between a and b
         *
         ***************************************************************************/
        private class Event : IComparable<Event>
        {
            internal readonly double Time;         // time that event is scheduled to occur
            internal readonly Particle A, B;       // particles involved in event, possibly null
            internal readonly int CountA, CountB;  // collision counts at event creation


            // create a new event to occur at time t involving a and b
            public Event(double t, Particle a, Particle b)
            {
                Time = t;
                A = a;
                B = b;
                if (a != null) CountA = a.GetCount();
                else CountA = -1;
                if (b != null) CountB = b.GetCount();
                else CountB = -1;
            }

            // compare times when two events will occur
            public int CompareTo(Event that)
            {
                return Time.CompareTo(that.Time);
            }

            // has any collision occurred between when event was created and now?
            public bool IsValid()
            {
                if (A != null && A.GetCount() != CountA) return false;
                if (B != null && B.GetCount() != CountB) return false;
                return true;
            }

        }


        /**
         * Unit tests the {@code CollisionSystem} data type.
         * Reads in the particle collision system from a standard input
         * (or generates {@code N} random particles if a command-line integer
         * is specified); simulates the system.
         *
         * @param args the command-line arguments
         */
        public static void Main(String[] args)
        {

            //StdDraw.setCanvasSize(600, 600);

            //// enable double buffering
            //StdDraw.enableDoubleBuffering();

            //// the array of particles
            //Particle[] particles;

            //// create n random particles
            //if (args.length == 1)
            //{
            //    int n = Integer.parseInt(args[0]);
            //    particles = new Particle[n];
            //    for (int i = 0; i < n; i++)
            //        particles[i] = new Particle();
            //}

            //// or read from standard input
            //else
            //{
            //    int n = StdIn.readInt();
            //    particles = new Particle[n];
            //    for (int i = 0; i < n; i++)
            //    {
            //        double rx = StdIn.readDouble();
            //        double ry = StdIn.readDouble();
            //        double vx = StdIn.readDouble();
            //        double vy = StdIn.readDouble();
            //        double radius = StdIn.readDouble();
            //        double mass = StdIn.readDouble();
            //        int r = StdIn.readInt();
            //        int g = StdIn.readInt();
            //        int b = StdIn.readInt();
            //        Color color = new Color(r, g, b);
            //        particles[i] = new Particle(rx, ry, vx, vy, radius, mass, color);
            //    }
            //}

            //// create collision system and simulate
            //CollisionSystem system = new CollisionSystem(particles);
            //system.simulate(10000);
        }

    }
}