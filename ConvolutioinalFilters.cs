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
    //Convolutional filters
    class ConvolutioinalFilters
    {
        public double[,] kernel;
        public int offset; //Usually set to 0
        public double divisor;

        public ConvolutioinalFilters(double[,] _kernel, int _offset, double _divisor)
        {
            kernel = _kernel;
            offset = _offset;
            divisor = _divisor;
        }
        public Image ApplyConvolutioinal(Image image)
        {
            Bitmap bitmap = new Bitmap(image);
            int width = bitmap.Width;
            int height = bitmap.Height;
            //makes copy of bitmap to memory for fast processing.
            BitmapData srcData = bitmap.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int bytes = srcData.Stride * srcData.Height;
            byte[] buffer = new byte[bytes];
            byte[] result = new byte[bytes];
            Marshal.Copy(srcData.Scan0, buffer, 0, bytes);
            bitmap.UnlockBits(srcData);
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    int sumR = 0;
                    int sumG = 0;
                    int sumB = 0;

                    int current;
                    for (int matrixY = -1; matrixY < kernel.GetLength(0) - 1; matrixY++)
                        for (int matrixX = -1; matrixX < kernel.GetLength(1) - 1; matrixX++)
                        {
                            // these coordinates will be outside the bitmap near all edges
                            int sourceX = x + matrixX;
                            int sourceY = y + matrixY;

                            if (sourceX < 0)
                                sourceX = 0;

                            if (sourceX >= bitmap.Width)
                                sourceX = bitmap.Width - 1;

                            if (sourceY < 0)
                                sourceY = 0;

                            if (sourceY >= bitmap.Height)
                                sourceY = bitmap.Height - 1;
                            //current pixel for kernel
                            current = sourceY * srcData.Stride + sourceX * 4;
                            sumR += (int)(buffer[current] * kernel[matrixX + 1, matrixY + 1]) + offset;
                            sumG += (int)(buffer[current + 1] * kernel[matrixX + 1, matrixY + 1]) + offset;
                            sumB += (int)(buffer[current + 2] * kernel[matrixX + 1, matrixY + 1]) + offset;
                        
                }
                    // filter bad pixels 
                    sumR = Helper.Clamp((int)(sumR/divisor), 0, 255);
                    sumG = Helper.Clamp((int)(sumG/divisor), 0, 255);
                    sumB = Helper.Clamp((int)(sumB/divisor), 0, 255);
                    // current in resulting bitmap
                    current = y * srcData.Stride + x * 4;
                    result[current] = (byte)sumR;
                    result[current + 1] = (byte)sumG;
                    result[current + 2] = (byte)sumB;
                    result[current + 3] = 255;
                }
            }
            Bitmap resImg = new Bitmap(width, height);
            BitmapData resData = resImg.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(result, 0, resData.Scan0, bytes);
            resImg.UnlockBits(resData);
            return resImg;
        }
        


    }

}

