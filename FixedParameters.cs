using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task_1
{
    class FixedParameters
    {
        //Functional filters
        //Brightness: (-1,1)
        public static double brightness = 0.1;
        //Contrast: [-100,100] - positive values increase and negative values decreas, 0 means no change
        public static double contrast = 10;
        //Gamma: (0,2] - for values less than 1 will be brighter and for values more than 1 darker, 1 means no change 
        public static double gamma = 0.8;
        public static int gammaConstant = 1; //Constant from the equation, usually set to 1

        //Convolution filters
        //Blur
        public static double[,] blur = new double[3, 3] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
        //Gaussian blur
        public static double[,] gaussianbBlur = new double[3, 3] { { 1, 2, 1 }, { 2, 4, 2 }, { 1, 2, 1 } };
        //Sharpen
        public static double[,] sharpen = new double[3, 3] { { -1,-1,-1 }, { -1,9,-1 }, { -1, -1, -1 } };
        //Edge detection
        public static double[,] edgeDiagonal = new double[3, 3] { { -1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 0 } };
        //Emboss
        public static double[,] emboss = new double[3, 3] { { 1, 1, -1 }, { 1, 1, -1 }, { 1, -1, -1 } };
    }
    class Helper
    {
        public static int Clamp(int value, int min, int max)
        {
            if (min > value) { return min; }
            else if (max < value) { return max; }
            else return value;
        }
    }
}
