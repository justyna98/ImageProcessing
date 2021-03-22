using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task_1
{
    class Color
    {
        int R;
        int G;
        int B;
        public Color(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }

      
        public static int EucliceanDistance(Color c1, Color c2)
        {
            int redDifference = c1.R - c2.R;
            int greenDifference = c1.G - c2.G;
            int blueDifference = c1.B - c2.B;

            return redDifference * redDifference + greenDifference * greenDifference + blueDifference * blueDifference;

        }
    }
    class Quantization
    {

    }
}
