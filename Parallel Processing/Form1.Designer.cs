namespace MazeSolver
{
    partial class Form1
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
            this.browseFileButton = new System.Windows.Forms.Button();
            this.parallelSolutionButton = new System.Windows.Forms.Button();
            this.sequentialSolutionButton = new System.Windows.Forms.Button();
            this.filePathLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // browseFileButton
            // 
            this.browseFileButton.Location = new System.Drawing.Point(93, 11);
            this.browseFileButton.Margin = new System.Windows.Forms.Padding(2);
            this.browseFileButton.Name = "browseFileButton";
            this.browseFileButton.Size = new System.Drawing.Size(107, 88);
            this.browseFileButton.TabIndex = 0;
            this.browseFileButton.Text = "Browse Maze File";
            this.browseFileButton.UseVisualStyleBackColor = true;
            this.browseFileButton.Click += new System.EventHandler(this.browseFileButton_Click);
            // 
            // parallelSolutionButton
            // 
            this.parallelSolutionButton.Location = new System.Drawing.Point(11, 127);
            this.parallelSolutionButton.Margin = new System.Windows.Forms.Padding(2);
            this.parallelSolutionButton.Name = "parallelSolutionButton";
            this.parallelSolutionButton.Size = new System.Drawing.Size(132, 51);
            this.parallelSolutionButton.TabIndex = 1;
            this.parallelSolutionButton.Text = "Parallel Solution";
            this.parallelSolutionButton.UseVisualStyleBackColor = true;
            this.parallelSolutionButton.Click += new System.EventHandler(this.parallelSolutionButton_Click);
            // 
            // sequentialSolutionButton
            // 
            this.sequentialSolutionButton.Location = new System.Drawing.Point(147, 127);
            this.sequentialSolutionButton.Margin = new System.Windows.Forms.Padding(2);
            this.sequentialSolutionButton.Name = "sequentialSolutionButton";
            this.sequentialSolutionButton.Size = new System.Drawing.Size(135, 51);
            this.sequentialSolutionButton.TabIndex = 2;
            this.sequentialSolutionButton.Text = "Sequential Solution";
            this.sequentialSolutionButton.UseVisualStyleBackColor = true;
            this.sequentialSolutionButton.Click += new System.EventHandler(this.sequentialSolutionButton_Click);
            // 
            // filePathLabel
            // 
            this.filePathLabel.AutoSize = true;
            this.filePathLabel.Location = new System.Drawing.Point(17, 108);
            this.filePathLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.filePathLabel.Name = "filePathLabel";
            this.filePathLabel.Size = new System.Drawing.Size(0, 13);
            this.filePathLabel.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(297, 189);
            this.Controls.Add(this.filePathLabel);
            this.Controls.Add(this.sequentialSolutionButton);
            this.Controls.Add(this.parallelSolutionButton);
            this.Controls.Add(this.browseFileButton);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Maze Solver";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button browseFileButton;
        private System.Windows.Forms.Button parallelSolutionButton;
        private System.Windows.Forms.Button sequentialSolutionButton;
        private System.Windows.Forms.Label filePathLabel;
    }
}

