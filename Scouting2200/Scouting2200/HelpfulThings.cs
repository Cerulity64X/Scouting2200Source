using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Scouting2200
{
    /// <summary>
    /// An iterator that simply counts by <c>step</c>.
    /// </summary>
    public class Step : IEnumerator<int>
    {
        public int cur;
        public int step;
        public int start;
        public int end;
        bool stepped = false;

        public Step(int start, int end, int step = 1)
        {
            cur = start;
            this.step = step;
            this.start = start;
            this.end = end;
        }
        public Step(int end)
        {
            cur = 0;
            start = 0;
            step = 1;
            this.end = end;
        }
        public Step()
        {
            cur = 0;
            start = 0;
            step = 1;
            end = int.MaxValue;
        }

        public int Current =>  cur;

        object IEnumerator.Current => cur;

        public void Dispose() { }

        public bool MoveNext()
        {
            bool success;
            if (stepped)
            {
                success = !cur.Overflows(step) && cur + step < end;
                cur += step;
            }
            else
            {
                success = true;
                stepped = true;
            }
            return success;
        }

        public void Reset()
        {
            cur = start;
        }
    }
    /// <summary>
    /// A class that converts to a <see cref="Zip{T, U}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    public class Zipper<T, U> : IEnumerable<(T, U)>
    {
        public IEnumerator<T> First;
        public IEnumerator<U> Second;
        public Zipper(IEnumerator<T> first, IEnumerator<U> second)
        {
            First = first;
            Second = second;
        }

        public IEnumerator<(T, U)> GetEnumerator()
        {
            return new Zip<T, U>(First, Second);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Zip<T, U>(First, Second);
        }
    }
    /// <summary>
    /// A class that contains two iterators, stepping until one is finished.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    public class Zip<T, U> : IEnumerator<(T, U)>
    {
        public IEnumerator<T> First;
        public IEnumerator<U> Second;
        public Zip(IEnumerator<T> first, IEnumerator<U> second)
        {
            First = first;
            Second = second;
        }
        public (T, U) Current => (First.Current, Second.Current);

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            First.Dispose();
            Second.Dispose();
        }

        public bool MoveNext()
        {
            bool fmn = First.MoveNext();
            bool smn = Second.MoveNext();
            return fmn && smn;
        }

        public void Reset()
        {
            First.Reset();
            Second.Reset();
        }
    }
    /// <summary>
    /// A class that simply wraps an IEnumerator within an IEnumerable.
    /// </summary>
    /// <typeparam name="T">The type of the iterator</typeparam>
    public class Enumewrapper<T> : IEnumerable<T>
    {
        IEnumerator<T> Iterator;
        public Enumewrapper(IEnumerator<T> iter)
        {
            Iterator = iter;
        }
        public IEnumerator<T> GetEnumerator() => Iterator;
        IEnumerator IEnumerable.GetEnumerator() => Iterator;
    }
    /// <summary>
    /// An iterator that contains a single item.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class One<T> : IEnumerator<T>
    {
        public T Item;
        /// <summary>
        /// Whether or not MoveNext() has been called.
        /// </summary>
        public bool Sent = false;
        public One(T item)
        {
            Item = item;
        }
        public T Current => Item;
        object IEnumerator.Current => Item;
        public void Dispose() { }

        public bool MoveNext()
        {
            bool ended = !Sent;
            Sent = true;
            return ended;
        }

        public void Reset()
        {
            Sent = false;
        }
    }
}
