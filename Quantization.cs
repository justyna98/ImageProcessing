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
        public int R { get; set; }
        public int G{ get; set; }
        public int B { get; set; }

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

        public override bool Equals(object obj)
        {
            var color = obj as NewColor;
            return color != null &&
                   R == color.R &&
                   G == color.G &&
                   B == color.B;
        }

        public override int GetHashCode()
        {
            var hashCode = -1520100960;
            hashCode = hashCode * -1521134295 + R.GetHashCode();
            hashCode = hashCode * -1521134295 + G.GetHashCode();
            hashCode = hashCode * -1521134295 + B.GetHashCode();
            return hashCode;
        }
    }
    class Cluster
    {
        public int noOfcolors = 0;
        public int sumR { get; set; }
        public int sumG { get; set; }
        public int sumB { get; set; }
        public List<double> distances = new List<double>();
        public NewColor centroid;
        public int id { get; set; }

        public Cluster(NewColor centroid, int index)
        {
            this.centroid = centroid;
            this.id = index;
        }

    }
    class Quantization
    {
        public unsafe  Image Apply(Image image, int k)
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

            result = KMeans( buffer,result,  k);



            //puts bytes into result Bitmap
            Bitmap resImg = new Bitmap(width, height);
            BitmapData resData = resImg.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(result, 0, resData.Scan0, bytes);
            resImg.UnlockBits(resData);
            return resImg;
        }
        public byte[] KMeans( byte[] buffer, byte[] result,   int k)
        {
            Random rand = new Random();
            var colors = new HashSet<NewColor>();
            //uniqe colors in an image
            for (int i = 0; i < buffer.Length; i = i + 4)
            {
                NewColor color = new NewColor(buffer[i + 2], buffer[i + 1], buffer[i]);
                colors.Add(color);
                
            }
            List<NewColor> allColors = colors.ToList();
            // limit k to existing number of colors
            if (k > colors.Count)
            {
                k = colors.Count;
            }
            NewColor[] centroids = new NewColor[k];
            NewColor[] avgCentroids = new NewColor[k];
            // random initial centroids
            for (int i = 0; i < k; i++)
            {
                centroids[i] = allColors[rand.Next(allColors.Count)];
            }
            
            bool changed =true;
            int iteration = 0;
            //till centroids are changing and maximum no of iteratioins is not achieved
            while (changed && iteration <100)
            {
                iteration++;
                List<Cluster> clusters = new List<Cluster>();
                //assign centroid to a cluster
                for (int i=0; i < k; i++)
                {
                    clusters.Add(new Cluster(centroids[i], i));
                    clusters[i].sumR =  centroids[i].R;
                    clusters[i].sumG =  centroids[i].G;
                    clusters[i].sumB =  centroids[i].B;
                    clusters[i].noOfcolors= 1;
                }
                // for all available colors assign clusters and calculate the resulting sums of RGB values
                for(int col=0; col < allColors.Count; col++)
                {
                    double minDistance = double.MaxValue;
                    int clusterIndex = -1;
                    NewColor color = allColors[col];
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
                        if (clusters[cl].id == clusterIndex)
                        {
                            clusters[clusterIndex].distances.Add(minDistance);
                            clusters[clusterIndex].sumR += color.R;
                            clusters[clusterIndex].sumG += color.G;
                            clusters[clusterIndex].sumB += color.B;
                            clusters[clusterIndex].noOfcolors+=1;
                        }
                    }

                }
                //for all centroids calculate the avg color in their cluster: if it equals centroid exit the loop, 
                //otherwise assign the calculated avg and continue loop
                for (int j = 0; j < centroids.Length; j++)
                {
                    int numOfColors = clusters[j].noOfcolors;
                    avgCentroids[j] = new NewColor((clusters[j].sumR / numOfColors), (clusters[j].sumG / numOfColors), (clusters[j].sumB / numOfColors));
                    if(centroids[j].R== avgCentroids[j].R && centroids[j].G == avgCentroids[j].G && centroids[j].B == avgCentroids[j].B)
                    {
                        changed = false;
                    }
                    else
                    {
                        centroids[j].R = avgCentroids[j].R;
                        centroids[j].G = avgCentroids[j].G;
                        centroids[j].B = avgCentroids[j].B;
                    }
                }
            }
            //for all pixels
            for (int i = 0; i < buffer.Length; i = i + 4)
            {
                NewColor color = new NewColor(buffer[i + 2], buffer[i + 1], buffer[i]);
                double minDistance = double.MaxValue;
                int clusterIndex = -1;
                for (int m = 0; m < k; m++)
                {
                    double distance = NewColor.EucliceanDistance(color, centroids[m]);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        clusterIndex = m;
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
