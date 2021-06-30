namespace Xml_Axe
{
    partial class Window
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Window));
            this.Drop_Zone = new System.Windows.Forms.PictureBox();
            this.Text_Box_Original_Path = new System.Windows.Forms.TextBox();
            this.Track_Bar_Tag_Value = new System.Windows.Forms.TrackBar();
            this.Label_Tag_Value = new System.Windows.Forms.Label();
            this.Label_Tag_Name = new System.Windows.Forms.Label();
            this.Button_Start = new System.Windows.Forms.PictureBox();
            this.Open_File_Dialog_1 = new System.Windows.Forms.OpenFileDialog();
            this.Button_Toggle_Settings = new System.Windows.Forms.PictureBox();
            this.Button_Reset_Blacklist = new System.Windows.Forms.PictureBox();
            this.Combo_Box_Tag_Name = new System.Windows.Forms.ComboBox();
            this.Combo_Box_Type_Filter = new System.Windows.Forms.ComboBox();
            this.Label_Type_Filter = new System.Windows.Forms.Label();
            this.Label_Entity_Name = new System.Windows.Forms.Label();
            this.Combo_Box_Tag_Value = new System.Windows.Forms.ComboBox();
            this.Combo_Box_Entity_Name = new System.Windows.Forms.ComboBox();
            this.Text_Box_Tags = new System.Windows.Forms.RichTextBox();
            this.Text_Box_Description = new System.Windows.Forms.RichTextBox();
            this.Check_Box_All_Occurances = new System.Windows.Forms.CheckBox();
            this.List_View_Selection = new System.Windows.Forms.ListView();
            this.Entries = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Button_Run = new System.Windows.Forms.PictureBox();
            this.Button_Percentage = new System.Windows.Forms.PictureBox();
            this.Button_Operator = new System.Windows.Forms.PictureBox();
            this.Button_Search = new System.Windows.Forms.PictureBox();
            this.Button_Scripts = new System.Windows.Forms.PictureBox();
            this.Button_Backup = new System.Windows.Forms.PictureBox();
            this.Button_Browse_Folder = new System.Windows.Forms.PictureBox();
            this.Button_Undo = new System.Windows.Forms.PictureBox();
            this.Button_Attribute = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.Drop_Zone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Track_Bar_Tag_Value)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Start)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Toggle_Settings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Reset_Blacklist)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Run)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Percentage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Operator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Search)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Scripts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Backup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Browse_Folder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Undo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Attribute)).BeginInit();
            this.SuspendLayout();
            // 
            // Drop_Zone
            // 
            this.Drop_Zone.BackColor = System.Drawing.SystemColors.MenuText;
            this.Drop_Zone.Location = new System.Drawing.Point(-1, -1);
            this.Drop_Zone.Name = "Drop_Zone";
            this.Drop_Zone.Size = new System.Drawing.Size(430, 190);
            this.Drop_Zone.TabIndex = 1;
            this.Drop_Zone.TabStop = false;
            this.Drop_Zone.Click += new System.EventHandler(this.Drop_Zone_Click);
            this.Drop_Zone.DragDrop += new System.Windows.Forms.DragEventHandler(this.Drop_Zone_DragDrop);
            this.Drop_Zone.DragEnter += new System.Windows.Forms.DragEventHandler(this.Drop_Zone_DragEnter);
            this.Drop_Zone.DragOver += new System.Windows.Forms.DragEventHandler(this.Drop_Zone_DragOver);
            // 
            // Text_Box_Original_Path
            // 
            this.Text_Box_Original_Path.BackColor = System.Drawing.Color.CadetBlue;
            this.Text_Box_Original_Path.Font = new System.Drawing.Font("Georgia", 12F);
            this.Text_Box_Original_Path.ForeColor = System.Drawing.SystemColors.Window;
            this.Text_Box_Original_Path.Location = new System.Drawing.Point(31, 195);
            this.Text_Box_Original_Path.Name = "Text_Box_Original_Path";
            this.Text_Box_Original_Path.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Text_Box_Original_Path.Size = new System.Drawing.Size(367, 26);
            this.Text_Box_Original_Path.TabIndex = 2;
            // 
            // Track_Bar_Tag_Value
            // 
            this.Track_Bar_Tag_Value.LargeChange = 1;
            this.Track_Bar_Tag_Value.Location = new System.Drawing.Point(25, 560);
            this.Track_Bar_Tag_Value.Name = "Track_Bar_Tag_Value";
            this.Track_Bar_Tag_Value.Size = new System.Drawing.Size(381, 45);
            this.Track_Bar_Tag_Value.TabIndex = 3;
            this.Track_Bar_Tag_Value.Scroll += new System.EventHandler(this.Track_Bar_Tag_Value_Scroll);
            // 
            // Label_Tag_Value
            // 
            this.Label_Tag_Value.AutoSize = true;
            this.Label_Tag_Value.Font = new System.Drawing.Font("Georgia", 20F);
            this.Label_Tag_Value.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.Label_Tag_Value.Location = new System.Drawing.Point(28, 478);
            this.Label_Tag_Value.Name = "Label_Tag_Value";
            this.Label_Tag_Value.Size = new System.Drawing.Size(196, 31);
            this.Label_Tag_Value.TabIndex = 4;
            this.Label_Tag_Value.Text = "New Tag Value";
            // 
            // Label_Tag_Name
            // 
            this.Label_Tag_Name.AutoSize = true;
            this.Label_Tag_Name.Font = new System.Drawing.Font("Georgia", 20F);
            this.Label_Tag_Name.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.Label_Tag_Name.Location = new System.Drawing.Point(31, 398);
            this.Label_Tag_Name.Name = "Label_Tag_Name";
            this.Label_Tag_Name.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Label_Tag_Name.Size = new System.Drawing.Size(138, 31);
            this.Label_Tag_Name.TabIndex = 6;
            this.Label_Tag_Name.Text = "Tag Name";
            // 
            // Button_Start
            // 
            this.Button_Start.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Button_Start.Location = new System.Drawing.Point(399, 193);
            this.Button_Start.Name = "Button_Start";
            this.Button_Start.Size = new System.Drawing.Size(30, 30);
            this.Button_Start.TabIndex = 7;
            this.Button_Start.TabStop = false;
            this.Button_Start.Click += new System.EventHandler(this.Button_Start_Click);
            this.Button_Start.MouseLeave += new System.EventHandler(this.Button_Start_MouseLeave);
            this.Button_Start.MouseHover += new System.EventHandler(this.Button_Start_MouseHover);
            // 
            // Open_File_Dialog_1
            // 
            this.Open_File_Dialog_1.FileName = "Open_File_Dialog_1";
            // 
            // Button_Toggle_Settings
            // 
            this.Button_Toggle_Settings.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Button_Toggle_Settings.Location = new System.Drawing.Point(1, 590);
            this.Button_Toggle_Settings.Name = "Button_Toggle_Settings";
            this.Button_Toggle_Settings.Size = new System.Drawing.Size(30, 30);
            this.Button_Toggle_Settings.TabIndex = 15;
            this.Button_Toggle_Settings.TabStop = false;
            this.Button_Toggle_Settings.Click += new System.EventHandler(this.Button_Toggle_Settings_Click);
            this.Button_Toggle_Settings.MouseLeave += new System.EventHandler(this.Button_Toggle_Settings_MouseLeave);
            this.Button_Toggle_Settings.MouseHover += new System.EventHandler(this.Button_Toggle_Settings_MouseHover);
            // 
            // Button_Reset_Blacklist
            // 
            this.Button_Reset_Blacklist.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Button_Reset_Blacklist.Location = new System.Drawing.Point(399, 590);
            this.Button_Reset_Blacklist.Name = "Button_Reset_Blacklist";
            this.Button_Reset_Blacklist.Size = new System.Drawing.Size(30, 30);
            this.Button_Reset_Blacklist.TabIndex = 16;
            this.Button_Reset_Blacklist.TabStop = false;
            this.Button_Reset_Blacklist.Click += new System.EventHandler(this.Button_Reset_Blacklist_Click);
            this.Button_Reset_Blacklist.MouseLeave += new System.EventHandler(this.Button_Reset_Blacklist_MouseLeave);
            this.Button_Reset_Blacklist.MouseHover += new System.EventHandler(this.Button_Reset_Blacklist_MouseHover);
            // 
            // Combo_Box_Tag_Name
            // 
            this.Combo_Box_Tag_Name.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.Combo_Box_Tag_Name.Font = new System.Drawing.Font("Georgia", 12F);
            this.Combo_Box_Tag_Name.FormattingEnabled = true;
            this.Combo_Box_Tag_Name.Location = new System.Drawing.Point(31, 432);
            this.Combo_Box_Tag_Name.Name = "Combo_Box_Tag_Name";
            this.Combo_Box_Tag_Name.Size = new System.Drawing.Size(367, 26);
            this.Combo_Box_Tag_Name.TabIndex = 18;
            this.Combo_Box_Tag_Name.TextChanged += new System.EventHandler(this.Combo_Box_Tag_Name_TextChanged);
            // 
            // Combo_Box_Type_Filter
            // 
            this.Combo_Box_Type_Filter.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.Combo_Box_Type_Filter.DropDownHeight = 244;
            this.Combo_Box_Type_Filter.Font = new System.Drawing.Font("Georgia", 12F);
            this.Combo_Box_Type_Filter.FormattingEnabled = true;
            this.Combo_Box_Type_Filter.IntegralHeight = false;
            this.Combo_Box_Type_Filter.Location = new System.Drawing.Point(31, 272);
            this.Combo_Box_Type_Filter.Name = "Combo_Box_Type_Filter";
            this.Combo_Box_Type_Filter.Size = new System.Drawing.Size(367, 26);
            this.Combo_Box_Type_Filter.TabIndex = 24;
            this.Combo_Box_Type_Filter.Text = "All Types";
            this.Combo_Box_Type_Filter.TextChanged += new System.EventHandler(this.Combo_Box_Type_Filter_TextChanged);
            // 
            // Label_Type_Filter
            // 
            this.Label_Type_Filter.AutoSize = true;
            this.Label_Type_Filter.Font = new System.Drawing.Font("Georgia", 20F);
            this.Label_Type_Filter.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.Label_Type_Filter.Location = new System.Drawing.Point(31, 238);
            this.Label_Type_Filter.Name = "Label_Type_Filter";
            this.Label_Type_Filter.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Label_Type_Filter.Size = new System.Drawing.Size(144, 31);
            this.Label_Type_Filter.TabIndex = 23;
            this.Label_Type_Filter.Text = "Type Filter";
            // 
            // Label_Entity_Name
            // 
            this.Label_Entity_Name.AutoSize = true;
            this.Label_Entity_Name.Font = new System.Drawing.Font("Georgia", 20F);
            this.Label_Entity_Name.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.Label_Entity_Name.Location = new System.Drawing.Point(31, 318);
            this.Label_Entity_Name.Name = "Label_Entity_Name";
            this.Label_Entity_Name.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Label_Entity_Name.Size = new System.Drawing.Size(166, 31);
            this.Label_Entity_Name.TabIndex = 21;
            this.Label_Entity_Name.Text = "Entity Name";
            // 
            // Combo_Box_Tag_Value
            // 
            this.Combo_Box_Tag_Value.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.Combo_Box_Tag_Value.Font = new System.Drawing.Font("Georgia", 12F);
            this.Combo_Box_Tag_Value.FormattingEnabled = true;
            this.Combo_Box_Tag_Value.Items.AddRange(new object[] {
            "True",
            "False",
            "",
            "Yes",
            "No"});
            this.Combo_Box_Tag_Value.Location = new System.Drawing.Point(31, 512);
            this.Combo_Box_Tag_Value.Name = "Combo_Box_Tag_Value";
            this.Combo_Box_Tag_Value.Size = new System.Drawing.Size(367, 26);
            this.Combo_Box_Tag_Value.TabIndex = 25;
            this.Combo_Box_Tag_Value.TextChanged += new System.EventHandler(this.Combo_Box_Tag_Value_TextChanged);
            // 
            // Combo_Box_Entity_Name
            // 
            this.Combo_Box_Entity_Name.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.Combo_Box_Entity_Name.DropDownHeight = 244;
            this.Combo_Box_Entity_Name.Font = new System.Drawing.Font("Georgia", 12F);
            this.Combo_Box_Entity_Name.FormattingEnabled = true;
            this.Combo_Box_Entity_Name.IntegralHeight = false;
            this.Combo_Box_Entity_Name.Items.AddRange(new object[] {
            "None",
            "Find_And_Replace",
            "Insert_Random_Int",
            "Insert_Random_Float"});
            this.Combo_Box_Entity_Name.Location = new System.Drawing.Point(31, 352);
            this.Combo_Box_Entity_Name.Name = "Combo_Box_Entity_Name";
            this.Combo_Box_Entity_Name.Size = new System.Drawing.Size(367, 26);
            this.Combo_Box_Entity_Name.TabIndex = 26;
            this.Combo_Box_Entity_Name.TextChanged += new System.EventHandler(this.Combo_Box_Entity_Name_TextChanged);
            this.Combo_Box_Entity_Name.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Combo_Box_Entity_Name_KeyPress);
            // 
            // Text_Box_Tags
            // 
            this.Text_Box_Tags.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.Text_Box_Tags.Location = new System.Drawing.Point(31, 272);
            this.Text_Box_Tags.Name = "Text_Box_Tags";
            this.Text_Box_Tags.Size = new System.Drawing.Size(367, 319);
            this.Text_Box_Tags.TabIndex = 27;
            this.Text_Box_Tags.Text = "";
            this.Text_Box_Tags.Visible = false;
            // 
            // Text_Box_Description
            // 
            this.Text_Box_Description.BackColor = System.Drawing.SystemColors.MenuText;
            this.Text_Box_Description.Font = new System.Drawing.Font("Georgia", 15F);
            this.Text_Box_Description.ForeColor = System.Drawing.SystemColors.Info;
            this.Text_Box_Description.Location = new System.Drawing.Point(12, 12);
            this.Text_Box_Description.Name = "Text_Box_Description";
            this.Text_Box_Description.Size = new System.Drawing.Size(404, 164);
            this.Text_Box_Description.TabIndex = 29;
            this.Text_Box_Description.Text = "";
            this.Text_Box_Description.Click += new System.EventHandler(this.Text_Box_Description_Click);
            // 
            // Check_Box_All_Occurances
            // 
            this.Check_Box_All_Occurances.AutoSize = true;
            this.Check_Box_All_Occurances.Font = new System.Drawing.Font("Georgia", 12F);
            this.Check_Box_All_Occurances.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.Check_Box_All_Occurances.Location = new System.Drawing.Point(219, 407);
            this.Check_Box_All_Occurances.Name = "Check_Box_All_Occurances";
            this.Check_Box_All_Occurances.Size = new System.Drawing.Size(184, 22);
            this.Check_Box_All_Occurances.TabIndex = 30;
            this.Check_Box_All_Occurances.Text = "Change all Occurances";
            this.Check_Box_All_Occurances.UseVisualStyleBackColor = true;
            // 
            // List_View_Selection
            // 
            this.List_View_Selection.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.List_View_Selection.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Entries});
            this.List_View_Selection.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.List_View_Selection.FullRowSelect = true;
            this.List_View_Selection.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.List_View_Selection.Location = new System.Drawing.Point(12, 12);
            this.List_View_Selection.Name = "List_View_Selection";
            this.List_View_Selection.Size = new System.Drawing.Size(404, 164);
            this.List_View_Selection.TabIndex = 31;
            this.List_View_Selection.UseCompatibleStateImageBehavior = false;
            this.List_View_Selection.View = System.Windows.Forms.View.Details;
            this.List_View_Selection.Visible = false;
            this.List_View_Selection.SelectedIndexChanged += new System.EventHandler(this.List_View_Selection_SelectedIndexChanged);
            this.List_View_Selection.DragDrop += new System.Windows.Forms.DragEventHandler(this.List_View_Selection_DragDrop);
            this.List_View_Selection.DragEnter += new System.Windows.Forms.DragEventHandler(this.List_View_Selection_DragEnter);
            this.List_View_Selection.DragOver += new System.Windows.Forms.DragEventHandler(this.List_View_Selection_DragOver);
            this.List_View_Selection.DoubleClick += new System.EventHandler(this.List_View_Selection_DoubleClick);
            // 
            // Entries
            // 
            this.Entries.Width = 396;
            // 
            // Button_Run
            // 
            this.Button_Run.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Button_Run.Location = new System.Drawing.Point(399, 510);
            this.Button_Run.Name = "Button_Run";
            this.Button_Run.Size = new System.Drawing.Size(30, 30);
            this.Button_Run.TabIndex = 32;
            this.Button_Run.TabStop = false;
            this.Button_Run.Click += new System.EventHandler(this.Button_Run_Click);
            this.Button_Run.MouseLeave += new System.EventHandler(this.Button_Run_MouseLeave);
            this.Button_Run.MouseHover += new System.EventHandler(this.Button_Run_MouseHover);
            // 
            // Button_Percentage
            // 
            this.Button_Percentage.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Button_Percentage.Location = new System.Drawing.Point(1, 430);
            this.Button_Percentage.Name = "Button_Percentage";
            this.Button_Percentage.Size = new System.Drawing.Size(30, 30);
            this.Button_Percentage.TabIndex = 33;
            this.Button_Percentage.TabStop = false;
            this.Button_Percentage.Visible = false;
            this.Button_Percentage.Click += new System.EventHandler(this.Button_Percentage_Click);
            this.Button_Percentage.MouseLeave += new System.EventHandler(this.Button_Percentage_MouseLeave);
            this.Button_Percentage.MouseHover += new System.EventHandler(this.Button_Percentage_MouseHover);
            // 
            // Button_Operator
            // 
            this.Button_Operator.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Button_Operator.Location = new System.Drawing.Point(1, 510);
            this.Button_Operator.Name = "Button_Operator";
            this.Button_Operator.Size = new System.Drawing.Size(30, 30);
            this.Button_Operator.TabIndex = 34;
            this.Button_Operator.TabStop = false;
            this.Button_Operator.Click += new System.EventHandler(this.Button_Operator_Click);
            this.Button_Operator.MouseLeave += new System.EventHandler(this.Button_Operator_MouseLeave);
            this.Button_Operator.MouseHover += new System.EventHandler(this.Button_Operator_MouseHover);
            // 
            // Button_Search
            // 
            this.Button_Search.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Button_Search.Location = new System.Drawing.Point(399, 270);
            this.Button_Search.Name = "Button_Search";
            this.Button_Search.Size = new System.Drawing.Size(30, 30);
            this.Button_Search.TabIndex = 35;
            this.Button_Search.TabStop = false;
            this.Button_Search.Click += new System.EventHandler(this.Button_Search_Click);
            this.Button_Search.MouseLeave += new System.EventHandler(this.Button_Search_MouseLeave);
            this.Button_Search.MouseHover += new System.EventHandler(this.Button_Search_MouseHover);
            // 
            // Button_Scripts
            // 
            this.Button_Scripts.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Button_Scripts.Location = new System.Drawing.Point(399, 430);
            this.Button_Scripts.Name = "Button_Scripts";
            this.Button_Scripts.Size = new System.Drawing.Size(30, 30);
            this.Button_Scripts.TabIndex = 36;
            this.Button_Scripts.TabStop = false;
            this.Button_Scripts.Click += new System.EventHandler(this.Button_Scripts_Click);
            this.Button_Scripts.MouseLeave += new System.EventHandler(this.Button_Scripts_MouseLeave);
            this.Button_Scripts.MouseHover += new System.EventHandler(this.Button_Scripts_MouseHover);
            // 
            // Button_Backup
            // 
            this.Button_Backup.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Button_Backup.Location = new System.Drawing.Point(1, 270);
            this.Button_Backup.Name = "Button_Backup";
            this.Button_Backup.Size = new System.Drawing.Size(30, 30);
            this.Button_Backup.TabIndex = 37;
            this.Button_Backup.TabStop = false;
            this.Button_Backup.Click += new System.EventHandler(this.Button_Backup_Click);
            this.Button_Backup.MouseLeave += new System.EventHandler(this.Button_Backup_MouseLeave);
            this.Button_Backup.MouseHover += new System.EventHandler(this.Button_Backup_MouseHover);
            // 
            // Button_Browse_Folder
            // 
            this.Button_Browse_Folder.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Button_Browse_Folder.Location = new System.Drawing.Point(1, 193);
            this.Button_Browse_Folder.Name = "Button_Browse_Folder";
            this.Button_Browse_Folder.Size = new System.Drawing.Size(30, 30);
            this.Button_Browse_Folder.TabIndex = 38;
            this.Button_Browse_Folder.TabStop = false;
            this.Button_Browse_Folder.MouseLeave += new System.EventHandler(this.Button_Browse_Folder_MouseLeave);
            this.Button_Browse_Folder.MouseHover += new System.EventHandler(this.Button_Browse_Folder_MouseHover);
            this.Button_Browse_Folder.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Button_Browse_Folder_MouseUp);
            // 
            // Button_Undo
            // 
            this.Button_Undo.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Button_Undo.Location = new System.Drawing.Point(1, 350);
            this.Button_Undo.Name = "Button_Undo";
            this.Button_Undo.Size = new System.Drawing.Size(30, 30);
            this.Button_Undo.TabIndex = 39;
            this.Button_Undo.TabStop = false;
            this.Button_Undo.Visible = false;
            this.Button_Undo.Click += new System.EventHandler(this.Button_Undo_Click);
            this.Button_Undo.MouseLeave += new System.EventHandler(this.Button_Undo_MouseLeave);
            this.Button_Undo.MouseHover += new System.EventHandler(this.Button_Undo_MouseHover);
            // 
            // Button_Attribute
            // 
            this.Button_Attribute.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Button_Attribute.Location = new System.Drawing.Point(399, 350);
            this.Button_Attribute.Name = "Button_Attribute";
            this.Button_Attribute.Size = new System.Drawing.Size(30, 30);
            this.Button_Attribute.TabIndex = 40;
            this.Button_Attribute.TabStop = false;
            this.Button_Attribute.Click += new System.EventHandler(this.Button_Attribute_Click);
            this.Button_Attribute.MouseLeave += new System.EventHandler(this.Button_Attribute_MouseLeave);
            this.Button_Attribute.MouseHover += new System.EventHandler(this.Button_Attribute_MouseHover);
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(428, 622);
            this.Controls.Add(this.Button_Attribute);
            this.Controls.Add(this.Button_Undo);
            this.Controls.Add(this.Button_Backup);
            this.Controls.Add(this.Button_Scripts);
            this.Controls.Add(this.Button_Search);
            this.Controls.Add(this.Button_Operator);
            this.Controls.Add(this.Button_Percentage);
            this.Controls.Add(this.Text_Box_Description);
            this.Controls.Add(this.Button_Run);
            this.Controls.Add(this.List_View_Selection);
            this.Controls.Add(this.Check_Box_All_Occurances);
            this.Controls.Add(this.Combo_Box_Entity_Name);
            this.Controls.Add(this.Combo_Box_Tag_Value);
            this.Controls.Add(this.Combo_Box_Type_Filter);
            this.Controls.Add(this.Label_Type_Filter);
            this.Controls.Add(this.Label_Entity_Name);
            this.Controls.Add(this.Combo_Box_Tag_Name);
            this.Controls.Add(this.Label_Tag_Name);
            this.Controls.Add(this.Button_Reset_Blacklist);
            this.Controls.Add(this.Button_Toggle_Settings);
            this.Controls.Add(this.Button_Start);
            this.Controls.Add(this.Label_Tag_Value);
            this.Controls.Add(this.Track_Bar_Tag_Value);
            this.Controls.Add(this.Text_Box_Original_Path);
            this.Controls.Add(this.Drop_Zone);
            this.Controls.Add(this.Button_Browse_Folder);
            this.Controls.Add(this.Text_Box_Tags);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(444, 660);
            this.MinimumSize = new System.Drawing.Size(444, 660);
            this.Name = "Window";
            this.Text = "Imperialware                    Bruteforce Axe v0.1";
            this.Load += new System.EventHandler(this.Window_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Drop_Zone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Track_Bar_Tag_Value)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Start)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Toggle_Settings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Reset_Blacklist)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Run)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Percentage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Operator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Search)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Scripts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Backup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Browse_Folder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Undo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Attribute)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox Drop_Zone;
        private System.Windows.Forms.TextBox Text_Box_Original_Path;
        private System.Windows.Forms.TrackBar Track_Bar_Tag_Value;
        private System.Windows.Forms.Label Label_Tag_Value;
        private System.Windows.Forms.Label Label_Tag_Name;
        private System.Windows.Forms.PictureBox Button_Start;
        private System.Windows.Forms.OpenFileDialog Open_File_Dialog_1;
        private System.Windows.Forms.PictureBox Button_Toggle_Settings;
        private System.Windows.Forms.PictureBox Button_Reset_Blacklist;
        private System.Windows.Forms.ComboBox Combo_Box_Tag_Name;
        private System.Windows.Forms.ComboBox Combo_Box_Type_Filter;
        private System.Windows.Forms.Label Label_Type_Filter;
        private System.Windows.Forms.Label Label_Entity_Name;
        private System.Windows.Forms.ComboBox Combo_Box_Tag_Value;
        private System.Windows.Forms.ComboBox Combo_Box_Entity_Name;
        private System.Windows.Forms.RichTextBox Text_Box_Tags;
        private System.Windows.Forms.RichTextBox Text_Box_Description;
        private System.Windows.Forms.CheckBox Check_Box_All_Occurances;
        private System.Windows.Forms.ListView List_View_Selection;
        private System.Windows.Forms.ColumnHeader Entries;
        private System.Windows.Forms.PictureBox Button_Run;
        private System.Windows.Forms.PictureBox Button_Percentage;
        private System.Windows.Forms.PictureBox Button_Operator;
        private System.Windows.Forms.PictureBox Button_Search;
        private System.Windows.Forms.PictureBox Button_Scripts;
        private System.Windows.Forms.PictureBox Button_Backup;
        private System.Windows.Forms.PictureBox Button_Browse_Folder;
        private System.Windows.Forms.PictureBox Button_Undo;
        private System.Windows.Forms.PictureBox Button_Attribute;
    }
}

