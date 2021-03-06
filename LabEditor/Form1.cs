﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabEditor
{
    public partial class Form1 : Form
    {
        GraphicsPath currentPath;
        Point oldLocation;
        public Pen currentPen;
        bool drawing;
        int historyCount;
        public Color historyColor;
        List<Image> History;
        Timer cyclee;
        public Graphics g;
        int X = 0;
        int Y = 0;
        int X0 = 0;
        int Y0 = 0;
        int figure = 0;

        public Form1()
        {
            InitializeComponent();
            picDrawingSurf.MouseDown += PicDrawingSurf_MouseDown;
            picDrawingSurf.MouseUp += PicDrawingSurf_MouseUp;
            picDrawingSurf.MouseMove += PicDrawingSurf_MouseMove;
            drawing = false;
            currentPen = new Pen(Color.Black);
            historyColor = currentPen.Color;
            currentPen.Width = trackBar1.Value;
            History = new List<Image>(); //initialize history list  
            cyclee = new Timer();
            cyclee.Start();

        }

        private void PicDrawingSurf_MouseMove(object sender, MouseEventArgs e)
        {

            label1.Text = e.X.ToString() + ", " + e.Y.ToString();
            if (drawing)
            {
                if(figure == 0)
                {
                    g = Graphics.FromImage(picDrawingSurf.Image);
                    currentPath.AddLine(oldLocation, e.Location);
                    g.DrawPath(currentPen, currentPath);
                    oldLocation = e.Location;
                    g.Dispose();
                    picDrawingSurf.Invalidate();
                }
                else
                {
                    X = oldLocation.X;
                    Y = oldLocation.Y;
                    X0 = e.Location.X - oldLocation.X;
                    Y0 = e.Location.Y - oldLocation.Y;
                }
                 
            }
        }

        private void PicDrawingSurf_MouseUp(object sender, MouseEventArgs e)
        {
            if(figure == 1) //Drawing square
            {
                Graphics g = Graphics.FromImage(picDrawingSurf.Image);
                Rectangle rect = new Rectangle(X, Y, X0, Y0);
                currentPath.AddRectangle(rect);
                g.DrawPath(currentPen, currentPath);
                oldLocation = e.Location;
                g.Dispose();
                picDrawingSurf.Invalidate();
            }

            

            if(figure == 2) //drawing circle
            {
                Graphics g = Graphics.FromImage(picDrawingSurf.Image);
                Rectangle circ = new Rectangle(X, Y, X0, Y0);
                currentPath.AddEllipse(circ);
                g.DrawPath(currentPen, currentPath);
                g.Dispose();
                picDrawingSurf.Invalidate();
            }
            if(figure == 0)
            {
                currentPath = new GraphicsPath();
                currentPath.Dispose();
            }




            //Removing unnecessary history
            History.RemoveRange(historyCount + 1, History.Count - historyCount - 1);
            History.Add(new Bitmap(picDrawingSurf.Image));
            if (historyCount + 1 < 10) historyCount++;
            if (History.Count - 1 == 10) History.RemoveAt(0);
            drawing = false;

            try
            {
                currentPath = new GraphicsPath();
                currentPath.Dispose();
            }
            catch { };

            currentPen.Color = historyColor;
        }

        private void PicDrawingSurf_MouseDown(object sender, MouseEventArgs e)
        {
            if (picDrawingSurf.Image == null)
            {
                MessageBox.Show("Create new file first!", "Error");
                return;
            }
            if (e.Button == MouseButtons.Left)
            {
                if (randColors.Checked == true)
                {
                    drawing = true;
                    oldLocation = e.Location;
                    currentPath = new GraphicsPath();
                    cyclee.Enabled = true;

                }
                else
                {
                    drawing = true;
                    oldLocation = e.Location;
                    currentPath = new GraphicsPath();
                }


            }
            else if (e.Button == MouseButtons.Right)
            {
                if (cyclee.Enabled == true)
                {
                    cyclee.Enabled = false;
                    historyColor = currentPen.Color;
                    currentPen.Color = Color.White;
                    drawing = true;
                    oldLocation = e.Location;
                    currentPath = new GraphicsPath();
                }
                else
                {
                    historyColor = currentPen.Color;
                    currentPen.Color = Color.White;
                    drawing = true;
                    oldLocation = e.Location;
                    currentPath = new GraphicsPath();
                }


            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            History.Clear();
            historyCount = 0;
            Bitmap pic = new Bitmap(493, 337);

            picDrawingSurf.Image = pic;
            History.Add(new Bitmap(picDrawingSurf.Image));

            if (picDrawingSurf.Image != null) //check before closing or creating new file
            {
                var result = MessageBox.Show("Save image before closing?", "Warning", MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.No: //if no, then nothing happens
                        break;
                    case DialogResult.Yes: //if yes, then this sends us to the saving method
                        saveToolStripMenuItem_Click(sender, e);
                        break;
                    case DialogResult.Cancel: //if cancel, then this just returns user to his masterpiece
                        return;
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileSave = new SaveFileDialog();
            fileSave.Filter = "JPEG Image|*.jpg|Bitmap Image|*.bmp|GIF Image|*.gif|PNG Image|*.png";
            fileSave.Title = "Save as...";
            fileSave.FilterIndex = 4; //default extension
            fileSave.ShowDialog();


            if (fileSave.FileName != "") //Letting user choose image format to save
            {
                System.IO.FileStream fs =
                    (System.IO.FileStream)fileSave.OpenFile();
                switch (fileSave.FilterIndex)
                {
                    case 1:
                        this.picDrawingSurf.Image.Save(fs, ImageFormat.Jpeg);
                        break;
                    case 2:
                        this.picDrawingSurf.Image.Save(fs, ImageFormat.Bmp);
                        break;
                    case 3:
                        this.picDrawingSurf.Image.Save(fs, ImageFormat.Gif);
                        break;
                    case 4:
                        this.picDrawingSurf.Image.Save(fs, ImageFormat.Png);

                        break;
                }
                fs.Close();
            }

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

            OpenFileDialog OP = new OpenFileDialog();
            OP.Filter = "JPEG Image|*.jpg|Bitmap Image|*.bmp|GIF Image|*.gif|PNG Image|*.png";
            OP.Title = "Open an Image File";
            OP.FilterIndex = 1;

            if (OP.ShowDialog() != DialogResult.Cancel)
            {
                picDrawingSurf.Image = new Bitmap(OP.FileName);
                History.Add(new Bitmap(OP.FileName));

            }


        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (picDrawingSurf.Image != null) //check before closing or creating new file
            {
                var result = MessageBox.Show("Save image before closing?", "Warning", MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.No: //if no, then nothing happens, app proceeds to close
                        Application.Exit();
                        break;
                    case DialogResult.Yes: //if yes, then this sends us to the saving method
                        saveToolStripMenuItem_Click(sender, e);
                        Application.Exit();
                        break;
                    case DialogResult.Cancel: //if cancel, then this just returns user to his masterpiece
                        return;
                }
            }
            else
            {
                Application.Exit();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            newToolStripMenuItem_Click(sender, e);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            saveToolStripMenuItem_Click(sender, e);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            openToolStripMenuItem_Click(sender, e);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            colorToolStripMenuItem_Click(sender, e);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentPen.Width = trackBar1.Value;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (History.Count != 0 && historyCount != 0)
            {
                picDrawingSurf.Image = new Bitmap(History[--historyCount]);
            }
            else MessageBox.Show("Nothing to undo", "Error");
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (historyCount < History.Count - 1)
            {
                picDrawingSurf.Image = new Bitmap(History[++historyCount]);
            }
            else MessageBox.Show("Nothing to redo", "Error");
        }

        private void solidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPen.DashStyle = DashStyle.Solid;
            solidToolStripMenuItem.Checked = true;
            dotToolStripMenuItem.Checked = false;
            dashDotDotToolStripMenuItem.Checked = false;
        }

        private void dotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPen.DashStyle = DashStyle.Dot;
            solidToolStripMenuItem.Checked = false;
            dotToolStripMenuItem.Checked = true;
            dashDotDotToolStripMenuItem.Checked = false;
        }

        private void dashDotDotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPen.DashStyle = DashStyle.DashDotDot;
            solidToolStripMenuItem.Checked = false;
            dotToolStripMenuItem.Checked = false;
            dashDotDotToolStripMenuItem.Checked = true;
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorPicking f = new ColorPicking();
            f.Owner = this;
            f.Show();
        }

        private void randColors_CheckedChanged(object sender, EventArgs e)
        {
            if (randColors.Checked == true)
            {
                cyclee.Interval = (int)Convert.ToDecimal(randFreq.Value);
                cyclee.Enabled = true;
                cyclee.Tick += Cyclee_Tick;


            }
            if (randColors.Checked == false)
            {
                cyclee.Enabled = false;

            }
        }

        private void Cyclee_Tick(object sender, EventArgs e)
        {
            Random rainboow = new Random();
            currentPen.Color = Color.FromArgb(rainboow.Next(255), rainboow.Next(255), rainboow.Next(255));
            historyColor = currentPen.Color;
            currentPath = new GraphicsPath();

        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            exitToolStripMenuItem_Click(sender, e);
        }

        private void triangleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            figure = 3;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To use, create new file or open one. LMB to draw, RMB to erase.", "Info");
        }

        private void circleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            figure = 2;
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            figure = 0;
        }

        private void triangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            figure = 1;
        }
    }
}
