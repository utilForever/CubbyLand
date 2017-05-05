using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubbyLand
{
    public class SimplexNoise
    {
        SimplexNoiseOctave[] octaves;
        double[] frequencies;
        double[] amplitudes;

        int largestFeature;
        double persistence;
        int seed;

        public SimplexNoise(int largest, double persistence, int seed)
        {
            this.largestFeature = largest;
            this.persistence = persistence;
            this.seed = seed;

            int numOctaves = (int) Math.Ceiling(Math.Log10(largest) / Math.Log10(2));

            octaves = new SimplexNoiseOctave[numOctaves];
            frequencies = new double[numOctaves];
            amplitudes = new double[numOctaves];

            Random r = new Random(seed);

            for (int i = 0; i < numOctaves; ++i)
            {
                octaves[i] = new SimplexNoiseOctave(r.Next());
                frequencies[i] = Math.Pow(2, i);
                amplitudes[i] = Math.Pow(persistence, numOctaves - i);
            }
        }

        public double[,] GenerateMap(int x, int y)
        {
            double[,] map = new double[x, y];

            for (int i = 0; i < y; ++i)
            {
                for (int j = 0; j < x; ++j)
                {
                    map[j, i] = GenerateNoise(j, i) / 2 + 0.5;
                }
            }

            return map;
        }

        public System.Drawing.Bitmap GenerateGreyScale(int x, int y)
        {
            System.Drawing.Bitmap g = new Bitmap(x, y);

            for (int i = 0; i < y; ++i)
            {
                for (int j = 0; j < x; ++j)
                {
                    double noise = GenerateNoise(j, i) / 2 + 0.5;

                    if (noise < 0.0)
                    {
                        noise = 0.0;
                    }
                    else if (noise > 1.0)
                    {
                        noise = 255.0;
                    }
                    else
                    {
                        noise *= 255.0;
                    }

                    g.SetPixel(j, i, System.Drawing.Color.FromArgb(30, (int) noise, (int) noise, (int) noise));
                }
            }

            return g;
        }

        public double GenerateNoise(double x, double y)
        {
            double result = 0;

            for (int i = 0; i < octaves.Length; i++)
            {
                result += octaves[i].GenerateNoise2D(x / frequencies[i], y / frequencies[i]) * amplitudes[i];
            }

            return result;
        }

        class SimplexNoiseOctave
        {
            public static int RANDOM = 0;
            private static int NUMSWAPS = 400;

            Gradient[] g3 =
            {
                new Gradient(1, 1, 0), new Gradient(-1, 1, 0), new Gradient(1, -1, 0), new Gradient(-1, -1, 0),
                new Gradient(1, 0, 1), new Gradient(-1, 0, 1), new Gradient(1, 0, -1), new Gradient(-1, 0, -1),
                new Gradient(0, 1, 1), new Gradient(0, -1, 1), new Gradient(0, 1, -1), new Gradient(0, -1, -1)
            };

            private static short[] p_supply =
            {
                151, 160, 137, 91, 90, 15,
                131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23,
                190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33,
                88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166,
                77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244,
                102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196,
                135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123,
                5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
                223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
                129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228,
                251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107,
                49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
                138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
            };

            private static short[] p = new short[256];

            private static short[] permutation = new short[512];
            private static short[] permutationM12 = new short[512];

            private static double F2 = (0.5) * (Math.Sqrt(3.0) - 1.0);
            private static double G2 = (3.0 - Math.Sqrt(3.0)) / 6.0;

            public SimplexNoiseOctave(int seed)
            {
                Random r;
                if (seed == RANDOM)
                {
                    r = new Random();
                }
                else
                {
                    r = new Random(seed);
                }

                for (int i = 0; i < 256; ++i)
                {
                    p[i] = p_supply[i];
                }

                for (int i = 0; i < NUMSWAPS; ++i)
                {
                    int sF = r.Next(p.Length);
                    int sT = r.Next(p.Length);

                    short temp = p[sF];
                    p[sF] = p[sT];
                    p[sT] = temp;
                }

                for (int i = 0; i < 512; ++i)
                {
                    permutation[i] = p[i & 255];
                    permutationM12[i] = (short) (permutation[i] % 12);
                }
            }

            public double GenerateNoise2D(double xIn, double yIn)
            {
                double n0;
                double n1;
                double n2;

                double hF = (xIn + yIn) * F2;

                int i = FastFloor(xIn + hF);
                int j = FastFloor(yIn + hF);

                double t = (i + j) * G2;

                double X0 = i - t;
                double Y0 = j - t;
                double x0 = xIn - X0;
                double y0 = yIn - Y0;

                int i1;
                int j1;

                if (x0 > y0)
                {
                    i1 = 1;
                    j1 = 0;
                }
                else
                {
                    i1 = 0;
                    j1 = 1;
                }

                double x1 = x0 - i1 + G2;
                double y1 = y0 - j1 + G2;
                double x2 = x0 - 1.0 + 2.0 * G2;
                double y2 = y0 - 1.0 + 2.0 * G2;

                int ii = i & 255;
                int jj = j & 255;

                int gi0 = permutationM12[ii + permutation[jj]];
                int gi1 = permutationM12[ii + i1 + permutation[jj + j1]];
                int gi2 = permutationM12[ii + 1 + permutation[jj + 1]];

                double t0 = 0.5 - x0 * x0 - y0 * y0;
                if (t0 < 0)
                {
                    n0 = 0.0;
                }
                else
                {
                    t0 *= t0;
                    n0 = t0 * t0 * Dot(g3[gi0], x0, y0);
                }

                double t1 = 0.5 - x1 * x1 - y1 * y1;
                if (t1 < 0)
                {
                    n1 = 0.0;
                }
                else
                {
                    t1 *= t1;
                    n1 = t1 * t1 * Dot(g3[gi1], x1, y1);
                }

                double t2 = 0.5 - x2 * x2 - y2 * y2;
                if (t2 < 0)
                {
                    n2 = 0.0;
                }
                else
                {
                    t2 = t2 * t2;
                    n2 = t2 * t2 * Dot(g3[gi2], x2, y2);
                }

                return 70 * (n0 + n1 + n2);
            }

            private static int FastFloor(double x)
            {
                int xI = (int) x;
                return (x < xI) ? (xI - 1) : xI;
            }

            private static double Dot(Gradient g, double x, double y)
            {
                return g.x * x + g.y * y;
            }

            class Gradient
            {
                public double x, y, z;

                public Gradient(double x, double y, double z)
                {
                    this.x = x;
                    this.y = y;
                    this.z = z;
                }
            }
        }
    }
}
