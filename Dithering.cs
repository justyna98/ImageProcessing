using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace task_1
{
    class Dithering
    {
        public Image Apply(Image image, int[] levels)
        {
            Bitmap bmp = new Bitmap(image);
            int width = bmp.Width;
            int height = bmp.Height;

            //makes copy of bitmap to memory for fast processing.
            BitmapData srcData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int stride = srcData.Stride;
            int bytes = stride * srcData.Height;
            byte[] buffer = new byte[bytes];
            byte[] result = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(srcData.Scan0, buffer, 0, bytes);
            bmp.UnlockBits(srcData);
            Filters filter = new Filters();
            //do sth to color channels
            if (levels.Length==1) {
                result= filter.Grayscale(buffer, result, width, height, stride);
                result = RandomDitheringGray(buffer, result, width, height, stride, levels);
            }
            else {
                result = RandomDitheringColor(buffer, result, width, height, stride, levels);
            }


            //puts bytes into result Bitmap
            Bitmap resImg = new Bitmap(width, height);
            BitmapData resData = resImg.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(result, 0, resData.Scan0, bytes);
            resImg.UnlockBits(resData);
            return resImg;
        }
        public byte[] RandomDitheringGray(byte[] buffer, byte[] result, int width, int height, int stride, int[] greyLevels)
        {
            int graylevel = greyLevels[0];
            Random rand = new Random();
            int[] threshold = new int[graylevel-1];

            for (int i = 0; i < buffer.Length; i = i + 4)
            {
                int grayscale = buffer[i];
                //creating treshold for gray levels
                for (int j = 0; j < graylevel - 1; j++)
                {
                    int lowBound = 255 * j / (graylevel - 1);
                    int upperBound=255*(j+1) / (graylevel - 1);
                    threshold[j] = rand.Next(lowBound, upperBound);

                }

                for(int k = 0; k < graylevel-1; k++)
                {
                    if (grayscale >= threshold[graylevel-2])
                    {
                        result[i] = result[i + 1] = result[i + 2] = 255;
                        break;
                    }
                    else if(grayscale<threshold[k])
                    {
                        result[i] = result[i + 1] = result[i + 2] = (byte)(255 * k / (graylevel - 1));
                        break;
                    }
                }

            }

            return result;
        }
        public byte[] RandomDitheringColor(byte[] buffer, byte[] result, int width, int height, int stride, int[] colorVals)
        {
            int r = colorVals[0];
            int g = colorVals[1];
            int b = colorVals[2];
            Random rand = new Random();
            int[] redVal = new int[r - 1];
            int[] greenVal = new int[g - 1];
            int[] blueVal = new int[b - 1];
            for (int i = 0; i < buffer.Length; i = i + 4)
            {
                int blue= buffer[i];
                int green = buffer[i+1];
                int red = buffer[i+2];
                //creating treshold for gray levels
                for (int j = 0; j < r - 1; j++)
                {
                    int lowBound = 255 * j / (r - 1);
                    int upperBound = 255 * (j + 1) / (r - 1);
                    redVal[j] = rand.Next(lowBound, upperBound);

                }
                for (int j = 0; j < g - 1; j++)
                {
                    int lowBound = 255 * j / (g - 1);
                    int upperBound = 255 * (j + 1) / (g - 1);
                    greenVal[j] = rand.Next(lowBound, upperBound);

                }
                for (int j = 0; j < b - 1; j++)
                {
                    int lowBound = 255 * j / (b - 1);
                    int upperBound = 255 * (j + 1) / (b - 1);
                    blueVal[j] = rand.Next(lowBound, upperBound);

                }

                for (int k = 0; k < r - 1; k++)
                {
                    if (red>= redVal[r - 2])
                    {
                        result[i + 2]= 255;
                        break;
                    }
                    else if (red < redVal[k])
                    {
                        result[i + 2] = (byte)(255 * k / (r - 1));
                        break;
                    }
                }
                for (int k = 0; k < g - 1; k++)
                {
                    if (green >= greenVal[g - 2])
                    {
                        result[i + 1] = 255;
                        break;
                    }
                    else if (green< greenVal[k])
                    {
                        result[i + 1] = (byte)(255 * k / (g - 1));
                        break;
                    }
                }
                for (int k = 0; k < b - 1; k++)
                {
                    if (blue >= blueVal[b - 2])
                    {
                        result[i ] = 255;
                        break;
                    }
                    else if (blue < blueVal[k])
                    {
                        result[i] = (byte)(255 * k / (b - 1));
                        break;
                    }
                }
                result[i + 3] = 255;

            }
            return result;
        }
    }
}
