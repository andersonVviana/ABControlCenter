namespace ADForm
{
    partial class DesblockUserForm
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
            iconButton1 = new FontAwesome.Sharp.IconButton();
            iconButton3 = new FontAwesome.Sharp.IconButton();
            label1 = new Label();
            SuspendLayout();
            // 
            // iconButton1
            // 
            iconButton1.BackColor = Color.Red;
            iconButton1.FlatAppearance.BorderSize = 0;
            iconButton1.FlatStyle = FlatStyle.Flat;
            iconButton1.IconChar = FontAwesome.Sharp.IconChar.Cancel;
            iconButton1.IconColor = Color.Black;
            iconButton1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            iconButton1.IconSize = 30;
            iconButton1.ImageAlign = ContentAlignment.MiddleLeft;
            iconButton1.Location = new Point(315, 89);
            iconButton1.Name = "iconButton1";
            iconButton1.Size = new Size(86, 35);
            iconButton1.TabIndex = 24;
            iconButton1.Text = "Não";
            iconButton1.TextAlign = ContentAlignment.MiddleRight;
            iconButton1.UseVisualStyleBackColor = false;
            iconButton1.Click += iconButton1_Click;
            // 
            // iconButton3
            // 
            iconButton3.BackColor = Color.Green;
            iconButton3.FlatStyle = FlatStyle.Flat;
            iconButton3.IconChar = FontAwesome.Sharp.IconChar.Check;
            iconButton3.IconColor = Color.Black;
            iconButton3.IconFont = FontAwesome.Sharp.IconFont.Auto;
            iconButton3.IconSize = 30;
            iconButton3.ImageAlign = ContentAlignment.MiddleLeft;
            iconButton3.Location = new Point(110, 89);
            iconButton3.Name = "iconButton3";
            iconButton3.Size = new Size(86, 35);
            iconButton3.TabIndex = 23;
            iconButton3.Text = "Sim";
            iconButton3.TextAlign = ContentAlignment.MiddleRight;
            iconButton3.UseVisualStyleBackColor = false;
            iconButton3.Click += iconButton3_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 15F);
            label1.ForeColor = Color.White;
            label1.Location = new Point(40, 23);
            label1.Name = "label1";
            label1.Size = new Size(65, 28);
            label1.TabIndex = 25;
            label1.Text = "label1";
            // 
            // DesblockUserForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(82, 37, 131);
            ClientSize = new Size(514, 155);
            Controls.Add(label1);
            Controls.Add(iconButton1);
            Controls.Add(iconButton3);
            FormBorderStyle = FormBorderStyle.None;
            Name = "DesblockUserForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "DesblockUserForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FontAwesome.Sharp.IconButton iconButton1;
        private FontAwesome.Sharp.IconButton iconButton3;
        private Label label1;
    }
}