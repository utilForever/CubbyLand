using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubbyLand
{
    public class Winding
    {
        public static Winding CW = new Winding("Clockwise");
        public static Winding CCW = new Winding("Counterclockwise");
        public static Winding NONE = new Winding("None");
        private string name;

        private Winding(string name)
        {
            this.name = name;
        }
    }
}
