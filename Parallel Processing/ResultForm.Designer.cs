﻿namespace Parallel_Processing
{
    partial class ResultForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.maze_ground = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // maze_ground
            // 
            this.maze_ground.BackgroundImage = global::Parallel_Processing.Properties.Resources.empty;
            this.maze_ground.Location = new System.Drawing.Point(10, 10);
            this.maze_ground.Name = "maze_ground";
            this.maze_ground.Size = new System.Drawing.Size(90, 90);
            this.maze_ground.TabIndex = 1;
            this.maze_ground.Paint += new System.Windows.Forms.PaintEventHandler(this.maze_ground_Paint);
            // 
            // ResultForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(114, 111);
            this.Controls.Add(this.maze_ground);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ResultForm";
            this.Text = "Maze Solver";
            this.Load += new System.EventHandler(this.ResultForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel maze_ground;
    }
}