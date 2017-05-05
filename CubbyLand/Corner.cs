using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubbyLand
{
    public class Corner
    {
        public List<Center> touches = new List<Center>();
        public List<Corner> adjacent = new List<Corner>();
        public List<DelaunayEdge> protrudes = new List<DelaunayEdge>();
        public Point loc;
        public int index;
        public bool border;
        public double elevation;
        public bool water;
        public bool ocean;
        public bool coast;
        public Corner downslope;
        public int river;
        public double moisture;
    }
}
