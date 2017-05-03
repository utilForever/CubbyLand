using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubbyLand
{
    public class Orientation
    {
        public static Orientation CW = new Orientation("CW");
        public static Orientation CCW = new Orientation("CCW");
        public string name;

        public Orientation(string name)
        {
            this.name = name;
        }

        public static Orientation Other(Orientation orientation)
        {
            return (orientation == CW ? CCW : CW);
        }
    }
}
