using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace task_1
{
    class Filters
    {
        public Image ApplyFilter(Image image, Func< byte[], byte[], int, int, int, byte[]> filter )
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
            Marshal.Copy(srcData.Scan0, buffer, 0, bytes);
            bmp.UnlockBits(srcData);
            
            //do sth to color channels
            result = filter( buffer, result, width, height, stride);
            
            //puts bytes into result Bitmap
            Bitmap resImg = new Bitmap(width, height);
            BitmapData resData = resImg.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(result, 0, resData.Scan0, bytes);
            resImg.UnlockBits(resData);
            return resImg;
        }
        public byte[] Inversion( byte[] buffer, byte[] result, int width, int height, int stride)
        {
            int current = 0;
            int colorChannels = 3;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    current = y * stride + x * 4;
                    for (int i = 0; i < colorChannels; i++)
                    {
                        //processing Blue, Green, Red
                        double inv = (double)buffer[current + i];
                        result[current + i] = (byte)((255 - inv));
                    }
                    //processing alpha
                    result[current + 3] = 255;
                }
            }
            return result;
        }
        public byte[] Brightness(byte[] buffer, byte[] result, int width, int height, int stride)
        {
            double brightness = FixedParameters.brightness;
            int current = 0;
            int colorChannels = 3;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    current = y * stride + x * 4;
                    for (int i = 0; i < colorChannels; i++)
                    {
                        double channel = (double)buffer[current + i];
                        if (brightness >= 0)
                            result[current + i] = (byte)((255 - channel) * brightness + channel);
                        else
                        {
                            brightness = 1 + brightness;
                            result[current + i] = (byte)(brightness * channel);
                        }
                    }
                    result[current + 3] = 255;

                }
            }
            return result;

        }

        public byte[] Contrast(byte[] buffer, byte[] result, int width, int height, int stride)
        {
            double contrast = FixedParameters.contrast;
            double cocntrastLevel = Math.Pow((100.0 + contrast) / 100.0, 2);

            int current = 0;
            int colorChannels = 3;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    current = y * stride + x * 4;
                    for (int i = 0; i < colorChannels; i++)
                    {
                        double channel = (double)buffer[current + i];
                        int newValue = (int)(((((channel / 255.0) - 0.5) * cocntrastLevel) + 0.5) * 255.0);
                        //clamp out of range functions
                        newValue = Helper.Clamp(newValue, 0, 255);
                        result[current + i] = (byte)newValue;


                    }
                    result[current + 3] = 255;

                }
            }
            return result;

        }

        public byte[] Gamma(byte[] buffer, byte[] result, int width, int height, int stride)
        {
            double gamma = FixedParameters.gamma;
            int gammaConstant = FixedParameters.gammaConstant;
            int current = 0;
            int colorChannels = 3;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    current = y * stride + x * 4;
                    for (int i = 0; i < colorChannels; i++)
                    {
                        double range = (double)buffer[current + i] / 255;
                        double correction = gammaConstant * Math.Pow(range, gamma);
                        result[current + i] = (byte)(correction * 255);

                    }
                    result[current + 3] = 255;

                }
            }
            return result;

        }
        public byte[] Grayscale(byte[] buffer, byte[] result, int width, int height, int stride)
        {
            for (int i = 0; i < buffer.Length; i = i + 4)
            {
                result[i + 3] = buffer[i + 3];

                double grayscale = (buffer[i] * 0.21) + (buffer[i + 1] * 0.71) + (buffer[i+2] * 0.071);
                result[i] = result[i + 1] = result[i + 2] = (byte)grayscale;
            }

            return result;
        }

    }
}