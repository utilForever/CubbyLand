using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubbyLand
{
    public class EdgeList
    {
        private double deltaX;
        private double xMin;
        private int hashSize;
        private List<HalfEdge> hash;
        public HalfEdge leftEnd;
        public HalfEdge rightEnd;

        public EdgeList(double xMin, double deltaX, int sqrtNSites)
        {
            this.xMin = xMin;
            this.deltaX = deltaX;
            this.hashSize = (2 * sqrtNSites);
            hash = new List<HalfEdge>(hashSize);

            leftEnd = HalfEdge.CreateDummy();
            rightEnd = HalfEdge.CreateDummy();
            leftEnd.edgeListLeftNeighbor = null;
            leftEnd.edgeListRightNeighbor = rightEnd;
            rightEnd.edgeListLeftNeighbor = leftEnd;
            rightEnd.edgeListRightNeighbor = null;

            hash.Add(leftEnd);

            for (int i = 1; i < hashSize - 1; i++)
            {
                hash.Add(null);
            }

            hash.Add(rightEnd);
        }

        public void Insert(HalfEdge lB, HalfEdge nHalfEdge)
        {
            nHalfEdge.edgeListLeftNeighbor = lB;
            nHalfEdge.edgeListRightNeighbor = lB.edgeListRightNeighbor;

            if (lB.edgeListRightNeighbor != null)
            {
                lB.edgeListRightNeighbor.edgeListLeftNeighbor = nHalfEdge;
            }

            lB.edgeListRightNeighbor = nHalfEdge;
        }

        public void Remove(HalfEdge hE)
        {
            hE.edgeListLeftNeighbor.edgeListRightNeighbor = hE.edgeListRightNeighbor;
            hE.edgeListRightNeighbor.edgeListLeftNeighbor = hE.edgeListLeftNeighbor;
            hE.e = Edge.DELETED;
            hE.edgeListLeftNeighbor = null;
            hE.edgeListRightNeighbor = null;
        }

        public HalfEdge EdgeListLeftNeighbor(Point p)
        {
            int i;
            int bucket;
            HalfEdge hE;

            bucket = (int)((p.x - xMin) / deltaX * hashSize);
            if (bucket < 0)
            {
                bucket = 0;
            }
            if (bucket >= hashSize)
            {
                bucket = hashSize - 1;
            }

            hE = GetHash(bucket);
            if (hE == null)
            {
                for (i = 1; true; ++i)
                {
                    if ((hE = GetHash(bucket - i)) != null)
                    {
                        break;
                    }
                    if ((hE = GetHash(bucket + i)) != null)
                    {
                        break;
                    }
                }
            }

            if ((hE == leftEnd) || ((hE != rightEnd) && hE.IsLeftOf(p)))
            {
                do
                {
                    hE = hE.edgeListRightNeighbor;
                } while (hE != rightEnd && hE.IsLeftOf(p));

                hE = hE.edgeListLeftNeighbor;
            }
            else
            {
                do
                {
                    hE = hE.edgeListLeftNeighbor;
                } while (hE != leftEnd && !hE.IsLeftOf(p));
            }

            if (bucket > 0 && bucket < hashSize - 1)
            {
                hash[bucket] = hE;
            }

            return hE;
        }

        private HalfEdge GetHash(int b)
        {
            HalfEdge hE;

            if ((b < 0) || (b >= hashSize))
            {
                return null;
            }

            hE = hash[b];

            if (hE != null && hE.e == Edge.DELETED)
            {
                hash[b] = null;
                return null;
            }
            else
            {
                return hE;
            }
        }

        public void Dispose()
        {
            HalfEdge hE = leftEnd;
            HalfEdge prevHE;

            while (hE != rightEnd)
            {
                prevHE = hE;
                hE = hE.edgeListRightNeighbor;
                prevHE.Dispose();
            }

            leftEnd = null;
            rightEnd.Dispose();
            rightEnd = null;

            hash.Clear();
            hash = null;
        }
    }
}