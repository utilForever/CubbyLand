using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubbyLand
{
    public class HalfEdgePriorityQueue
    {
        private List<HalfEdge> hash;
        private int count;
        private int minBucket;
        private int hashSize;
        private double yMin;
        private double deltaY;

        public HalfEdgePriorityQueue(double yMin, double deltaY, int sqrtNSites)
        {
            this.yMin = yMin;
            this.deltaY = deltaY;
            this.hashSize = 4 + sqrtNSites;

            Initialize();
        }

        private void Initialize()
        {
            this.count = 0;
            this.minBucket = 0;
            this.hash = new List<HalfEdge>(hashSize);

            for (int i = 0; i < hashSize; ++i)
            {
                hash.Add(HalfEdge.CreateDummy());
                hash[i].nextInPriorityQueue = null;
            }
        }

        public void Insert(HalfEdge hE)
        {
            HalfEdge prev;
            HalfEdge next;
            int insBucket = Bucket(hE);

            if (insBucket < minBucket)
            {
                minBucket = insBucket;
            }

            prev = hash[insBucket];

            while (((next = prev.nextInPriorityQueue) != null) &&
                   ((hE.yStar > next.yStar) || ((hE.yStar == next.yStar) &&
                   (hE.v.GetCoord().x > next.v.GetCoord().x))))
            {
                prev = next;
            }

            hE.nextInPriorityQueue = prev.nextInPriorityQueue;
            prev.nextInPriorityQueue = hE;

            this.count++;
        }

        public void Remove(HalfEdge hE)
        {
            HalfEdge prev;
            int rBucket = Bucket(hE);

            if (hE.v != null)
            {
                prev = hash[rBucket];

                while (prev.nextInPriorityQueue != hE)
                {
                    prev = prev.nextInPriorityQueue;
                }

                prev.nextInPriorityQueue = hE.nextInPriorityQueue;

                this.count--;
                hE.v = null;
                hE.nextInPriorityQueue = null;

                hE.Dispose();
            }
        }

        private int Bucket(HalfEdge hE)
        {
            int b = (int)((hE.yStar - this.yMin) / this.deltaY * this.hashSize);

            if (b < 0)
            {
                b = 0;
            }
            if (b >= this.hashSize)
            {
                b = this.hashSize - 1;
            }

            return b;
        }

        private bool IsEmpty(int b)
        {
            return (hash[b].nextInPriorityQueue == null);
        }

        private void AdjustMinBucket()
        {
            while ((this.minBucket < (this.hashSize - 1)) && IsEmpty(this.minBucket))
            {
                minBucket++;
            }
        }

        public bool Empty()
        {
            return this.count <= 0;
        }

        public Point Min()
        {
            AdjustMinBucket();

            HalfEdge a = this.hash[this.minBucket].nextInPriorityQueue;

            return new Point(a.v.GetCoord().x, a.yStar);
        }

        public HalfEdge ExtractMin()
        {
            HalfEdge hE;

            hE = this.hash[minBucket].nextInPriorityQueue;

            this.hash[minBucket].nextInPriorityQueue = hE.nextInPriorityQueue;
            this.count--;
            hE.nextInPriorityQueue = null;

            return hE;
        }

        public void Dispose()
        {
            for (int i = 0; i < hashSize; i++)
            {
                hash[i].Dispose();
            }

            hash.Clear();
            hash = null;
        }
    }
}
