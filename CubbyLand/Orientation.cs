using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubbyLand
{
    public class Orientation
    {
        public static Orientation LEFT = new Orientation("LEFT");
        public static Orientation RIGHT = new Orientation("RIGHT");
        public string name;

        public Orientation(string name)
        {
            this.name = name;
        }

        public static Orientation Other(Orientation orientation)
        {
            return (orientation == LEFT ? RIGHT : LEFT);
        }
    }
}
