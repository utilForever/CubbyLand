using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubbyLand
{
    public class GraphImplementation : VoronoiGraph
    {
        public enum ColorData
        {
            OCEAN = 0x44447A,
            SHALLOW_OCEAN = 0x4488AA,
            DEEPER_OCEAN = 0x222255,
            LAKE = 0x336699,
            BEACH = 0xA09077,
            SNOW = 0xFFFFFF,
            TUNDRA = 0xBBBBAA,
            BARE = 0x888888,
            SCORCHED = 0x555555,
            TAIGA = 0x99AA77,
            SHRUBLAND = 0x889977,
            TEMPERATE_DESERT = 0xC9D29B,
            TEMPERATE_RAIN_FOREST = 0x448855,
            TEMPERATE_DECIDUOUS_FOREST = 0x679459,
            GRASSLAND = 0x88AA55,
            SUBTROPICAL_DESERT = 0xD2B98B,
            ICE = 0x99FFFF,
            MARSH = 0x2F6666,
            TROPICAL_RAIN_FOREST = 0x337755,
            TROPICAL_SEASONAL_FOREST = 0x559944,
            COAST = 0x33335A,
            LAKESHORE = 0x25588,
            RIVER = 0x225588
        };

        public GraphImplementation(Voronoi v, int numLloyd, Random r, bool more) : base(v, numLloyd, r, more)
        {
            OCEAN = GetColor(ColorData.OCEAN);
            LAKE = GetColor(ColorData.LAKE);
            BEACH = GetColor(ColorData.BEACH);
            RIVER = GetColor(ColorData.RIVER);
        }
        
        protected override Enum GetBiome(Center p)
        {
            if (p.ocean)
            {
                if (p.elevation > 0.9)
                {
                    return ColorData.OCEAN;
                }
                else if (p.elevation > 0.3)
                {
                    return ColorData.OCEAN;
                }
                else
                {
                    return ColorData.OCEAN;
                }
            }
            else if (p.water)
            {
                if (p.elevation < 0.1)
                {
                    return ColorData.MARSH;
                }
                else if (p.elevation > 0.8)
                {
                    return ColorData.ICE;
                }
                else
                {
                    return ColorData.LAKE;
                }
            }
            else if (p.coast)
            {
                return ColorData.BEACH;
            }
            else if (p.elevation > 0.8)
            {
                if (p.moisture > 0.50)
                {
                    return ColorData.SNOW;
                }
                else if (p.moisture > 0.33)
                {
                    return ColorData.TUNDRA;
                }
                else if (p.moisture > 0.16)
                {
                    return ColorData.BARE;
                }
                else
                {
                    return ColorData.SCORCHED;
                }
            }
            else if (p.elevation > 0.6)
            {
                if (p.moisture > 0.66)
                {
                    return ColorData.TAIGA;
                }
                else if (p.moisture > 0.33)
                {
                    return ColorData.SHRUBLAND;
                }
                else
                {
                    return ColorData.TEMPERATE_DESERT;
                }
            }
            else if (p.elevation > 0.3)
            {
                if (p.moisture > 0.83)
                {
                    return ColorData.TEMPERATE_RAIN_FOREST;
                }
                else if (p.moisture > 0.5)
                {
                    return ColorData.TEMPERATE_DECIDUOUS_FOREST;
                }
                else if (p.moisture > 0.16)
                {
                    return ColorData.GRASSLAND;
                }
                else
                {
                    return ColorData.TEMPERATE_DESERT;
                }
            }
            else
            {
                if (p.moisture > 0.66)
                {
                    return ColorData.TROPICAL_RAIN_FOREST;
                }
                else if (p.moisture > 0.33)
                {
                    return ColorData.TROPICAL_SEASONAL_FOREST;
                }
                else if (p.moisture > 0.16)
                {
                    return ColorData.GRASSLAND;
                }
                else
                {
                    return ColorData.SUBTROPICAL_DESERT;
                }
            }
        }

        protected override Color GetColor(Enum biome)
        {
            ColorData sel = (ColorData)biome;
            int colorCode = (int)sel;
            int r = (colorCode & (0xFF0000)) >> 16;
            int g = (colorCode & (0x00FF00)) >> 8;
            int b = colorCode & (0x0000FF);

            return Color.FromArgb(r, g, b);
        }
    }
}
