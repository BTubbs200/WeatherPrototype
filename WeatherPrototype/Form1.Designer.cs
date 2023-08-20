namespace WeatherPrototype
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            saveOutputtedLoopToolStripMenuItem = new ToolStripMenuItem();
            debugToolStripMenuItem = new ToolStripMenuItem();
            showOutputToolStripMenuItem = new ToolStripMenuItem();
            displayPicturebox = new PictureBox();
            statusLabel = new Label();
            demoButton = new Button();
            debugOutputListBox = new ListBox();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)displayPicturebox).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, debugToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(784, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openToolStripMenuItem, saveOutputtedLoopToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(186, 22);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // saveOutputtedLoopToolStripMenuItem
            // 
            saveOutputtedLoopToolStripMenuItem.Enabled = false;
            saveOutputtedLoopToolStripMenuItem.Name = "saveOutputtedLoopToolStripMenuItem";
            saveOutputtedLoopToolStripMenuItem.Size = new Size(186, 22);
            saveOutputtedLoopToolStripMenuItem.Text = "Save Outputted Loop";
            saveOutputtedLoopToolStripMenuItem.Click += saveOutputtedLoopToolStripMenuItem_Click;
            // 
            // debugToolStripMenuItem
            // 
            debugToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { showOutputToolStripMenuItem });
            debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            debugToolStripMenuItem.Size = new Size(54, 20);
            debugToolStripMenuItem.Text = "Debug";
            // 
            // showOutputToolStripMenuItem
            // 
            showOutputToolStripMenuItem.Name = "showOutputToolStripMenuItem";
            showOutputToolStripMenuItem.Size = new Size(175, 22);
            showOutputToolStripMenuItem.Text = "Show Program Log";
            showOutputToolStripMenuItem.Click += showOutputToolStripMenuItem_Click;
            // 
            // displayPicturebox
            // 
            displayPicturebox.BorderStyle = BorderStyle.FixedSingle;
            displayPicturebox.Location = new Point(93, 37);
            displayPicturebox.Name = "displayPicturebox";
            displayPicturebox.Size = new Size(600, 501);
            displayPicturebox.TabIndex = 1;
            displayPicturebox.TabStop = false;
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            statusLabel.Location = new Point(93, 548);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(181, 21);
            statusLabel.TabIndex = 2;
            statusLabel.Text = "Please upload a loop...";
            // 
            // demoButton
            // 
            demoButton.Enabled = false;
            demoButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            demoButton.Location = new Point(560, 544);
            demoButton.Name = "demoButton";
            demoButton.Size = new Size(133, 35);
            demoButton.TabIndex = 3;
            demoButton.Text = "Process Loop";
            demoButton.UseVisualStyleBackColor = true;
            demoButton.Click += demoButton_Click;
            // 
            // debugOutputListBox
            // 
            debugOutputListBox.BackColor = SystemColors.ControlLight;
            debugOutputListBox.FormattingEnabled = true;
            debugOutputListBox.HorizontalScrollbar = true;
            debugOutputListBox.ItemHeight = 15;
            debugOutputListBox.Location = new Point(93, 579);
            debugOutputListBox.Name = "debugOutputListBox";
            debugOutputListBox.Size = new Size(445, 64);
            debugOutputListBox.TabIndex = 5;
            debugOutputListBox.Visible = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 661);
            Controls.Add(debugOutputListBox);
            Controls.Add(demoButton);
            Controls.Add(statusLabel);
            Controls.Add(displayPicturebox);
            Controls.Add(menuStrip1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MainMenuStrip = menuStrip1;
            MaximizeBox = false;
            MaximumSize = new Size(800, 700);
            MinimumSize = new Size(800, 700);
            Name = "Form1";
            Text = "WeatherProtoype v0.1.0";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)displayPicturebox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private PictureBox displayPicturebox;
        private ToolStripMenuItem debugToolStripMenuItem;
        private Label statusLabel;
        private Button demoButton;
        private ListBox debugOutputListBox;
        private ToolStripMenuItem showOutputToolStripMenuItem;
        private ToolStripMenuItem saveOutputtedLoopToolStripMenuItem;
    }
}