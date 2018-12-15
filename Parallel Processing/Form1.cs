using Parallel_Processing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MazeSolver
{
    public partial class Form1 : Form
    {
        Maze mz;

        public Form1()
        {
            InitializeComponent();
            parallelSolutionButton.Enabled = false;
            sequentialSolutionButton.Enabled = false;
        }

        private void browseFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "Text Files (*.txt)|*.txt";
            choofdlog.FilterIndex = 1;
            choofdlog.Multiselect = false;

            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                string sFileName = choofdlog.FileName;
                Console.WriteLine("File Path: " + sFileName);
                filePathLabel.Text = sFileName;
                parallelSolutionButton.Enabled = true;
                sequentialSolutionButton.Enabled = true;
                mz = new Maze(sFileName);
            }
            else
            {
                filePathLabel.Text = "Please, select your maze file!";
                parallelSolutionButton.Enabled = false;
                sequentialSolutionButton.Enabled = false;
            }
            

        }

        private void parallelSolutionButton_Click(object sender, EventArgs e)
        {
            mz.Parallel_Solve();
        }

        private void sequentialSolutionButton_Click(object sender, EventArgs e)
        {
            mz.Sequential_Solve();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
