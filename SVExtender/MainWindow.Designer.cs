namespace SVExtender
{
    sealed partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.button1 = new System.Windows.Forms.Button();
            this.RoboCheckBox = new System.Windows.Forms.CheckBox();
            this.ExtendCheckBox = new System.Windows.Forms.CheckBox();
            this.MapCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(254, 59);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // RoboCheckBox
            // 
            this.RoboCheckBox.AutoSize = true;
            this.RoboCheckBox.Location = new System.Drawing.Point(12, 42);
            this.RoboCheckBox.Name = "RoboCheckBox";
            this.RoboCheckBox.Size = new System.Drawing.Size(167, 17);
            this.RoboCheckBox.TabIndex = 1;
            this.RoboCheckBox.Text = "Use Robo folder Instead of txt";
            this.RoboCheckBox.UseVisualStyleBackColor = true;
            // 
            // ExtendCheckBox
            // 
            this.ExtendCheckBox.AutoSize = true;
            this.ExtendCheckBox.Location = new System.Drawing.Point(12, 19);
            this.ExtendCheckBox.Name = "ExtendCheckBox";
            this.ExtendCheckBox.Size = new System.Drawing.Size(59, 17);
            this.ExtendCheckBox.TabIndex = 2;
            this.ExtendCheckBox.Text = "Extend";
            this.ExtendCheckBox.UseVisualStyleBackColor = true;
            // 
            // MapCheckBox
            // 
            this.MapCheckBox.AutoSize = true;
            this.MapCheckBox.Location = new System.Drawing.Point(12, 65);
            this.MapCheckBox.Name = "MapCheckBox";
            this.MapCheckBox.Size = new System.Drawing.Size(162, 17);
            this.MapCheckBox.TabIndex = 3;
            this.MapCheckBox.Text = "Use Map folder Instead of txt";
            this.MapCheckBox.UseVisualStyleBackColor = true;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(341, 94);
            this.Controls.Add(this.MapCheckBox);
            this.Controls.Add(this.ExtendCheckBox);
            this.Controls.Add(this.RoboCheckBox);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox RoboCheckBox;
        private System.Windows.Forms.CheckBox ExtendCheckBox;
        private System.Windows.Forms.CheckBox MapCheckBox;
    }
}

