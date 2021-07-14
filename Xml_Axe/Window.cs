using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using System.Text.RegularExpressions;
using System.Linq;
using System.Xml.Linq;

using System.Diagnostics;
using Microsoft.Win32;
using Microsoft.VisualBasic.FileIO; // For the "Deleting()" function



// ====================================================================================================
// **************************************** 03.2021 Imperial ******************************************
// ====================================================================================================



namespace Xml_Axe
{


    public partial class Window : Form
    {
        public Window()
        {
            InitializeComponent();

            /*
            List_View_Selection.View = View.Details;
            List_View_Selection.HeaderStyle = ColumnHeaderStyle.None;
            List_View_Selection.FullRowSelect = true;
            List_View_Selection.Columns.Add("Farben");
            List_View_Selection.Columns[0].Width = List_View_Selection.Width - 8;
            */
        }


        int Scale_Factor = 10;
        float Min_Float_Range = 0.1F;
        float Max_Float_Range = 1.0F;


        // These 3 are used for scrolling with the XY button
        int Int_Distance = 10;
        float Float_Distance = 1F;
        float Last_Value = 0F;


        int Min_Int_Range = 1;
        int Max_Int_Range = 10;

        string Tag_List = "";
        bool User_Input = false;


        string UI_Mode = "Normal"; // Settings, Backup, Script, Search
        string Operation_Mode = "Int";
        string Scale_Mode = "XY";

        bool EAW_Mode = true;
        bool Silent_Mode = false;
        bool Debug_Mode = true;
        bool Ying_Dominates = false;
        bool Skipp_First_Trigger = true;

        string Temporal_A, Temporal_B = "";
        int Temporal_C = 0;
        int Temporal_D = 0;
        string Selected_Xml = "";
        string Queried_Attribute = "Name"; // Preseting
        string Text_Format_Delimiter = ";";
        string Program_Directory = "";
        string Script_Directory = "";
        string Last_Combo_Box_Tag_Name = "";
        string Last_Combo_Box_Entity_Name = "";


        string Sync_Path, Selected_Backup_Path, Root_Backup_Path, Backup_Path, Backup_Folder, User_Name, Current_Backup, Package_Name = ""; 
        string Backup_Info = @"\Axe_Info.txt";
 
        bool At_Top_Level = true;
        bool Enable_Undo = false;
        string Last_Backup_Selection, Last_Backup_Comment, Last_Backup_Time, Time_Stamp, Current_Hour = "";
        public int Last_Backup_Minute, Current_Minute = 0;
        public int Fetch_Intervall_Minutes = 1;
       

        string[] Balancing_Tags = null; // new string[] { };
        public Color Theme_Color = Color.CadetBlue;
        public string Xml_Directory = Properties.Settings.Default.Xml_Directory;
        public string Mod_Directory = Properties.Settings.Default.Mod_Directory;
        public string Mod_Name = Path.GetFileName(Properties.Settings.Default.Mod_Directory);

        public List<string> Found_Scripts = null;
        public List<string> Found_Factions = new List<string>();
        public List<string> Category_Masks = new List<string>();
        public List<string> Found_Entities = new List<string>();


        List<string> Registered_Files = new List<string>();
        List<string> Not_Matched_Yet = new List<string>();
        List<string> Dont_Copy = new List<string>();

        public List<string> File_Collection = new List<string>();
        public List<string> Blacklisted_Xmls = new List<string>();
        List<string> Difference_List = new List<string>();
        public List<string> Temporal_E = new List<string>();

        // "First_Attribute" is not included here because it isn't supposed to be ignored
        string[] Ignored_Attribute_Values = new string[] { "", "None", "Find_And_Replace", "Insert_Random_Int", "Insert_Random_Float" };





        // ========================== Load ==========================

        private void Window_Load(object sender, EventArgs e)
        {

            this.Size = new Size(Properties.Settings.Default.Last_Width, Properties.Settings.Default.Last_Height);
    
               
            Drop_Zone.AllowDrop = true;
            List_View_Selection.AllowDrop = true;

            Set_Resource_Button(Drop_Zone, Get_Start_Image()); 
            Set_Resource_Button(Button_Browse_Folder, Properties.Resources.Button_Folder_Green); 
            Set_Resource_Button(Button_Start, Properties.Resources.Button_Logs);
            Set_Resource_Button(Button_Backup, Properties.Resources.Button_Clock);
            Set_Resource_Button(Button_Search, Properties.Resources.Button_Search);
            Set_Resource_Button(Button_Attribute, Properties.Resources.Button_Ying);
            Set_Resource_Button(Button_Percentage, Properties.Resources.Button_Percent);
            Set_Resource_Button(Button_Scripts, Properties.Resources.Button_Flash);
            Set_Resource_Button(Button_Operator, Properties.Resources.Button_Minus);
            Set_Resource_Button(Button_Run, Properties.Resources.Button_Axe);
            Set_Resource_Button(Button_Toggle_Settings, Properties.Resources.Button_Settings);
            Set_Resource_Button(Button_Undo, Properties.Resources.Button_Refresh);
            Set_Resource_Button(Button_Reset_Blacklist, Properties.Resources.Button_Controller);


            Control[] Controls = { Button_Browse_Folder, Button_Start, Button_Backup, Button_Run, Button_Search, Button_Attribute, 
                                   Button_Percentage, Button_Scripts, Button_Operator, Button_Reset_Blacklist, Button_Undo, Button_Toggle_Settings };
            foreach (Control Selectrion in Controls) { Selectrion.BackColor = Color.Transparent; }   
    
     
       
       

            Tag_List = Properties.Settings.Default.Tags;
            if (Tag_List == null | Tag_List == "") 
            {   Reset_Tag_List();
                Properties.Settings.Default.Tags = Tag_List;              
                Properties.Settings.Default.Save();          
            }

            Text_Box_Tags.Text = Tag_List;
            Reset_Tag_Box();
            Reset_Root_Tag_Box();


            Queried_Attribute = Properties.Settings.Default.Attribute_Name; 
            Text_Format_Delimiter = Get_Setting_Value("Text_Format_Delimiter");


            if (Verify_Setting("User_Name")) { User_Name = "_" + Get_Setting_Value("User_Name"); }
            else { User_Name = ""; }

            if (User_Name == "_") { User_Name = ""; }


            if (Text_Format_Delimiter == "\\t" | Text_Format_Delimiter == "t") { Text_Format_Delimiter = "\"\t\""; } // Correction Override
            


            // ================== INSTALLATION ==================
            Program_Directory = Get_Program_Dir();
            // Backup_Time(); // Setting a time stamp, running this allows to continue the 5 min timeout from the last session
            Backup_Path = Program_Directory + @"Backup\";
            Script_Directory = Program_Directory + @"Scripts\";


            if (!Directory.Exists(Script_Directory))
            {
                // iConsole(200, 100, Program_Directory);

                byte[] Archive = Properties.Resources.Scripts;
                File.WriteAllBytes(Program_Directory + "Delete_Me.zip", Archive);

                System.IO.Compression.ZipFile.ExtractToDirectory(Program_Directory + @"Delete_Me.zip", Path.GetDirectoryName(Program_Directory));

                // Trashing after extraction, named the archive "Delete_Me" for the case that its autodeletion fails
                Deleting(Program_Directory + "Delete_Me.zip");
            }


   



            // Loading Values, this needs to happen AFTER Reset_Tag_Box() or it will cause errors
            Text_Box_Original_Path.Text = Properties.Settings.Default.Last_File;
            if (Properties.Settings.Default.Steam_Exe_Path == "") { Check_for_Steam_Version(); }

            if (Match_Setting("Store_Last_Settings"))
            {   Combo_Box_Entity_Name.Text = Properties.Settings.Default.Entity_Name;
                Combo_Box_Type_Filter.Text = Properties.Settings.Default.Type_Filter;
                Combo_Box_Tag_Name.Text = Properties.Settings.Default.Tag_Name;
                Combo_Box_Tag_Value.Text = Properties.Settings.Default.Tag_Value;
                Track_Bar_Tag_Value.Value = Properties.Settings.Default.Trackbar_Value;
            }


            // Running this AFTER loading of settings so Combo_Box_Entity_Name.Text can select the last attribute that was selected before close of application.
            Load_Xml_Content(Properties.Settings.Default.Last_File, false); // Need this loaded so In_Selected_Xml(); can match true
            
            Label_Entity_Name.Text = Queried_Attribute; 

            if (Match_Setting("Disable_EAW_Mode"))
            {
                EAW_Mode = false;                      
                Label_Type_Filter.Text = "Parent Name";
            }


            Text_Box_Original_Path.BackColor = Theme_Color;

            User_Input = true;
        }





        //-----------------------------------------------------------------------------
        // Enabling Drag and Drop
        //-----------------------------------------------------------------------------

        // Don't forget about Drag_And_Drop_Area.AllowDrop = true; in the constructor!
        private void Drop_Zone_DragDrop(object sender, DragEventArgs e)
        {
            var Data = e.Data.GetData(DataFormats.FileDrop);
            if (Data != null)
            {
                var File_Names = Data as string[];
                if (File_Names.Length > 0)
                {
                    // string Image_Name = Path.GetFileName(File_Names[0]);                    
                    try // Just to be on the safe side
                    {                      
                        if (!System.Text.RegularExpressions.Regex.IsMatch(File_Names[0], "(?i).*?" + ".xml$")
                           & !File.GetAttributes(File_Names[0]).HasFlag(FileAttributes.Directory)) // Check whether it is a file or a directory                                                     
                        { iConsole(600, 200, "\nError: the file needs to either be a folder or of .xml format."); }
                        else { Set_Paths(File_Names[0]); }

                    } catch {}
                }
            }

        }

        private void Drop_Zone_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy; // This means the dragged Map images        
        }

        private void Drop_Zone_DragOver(object sender, DragEventArgs e)
        { e.Effect = DragDropEffects.Move; }


        private void Drop_Zone_Click(object sender, EventArgs e)
        { Disable_Description(); } // Hiding




        private void Text_Box_Description_Click(object sender, EventArgs e)
        {
            if (UI_Mode != "Backup") { Disable_Description(); }
        }

        private void Disable_Description()
        {
            Text_Box_Description.Text = "";
            Text_Box_Description.Visible = false;
        }

        //===========================// 
        private void List_View_Selection_DoubleClick(object sender, EventArgs e)
        {
            if (UI_Mode != "Backup" && List_View_Selection.Items.Count > 0)
            {
                for (int i = List_View_Selection.Items.Count - 1; i >= 0; --i)
                { List_View_Selection.Items[i].Selected = true; }
            }
        }

        //===========================// 
        private void Get_Sync_Path()
        {   try
            {   Sync_Path = Get_Backup_Info(Root_Backup_Path)[0];

                // iConsole(400, 100, "Sync_Path is \n" + Sync_Path);

                if (!Sync_Path.EndsWith(@"\")) { Sync_Path += @"\"; } // Needs to run BEFORE Sync_Path setting, \ would only break the path

                // Failsafe, if there are no backups to read, fallback to Xml_Directory
                if (Sync_Path == @"None\") { Sync_Path = Xml_Directory; }
            } catch { iConsole(400, 100, "Faild to load Sync_Path."); }
        }

        //===========================// 
        private void List_View_Selection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UI_Mode == "Script") { Run_Script(); } // Script Mode override

            else if (UI_Mode == "Backup") // Just move into the Xml Directory that is currently selected 
            {   if (At_Top_Level) // Leave this below here!
                {   Button_Operator_MouseLeave(null, null);
                    // Button_Scripts.Visible = true;
                    Button_Attribute.Visible = true;

                    Backup_Folder = Select_List_View_First(List_View_Selection); // This defines which Backup dir is targeted!!
                    Selected_Backup_Path = Backup_Path + Backup_Folder + @"\";

                    Zoom_List_View(2); 
                    // Grabbing the Path we're going to use to sync at
                    
                    Refresh_Backup_Stack();
                    // Get_Sync_Path() needs to run AFTER Refresh_Backup_Stack() because it loads Root_Backup_Info  
                    Get_Sync_Path();
                   

                    Button_Search_MouseLeave(null, null);
                    Button_Attribute_MouseLeave(null, null);           
                }

                else // if (!At_Top_Level)
                {
                    // User_Input coordinates the timing with the Restore() function.
                    // De-select Parent Backups while no slot is selected
                    if (User_Input) { Set_Backup_Checker(); } 
                    
                    // Switch between Red and Green Color
                    Button_Browse_Folder_MouseLeave(null, null);           
                }                                                             
            }

            else // Normal Mode
            {
                if (List_View_Selection.SelectedItems.Count < 2)
                { Combo_Box_Entity_Name.Text = Select_List_View_First(List_View_Selection); }
                else
                {
                    Combo_Box_Entity_Name.Text = "Multi";
                }
            }

        }




        public List<string> Refresh_Backup_Directory()
        {
            Temporal_E = Get_All_Directories(Backup_Path, true);

            foreach (string Folder in Temporal_E)
            {
                string Folder_Name = Folder.Replace(Backup_Path, "");
                if (Folder_Name != "" && !List_View_Matches(List_View_Selection, Folder_Name))
                { List_View_Selection.Items.Add(Folder_Name); }
            }
 
            Set_Checker(List_View_Selection, Theme_Color);
            return Temporal_E;
        }


        public void Refresh_Backup_Stack()
        {   try
            {
                Root_Backup_Path = Backup_Path + Backup_Folder + Backup_Info;
                Current_Backup = Get_Backup_Info(Root_Backup_Path)[1];
                //Label_Entity_Name.Text = Current_Version; // Obsolete because I highlight bg color now
                //Label_Entity_Name.Location = new Point(86, -2);
             

                List_View_Selection.Items.Clear();               
                Get_Backup_Dirs(); 
                At_Top_Level = false;

                Set_Backup_Checker();             

            } catch {}  
        }


        private List<string> Get_Backup_Dirs()
        {
            List<string> Found_Directories = new List<string>();
            Temporal_E = Get_All_Directories(Backup_Path + @"\" + Backup_Folder, true);
            Temporal_E.Reverse();

            foreach (string Found_Dir in Temporal_E)
            {
                string Folder_Name = Found_Dir.Replace(Backup_Path + @"\" + Backup_Folder + @"\", "");

                // "Current" here means the Current directory in .\Backup, this program assembles new patches inside of it.
                if (Folder_Name != "" && Folder_Name != "Current" && Folder_Name != Backup_Folder && !List_View_Matches(List_View_Selection, Folder_Name))
                {
                    if (UI_Mode == "Backup") { List_View_Selection.Items.Add(Folder_Name); }
                    else { Found_Directories.Add(Folder_Name); }  
                }
            }

            return Found_Directories;
        }
   

        private void List_View_Selection_DragDrop(object sender, DragEventArgs e)
        {
            var Data = e.Data.GetData(DataFormats.FileDrop);
            if (Data != null)
            {
                var File_Names = Data as string[];
                if (File_Names.Length > 0)
                {
                    // string Image_Name = Path.GetFileName(File_Names[0]);                    
                    try // Just to be on the safe side
                    {
                        if (UI_Mode == "Backup" && At_Top_Level)
                        {
                            Temporal_B = Path.GetFileName(File_Names[0]);
                            if (!Directory.Exists(Backup_Path + Temporal_B)) { Directory.CreateDirectory(Backup_Path + Temporal_B); Refresh_Backup_Directory(); }
                        }
                        else
                        {   if (!Regex.IsMatch(File_Names[0], "(?i).*?" + ".xml$")
                               & !File.GetAttributes(File_Names[0]).HasFlag(FileAttributes.Directory)) // Check whether it is a file or a directory                                                     
                            { iConsole(600, 200, "\nError: the file needs to either be a folder or of .xml format."); }
                            else { Set_Paths(File_Names[0]); }
                        }

                    }
                    catch { }
                }
            }
        }

        private void List_View_Selection_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy; // This means the dragged Map images        
        }

        private void List_View_Selection_DragOver(object sender, DragEventArgs e)
        { e.Effect = DragDropEffects.Move; }


        //===========================// 

        private void Set_Paths(string The_Path, bool Refresh_Dir = true)
        {
            Text_Box_Original_Path.Text = The_Path;
            Properties.Settings.Default.Last_File = The_Path;
            Temporal_A = Path.GetDirectoryName(The_Path);


            if (Refresh_Dir)
            {
                Combo_Box_Entity_Name.Text = ""; // Resetting         
                
                for (int i = 0; i < 10; i++)
                {
                    // Keep removing the last directory in the path,
                    if (Is_Match(Temporal_A, "xml$"))
                    {
                        List_View_Selection.Visible = true;
                        Load_Xml_Content(The_Path, true);
                    }


                    // Don't use else if here!
                    if (!Is_Match(Temporal_A, "xml$")) 
                    {
                        if (!EAW_Mode)
                        {   // Then the parent dir is always the selected Xml_Directory 
                            Xml_Directory = Temporal_A;
                            if (!Xml_Directory.EndsWith(@"\")) { Xml_Directory += @"\"; }

                            Properties.Settings.Default.Xml_Directory = Xml_Directory; 
                            break;
                        }

                        Temporal_A = Path.GetDirectoryName(Temporal_A) + @"\"; // Trying to utilize recursion on Temporal_A in the next loop
                    } 

                    else if (Is_Match(Temporal_A, "data$"))
                    {
                        Xml_Directory = Temporal_A + @"\xml\"; // Updating 
                        Properties.Settings.Default.Xml_Directory = Xml_Directory;
                        // Leaping back for a directy, to get the name of the Modpath
                        Properties.Settings.Default.Mod_Directory = Path.GetDirectoryName(Temporal_A);
                        Mod_Directory = Properties.Settings.Default.Mod_Directory;
                        Mod_Name = Path.GetFileName(Mod_Directory);

                        // iConsole(600, 100, Path.GetDirectoryName(Path.GetDirectoryName(Temporal_A)));
                        break;
                    }

                    else //Until we got to the Xml directory
                    {                      
                        Xml_Directory = Temporal_A; // Updating 


                        for (int d = 0; d < 8; d++)
                        {
                            try {
                                if (Xml_Directory.ToLower().EndsWith(@"xml")) { Xml_Directory += @"\"; break; }
                                else
                                {
                                    Xml_Directory = Path.GetDirectoryName(Xml_Directory); // Repeat until matched or empty
                                    // iConsole(400, 100, Xml_Directory);
                                } 

                            } catch { Xml_Directory = Temporal_A; break; } // Something went wrong!
                        }




                        // if (!Xml_Directory.EndsWith(@"\")) { Xml_Directory += @"\"; }

                        Properties.Settings.Default.Xml_Directory = Xml_Directory;


                    
                        Temporal_B = Path.GetDirectoryName(Path.GetDirectoryName(Xml_Directory));

                        // Leaping back by 2 directoies, to get the name of the Modpath
                        if (Temporal_B.ToLower().EndsWith("data")) { Properties.Settings.Default.Mod_Directory = Path.GetDirectoryName(Temporal_B); }
                        else { Properties.Settings.Default.Mod_Directory = Temporal_B; } // Hoping this is the right one

                        Mod_Directory = Properties.Settings.Default.Mod_Directory;
                        Mod_Name = Path.GetFileName(Mod_Directory);

                        // iConsole(600, 100, Path.GetDirectoryName(Path.GetDirectoryName(Xml_Directory)));
                        break;
                    }
                }
            }

            else { List_View_Selection.Visible = true; } // If not Refresh_Dir, we trust that the selected file is a xml
            

            Load_Xml_Content(The_Path, true);

            Set_Resource_Button(Button_Start, Properties.Resources.Button_Logs_Lit);
            Set_Checker(List_View_Selection, Theme_Color);

            if (Text_Box_Description.Visible) { Disable_Description(); }

         
            Properties.Settings.Default.Save();
        }


        //===========================//

        private void Track_Bar_Tag_Value_Scroll(object sender, EventArgs e)
        {   User_Input = false;
            
            string Operator = "";     
            if (Combo_Box_Tag_Value.Text.StartsWith("-")) { Operator = "-"; } // Remain -



            if (Operation_Mode == "Point")
            {
                int New_Value = Track_Bar_Tag_Value.Value;
                // Int_Distance is reset to 10, each time Combo_Box_Entity_Name.Text is set to "Insert_Random_Int" 
         
                if (Scale_Mode == "XY")
                {   Min_Int_Range = New_Value;
                    Max_Int_Range = New_Value + Int_Distance; // Apply distance between the 2 values
                }
                else if (Scale_Mode == "X")
                {  
                    Update_Distance();
                    Min_Int_Range = New_Value;
                }
                else if (Scale_Mode == "Y") 
                {
                    Update_Distance(); // Updating moves Int_Distance value up or down according to the scroll bar
                    Max_Int_Range = (int)Last_Value + Int_Distance; // Last_Value because New_Value means +2 instead of +1           
                }

                
                Last_Value = New_Value;
                Combo_Box_Tag_Value.Text = (Operator + Min_Int_Range + " | " + Operator + Max_Int_Range).Replace(",", ".");
            }
            else if (Operation_Mode == "Point_Float")
            {
                float New_Value = (float)Track_Bar_Tag_Value.Value / 10;               
                // Float_Distance is reset to 1.0F, each time Combo_Box_Entity_Name.Text is set to "Insert_Random_Float"
                

                if (Scale_Mode == "XY")
                {
                    Min_Float_Range = New_Value;
                    Max_Float_Range = New_Value + Float_Distance;
                }
                else if (Scale_Mode == "X") { Update_Distance(New_Value); Min_Float_Range = New_Value; }
                else if (Scale_Mode == "Y") { Update_Distance(New_Value); Max_Float_Range = Last_Value + Float_Distance; }


                Last_Value = New_Value;
                // string Prefix = "1.";
                if (Scale_Mode == "X" && Track_Bar_Tag_Value.Value == 10) { Max_Float_Range = 2; } //Prefix = ""; }
                string Float_Range = Max_Float_Range.ToString("n1"); // Format to string with 1 decimal number

                Combo_Box_Tag_Value.Text = (Operator + Min_Float_Range + " | " + Operator + Float_Range).Replace(",", ".");             
            }
   
            else
            {                
                string Percentage = "";
           
                if (Operation_Mode == "Percent") // Don't place this above!
                {
                    if (!Combo_Box_Tag_Value.Text.StartsWith("-")) { Operator = "+"; } // Defaulting Prefix to + 
                    Percentage = "%";
                }

                Combo_Box_Tag_Value.Text = Operator + (Track_Bar_Tag_Value.Value * Scale_Factor) + Percentage;
            }


            User_Input = true;

            // Check to disable Bool Mode
            Combo_Box_Tag_Value_TextChanged(null, null); // This must run with User_Input
        }


        //===========================//
        private bool Update_Distance(float Passed_Value = 0) // Returning true means value gets positive, minus direction returns negative
        {
            if (Passed_Value > 0) // Then float type
            {   if (Last_Value < Passed_Value) { Float_Distance += 0.1F; return true; }
                else if (Last_Value > Passed_Value && Float_Distance > 0.1F) { Float_Distance -= 0.1F; return false; }
            }
            else
            {
                if (Last_Value < Track_Bar_Tag_Value.Value) { Int_Distance++; return true; }
                else if (Last_Value > Track_Bar_Tag_Value.Value) { Int_Distance--; return false; }
            }

            return false;
        }

        //===========================//

        private void Button_Browse_Folder_MouseUp(object sender, MouseEventArgs Mouse)
        {
            if (UI_Mode == "Script") { Execute(Script_Directory); }

            else if (UI_Mode == "Backup") 
            {
                if (At_Top_Level) { Execute(Backup_Path); } 
                else
                {
                    string Selection = Select_List_View_First(List_View_Selection);

                    if (Selection == "") { Execute(Backup_Path); return; } // Failsafe fallback to parent directory
                    
                    else if (Mouse.Button == MouseButtons.Right)
                    {   Set_Resource_Button(Button_Browse_Folder, Properties.Resources.Button_File_Lit);
                        Execute(Get_Backup_Path(Selection) + Backup_Info);
                        System.Threading.Thread.Sleep(2000);
                        return;                      
                    }
                   

                    Temporal_E = Get_All_Directories(Backup_Path + Backup_Folder, true);
                    Temporal_E.Reverse();
                    // iConsole(400, 100, string.Join("\n", Temporal_E));

                    foreach (string Found_Dir in Temporal_E)
                    {
                        string Folder_Name = Found_Dir.Replace(Selected_Backup_Path, "");                

                        if (Folder_Name != "" && Folder_Name == Selection)
                        {   // iConsole(400, 100, Folder_Name + " == " + Selection);
                            Execute(Found_Dir); return;
                        }
                    }
                }            
            }

            else // Normal Mode
            {
                if (Mouse.Button == MouseButtons.Right)
                {
                    Set_Resource_Button(Button_Browse_Folder, Properties.Resources.Button_File_Lit);
                    Execute(Properties.Settings.Default.Last_File);
                    System.Threading.Thread.Sleep(2000); // Leave some time to show the File Icon
                    return;
                }


                Open_File_Dialog_1.FileName = "";
                if (Properties.Settings.Default.Last_File != null & Properties.Settings.Default.Last_File != "")
                { Open_File_Dialog_1.InitialDirectory = Path.GetDirectoryName(Properties.Settings.Default.Last_File); }

                Open_File_Dialog_1.Filter = "xml files (*.xml)|*.xml|Text files (*.txt)|*.txt";
                //Open_File_Dialog_1.FilterIndex = 1;
                //Open_File_Dialog_1.RestoreDirectory = true;
                //Open_File_Dialog_1.CheckFileExists = true;
                //Open_File_Dialog_1.CheckPathExists = true;

                try
                {   // If the Open Dialog found a File
                    if (Open_File_Dialog_1.ShowDialog() == DialogResult.OK)
                    {
                        Text_Box_Original_Path.Text = Open_File_Dialog_1.FileName;
                        Set_Paths(Open_File_Dialog_1.FileName);
                    }
                } catch {}
            }
          
        }

        private void Button_Browse_Folder_MouseHover(object sender, EventArgs e)
        {
            Temporal_C = 0;
            try { Temporal_C = Select_List_View_Items(List_View_Selection).Count; } catch {}

            if (UI_Mode == "Script" || UI_Mode == "Backup" && Temporal_C == 0)
            { Set_Resource_Button(Button_Browse_Folder, Properties.Resources.Button_Folder_Red_Lit); }         
            else { Set_Resource_Button(Button_Browse_Folder, Properties.Resources.Button_Folder_Green_Lit); }
        }

        private void Button_Browse_Folder_MouseLeave(object sender, EventArgs e)
        {
            Temporal_C = 0;
            try { Temporal_C = Select_List_View_Items(List_View_Selection).Count; }  catch {}

            if (UI_Mode == "Script" || UI_Mode == "Backup" && Temporal_C == 0)
            { Set_Resource_Button(Button_Browse_Folder, Properties.Resources.Button_Folder_Red); }
            else { Set_Resource_Button(Button_Browse_Folder, Properties.Resources.Button_Folder_Green); }
        }


        //===========================//
        private void Button_Start_Click(object sender, EventArgs e)
        {
            if (UI_Mode == "Backup") // Deletion Function
            {
                Temporal_E = Select_List_View_Items(List_View_Selection);
                if (Temporal_E.Count() == 0) { iConsole(400, 100, "\nPlease select any of the backups."); return; }


                bool Selected_Base = false;
                foreach (string Folder_Path in Temporal_E)
                {
                    if (Folder_Path == Current_Backup)
                    {
                        iConsole(540, 140, "\nYou can not delete the selected backup, \nplease checkout -> any other backup and try again.\nI recommend to checkout the newest possible one.");
                        return; // Exit as the Checked out Backup can not be deleted.
                    }

                    else if (Folder_Path.EndsWith("_Base")) { Selected_Base = true; break; }
                }

        



                if (Selected_Base)
                { iDialogue(540, 260, "Do It", "Cancel", "false", "false", "\nWait, you selected a \"Base\" backup. But the backup \n" +
                    "feature needs it to compare against new changes.\n" +
                    "Are you sure you wish to delete the selected backup?\n\n" +
                    "Instead you could merge Backups into a new Base." ); 
                }
              
                else
                {   string s = "";
                    if (Temporal_E.Count > 1) { s = "s"; }

                    iDialogue(540, 200, "Do It", "Cancel", "false", "false", "\nAre you sure you wish to delete the \nselected backup" + s + "?");
                }

                if (Caution_Window.Passed_Value_A.Text_Data == "false") { return; }

           

                foreach (string Folder_Path in Temporal_E)
                {
                    string Full_Path = Selected_Backup_Path + Folder_Path;
                    // iConsole(600, 100, Full_Path);

                    if (Directory.Exists(Full_Path)) 
                    { 
                        // Directory.Delete(Full_Path);
                        Deleting(Full_Path);

                        foreach (ListViewItem Item in List_View_Selection.Items)
                        { if (Item.Text == Folder_Path) { List_View_Selection.Items.Remove(Item); } }
                    }                   
                }

                Refresh_Backup_Stack();
            }
            
            else
            {   if (List_View_Selection.Visible) 
                {   List_View_Selection.Visible = false; 
                    Zoom_List_View(1);

                    if (UI_Mode == "Search") // toggle
                    {   Clear_Last_Mode(); 
                        Set_UI_Into_Search_Mode(false); // Needs to run after Clear_Last_Mode(); 
                    } 
                }
                else
                {   Load_Xml_Content(Properties.Settings.Default.Last_File); // Auto toggles to visible 

                    if (Text_Box_Description.Visible) { Disable_Description(); }
                }     
            }
                                 
        }

        private void Button_Start_MouseHover(object sender, EventArgs e)
        {
            if (UI_Mode == "Backup") { Set_Resource_Button(Button_Start, Properties.Resources.Button_Delete_Lit); }
            else { Set_Resource_Button(Button_Start, Properties.Resources.Button_Logs_Lit); }
        }

        private void Button_Start_MouseLeave(object sender, EventArgs e)
        {
            if (UI_Mode == "Backup") { Set_Resource_Button(Button_Start, Properties.Resources.Button_Delete); }
            else
            {   if (List_View_Selection.Visible) // Use its visability as bool toggle ;)
                { Set_Resource_Button(Button_Start, Properties.Resources.Button_Logs_Lit); }
                else { Set_Resource_Button(Button_Start, Properties.Resources.Button_Logs); }
            }
        }

      
        //===========================// 

        private void Button_Run_Click(object sender, EventArgs e)
        {   
            string Parent_Name = Combo_Box_Type_Filter.Text;
            string Parent_Attribute_Value = Combo_Box_Entity_Name.Text;

            string Child_Name = Combo_Box_Tag_Name.Text;
            string Child_Value = Combo_Box_Tag_Value.Text;
        

           
            // iConsole(600, 400, Check_for_Steam_Version()); // Debug
            if (Child_Name == "") { return; }


            if (Operation_Mode == "Percent" && Child_Value == "-100%")
            {   iDialogue(540, 200, "Do It", "Cancel", "false", "false", "\nRemoving -100% means set the value to 0 \nare you sure about that?");
                if (Caution_Window.Passed_Value_A.Text_Data == "false") { return; }
            }

            Set_Resource_Button(Drop_Zone, Get_Start_Image());
      

   
            if (File.Exists(Xml_Directory + "Axe_Blacklist.txt")) 
            { Blacklisted_Xmls = File.ReadAllLines(Xml_Directory + "Axe_Blacklist.txt").ToList(); }



            Temporal_C = 0;
            // bool Warn_User = true;
            string The_Settings = Properties.Settings.Default.Tags;
            List<string> Related_Xmls = new List<string>();


            // Storing last search
            if (Array_Matches(Ignored_Attribute_Values, Parent_Attribute_Value)) 
            { Properties.Settings.Default.Entity_Name = ""; }
            else { Properties.Settings.Default.Entity_Name = Wash_String(Parent_Attribute_Value); }


            if (Child_Name != "Rebalance_Everything" & Parent_Name != "Faction Name Filter"
                & Parent_Name != "Category Mask Filter") // We don't want the user to accidently re-apply such a powerfull setting
            {
                Properties.Settings.Default.Type_Filter = Parent_Name;

                // Would otherwise end up in loading a broken selection, because the selection was stored in the ListView of the last session:
                if (Parent_Attribute_Value != "Multi") { Properties.Settings.Default.Tag_Name = Child_Name; }

                if (Child_Value.Contains("%")) { Properties.Settings.Default.Tag_Value = ""; } // Preventing Errors
                else { Properties.Settings.Default.Tag_Value = Child_Value; }
            }

            Properties.Settings.Default.Trackbar_Value = Track_Bar_Tag_Value.Value;
            Properties.Settings.Default.Save(); // Storing last usage



            Backup_Folder = Mod_Name;
            Selected_Backup_Path = Backup_Path + Backup_Folder + @"\";
            Root_Backup_Path = Backup_Path + Backup_Folder + Backup_Info;

            Current_Backup = Get_Backup_Info(Root_Backup_Path)[1];
            Get_Sync_Path();


            // if (The_Settings.Contains("Show_Files_That_Would_Change = true") | The_Settings.Contains("Request_Approval=true"))
            if (Match_Setting("Request_File_Approval") & !In_Selected_Xml(Parent_Attribute_Value))
            {
                // Warn_User = false;                  
                Related_Xmls = Slice(false); // Means don't apply any changes
                try
                {
                    Temporal_C = (Related_Xmls.Count() * 30) + 160;
                    if (Temporal_C > 680) { Temporal_C = 680; }
          

               

                    if (Related_Xmls.Count == 0) // the Slice function is supposed to fill this list with paths
                    {
                        string Error_Text = "\nI'm sorry, no entries with Attribute Name \"" + Queried_Attribute
                        + "\"\nand Attribute Value \"" + Parent_Attribute_Value + "\" were found \nto contain the child name \"" + Child_Name + "\"";

                        if (Array_Matches(Ignored_Attribute_Values, Parent_Attribute_Value)) // Then the query went by filter, which is name of the Entities root tag
                        {
                            Error_Text = "\nI'm sorry, no entries with Entity Parent Tag Name \"" + Parent_Name
                            + "\" were found \nto contain the child name \"" + Child_Name + "\"";
                        }


                        iConsole(600, 100, Error_Text); return;
                    } // 680
                    else { iDialogue(740, Temporal_C, "Yes", "Cancel", "Ignore", "Blacklist", "(Strg + Click to select multiple.)   Are you sure you wish to apply changes to:", Related_Xmls, true); }

                } catch { iConsole(400, 100, "Error"); }


                if (Caution_Window.Passed_Value_A.Text_Data == "false") { return; }
            }



            if (Is_Time_To_Backup()) { Backup_Time(); } // This updates the Time_Stamp variable we use to define name of the Backup dir!
            Package_Name = Time_Stamp + User_Name;

            Related_Xmls = Slice(true); // This line does the actual Job!

            bool Has_Collapsed = Collapse_Oldest_Backup("Silent");

            Enable_Undo = true;
            Toggle_Undo_Button(true);


            Set_Resource_Button(Drop_Zone, Get_Done_Image());
            if (List_View_Selection.Visible) { Button_Start_Click(null, null); } // Hiding open Xml

            if (Text_Box_Description.Visible) { Disable_Description(); }





            if (Related_Xmls == null || Related_Xmls.Count() == 0) { return; }

            try
            {
                string This_Backup = Package_Name + "_Auto";

                // if (Verify_Setting("Backups_Per_Directory")) { // Disabled Feature



                // ======== Formating Action Report Information: ========
                string Change_Info = "Stored before applying following changes: \n";
                string[] Tag_Values = new string[] { "Parent Name = ", "\nAttribute ", "\nChild Name = ", "\nChild Value = " };
                if (EAW_Mode) { Tag_Values = new string[] { "Type = ", "\nUnit ", "\nTag Name = ", "\nTag Value = " }; } // Use other names



                if (Parent_Name != "" && Parent_Name != "All Types") { Change_Info += Tag_Values[0] + Parent_Name; }
                if (Parent_Attribute_Value != "" && Parent_Attribute_Value != "None") 
                { Change_Info += Tag_Values[1] + Queried_Attribute + " = \"" + Parent_Attribute_Value + "\""; }

                if (Child_Name != "") { Change_Info += Tag_Values[2] + Child_Name; }
                if (Child_Value != "") { Change_Info += Tag_Values[3] + Child_Value; }



                // Update the Backup Version and AUTOMATICALLY CREATE BACKUP from the list of Related_Xmls
                // It happens to fail to Collapse when the User has the last backup in the stack loaded.
                if (!File.Exists(Selected_Backup_Path + This_Backup + Backup_Info))
                { Create_Backup_Info(Mod_Name, This_Backup, Change_Info, string.Join("\n", Related_Xmls), false, true); }
               
                else
                {   // iConsole(400, 100,  Selected_Backup_Path + This_Backup); 

            
                    foreach (string Present_File in Get_Segment_Info(Current_Backup, "Changed_Files"))
                    {   // Removing duplicates
                        if (Related_Xmls.Contains(Present_File)) { Related_Xmls.Remove(Present_File); }
                    }


                    // CAUTION, there are difference between this method and Create_Backup_Info() above.
                    Write_Into_Segment(This_Backup, "Changed_Files", Related_Xmls);
                }              
            
                    
            } catch { iConsole(400, 100, "\nFailed to create Axe_Info.txt for the current backup."); }
           


            // Disabled Feature, quite obsolete
            /*if (Related_Xmls.Count() > 0 & Warn_User & !In_Selected_Xml(Combo_Box_Entity_Name.Text) 
              & Match_Setting("Show_Changed_Files"))
            {
                Temporal_C = (Related_Xmls.Count() * 30) + 90;
                if (Temporal_C > 680) { Temporal_C = 680; }
                iConsole(560, Temporal_C, "\nApplied Changes to the following files: \n\n" + string.Join("\n", Related_Xmls));
            }*/
        }


        private void Button_Run_MouseHover(object sender, EventArgs e)
        { Set_Resource_Button(Button_Run, Properties.Resources.Button_Axe_Lit); }

        private void Button_Run_MouseLeave(object sender, EventArgs e)
        { Set_Resource_Button(Button_Run, Properties.Resources.Button_Axe); }


        public bool Verify_Setting(string The_Setting)
        {
            string Found_Setting = Get_Setting_Value(The_Setting);

            if (Found_Setting != "0" && Found_Setting != "" && !Match_Without_Emptyspace(Found_Setting, "false")) { return true; }
            return false;
        }


        private bool Match_Setting(string Entry)
        {   if (Match_Without_Emptyspace(Properties.Settings.Default.Tags, Entry + " = true")) { return true; }
            return false;
        }

        // Custom_Parameters, Queried_Attribute, Text_Format_Delimiter
        private string Get_Setting_Value(string Entry, bool Return_Full_Line = false)
        {
            foreach (string Line in Properties.Settings.Default.Tags.Split('\n'))
            {   if (Line != "" & Line.Contains(Entry))
                {   if (Return_Full_Line) { return Remove_Emptyspace_Prefix(Line).Replace("\r", ""); }
                    else { return Remove_Emptyspace_Prefix(Line.Split('=')[1]).Replace("\r", ""); }
                } 
            }

            return "";
        }


        private bool Match_Without_Emptyspace(string Entry_1, string Entry_2)
        {   try { if (Regex.IsMatch(Entry_1.Replace(" ", ""), "(?i)" + Entry_2.Replace(" ", ""))) { return true; } } catch {}    
            return false;
        }


        private bool Match_Without_Emptyspace_2(string Entry_1, string Entry_2)
        {   if (Entry_1.ToLower().Replace(" ", "").Contains(Entry_2.ToLower().Replace(" ", ""))) { return true; }
            return false;
        }

      
        private bool Is_Match(string Entry_1, string Entry_2)
        {   try { if (Regex.IsMatch(Entry_1, "(?i)" + Entry_2)) { return true; } } catch {}          
            return false;
        }

        private bool Is_Match_2(string Entry_1, string Entry_2) // This is as slow as the Regex method XP
        {
            if (Entry_1.ToLower() == Entry_2.ToLower()) { return true; }
            return false;
        }


        public string Get_Item_That_Contains(List<string> The_List, string Item)
        {
            foreach (string Entry in The_List)
            { if (Entry.Contains(Item)) { return Entry; } }

            return "";
        }


        // public List <string> Load_Xml_Content(string Xml_Path)
        public void Load_Xml_Content(string Xml_Path, bool Show_List = true)
        {
            List_View_Selection.Items.Clear();

            // List<string> Found_Entities = null;
            IEnumerable<XElement> Instances = null;
            if (!File.Exists(Xml_Path)) { iConsole(200, 100, "\nCan't find the Xml."); return; }
        
            List_View_Selection.Visible = true;


            try
            {   // ===================== Opening Xml File =====================
                XDocument Xml_File = XDocument.Load(Xml_Path, LoadOptions.PreserveWhitespace);
             
                Instances = // Target all entities in the whole Mod!
                  from All_Tags in Xml_File.Root.Descendants()
                    // Selecting all non empty tags that have the Attribute "Name", null because we need all selected.
                      where All_Tags != null
                        select All_Tags;



                // iConsole(200, 100, string.Join("\n", Instances));

                // =================== Checking Xml Instance ===================
                foreach (XElement Instance in Instances)
                {   if (Instance.Descendants().Any())
                    {
                        string Entity_Name = (string)Instance.Attribute(Queried_Attribute);
                        if (Match_Without_Emptyspace(Queried_Attribute, "First_Attribute")) { Entity_Name = (string)Instance.FirstAttribute; }

                        if (Entity_Name != null)
                        {
                            if (!Match_Without_Emptyspace(Entity_Name, "Death_Clone") & !List_View_Matches(List_View_Selection, Entity_Name))
                            { List_View_Selection.Items.Add(Entity_Name); }
                        }
                    }
                }

                Set_Checker(List_View_Selection, Theme_Color);
                if (!Show_List) { List_View_Selection.Visible = false; }


                // If the selected entity is within the selected Xml:
                if (In_Selected_Xml(Combo_Box_Entity_Name.Text)) 
                { 
                    foreach (ListViewItem Item in List_View_Selection.Items)
                    {
                        if (Item.Text == Combo_Box_Entity_Name.Text) { Item.Selected = true; break; }
                    }
                }

            } catch {}
        }


        public bool Array_Matches(string[] The_List, string Item, bool Case_Sensitive = true)
        {
            if (Case_Sensitive)
            { 
                foreach (string Entry in The_List) { if (Entry == Item) { return true; } } 
            }
            else 
            {
                foreach (string Entry in The_List) { if (Entry.ToLower() == Item.ToLower()) { return true; } } 
            }

            return false;
        }

        public bool List_Matches(List<string> The_List, string Item)
        {
            bool Is_Match = false;
            foreach (string Entry in The_List)
            {
                if (Entry == Item) { Is_Match = true; }
            }

            return Is_Match;
        }

        public bool List_View_Matches(ListView The_List, string Item)
        {
            bool Is_Match = false;
            foreach (ListViewItem Entry in The_List.Items)
            {
                if (Entry.Text == Item) { Is_Match = true; }
            }

            return Is_Match;
        }


        public bool Combo_Box_Matches(ComboBox The_List, string Item, bool Case_Sensitive = false)
        {
            if (Case_Sensitive)
            {   foreach (string Entry in The_List.Items)
                { if (Entry.ToLower() == Item.ToLower()) { return true; } }
            }
            else
            {   foreach (string Entry in The_List.Items)
                {
                    if (Entry == Item) { return true; }
                }
            }

            return false;
        }



        public bool Collapse_Oldest_Backup(string Certain_Backup)
        {
            string Backup_Count = Get_Setting_Value("Backups_Per_Directory");        

            if (Verify_Setting("Backups_Per_Directory"))
            {               
                int Maximal_Backups = Int32.Parse(Backup_Count);
                int Found_Backups = Get_All_Directories(Backup_Path + Backup_Folder, true).Count();


                // iConsole(400, 100, Backup_Path + Backup_Folder);
                // iConsole(400, 100, "Set to " + Maximal_Backups);
                if (Found_Backups > Maximal_Backups) // If reached max count, we need to delete the last one!
                {
                    // iConsole(400, 100, "Found " + Found_Backups + " from " + Maximal_Backups);
                    if (Collapse_Backup_Stack(Certain_Backup)) { return true; }
                    // Outdated: if (Temporal_E.First() != Current_Backup) { Deleting(Temporal_E.First()); return true; }// Trash ONLY the last one
                }
            }

            return false;
        }

        
        //-----------------------------------------------------------------------------
        // Main Function
        //-----------------------------------------------------------------------------
      
        private List<string> Slice(bool Apply_Changes = true)
        {      
            // int Query = 0;
            Selected_Xml = "";
            string Xml_Directory = Properties.Settings.Default.Xml_Directory;
            string Entity_Name = Wash_String(Combo_Box_Entity_Name.Text);
            string Selected_Tag = Regex.Replace(Combo_Box_Tag_Name.Text, "[\n\r\t </>]", ""); // Also removing </> tag values
            string Selected_Type = Wash_String(Combo_Box_Type_Filter.Text);

          
            string Backup_File = "";
            // Backup_Folder was set to "Mod_Name" in Button_Run_Click()
            string Path_To_Backup = Backup_Path + Backup_Folder;



            // XElement Selected_Instance = null;
            IEnumerable<XElement> Found_Instances = null;
            List <string> Changed_Xmls = new List<string>();

                   
            if (!Directory.Exists(Xml_Directory)) { iConsole(200, 100, "\nCan't find the Xml Directory."); return null; }
        

            if (Apply_Changes) // File_Collection is a global variable, feeded from the remaining filenames in the Caution_Window 
            {   
                if (File_Collection == null | File_Collection.Count == 0) { File_Collection = Get_All_Files(Xml_Directory, "xml"); } // Failsafe
                // iConsole(560, 600, Xml_Directory + string.Join("\n", File_Collection)); // return null; // Debug Code
       

                // Using Mod_Name instead of Backup_Folder here, because we're explicitely targeting the working directory.
                if (!Directory.Exists(Path_To_Backup)) { Directory.CreateDirectory(Path_To_Backup); }

                if (Get_All_Directories(Path_To_Backup, true).Count() == 0) // Then we need to create a innitial "Base" backup
                {
                    // Just copy all files as innitial backup, we are going to need them later on to compare filesizes against this backup!
                    Copy_Now(Sync_Path, Path_To_Backup + @"\" + Package_Name + @"_Base\");

                    Temporal_B = "\nCreated a backup of the whole directory into  \n" +
                      Backup_Path + "\n" + Backup_Folder + @"\" + Package_Name + @"_Base" +
                        "\n\n  We are going to need it as \"Base\" later on, to \ncompare filesizes against this innitial backup.\n";
                
                
                    iConsole(400, 260, Temporal_B); // Tell it to the user.


                    // Base version to true, otherwise it will complain about a missmatched path
                    Create_Backup_Info(Mod_Name, Package_Name + @"_Base", "", "\n" + Temporal_B.Replace("\n", ""));
                    Refresh_Backup_Stack();
                    At_Top_Level = false; // Correcting

                    File_Collection.Clear();
                    Temporal_B = "";
                    return null;
                }                

            } else { File_Collection = Get_All_Files(Xml_Directory, "xml"); }

        

            foreach (var Xml in File_Collection)
            {   
                try {


                    Selected_Xml = Xml;
                    // Ignoring blacklisted Xmls
                    if (Blacklisted_Xmls != null) { if (Blacklisted_Xmls.Contains(Selected_Xml.Replace(Xml_Directory, ""))) { continue; } }

                    XDocument Xml_File = XDocument.Load(Selected_Xml, LoadOptions.PreserveWhitespace);



                    if (In_Selected_Xml(Entity_Name)) // Select Multiple by Name
                    {                     
                        Selected_Xml = Properties.Settings.Default.Last_File; // Overwride what ever xml this loop selected before
                        Xml_File = XDocument.Load(Selected_Xml, LoadOptions.PreserveWhitespace);
                        List<string> Selected_Entities = Select_List_View_Items(List_View_Selection);


                        if (Match_Without_Emptyspace(Queried_Attribute, "First_Attribute"))
                        {
                            Found_Instances =
                               from All_Tags in Xml_File.Root.Descendants()
                               where List_Matches(Selected_Entities, (string)All_Tags.FirstAttribute)
                               select All_Tags;
                        }
                        else
                        {
                            Found_Instances =
                              from All_Tags in Xml_File.Root.Descendants()
                              where List_Matches(Selected_Entities, (string)All_Tags.Attribute(Queried_Attribute))
                              select All_Tags;
                        }
                    }
                    else { Found_Instances = Get_Entities(Xml_File, Entity_Name, Selected_Tag, Selected_Type); }


            

                    if (Found_Instances.Count() == 0) { continue; }
                    // iConsole(500, 500, Selected_Xml + "\n\n" + string.Join("\n", Found_Instances));
                  

                    // =================== Checking Xml Instance ===================
                    foreach (XElement Instance in Found_Instances)
                    {
                       
                        if (Selected_Tag == "Enable_Abilities" | Selected_Tag == "Enable_Passive_Abilities")
                        {
                            string The_Value = "";
                            string Ability_Tag_Name = "Unit_Abilities_Data";
                            Temporal_A = Selected_Xml.Replace(Xml_Directory, ""); 
                                

                            if (Selected_Tag == "Enable_Abilities" && Instance.Descendants("Unit_Abilities_Data").Any())
                            {   The_Value = Instance.Descendants(Ability_Tag_Name).First().FirstAttribute.Value; // Always first attribute
                                // string The_Name = Instance.Attribute("Name").Value;
                                // iConsole(400, 100, The_Name + ", " + The_Value); // return Changed_Xmls;

                                if (!Changed_Xmls.Contains(Temporal_A)) { Changed_Xmls.Add(Temporal_A); }
                            }
                            else if (Selected_Tag == "Enable_Passive_Abilities" && Instance.Descendants("Abilities").Any())
                            {
                                Ability_Tag_Name = "Abilities";
                                The_Value = Instance.Descendants(Ability_Tag_Name).First().FirstAttribute.Value;

                                // this here makes the Set_Tag() function obsolete for this case:
                                if (!Changed_Xmls.Contains(Temporal_A)) { Changed_Xmls.Add(Temporal_A); }
                            }


                            if (Get_Text_Box_Bool() == "True" && The_Value == "No")
                            { Instance.Descendants(Ability_Tag_Name).First().FirstAttribute.Value = "Yes"; }

                            else if (Get_Text_Box_Bool() == "False" && The_Value == "Yes")
                            { Instance.Descendants(Ability_Tag_Name).First().FirstAttribute.Value = "No"; }
                        }

                      
                        
                    
                        else if (Instance.Descendants().Any())
                        {
                            string The_Tag = "";

                            if (Combo_Box_Entity_Name.Text == "Find_And_Replace")
                            {
                                The_Tag = Instance.Descendants().First(x => x.Value == Combo_Box_Tag_Name.Text).Name.ToString();
                                Set_Tag(Changed_Xmls, Instance, Selected_Xml, The_Tag, Apply_Changes);
                              

                                // iConsole(400, 100, Instance.Descendants().First(x => x.Value == Combo_Box_Tag_Name.Text).Name.ToString());
                            }
                            else if (Combo_Box_Tag_Name.Text == "Minor_Heroes_To_Major")
                            {   // Changing Selected_Tag from  "Minor_Heroes_To_Major" to "Show_Hero_Head" here, to match the right units!
                                Set_Tag(Changed_Xmls, Instance, Selected_Xml, "Show_Hero_Head", Apply_Changes);
                            }
                            else if (Combo_Box_Tag_Name.Text == "Major_Heroes_To_Minor")
                            { Set_Tag(Changed_Xmls, Instance, Selected_Xml, "Is_Named_Hero", Apply_Changes); }                      

                            else if (Combo_Box_Tag_Name.Text == "Scale_Galaxies")
                            {
                                if (Instance.Name == "Planet")  // Scale Factor is only supposed to affect Planets in this context
                                { Set_Tag(Changed_Xmls, Instance, Selected_Xml, "Scale_Factor", Apply_Changes); }

                                Set_Tag(Changed_Xmls, Instance, Selected_Xml, "Galactic_Position", Apply_Changes);
                            }
                            else if (Combo_Box_Tag_Name.Text == "All_Damage")
                            {
                                Set_Tag(Changed_Xmls, Instance, Selected_Xml, "Damage", Apply_Changes);
                                Set_Tag(Changed_Xmls, Instance, Selected_Xml, "Projectile_Damage", Apply_Changes);
                            }




                            else if (Combo_Box_Tag_Name.Text != "Rebalance_Everything") // This one would usually trigger
                            {
                                Set_Tag(Changed_Xmls, Instance, Selected_Xml, Selected_Tag, Apply_Changes);
                            }
                            else if (Balancing_Tags != null)
                            {   // string[] Balancing_Tags = new string[] { "Tactical_Health", "Shield_Points", "Shield_Refresh_Rate"};
                                foreach (string Current_Tag in Balancing_Tags) // Balancing_Tags is a global variable that is loaded from the Tags setting.
                                {
                                    Set_Tag(Changed_Xmls, Instance, Selected_Xml, Current_Tag, Apply_Changes);
                                }
                            }


                        }
                    }

                   

                    

                    if (Apply_Changes)
                    {
                        Backup_File = Selected_Backup_Path + Package_Name + @"_Auto\" + (Selected_Xml.Replace(Sync_Path, "")); // Creating Sub-Directories:                        
                        if (!Directory.Exists(Path.GetDirectoryName(Backup_File))) { Directory.CreateDirectory(Path.GetDirectoryName(Backup_File)); }

                        File.Copy(Selected_Xml, Backup_File, true);  // Creating a Backup                           
                        
                        Xml_File.Save(Selected_Xml);
                        // iConsole(500, 100, "\nSaving to " + Selected_Xml);                     
                    }

                    if (In_Selected_Xml(Entity_Name)) { return Changed_Xmls; } // Exiting after the first (and only) Xml File.
      
                } catch {}
            }


            // !Apply_Changes because it shall trigger once only
            // if (!Apply_Changes) { iConsole(300, 100, Query + " Is the Case of Query"); } 


            File_Collection = new List<string>(); // Clearing for the next time          
            return Changed_Xmls;      
        }

    
        //===========================//
        // I need to pass in quite a lot of parameters here ...
        public void Set_Tag(List<string> Changed_Xmls, XElement Instance, string Selected_Xml, string Selected_Tag, bool Apply_Changes)
        {   int Changed_Entities = 0;
   

            try
            {                   
                // Selected_Instance = Instance;                         
                if (Instance.Descendants(Selected_Tag).Any()) // Set the new tag value(s)
                {
                    Temporal_A = Selected_Xml.Replace(Xml_Directory, ""); // Removing Path
                    // This needs to run outside of Apply_Changes, because its supposed to collect candidate files before any changes are applied in the 2nd run of this function
                    if (!Changed_Xmls.Contains(Temporal_A)) { Changed_Xmls.Add(Temporal_A); }


                    if (Apply_Changes)
                    {
                        string New_Value = Combo_Box_Tag_Value.Text.Replace("+", "");
                        
                      
                        // Deliberately NOT comparing to Selected_Tag here, because we use "Show_Hero_Head" as Selected_Tag, to match the targeted entities in first line.
                        if (Combo_Box_Tag_Name.Text == "Minor_Heroes_To_Major") 
                        {          
                            string Head_Value = Instance.Descendants("Show_Hero_Head").Last().Value;
                            
                            if (!Instance.Descendants("Show_Hero_Head").Any()) { return; } // This only makes sense for units that have this tag, and set to true   
                            else if (!Match_Without_Emptyspace(Head_Value, "True") & !Match_Without_Emptyspace(Head_Value, "Yes")) { return; }
                            
                            else if (Instance.Descendants("Is_Named_Hero").Any()) { Instance.Descendants("Is_Named_Hero").Last().Value = "Yes"; }
                            else { Instance.Descendants("Show_Hero_Head").Last().AddAfterSelf("\n\t", new XElement("Is_Named_Hero", "Yes")); }
                           
                            return; // Exiting here because this tag has its own way to be handled
                        }
                        else if (Combo_Box_Tag_Name.Text == "Major_Heroes_To_Minor") 
                        {          
                            string Named_Hero = Instance.Descendants("Is_Named_Hero").Last().Value;

                            if (!Instance.Descendants("Is_Named_Hero").Any()) { return; } // This only makes sense for units that have this tag, and set to true   
                            else if (!Match_Without_Emptyspace(Named_Hero, "True") & !Match_Without_Emptyspace(Named_Hero, "Yes")) { return; }

                            else if (Instance.Descendants("Show_Hero_Head").Any()) { Instance.Descendants("Show_Hero_Head").Last().Value = "Yes"; }
                            else { Instance.Descendants("Is_Named_Hero").Last().AddAfterSelf("\n\t", new XElement("Show_Hero_Head", "Yes")); }

                            Instance.Descendants("Is_Named_Hero").Last().Value = "No"; // Degrading him to Minor Hero
                           
                            return; 
                        }


                        else if (Selected_Tag == "Planet_Surface_Accessible")
                        {
                            if (Match_Without_Emptyspace(New_Value, "True") | Match_Without_Emptyspace(New_Value, "Yes"))
                            {   if (!Instance.Descendants("Land_Tactical_Map").Any() | !Is_Match(Instance.Descendants("Land_Tactical_Map").First().Value, ".ted"))
                                { return; } // Failsafe, we must not apply "true" to planets that have no ground maps - or the game will crash!
                            }
                        }
                                    

               

                        Changed_Entities++;
                        foreach (XElement Target in Instance.Descendants(Selected_Tag))
                        {
                            if (Operation_Mode == "Percent")
                            {   string Full_Value = "";


                                if (Selected_Tag == "Galactic_Position") // Would usually run in "Scale_Galaxies" mode
                                {
                                    string[] Position = Wash_String(Target.Value).Split(',');

                                    for (int i = 0; i < Position.Count(); i++)
                                    {                                                                 
                                        if (i == 0 & Scale_Mode.Contains("X")) // XY and X
                                        { Full_Value += Process_Percentage(Position[i]) + ", "; }

                                        else if (i == 1 & Scale_Mode.Contains("Y")) // XY and Y
                                        { Full_Value += Process_Percentage(Position[i]) + ", "; }

                                        else 
                                        {   Full_Value += Position[i];
                                            if (i < Position.Count() - 1) { Full_Value += ", "; }
                                        } // Full_Value += Process_Percentage(Position[i]); break; } // Disable any scaling of 3rd entry (Z layer)
                                    }
                                    Target.Value = Full_Value;

                                }
                                else if (Selected_Tag == "Scale_Factor")
                                {
                                    int Weaker = 1; // In Galaxy scale the scaling is weakened, in order to have this value weaker then the full 100%
                                    if (Combo_Box_Tag_Name.Text == "Scale_Galaxies") 
                                    {
                                        if (Scale_Mode == "XY") // Don't scale for only X or only Y axis!
                                        {   Weaker = 8; // 3 means 33% of the amount it would usually scale.
                                            Target.Value = Process_Percentage(Target.Value, Weaker); 
                                        } 
                                       
                                    } else { Target.Value = Process_Percentage(Target.Value, Weaker); }                                                           
                                }

                                else if (Selected_Tag == "Max_Speed")
                                {
                                    Target.Value = Process_Percentage(Target.Value);

                                    if (Instance.Descendants("Min_Speed").Any()) // Min_Speed is bundled to Max_Speed
                                    {
                                        string Min_Speed = Instance.Descendants("Min_Speed").Last().Value;
                                        Instance.Descendants("Min_Speed").Last().Value = Process_Percentage(Min_Speed);
                                    }
                                }


                                // Needs different treatment because its value is seperated by emptyspace
                                else if (Selected_Tag == "Radar_Icon_Size")
                                {
                                    foreach (string Factor in Target.Value.Split(' '))
                                    {
                                        float Test = 0;

                                        if (float.TryParse(Factor.Replace(".", ","), out Test)) // Then it contains a number and can be processed
                                        {
                                            // Only after first entry, add two emptyspace as new seperators
                                            if (Full_Value == "") { Full_Value += Process_Percentage(Factor) + "  "; }
                                            else { Full_Value += Process_Percentage(Factor); break; }
                                        }
                                    }
                                 
                                    Target.Value = Full_Value;
                                }

                                else { Target.Value = Process_Percentage(Target.Value); }
                            }



                            else
                            {   if (Combo_Box_Tag_Value.Text.Contains("%")) { Combo_Box_Tag_Value.Text = Combo_Box_Tag_Value.Text.Replace("%", ""); } // Removing Mistakes

                                string Result = Combo_Box_Tag_Value.Text.Replace("+", "");


                                if (Operation_Mode.Contains("Point")) // Don't chain this to the statement above.
                                {
                                    string[] Input = Wash_String(Result).Split('|');
                                    int Value_1 = 0;
                                    int Value_2 = 0;

                                    Int32.TryParse(Input[0], out Value_1);
                                    Int32.TryParse(Input[1], out Value_2);
                                    // Random Randomize = new Random();
                                    
                                    try
                                    {
                                        if (Operation_Mode == "Point" && Combo_Box_Entity_Name.Text == "Insert_Random_Int") 
                                        {
                                            // Result = (Randomize.Next(Value_1, Value_2)).ToString();
                                            Result = Random_Int(Value_1, Value_2).ToString();
                                        }

                                        else if (Operation_Mode == "Point_Float") // Don't chain this to the statement above.
                                        {                                          
                                            if (Selected_Tag == "Radar_Icon_Size") 
                                            { if (Result.Contains("|")) { Result = Result.Replace("|", ""); } } // Just set value to what the user sees.

                                            else if (Combo_Box_Entity_Name.Text == "Insert_Random_Float")
                                            {
                                                float Value_3 = 0;
                                                float Value_4 = 0;

                                                float.TryParse(Input[0], out Value_3);
                                                float.TryParse(Input[1], out Value_4);

                                                //string Before_Point = Randomize.Next(Value_1, Value_2).ToString();//number before decimal point
                                                //string After_Point_1 = Randomize.Next(Value_1, Value_2).ToString();//1st decimal point
                                                //string After_Point_2 = Randomize.Next(Value_1, Value_2).ToString();//2nd decimal point

                                                // string Before_Point = Random_Int((int)Value_3 * 10, (int)Value_4 * 10).ToString();//number before decimal point
                                                //string After_Point_1 = Random_Int((int)Value_3 * 10, (int)Value_4 * 10).ToString();//1st decimal point
                                                //string Combined = Before_Point + "." + After_Point_1; // + After_Point_2;


                                                // *10 to get rid of decimal nr
                                                Value_4 = Random_Int((int)Value_3 * 10, (int)Value_4 * 10);

                                                // /100 to turn it back into float with "n1" being 1 slot after decimal point
                                                Result = (Value_4 / 100).ToString("n1").Replace(",", ".");
                                                //Result = float.Parse(Before_Point).ToString();                                                
                                            }
                                        }
                                    } catch {}
                                }

                                Target.Value = Result;
                            }
                     
                           
                            if (!Check_Box_All_Occurances.Checked) { return; } // Stop after first occurance
                        }
                    }
                }
            } catch {}         
        }

        //===========================//

        public string Process_Percentage(string The_Value, int Weakening = 1)
        {   
            try // Refactoring the old value!
            {   
                int Percentage = 0;
                float Original_Value = 0;

                // Replace(".", ",") because otherwise it does not interpret . as comma 
                float.TryParse(The_Value.Replace(".", ","), out Original_Value); 
                Int32.TryParse(Remove_Operators(Combo_Box_Tag_Value.Text), out Percentage);

                if (Combo_Box_Tag_Value.Text.Contains("-")) // Shrink Value
                { The_Value = (Original_Value - ((Original_Value / 100) * (float)(Percentage / Weakening))).ToString(); }
                    

                else // if (Combo_Box_Tag_Value.Text.Contains("+")) // Grow Value
                { The_Value = (Original_Value + ((Original_Value / 100) * (float)(Percentage / Weakening))).ToString(); }

                // iConsole(400, 100, Original_Value + " +- (" + (Original_Value / 100) + " / " + (float)(Percentage / Weakening) + ") = " + The_Value);

            } catch {}

            // The , might be confused by EAW with table values so . fitts better
            return The_Value.Replace(",", "."); 
        }


        //===========================//

        // The random number provider.
        private System.Security.Cryptography.RNGCryptoServiceProvider Rand =
            new System.Security.Cryptography.RNGCryptoServiceProvider();

        // Return a random integer between a min and max value.
        private int Random_Int(int min, int max)
        {
            uint scale = uint.MaxValue;
            while (scale == uint.MaxValue)
            {
                // Get four random bytes.
                byte[] four_bytes = new byte[4];
                Rand.GetBytes(four_bytes);

                // Convert that into an uint.
                scale = BitConverter.ToUInt32(four_bytes, 0);
            }

            // Add min to the scaled difference between max and min.
            return (int)(min + ((max + 1) - min) *
                (scale / (double)uint.MaxValue));
        }

        //===========================//
        private float Random_Float(float min, float max)
        {
            float result = 0;
            uint scale = uint.MaxValue;
            while (scale == uint.MaxValue)
            {
                // Get four random bytes.
                byte[] four_bytes = new byte[4];
                Rand.GetBytes(four_bytes);

                // Convert that into an uint.
                scale = BitConverter.ToUInt32(four_bytes, 0);
                result = BitConverter.ToSingle(four_bytes, 0);
            }

            // Add min to the scaled difference between max and min.
            return (float)(min + ((max + 1) - min) *
                (result / (float)uint.MaxValue));
        }



        //===========================//

        public IEnumerable<XElement> Get_Entities(XDocument Xml_File, string Entity_Name, string Selected_Tag, string Selected_Type)
        {
            IEnumerable<XElement> Instances = null;
            int Query = 0;

            // ===================== Opening Xml File =====================                  


            if (Combo_Box_Type_Filter.Text == "Faction Name Filter")
            {
                Query = 2;

                Instances =
                   from All_Tags in Xml_File.Root.Descendants() // Entity_Name means the Faction name here
                   where All_Tags.Descendants("Affiliation").Any() // We need this to prevent null exceptions
                   where All_Tags.Descendants("Affiliation").Last().Value.Contains(Entity_Name)
                   select All_Tags; // Last() because it overwrites the first occurances ingame
            }

            else if (Combo_Box_Type_Filter.Text == "Category Mask Filter")
            {
                Query = 3;

                Instances =
                   from All_Tags in Xml_File.Root.Descendants() // Entity_Name means the Faction name here
                   where All_Tags.Descendants("CategoryMask").Any() // We need this to prevent null exceptions
                   where All_Tags.Descendants("CategoryMask").Last().Value.Contains(Entity_Name)
                   select All_Tags; // Last() because it overwrites the first occurances ingame
            }

            else if (!Array_Matches(Ignored_Attribute_Values, Combo_Box_Entity_Name.Text)) // Select a single Entity by Name
            {
                Query = 4;


                if (Match_Without_Emptyspace(Queried_Attribute, "First_Attribute"))
                {
                    Instances =
                      from All_Tags in Xml_File.Root.Descendants()
                      where (string)All_Tags.FirstAttribute == Entity_Name
                      select All_Tags;
                }
                else
                {
                    Instances =
                      from All_Tags in Xml_File.Root.Descendants()
                      where (string)All_Tags.Attribute(Queried_Attribute) == Entity_Name
                      select All_Tags;
                }
            }
            else if (Combo_Box_Entity_Name.Text == "Find_And_Replace")
            {
                Query = 5;

                Instances =
                  from All_Tags in Xml_File.Root.Descendants()
                  where All_Tags.Descendants().Any(x => x.Value == Combo_Box_Tag_Name.Text)
                  select All_Tags;
                // if (Instances.Count() > 0) { iConsole(400, 100, (string)Instances.First().FirstAttribute); }                   
            }

            // Match by Tag Value (= fake entity type)
            else if (Selected_Type == "FighterUnit") // If Fighter Locomotor can be found we spoof the fake tagname "FighterUnit"
            {
                Query = 6;

                Instances =
                  from All_Tags in Xml_File.Root.Descendants()
                  where All_Tags.Descendants("SpaceBehavior").Any() // We need this to prevent null exceptions! And it increases speed a lot.
                  where All_Tags.Descendants("SpaceBehavior").Last().Value.Contains("FIGHTER_LOCOMOTOR")
                  select All_Tags;

            }

            else if (Selected_Type != "" & Combo_Box_Type_Filter.Text != "All Types") // By Entity Type
            {
                Query = 7;

                Instances =
                  from All_Tags in Xml_File.Root.Descendants()
                  where All_Tags.Name == Selected_Type
                  select All_Tags;
            }

            else if (Selected_Tag == "Max_Speed")
            {
                Query = 8;

                Instances =
                   from All_Tags in Xml_File.Root.Descendants()
                   where All_Tags.Descendants(Selected_Tag).Any()
                       // Means they are excluded, unless they are EXPLICITLY specified as Selected_Type!                      
                   & (All_Tags.Name != "Particle" | Selected_Type == "Particle")
                   & (All_Tags.Name != "Projectile" | Selected_Type == "Projectile")

                   select All_Tags;
            }

            else if (Selected_Tag == "Scale_Factor")
            {
                Query = 9;

                /*
                Only this Scale_Factor_List matches to "All Types", because otherwise the change of Scale_Factor would affect all of these types:
                Planet, Projectile, Particle
                Prop, SpaceProp, Container, Marker, MiscObject, SpecialEffect, MOV_Cinematic,  
                SpacePrimarySkydome, SpaceSecondarySkydome, LandPrimarySkydome, LandSecondarySkydome, 

                StarBase, SpaceBuildable, SpecialStructure, SecondaryStructure, MultiplayerStructureMarker, 
                GroundBase, GroundStructure, GroundBuildable, 
                */

                List<string> Scale_Factor_List = new List<string>() { "SpaceUnit", "UniqueUnit", "StarBase" };
                // Scale_Factor_List.Add(Combo_Box_Type_Filter.Text); // This little gimmick because the OR operator in Linq is horrible... 


                Instances =
                   from All_Tags in Xml_File.Root.Descendants()
                   where List_Matches(Scale_Factor_List, All_Tags.Name.ToString())
                   where All_Tags.Descendants(Selected_Tag).Any()

                   select All_Tags;
            }

            else if (Selected_Tag == "Enable_Abilities" | Selected_Tag == "Enable_Passive_Abilities")
            {
                string Required_Tag = "Unit_Abilities_Data";
                if (Selected_Tag == "Enable_Passive_Abilities") { Required_Tag = "Abilities"; }

                Query = 10;

                Instances =
                  from All_Tags in Xml_File.Root.Descendants()
                  where All_Tags.Descendants(Required_Tag).Any()
                  select All_Tags;
            }
            else // Target all entities in the whole Mod!
            {
                Query = 11;

                Instances =
                  from All_Tags in Xml_File.Root.Descendants()
                  // Selecting all non empty tags that have the Queried_Attribute "Name", null because we need all selected.
                  where All_Tags != null
                  select All_Tags;
            }

            return Instances;
        }


        //===========================//

        public string Wash_String(string The_String)
        { return Regex.Replace(The_String, "[\n\r\t ]", ""); }

        //===========================//

        public string Remove_Emptyspace_Prefix(string The_String)
        {   try
            {   // Removing empty space
                while (The_String[0] == ' ') { The_String = The_String.Substring(1, The_String.Length - 1); }
            } catch {}          

            return The_String;
        }

        //===========================//
        public bool In_Selected_Xml(string Entry) 
        {   if (Entry == "Multi") { return true; }
            else if (List_View_Matches(List_View_Selection, Entry)) { return true; }
            return false;
        }
      
        //===========================//
        private void Button_Toggle_Settings_Click(object sender, EventArgs e)
        {            
            if (Text_Box_Tags.Visible == true)
            {
                if (Combo_Box_Type_Filter.Text != "Faction Name Filter" && Combo_Box_Type_Filter.Text != "Category Mask Filter") 
                { Button_Search.Visible = true; }
                Text_Box_Tags.Visible = false;

                Clear_Last_Mode();
                Set_UI_Into_Settings_Mode(false); // Needs to run after Clear_Last_Mode();    
 
                Set_Resource_Button(Button_Reset_Blacklist, Properties.Resources.Button_Controller);
          
    
                if (Tag_List != Text_Box_Tags.Text) // Then needs to update
                {   Properties.Settings.Default.Tags = Text_Box_Tags.Text;
                    Properties.Settings.Default.Save();
                    Tag_List = Text_Box_Tags.Text;
                    // Refreshing Values:
                    Text_Format_Delimiter = Get_Setting_Value("Text_Format_Delimiter");

                    if (Text_Format_Delimiter == "\\t" | Text_Format_Delimiter == "t") { Text_Format_Delimiter = "\"\t\""; } // Correction Override


                    string New_Program_Dir = Get_Program_Dir(true);

                    if (Program_Directory != New_Program_Dir)
                    {  
                        Program_Directory = New_Program_Dir; // Update                     
                        New_Program_Dir = Path.GetDirectoryName(Program_Directory);

                        try
                        {   // iConsole(400, 200, "Moving " + Script_Directory + "\n to  " + New_Program_Dir);
                            if (!Directory.Exists(New_Program_Dir)) { Directory.CreateDirectory(New_Program_Dir); }

                            Moving(Path.GetDirectoryName(Backup_Path), New_Program_Dir);
                            Moving(Path.GetDirectoryName(Script_Directory), New_Program_Dir);

                            Backup_Path = Program_Directory + @"Backup\";
                            Script_Directory = Program_Directory + @"Scripts\"; // Needs to run AFTER old Script_Directory was moved
                        
                        } catch { iConsole(500, 200, "\nFailed to move " + Script_Directory + "\n to  " + New_Program_Dir); }
                    }
                }    
                   
                Reset_Tag_Box();
                Reset_Root_Tag_Box();

                Toggle_Undo_Button(Enable_Undo); // Show it if it was visible before
             

                if (Match_Setting("Disable_EAW_Mode"))
                {
                    Label_Entity_Name.Text = Queried_Attribute; 

                    EAW_Mode = false;                   
                    Label_Type_Filter.Text = "Parent Name";
                }


             
                else if (Combo_Box_Type_Filter.Text == "Category Mask Filter")
                {   
                    EAW_Mode = true;
                    Label_Entity_Name.Text = "Category Mask";
                }

                // This Check needs to run AFTER Reset_Tag_Box();
                else if (Combo_Box_Type_Filter.Text == "Faction Name Filter")
                {
                    EAW_Mode = true;                 
                    Label_Entity_Name.Text = "Faction Name";
                } 
                else if (Combo_Box_Entity_Name.Text == "Find_And_Replace")
                {
                    EAW_Mode = true;
                    Label_Entity_Name.Text = "Old Tag Value";
                    Label_Type_Filter.Text = "Filter Type";
                }
                else // EAW Mode
                {
                    Label_Entity_Name.Text = Queried_Attribute; 

                    EAW_Mode = true;
                    Label_Type_Filter.Text = "Filter Type";
                }
            }
            else if (List_View_Selection.Size.Height < 200) 
            {
                Text_Box_Tags.Visible = true;
                Set_UI_Into_Settings_Mode(true);                                     
                Set_Resource_Button(Button_Reset_Blacklist, Properties.Resources.Button_Refresh);
                Label_Entity_Name.Text = "List of Tags";            
                Text_Box_Tags.Focus(); // So the user can scroll


                if (Match_Setting("Show_Tooltip"))
                {
                    Text_Box_Description.Visible = true;

                    // Special Tooltip, that describes the Settings                   
                    Text_Box_Description.Text = "Syntax: \nType Tag_Name = Type # Comment\n" + 
                       "The expected variable type can be:\nbool, string or int\n" +
                       "You can also set any number after = sign as scale factor for the scrollbar.";
                 }

                Toggle_Undo_Button(); // Hide it

                return;
            }
        }

  
        private void Button_Toggle_Settings_MouseHover(object sender, EventArgs e)
        { Set_Resource_Button(Button_Toggle_Settings, Properties.Resources.Button_Settings_Lit); }

        private void Button_Toggle_Settings_MouseLeave(object sender, EventArgs e)
        {
            if (Text_Box_Tags.Visible) // Use its visability as bool toggle ;)
            { Set_Resource_Button(Button_Toggle_Settings, Properties.Resources.Button_Settings_Lit); }
            else { Set_Resource_Button(Button_Toggle_Settings, Properties.Resources.Button_Settings); }    
        }




        //===========================//

        private void Button_Undo_Click(object sender, EventArgs e)
        {
            // Backup_Folder was set to "Mod_Name" in Button_Run_Click()
            List<string> Found_Backups = Get_All_Directories(Backup_Path + Backup_Folder, true);               
            Found_Backups.Reverse();
     
            // iConsole(500, 500, Found_Backups[0]);  // Get the first one    
            Restore(Current_Backup + User_Name, Found_Backups[0], false, true, true, false);
            Deleting(Found_Backups[0]); // Getting rid of it after Undo


            Enable_Undo = false; 
            Toggle_Undo_Button(false);
        }

        private void Button_Undo_MouseHover(object sender, EventArgs e)
        { Set_Resource_Button(Button_Undo, Properties.Resources.Button_Refresh_Lit); }

        private void Button_Undo_MouseLeave(object sender, EventArgs e)
        { Set_Resource_Button(Button_Undo, Properties.Resources.Button_Refresh); }



        private void Toggle_Undo_Button(bool Show_It = false)
        {   if (Show_It)
            {   // Combo_Box_Tag_Value.Size = new Size(331, 26);
                Button_Undo.Visible = true;
            }
            else 
            {   // Combo_Box_Tag_Value.Size = new Size(367, 26); // Back to normal size
                Button_Undo.Visible = false;
            }        
        }
       


        //===========================//
        private void Button_Reset_Blacklist_Click(object sender, EventArgs e)
        {
            if (Text_Box_Tags.Visible)
            {
                iDialogue(540, 200, "Do It", "Cancel", "false", "false", "\nAre you sure you wish reset the List of Tags?");
                if (Caution_Window.Passed_Value_A.Text_Data == "false") { return; }

                Reset_Tag_List();
                Reset_Tag_Box();
                Reset_Root_Tag_Box();
                Text_Box_Tags.Text = Tag_List;
                if (Text_Box_Tags.Visible == false) { Text_Box_Tags.Visible = true; }
            }
            else
            {
                bool Affinity = false;
                bool Priority = false;
                bool Has_Custom_Path = false;

                if (Match_Setting("Set_Launch_Affinity")) { Affinity = true; }
                if (Match_Setting("High_Launch_Priority")) { Priority = true; }
                if (Verify_Setting("Custom_Program_Path")) { Has_Custom_Path = true; }


                string Custom_Parameter_Line = Get_Setting_Value("Custom_Start_Parameters", true);
                string Custom_Parameters = "";
                try { Custom_Parameters = Custom_Parameter_Line.Split('=')[1]; } catch {}
                    

                if (!EAW_Mode && !Has_Custom_Path)
                {
                    iConsole(400, 190, "\nPlease write any Filepath into \nSettings\\Custom_Program_Path \nand its Arguments to Custom_Start_Parameters" +
                    "\nIn order to launch them by this button."
                    ); return;
                }



                string Steam_Path = Properties.Settings.Default.Steam_Exe_Path;
                string Program_Path = "";
                string Arguments = "";


                if (Has_Custom_Path) // Custom Override
                {   Program_Path = Get_Setting_Value("Custom_Program_Path");
                    if (EAW_Mode && !Program_Path.EndsWith(".exe")) { Program_Path += ".exe"; } // Append suffix
                } 
                else if (EAW_Mode) { Program_Path = Path.GetDirectoryName(Steam_Path) + @"\steamapps\common\Star Wars Empire at War\corruption\StarWarsG.exe"; }
              

                


                // if (Custom_Parameters != "" & !Match_Without_Emptyspace(Properties.Settings.Default.Tags, "Custom_Start_Parameters = false"))
                if (Verify_Setting("Custom_Start_Parameters"))
                {  
                    if (!EAW_Mode) { Arguments = Custom_Parameters; }  

                    else // EAW Mode, Inserting "STEAMMOD" if they are workshop items
                    {   bool Is_Steam_Workshop_Id = false;
                        string[] Command = Custom_Parameters.Split(' ');
                        
                        for (int i = 0; i < Command.Count(); i++)
                        {
                            if (Command[i] == " " || Command[i] == "") { continue; }

                            else if (Is_Steam_Workshop_Id || Command[i].All(char.IsDigit) && Command[i].Count() > 9)
                            {
                                Is_Steam_Workshop_Id = true;
                                Arguments += "STEAMMOD=" + Command[i] + " ";
                            }

                            else if (Command[i].ToLower() == "modpath" || Command[i].ToLower() == "steammod")
                            {   try // Then we need to insert the Modname, that follows the = sign which was split for Custom_Parameters above 
                                {   Temporal_A = "";
                                    Temporal_A = Remove_Emptyspace_Prefix(Custom_Parameter_Line.Split('=')[2]); // its probably [2]
                          
                                    Arguments += Command[i] + "=" + Temporal_A;
                                    // iConsole(400, 100, "\"" + Temporal_A + "\"");
                                } catch {}
                            }

                            else { Arguments += Command[i] + " "; }                 
                        }

                        // Prepending
                        if (!Arguments.ToLower().StartsWith("modpath") && !Arguments.ToLower().StartsWith("steammod")) { Arguments = @"Modpath=Mods\" + Arguments; }
                    }                                                                              
                }

                else if (EAW_Mode) // Otherwised this button is used to launch the game
                {                                 
                    Arguments = @"Modpath=Mods\" + Mod_Name + " Ignoreasserts"; // Auto complete Arguments from Modname

                    // Overwride by User Setting
                    //if (Verify_Setting("Custom_Modpath"))
                    //{   Arguments = "Modpath=" + Get_Setting_Value("Custom_Modpath") + " Ignoreasserts";
                    //    //  Arguments = "Modpath=" + Mod_Directory.Replace(" ", "\\ ") + " Ignoreasserts";
                    //} else

                    if (Mod_Name.All(char.IsDigit)) // If all characters are numbers, that means we target a Workshop mod
                    {   // So argument 1 targets now the Workshop dir            
                        // Arguments = @"Modpath=..\..\..\workshop\content\32470\" + Mod_Name; 
                        Arguments = @"Steammod=" + Mod_Name + " Ignoreasserts";
                    }

                    Check_Process(Steam_Path); // Now, that we made sure Steam is running, we can launch the game 
                }


                iConsole(400, 100, "\"" + Program_Path + " " + Arguments + "\""); // Debug
                // Execute(Command[0], Arguments); // This version would throw no error if the process can't be found                                                                                      
                // Check_Process(Program_Path, Arguments, Affinity, Priority);              
            }      
        }



        public bool Check_Process(string Program_Path, string Arguments = "", bool Set_Affinity = false, bool High_Priority = false)
        {
            string Program_Name = "";
            try { Path.GetFileNameWithoutExtension(Program_Path); } catch {}
            Process[] The_Process = Process.GetProcessesByName(Program_Name);

            // Make sure there is an instance of the app is running
            if (The_Process.Length == 0)
            {   try
                {
                    ProcessStartInfo Process_Info = new ProcessStartInfo();
                    Process_Info.FileName = Program_Path;
                    Process_Info.Arguments = Arguments;

                    // iConsole(60, 100, Program_Path + " " + Arguments); // return true;
                    //The_Process[0] = Process.Start(Process_Info);
                    Process.Start(Process_Info);

                    The_Process = Process.GetProcessesByName(Program_Name); // Retrieve the app processes. 
                } catch { iConsole(60, 100, "\nFailed to find and launch " + Program_Name); return false; }
            }



            try 
            {   if (Set_Affinity | High_Priority) //Process[] The_Processes; 
                {   // Get the ProcessThread collection for the first instance
                    ProcessThreadCollection The_Threads = The_Process[0].Threads;

                    if (Set_Affinity)
                    {
                        int Logical_Processors = Environment.ProcessorCount;

                        // Set the properties on the first ProcessThread in the collection
                        The_Threads[0].IdealProcessor = Logical_Processors -1;                 
                        The_Threads[0].ProcessorAffinity = (IntPtr)Logical_Processors - 1; // Set to the last thread

                        // The_Threads[0].IdealProcessor = 0; // Former Value
                        // The_Threads[0].ProcessorAffinity = (IntPtr)1;  // Former Value
                    }

                    if (High_Priority) { The_Process[0].PriorityClass = ProcessPriorityClass.High; }
                }

                if (The_Process[0] != null) { return true; }
            }
            catch 
            {   
                string Error_Text = "\nFailed to find the process by its file name.";
                //  Fails because the ProcessThreadCollection The_Threads can't get the process
                if (Set_Affinity & High_Priority) { Error_Text = "\nFailed to set Affinity and Priority, because \nI could not find the process by its file name."; }
                else if (Set_Affinity) { Error_Text = "\nFailed to set Affinity, because \nI could not find the process by its file name."; }
                else if (High_Priority) { Error_Text = "\nFailed to set Affinity, because \nI could not find the process by its file name."; }

                if (Set_Affinity | High_Priority) { iConsole(400, 100, Error_Text); }
            }


          
            return false;
        }



        // Checks for Steam.exe, Checks Gamepaths for "Steam" and sets "Is Steam Version" UI Toggle to true and patches all Steam User start entries.
        public string Check_for_Steam_Version()
        {   string Steam_Exe_Path = "";

            try
            {   RegistryKey Steam_Reg_Key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\steam\Shell\Open\Command");
                if (Steam_Reg_Key == null) { iConsole(600, 100, "\n    Error: Registry Key for Steam was not found."); return ""; }

                // Setting Global Variable
                Steam_Exe_Path = Steam_Reg_Key.GetValue("") as string;
                if (Steam_Exe_Path == null | Steam_Exe_Path == "") { iConsole(600, 100, "\n    Value of Registry Key for Steam.exe was not found."); return ""; }

                // "C:\Program Files (x86)\Steam\steam.exe" "%1"
                try { Steam_Exe_Path = Steam_Exe_Path.Substring(1, Steam_Exe_Path.IndexOf('"', 1)).Replace('"', ' '); }
                catch { }

                if (File.Exists(Steam_Exe_Path))
                {   Properties.Settings.Default.Steam_Exe_Path = Steam_Exe_Path;
                    Properties.Settings.Default.Save();
                }
                else { iConsole(600, 100, "\n    Error: Could not find the Steam.exe"); return ""; }

            } catch {}          
    
            return Steam_Exe_Path;
        }


        private void Button_Reset_Blacklist_MouseHover(object sender, EventArgs e)
        {   if (Text_Box_Tags.Visible == true)
            { Set_Resource_Button(Button_Reset_Blacklist, Properties.Resources.Button_Refresh_Lit); }
            else { Set_Resource_Button(Button_Reset_Blacklist, Properties.Resources.Button_Controller_Lit); }
        }

        private void Button_Reset_Blacklist_MouseLeave(object sender, EventArgs e)
        {   if (Text_Box_Tags.Visible == true)
            { Set_Resource_Button(Button_Reset_Blacklist, Properties.Resources.Button_Refresh); }
            else { Set_Resource_Button(Button_Reset_Blacklist, Properties.Resources.Button_Controller); } 
        }

      

        //-----------------------------------------------------------------------------
        // 
        //-----------------------------------------------------------------------------
     
        public void Set_Resource_Button(PictureBox The_Button, Bitmap Resource_Button, int Scale_Treshold = 0)
        {
            try { The_Button.BackgroundImage = Resize_Resource_Image(Resource_Button, The_Button.Size.Width + Scale_Treshold, The_Button.Size.Height + Scale_Treshold); }  catch { }
        }


        //===========================//

        public static Image Resize_Resource_Image(Bitmap image, int new_width, int new_height)
        {
            try
            {
                Bitmap new_image = new Bitmap(new_width, new_height);
                Graphics Picture = Graphics.FromImage((Image)new_image);
                Picture.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                Picture.DrawImage(image, 0, 0, new_width, new_height);
                return new_image;
            }
            catch { }
            return null;
        }
        //===========================//
    

        public char[] Append(char[] x, char[] y)
        {
            if (x == null | y == null) { return null; }
            char[] z = new char[x.Length + y.Length];

            int j = 0;
            for (int i = 0; i < (x.Length + y.Length); i++)  // to combine two arrays
            {
                if (i < x.Length) { z[i] = x[i]; }
                else
                {
                    z[i] = y[j];
                    j = j + 1;
                }
            }

            return z;     
        }
        //===========================//

        public bool Char_Starts_With(char[] x, char y)
        {   if (x[0] == y) { return true; }
            return false;
        }


        //===========================//
        public Bitmap Get_Start_Image()
        {
            return Properties.Resources.Idle_01;              
        }

        //===========================//


        public Bitmap Get_Done_Image()
        {
            return Properties.Resources.Done_01;

           /*
           Bitmap Result = null;
           Random rnd = new Random();
           int Value = rnd.Next(1, 2);  // creates a number between 1 and 4

            if (Properties.Settings.Default.Star_Wars_Theme == false)
            {                                          
                // Otherwise return a Star Wars based image:
                switch (Value)
                {
                    case 1:
                        Result = Properties.Resources.Done_01;
                        break;
                    //case 2:
                    //    Result = Properties.Resources.Shadow_Clone_02;
                    //    break;                

                    default:
                        Result = Properties.Resources.Done_01;
                        break;
                }

                return Result;
            }
            */


            /*
            Value = rnd.Next(1, 4);  // creates a number between 1 and 4
            
            // Otherwise return a Star Wars based image:
            switch (Value)
            {
                case 1:
                    Result = Properties.Resources.Compacted_01;
                    break;
                case 2:
                    Result = Properties.Resources.Compacted_02;
                    break;
                case 3:
                    Result = Properties.Resources.Compacted_03;
                    break;
                case 4:
                    Result = Properties.Resources.Compacted_04;
                    break;

                default:
                    Result = Properties.Resources.Compacted_01;
                    break;
            }
            */

            // return Result;

        }


        //===========================//
        // # Show_Changed_Files = true  Get_Setting_Value();
 
        public void Reset_Tag_List()
        {
            Tag_List = @"# ==================== Settings ====================
# Show_Tooltip = true
# Store_Last_Settings = true
# Request_File_Approval = true
# RGBA_Color = 100, 170, 170, 255 # Cadet Blue

# Set_Launch_Affinity = true
# High_Launch_Priority = true
# Custom_Program_Path = 
# Custom_Start_Parameters = 
# User_Name = false
# Backups_Per_Directory = 10
# Program_Directory = %AppData%\Local\Xml_Axe\
# Disable_EAW_Mode = false
# Text_Format_Delimiter = ;



# ================== Bool Values ===================

bool Planet_Surface_Accessible # Set to No and it will turn all GCs to space only because it sets all Planets to unaccessible. This operation is kinda reversible: it checks if a planet has a ground.ted map to determine whether it is safe to set surface back to accessible.

bool Is_Targetable # Defines whether or not all Hardpoints in current selection or the Mod can be targeted. Not reversible because all Hardpoints in the selection get the same value.

bool Is_Destroyable # Defines whether or not all Hardpoints in current selection or the Mod can be destroyed. Not reversible because all Hardpoints in the selection get the same value.

bool Is_Named_Hero # Set to No and no more Heroes will respawn. The entities that don't have Show_Hero_Head will also hide their hero Images in tactical battles. Not reversible because all Units in the selection get the same value.

bool Show_Hero_Head # This turns a unit into a minor Hero, that does not respawn and their icon won't show up in GC mode. Set to No to hide all (tactical) Icons of minor Heroes. Not reversible because all Units in the selection get the same value.

bool Minor_Heroes_To_Major # This converts all minor Heroes to major heroes that do respawn. Not reversible because minor heroes are merged into the group of majors.

bool Major_Heroes_To_Minor # This converts all major Heroes to minor heroes, and they won't longer respawn or show their icons in the GC mode. Icons of minor heroes only show up in tactical battles. Not reversible because major heroes are merged into the group of minors. 

bool Enable_Abilities # This sets the SubObjectList attribute of the selected entities and can therefore disable or enable all abilities of the Game. Not reversible at all because units that had their abilities disabled before were never meaned to be batch-enabled!

bool Enable_Passive_Abilities # This sets the SubObjectList attribute of the selected entities, it targets passive abilities like systemspy and all fieldcommander bonus. Not reversible at all because units that had their abilities disabled before were never meaned to be batch-enabled!

bool Projectile_Does_Shield_Damage # Set to Yes and apply to the whole mod, to disable all shield piercing effects. Not reversible because all Projectiles in the selection get the same value.

bool Projectile_Does_Energy_Damage # Careful, if set to yes Hitpoint damage will start once enemy energy level reaches 0. Not reversible because all Projectiles in the selection get the same value.

bool Projectile_Does_Hitpoint_Damage # Not reversible because all Projectiles in the selection get the same value.


# =================== Int Values ===================

Tactical_Health = 100

Shield_Points = 100

int Shield_Refresh_Rate = 5 # Usually about 30 for capital ships and less for weaker classes.

int Max_Speed = 1 # In Percent Mode this is bundled to the <Min_Speed> tag, it grows or shrinks both values by the same amount. This Setting ignores objects of Projectile type, unless you explicitly select them as Filter Type.

Percent All_Damage = 10 # This setting bundles Damage and Projectile_Damage as one scalable setting.

Percent Fire_Min_Recharge_Seconds = 10

Percent Fire_Max_Recharge_Seconds = 10

Percent Scale_Factor = 1 # Use this in Percent Mode to scale all units in a Mod. NOTE: The *All Types* filter only means SpaceUnit, UniqueUnit and StarBase. You need to select all other entities explicitly as Filter Type: TransportUnit, Space Heroes, Projectile, Particle and Planet will be ignored, unless you scale them type by type. Keep in mind to not scale too much, because the Particles in models are not scaled by this. Reversible.

Percent Scale_Galaxies = 100 # Adjusts size of Planets and their *Galaxy_Core_Art_Model* and scales their relative position to each other through the Galactic_Position tag. If you are lucky and single GCs are sorted within certain files you can *ignore their files and scale GCs individually.

Int Select_Box_Scale = 100 # Set to 0 and all Ships and Troops will have their select box deactivated. Not reversible because all values in the selection become 0 which can't be scalled. In percent mode this scales the size of all select box circles.

point_float Radar_Icon_Size = 1 # You can scale this double value tag in Percent Mode, along with Scale_Factor to match the new model sizes on the radar. 

Layer_Z_Adjust = 100 # In percent mode this will scale the distance between all height layer values. Reversible if you figure out the correct % values.

Space_Tactical_Unit_Cap = 10 # Sets Unit cap in space tactical battles, for all Factions in the Mod. Don't put too high or it will cause laggs. Actually reversible but all factions are merged to have the same value.

Build_Cost_Credits = 100 # Set the price to 1, then you can build as many units as the population cap allows. Not reversible. In percent mode all prices can be scalled, which is kinda reversible if you work out the correct % values.

Tactical_Build_Cost_Multiplayer = 100 # Set the price to 1 for all Skirmish units. Not reversible. In percent mode all prices can be scalled, which is kinda reversible if you work out the correct % values.

int Population_Value # Scale down in Percent Mode to reduce population requirements for building and spawning units. Reversible.

string Encyclopedia_Text # Axe this as empty string to wipe away all Encyclopedia Texts. Obviously entirely irreversible, please use only for fun purposes!

Percent Rebalance_Everything = Tactical_Health, Shield_Points, Shield_Refresh_Rate, Projectile_Damage, Damage # This balances the most important aspects of the Game: Tactical_Health, Shields, Shield_Refresh_Rate, Projectile_Damage. You can remove or add more Tag types to this tag in the settings! Then they will scale as group by the same % value.
";
        }

  
        //===========================//
        public void Reset_Tag_Box()
        {                
            Combo_Box_Tag_Name.Items.Clear();
            if (User_Input) { Combo_Box_Tag_Name.Text = ""; }
            Disable_Description();

            foreach (string Tag in Process_Tags(Text_Box_Tags.Text))
            { Combo_Box_Tag_Name.Items.Add(Tag); }


            if (Verify_Setting("User_Name")) { User_Name = "_" + Get_Setting_Value("User_Name"); }
            else { User_Name = ""; }
            if (User_Name == "_") { User_Name = ""; }


            // Set Color
            foreach (string Phrase in Tag_List.Split('\n'))
            {   if (Phrase.StartsWith("//") || Phrase == "") { continue; }
                else if (Phrase.Contains("RGBA_Color"))
                {   try
                    {
                        // .Split('#') to get rid of comments after
                        string Setting = (Phrase.Replace(" ", "")).Split('#')[1].Split('=')[1];
                        // iConsole(400, 200, Setting);

                        string[] RGBA = Setting.Split(','); // Split the 4 RGBA values into int

                        int R = 0;
                        int G = 0;
                        int B = 0;
                        int A = 0;

                        Int32.TryParse(RGBA[0], out R);
                        Int32.TryParse(RGBA[1], out G);
                        Int32.TryParse(RGBA[2], out B);
                        Int32.TryParse(RGBA[3], out A);

                        Theme_Color = Color.FromArgb(A, R, G, B);


                        Control[] Controls = { Label_Entity_Name,Label_Type_Filter,Label_Tag_Name,Check_Box_All_Occurances,Label_Tag_Value };
                        foreach (Control The_Control in Controls)
                        { The_Control.ForeColor = Theme_Color; }

                        Set_Checker(List_View_Selection, Theme_Color);
                        break;
                    } catch {}
                }
            }
        }


        //===========================//
        public void Reset_Root_Tag_Box()
        {
            Combo_Box_Type_Filter.Items.Clear();

            if (!EAW_Mode) // Only for EAW Mode
            { Combo_Box_Type_Filter.Items.Add("All Types"); }

            else   
            {
                string[] Entries = new string[] {"All Types", "All in loaded Xml", "Faction Name Filter", "Category Mask Filter", "", "SpaceUnit", "FighterUnit", "UniqueUnit", 
                    "TransportUnit", "GroundInfantry", "GroundVehicle", "HeroUnit", "", "Squadron", "HeroCompany", "GroundCompany", "Planet",
                    "Faction", "HardPoint", "Projectile", "Particle", "", "StarBase", "SpaceBuildable", "SpecialStructure", "SpaceProp", "Prop", "TechBuilding", "GroundBase", 
                    "GroundStructure", "GroundBuildable", "", "Container", "Marker", "MiscObject", "SecondaryStructure", "MultiplayerStructureMarker", "", 
                    "SpacePrimarySkydome", "SpaceSecondarySkydome", "LandPrimarySkydome", "LandSecondarySkydome",
                };

                foreach(string Entry in Entries) { Combo_Box_Type_Filter.Items.Add(Entry); }
            }
        }

        //===========================//

        private void Reset_Script_Box(List<string> Source_List = null)
        {
            List_View_Selection.Items.Clear();

            if (Source_List == null) { return; }
            try
            {   // foreach (string Entry in Source_List)
                for (int i = 0; i <= Source_List.Count - 1; i++)
                {
                    if (i > 20) { break; } // Size Limit

                    string Source = Source_List[i];
                    if (Source != "" && !List_View_Matches(List_View_Selection, Source)) { List_View_Selection.Items.Add(Source); }
                }
            } catch {}

            Set_Checker(List_View_Selection, Theme_Color);
        }

        //===========================//
        private void Combo_Box_Entity_Name_TextChanged(object sender, EventArgs e)
        {

            if (Combo_Box_Entity_Name.Text == "Find_And_Replace") { Label_Tag_Name.Text = "Old Tag Value"; }
            else { Label_Tag_Name.Text = "Tag Name"; }


            if (Combo_Box_Entity_Name.Text == "Insert_Random_Int") 
            {
                Operation_Mode = "Point";
                Track_Bar_Tag_Value_Scroll(null, null); // Showing the 2 float values in the textbox for us.  
                Button_Scripts_MouseLeave(null, null); // Showing the XY image variant of this button

                Label_Tag_Value.Text = "Range of Int values";
                Int_Distance = 10; // Reset distance between x and y value
                Last_Value = 0;

                if (Match_Setting("Show_Tooltip"))
                {   Text_Box_Description.Visible = true;

                    // Special Tooltip, that describes the Random Mode                   
                    Text_Box_Description.Text = "The two entries in the value text box define the range of random values to fill into each selected xml tag while the Axe runns in Random Mode. Please watch out to not use this for tags that expect any other variable type then int.";                   
                }
            }
            else if (Combo_Box_Entity_Name.Text == "Insert_Random_Float")
            {
                Operation_Mode = "Point_Float";
                Track_Bar_Tag_Value_Scroll(null, null);
                Button_Scripts_MouseLeave(null, null); 

                Label_Tag_Value.Text = "Range of float values";
                Float_Distance = 1F;
                Last_Value = 0;

                if (Match_Setting("Show_Tooltip"))
                {
                    Text_Box_Description.Visible = true;

                    // Special Tooltip, that describes the Random Float Mode                   
                    Text_Box_Description.Text = "The two entries in the value text box define the range of random values to fill into each selected xml tag while the Axe runns in Random Mode. Please watch out to not use this for tags that expect any other variable type then float.";
                }
            }
            else 
            {   Disable_Description();
                Operation_Mode = "Int";
                Label_Tag_Value.Text = "New Tag Value";
            }


            if (Ying_Dominates) 
            {   Queried_Attribute = Combo_Box_Entity_Name.Text;

                Properties.Settings.Default.Attribute_Name = Queried_Attribute; 
                Properties.Settings.Default.Save();
            }
         
        }




        private void Combo_Box_Entity_Name_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Return))
            {
                if (Ying_Dominates) { Button_Attribute_Click(null, null); }
                else { Button_Search_Click(null, null); } // Otherwise understand the enter key as order to search for this term.
            }
        }

 
        //===========================//
        private void Combo_Box_Type_Filter_TextChanged(object sender, EventArgs e)
        {
            if (!User_Input) { return; } // Preventing Recursion


            switch (Combo_Box_Type_Filter.Text)
            {
                case "GroundInfantry":
                    Scale_Factor = 1;
                    break;
                case "GroundVehicle":
                    Scale_Factor = 1;
                    break;
                case "GroundCompany":
                    Scale_Factor = 1;
                    break;
                case "Planet":
                    Scale_Factor = 1;
                    break;
                         
                //case "Faction":
                //    Scale_Factor = 1;
                //    break;
                    
            }

            if (Operation_Mode == "Percent") { Scale_Factor = 10; } // Percentage Override

            if (Last_Combo_Box_Entity_Name == "Faction Name Filter" | Last_Combo_Box_Entity_Name == "Category Mask Filter")
            {   
                Last_Combo_Box_Entity_Name = "";
                Combo_Box_Entity_Name.Text = "";
                Reset_Combo_Box_Entity_Name(); 
            } // Don't chain here!
    


            if (Combo_Box_Type_Filter.Text == "Faction Name Filter")
            {
                Label_Entity_Name.Text = "Faction Name";
                Label_Type_Filter.Text = "Filter Type";
                Button_Search.Visible = false;   


                if (Found_Factions.Count() == 0) 
                { Found_Factions = Query_For_Entity_Parent("Faction"); }

                if (Found_Factions.Count() > 0)
                {
                    Combo_Box_Entity_Name.Items.Clear();

                    foreach (string Faction_Name in Found_Factions)
                    {   if (Faction_Name != "" & !Combo_Box_Matches(Combo_Box_Entity_Name, Faction_Name, true))
                        { Combo_Box_Entity_Name.Items.Add(Faction_Name); }
                    }

                  
                    User_Input = false; // Preventing Recursion
                    Combo_Box_Entity_Name.Text = "";
                    User_Input = true;

                    Last_Combo_Box_Entity_Name = "Faction Name Filter";
                    Combo_Box_Entity_Name.DroppedDown = true; // Show results to the User                  
                }
            }
            else if (Combo_Box_Type_Filter.Text == "Category Mask Filter")
            {
                Label_Entity_Name.Text = "Category Mask";
                Label_Type_Filter.Text = "Filter Type";
                Button_Search.Visible = false;

               
                if (Category_Masks.Count() == 0)
                { Category_Masks = Query_For_Tag("CategoryMask", "", true); }
                

                if (Category_Masks.Count() > 0)
                {
                    Combo_Box_Entity_Name.Items.Clear();
                    foreach (string Categories in Category_Masks)
                    {
                        string[] Category = Wash_String(Categories).Split('|');
 
                        for (int i = Category.Count() - 1; i >= 0; --i)
                        {
                            if (Category[i] != "" & !Combo_Box_Matches(Combo_Box_Entity_Name, Category[i], true))
                            { Combo_Box_Entity_Name.Items.Add(Category[i]); }
                        }
                    }


                    User_Input = false; // Preventing Recursion
                    Combo_Box_Entity_Name.Text = "";
                    User_Input = true;

                    Last_Combo_Box_Entity_Name = "Category Mask Filter";
                    Combo_Box_Entity_Name.DroppedDown = true;                
                }
                
            } else { Set_Label_Entity_Name_Text(); }

        

            if (Combo_Box_Type_Filter.Text == "All in loaded Xml") // Don't use elseif here
            {   Combo_Box_Entity_Name.Text = "Multi";
                Combo_Box_Type_Filter.Text = ""; // Because then this can trigger a 2nd time in a row

                if (!List_View_Selection.Visible)
                {
                    Button_Start_Click(null, null); // This shows the List_View with Xml Entities of the loaded Xml
                    Set_Resource_Button(Button_Start, Properties.Resources.Button_Logs_Lit);
                }

                if (List_View_Selection.Items.Count > 0)
                {   for (int i = List_View_Selection.Items.Count - 1; i >= 0; --i)
                    {   // Selecting everything
                        if (List_View_Selection.Items[i].Text != "")
                        { List_View_Selection.Items[i].Selected = true; }
                    }
                }
                List_View_Selection.Focus();
            }
            //else if (Combo_Box_Entity_Name.Text ==  "Multi")
            //{ Combo_Box_Entity_Name.Text = ""; }

            // iConsole(400, 100, "\"" + Combo_Box_Type_Filter.Text + "\"");

        }



        public void Reset_Combo_Box_Entity_Name()
        {
            Combo_Box_Entity_Name.Items.Clear();
            Combo_Box_Entity_Name.Items.Add("None");
            Combo_Box_Entity_Name.Items.Add("Find_And_Replace");
            Combo_Box_Entity_Name.Items.Add("Insert_Random_Int");
            Combo_Box_Entity_Name.Items.Add("Insert_Random_Float");
        }

        public void Set_Label_Entity_Name_Text()
        {
            if (!EAW_Mode)
            {
                Label_Entity_Name.Text = Queried_Attribute; 

                Label_Type_Filter.Text = "Parent Name";
                Button_Search.Visible = true;
            }
            else if (Combo_Box_Entity_Name.Text == "Find_And_Replace")
            {
                Label_Entity_Name.Text = "Old Tag Value";
                Label_Type_Filter.Text = "Filter Type";
                Button_Search.Visible = true;
            }
            else
            {
                Label_Entity_Name.Text = Queried_Attribute; 
                Label_Type_Filter.Text = "Filter Type";
                Button_Search.Visible = true;
            }
        }
      

        //===========================//
        private List<string> Query_For_Entity_Parent(string Parent_Tag_Name)
        {
            IEnumerable<XElement> Instances = null;
            Found_Entities = new List<string>();


            foreach (var Xml in Get_All_Files(Xml_Directory, "xml"))
            {
                try
                {   // ===================== Opening Xml File =====================                            
                    XDocument Xml_File = XDocument.Load(Xml, LoadOptions.PreserveWhitespace);

                    if (Parent_Tag_Name == "All Types")
                    { iConsole(400, 100, "\nSorry, \"All Types\" is not specific enough to search."); return Found_Entities; }
                    else
                    { Instances =
                        from All_Tags in Xml_File.Root.Descendants()
                        where All_Tags.Name == Parent_Tag_Name
                        select All_Tags;
                    }
                }
                catch {}


                if (Instances.Any())
                {
                    foreach (XElement Instance in Instances)
                    {
                        try
                        {   // Be aware Queried_Attribute is a variable that decides the outcome!
                            string Faction_Name = (string)Instance.Attribute(Queried_Attribute);
                            if (Match_Without_Emptyspace(Queried_Attribute, "First_Attribute")) { Faction_Name = (string)Instance.FirstAttribute; }

                            if (!Found_Entities.Contains(Faction_Name))
                            { Found_Entities.Add(Faction_Name); }
                        } catch {}
                    }                 
                }
            }

            return Found_Entities;
        }


        //===========================//
        // Query_For_Tag(Tag_Name, Tag_Value) = Return Name of entities that contain this value in its Tag_Name
        // Query_For_Tag(Tag_Name) = Return Name of entities that contain this Tag_Name
        //  Query_For_Tag(Tag_Name, "", true) = return the Value inside of the tag

        private List<string> Query_For_Tag(string Tag_Name, string Tag_Value = "", bool Return_Tag_Content = false)
        {
            IEnumerable<XElement> Instances = null;
            Found_Entities = new List<string>();


            foreach (var Xml in Get_All_Files(Xml_Directory, "xml"))
            {
                try
                {   // ===================== Opening Xml File =====================                            
                    XDocument Xml_File = XDocument.Load(Xml, LoadOptions.PreserveWhitespace);

                    if (Tag_Value == "") // Then just search for entities with this child tag
                    {
                        Instances =
                         from All_Tags in Xml_File.Root.Descendants()
                         where All_Tags.Descendants(Tag_Name).Any()
                         select All_Tags;
                    }
                    else
                    {
                        Instances =
                         from All_Tags in Xml_File.Root.Descendants()
                         where All_Tags.Descendants(Tag_Name).Any() // We need this to prevent null exceptions! And it increases speed a lot.
                         where All_Tags.Descendants(Tag_Name).Last().Value.Contains(Tag_Value)
                         select All_Tags;
                    }
                }
                catch {}


                if (Instances.Any())
                {
                    foreach (XElement Instance in Instances)
                    {
                        try
                        {   // Be aware Queried_Attribute is a variable that decides the outcome!
                            string Current_Name = (string)Instance.Attribute(Queried_Attribute);
                            if (Match_Without_Emptyspace(Queried_Attribute, "First_Attribute")) { Current_Name = (string)Instance.FirstAttribute; }

                            if (Return_Tag_Content) { Current_Name = (string)Instance.Descendants(Tag_Name).Last().Value; }

                            if (!Found_Entities.Contains(Current_Name))
                            { Found_Entities.Add(Current_Name); }
                        }
                        catch { }
                    }
                }
            }

            return Found_Entities;
        }


        //===========================//
        private void Combo_Box_Tag_Name_TextChanged(object sender, EventArgs e)
        {
            if (!User_Input) { return; }     
            bool Reset_Type_Filter = false;



            switch (Combo_Box_Tag_Name.Text)
            {
                case "Planet_Surface_Accessible":
                    Combo_Box_Type_Filter.Text = "Planet";                 
                    break;
                 case "Scale_Galaxies":
                    Combo_Box_Type_Filter.Text = "Planet";
                    Button_Scripts_MouseLeave(null, null);          
                    break;

                  
                case "Is_Targetable":
                    Combo_Box_Type_Filter.Text = "HardPoint";
                    break;
                case "Is_Destroyable":
                    Combo_Box_Type_Filter.Text = "HardPoint";
                    break;
                case "Is_Named_Hero":
                    Reset_Type_Filter = true;
                    break;


                case "Projectile_Does_Shield_Damage":
                    Combo_Box_Type_Filter.Text = "Projectile";
                    break;
                case "Projectile_Does_Energy_Damage":
                    Combo_Box_Type_Filter.Text = "Projectile";
                    break;
                case "Projectile_Does_Hitpoint_Damage":
                    Combo_Box_Type_Filter.Text = "Projectile";
                    break;


                case "Tactical_Health":
                    Reset_Type_Filter = true;
                    break;
                case "Shield_Points":
                    Reset_Type_Filter = true;
                    break;
                case "Shield_Refresh_Rate":
                    Reset_Type_Filter = true;
                    break;
                case "Select_Box_Scale":
                    Reset_Type_Filter = true;
                    break;

                case "Space_Tactical_Unit_Cap":
                    Combo_Box_Type_Filter.Text = "Faction";
                    break;                 
                case "Build_Cost_Credits":
                    Reset_Type_Filter = true;
                    break;
                case "Tactical_Build_Cost_Multiplayer":
                    Reset_Type_Filter = true;
                    break;

                default:                
                    Reset_Type_Filter = true;
                    break;
            }
              

            if (Reset_Type_Filter) 
            {
                string Probably_Wrong = "Planet,HardPoint,Faction,Projectile";

                foreach (string Entry in Probably_Wrong.Split(','))
                {   if (Entry == Combo_Box_Type_Filter.Text)
                    { Combo_Box_Type_Filter.Text = ""; break; }
                }
            }


            Process_Tags(Text_Box_Tags.Text);
            Last_Combo_Box_Tag_Name = Combo_Box_Tag_Name.Text;           
        }

        //===========================//
        private bool Execute(string File_Path, string Arguments = "", string Working_Dir = "")
        {   try
            {   ProcessStartInfo Process_Info = new ProcessStartInfo();
                Process_Info.FileName = File_Path;
                Process_Info.Arguments = Arguments;

                if (Working_Dir != "") { Process_Info.WorkingDirectory = Working_Dir; }

                // iConsole(60, 100, Program_Path + " " + Arguments); // return true;
                Process.Start(Process_Info);

                // The_Process = Process.GetProcessesByName(Program_Name); // Retrieve the app processes. 
            } catch { iConsole(60, 100, "\nFailed to find and launch " + File_Path); return false; }

            return true;
        }


        //===========================//
        private void Run_Cmd(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            // start.Arguments = string.Format("{0} {1}", cmd, args);

            if (cmd.EndsWith("py")) 
            {   start.FileName = @"python.exe";
                start.Arguments = cmd + " " + args;
            }
            else 
            {   start.FileName = cmd;
                start.Arguments = args;
            }        
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.RedirectStandardInput = true;
            start.CreateNoWindow = true;
 

            using (Process process = Process.Start(start))
            {
                process.WaitForExit();
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    iConsole(600, 400, result);
                    // Console.Write(result);
                }
            }
        }

        //===========================//
        private List<string> Process_Tags(string Input = "")
        {
            User_Input = false;
            string Scale_Format = "";
            string[] Tag_Info = new string[] { };
            List<string> List_Of_Tags = new List<string>();
            string[] Variable_Types = new string[] { "Bool", "String", "Point_Float", "Point", "Percent", "Int"};


            // The + " #" makes sure the last entry comment stays empty
            foreach (string Phrase in (Input + " #").Split('\n'))
            {
                if (Phrase.StartsWith("//") || Phrase.StartsWith("#") || Phrase == "" || Phrase == "\n" || Phrase == "\r") { continue; } // Skip commented Lines

                string Tag_Name = Phrase.Replace(" ", "");
                string Tag_Comment = ""; // Reset

                try
                {   if (Phrase.Contains("#"))
                    {   Tag_Info = Phrase.Split('#');
                        Tag_Name = Tag_Info[0].Replace(" ", "");
                        Tag_Comment = Tag_Info[1];
                    }


                    Operation_Mode = "Int"; // Defaulting

                    foreach (string Var_Type in Variable_Types)
                    {
                        if (Tag_Name.ToLower().StartsWith(Var_Type.ToLower()))
                        {   Operation_Mode = Var_Type;
                            Tag_Name = Tag_Name.Substring(Var_Type.Length, Tag_Name.Length - Var_Type.Length);// Removing prefix
                        }
                    }


                    // This overwrites the "Scale_Format" from above - which is important for the range of int type Scale_Factor 
                    if (Tag_Name.Contains("=")) // Seperating the tag name and its expected format (int, bool or string)
                    {
                        Tag_Info = Tag_Name.Split('=');
                        Tag_Name = Tag_Info[0];
                        Scale_Format = Tag_Info[1];
                        // iConsole(400, 200, Tag_Name + " + " + Scale_Format);
                    }


                    

                    if (Tag_Name != "") { List_Of_Tags.Add(Tag_Name); };

                    // Loading the list of Tags, the values of all these tags are going to be scalled as group.
                    if (Tag_Name == "Rebalance_Everything") { Balancing_Tags = Scale_Format.Split(','); }



                    // Show description of the active tab
                    if (Tag_Name == Combo_Box_Tag_Name.Text)
                    {
                        // iConsole(400, 200, Tag_Name + " + " + Tag_Comment);

                        if (Tag_Comment == "") { Disable_Description(); }
                        else if (Match_Setting("Show_Tooltip"))
                        {
                            if (Tag_Comment[0] == ' ') { Tag_Comment = Remove_Emptyspace_Prefix(Tag_Comment); }

                            Text_Box_Description.Text = Tag_Comment;
                            Text_Box_Description.Visible = true;
                        }
                        break;
                    }
                } catch {}          
            }




            if (Text_Box_Tags.Visible) // Not doing the stuff below while in Settings mode
            {   User_Input = true;
                return List_Of_Tags;
            }
            else if (Operation_Mode == "" | Operation_Mode == "String")
            {   Track_Bar_Tag_Value.Visible = false;
                // Combo_Box_Tag_Value.Items.Clear(); // Disabled

                Operation_Mode = "Int";
                Button_Operator_MouseLeave(null, null);
            }
            else if ( Operation_Mode == "Bool")
            {   Button_Percentage.Visible = false;
                Track_Bar_Tag_Value.Visible = false;               

                if (Get_Text_Box_Bool() == "Neither") { Combo_Box_Tag_Value.Text = ""; }           
            }

            else // if (Operation_Mode == "Int") // It will probably be int
            {
                if (Operation_Mode == "Percent") 
                {   Scale_Factor = 10;
                    Operation_Mode = "Int"; // So the button toggles it properly back into Percent mode 
                    Button_Percentage_Click(null, null); 
                }

                else if (!Operation_Mode.Contains("Point"))                                        
                {
                    int.TryParse(Scale_Format, out Scale_Factor);
                    if (Scale_Factor == 0) { Scale_Factor = 100; } // Failsafe, default Scale Factor is 100
                }
                // iConsole(400, 200, "Scale is " + Scale_Factor);

                // Resetting the right scale factor
                if (Scale_Factor == 10) { Combo_Box_Type_Filter_TextChanged(null, null); }
                if (Get_Text_Box_Bool() != "Neither") { Combo_Box_Tag_Value.Text = ""; }        
          

                Button_Percentage.Visible = true;
                Track_Bar_Tag_Value.Visible = true;

                Silent_Mode = false; // Bypass automatic silencing of Hint-box
                Track_Bar_Tag_Value_Scroll(null, null); // Updating to Values
                Silent_Mode = true;
                // Combo_Box_Tag_Value.Items.Clear(); // Disabled
            }

            Button_Operator_MouseLeave(null, null);  
            Button_Scripts_MouseLeave(null, null); // Starting or exiting XY mode for Points
            Button_Percentage_MouseLeave(null, null);


            User_Input = true;
            return List_Of_Tags;
        }




        private string Get_Text_Box_Bool()
        {
            if (Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "True")| Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "Yes")) { return "True"; }
            else if (Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "False") | Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "No")) { return "False"; }
            return "Neither";
        }
   


        private void Combo_Box_Tag_Value_TextChanged(object sender, EventArgs e)
        { 
            if (User_Input) 
            {   if (Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "True") | Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "False")
                    | Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "Yes") | Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "No"))
                { Operation_Mode = "Bool"; }
                else if (Operation_Mode == "Percent")
                {   // Don't move this to above.
                    if (!Combo_Box_Tag_Value.Text.Contains("%")) { Combo_Box_Tag_Value.Text += "%"; }
                }
                else if (Operation_Mode != "Point" && Operation_Mode != "Point_Float") { Operation_Mode = "Int"; }
            }
            if (Silent_Mode) { Disable_Description(); }

            Button_Operator_MouseLeave(null, null); // Check if bool and refresh
            Button_Percentage_MouseLeave(null, null);
        }



        //===========================//
        
        // Window Size expects 2 int parameters like 700, 200
        public void iConsole(int Window_Size_X, int Window_Size_Y, string Text)
        {   // Innitiating new Form
            Caution_Window Display = new Caution_Window();
            Display.Size = new Size(Window_Size_X, Window_Size_Y);

            // Using Theme colors for Text and Background
            Display.Text_Box_Caution_Window.BackColor = Color.Gray;
            Display.Text_Box_Caution_Window.ForeColor = Color.White;

        
            List<string> New_Text = new List<string>();

            try
            {   foreach (string Line in Text.Split('\n'))
                { New_Text.Add("      " + Line); }

                Display.Text_Box_Caution_Window.Text = string.Join("\n", New_Text); // "\n      " + Text;
                Display.Show();
            } catch {}

        }


        //===========================//

        public void iDialogue(int Window_Size_X, int Window_Size_Y, string Button_A_Text, string Button_B_Text, string Button_C_Text, string Button_D_Text, string Text, List<string> The_List = null, bool List_Exclusion_Mode = false)
        {
            //========== Displaying Error Messages to User   
            // Innitiating new Form
            Caution_Window Display = new Caution_Window();
            Display.Size = new Size(Window_Size_X, Window_Size_Y);

            // Using Theme colors for Text and Background
            Display.Text_Box_Caution_Window.BackColor = Color.Gray;
            Display.Text_Box_Caution_Window.ForeColor = Color.White;
            Display.List_View_Info.BackColor = Color.Gray;


            int Button_Y = 96; // 88
            Display.Button_Invert_Selection.Location = new Point(670, Display.Size.Height - (Button_Y - 7));

           

            if (Button_D_Text != "false")
            {
                // The first 2 buttons moves aside to free space for this one
                Display.Button_Caution_Box_1.Visible = true;
                Display.Button_Caution_Box_1.Text = Button_A_Text;
                Display.Button_Caution_Box_1.Location = new Point(30, Display.Size.Height - Button_Y);

                Display.Button_Caution_Box_2.Visible = true;
                Display.Button_Caution_Box_2.Text = Button_B_Text;
                Display.Button_Caution_Box_2.Location = new Point(350, Display.Size.Height - Button_Y);

                Display.Button_Caution_Box_3.Visible = true;
                Display.Button_Caution_Box_3.Text = Button_C_Text;
                Display.Button_Caution_Box_3.Location = new Point(190, Display.Size.Height - Button_Y);

                Display.Button_Caution_Box_4.Visible = true;
                Display.Button_Caution_Box_4.Text = Button_D_Text;
                Display.Button_Caution_Box_4.Location = new Point(510, Display.Size.Height - Button_Y);
            }
            else if (Button_C_Text != "false")
            {
                // The first 2 buttons moves aside to free space for this one
                Display.Button_Caution_Box_1.Visible = true;
                Display.Button_Caution_Box_1.Text = Button_A_Text;
                Display.Button_Caution_Box_1.Location = new Point(60, Display.Size.Height - Button_Y);

                Display.Button_Caution_Box_2.Visible = true;
                Display.Button_Caution_Box_2.Text = Button_B_Text;
                Display.Button_Caution_Box_2.Location = new Point(380, Display.Size.Height - Button_Y);

                Display.Button_Caution_Box_3.Visible = true;
                Display.Button_Caution_Box_3.Text = Button_C_Text;
                Display.Button_Caution_Box_3.Location = new Point(220, Display.Size.Height - Button_Y);
            }

            else if (Button_B_Text != "false")
            {
                Display.Button_Caution_Box_2.Visible = true;
                Display.Button_Caution_Box_2.Text = Button_B_Text;
                Display.Button_Caution_Box_2.Location = new Point(280, Display.Size.Height - Button_Y);
            }


            if (Button_A_Text != "false" & Button_C_Text == "false")
            {
                Display.Button_Caution_Box_1.Visible = true;
                Display.Button_Caution_Box_1.Text = Button_A_Text;
                Display.Button_Caution_Box_1.Location = new Point(120, Display.Size.Height - Button_Y);
            }


            if (The_List == null)
            { 
                // Display.Text_Box_Caution_Window.Text = Text;
                List<string> New_Text = new List<string>();
                foreach (string Line in Text.Split('\n'))
                { New_Text.Add("      " + Line); }
                Display.Text_Box_Caution_Window.Text = string.Join("\n", New_Text);
            }
            else
            {
                Display.MinimumSize = new Size(Window_Size_X, Window_Size_Y);
                Display.List_Exclusion_Mode = List_Exclusion_Mode;
                Display.List_View_Info.Visible = true;
                Display.Button_Invert_Selection.Visible = true;
                Display.List_View_Info.Items.Add(Text); // Text serves as Header here
                Display.List_View_Info.Items.Add("");
                Display.List_View_Info.Items.Add("");

                foreach (string Entry in The_List)
                { Display.List_View_Info.Items.Add(Entry); }

                Set_Checker(Display.List_View_Info, Theme_Color);
            }


            Display.ShowDialog(this);
        }


        //===========================//
        public List<string> Get_All_Files(string Dir_Path, string Extension = "*") // Extension defaults to all files of all types
        {
            List<string> All_Files = new List<string>();

            string Error = "\nPlease dragg and drop any target dir, \ninto either of the boxes.";
            if (Extension == "xml") { Error = "\nPlease dragg and drop any target Xml, \nor the Xml directory of your mod into the Dropzone."; }
            
            
            if (Dir_Path == "" | Dir_Path == null)
            { iConsole(600, 100, Error); return null; }


            try
            {   if (Directory.Exists(Dir_Path))
                {   foreach (string The_File in Directory.GetFiles(Dir_Path, "*." + Extension, System.IO.SearchOption.AllDirectories))
                    { if (!The_File.EndsWith(".config")) { All_Files.Add(The_File); } }
                }
                else
                { iConsole(600, 100, Error); return null;  }
            } catch {}

            return All_Files;
        }


        //===========================//
        public List<string> Get_All_Directories(string Dir_Path, bool Only_Root = false) // Extension defaults to all files of all types
        {
            List<string> All_Folders = new List<string>();
            string The_Error = "No Directory Path was specified.";
            if (Dir_Path == "" | Dir_Path == null) { iConsole(600, 100, The_Error); return null; }

            try
            {   string[] Found_Folders = null;
                if (Only_Root) { Found_Folders = Directory.GetDirectories(Dir_Path, "*.*", System.IO.SearchOption.TopDirectoryOnly); }
                else { Found_Folders = Directory.GetDirectories(Dir_Path, "*.*", System.IO.SearchOption.AllDirectories); }
                
                if (Directory.Exists(Dir_Path))
                {
                    foreach (string Folder in Found_Folders)
                    { All_Folders.Add(Folder); }
                }
                else { iConsole(600, 100, The_Error); return null; }

            } catch {}

            return All_Folders;
        }



        //===========================//
        // This returns all, even the unselected ones
        public List<string> Get_All_List_View_Items(ListView List_View)
        {
            List<string> Selection_List = new List<string>();
            if (List_View.Items.Count < 1) { return null; }

            for (int i = List_View.Items.Count - 1; i >= 0; --i)
            {
                if (List_View.Items[i].Text != "") { Selection_List.Add(List_View.Items[i].Text); }
            } return Selection_List;
        }


        //=====================//
        // Returns only selected items
        public List<string> Select_List_View_Items(ListView List_View)
        {
            List<string> Selection_List = new List<string>();
            if (List_View.Items.Count < 1) { return null; }

            for (int i = List_View.Items.Count - 1; i >= 0; --i)
            {   // Was "if (List_View.Items[i].Selected & List_View_Hard_Points.Items[i].Text != "")"
                if (List_View.Items[i].Selected & List_View.Items[i].Text != "")
                { Selection_List.Add(List_View.Items[i].Text); }
            } return Selection_List;
        }

        //===========================//
        public string Select_List_View_First(ListView List_View, bool Fallback_to_Slot_0 = false)
        {   // If none is selected we grab anything in Slot 0
            if (List_View.SelectedItems.Count < 1 & Fallback_to_Slot_0 && List_View.Items.Count > 0)
            {
                try
                {
                    if (List_View.Items[0].Text != "")
                    {
                        List_View.Items[0].Selected = true;
                        List_View.Select();
                    }
                }
                catch { }
            }

            if (List_View.SelectedItems.Count > 0)
            { if (List_View.SelectedItems[0].Text != "") { return List_View.SelectedItems[0].Text; } }

            return "";
        }


        //===========================// Theme_Color
        public void Set_Checker(ListView The_Box, Color Selected_Color)
        {
            // Sorting here instead of the default property because of timing issues with the loop below
            The_Box.Sort();

            foreach (ListViewItem Item in The_Box.Items)
            {   // Every second Tag should have this value in order to create a checkmate pattern with good contrast
                if (Item.Index % 2 == 0)
                {   Item.ForeColor = Color.White;
                    Item.BackColor = Selected_Color;

                    //if (Color_Override != Color.White) { Item.BackColor = Color_Override; }
                    //else { Item.BackColor = Theme_Color; }
                }
                else
                {   Item.ForeColor = Color.Black;
                    Item.BackColor = Color.LightGray;
                }
            }
        }


        // ========================================================
        // Visualizing user interaction on the UI
        // ========================================================
        public void Set_Backup_Checker(string Certain_Backup = "")
        {
            Set_Checker(List_View_Selection, Theme_Color);


            if (Certain_Backup != "") // Highlight Parents of his.
            {
                List<string> Current_Branch = Get_Backup_Parents(Certain_Backup);
                // iConsole(400, 200, string.Join("\n", Current_Branch));


                foreach (ListViewItem Item in List_View_Selection.Items)
                {
                    if (Item.Text == Certain_Backup) { Item.BackColor = Color.DodgerBlue; } // Color.AliceBlue; }
                    else if (Current_Branch.Contains(Item.Text)) { Item.BackColor = Color.DeepSkyBlue; }
                    else if (Item.Text == Current_Backup) { Item.BackColor = Color.Orange; } // break; } // Highlighting Selection                  
                }
            }
            else // Simple Selection
            {
                foreach (ListViewItem Item in List_View_Selection.Items)
                { if (Item.Text == Current_Backup) { Item.BackColor = Color.Orange; } }
            }
        }

        //===========================//

        // Move_Unused_Files("models"); Move_Unused_Files("textures"); or just Move_Unused_Files(); to move all of them
        private void Move_Unused_Files(string Mode = "all")
        {                      
            string Model_Directory = Mod_Directory + @"\Data\Art\Models";
            string Texture_Directory = Mod_Directory + @"\Data\Art\Textures";
            string Extension = ".alo"; // Defaulting to .alo
            string Unused_Dir = Model_Directory + @"\Unused";       

            List<string> Models = Get_All_Files(Model_Directory, "alo");
            List<string> All_DDS = Get_All_Files(Texture_Directory, "dds");
            List<string> All_TGA = Get_All_Files(Texture_Directory, "tga");
            List<string> Last_Selection = Models;


            if (Models.Count() > 0 & !Directory.Exists(Unused_Dir)) { Directory.CreateDirectory(Unused_Dir); }
            if (!Directory.Exists(Texture_Directory + @"\Unused")) { Directory.CreateDirectory(Texture_Directory + @"\Unused"); }



            foreach(string Missing_File in File.ReadAllLines(Properties.Settings.Default.Mod_Directory + @"\Data\cleanup.txt"))
            {
                if (Missing_File == "") { continue; }

               
                if (Missing_File.Contains(".alo"))
                {
                    if (Mode == "textures") { continue; } // Move the textures only!
                    
                    Extension = "alo"; // Defaulting to .alo
                    Unused_Dir = Model_Directory + @"\Unused";                 
                    Last_Selection = Models;                  
                }
                else if (Missing_File.Contains(".dds"))
                {
                    if (Mode == "models") { continue; } // Move the models only!

                    Unused_Dir = Texture_Directory + @"\Unused";               
                    Extension = "dds";
                    Last_Selection = All_DDS;
                }
                else if (Missing_File.Contains(".tga"))
                {
                    if (Mode == "models") { continue; }

                    Unused_Dir = Texture_Directory + @"\Unused";
                    Extension = "tga";
                    Last_Selection = All_TGA;
                }


                // Keeping the beginning of the File_Path but removing the "Missing" or "Unused" info
                string The_File = Regex.Replace(Missing_File, Extension + ".*", Extension).Replace(@"/", @"\"); // So \ signs match properly
  


                foreach (string File_Path in Last_Selection)
                {
                    if (File_Path.ToLower().EndsWith(The_File)) // .ToLower() because the .py script converts all readings to lower
                    {
                        if (The_File.Contains(@"\")) // Considering sub directories   
                        {   
                            // continue; // temporary disabled until the .py script stops false reporting of models inside of subdirectories
                            Unused_Dir = Path.GetDirectoryName(File_Path) + @"\Unused";
                            if (!Directory.Exists(Unused_Dir)) { Directory.CreateDirectory(Unused_Dir); }  
                        }                                       
                                                                     
                        Moving(File_Path, Unused_Dir);
                        // iConsole(300, 100, File_Path.ToLower() + ", " + The_File); // Visualising what happens in the memory

                        break; // If matches we break this loop, so we continue to the next entry
                    }                   
                } 
            }

        }



       //=====================//
        public void Moving(string Path_and_File, string New_Path)
        {   try
            {   FileAttributes The_Attribute = File.GetAttributes(Path_and_File);
                if (!Directory.Exists(New_Path)) { Directory.CreateDirectory(New_Path); }

                //detect whether its a directory or file
                if ((The_Attribute & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    try { System.IO.Directory.Move(Path_and_File, New_Path + @"\" + Path.GetFileName(Path_and_File)); } catch {}
                }
                else
                {
                    try { System.IO.File.Move(Path_and_File, New_Path + @"\" + Path.GetFileName(Path_and_File)); } catch {}
                }
            } catch {}           
        }


        //========= Thanks to jaysponsored form Stackoverfl0w ==========//
        // I almost can't believe Microsoft doesent provide a proper Copy function for Directories !! 
        public void Copy_Now(string Source_Directory, string Destination_Directory)
        {
            if (!Directory.Exists(Source_Directory)) { return; }

            // substring is to remove Destination_Directory absolute path (E:\).
            try {  
                // Create subdirectory structure in destination    
                foreach (string dir in Directory.GetDirectories(Source_Directory, "*", System.IO.SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(Destination_Directory + dir.Substring(Source_Directory.Length));
                    // Example:
                    //     > C:\sources (and not C:\E:\sources)
                }
            } catch {}

            try {
                foreach (string file_name in Directory.GetFiles(Source_Directory, "*.*", System.IO.SearchOption.AllDirectories))
                {
                    if (file_name.EndsWith("Axe_Info.txt")){ continue; } // Exception
                    File.Copy(file_name, Destination_Directory + file_name.Substring(Source_Directory.Length), true);
                }
            } catch {}              
        }

        //=====================//
        // If exists in source path copy and OVERWRITE that to target
        public void Verify_Copy(string Source_Path_and_File, string New_Path_and_File)
        {  
            if (!Directory.Exists(Path.GetDirectoryName(New_Path_and_File)))
            { Directory.CreateDirectory(Path.GetDirectoryName(New_Path_and_File)); }

            File.Copy(Source_Path_and_File, New_Path_and_File, true);                    
        }

        //=====================//
        public void Deleting(string Data)
        {
            if (Directory.Exists(Data)) 
            {
                try { Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(Data, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin); }
                catch {}
            }
            else if (File.Exists(Data)) 
            {
                try { Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(Data, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin); }
                catch {}
            }
        }



        //=====================//
        private void Button_Backup_Click(object sender, EventArgs e)
        {
            if (UI_Mode == "Backup") 
            {           
                Zoom_List_View(1);
                Clear_Last_Mode(); 
                Set_UI_Into_Backup_Mode(false); // Needs to run after Clear_Last_Mode(); 

                At_Top_Level = true;
                Skipp_First_Trigger = true;
                Load_Xml_Content(Properties.Settings.Default.Last_File); // Auto toggles to visible  

                Button_Operator.Location = new Point(1, 510); // Back to its original location
                Toggle_Undo_Button(Enable_Undo);

                Disable_Description();
                Button_Attribute_MouseLeave(null, null); // Don't move this below            
                //Set_Label_Entity_Name_Text();
                //Label_Entity_Name.Location = new Point(31, 238);
            }            
            else
            {
                if (UI_Mode == "Search") { Button_Browse_Folder.Visible = true; }

                UI_Mode = "Backup";
                Zoom_List_View(3);
                Set_UI_Into_Backup_Mode(true);


                List_View_Selection.Items.Clear();
                List_View_Selection.Visible = true;

                Button_Operator.Visible = true;
                Button_Operator.Location = new Point(1, 350);

                if (Text_Box_Description.Visible) { Disable_Description(); }


                Refresh_Backup_Directory();
                Toggle_Undo_Button(false);
          
                // if (Temporal_E.Count() == 1) { Refresh_Backup_Stack(); } // Then auto forward into the one backup dir
                // Otherwise let the user choose              
            }


            // Toggeling Buttons for this mode
            Button_Start_MouseLeave(null, null);                 
            Button_Search_MouseLeave(null, null);
            Button_Operator_MouseLeave(null, null);
            Button_Browse_Folder_MouseLeave(null, null);
        }

        private void Button_Backup_MouseHover(object sender, EventArgs e)
        { Set_Resource_Button(Button_Backup, Properties.Resources.Button_Clock_Lit); }

        private void Button_Backup_MouseLeave(object sender, EventArgs e)
        {   if (UI_Mode == "Backup") { Set_Resource_Button(Button_Backup, Properties.Resources.Button_Clock_Lit); }
            else { Set_Resource_Button(Button_Backup, Properties.Resources.Button_Clock); }
        }





        public string Backup_Time()
        {   try // Setting Timestamp
            {   Last_Backup_Time = DateTime.Now.ToString("yyyy/MM/dd'_'HH");
                Int32.TryParse(DateTime.Now.ToString("mm"), out Last_Backup_Minute); // Getting Minute 

                string Minute = "." + Last_Backup_Minute; // Lol parses with just 1 digit
                if (Last_Backup_Minute < 10) { Minute = ".0" + Last_Backup_Minute; }

                Time_Stamp = Last_Backup_Time + Minute;

                return Time_Stamp;
            } catch {}

            return "";
        }


        public bool Is_Time_To_Backup()
        {   try
            {   Current_Hour = DateTime.Now.ToString("yyyy/MM/dd'_'HH");
                Int32.TryParse(DateTime.Now.ToString("mm"), out Current_Minute); // Getting Minute 
           
                // iConsole(600, 100, Current_Hour + "   Hour   " + Last_Backup_Time + "\nMinute:   " + (Current_Minute - Last_Backup_Minute)); 
                // Different hour means we need to refresh -- Or if more then X min passed since last fetch
                if (Current_Hour != Last_Backup_Time | Current_Minute - Last_Backup_Minute > Fetch_Intervall_Minutes - 1) { return true; }            
            
            } catch { return true; } // Fetching anyways..

            return false;
        }



        //=====================//
        private void Button_Search_Click(object sender, EventArgs e)
        {
            //List<string> Target_Content = Get_Segment_Info("2021.07.12_05.46", "Branches");
            //iConsole(500, 400, "Target is :\n" + string.Join("\n", Target_Content));

            //Collapse_Segment_Infos(Get_Segment_Info("2021.07.12_05.46", "Branches"), "2021.07.12_05.46"); 



           
            // Fuse_Segments("2021.07.12_05.45_Base", "2021.07.12_05.46");

            // Write_Into_Segment("2021.07.12_05.45_Base", "Comments", "Fuck", "Same", true);

            // List<string> The_Backup_Folders = Get_All_Directories(Backup_Path + Backup_Folder, true);

            // Copy_Now(@"C:\Users\Mod\Desktop\Ted\", @"C:\Users\Mod\AppData\Local\Xml_Axe\Backup\Ted\");


            // iConsole(400, 100, Xml_Directory); return;                   
            // iConsole(500, 500, string.Join("\n", The_Backup_Folders)); return; 
            // return;


            if (UI_Mode == "Backup") // Just show the last results
            {         
                // Enter Search Mode: Toggle older searches
                if (At_Top_Level)
                {
                    UI_Mode = "Search";
                    Button_Browse_Folder.Visible = false;

                    // At_Top_Level = true;
                    Button_Backup_MouseLeave(null, null);                   
                    Button_Attribute.Visible = false;

                    Button_Operator.Location = new Point(1, 510);
                    Button_Operator.Visible = false;

                    List_View_Selection.Items.Clear();

                    // Making sure icon is right, as "At_Top_Level" means we move to Search Mode
                    Button_Start_MouseLeave(null, null); 


                    foreach (string Entry in Found_Entities)
                    { List_View_Selection.Items.Add(Entry); }
                    Set_Checker(List_View_Selection, Color.Black);
                }

                else // Collapse the stack of selected Backups into the Base version.
                {                            
                    Temporal_E = Select_List_View_Items(List_View_Selection);
                    Temporal_D = Temporal_E.Count();

                    if (Temporal_E == null | Temporal_D == 0)
                    {   iConsole(400, 200, "\nPlease select 1 or more backups that follow \neach other by Strg + Click, \n" +
                        "In order to merge them into the Base backup. \nYou can either double click the list to select all."); 
                    }
                        
                    // Collapsing the whole History of the selected Backup:
                    else if (Temporal_D == 1) { Collapse_Backup_History(Select_List_View_First(List_View_Selection)); }
            

                    else // Collapsing a stack of multiple selected Backups:
                    {   iDialogue(540, 200, "Merge", "Cancel", "false", "false", "\nDo you wish to merge the " + Temporal_D + " selected backups \ninto the Base backup?");                      
                      
                        if (Caution_Window.Passed_Value_A.Text_Data == "false") { return; } // User Abbort 
                        else { Collapse_Backup_Stack(); }                      
                    }
                }
                        
                return;
            }

            else if (Combo_Box_Type_Filter.Focused) // Search by Parent Node Name.
            {
              
                if (Combo_Box_Type_Filter.Text == "") { return; }
                else if (Combo_Box_Type_Filter.Text == "FighterUnit") { Query_For_Tag("SpaceBehavior", "FIGHTER_LOCOMOTOR"); }
                else
                {   // Caution, Query_For_Entity_Parent() only returns a list of Entity Names, nothing more!            
                    Query_For_Entity_Parent(Combo_Box_Type_Filter.Text);
                }


                // Found_Entities because that is the list used in Query_For_Entity_Parent() & Query_For_Tag()
                if (Found_Entities.Count > 0) 
                { 
                    List_View_Selection.Items.Clear();

                    foreach (string Parent_Tag_Name in Found_Entities)
                    {
                        if (!List_View_Matches(List_View_Selection, Parent_Tag_Name))
                        { List_View_Selection.Items.Add(Parent_Tag_Name); }
                    }
              
                    UI_Mode = "Search";
                    Set_UI_Into_Search_Mode(true);
                    List_View_Selection.Visible = true;
                    Zoom_List_View(3);

                   

                    if (List_View_Selection.Items.Count > 0) { List_View_Selection.Items[0].Selected = true; }
                    List_View_Selection.Focus();

                    
                    Set_Resource_Button(Button_Start, Properties.Resources.Button_Logs_Lit);
                    Set_Checker(List_View_Selection, Color.Black);
                }
                return; // Preventing the search below (that searches for content in Combo_Box_Entity_Name.Text).
            }



            string Entity_Name = Wash_String(Combo_Box_Entity_Name.Text);
            if (Array_Matches(Ignored_Attribute_Values, Combo_Box_Entity_Name.Text)) { return; }


            // Matched in selected XML, so just show that one
            if (UI_Mode != "Search" & Found_In_Xml(Entity_Name)) { return; }


            Clear_Last_Mode(); 
            Set_UI_Into_Search_Mode(false); // Needs to run after Clear_Last_Mode(); 

            if (List_View_Selection.Size.Height > 482) { List_View_Selection.Items.Clear(); }


            IEnumerable<XElement> Instances = null;


            foreach (var Xml in Get_All_Files(Xml_Directory, "xml"))
            {
                try
                {   // ===================== Opening Xml File =====================                            
                    XDocument Xml_File = XDocument.Load(Xml, LoadOptions.PreserveWhitespace);

                    if (Match_Without_Emptyspace(Queried_Attribute, "First_Attribute"))
                    {  Instances =
                          from All_Tags in Xml_File.Root.Descendants()
                          where (string)All_Tags.FirstAttribute == Entity_Name                 
                          select All_Tags;        
                    }
                    else
                    {  Instances =
                          from All_Tags in Xml_File.Root.Descendants()
                          where (string)All_Tags.Attribute(Queried_Attribute) == Entity_Name // Fast Search                    
                          select All_Tags;
                    }


                    if (Instances.Any() & File.Exists(Xml)) { Set_Paths(Xml, false); Combo_Box_Entity_Name.Text = Entity_Name; break; }
                }
                catch { }
            }


            if (!Instances.Any())
            {
                foreach (var Xml in Get_All_Files(Xml_Directory, "xml"))
                {   try
                    {   XDocument Xml_File = XDocument.Load(Xml, LoadOptions.PreserveWhitespace);

                        if (Match_Without_Emptyspace(Queried_Attribute, "First_Attribute"))
                        {   Instances =
                               from All_Tags in Xml_File.Root.Descendants()
                               where Is_Match((string)All_Tags.FirstAttribute, Entity_Name)
                               select All_Tags;
                        }
                        else
                        {   Instances =
                              from All_Tags in Xml_File.Root.Descendants()
                              // Regex; This is damn slow - but it delivers results
                              where Is_Match((string)All_Tags.Attribute(Queried_Attribute), Entity_Name)
                              select All_Tags;
                        }

                        if (Instances.Any() & File.Exists(Xml)) { Set_Paths(Xml); break; } // Don't select Entity_Name here because its spelled wrong 
                    } catch {}
                }
            }

        
            Zoom_List_View(1); // Minimizing again
            Found_In_Xml(Entity_Name); // Just to select the found entity
        }



        private void Button_Search_MouseHover(object sender, EventArgs e)
        {   if (UI_Mode == "Backup" && !At_Top_Level) { Set_Resource_Button(Button_Search, Properties.Resources.Button_Stack_Lit); }
            else { Set_Resource_Button(Button_Search, Properties.Resources.Button_Search_Lit); }
        }

        private void Button_Search_MouseLeave(object sender, EventArgs e)
        {   if (UI_Mode == "Backup" && !At_Top_Level) { Set_Resource_Button(Button_Search, Properties.Resources.Button_Stack); }
            else { Set_Resource_Button(Button_Search, Properties.Resources.Button_Search); }
        }







        private void Button_Attribute_Click(object sender, EventArgs e)
        {
            if (UI_Mode == "Backup")
            {
                Toggle_Undo_Button(Enable_Undo);

                if (At_Top_Level) { return; } // Leave this as it is.

                string Selected_Backup = Select_List_View_First(List_View_Selection);
                string Working_Directory = Backup_Path + Backup_Folder + @"\Current";
                // string Directory_Name = "";

            
                bool Move_Backwards = false;

                foreach (ListViewItem Item in List_View_Selection.Items)
                {
                    if (Item.Text == Selected_Backup) { break; }

                    // Find out whether Current_Backup comes before Selected_Backup
                    else if (Item.Text == Current_Backup) { Move_Backwards = true; break; }                                         
                }


                if (Selected_Backup == null || Selected_Backup == "") { iConsole(400, 100, "\nPlease select any of the backups to restore it."); }

                else if (Current_Backup == Selected_Backup) { iConsole(400, 100, "\nThis should already be the current version."); }

                else if (Current_Backup != "")
                {                  
                    /* // Changed my mind about this option to only check out the files inside of that Backup.
                    iDialogue(580, 240, "Restore All", "Cancel", "Only Inside", "false", "\nDo you wish to restore to the backup " +
                       Selected_Backup + "?\n\nFor all changed files between this backup and the \ncurrent state, or only for the files inside of this backup?"
                        // + Xml_Directory.Replace(Mod_Directory, "") + "?"
                       );


                    if (Caution_Window.Passed_Value_A.Text_Data == "false") { return; } // User Abbort

                    // Create_New_Backup(true); // Check for AutoStash changes


                    // Moving backwards in the history means we better not change the newest version of files that are not inside of the target patch
                    // We also ignore all files outside of this patch if the user decided "else" while moving forward  | Move_Backwards)
                    if (Caution_Window.Passed_Value_A.Text_Data == "else") { Restore(Current_Backup + User_Name, Selected_Backup, Move_Backwards, false); }

                    else { */

  

                    iDialogue(580, 240, "Overwrite", "Cancel", "Stash", "false", "\nDo you wish to Stash new changes (slow)\n" +
                        "or to Overwrite the working directory (fast) with:\n" +
                       Selected_Backup + "?\n\n"              
                    );

                    if (Caution_Window.Passed_Value_A.Text_Data == "false") { return; } // User Abbort
                    else if (Caution_Window.Passed_Value_A.Text_Data == "else") { Create_New_Backup(true, Move_Backwards); }// Check for AutoStash changes


                    Restore(Current_Backup + User_Name, Selected_Backup, Move_Backwards, true); // Current_Backup is allowed to be remaining "" here.                             
                    Current_Backup = Selected_Backup; // Updating Values and button
                    // Button_Attribute_MouseLeave(null, null); 
                }
            }


            else if (Ying_Dominates)
            {
                Ying_Dominates = false; // toggling

                if (Combo_Box_Entity_Name.Text == "First_Attribute") { Label_Entity_Name.Text = "First Attribute"; }
                else { Label_Entity_Name.Text = Combo_Box_Entity_Name.Text; }

                Combo_Box_Entity_Name.Text = Properties.Settings.Default.Entity_Name;

                Reset_Combo_Box_Entity_Name();

                Combo_Box_Entity_Name.ForeColor = Color.Black;
                Combo_Box_Entity_Name.BackColor = Color.LightGray;
            }

            else if (!Ying_Dominates)
            {
                Properties.Settings.Default.Entity_Name = Combo_Box_Entity_Name.Text;
                Properties.Settings.Default.Save();

                Combo_Box_Entity_Name.Items.Clear(); // Special selection type
                Combo_Box_Entity_Name.Items.Add("Name");
                Combo_Box_Entity_Name.Items.Add("First_Attribute");


                Combo_Box_Entity_Name.Text = Properties.Settings.Default.Attribute_Name;                          
                Label_Entity_Name.Text = "";


                Combo_Box_Entity_Name.ForeColor = Color.White;
                Combo_Box_Entity_Name.BackColor = Color.Black;

                // Run after "Label_Entity_Name.Text =" to prevent it from unnecessary re-saving the old value:
                Ying_Dominates = true;
            }


            Button_Attribute_MouseHover(null, null); // Update UI
        }


        private void Button_Attribute_MouseHover(object sender, EventArgs e)
        {
            if (UI_Mode == "Backup") { Set_Resource_Button(Button_Attribute, Properties.Resources.Button_Run_Lit); }
            else 
            { 
                if (Ying_Dominates) { Set_Resource_Button(Button_Attribute, Properties.Resources.Button_Yang_Lit); }
                else if (!Ying_Dominates) { Set_Resource_Button(Button_Attribute, Properties.Resources.Button_Ying_Lit); }
            }
        }

        private void Button_Attribute_MouseLeave(object sender, EventArgs e)
        {
            if (UI_Mode == "Backup") { Set_Resource_Button(Button_Attribute, Properties.Resources.Button_Run); }
            else
            {   if (Ying_Dominates) { Set_Resource_Button(Button_Attribute, Properties.Resources.Button_Yang); }
                else if (!Ying_Dominates) { Set_Resource_Button(Button_Attribute, Properties.Resources.Button_Ying); }
            }
        }

   



        private bool Found_In_Xml(string Entity_Name)
        {
            if (In_Selected_Xml(Entity_Name))
            {
                List_View_Selection.Visible = true;

                if (List_View_Selection.Items.Count > 0) // Auto selecting the item
                {
                    if (List_View_Selection.SelectedItems.Count > 0)
                    { List_View_Selection.SelectedItems[0].Selected = false; }

                    for (int i = List_View_Selection.Items.Count - 1; i >= 0; --i)
                    {   // Selecting everything
                        if (List_View_Selection.Items[i].Text == Entity_Name)
                        {   List_View_Selection.Items[i].Selected = true;
                            List_View_Selection.Focus(); return true;
                        }
                    }
                }
                return false;
            }

            return false;
        }




        private void Button_Percentage_Click(object sender, EventArgs e)
        {
            if (Operation_Mode == "Percent")
            {
                Operation_Mode = "Int";
                User_Input = false; // Un-Percenting
                Combo_Box_Tag_Value.Text = Remove_Operators(Combo_Box_Tag_Value.Text);
                User_Input = true;

                if (Text_Box_Description.Text.StartsWith("While in Percent Mode")) { Text_Box_Description.Visible = false; }
            }
            else
            {   Operation_Mode = "Percent"; // Needs to be set BEFORE Process_Tags()
                Combo_Box_Tag_Value.Text = ""; // Prepare for percent input
                Button_Scripts_MouseLeave(null, null); // Starting or exiting XY mode for Points

                if (sender != null & Match_Setting("Show_Tooltip"))
                {   Text_Box_Description.Visible = true;

                    // Special Tooltip, that describes the Percent Mode                   
                    Text_Box_Description.Text = "While in Percent Mode, Xml Axe gets the original values of the selected tag.\n" +
                    "And it scales them either UP or DOWN by the specified amount of %.\n\n" +
                    "If no certain unit is selected, and no Type Filter is set, this percent balancing will be applied to all entities that posess the selected tag.";                   
                }
            }
        }

        private string Remove_Operators(string The_Text)
        { return The_Text.Replace("+", "").Replace("-", "").Replace("%", ""); }

        private void Button_Percentage_MouseHover(object sender, EventArgs e)
        { Set_Resource_Button(Button_Percentage, Properties.Resources.Button_Percent_Lit); }

        private void Button_Percentage_MouseLeave(object sender, EventArgs e)
        {
            if (Operation_Mode == "Percent") { Set_Resource_Button(Button_Percentage, Properties.Resources.Button_Percent_Lit); }
            else { Set_Resource_Button(Button_Percentage, Properties.Resources.Button_Percent); }
        }




        private void Button_Scripts_Click(object sender, EventArgs e)
        {
            if (UI_Mode == "Script") // Toggle between Script Mode
            {
                Clear_Last_Mode(); 
                Set_UI_Into_Script_Mode(false); // Needs to run after Clear_Last_Mode(); 

                Drop_Zone.Visible = true;
                Zoom_List_View(1);
              

                Button_Browse_Folder.Location = new Point(1, 193);

                // Loading the Xml instead of the available scripts in script mode
                Load_Xml_Content(Properties.Settings.Default.Last_File);
                Set_Resource_Button(Button_Start, Properties.Resources.Button_Logs_Lit);

                if (Text_Box_Description.Visible) { Disable_Description(); }
                Toggle_Undo_Button(Enable_Undo);
            }
      
            else if (Operation_Mode.Contains("Point") | Combo_Box_Tag_Name.Text == "Scale_Galaxies") // Cycle
            {
                if (Scale_Mode == "XY") { Scale_Mode = "X"; }
                else if (Scale_Mode == "X") { Scale_Mode = "Y"; }
                else if (Scale_Mode == "Y") { Scale_Mode = "XY"; }
                Button_Scripts_MouseLeave(null, null);
            }

            else
            {
                UI_Mode = "Script";
                Set_UI_Into_Script_Mode(true);

                Drop_Zone.Visible = false;
                Toggle_Undo_Button(false);
                List_View_Selection.Visible = true;
                Zoom_List_View(3);
               
                Button_Browse_Folder.Location = new Point(1, 430);
                if (Text_Box_Description.Visible) { Disable_Description(); }


                Found_Scripts = Get_All_Files(Script_Directory);
                List<string> Script_Names = new List<string>();

                if (Found_Scripts != null)
                {   try
                    {   foreach (string File_Path in Found_Scripts)
                        {                        
                            if (!File_Path.EndsWith("json") & !File_Path.Contains("Hidden"))
                            {
                                string[] Blacklist = new string[] { "Installer" }; // Match and disqualify by filename
                                
                                // These Python scrips are only meant for EAW mode
                                if (!EAW_Mode) { Blacklist = new string[] { "Installer", "Dat_to_File", "File_to_Dat", "Find_Textures_In_Maps", "Make_Mp_Maps_Default", "Mod_Cleanup"}; }

                                if (!Blacklist.Contains(Path.GetFileNameWithoutExtension(File_Path))) { Script_Names.Add(Path.GetFileName(File_Path)); }
                            }                           
                        }
                        Reset_Script_Box(Script_Names);
                    } catch {}
                }
            }

            
            Button_Browse_Folder_MouseLeave(null, null); // Ordering the Icon to change color
            try { List_View_Selection.Columns[0].Width = List_View_Selection.Width - 8; } catch {}
                
        }


        private string[] Get_Backup_Info(string Info_Path)
        {
            string [] Backup_Info = new string [2]; 
            // Directory_Name = "";

            try
            {   // Info_Path used to be Xml_Directory + "Axe_Info.txt"
                foreach (string Line in File.ReadAllLines(Info_Path))
                {
                    if (Line == "") { continue; }
                    string Content = Remove_Emptyspace_Prefix(Line.Split('=')[1]).Replace("\r\t", "");

                    if (Line.Contains("Directory_Name")) { Backup_Info[0] = Content; }
                    else if (Line.Contains("Version")) { Backup_Info[1] = Content; break; }
                    // Need to break after the 2nd setting was loaded, otherwise it would loop through many files, which isn't necessary.           
                }
            }
            catch
            {   Backup_Info[0] = "None";
                Backup_Info[1] = "None"; 
            }

            return Backup_Info;
        }


        private void Button_Scripts_MouseHover(object sender, EventArgs e)
        {
            if (Operation_Mode.Contains("Point") | Combo_Box_Tag_Name.Text == "Scale_Galaxies")
            {
                if (Scale_Mode == "XY") { Set_Resource_Button(Button_Scripts, Properties.Resources.Button_XY_Lit); }
                else if (Scale_Mode == "X") { Set_Resource_Button(Button_Scripts, Properties.Resources.Button_X_Lit); }
                else if (Scale_Mode == "Y") { Set_Resource_Button(Button_Scripts, Properties.Resources.Button_Y_Lit); }
            }           
            else { Set_Resource_Button(Button_Scripts, Properties.Resources.Button_Flash_Lit); }
        }

        private void Button_Scripts_MouseLeave(object sender, EventArgs e)
        {
            if (UI_Mode == "Script") { Set_Resource_Button(Button_Scripts, Properties.Resources.Button_Flash_Lit); }
            else
            {             
                if (Operation_Mode.Contains("Point") | Combo_Box_Tag_Name.Text == "Scale_Galaxies")
                {
                    if (Scale_Mode == "XY") { Set_Resource_Button(Button_Scripts, Properties.Resources.Button_XY); }
                    else if (Scale_Mode == "X") { Set_Resource_Button(Button_Scripts, Properties.Resources.Button_X); }
                    else if (Scale_Mode == "Y") { Set_Resource_Button(Button_Scripts, Properties.Resources.Button_Y); }
                }
                else { Set_Resource_Button(Button_Scripts, Properties.Resources.Button_Flash); }
            }
        }


        //=====================//

        public bool Collapse_Backup_History(string Selected_Backup = "")
        {           
            string Target_Path = "";
            List<string> Found_Backups = new List<string>();
            List<string> Current_Branch = Get_Backup_Parents(Selected_Backup, true);
            Temporal_D = Current_Branch.Count() -1;

            Temporal_A = "\nThe selected Backup was marked for merging \ninto the Base backup. " +
                         "Please load/checkout any \nother backup by the arrow button to unlock this one.";
            // iConsole(600, 400, string.Join("\n", Current_Branch)); return false;
          


            if (Temporal_D > 4)
            {
                Temporal_C = (Temporal_D * 30) + 200;
                if (Temporal_C > 680) { Temporal_C = 680; }

                iDialogue(540, Temporal_C, "Do It", "Cancel", "false", "false", "\nYou are about to merge these " 
                    + Temporal_D + " Backups.\nAre you sure you wish to proceed?\n\n" + string.Join("\n", Current_Branch));

                if (Caution_Window.Passed_Value_A.Text_Data == "false") { return false; }
            }

            // Collecting all entries from all files and moving down towards the _Base version.
            Collapse_Segment_Infos(Current_Branch, Selected_Backup); // return false;



            Current_Branch.Reverse(); // Important, to paste the versions over each other in chronological order. 





            string Working_Directory = Backup_Path + Backup_Folder + @"\Current";
            if (Directory.Exists(Working_Directory)) { Deleting(Working_Directory); } // Artifact from last usage
            Directory.CreateDirectory(Working_Directory);


          
            try {   
                foreach (string File_Path in Get_All_Directories(Backup_Path + Backup_Folder, true))
                {   foreach (string Entry in Current_Branch)
                    {
                        if (!File_Path.EndsWith(Entry)) { continue; } 
                        else if (File_Path.EndsWith("Current")) { continue; }

                        if (Entry != "" && Entry != " " && !Found_Backups.Contains(Entry)) { Found_Backups.Add(Entry); } // Match
                        else { continue; }
 

                        // Comment out this line out to fall back into "Current" as working dir.
                        if (Entry.EndsWith("Base"))
                        {   Target_Path = File_Path + @"\";


                            try
                            {   // The .txt of the Base directory is already stitched together by Collapse_Segment_Infos() at the top here.
                                File.Copy(File_Path + Backup_Info, Root_Backup_Path, true);
                                // iConsole(400, 100, File_Path + Backup_Info);
                            }
                            catch { Create_Backup_Info(Backup_Folder, Path.GetFileName(File_Path)); } // If failed to find it, we just create a new one.


                            continue; // Ignoring the Base backup, thats our target 
                        } 


                        else if (Entry == Current_Backup) // Stop, we can't Collapse the currently loaded backup
                        { iConsole(550, 180, Temporal_A); return false; } // Can't be Entry.EndsWith("Base")

                  
                        // iConsole(600, 300, "\n" + Entry + " in " + File_Path + "\n\nto\n\n" + Working_Directory);
                        Copy_Now(File_Path, Working_Directory);
                        Deleting(File_Path);                 
                    }
                }

                if (Target_Path != "") // Because dynamic assigning of Working_Directory to Target_Path above, wouldn't work timing wise. 
                {
                    Copy_Now(Working_Directory, Target_Path);
                    Deleting(Working_Directory);
                }
              
                // iConsole(400, 100, Target_Path); return false;



                if (Selected_Backup != "Silent") // Only for auto-merge mode the notification is muted
                {
                    Refresh_Backup_Stack(); // Visualising changes to UI

                    Found_Backups.Reverse(); // Re-reversing to show the correct order to the user.
                    Temporal_C = (Found_Backups.Count() * 30) + 140;
                    if (Temporal_C > 680) { Temporal_C = 680; }
                   
                    // Temporal_C as Line Count
                    iConsole(600, Temporal_C, "\nMerged Backups into Base in the following order:\n\n" + string.Join("\n", Found_Backups));
                }
                return true;

            } catch { iConsole(400, 100, "\nFailed to merge selected Backup branch into Base."); }


            return false;
        }



        //=====================//

        public bool Collapse_Backup_Stack(string Certain_Backup = "")
        {                             
            try
            {   bool Selected_First = false;
                bool Detected_Selection_Gap = false;
                List<string> Backup_Files = new List<string>();
                List<string> Found_Backups = new List<string>();
                string Target_Path = "";
                string Working_Directory = Backup_Path + Backup_Folder + @"\Current\";
           
              
                Temporal_A = "\nThe selected Backup was marked for merging \ninto the Base backup. " +
                             "Please load/checkout any \nother backup by the arrow button to unlock this one.";


                if (Certain_Backup != "")
                {
                    if (UI_Mode == "Backup") 
                    {   Found_Backups = Get_All_List_View_Items(List_View_Selection);
                        Found_Backups.Reverse(); // Because the for loop below expects the reversed format from List_View_Selection in Backup Mode                   
                    }
                    else { Found_Backups = Get_Backup_Dirs(); }
                 

                    for (int i = Found_Backups.Count - 1; i >= 1; i--)
                    {
                        string Entry = Found_Backups[i];


                        if (Entry.EndsWith("Base")) { Backup_Files.Add(Entry); } // Grab the bottom most Base
                        else // if (!Entry.EndsWith("Base")) 
                        {
                            // Stop, we can't Collapse the currently loaded backup
                            if (Entry == Current_Backup && Certain_Backup != "Silent") { iConsole(550, 180, Temporal_A); return false; }

                            Backup_Files.Add(Entry); break; // Bottom most Backup                        
                        }                                                                           
                    }
                    // iConsole(400, 100, string.Join("\n", Backup_Files));
                }

                else // This part is always called within the Backup (management) mode:
                {                  
                    foreach (ListViewItem Item in List_View_Selection.Items)
                    {   if (Item.Text != "Current" && Item.Selected | Item.Text.EndsWith("Base"))
                        {
                            if (Detected_Selection_Gap) { iConsole(400, 100, "\nYou need to select all targeted collumns in a row \notherwise that would break the right sync order."); return false; }

                            if (Item.Text.EndsWith("Base")) 
                            {   // Done when we hit the first Backup with _Base extention, if anything else was "Selected_First"
                                if (Selected_First) { Backup_Files.Add(Item.Text); break; } // Don't place the if statement above
                                else { iConsole(600, 160, "\nI can't merge a Base version into a older Backup. \nYou need to select backups OVER the Base version \nin order to merge them down the stack."); return false; }
                            }
                            else // if (!Item.Text.EndsWith("Base")) 
                            {   // Stop, we can't Collapse the currently loaded backup
                                if (Item.Text == Current_Backup) { iConsole(550, 180, Temporal_A); return false; }
                                Backup_Files.Add(Item.Text); Selected_First = true; 
                            }
                        
                        } else if (Selected_First) { Detected_Selection_Gap = true; } // Ignoring all gaps until the first is selected                                
                    }


                    Backup_Files.Reverse(); // Important, to paste the versions over each other in chronological order.                   
                }


                if (Directory.Exists(Working_Directory)) { Deleting(Working_Directory); } // Artifact from last usage
                Directory.CreateDirectory(Working_Directory); // If it hasn't returned so far, we are going to need this



                foreach (string File_Path in Get_All_Directories(Backup_Path + Backup_Folder, true))
                {   foreach (string Entry in Backup_Files)
                    {
                        if (!File_Path.EndsWith(Entry)) { continue;}
                        if (Entry.EndsWith("Base")) { Target_Path = File_Path + @"\"; continue; } // Ignoring the Backup, thats our target 


                        // iConsole(600, 300, Entry + " in " + File_Path + "\n\nto\n\n" + Working_Directory);                   
                        Copy_Now(File_Path, Working_Directory);                            
                        Deleting(File_Path);                      
                    }
                }


                if (Target_Path != "") // Because dynamic assigning of Working_Directory to Target_Path above, wouldn't work timing wise. 
                {   Copy_Now(Working_Directory, Target_Path);
                    Deleting(Working_Directory);
                }


                if (Certain_Backup != "Silent") // Only for auto-merge mode the notification is muted
                {
                    Refresh_Backup_Stack(); // Visualising changes to UI

                    Backup_Files.Reverse(); // Re-reversing to show the correct order to the user.
                    Temporal_C = (Backup_Files.Count() * 30) + 140;
                    if (Temporal_C > 680) { Temporal_C = 680; }
                   
                    // Temporal_C as Line Count
                    iConsole(600, Temporal_C, "\nMerged Backups into Base in the following order:\n\n" + string.Join("\n", Backup_Files));
                }


                return true;

            } catch { iConsole(400, 100, "Failed to merge selected Backups into Base."); }

            return false;
        }




        public void Load_Changes(string Backup_Name, string Working_Directory, string Segment_Name, bool Move_Backwards)
        {
            string Noted_Path = "";
            List<string> Managed_Files = new List<string>();
            List<string> Backups = Get_All_Directories(Backup_Path + Backup_Folder, true);

            if (Debug_Mode) { Managed_Files.Add("Targeting " + Backup_Name + "\n"); }
            if (Move_Backwards) { Backups.Reverse(); }


            try { 
         
                // Deliberately NOT using "Any" as backupname here because we need to filter out only the selected backups!
                foreach (string Noted_File in Get_Segment_Info(Backup_Name, Segment_Name, false, false))
                {

                    Noted_Path = "";
                    bool Found_File = false;

                    // foreach (string Found_Dir in Backups)
                    for (int i = 0; i < Backups.Count(); i++)
                    {
                        Noted_Path = Backups[i] + @"\" + Noted_File;
                        if (File.Exists(Noted_Path)) { Found_File = true; break; } // Assigned the right path
                    }




                    if (Segment_Name == "Removed_Files")
                    {
                        Deleting(Sync_Path + Noted_File);
                        Deleting(Working_Directory + @"\" + Noted_File);
                        if (!Dont_Copy.Contains(Noted_File)) { Dont_Copy.Add(Noted_File); }
                        if (Debug_Mode) { Managed_Files.Add("Delete  " + Sync_Path + Noted_File); }
                    }
                    else if (Segment_Name == "Added_Files")
                    {
                        File.Copy(Noted_Path, Working_Directory + @"\" + Noted_File, true);             
                        if (Debug_Mode) { Managed_Files.Add("Create  " + Noted_File); }
                    }                                                                
                }

                // if (Debug_Mode && Managed_Files.Count() > 1) { iConsole(400, 600, "\n" + string.Join("\n", Managed_Files)); }
            } catch {}
        }

        

        public void Revert_Changes(string Backup_Name, string Working_Directory, string Segment_Name, bool Move_Backwards)
        {
            string Noted_Path = "";
            List<string> Managed_Files = new List<string>();
            List<string> Backups = Get_All_Directories(Backup_Path + Backup_Folder, true);

            if (Debug_Mode) { Managed_Files.Add("Targeting " + Backup_Name + "\n"); }
            if (Move_Backwards) { Backups.Reverse(); }


            try 
            {
                // Deliberately NOT using "Any" as backupname here because we need to filter out only the selected backups!
                foreach (string Noted_File in Get_Segment_Info(Backup_Name, Segment_Name, false, false))
                {

                    Noted_Path = "";
                    bool Found_File = false;                

                    // foreach (string Found_Dir in Backups)
                    for (int i = 0; i < Backups.Count(); i++)               
                    {
                        Noted_Path = Backups[i] + @"\" + Noted_File;
                        if (File.Exists(Noted_Path)) { Found_File = true; break; } // Assigned the right path
                    }
     
          


                    if (Segment_Name == "Removed_Files")
                    {
                        // Means restore it from the newest possible backup
                        if (Move_Backwards && Found_File && !Dont_Copy.Contains(Noted_File))  // Deletion_File is a full path here, contrary to below!   
                        { 
                            File.Copy(Noted_Path, Working_Directory + @"\" + Noted_File, true);
                            // File.WriteAllText(Working_Directory + @"\" + Noted_File, "Backup is \n" + Backup_Name); // Just for debugging the timing
                               
                            // if (Dont_Copy.Contains(Noted_File)) { Dont_Copy.Remove(Noted_File); } // Revert that command
                            if (Debug_Mode) { Managed_Files.Add("Create  " + Noted_Path); }
                        }
                                            
                        // CAUTION, !Move_Backwards will cause Get_Segment_Info() to return only file names without path here - which is a different meaning.
                        else
                        {
                            Deleting(Sync_Path + Noted_File);
                            Deleting(Working_Directory + @"\" + Noted_File);
                            if (!Dont_Copy.Contains(Noted_File)) { Dont_Copy.Add(Noted_File); }
                            if (Debug_Mode) { Managed_Files.Add("Delete  " + Sync_Path + Noted_File); }                       
                        }                     
                    }


                    else if (Segment_Name == "Added_Files") // Then reverse the order
                    {
                        if (Move_Backwards) 
                        {
                            Deleting(Sync_Path + Noted_File);
                            Deleting(Working_Directory + @"\" + Noted_File);
                            if (!Dont_Copy.Contains(Noted_File)) { Dont_Copy.Add(Noted_File); }
                            if (Debug_Mode) { Managed_Files.Add("Delete  " + Sync_Path + Noted_File); }                          
                        }
                        else if (Found_File && !Dont_Copy.Contains(Noted_File)) 
                        { 
                            File.Copy(Noted_Path, Working_Directory + @"\" + Noted_File, true);
                            // File.WriteAllText(Working_Directory + @"\" + Noted_File, "Backup is \n" + Backup_Name); // Just for debugging the timing
                              
                            // if (Dont_Copy.Contains(Noted_File)) { Dont_Copy.Remove(Noted_File); }
                            if (Debug_Mode) { Managed_Files.Add("Create  " + Noted_File); }
                        }             
                    }                           
                }

                // if (Debug_Mode && Managed_Files.Count() > 1) { iConsole(400, 600, "\n" + string.Join("\n", Managed_Files)); }

            } catch {}
        }




        // All_Files_Since_This means all files in other Backups then this one, that are between the Current_Version and the Target_Backup.
        public void Restore(string Current_Version, string Target_Backup, bool Move_Backwards, bool All_Files_Since_This = true, bool Ignore_Branches = false, bool Set_Selected = true)
        {   // string Current = Select_List_View_First(List_View_Selection);

            if (Current_Version == Target_Backup) 
            { iConsole(400, 160, "\n" + Current_Version + " is already the loaded version."); return; } // Nothing to do


            try {

                string Working_Directory = Backup_Path + Backup_Folder + @"\Current";

                Dont_Copy = new List<string>(); // Clear
                List<string> Backups = Get_All_Directories(Backup_Path + Backup_Folder, true);
                List<string> Backup_Files = new List<string>();

             


                foreach(string Entry in Backups)
                {
                    if (Entry.EndsWith("Current")) { Backups.Remove(Entry); break; }
                }


                if (Move_Backwards) { Backups.Reverse(); }
                int Start_At = 0;
                int Passed_Slots = 0;
                



                foreach (string Found_Dir in Backups)
                {
                    if (All_Files_Since_This | Found_Dir.EndsWith(Target_Backup)) // Then its the selected directory
                    {
                        foreach (string File_Path in Get_All_Files(Found_Dir))
                        {   // Collecting all files inside for later.
                            string Selected_File = File_Path.Replace(Found_Dir, "");
                            if (!Backup_Files.Contains(Selected_File)) { Backup_Files.Add(Selected_File); }
                        }                
                    }

                    // break; // Stop, or the Passed_Slots will continue incrementing even after we found this folder.                       
                    if (Found_Dir.EndsWith(Target_Backup)) { break; }
                    else { Passed_Slots++; } // How many slots are passing until we find the selected dir?  

                    if (Found_Dir.EndsWith(Current_Version))
                    { Start_At = Passed_Slots; } // To leap over unnecessary ones
                }
                // iConsole(500, 100, string.Join("\n", Backup_Files));






                if (Directory.Exists(Working_Directory)) { Deleting(Working_Directory); } // Artifact from last usage
                Directory.CreateDirectory(Working_Directory);

                int Cycles = 0;
                List<string> Stack_History = new List<string>();
               
                List<string> Current_Branch = new List<string>();

                Current_Branch.Add(Current_Version); // To prevent it from not being executed below
             

                if (Move_Backwards) 
                {   foreach (string Backup in Get_Segment_Info(Current_Version, "Branches"))
                    {
                        if (!Current_Branch.Contains(Backup)) { Current_Branch.Add(Backup); }
                    }
                } 

              


                // ============ Invert existence of the added or removed files ============ 
                for (int i = Start_At; i <= Passed_Slots; i++)
                {   
                    try 
                    {   // Path.GetFileName(Backups[i - 1] might cause crashes..

                        bool Matched_Same_Branch = false;
                        string Backup_Name = Path.GetFileName(Backups[i]);

                        if (!Ignore_Branches && Move_Backwards)
                        {  // Need the shift of -1 slot because the removed files shall be re-added once we pass this Backup to the next oldest one, not instantly.                      
                            Backup_Name = Path.GetFileName(Backups[i - 1]); 
                            // Skip if not part of this branch  
                            if (!List_Matches(Current_Branch, Backup_Name)) { Stack_History.Add(Backup_Name + "   skipped (wrong branch)"); continue; }                      
                        }
                        else if (!Ignore_Branches)
                        {   // string Target_Backup = Path.GetFileName(Backups[Passed_Slots]); // Is already received as parameter of this function.

                            // If the ancestors in the branch of Target Backup matches the the Backup_Name || or if this backup is Target_Backup itself
                            if (List_Matches(Get_Segment_Info(Target_Backup, "Branches"), Backup_Name) || Target_Backup == Backup_Name) { Matched_Same_Branch = true; }
                            if (!Matched_Same_Branch) { Stack_History.Add(Backup_Name + "   skipped (wrong branch)"); continue; }                                               
                        }



                   
                        if (All_Files_Since_This) // Remove deletion files from all backups between top most backup and the bottom one.
                        {
                            Revert_Changes(Backup_Name, Working_Directory, "Added_Files", Move_Backwards);
                            Revert_Changes(Backup_Name, Working_Directory, "Removed_Files", Move_Backwards);

                            Stack_History.Add(Backup_Name); // Because this backup passed the branch check above.

                        }

                        // When reached the bottom-most slot, this is 1 slot before the Target_Backup.
                        if (!All_Files_Since_This || Move_Backwards && i == Passed_Slots)
                        {
                            // Instead of using Revert_Changes() like all backups on the way, the last one does Load_Changes() instead
                            Load_Changes(Target_Backup, Working_Directory, "Added_Files", Move_Backwards);
                            Load_Changes(Target_Backup, Working_Directory, "Removed_Files", Move_Backwards);
                            // if (Debug_Mode) { iConsole(400, 100, "Bottom most is " + i + " with " + Target_Backup); }

                            Stack_History.Add(Target_Backup);

                        }


                    } catch { iConsole(400, 100, "Reverting functions crashed."); }             
                }





                // ============ Invert existence of the added or removed files ============ 
                // if (Debug_Mode) { iConsole(400, 600, "\nDont Copy \n\n" + string.Join("\n", Dont_Copy)); }


                List<string> Execution_Order = new List<string>();

                // Don't place this into the loop above! Because this needs to see all collected files in the list "Dont_Copy" when the first loop finishes.
                for (int i = Start_At; i <= Passed_Slots; i++) 
                {   Cycles++;

                    Execution_Order.Add(Backups[i]);


                    foreach (string File_Path in Backup_Files)
                    {   try 
                        {
                            string Target_Directory = Path.GetDirectoryName(Working_Directory + File_Path);
                            if (!Directory.Exists(Target_Directory)) { Directory.CreateDirectory(Target_Directory); }

                        

                            string Selected_Path = Backups[i] + File_Path; // The file path inside of older or newer backups!

                            if (File.Exists(Selected_Path) && !Dont_Copy.Contains(Path.GetFileName(File_Path)))
                            {
                                // Overwrite Copy the last iteration inside of the Working Directory!
                                File.Copy(Selected_Path, Working_Directory + File_Path, true);

                                // File.WriteAllText(Working_Directory + File_Path, "Cycle is " + i + "   in   \n" + Path.GetFileName(Backups[i])); // Just for debugging the timing
                                // if (Debug_Mode) { iConsole(600, 100, Selected_File + "   To   " + Working_Directory + File_Path); }
                            }
                        } catch {}
                    }
                }

                // if (Debug_Mode) { iConsole(400, 600, "\nExecution Order: \n\n" + string.Join("\n", Execution_Order)); }






                string Update = "UPDATED";

                string Intruduction = "\nMoved up the Backup stack in Historical order.";
                if (Move_Backwards) { Update = "DOWNGRADED";  Intruduction = "\nMoved down the Backup stack in Historical order."; }

                if (All_Files_Since_This) { Intruduction += "\nAnd " + Update + " changes in all files inbetween,\nthat are not part of the target backup.\n\n"; }
                else { Intruduction += "\nWithout changing any files inbetween, \nthat are not part of the target backup.\n\n"; }


                Temporal_C = (Stack_History.Count() * 30) + 190;
                if (Temporal_C > 680) { Temporal_C = 680; }


                // Silent mode if Ignore_Branches
                if (!Ignore_Branches) { iConsole(560, Temporal_C, Intruduction + string.Join("\n", Stack_History)); }


                //string s = "";
                //if (Cycles > 1) { s = "s"; }
                //iConsole(500, 100, "\nJumped by " + Cycles + " slot" + s + "."); // Confirming the right amount of directories to pass


                // Copying the Backup info of the selected backup up into the root dir, this is from where the program stores which is selected.               
                if (Set_Selected)
                {   try
                    {   // This always sets the target Backup as active one
                        File.Copy(Selected_Backup_Path + Target_Backup + Backup_Info, Root_Backup_Path, true);
                    }
                    catch { Create_Backup_Info(Backup_Folder, Target_Backup); } // If failed to find it, we just create a new one.
                }



                User_Input = false; // Disabling Set_Backup_Checker() intervention inside of List_View_Selection_SelectedIndexChanged()


                // Label_Entity_Name.Text = Target_Backup; // Updating UI Info (outdated)
                Set_Checker(List_View_Selection, Theme_Color); // Erasing last selection

                foreach (ListViewItem Item in List_View_Selection.Items)
                {
                    if (Item.Text == Target_Backup)
                    {
                        Item.BackColor = Color.Orange;
                        Item.Selected = false; // So it can be seen in orange
                        break;
                    } // Highlighting Selection
                }
         

                // iConsole(600, 100, Working_Directory + "   To   " + Sync_Path + "This");
                if (Directory.Exists(Working_Directory))
                {   Copy_Now(Working_Directory, Sync_Path);
                    Deleting(Working_Directory);             
                }


                User_Input = true;

             } catch { iConsole(400, 100, "\nRestore function has crashed."); }         
        }




        // Use "Time_Stamp" as argument for Target_Backup_Name
        private void Create_Backup_Info(string Directory_Name, string Target_Backup_Name, string Comments = "", string Changed_Files = "", bool Load_Backup = true, bool Create_Branches = false, string Added_Files = "", string Removed_Files = "")
        {  
            // Package_Name variable is updated by Button_Run_Click() or Create_New_Backup()        
            string This_Backup_Path = Backup_Path + Directory_Name + @"\" + Target_Backup_Name;

            string This_Backup_Info = This_Backup_Path + Backup_Info;
            string Root_Backup_Info = Backup_Path + Directory_Name + Backup_Info;

            string Info_File = "";
            


            try
            {   // Make sure there is a backup dir      
                if (!Directory.Exists(This_Backup_Path)) { Directory.CreateDirectory(This_Backup_Path); }
                
                
                Info_File =
                @"Directory_Name = " + Sync_Path.Remove(Sync_Path.Length - 1) +
                "\nVersion = " + Target_Backup_Name + "\n\n\n";


                if (Comments != "")
                {
                    Info_File +=
                        "//============================================================\\\\" +
                        "\nComments" +
                        "\n//============================================================\\\\" +
                        "\n" + Comments + "\n\n\n"; // Append_File_Info can be a joined List<string> of file names here.                 
                }   

                if (Changed_Files != "")
                {
                    Info_File +=
                        "//============================================================\\\\" +
                        "\nChanged_Files" +
                        "\n//============================================================\\\\" +
                        "\n" + Changed_Files + "\n\n\n"; // Append_File_Info can be a joined List<string> of file names here.                 
                }           

                if (Added_Files != "")
                {
                    Info_File +=
                        "//============================================================\\\\" +
                        "\nAdded_Files" +
                        "\n//============================================================\\\\" +
                        "\n" + Added_Files + "\n\n\n";                 
                }

                if (Removed_Files != "")
                {
                    Info_File +=
                        "//============================================================\\\\" +
                        "\nRemoved_Files" +
                        "\n//============================================================\\\\" +
                        "\n" + Removed_Files + "\n\n\n";                 
                }


                if (Create_Branches && Current_Backup != "None") try
                {
                    Info_File +=
                       "//============================================================\\\\" +
                       "\nBranches" +
                       "\n//============================================================\\\\" +
                       "\n" + Current_Backup;


                    Temporal_E.Clear();
                    Temporal_E = Get_Segment_Info(Current_Backup, "Branches");

                    // Inherit Branches from the Parents Axe_Info.txt
                    if (Temporal_E.Count() > 0) { Info_File += "\n" + string.Join("\n", Temporal_E) + "\n\n\n"; }
                 


                    // File.WriteAllText(Own_Branch, Info_File);
                } catch { iConsole(600, 200, "\nFailed to create Branch info."); }



                File.WriteAllText(This_Backup_Info, Info_File);

            } catch { iConsole(600, 200, "\nFailed to create or find the path for Axe_Info.txt."); }



        

            try
            {   // Adding a Copy into root directory for Backups, that marks this backup as the loaded one. 
                // CAUTION, Directory_Name means the different Folders in Backup_Path!
                if (Load_Backup) { File.Copy(This_Backup_Info, Root_Backup_Info, true); }

            } catch { iConsole(600, 200, "\nFailed to create or find the path: \n" + This_Backup_Info); } 
        }





        private void Zoom_List_View(int List_Size)
        {
            if (List_Size == 1)
            {
                Drop_Zone.Visible = true;
                List_View_Selection.Size = new Size(this.Size.Width - 40, 164);
                List_View_Selection.Location = new Point(12, 12);
            }
            else if (List_Size == 2)
            {
                Drop_Zone.Visible = false; // Hiding Background                          
                List_View_Selection.Size = new Size(this.Size.Width - 76, 398);
                List_View_Selection.Location = new Point(31, 194);

                Text_Box_Description.Text = ""; // Clear
                Text_Box_Description.Visible = true;
            }
            else if (List_Size == 3)
            {
                Drop_Zone.Visible = false;
                List_View_Selection.Size = new Size(this.Size.Width - 76, 562);
                List_View_Selection.Location = new Point(31, 29);
            }

            List_View_Selection.Columns[0].Width = List_View_Selection.Size.Width - 8;
        }



        //=====================// 

        private void Clear_Last_Mode()
        {
            if (UI_Mode == "Normal") { return; }

            switch (UI_Mode)
            {
                case "Settings":
                    Set_UI_Into_Settings_Mode(false);
                break;

                case "Backup":
                    Set_UI_Into_Backup_Mode(false);
                break;     
           
                case "Script":
                    Set_UI_Into_Script_Mode(false);
                break;

                case "Search":
                    Set_UI_Into_Search_Mode(false);
                break; 

                // default: 
                // break;
            }

            UI_Mode = "Normal";
        }


        //=====================// 

        private void Set_UI_Into_Settings_Mode(bool Mode)
        {   Control[] Controls = { Button_Search, Button_Run, Button_Backup, Button_Attribute, Button_Percentage, Button_Scripts, Button_Operator, Label_Type_Filter };
            foreach (Control Selectrion in Controls) { Selectrion.Visible = !Mode; } // Hide or show all        
        }

        private void Set_UI_Into_Backup_Mode(bool Mode)
        {   Control[] Controls = { Button_Run, Button_Attribute, Button_Scripts, Button_Percentage, Button_Toggle_Settings };
            foreach (Control Selectrion in Controls) { Selectrion.Visible = !Mode; } // Hide or show all        
        }

        private void Set_UI_Into_Script_Mode(bool Mode)
        {   Control[] Controls = { Button_Start, Button_Run, Button_Backup, Button_Search, Button_Attribute, Button_Percentage, Button_Operator, Button_Toggle_Settings };
            foreach (Control Selectrion in Controls) { Selectrion.Visible = !Mode; } // Hide or show all        
        }

        private void Set_UI_Into_Search_Mode(bool Mode)
        {   Control[] Controls = { Button_Browse_Folder, Button_Run, Button_Attribute, Button_Scripts, Button_Percentage, Button_Operator, Button_Toggle_Settings };
            foreach (Control Selectrion in Controls) { Selectrion.Visible = !Mode; } // Hide or show all        
        }


        //=====================// 

        private void Run_Script()
        {
            string Selection = Select_List_View_First(List_View_Selection);


            if (Found_Scripts != null && Selection != null && Selection != "")
            {   foreach (string File_Path in Found_Scripts)
                {   if (File_Path.EndsWith(Selection)) // If is the selected file               
                    {

                        // Piping arguments into the script code below makes only sense in EAW Mode 
                        if (!EAW_Mode) { Execute(File_Path, "", Script_Directory); return; } 


                        string[] Possible_Files = new string[] { "Dat_to_File.py", "File_to_Dat.py" }; // Match by Filename

                        foreach (string File_Name in Possible_Files)
                        {
                            if (Match_Without_Emptyspace_2(Selection, File_Name))                              
                            {
                                string Extension = "txt";
                                if (Text_Format_Delimiter == "\"\t\"") { Extension = "tsv"; }                             
                                else if (Text_Format_Delimiter == "," | Text_Format_Delimiter == ";") { Extension = "csv"; }

                                Execute(File_Path, "\"" + Mod_Directory + "\\Data\" " + Text_Format_Delimiter + " " + Extension, Script_Directory);                               

                                // Alternate way: Specify path to the .json file as argument:
                                // Execute(File_Path, "\"" + Mod_Directory + "\\Data\"" + " " + Script_Directory + "\\vanilla_dump.json");

                                // iConsole(400, 100, File_Path + " \"" + Mod_Directory + "\\Data\"" + "\npause");  
                                return;                            
                            }
                        }
                        
                    
                                                                            


                        string Vanilla_Dir = Path.GetDirectoryName(Properties.Settings.Default.Steam_Exe_Path) + @"\steamapps\common\Star Wars Empire at War\Data";
                        Execute(File_Path, "\"" + Mod_Directory + "\\Data\" \"" + Vanilla_Dir + "\"", Script_Directory);

                        Possible_Files = new string[] { "Mod_Cleanup.py", "01_mod_cleanup.py", "Test.py" };

                        foreach (string File_Name in Possible_Files)
                        {
                            if (Match_Without_Emptyspace_2(Selection, File_Name)) // Append the Modpath to the Cleanup Script                             
                            {

                                Temporal_E = File.ReadAllLines(Properties.Settings.Default.Mod_Directory + @"\Data\cleanup.txt").ToList();
                                Temporal_C = (Temporal_E.Count() * 30) + 160;
                                if (Temporal_C > 680) { Temporal_C = 680; }

                                iDialogue(680, Temporal_C, "Yes, All", "Textures", "Models only", "Cancel", "Do you wish to move these into a \"Unused\" folder:", Temporal_E);

                                if (Caution_Window.Passed_Value_A.Text_Data == "true") { Move_Unused_Files(); }
                                else if (Caution_Window.Passed_Value_A.Text_Data == "false") { Move_Unused_Files("textures"); }
                                else if (Caution_Window.Passed_Value_A.Text_Data == "else") { Move_Unused_Files("models"); }
                                // else if (Caution_Window.Passed_Value_A.Text_Data == "other") { } // Does nothing


                                Temporal_E = new List<string>(); // reset
                            }
                        }

                        return;

                    }
                }
            } return; // Because it isn't supposed to process xml stuff of below in Script_Mode.         
        }


        private string Get_Program_Dir(bool check_string = false)
        {
            string New_Program_Dir = Get_Setting_Value("Program_Directory");
            string App_Data = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            if (check_string)
            {
                Properties.Settings.Default.Tags = Properties.Settings.Default.Tags.Replace(Program_Directory, New_Program_Dir);
                Properties.Settings.Default.Save();

                Tag_List = Properties.Settings.Default.Tags;
                Text_Box_Tags.Text = Tag_List;
            }   



            if (New_Program_Dir == "") { New_Program_Dir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Xml_Axe\"; }
            else if (New_Program_Dir.Contains(@"%AppData%\Local")) { New_Program_Dir = New_Program_Dir.Replace(@"%AppData%\Local", App_Data); }         
            else if (New_Program_Dir.Contains("%AppData%")) { New_Program_Dir = New_Program_Dir.Replace("%AppData%", App_Data); }

            if (New_Program_Dir[0] == ' ') { New_Program_Dir = Remove_Emptyspace_Prefix(New_Program_Dir); } // Removing empty space
            if (!New_Program_Dir.EndsWith("\\")) { New_Program_Dir += "\\"; }
                   
            return New_Program_Dir;
        }



        // ============================================= 
        // Check files in the Main_Directory for changes that are not contained in any of the Backups:
        // ============================================= 

        private void Create_New_Backup(bool Is_Auto_Stash = false, bool Move_Backwards = false)
        {
            if (Is_Time_To_Backup()) { Backup_Time(); }

            iConsole(400, 100, "Test: Creating New Backup");

            Package_Name = Time_Stamp + User_Name;
         


            List<string> The_Backup_Folders = Get_All_Directories(Backup_Path + Backup_Folder, true);
            // Reversing "The_Backup_Folders" controlls whether we check all Backup dirs from top to bottom or from bottom to top,
            // The order they are feeded into Check_Files() below allows to remove old filesize missmatches in Difference_List() to be nullified by newer matches.
            // And also in order to stay synchronous with the UI view.
            if (Is_Auto_Stash)
            {   Package_Name += "_Stash";

                // Move order is VERY important for Auto Stash (whether or not Move_Backwards);
                // To prevent confusion of changes from the past with new changes, we stack the Backups in inverted order! 
                if (!Move_Backwards) { The_Backup_Folders.Reverse(); }
            }


            if (The_Backup_Folders.Count() == 0)
            {   // Just copy all files as innitial backup, we are going to need them later on to compare filesizes against this backup!
                Temporal_A = "";
                Temporal_A = Selected_Backup_Path + Package_Name + @"_Base"; // Target Directory
                if (!Directory.Exists(Temporal_A)) { Directory.CreateDirectory(Temporal_A); }


                Copy_Now(Path.GetDirectoryName(Sync_Path), Temporal_A);
                // iConsole(600, 400, Path.GetDirectoryName(Sync_Path) + "\n\n into \n\n" + Temporal_A);

                Temporal_B = "\nCreated a backup of the whole directory into  \n" +
                    Backup_Path + "\n" + Backup_Folder + @"\" + Package_Name + @"_Base" +
                    "\n\n  We are going to need it as \"Base\" later on, to \ncompare filesizes against this innitial backup.\n";
                
                iConsole(400, 260, Temporal_B); // Tell it to the user.


                // Base version to true, otherwise it will complain about a missmatched path
                Create_Backup_Info(Backup_Folder, Package_Name + @"_Base", "\n" + Temporal_B.Replace("\n", ""), "", true, true);
                Refresh_Backup_Stack(); 
                
                Temporal_B = "";
                return;
            }



            

            // List<string> Backup_File_List = new List<string>();
            List<string> Main_Directory = Get_All_Files(Sync_Path);
            List<string> Matches_List = new List<string>();
           
            List<string> Added_File_Paths = new List<string>();
            List<string> Added_Files = new List<string>();
            List<string> Missing_Files = new List<string>();
            List<string> Changed_Files = new List<string>();
           

            Registered_Files = new List<string>(); // Clear global Variables
            Not_Matched_Yet = new List<string>();


            foreach (string Found_Dir in The_Backup_Folders)
            {
                // Backup_File_List = Get_All_Files(Found_Dir);
                Matches_List = Check_Files(Main_Directory, Get_All_Files(Found_Dir), false, true);


                if (Found_Dir == The_Backup_Folders[0]) { Difference_List = Matches_List; } // Just dump the whole path list into the other one as its empty.
                else
                {
                    foreach (string File_Path in Matches_List)
                    {
                        if (!Difference_List.Contains(File_Path)) { Difference_List.Add(File_Path); }
                    }               
                }                         
            }

            

            foreach (string Possability in Not_Matched_Yet)
            {
                if (!List_Matches_Filename(Registered_Files, Possability, true))
                { Added_File_Paths.Add(Possability); }// Detected new file!                               
            }


            

            List<string> Older_Missing_Files = Get_Segment_Info("Any", "Removed_Files", true);

         
            foreach (string Register in Registered_Files)
            { 
                // + "\n" because these are full paths and the eye should be able to tell where the next path beginns
                if (!List_Matches_Filename(Main_Directory, Register, true)) // Detected a removed file 
                {
                    string Register_Name = Path.GetFileName(Register);

                    // Add to list, if this file is not already mentioned in any older patch
                    // if (!List_Matches_Filename(Older_Missing_Files, Register, true)) { Missing_Files.Add(Register); } // Log the full path
                    if (!Older_Missing_Files.Contains(Register_Name)) { Missing_Files.Add(Register_Name); }
                }               
            }

            // iConsole(400, 600, string.Join("\n", Older_Missing_Files));




            if (Difference_List.Count() == 0 && Added_File_Paths.Count() == 0 && Missing_Files.Count() == 0) 
            {
                if (!Is_Auto_Stash) { iConsole(200, 100, "\nNo file changes detected."); }
                return;
            }
            // else { iConsole(400, 400, string.Join("\n", Difference_List)); } // Beware that Joining the list also modifies it, maybe even clears it..                
            // =============================================    
            // return;



            if (Difference_List.Count() > 0)
            {   foreach (string Entry in Difference_List)
                {
                    Temporal_B = Entry.Replace(Sync_Path, "");

                    Changed_Files.Add(Temporal_B); // Gonna use this in the Info report below
                    Verify_Copy(Entry, Selected_Backup_Path + Package_Name + @"\" + Temporal_B);
                }
            }

            if (Added_File_Paths.Count() > 0)
            {
                foreach (string Entry in Added_File_Paths)
                {
                    Temporal_B = Entry.Replace(Sync_Path, "");

                    Added_Files.Add(Temporal_B);
                    Verify_Copy(Entry, Selected_Backup_Path + Package_Name + @"\" + Temporal_B);
                }
            }



            bool Has_Collapsed = Collapse_Oldest_Backup("Last");


            Temporal_A = "";
            string Changes = string.Join("\n", Changed_Files);

            if (Is_Auto_Stash) { Temporal_A = Changes; } // Auto Stash has Changes as copy in the Coment




            Create_Backup_Info(Backup_Folder, Package_Name, Temporal_A, Changes, true, true,  // "Created a backup, based on different file sizes from:\n\n\n" +                
                string.Join("\n", Added_Files), string.Join("\n", Missing_Files)); // "Load_Backup" was Has_Collapsed

            // Use "\n\n" for additional new lines between the entries.

            Refresh_Backup_Stack();      
        }


        //=====================//
        public bool List_Matches_Filename(List<string> Path_List, string File_Name, bool File_Name_Is_Path = false)
        {
            foreach (string File_Path in Path_List)
            {
                try // Sometimes illegal characters crash this..
                {
                    if (File_Name_Is_Path && Path.GetFileName(File_Path) == Path.GetFileName(File_Name)) { return true; }
                    else if (!File_Name_Is_Path && Path.GetFileName(File_Path) == File_Name) { return true; }
                }
                catch { }
            }
            return false;
        }




        //=====================//

        // Mouse Up because this event runs the right timing to grab the newest selected item.
        private void List_View_Selection_MouseUp(object sender, MouseEventArgs Mouse)
        {
            if (UI_Mode != "Backup" || At_Top_Level) { return; }
            if (Skipp_First_Trigger) { Skipp_First_Trigger = false; return; }


            string Selection = Select_List_View_First(List_View_Selection);
            if (Selection == "") { return; }
            // iConsole(400, 100, Selection);


            List<string> Comment = new List<string>();
            Comment = Get_Segment_Info(Selection, "Comments");
            string New_Text = Text_Box_Description.Text;
            // iConsole(600, 400, string.Join("\n", Comment));

            if (Mouse.Button == MouseButtons.Right)
            { Set_Backup_Checker(); } // Ignore Parents
            else { Set_Backup_Checker(Selection); } 



            // ========================================================
            // Save eventual changes to Comment Section
            // ========================================================

            if (Last_Backup_Selection != "" && Selection != Last_Backup_Selection)
            {
                if (Last_Backup_Comment != "" && New_Text != Last_Backup_Comment) // Has anything changed meanwhile?
                {
                    // iConsole(600, 400, "Triggered");

                    // Making sure there is proper formatting
                    if (New_Text != "") // Don't move this up
                    {
                        if (!New_Text.EndsWith("\n")) { New_Text += "\n"; }
                        if (!New_Text.EndsWith("\n\n")) { New_Text += "\n"; }
                        if (!New_Text.EndsWith("\n\n\n")) { New_Text += "\n"; }
                    }

                    // iConsole(600, 400, "Updating " + Last_Backup_Selection + "  with\n\n" + New_Text);
                    Write_Into_Segment(Last_Backup_Selection, "Comments", New_Text);

                    try
                    {   if (Current_Backup != "" && Last_Backup_Selection == Current_Backup)
                        {
                            Temporal_A = Selected_Backup_Path + Current_Backup;
                            if (Current_Backup.EndsWith(@"\")) { Temporal_A = Get_Backup_Path(Current_Backup); }

                            // iConsole(600, 300, "\n" + Temporal_A + Backup_Info + "\n\ninto\n\n" + Root_Backup_Path);
                            try
                            {
                                File.Copy(Temporal_A + Backup_Info, Root_Backup_Path, true); // Overwrite root Axe_Info.txt
                            }
                            catch{}
                        }
                    } catch{}
                }

                Text_Box_Description.Text = "";
                Last_Backup_Selection = Selection;
            }



            // ========================================================
            // Append Comment to the Axe_Info.txt
            // ========================================================

            // DON'T replace Text_Box_Description.Text here with "New_Text"!!!  Because it causes issues.
            if (Comment.Count() == 0)
            {                          
                if (Last_Backup_Selection != "" && Text_Box_Description.Text != "" && Text_Box_Description.Text != "\n")
                {
                    // iConsole(600, 400, "Writing new " + Last_Backup_Selection + "  with\n\n" + Text_Box_Description.Text);

                    Write_Into_Segment(Last_Backup_Selection, "Comments", Text_Box_Description.Text);
                    Last_Backup_Comment = Text_Box_Description.Text;
                }
            }

            else
            {   // Disabled, because we can't destinguish the "Changed_Files" from the Comment..
                //Temporal_E.Clear(); 
                //Temporal_E = Get_Segment_Info(Selection, "Changed_Files");

                Last_Backup_Comment = string.Join("\n", Comment);
                Text_Box_Description.Text = Last_Backup_Comment;


                //if (Temporal_E.Count() > 0)
                //{
                //    Text_Box_Description.Text += "\n\n//======= Changed Files: =======\\\\ \n\n"
                //        + string.Join("\n", Get_Segment_Info(Selection, "Changed_Files"));
                //}
            }                        
        }



        //=====================//

        public string Get_Backup_Path(string Backup_Name)
        {
            foreach (string Backup in Get_All_Directories(Backup_Path + Backup_Folder, true))
            {
                if (Path.GetFileName(Backup) == Backup_Name) { return Backup; }
            }

            return "";
        }

        //=====================//

        public List<string> Get_Backup_Parents(string Backup_Name, bool Include_Selection = false)
        {
            List<string> Current_Branch = new List<string>();
            if (Include_Selection) { Current_Branch.Add(Backup_Name); }


            foreach (string Backup in Get_Segment_Info(Backup_Name, "Branches"))
            {               
                if (!Current_Branch.Contains(Backup)) { Current_Branch.Add(Backup); } // Preventing Duplicates
            }

            return Current_Branch;
        }

        //=====================//

        // Just dump in the whole parent history of a backup here. Insert the selected Backup as Last_Backup, in order to start at its Axe_Info.txt
        // It iterates and syncs all Segment types, if you specify no certain Segment_Name
        public void Collapse_Segment_Infos(List<string> Backup_List, string Last_Backup = "", string Segment_Name = "") 
        {
            foreach (string Backup in Backup_List) 
            {
                if (Last_Backup == "") { Last_Backup = Backup; }
                // Keep on fusing with both their predecessor and successor, until we reach the last backup in the list :)
                else { Fuse_Segments(Backup, Last_Backup, Segment_Name); Last_Backup = Backup; }
            }
        }

        //=====================//
      
        public void Fuse_Segments(string Source_Backup, string Target_Backup, string Segment_Name = "")
        {
            List<string> Target_Content = new List<string>();
            string[] Segments = new string[] { "Comments", "Changed_Files", "Added_Files", "Removed_Files"}; // , "Branches"}; Ignore Branches
            if (Segment_Name != "") { Segments = new string[] { Segment_Name }; } // Just the specified segment instead of all

            try
            {
                // string Stitched_File = ""; // We stitch segment by segment into this new file
                string Stitched_File = File.ReadAllText(Selected_Backup_Path + Source_Backup + Backup_Info).Replace("\r", "");
                // If "" it will run Write_Into_Segment() to just return "".


                foreach (string Segment in Segments)
                {
                    Target_Content.Clear();
                    Target_Content = Get_Segment_Info(Target_Backup, Segment);


                    foreach (string Entry in Get_Segment_Info(Source_Backup, Segment))
                    {
                        if (Target_Content.Contains(Entry)) { Target_Content.Remove(Entry); } // Removing duplicate lines
                    }


                    if (Target_Content.Count() > 0)
                    {
                        // iConsole(500, 400, Segment + " is :\n\n" + string.Join("\n", Target_Content));

                        // Position of Target and Source backup are exchanged here!!!
                        Stitched_File = Write_Into_Segment(Source_Backup, Segment, Target_Content, Target_Backup, true, Stitched_File);
                        // iConsole(600, 600, "The file is :\n" + Stitched_File);
                    }
                }

                // iConsole(600, 600, "Final file is :\n" + Stitched_File);
                File.WriteAllText(Selected_Backup_Path + Source_Backup + Backup_Info, Stitched_File);
            }
            catch { iConsole(500, 140, "\nFailed to stitch Axe_Info.txt files together."); }
        }


        //=====================//

        public string Write_Into_Segment(string Source_Backup, string Segment_Name, List<string> Content, string Target_Backup = "Same", bool Prepend_To_Segment = false, string Passed_File = "")
        { return Write_Into_Segment(Source_Backup, Segment_Name, string.Join("\n", Content), Target_Backup, Prepend_To_Segment, Passed_File); }

        //=====================//

        public string Write_Into_Segment(string Source_Backup, string Segment_Name, string Content, string Target_Backup = "Same", bool Prepend_To_Segment = false, string Passed_File = "")
        {
            if (Wash_String(Content) == "") { Content = " "; } // If it consists of only \n \t \r and " " characters we just write a " " instead.


            // For Auto Backups, Backup_Folder was set to "Mod_Name" in Button_Run_Click()
            foreach (string Backup in Get_All_Directories(Backup_Path + Backup_Folder, true))
            {
                if (Source_Backup != "Any" && Path.GetFileName(Backup) != Source_Backup) { continue; }
                else if (!File.Exists(Backup + Backup_Info)) { continue; }

                // iConsole(600, 200, Backup + "  \n" + Content);
             

                string Segment = 
                    "//============================================================\\\\" +
                    "\n" + Segment_Name +
                    "\n//============================================================\\\\\n";


                string The_File = Passed_File; // .Replace("\r", "") should already be applied before passed into this function.
                if (Passed_File == "") { The_File = File.ReadAllText(Backup + Backup_Info).Replace("\r", ""); }
                // iConsole(600, 600, "Writing into this file: \n\n" + The_File); 
             

                if (!The_File.Contains(Segment_Name))
                {
                    if (Content == " ") { return ""; } // We won't just add a empty segment, will we.

                    The_File += "\n" + Segment + Content + "\n\n\n"; // Append 
                    // iConsole(600, 200, "Appending \n\n\"" + Content + "\""); 
                } 
                else
                {
                    if (!Prepend_To_Segment)
                    {   // iConsole(500, 500, "\nReplacing: \n\"" + Old_Segment + "\" \n\nwith \n\n\"" + Content + "\"");                                
                        // iConsole(500, 500, "\nReplacing: \n\"" + Segment + "\n" + Old_Segment + "\" \n\nwith \n\n\"" + Segment + Content + "\"");                 


                        // Old_Segment = string.Join("\n", Get_Segment_Info(Source_Backup, Segment_Name)); // Old Method


                        bool Started = false;
                        bool Ignored_First = false;
                        Temporal_E.Clear();
                        // int Skip_First = 2; // Skipping the Header
                        Temporal_E.Add("//============================================================\\\\"); // Starting Line


                        foreach (string Line in File.ReadAllLines(Backup + Backup_Info))
                        {   // Skipping everything but the chapter after "Removed_Files"
                            if (Line.StartsWith(Segment_Name)) { Started = true; }
                            else if (Started && Line.StartsWith("//============="))
                            {
                                if (!Ignored_First) { Ignored_First = true; } // Ignore the first //============= that comes after the Segment Name
                                else break; // Exit at Stop_Segment
                            }
                            else if (!Started) { continue; }
                            else if (Line.StartsWith("//") || Line.StartsWith("#")) { continue; } //|| Line == "\n" || Line == "") { continue; }


                            //if (Skip_First > 0) { Skip_First--; } else { 
                                Temporal_E.Add(Line);
                            //} // This is the legit area to be replaced
                        }

                        // We need to replace the full Old_Segment with both Segment + Content, 
                        // In order to prevent repeating \n \r \t patterns from being miss-overwritten for multiple times within the file.
                        string Old_Segment = string.Join("\n", Temporal_E).Replace("\r", "");



                        if (Old_Segment == "") { Prepend_To_Segment = true; } // Then Prepend below!                      
                        else
                        {   
                            try { The_File = The_File.Replace(Old_Segment, Segment + Content + "\n"); } // Replace it
                            catch { iConsole(540, 100, "\nFailed to overwrite the existing " + Segment_Name + " segment."); }
                        }
                    }

                    // Don't use else if here.
                    if (Prepend_To_Segment) { The_File = The_File.Replace(Segment, Segment + Content + "\n\n"); }
                }




                if (Passed_File != "")
                {   // iConsole(600, 600, "Passing File:\n\n" + The_File);
                    return The_File; // Just return
                } 
                    

                // Use the Input File to write into itself
                else if (Target_Backup == "Same") 
                {   File.WriteAllText(Backup + Backup_Info, The_File);
                    // iConsole(500, 200, "\nWriting into \n" + Backup + Backup_Info);
                }
                else 
                {             
                    string New_Backup = Path.GetDirectoryName(Backup) + @"\" + Target_Backup;
                    if (!Directory.Exists(New_Backup)) { Directory.CreateDirectory(New_Backup); }

                    File.WriteAllText(New_Backup + Backup_Info, The_File);
                    // iConsole(500, 200, "\nWriting into \n" + New_Backup + Backup_Info);
                }


                // Quit because we already edited the Backup we need
                if (Source_Backup != "Any" && Path.GetFileName(Backup) == Source_Backup) { return ""; } 
            }

            return ""; 
        }


        //=====================//

        public List<string> Get_Segment_Info(string Backup_Name, string Segment_Name, bool Is_Full_Path = false, bool Return_Filename = false, string Passed_File = "")
        {
            List<string> Segment_Info = new List<string>();
            string The_File = Passed_File;


            foreach (string Backup in Get_All_Directories(Backup_Path + Backup_Folder, true)) // Was Backup_Path + Mod_Name
            {
                if (Backup_Name != "Any" && Path.GetFileName(Backup) != Backup_Name) { continue; } // We only want info from target Backup(s)
                else if (!File.Exists(Backup + Backup_Info)) { continue; }



                bool Started = false;
                bool Ignored_First = false;
                // iConsole(500, 600, File.ReadAllText(Backup + Backup_Info));



                if (Passed_File == "") { The_File = File.ReadAllText(Backup + Backup_Info); }
                // else { iConsole(500, 600, "Reading passed file:\n\n" + The_File); }  // Otherwise it remains the Passed_File
               



                foreach (string Line in The_File.Split('\n'))
                {
                    // Skipping everything but the chapter after "Removed_Files"
                    if (Line.StartsWith(Segment_Name)) { Started = true; }
                    else if (Started && Line.StartsWith("//============="))
                    {   if (!Ignored_First) { Ignored_First = true; } // Ignore the first //============= that comes after the Segment Name
                        else break; // Exit at Stop_Segment
                    } 

                    else if (!Started) { continue; }
                    else if (Line.StartsWith("//") || Line.StartsWith("#")) { continue; } //|| Line == "\n" || Line == "") { continue; }


                    else if (Is_Full_Path)
                    {   try
                        {
                            if (Return_Filename && !Segment_Info.Contains(Path.GetFileName(Line))) { Segment_Info.Add(Path.GetFileName(Line)); }
                            else if (!Return_Filename && !List_Matches_Filename(Segment_Info, Line, true)) { Segment_Info.Add(Line); }// Preventing double entries 
                        }
                        catch { }
                    }

                    else // if (!Is_Full_Path)
                    {
                        // Visualize_Characters(Line);
                        
                        // Exception for "\n" and "" characters: We need those to preserve formatting!
                        if (Line == "\n" || Line == "\n\n" || Line == "\n\n\n" || Line == "") { Segment_Info.Add(Line); }
                        else if (!Segment_Info.Contains(Line)) { Segment_Info.Add(Line); }
                    }
                }
                

                // Quit the loop because we already got the info of the Backup we need
                if (Backup_Name != "Any" && Path.GetFileName(Backup) == Backup_Name) { break; } 
            }

        
            return Segment_Info;
        }




        //=====================//

        public void Visualize_Characters(string Text_1, string Text_2 = "")
        {
            Text_1 = Text_1.Replace("\t", "TAB").Replace("\n", "\nNEW").Replace("\r", "CARRIAGE").Replace(" ", "_");

            if (Text_2 != "")
            {
                Text_2 = Text_2.Replace("\t", "TAB").Replace("\r", "CARRIAGE").Replace(" ", "_");
                iConsole(500, 500, Text_1 + "\n\n//======================\n\n" + Text_2);
            }
            else { iConsole(500, 500, Text_1); }
        }


        //=====================//


        // Set Shall_Match to false to return a list of files that never matched instead of the matched ones.
        public List<string> Check_Files(List<string> Return_If_Not_Matched, List<string> Return_If_Matched, bool Shall_Match = true, bool Return_Full_Path = true)
        {
            List<string> Results = new List<string>();         

            try
            {   // Difference between Return_If_Matched and Return_If_Not_Matched is, that they have different paths at the beginning, 
                // And so the directory with files of interest is the one you want to either return if matched or not.
                foreach (string File_A in Return_If_Not_Matched)
                {
                    try
                    {
                        bool Name_Matched = false;
                        bool Size_Matched = false;
                        string Name_A = Path.GetFileName(File_A);
                        if (Name_A == "Axe_Info.txt") { continue; }


                        foreach (string File_B in Return_If_Matched)
                        {   
                            try
                            {                              
                                string Name_B = Path.GetFileName(File_B);
                                if (Name_B == "Axe_Info.txt") { continue; }


                                if (Return_Full_Path && !Registered_Files.Contains(File_B)) { Registered_Files.Add(File_B); } 
                                else if (!Return_Full_Path && !Registered_Files.Contains(Name_B)) { Registered_Files.Add(Name_B); } // Registered_Files is a Global Variable


                                if (Name_A == Name_B) // If the file names do match
                                {
                                    Name_Matched = true;
                                   
                                    long Length_A = new System.IO.FileInfo(File_A).Length;
                                    long Length_B = new System.IO.FileInfo(File_B).Length;

                                    // iConsole(400, 100, Path.GetFileName(File_A) + "  and  " + Path.GetFileName(File_B));


                                    if (Length_A == Length_B) // If file sizes match 
                                    {
                                        Size_Matched = true;

                                        if (Shall_Match && !Results.Contains(File_B))
                                        {
                                            if (Return_Full_Path) { Results.Add(File_B); }
                                            else if (!Results.Contains(Name_B)) { Results.Add(Name_B); }

                                            continue; // Due to matched Size, proceed with the next file instead of looping down through the rest of the list.
                                        }


                                        // Its Critical to use File_A here!  Not File_B, because we're looking for the match partner that was stored
                                        // in the "Return_If_Not_Matched" for the last cycle(s) of Check_Files() before this one: Inside of the global "Difference_List" variable.
                                        // Clearing older matches that were adressed by newer patches/backups:
                                        if (Return_Full_Path & Difference_List.Contains(File_A)) { Difference_List.Remove(File_A); }

                                        else if (Difference_List.Contains(File_A.Replace(Sync_Path, "")))
                                        { Difference_List.Remove(File_A.Replace(Sync_Path, "")); }
                                    }
                                    continue; // Due to the matched name, we stop searching here for the remaining entries in the list
                                }                                   
                                  
                            } catch {}
                        }


                        if (!Name_Matched)
                        {   if (Return_Full_Path)
                            {   bool Matched = false;

                                foreach (string Possability in Not_Matched_Yet) // Not_Matched_Yet is a Global Variable                                                            
                                {
                                    if (Path.GetFileName(Possability) == Name_A) { Matched = true; break; }
                                }
                                if (!Matched) { Not_Matched_Yet.Add(File_A); }

                            }
                            else if (!Not_Matched_Yet.Contains(Name_A)) { Not_Matched_Yet.Add(Name_A); }
                        }



                        // Name_Matched means it is existing but different
                        if (!Shall_Match && !Size_Matched && Name_Matched) // Exclusion of my log file
                        {
                            if (Return_Full_Path && !Results.Contains(File_A)) { Results.Add(File_A); }
                            else if (!Results.Contains(Name_A)) { Results.Add(Name_A); }
                        }
                  
                    } catch {}
                }
            } catch { iConsole(400, 100, "File Comparison function has crashed."); }


            // iConsole(500, 500, string.Join("\n", Not_Matched_Yet));

            return Results;
        }



        //=====================// 

        private void Button_Operator_Click(object sender, EventArgs e)
        {
            if (UI_Mode == "Backup") // MANUALLY CREATE A NEW BACKUP OF THE FULL XML DIR
            {
                if (At_Top_Level) // && !Directory.Exists(Backup_Path + Mod_Name)) // Don't use Backup_Folder instead of Mod_Name here!
                {   // Directory.CreateDirectory(Backup_Path + Mod_Name); // Creating Mod Dir

                    using (var Folder_Browser_Dialog_1 = new FolderBrowserDialog())
                    {   if (Folder_Browser_Dialog_1.ShowDialog() == DialogResult.OK)
                        {   // Creating Mod Dir
                            Temporal_A = Backup_Path + Path.GetFileName(Folder_Browser_Dialog_1.SelectedPath);
                           
                            if (!Directory.Exists(Temporal_A)) { Directory.CreateDirectory(Temporal_A); }
                            // iConsole(600, 100, Temporal_A);   

                            if (!File.Exists(Temporal_A + Backup_Info))
                            { File.WriteAllText(Temporal_A + Backup_Info, @"Directory_Name = " + Folder_Browser_Dialog_1.SelectedPath); }                                                           
                        }      
                    }

                    Refresh_Backup_Directory();                   
                }

                else // Means create new Backup
                {  
                    /*  // 540, 240  This check is quite annoying, disabled.
                    iDialogue(540, 210, "Do It", "Cancel", "false", "false", "\nDo you wish to create a new backup of the\n"
                    + Path.GetFileName(Sync_Path.Remove(Sync_Path.Length - 1)) + " directory?\n\n"
                        // + "This will also delete backups older then the " + Int32.Parse(Get_Setting_Value("Backups_Per_Directory")) + "th slot."
                    );
                    */

                    if (List_View_Selection.Items.Count > 0)
                    {

                        Temporal_B = "";

                        foreach (ListViewItem Item in List_View_Selection.Items)
                        {
                            if (!Item.Text.EndsWith("Stash") && !Item.Text.EndsWith("Auto")) 
                            { Temporal_B = Item.Text; break; } // Just get the first that is no _Stash
                        }

                 
                        // We need to be up to date, otherwise this action would twist the sync
                        if (Current_Backup != Temporal_B) // If Current_Backup is NOT the top-most one
                        {
                            iDialogue(540, 236, "Continue", "Cancel", "false", "false", "\nA older Backup is currently loaded/checked out.\n" +
                            //"Please load the newest/top most backup, because it \nallows us to continue the chain in correct order."
                            "If it doesn't write from the top most one, the \nnew backup will end up in a different branch.\n" +
                            "Do you wish to continue anyways?");


                            if (Caution_Window.Passed_Value_A.Text_Data == "false") { return; }
                        }
                    }
               

                    Create_New_Backup();
                }
            }


            else if ( Operation_Mode == "Bool")
            {   if (Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "True"))
                {   Combo_Box_Tag_Value.Text = "False";
                    Set_Resource_Button(Button_Operator, Properties.Resources.Button_Bool);
                }
                else if (Combo_Box_Tag_Value.Text == "" | Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "False"))
                {
                    Combo_Box_Tag_Value.Text = "True";
                    Set_Resource_Button(Button_Operator, Properties.Resources.Button_Bool_Lit);
                }
          
                else if (Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "Yes"))
                {   Combo_Box_Tag_Value.Text = "No";
                    Set_Resource_Button(Button_Operator, Properties.Resources.Button_Bool);
                }
                else if (Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "No"))
                {
                    Combo_Box_Tag_Value.Text = "Yes";
                    Set_Resource_Button(Button_Operator, Properties.Resources.Button_Bool_Lit);
                }        
            }
            else if (Combo_Box_Tag_Value.Text != "") // & Operation_Mode == "Percent")  // Toggle
            {
                string The_Value = Combo_Box_Tag_Value.Text;

                if (The_Value.Contains("-")) { Combo_Box_Tag_Value.Text = The_Value.Replace("-", "+"); }
                else if (The_Value.Contains("+")) { Combo_Box_Tag_Value.Text = The_Value.Replace("+", "-"); }         
                else { Combo_Box_Tag_Value.Text = "-" + The_Value; }
            }

        }

     
        private void Button_Operator_MouseHover(object sender, EventArgs e)
        {
            if (UI_Mode == "Backup") { Set_Resource_Button(Button_Operator, Properties.Resources.Button_Plus_Lit); }

            else if (Operation_Mode == "Bool")
            {   if (Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "True") | Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "Yes"))
                { Set_Resource_Button(Button_Operator, Properties.Resources.Button_Bool_Lit); }
                else { Set_Resource_Button(Button_Operator, Properties.Resources.Button_Bool); }
            }
            else
            {   if (Combo_Box_Tag_Value.Text.Contains("-")) { Set_Resource_Button(Button_Operator, Properties.Resources.Button_Plus_Lit); }
                else { Set_Resource_Button(Button_Operator, Properties.Resources.Button_Minus_Lit); }
            }

        }

        private void Button_Operator_MouseLeave(object sender, EventArgs e)
        {
            if (UI_Mode == "Backup") { Set_Resource_Button(Button_Operator, Properties.Resources.Button_Plus); }

            else if (Operation_Mode == "Bool")
            {   if (Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "True") | Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "Yes")) 
                { Set_Resource_Button(Button_Operator, Properties.Resources.Button_Bool_Lit); }
                else { Set_Resource_Button(Button_Operator, Properties.Resources.Button_Bool); }
            }
            else
            {   if (Combo_Box_Tag_Value.Text.Contains("-")) { Set_Resource_Button(Button_Operator, Properties.Resources.Button_Plus); }
                else { Set_Resource_Button(Button_Operator, Properties.Resources.Button_Minus); }
            }
        }



        private void Window_ResizeEnd(object sender, EventArgs e)
        {
             Properties.Settings.Default.Last_Width = this.Size.Width;
             Properties.Settings.Default.Last_Height = this.Size.Height;
             Properties.Settings.Default.Save();

             // if (this.Size.Width > Screen.FromControl(this).Bounds.Width - 200) { this.Size = new Size(444, 660); }
             // iConsole(400, 100, Screen.FromControl(this).Bounds.Width.ToString()); 
        }



    
        //===========================//



    }
}
