using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets._Source.Model
{
    public class MinPQ<TKey> : IEnumerable<TKey>
    {
        private readonly Comparer<TKey> mComparator; // optional comparator
        private TKey[] _keys; // store items at indices 1 to _n
        private int _n; // number of items on priority queue

        /// <summary>
        /// Initializes an empty priority queue with the given initial capacity.
        /// </summary>
        /// <param name="initCapacity">the initial capacity of this priority queue</param>
        public MinPQ(int initCapacity)
        {
            _keys = new TKey[initCapacity + 1];
            _n = 0;
        }

        /// <summary>
        /// Initializes an empty priority queue.
        /// </summary>
        public MinPQ()
            : this(1)
        {
        }

        /// <summary>
        /// Initializes an empty priority queue with the given initial capacity,
        /// using the given comparator.
        /// </summary>
        /// <param name="initCapacity">the initial capacity of this priority queue</param>
        /// <param name="comparator">the order in which to compare the keys</param>
        public MinPQ(int initCapacity, Comparer<TKey> comparator)
        {
            mComparator = comparator;
            _keys = new TKey[initCapacity + 1];
            _n = 0;
        }

        /// <summary>
        /// Initializes an empty priority queue using the given comparator. 
        /// </summary>
        /// <param name="comparator">comparator the order in which to compare the keys</param>
        public MinPQ(Comparer<TKey> comparator)
            : this(1, comparator)
        {
        }

        /// <summary>
        /// Initializes a priority queue from the array of keys.
        /// Takes time proportional to the number of keys, using sink-based heap construction.
        /// </summary>
        /// <param name="keys">the array of keys</param>
        public MinPQ(TKey[] keys)
        {
            _n = keys.Length;
            _keys = new TKey[keys.Length + 1];
            for (int i = 0; i < _n; i++)
                _keys[i + 1] = keys[i];
            for (int k = _n / 2; k >= 1; k--)
                Sink(k);
            System.Diagnostics.Debug.Assert(IsMinHeap());
        }

        /// <summary>
        /// Returns true if this priority queue is empty.
        /// </summary>
        /// <returns>
        /// <code>true</code> if this priority queue is empty;
        /// <code>false</code> otherwise
        /// </returns>
        public bool IsEmpty()
        {
            return _n == 0;
        }

        /// <summary>
        /// Returns the number of keys on this priority queue.
        /// </summary>
        /// <returns>the number of keys on this priority queue</returns>
        public int Size()
        {
            return _n;
        }

        /// <summary>
        /// Returns a smallest key on this priority queue.
        /// </summary>
        /// <returns>a smallest key on this priority queue</returns>
        /// <exception cref="KeyNotFoundException">if this priority queue is empty</exception>
        public TKey Min()
        {
            if (IsEmpty()) throw new KeyNotFoundException("Priority queue underflow");
            return _keys[1];
        }

        // helper function to double the size of the heap array
        private void Resize(int capacity)
        {
            System.Diagnostics.Debug.Assert(capacity > _n);
            TKey[] temp = new TKey[capacity];
            for (int i = 1; i <= _n; i++)
            {
                temp[i] = _keys[i];
            }

            _keys = temp;
        }

        /// <summary>
        /// Adds a new key to this priority queue.
        /// </summary>
        /// <param name="x">the key to add to this priority queue</param>
        public void Insert(TKey x)
        {
            // double size of array if necessary
            if (_n == _keys.Length - 1) Resize(2 * _keys.Length);

            // add x, and percolate it up to maintain heap invariant
            _keys[++_n] = x;
            Swim(_n);
            System.Diagnostics.Debug.Assert(IsMinHeap());
        }

        /// <summary>
        /// Removes and returns a smallest key on this priority queue.
        /// </summary>
        /// <returns>a smallest key on this priority queue</returns>
        /// <exception cref="KeyNotFoundException">if this priority queue is empty</exception>
        public TKey DelMin()
        {
            if (IsEmpty()) throw new KeyNotFoundException("Priority queue underflow");
            TKey min = _keys[1];
            Exch(1, _n--);
            Sink(1);
            _keys[_n + 1] = default(TKey); // to avoid loiterig and help with garbage collection
            if ((_n > 0) && (_n == (_keys.Length - 1) / 4)) Resize(_keys.Length / 2);
            System.Diagnostics.Debug.Assert(IsMinHeap());
            return min;
        }


        /***************************************************************************
         * Helper functions to restore the heap invariant.
         ***************************************************************************/

        private void Swim(int k)
        {
            while (k > 1 && Greater(k / 2, k))
            {
                Exch(k, k / 2);
                k = k / 2;
            }
        }

        private void Sink(int k)
        {
            while (2 * k <= _n)
            {
                int j = 2 * k;
                if (j < _n && Greater(j, j + 1)) j++;
                if (!Greater(k, j)) break;
                Exch(k, j);
                k = j;
            }
        }

        /***************************************************************************
         * Helper functions for compares and swaps.
         ***************************************************************************/
        private bool Greater(int i, int j)
        {
            if (mComparator == null)
            {
                return ((IComparable<TKey>) _keys[i]).CompareTo(_keys[j]) > 0;
            }
            else
            {
                return mComparator.Compare(_keys[i], _keys[j]) > 0;
            }
        }

        private void Exch(int i, int j)
        {
            TKey swap = _keys[i];
            _keys[i] = _keys[j];
            _keys[j] = swap;
        }

        // is _keys[1..N] a min heap?
        private bool IsMinHeap()
        {
            return IsMinHeap(1);
        }

        // is subtree of _keys[1.._n] rooted at k a min heap?
        private bool IsMinHeap(int k)
        {
            if (k > _n) return true;
            int left = 2 * k;
            int right = 2 * k + 1;
            if (left <= _n && Greater(k, left)) return false;
            if (right <= _n && Greater(k, right)) return false;
            return IsMinHeap(left) && IsMinHeap(right);
        }


        /// <summary>
        /// Returns an enumerator that iterates over the keys on this priority queue
        /// in ascending order.
        /// The enumerator doesn't implement <code>Dispose()</code> and <code>Reset()</code> since it's optional.
        /// </summary>
        /// <returns>an enumerator that iterates over the keys in ascending order</returns>
        public IEnumerator<TKey> GetEnumerator()
        {
            return new HeapIterator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class HeapIterator : IEnumerator<TKey>
        {
            // create a new _keys
            private MinPQ<TKey> _copy;

            // add all items to copy of heap
            // takes linear time since already in heap order so no keys move
            public HeapIterator(MinPQ<TKey> minPQ)
            {
                if (minPQ.mComparator == null)
                {
                    _copy = new MinPQ<TKey>(minPQ.Size());
                }
                else
                {
                    _copy = new MinPQ<TKey>(minPQ.Size(), minPQ.mComparator);
                }

                for (int i = 1; i <= minPQ._n; i++)
                {
                    _copy.Insert(minPQ._keys[i]);
                }
            }

            public bool MoveNext()
            {
                if (_copy.IsEmpty())
                {
                    return false;
                }

                Current = _copy.DelMin();
                return Current != null;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            public TKey Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }
    }
}