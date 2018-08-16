using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;

namespace QueueTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void pd1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("123");
        }
       
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //pictureBox1.down += new Action(pictureBox1_down);
            //pictureBox1.up += new Action(pictureBox1_up);
            pictureBox1.down +=new Action<object>(pictureBox1_down);
            pictureBox1.up += new Action<object>(pictureBox1_up);
        }

        void pictureBox1_up(object sender)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.数字按钮;
        }

        void pictureBox1_down(object sender)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.数字按钮2;
 
        }

        private void pd1_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.数字按钮2;
 
        }

        private void pd1_MouseUp(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.数字按钮;
        }
    }
}
