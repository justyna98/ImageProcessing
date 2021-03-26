using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace task_1
{
    public partial class Form1 : Form
    {
        Image filtered;
        Image original;
        Filters filter = new Filters();
        Dithering dither= new Dithering();
        Quantization kmeans = new Quantization();
        ConvolutioinalFilters blurFilter = new ConvolutioinalFilters(FixedParameters.blur, 0, 9);
        ConvolutioinalFilters gaussianBlurFilter = new ConvolutioinalFilters(FixedParameters.blur, 0, 8);
        ConvolutioinalFilters sharpenFilter = new ConvolutioinalFilters(FixedParameters.gaussianbBlur, 0, 15);
        ConvolutioinalFilters edgeDiagonalFilter = new ConvolutioinalFilters(FixedParameters.edgeDiagonal, 0, 1);
        ConvolutioinalFilters embossFilter = new ConvolutioinalFilters(FixedParameters.emboss, 0, 1);

        public Form1()
        {
            InitializeComponent();
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            groupBox3.Enabled = false;
            groupBox4.Enabled = false;
            groupBox5.Enabled = false;

        }
        public void Unblock()
        {
            groupBox1.Enabled = true;
            groupBox2.Enabled = true;
            groupBox3.Enabled = true;
            groupBox4.Enabled = true;
            groupBox5.Enabled = true;

        }

        //Load image
        private void button10_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Files|*.jpg;*.jpeg;*.png";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                    original= Image.FromFile(filePath);
                    pictureBox1.Image = original;
                    filtered= Image.FromFile(filePath);
                    pictureBox2.Image = filtered;
                    Unblock();

                }
            }
            radioButton1.Checked=true;
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;

        }

        //Save image
        private void button12_Click(object sender, EventArgs e)
        {
            if(pictureBox1.Image !=null)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Files|*.jpg;*.jpeg;*.png";
                saveFileDialog1.Title = "Save the new image";
                saveFileDialog1.FilterIndex = 2;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (saveFileDialog1.FileName != "")
                    {
                        Bitmap filteredImage = (Bitmap)pictureBox2.Image;
                        filteredImage.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    }

                }
            }
            else
            {
                MessageBox.Show("Choose an image");
                return;
            }

        }

        //Reset
        private void button11_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                Bitmap pic = new Bitmap(pictureBox1.Image);
                pictureBox2.Image = pic;
                filtered = pictureBox2.Image;
            }
            else
            {
                MessageBox.Show("Choose an image");
                return;
            }

        }

        //Inverse
        private void button1_Click(object sender, EventArgs e)
        {
            filtered = filter.ApplyFilter(filtered, filter.Inversion);
            pictureBox2.Image = filtered;
        }

        //Brightness
        private void button2_Click(object sender, EventArgs e)
        {
            filtered = filter.ApplyFilter(filtered, filter.Brightness);
            pictureBox2.Image = filtered;

        }

        //Contrast
        private void button3_Click(object sender, EventArgs e)
        {
            filtered = filter.ApplyFilter(filtered, filter.Contrast);
            pictureBox2.Image = filtered;
        }

        //Gamma
        private void button4_Click(object sender, EventArgs e)
        {

            pictureBox2.Image = filter.ApplyFilter(pictureBox2.Image, filter.Gamma);
        }


        //Image Size
        private void radioButton2_Click(object sender, EventArgs e)
        {
            if (radioButton2.Checked && pictureBox1.SizeMode == PictureBoxSizeMode.AutoSize)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;

            }
            else if(radioButton1.Checked && pictureBox1.SizeMode == PictureBoxSizeMode.Zoom)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
            }
        }

        //Box blur
        private void button5_Click(object sender, EventArgs e)
        {
            filtered = blurFilter.ApplyConvolutioinal(filtered);
            pictureBox2.Image = filtered;

        }

        //Gaussian smoothing
        private void button6_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = gaussianBlurFilter.ApplyConvolutioinal(pictureBox2.Image);
        }

        //Sharpen
        private void button7_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = sharpenFilter.ApplyConvolutioinal(pictureBox2.Image);
        }

        //Emboss
        private void button8_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = embossFilter.ApplyConvolutioinal(pictureBox2.Image);
        }

        //Edge detection
        private void button9_Click(object sender, EventArgs e)
        { 
            pictureBox2.Image = edgeDiagonalFilter.ApplyConvolutioinal(pictureBox2.Image);
        }

        //Grayscale
        private void button13_Click(object sender, EventArgs e)
        {
            filtered = filter.ApplyFilter(filtered, filter.Grayscale);
            pictureBox2.Image = filtered;
        }

        // k-means color quantization
        private void button15_Click(object sender, EventArgs e)
        {
            int k = (int)numericUpDown1.Value;
            filtered = kmeans.Apply(filtered, k);
            pictureBox2.Image = filtered;


        }


        //Random dithering - Color 
        private void button14_Click(object sender, EventArgs e)
        {
            int r = (int)numericUpDown4.Value;
            int g = (int)numericUpDown5.Value;
            int b = (int)numericUpDown6.Value;
            int[] colorvals = { r, g, b };
            filtered = dither.Apply(filtered, colorvals);
            pictureBox2.Image = filtered;
        }

        //Random dithering - Grayscale
        private void button17_Click(object sender, EventArgs e)
        {
            int g = (int)numericUpDown3.Value;
            int[] graylevels = { g };
            filtered = dither.Apply(filtered, graylevels);
            pictureBox2.Image = filtered;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "lena")
            {
                original = Properties.Resources.lena;
                pictureBox1.Image = original;
                filtered = Properties.Resources.lena;
                pictureBox2.Image = filtered;
            }
            if (comboBox1.Text == "ara")
            {
                original = Properties.Resources.ara;
                pictureBox1.Image = original;
                filtered = Properties.Resources.ara;
                pictureBox2.Image = filtered;
            }
            if (comboBox1.Text == "puppies")
            {
                original = Properties.Resources.puppies;
                pictureBox1.Image = original;
                filtered = Properties.Resources.puppies;
                pictureBox2.Image = filtered;
            }
            if (comboBox1.Text == "glacier")
            {
                original = Properties.Resources.glacier;
                pictureBox1.Image = original;
                filtered = Properties.Resources.glacier;
                pictureBox2.Image = filtered;
            }
            Unblock();
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
            radioButton1.Checked=true;
        }
    }
}
