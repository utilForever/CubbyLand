using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubbyLand
{
    public class Site
    {
        private static Stack<Site> pool = new Stack<Site>();

        public Point coord;
        public double weight;
        private int index;
        public List<Edge> edges;
        private List<Point> reg;
        private List<Orientation> edgeOrientations;

        private static double EPSILON = 0.005;

        public void AddEdge(Edge e)
        {
            edges.Add(e);
        }
    }
}
