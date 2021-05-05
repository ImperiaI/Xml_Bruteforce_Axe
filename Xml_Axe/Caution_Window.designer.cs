namespace Xml_Axe
{
    public partial class Caution_Window
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Caution_Window));
            this.Text_Box_Caution_Window = new System.Windows.Forms.RichTextBox();
            this.Button_Caution_Box_2 = new System.Windows.Forms.Button();
            this.Button_Caution_Box_1 = new System.Windows.Forms.Button();
            this.Button_Caution_Box_3 = new System.Windows.Forms.Button();
            this.List_View_Info = new System.Windows.Forms.ListView();
            this.Columns = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Button_Caution_Box_4 = new System.Windows.Forms.Button();
            this.Button_Invert_Selection = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Invert_Selection)).BeginInit();
            this.SuspendLayout();
            // 
            // Text_Box_Caution_Window
            // 
            this.Text_Box_Caution_Window.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Text_Box_Caution_Window.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Text_Box_Caution_Window.Font = new System.Drawing.Font("Georgia", 15F);
            this.Text_Box_Caution_Window.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.Text_Box_Caution_Window.Location = new System.Drawing.Point(-5, -5);
            this.Text_Box_Caution_Window.Margin = new System.Windows.Forms.Padding(4);
            this.Text_Box_Caution_Window.Name = "Text_Box_Caution_Window";
            this.Text_Box_Caution_Window.ReadOnly = true;
            this.Text_Box_Caution_Window.ShowSelectionMargin = true;
            this.Text_Box_Caution_Window.Size = new System.Drawing.Size(758, 320);
            this.Text_Box_Caution_Window.TabIndex = 0;
            this.Text_Box_Caution_Window.Text = "";
            this.Text_Box_Caution_Window.WordWrap = false;
            // 
            // Button_Caution_Box_2
            // 
            this.Button_Caution_Box_2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.Button_Caution_Box_2.Font = new System.Drawing.Font("Georgia", 14F);
            this.Button_Caution_Box_2.Location = new System.Drawing.Point(397, 260);
            this.Button_Caution_Box_2.Name = "Button_Caution_Box_2";
            this.Button_Caution_Box_2.Size = new System.Drawing.Size(126, 40);
            this.Button_Caution_Box_2.TabIndex = 96;
            this.Button_Caution_Box_2.Text = "No";
            this.Button_Caution_Box_2.UseVisualStyleBackColor = true;
            this.Button_Caution_Box_2.Visible = false;
            this.Button_Caution_Box_2.Click += new System.EventHandler(this.Button_Caution_Box_2_Click);
            // 
            // Button_Caution_Box_1
            // 
            this.Button_Caution_Box_1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.Button_Caution_Box_1.Font = new System.Drawing.Font("Georgia", 14F);
            this.Button_Caution_Box_1.Location = new System.Drawing.Point(109, 260);
            this.Button_Caution_Box_1.Name = "Button_Caution_Box_1";
            this.Button_Caution_Box_1.Size = new System.Drawing.Size(126, 40);
            this.Button_Caution_Box_1.TabIndex = 97;
            this.Button_Caution_Box_1.Text = "Yes";
            this.Button_Caution_Box_1.UseVisualStyleBackColor = true;
            this.Button_Caution_Box_1.Visible = false;
            this.Button_Caution_Box_1.Click += new System.EventHandler(this.Button_Caution_Box_1_Click);
            // 
            // Button_Caution_Box_3
            // 
            this.Button_Caution_Box_3.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.Button_Caution_Box_3.Font = new System.Drawing.Font("Georgia", 14F);
            this.Button_Caution_Box_3.Location = new System.Drawing.Point(253, 260);
            this.Button_Caution_Box_3.Name = "Button_Caution_Box_3";
            this.Button_Caution_Box_3.Size = new System.Drawing.Size(126, 40);
            this.Button_Caution_Box_3.TabIndex = 98;
            this.Button_Caution_Box_3.Text = "Maybe";
            this.Button_Caution_Box_3.UseVisualStyleBackColor = true;
            this.Button_Caution_Box_3.Visible = false;
            this.Button_Caution_Box_3.Click += new System.EventHandler(this.Button_Caution_Box_3_Click);
            // 
            // List_View_Info
            // 
            this.List_View_Info.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.List_View_Info.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Columns});
            this.List_View_Info.Font = new System.Drawing.Font("Georgia", 12F);
            this.List_View_Info.ForeColor = System.Drawing.Color.White;
            this.List_View_Info.FullRowSelect = true;
            this.List_View_Info.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.List_View_Info.Location = new System.Drawing.Point(-5, -5);
            this.List_View_Info.Name = "List_View_Info";
            this.List_View_Info.Size = new System.Drawing.Size(758, 320);
            this.List_View_Info.TabIndex = 99;
            this.List_View_Info.UseCompatibleStateImageBehavior = false;
            this.List_View_Info.View = System.Windows.Forms.View.Details;
            this.List_View_Info.Visible = false;
            // 
            // Columns
            // 
            this.Columns.Width = 502;
            // 
            // Button_Caution_Box_4
            // 
            this.Button_Caution_Box_4.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.Button_Caution_Box_4.Font = new System.Drawing.Font("Georgia", 14F);
            this.Button_Caution_Box_4.Location = new System.Drawing.Point(541, 260);
            this.Button_Caution_Box_4.Name = "Button_Caution_Box_4";
            this.Button_Caution_Box_4.Size = new System.Drawing.Size(126, 40);
            this.Button_Caution_Box_4.TabIndex = 100;
            this.Button_Caution_Box_4.Text = "Other";
            this.Button_Caution_Box_4.UseVisualStyleBackColor = true;
            this.Button_Caution_Box_4.Visible = false;
            this.Button_Caution_Box_4.Click += new System.EventHandler(this.Button_Caution_Box_4_Click);
            // 
            // Button_Invert_Selection
            // 
            this.Button_Invert_Selection.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.Button_Invert_Selection.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Button_Invert_Selection.Location = new System.Drawing.Point(695, 265);
            this.Button_Invert_Selection.Name = "Button_Invert_Selection";
            this.Button_Invert_Selection.Size = new System.Drawing.Size(30, 30);
            this.Button_Invert_Selection.TabIndex = 101;
            this.Button_Invert_Selection.TabStop = false;
            this.Button_Invert_Selection.Visible = false;
            this.Button_Invert_Selection.Click += new System.EventHandler(this.Button_Invert_Selection_Click);
            this.Button_Invert_Selection.MouseLeave += new System.EventHandler(this.Button_Invert_Selection_MouseLeave);
            this.Button_Invert_Selection.MouseHover += new System.EventHandler(this.Button_Invert_Selection_MouseHover);
            // 
            // Caution_Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(748, 312);
            this.Controls.Add(this.Button_Invert_Selection);
            this.Controls.Add(this.Button_Caution_Box_4);
            this.Controls.Add(this.Button_Caution_Box_3);
            this.Controls.Add(this.Button_Caution_Box_1);
            this.Controls.Add(this.Button_Caution_Box_2);
            this.Controls.Add(this.List_View_Info);
            this.Controls.Add(this.Text_Box_Caution_Window);
            this.Font = new System.Drawing.Font("Georgia", 10F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(516, 150);
            this.Name = "Caution_Window";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Caution";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Caution_Window_FormClosed);
            this.Load += new System.EventHandler(this.Caution_Window_Load);
            this.Resize += new System.EventHandler(this.Caution_Window_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.Button_Invert_Selection)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.RichTextBox Text_Box_Caution_Window;
        public System.Windows.Forms.Button Button_Caution_Box_2;
        public System.Windows.Forms.Button Button_Caution_Box_1;
        public System.Windows.Forms.Button Button_Caution_Box_3;
        private System.Windows.Forms.ColumnHeader Columns;
        public System.Windows.Forms.ListView List_View_Info;
        public System.Windows.Forms.Button Button_Caution_Box_4;
        public System.Windows.Forms.PictureBox Button_Invert_Selection;

    }
}