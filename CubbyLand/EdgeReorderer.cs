using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubbyLand
{
    public class EdgeReorderer
    {
        private List<Edge> edges;
        private List<Orientation> edgeOrientations;

        public List<Edge> GetEdges()
        {
            return edges;
        }

        public List<Orientation> GetEdgeOrientations()
        {
            return edgeOrientations;
        }

        public EdgeReorderer(List<Edge> oEdges, string criterion)
        {
            edges = new List<Edge>();
            edgeOrientations = new List<Orientation>();

            if (oEdges.Count > 0)
            {
                edges = ReorderEdges(oEdges, criterion);
            }
        }

        public void Dispose()
        {
            edges = null;
            edgeOrientations = null;
        }

        private List<Edge> ReorderEdges(List<Edge> oEdges, string criterion)
        {
            int i;
            int n = oEdges.Count;
            Edge e;

            List<bool> done = new List<bool>(n);
            int nDone = 0;

            for (int k = 0; k < n; k++)
            {
                done.Add(false);
            }

            List<Edge> nEdges = new List<Edge>();

            i = 0;
            e = oEdges[i];
            nEdges.Add(e);
            edgeOrientations.Add(Orientation.LEFT);

            ICoord fPoint;
            ICoord lPoint;

            if (criterion == "v")
            {
                fPoint = e.GetLeftVertex();
                lPoint = e.GetRightVertex();
            }
            else
            {
                fPoint = e.GetLeftSite();
                lPoint = e.GetRightSite();
            }

            if (fPoint == Vertex.VERTEX_AT_INFINITY || lPoint == Vertex.VERTEX_AT_INFINITY)
            {
                return new List<Edge>();
            }

            done[i] = true;
            nDone++;

            while (nDone < n)
            {
                for (i = 1; i < n; ++i)
                {
                    if (done[i])
                        continue;

                    e = oEdges[i];

                    ICoord leftPoint;
                    ICoord rightPoint;

                    if (criterion == "v")
                    {
                        leftPoint = e.GetLeftVertex();
                        rightPoint = e.GetRightVertex();
                    }
                    else
                    {
                        leftPoint = e.GetLeftSite();
                        rightPoint = e.GetRightSite();
                    }

                    if (leftPoint == Vertex.VERTEX_AT_INFINITY || rightPoint == Vertex.VERTEX_AT_INFINITY)
                    {
                        return new List<Edge>();
                    }

                    if (leftPoint == lPoint)
                    {
                        lPoint = rightPoint;
                        edgeOrientations.Add(Orientation.LEFT);
                        nEdges.Add(e);
                        done[i] = true;
                    }
                    else if (rightPoint == fPoint)
                    {
                        fPoint = leftPoint;
                        edgeOrientations.Insert(0, Orientation.LEFT);
                        nEdges.Insert(0, e);
                        done[i] = true;
                    }
                    else if (leftPoint == fPoint)
                    {
                        fPoint = rightPoint;
                        edgeOrientations.Insert(0, Orientation.RIGHT);
                        nEdges.Insert(0, e);
                        done[i] = true;
                    }
                    else if (rightPoint == lPoint)
                    {
                        lPoint = leftPoint;
                        edgeOrientations.Add(Orientation.RIGHT);
                        nEdges.Add(e);
                        done[i] = true;
                    }

                    if (done[i])
                    {
                        ++nDone;
                    }
                }
            }

            return nEdges;
        }
    }
}