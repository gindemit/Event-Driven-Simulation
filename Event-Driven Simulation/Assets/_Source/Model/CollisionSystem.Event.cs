using System;

namespace Assets._Source.Model
{
    public sealed partial class CollisionSystem
    {

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
        private struct Event : IComparable<Event>
        {
            internal readonly double Time;         // time that event is scheduled to occur
            internal readonly Particle A, B;       // particles involved in event, possibly null
            private readonly int _countA;  // collision counts at event creation
            private readonly int _countB;  // collision counts at event creation


            // create a new event to occur at time t involving a and b
            public Event(double t, Particle a, Particle b)
            {
                Time = t;
                A = a;
                B = b;
                if (a != null) _countA = a.GetCount();
                else _countA = -1;
                if (b != null) _countB = b.GetCount();
                else _countB = -1;
            }

            // compare times when two events will occur
            public int CompareTo(Event that)
            {
                return Time.CompareTo(that.Time);
            }

            // has any collision occurred between when event was created and now?
            public bool IsValid()
            {
                if (A != null && A.GetCount() != _countA) return false;
                if (B != null && B.GetCount() != _countB) return false;
                return true;
            }
        }
    }
}