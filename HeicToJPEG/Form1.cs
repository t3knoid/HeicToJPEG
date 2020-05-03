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
using ImageConverters;

namespace HeicToJPEG
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btBrowse_Click(object sender, EventArgs e)
        {
            setImageToConvert();
        }

        private void setImageToConvert()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                this.tbImageToConvert.Text = openFileDialog1.FileName;
            }
        }

        private void btConvert_Click(object sender, EventArgs e)
        {
            try
            {
                JPEGImage image = new JPEGImage(tbImageToConvert.Text);
                image.ToFile();
                MessageBox.Show("Conversion successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed converting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setImageToConvert();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tbImageToConvert_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length != 0)
            {
                tbImageToConvert.Text = files[0];
            }
        }

        private void tbImageToConvert_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void tbImageToConvert_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.btConvert.Enabled = false;
        }

        private void tbImageToConvert_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(tbImageToConvert.Text) && File.Exists(tbImageToConvert.Text))
            {
                this.btConvert.Enabled = true;
            }
            else
            {
                this.btConvert.Enabled = false;
            }

        }
    }
}
