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
    class NewColor
    {
        public int R;
        public int G;
        public int B;
        public NewColor(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }


        public static int EucliceanDistance(NewColor c1, NewColor c2)
        {
            int redDifference = c1.R - c2.R;
            int greenDifference = c1.G - c2.G;
            int blueDifference = c1.B - c2.B;

            return redDifference * redDifference + greenDifference * greenDifference + blueDifference * blueDifference;

        }

        internal static System.Drawing.Color FromArgb(int v1, int v2, int v3)
        {
            throw new NotImplementedException();
        }
    }
    class Cluster
    {
        public int noOfcolors { get; set; }
        public int sumR { get; set; }
        public int sumG { get; set; }
        public int sumB { get; set; }

        public NewColor centroid;
        public int index { get; set; }

        public Cluster(NewColor centroid, int index, int count = 0)
        {
            this.centroid = centroid;
            this.index = index;
        }



    }
    class Quantization
    {
        public Image Apply(Image image, int k)
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
            //do sth to color channels
            result = KMeans(buffer, result, width, height, stride, k);



            //puts bytes into result Bitmap
            Bitmap resImg = new Bitmap(width, height);
            BitmapData resData = resImg.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(result, 0, resData.Scan0, bytes);
            resImg.UnlockBits(resData);
            return resImg;
        }
        public byte[] KMeans(byte[] buffer, byte[] result, int width, int height, int stride, int k)
        {
            HashSet<NewColor> colors = new HashSet<NewColor>();
            List<NewColor> pixels = new List<NewColor>();
            Random rand = new Random();
            //Find all unique colors in the image
            for (int i = 0; i < buffer.Length; i = i + 4)
            {
                NewColor color = new NewColor(buffer[i + 2], buffer[i + 1], buffer[i]);
                colors.Add(color);
                pixels.Add(color);
            }
            // Adjust number of centroids if needed
            if (k > colors.Count)
            {
                k = colors.Count;
            }
            //Selecting random centroids
            NewColor[] centroids = new NewColor[k];
            NewColor[] newCentroids = new NewColor[k];
            for (int i = 0; i < k; i++)
            {
                int c = rand.Next(colors.Count);
                centroids[i] = colors.ElementAt(c);
            }
            bool changed =true;

            while (changed == true)
            {
                List<Cluster> clusters = new List<Cluster>();
                for(int i=0; i < k; i++)
                {
                    clusters.Add(new Cluster(centroids[i], i));
                }
                for(int pix=0; pix < pixels.Count; pix++)
                {
                    double minDistance = 100000;
                    int clusterIndex = -1;
                    NewColor color = pixels[pix];
                    for(int c = 0; c < centroids.Length; c++)
                    {
                        double distance = NewColor.EucliceanDistance(color, centroids[c]);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            clusterIndex = c;
                        }
                    }
                    for (int cl=0; cl < clusters.Count; cl++)
                    {
                        if (clusters[cl].index == clusterIndex)
                        {
                            clusters[clusterIndex].sumR = (clusters[clusterIndex].sumR + color.R);
                            clusters[clusterIndex].sumG = (clusters[clusterIndex].sumG + color.G);
                            clusters[clusterIndex].sumB = (clusters[clusterIndex].sumB + color.B);
                            clusters[clusterIndex].noOfcolors+=1;
                        }
                    }

                }

                for(int j = 0; j < centroids.Length; j++)
                {
                    int numOfColors = clusters[j].noOfcolors;
                    if (numOfColors == 0)
                    {
                        numOfColors = 1;
                    }
                    newCentroids[j] = new NewColor((clusters[j].sumR / numOfColors), (clusters[j].sumG / numOfColors), (clusters[j].sumB / numOfColors));
                    if(centroids[j].R== newCentroids[j].R && centroids[j].G == newCentroids[j].G && centroids[j].B == newCentroids[j].B)
                    {
                        changed = false;
                    }
                    else
                    {
                        centroids[j].R = newCentroids[j].R;
                        centroids[j].G = newCentroids[j].G;
                        centroids[j].B = newCentroids[j].B;
                    }
                }
            }
            for (int i = 0; i < buffer.Length; i = i + 4)
            {
                NewColor color = new NewColor(buffer[i + 2], buffer[i + 1], buffer[i]);
                double minDistance = 100000;
                int clusterIndex = -1;
                for (int c = 0; c < centroids.Length; c++)
                {
                    double distance = NewColor.EucliceanDistance(color, centroids[c]);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        clusterIndex = c;
                    }
                }
                result[i + 3] = 255;
                result[i + 2] = (byte)centroids[clusterIndex].R;
                result[i + 1] = (byte)centroids[clusterIndex].G;
                result[i ] = (byte)centroids[clusterIndex].B;
            }



            return result;

        }
    }
}
