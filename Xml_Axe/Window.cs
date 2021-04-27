﻿using System;
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
        string Tag_List = "";
        bool User_Input = false;
        bool Percent_Mode = false;
        bool EAW_Mode = true;
        bool Bool_Mode = false;
        bool Script_Mode = false;
        string Temporal_A, Temporal_B = "";
        string Queried_Attribute = "Name"; // Preseting
        string Text_Format_Delimiter = ";";
        string Script_Directory = "";
        string Last_Combo_Box_Tag_Name = "";
        string[] Balancing_Tags = null; // new string[] { };
        public Color Theme_Color = Color.CadetBlue;
        public string Xml_Directory = Properties.Settings.Default.Xml_Directory;
        public List<string> Found_Scripts = null;
        public List<string> File_Collection = new List<string>();
        public List<string> Blacklisted_Xmls = null;
        public List<string> Temporal_E = new List<string>();
     


        private void Window_Load(object sender, EventArgs e)
        {
               
            Drop_Zone.AllowDrop = true;
            List_View_Selection.AllowDrop = true;

            Set_Resource_Button(Drop_Zone, Get_Start_Image()); 
            Set_Resource_Button(Button_Browse, Properties.Resources.Button_File);
            Set_Resource_Button(Button_Start, Properties.Resources.Button_Logs);
            Set_Resource_Button(Button_Search, Properties.Resources.Button_Search);
            Set_Resource_Button(Button_Percentage, Properties.Resources.Button_Percent);
            Set_Resource_Button(Button_Scripts, Properties.Resources.Button_Flash);
            Set_Resource_Button(Button_Operator, Properties.Resources.Button_Minus);
            Set_Resource_Button(Button_Run, Properties.Resources.Button_Axe);
            Set_Resource_Button(Button_Toggle_Settings, Properties.Resources.Button_Settings);
            Set_Resource_Button(Button_Reset_Blacklist, Properties.Resources.Button_Controller);


            Control[] Controls = { Button_Browse, Button_Start, Button_Run, Button_Search, Button_Percentage,
                                   Button_Scripts, Button_Operator, Button_Reset_Blacklist, Button_Toggle_Settings };
            foreach (Control Selectrion in Controls) { Selectrion.BackColor = Color.Transparent; }   
    
     

            if (File.Exists(Xml_Directory + "Axe_Blacklist.txt"))
            { Blacklisted_Xmls = File.ReadAllLines(Xml_Directory + "Axe_Blacklist.txt").ToList(); }

            Load_Xml_Content(Properties.Settings.Default.Last_File, false); // Need this loaded so In_Selected_Xml(); can match true
            

            Tag_List = Properties.Settings.Default.Tags;
            if (Tag_List == null | Tag_List == "") 
            {   Reset_Tag_List();
                Properties.Settings.Default.Tags = Tag_List;              
                Properties.Settings.Default.Save();          
            }

            Text_Box_Tags.Text = Tag_List;
            Reset_Tag_Box();
            Reset_Root_Tag_Box();


            Queried_Attribute = Get_Setting_Value("Queried_Attribute"); // Needs to run after Reset_Tag_List()!
            Text_Format_Delimiter = Get_Setting_Value("Text_Format_Delimiter");


            // ================== INSTALLATION ==================
            Script_Directory = Get_Scripts_Dir();
            if (!Directory.Exists(Script_Directory))
            {
                string Parent_Dir = Path.GetDirectoryName(Script_Directory);
                // iConsole(200, 100, Scripts_Directory);

                byte[] Archive = Properties.Resources.Scripts;
                File.WriteAllBytes(Parent_Dir + @"\Delete_Me.zip", Archive);

                System.IO.Compression.ZipFile.ExtractToDirectory(Parent_Dir + @"\Delete_Me.zip", Parent_Dir);

                // Trashing after extraction, named the archive "Delete_Me" for the case that its autodeletion fails
                Deleting(Parent_Dir + @"\Delete_Me.zip");
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


            if (Match_Setting("Disable_EAW_Mode"))
            {
                EAW_Mode = false;
                Label_Entity_Name.Text = "Attribute";
                Label_Type_Filter.Text = "Parent Name";
            }

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
        { Disable_Description(); }

        private void Disable_Description()
        {
            Text_Box_Description.Text = "";
            Text_Box_Description.Visible = false;
        }

        //===========================// 
        private void List_View_Selection_DoubleClick(object sender, EventArgs e)
        {
            if (List_View_Selection.Items.Count > 0)
            {
                for (int i = List_View_Selection.Items.Count - 1; i >= 0; --i)
                { List_View_Selection.Items[i].Selected = true; }
            }
        }

        private void List_View_Selection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Script_Mode) { Run_Script(); return; } // Script Mode override


            if (List_View_Selection.SelectedItems.Count < 2)
            { Combo_Box_Entity_Name.Text = Select_List_View_First(List_View_Selection); }
            else
            {
                Combo_Box_Entity_Name.Text = "Multi";
            }
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
                        if (!System.Text.RegularExpressions.Regex.IsMatch(File_Names[0], "(?i).*?" + ".xml$")
                           & !File.GetAttributes(File_Names[0]).HasFlag(FileAttributes.Directory)) // Check whether it is a file or a directory                                                     
                        { iConsole(600, 200, "\nError: the file needs to either be a folder or of .xml format."); }
                        else { Set_Paths(File_Names[0]); }

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
                            Properties.Settings.Default.Xml_Directory = Xml_Directory; 
                            break;
                        }

                        Temporal_A = Path.GetDirectoryName(Temporal_A); // Trying to utilize recursion on Temporal_A in the next loop
                    } 

                    else if (Is_Match(Temporal_A, "data$"))
                    {
                        Xml_Directory = Temporal_A + @"\xml\"; // Updating 
                        Properties.Settings.Default.Xml_Directory = Xml_Directory;
                        // Leaping back for a directy, to get the name of the Modpath
                        Properties.Settings.Default.Mod_Directory = Path.GetDirectoryName(Temporal_A); ;

                        // iConsole(600, 100, Path.GetDirectoryName(Path.GetDirectoryName(Temporal_A)));
                        break;
                    }

                    else //Until we got to the Xml directory
                    {                      
                        Xml_Directory = Temporal_A + @"\"; // Updating 
                        Properties.Settings.Default.Xml_Directory = Xml_Directory;


                        // Leaping back by 2 directoies, to get the name of the Modpath
                        Properties.Settings.Default.Mod_Directory = Path.GetDirectoryName(Path.GetDirectoryName(Temporal_A)); ;

                        // iConsole(600, 100, Path.GetDirectoryName(Path.GetDirectoryName(Temporal_A)));
                        break;
                    }
                }
            }

            else // If not Refresh_Dir, we trust that the selected file is a xml
            {   Load_Xml_Content(The_Path, true);
                List_View_Selection.Visible = true;                             
            }


            Set_Resource_Button(Button_Start, Properties.Resources.Button_Logs_Lit);
            Set_Checker(List_View_Selection);

            if (Text_Box_Description.Visible) { Disable_Description(); }

         
            Properties.Settings.Default.Save();
        }


        //===========================//

        private void Track_Bar_Tag_Value_Scroll(object sender, EventArgs e)
        {   User_Input = false;
            string Operator = "";
            string Percentage = "";

            if (Combo_Box_Tag_Value.Text.StartsWith("-")) { Operator = "-"; } // Remain -

            if (Percent_Mode) 
            {   if (!Combo_Box_Tag_Value.Text.StartsWith("-")) { Operator = "+"; } // Defaulting Prefix to + 
                Percentage = "%";
            }

            Combo_Box_Tag_Value.Text = Operator + (Track_Bar_Tag_Value.Value * Scale_Factor) + Percentage;
            User_Input = true;


            // Check to disable Bool_Mode
            Combo_Box_Tag_Value_TextChanged(null, null); // This must run with User_Input
        }


        //===========================//
        private void Button_Browse_Click(object sender, EventArgs e)
        {
            if (Script_Mode) { Execute(Script_Directory); return; }


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
                {   Text_Box_Original_Path.Text = Open_File_Dialog_1.FileName;
                    Set_Paths(Open_File_Dialog_1.FileName);
                }
            }  catch {}
        }

        private void Button_Browse_MouseHover(object sender, EventArgs e)
        { Set_Resource_Button(Button_Browse, Properties.Resources.Button_File_Lit); }

        private void Button_Browse_MouseLeave(object sender, EventArgs e)
        { Set_Resource_Button(Button_Browse, Properties.Resources.Button_File); }

       
        //===========================//
        private void Button_Start_Click(object sender, EventArgs e)
        {         
            if (List_View_Selection.Visible) { List_View_Selection.Visible = false; }
            else 
            {   Load_Xml_Content(Properties.Settings.Default.Last_File); // Auto toggles to visible 
             
                if (Text_Box_Description.Visible) { Disable_Description(); }
            }                            
        }

        private void Button_Start_MouseHover(object sender, EventArgs e)
        { Set_Resource_Button(Button_Start, Properties.Resources.Button_Logs_Lit); }

        private void Button_Start_MouseLeave(object sender, EventArgs e)
        {
            if (List_View_Selection.Visible) // Use its visability as bool toggle ;)
            { Set_Resource_Button(Button_Start, Properties.Resources.Button_Logs_Lit); }
            else { Set_Resource_Button(Button_Start, Properties.Resources.Button_Logs); } 
        }

      
        //===========================// 

        private void Button_Run_Click(object sender, EventArgs e)
        {
            // iConsole(600, 400, Check_for_Steam_Version()); // Debug
            if (Combo_Box_Tag_Name.Text == "") { return; }
            Set_Resource_Button(Drop_Zone, Get_Start_Image());
           

            int Line_Count = 0;
            // bool Warn_User = true;
            string The_Settings = Properties.Settings.Default.Tags;
            List<string> Related_Xmls = new List<string>();


            // if (The_Settings.Contains("Show_Files_That_Would_Change = true") | The_Settings.Contains("Request_Approval=true"))
            if (Match_Setting("Request_File_Approval") & !In_Selected_Xml(Combo_Box_Entity_Name.Text))
            {
                // Warn_User = false;                  
                Related_Xmls = Slice(false); // Means don't apply any changes
                Line_Count = (Related_Xmls.Count() * 30) + 160;
                if (Line_Count > 680) { Line_Count = 680; }

               

                if (Related_Xmls.Count == 0) // the Slice function is supposed to fill this list with paths
                {
                    string Error_Text = "\nI'm sorry, no entries with Attribute Name \"" + Queried_Attribute
                    + "\"\nand Attribute Value \"" + Combo_Box_Entity_Name.Text + "\" were found \nto contain the child name \"" + Combo_Box_Tag_Name.Text + "\"";

                    if (Combo_Box_Entity_Name.Text == "" | Combo_Box_Entity_Name.Text == "None") // Then the query went by filter, which is name of the Entities root tag
                    {
                        Error_Text = "\nI'm sorry, no entries with Entity Parent Tag Name \"" + Combo_Box_Type_Filter.Text
                        + "\" were found \nto contain the child name \"" + Combo_Box_Tag_Name.Text + "\"";
                    }


                    iConsole(600, 100, Error_Text); return;
                }
                else { iDialogue(680, Line_Count, "Yes", "Cancel", "Ignore", "Blacklist", "Are you sure you wish to apply changes to:", Related_Xmls, true); }


                if (Caution_Window.Passed_Value_A.Text_Data == "false") { return; }
            }


            Related_Xmls = Slice(true); // This line does the actual Job!
            Set_Resource_Button(Drop_Zone, Get_Done_Image());
            if (List_View_Selection.Visible) { Button_Start_Click(null, null); } // Hiding open Xml

            if (Combo_Box_Entity_Name.Text == "None") { Properties.Settings.Default.Entity_Name = ""; }
            else { Properties.Settings.Default.Entity_Name = Wash_String(Combo_Box_Entity_Name.Text); }


            if (Combo_Box_Tag_Name.Text != "Rebalance_Everything") // We don't want the user to accidently re-apply such a powerfull setting
            {   Properties.Settings.Default.Type_Filter = Combo_Box_Type_Filter.Text;
                Properties.Settings.Default.Tag_Name = Combo_Box_Tag_Name.Text;

                if (Combo_Box_Tag_Value.Text.Contains("%")) { Properties.Settings.Default.Tag_Value = ""; } // Preventing Errors
                else { Properties.Settings.Default.Tag_Value = Combo_Box_Tag_Value.Text; }
            }
           

            Properties.Settings.Default.Trackbar_Value = Track_Bar_Tag_Value.Value;
            Properties.Settings.Default.Save(); // Storing last usage

            if (Text_Box_Description.Visible) { Disable_Description(); }

            // Disabled Feature, quite obsolete
            /*if (Related_Xmls.Count() > 0 & Warn_User & !In_Selected_Xml(Combo_Box_Entity_Name.Text) 
              & Match_Setting("Show_Changed_Files"))
            {
                Line_Count = (Related_Xmls.Count() * 30) + 90;
                if (Line_Count > 680) { Line_Count = 680; }
                iConsole(560, Line_Count, "\nApplied Changes to the following files: \n\n" + string.Join("\n", Related_Xmls));
            }*/
        }


        private void Button_Run_MouseHover(object sender, EventArgs e)
        { Set_Resource_Button(Button_Run, Properties.Resources.Button_Axe_Lit); }

        private void Button_Run_MouseLeave(object sender, EventArgs e)
        { Set_Resource_Button(Button_Run, Properties.Resources.Button_Axe); }



        private bool Match_Setting(string Entry)
        {   if (Match_Without_Emptyspace(Properties.Settings.Default.Tags, Entry + " = true")) { return true; }
            return false;
        }

        // Custom_Modpath, Queried_Attribute, Text_Format_Delimiter
        private string Get_Setting_Value(string Entry)
        {
            foreach (string Line in Properties.Settings.Default.Tags.Split('\n'))
            { if (Line != "" & Line.Contains(Entry)) { return Remove_Emptyspace_Prefix(Line.Split('=')[1]); } }

            return "";
        }


        private bool Match_Without_Emptyspace(string Entry_1, string Entry_2)
        {
            if (Regex.IsMatch(Entry_1.Replace(" ", ""), "(?i)" + Entry_2.Replace(" ", ""))) { return true; }
            return false;
        }


        private bool Match_Without_Emptyspace_2(string Entry_1, string Entry_2)
        {   if (Entry_1.ToLower().Replace(" ", "").Contains(Entry_2.ToLower().Replace(" ", ""))) { return true; }
            return false;
        }

      
        private bool Is_Match(string Entry_1, string Entry_2)
        {
            if (Regex.IsMatch(Entry_1, "(?i)" + Entry_2)) { return true; }
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
            // List<string> Found_Entities = null;
            IEnumerable<XElement> Instances = null;
            if (!File.Exists(Xml_Path)) { iConsole(200, 100, "\nCan't find the Xml."); return; }
            List_View_Selection.Items.Clear();
            List_View_Selection.Visible = true;


            try
            {   // ===================== Opening Xml File =====================
                XDocument Xml_File = XDocument.Load(Xml_Path, LoadOptions.PreserveWhitespace);
             
                Instances = // Target all entities in the whole Mod!
                  from All_Tags in Xml_File.Root.Descendants()
                    // Selecting all non empty tags that have the Attribute "Name", null because we need all selected.
                      where All_Tags != null
                        select All_Tags;
                

                // =================== Checking Xml Instance ===================
                foreach (XElement Instance in Instances)
                {   if (Instance.Descendants().Any())
                    {
                        string Entity_Name = (string)Instance.Attribute(Queried_Attribute);

                        if (Entity_Name != null)
                        {
                            if (!Match_Without_Emptyspace(Entity_Name, "Death_Clone") & !List_View_Matches(List_View_Selection, Entity_Name))
                            { List_View_Selection.Items.Add(Entity_Name); }
                        }
                    }
                }

                Set_Checker(List_View_Selection);
                if (!Show_List) { List_View_Selection.Visible = false; }
            } catch {}
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


        public bool Combo_Box_Matches(ComboBox The_List, string Item)
        {
            bool Is_Match = false;
            foreach (string Entry in The_List.Items)
            {
                if (Entry == Item) { Is_Match = true; }
            }

            return Is_Match;
        }


        
        //-----------------------------------------------------------------------------
        // Main Function
        //-----------------------------------------------------------------------------
      
        private List<string> Slice(bool Apply_Changes = true)
        {
            // iConsole(500, 100, Properties.Settings.Default.Xml_Directory);
            // iConsole(500, 100, Properties.Settings.Default.Mod_Directory);         

            string Selected_Xml = "";
            string Xml_Directory = Properties.Settings.Default.Xml_Directory;
            string Entity_Name = Wash_String(Combo_Box_Entity_Name.Text);
            string Selected_Tag = Regex.Replace(Combo_Box_Tag_Name.Text, "[\n\r\t </>]", ""); // Also removing </> tag values
            string Selected_Type = Wash_String(Combo_Box_Type_Filter.Text);

            // XElement Selected_Instance = null;
            IEnumerable<XElement> Instances = null;
            List <string> Changed_Xmls = new List<string>();

          

            
            if (!Directory.Exists(Xml_Directory))
            { iConsole(200, 100, "\nCan't find the Xml Directory."); return null; }



            if (Apply_Changes) // File_Collection is a global variable, feeded from the remaining filenames in the Caution_Window 
            {   if (File_Collection == null | File_Collection.Count == 0) { File_Collection = Get_All_Files(Xml_Directory, "xml"); } // Failsafe
                // iConsole(560, 600, Xml_Directory + string.Join("\n", File_Collection)); // return null; // Debug Code
            }
            else { File_Collection = Get_All_Files(Xml_Directory, "xml"); }



            foreach (var Xml in File_Collection)
            {   
                try {
                    Selected_Xml = Xml;
                    // Ignoring blacklisted Xmls
                    if (Blacklisted_Xmls != null) { if (Blacklisted_Xmls.Contains(Selected_Xml.Replace(Xml_Directory, ""))) { continue; } }
                    
                    

                    XDocument Xml_File = XDocument.Load(Selected_Xml, LoadOptions.PreserveWhitespace);
                    
                    // ===================== Opening Xml File =====================
                    int Query = 0;

                    if (In_Selected_Xml(Entity_Name)) // Select Multiple by Name
                    {
                        Query = 1;

                        Selected_Xml = Properties.Settings.Default.Last_File; // Overwride what ever xml this loop selected before
                        Xml_File = XDocument.Load(Selected_Xml, LoadOptions.PreserveWhitespace);
                        List<string> Selected_Entities = Select_List_View_Items(List_View_Selection);

                       Instances =
                         from All_Tags in Xml_File.Root.Descendants()
                         where List_Matches(Selected_Entities, (string)All_Tags.Attribute(Queried_Attribute))
                         select All_Tags;
                    }


                    else if (Combo_Box_Type_Filter.Text == "Faction Name Filter")
                    {
                        Query = 2;

                        Instances =
                           from All_Tags in Xml_File.Root.Descendants() // Entity_Name means the Faction name here
                           where All_Tags.Descendants("Affiliation").Last().Value.Contains(Entity_Name)
                           select All_Tags; // Last() because it overwrites the first occurances ingame
                    }
                    else if (Entity_Name != "" & Entity_Name != "None") // Select a single Entity by Name
                    {
                        Query = 3;

                        Instances =
                          from All_Tags in Xml_File.Root.Descendants()
                          where (string)All_Tags.Attribute(Queried_Attribute) == Entity_Name
                          select All_Tags;
                    }
                    else if (Selected_Type != "" & Combo_Box_Type_Filter.Text != "All Types") // By Entity Type
                    {
                        Query = 4;

                        Instances =
                          from All_Tags in Xml_File.Root.Descendants()
                          where All_Tags.Name == Selected_Type
                          select All_Tags;
                    }
                    else // Target all entities in the whole Mod!
                    {
                        Query = 5;

                        Instances =
                          from All_Tags in Xml_File.Root.Descendants()
                          // Selecting all non empty tags that have the Queried_Attribute "Name", null because we need all selected.
                          where All_Tags != null
                          select All_Tags;
                    }

                    // iConsole(300, 100, Query + " Is the Case of Query");



                    // =================== Checking Xml Instance ===================
                    foreach (XElement Instance in Instances)
                    {   if (Instance.Descendants().Any())
                        {
                            if (Combo_Box_Tag_Name.Text != "Rebalance_Everything")
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

                    if (Apply_Changes) { Xml_File.Save(Selected_Xml); } //  iConsole(500, 100, "\nSaving to " + Xml); }
                    if (In_Selected_Xml(Entity_Name)) { return Changed_Xmls; } // Exiting after the first (and only) Xml File.
      
                } catch {}
            }

            File_Collection = new List<string>(); // Clearing for the next time          
            return Changed_Xmls;      
        }


        //===========================//
        // I need to pass in quite a lot of parameters here ...
        public void Set_Tag(List<string> Changed_Xmls, XElement Instance, string Selected_Xml, string Selected_Tag, bool Apply_Changes)
        {   int Changed_Entities = 0;
   

            try
            {   // Selected_Instance = Instance;                         
                if (Instance.Descendants(Selected_Tag).Any()) // Set the new tag value(s)
                {
                    Temporal_A = Selected_Xml.Replace(Xml_Directory, ""); // Removing Path
                    if (!Changed_Xmls.Contains(Temporal_A))
                    { Changed_Xmls.Add(Temporal_A); }


                    if (Apply_Changes)
                    {
                        string New_Value = Combo_Box_Tag_Value.Text.Replace("+", "");
                        
                        if (Selected_Tag == "Planet_Surface_Accessible")
                        {   if (Match_Without_Emptyspace(New_Value, "True") | Match_Without_Emptyspace(New_Value, "Yes"))
                            {   if (!Instance.Descendants("Land_Tactical_Map").Any() | !Is_Match(Instance.Descendants("Land_Tactical_Map").First().Value, ".ted"))
                                { return; } // Failsafe, we must not apply "true" to planets that have no ground maps - or the game will crash!
                            }
                        } 


                        Changed_Entities++;
                        foreach (XElement Target in Instance.Descendants(Selected_Tag))
                        {
                            if (!Percent_Mode) 
                            {
                                if (Combo_Box_Tag_Value.Text.Contains("%")) { Combo_Box_Tag_Value.Text = Combo_Box_Tag_Value.Text.Replace("%", ""); } // Removing Mistakes
                                Target.Value = Combo_Box_Tag_Value.Text.Replace("+", "");
                            }

                            else try // Refactoring the old value!
                            {   int Percentage = 0;
                                decimal Original_Value = 0;

                                Decimal.TryParse(Target.Value, out Original_Value);
                                Int32.TryParse(Remove_Operators(Combo_Box_Tag_Value.Text), out Percentage);

                                if (Combo_Box_Tag_Value.Text.Contains("-")) // Shrink Value
                                { Target.Value = (Original_Value - ((Original_Value / 100) * Percentage)).ToString(); }

                                else // if (Combo_Box_Tag_Value.Text.Contains("+")) // Grow Value
                                { Target.Value = (Original_Value + ((Original_Value / 100) * Percentage)).ToString(); }
                            }  catch {}

                            if (!Check_Box_All_Occurances.Checked) { break; } // Stop after first occurance
                        }
                    }
                }
            } catch {}         
        }

        //===========================//

        public string Wash_String(string The_String)
        { return Regex.Replace(The_String, "[\n\r\t ]", ""); }


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
                if (Combo_Box_Type_Filter.Text != "Faction Name Filter") { Button_Search.Visible = true; }
                Text_Box_Tags.Visible = false;
                Set_UI_Into_Settings_Mode(true);    
                Set_Resource_Button(Button_Reset_Blacklist, Properties.Resources.Button_Controller);
          
    
                if (Tag_List != Text_Box_Tags.Text) // Then needs to update
                {   Properties.Settings.Default.Tags = Text_Box_Tags.Text;
                    Properties.Settings.Default.Save();
                    Tag_List = Text_Box_Tags.Text;
                    // Refreshing Values:
                    Queried_Attribute = Get_Setting_Value("Queried_Attribute");
                    Text_Format_Delimiter = Get_Setting_Value("Text_Format_Delimiter");


                    string New_Script_Dir = Get_Scripts_Dir(true);

                    if (Script_Directory != New_Script_Dir)
                    {                       
                        Moving(Script_Directory, Path.GetDirectoryName(New_Script_Dir));
                        Script_Directory = New_Script_Dir;
                    }
                }                       
                Reset_Tag_Box();
                Reset_Root_Tag_Box();


             

                if (Match_Setting("Disable_EAW_Mode"))
                {
                    EAW_Mode = false;
                    Label_Entity_Name.Text = "Attribute";
                    Label_Type_Filter.Text = "Parent Name";
                }

                // This Check needs to run AFTER Reset_Tag_Box();
                else if (Combo_Box_Type_Filter.Text == "Faction Name Filter")
                {
                    EAW_Mode = true;
                   
                    Label_Entity_Name.Text = "Faction Name";
                } 
                else
                {
                    EAW_Mode = true;
                    Label_Entity_Name.Text = "Entity Name";
                    Label_Type_Filter.Text = "Filter Type";
                }
            }
            else
            {
                Text_Box_Tags.Visible = true;
                Set_UI_Into_Settings_Mode(false);                                     
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
                return;
            }
        }

        private void Set_UI_Into_Settings_Mode(bool Mode)
        {
            Control[] Controls = { Button_Search, Button_Run, Button_Percentage, Button_Scripts, Button_Operator, Label_Type_Filter };
            foreach (Control Selectrion in Controls) { Selectrion.Visible = Mode; } // Hide or show all        
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

                if (Match_Setting("Set_Launch_Affinity")) { Affinity = true; }
                if (Match_Setting("High_Launch_Priority")) { Priority = true; }


                string Custom_Parameters = Get_Setting_Value("Custom_Start_Parameters");
                if (!EAW_Mode && Custom_Parameters == "") 
                { iConsole(400, 100, "\nPlease write any Filepath + Arguments into \nSettings\\Custom_Start_Parameters" +
                    "\nThat we could launch by this button."
                    ); 
                }


                if (Custom_Parameters != "" & !Match_Without_Emptyspace(Properties.Settings.Default.Tags, "Custom_Start_Parameters = false"))
                {
                    string[] Command = Custom_Parameters.Split(' ');
                    string Arguments = "";
                    for (int i = 1; i < Command.Count(); i++) { Arguments += Command[i] + " "; }

                    // iConsole(400, 100, "\"" + Custom_Parameters + "\""); // Debug
                    // Execute(Command[0], Arguments); // This version would throw no error if the process can't be found
                    Check_Process(Command[0], Arguments, Affinity, Priority);

                }
                else if (EAW_Mode) // Otherwised this button is used to launch the game
                {
                    string Steam_Path = Properties.Settings.Default.Steam_Exe_Path;
                    string Program_Path = Path.GetDirectoryName(Steam_Path) + @"\steamapps\common\Star Wars Empire at War\corruption\StarWarsG.exe";

                    string Mod_Name = Path.GetFileName(Properties.Settings.Default.Mod_Directory);
                    string Arguments = @"Modpath=Mods\" + Mod_Name + " Ignoreasserts";


                    // Overwride by User Setting
                    if (!Match_Without_Emptyspace(Properties.Settings.Default.Tags, "Custom_Modpath = false"))
                    {
                        Arguments = "Modpath=" + Get_Setting_Value("Custom_Modpath") + " Ignoreasserts";
                        //  Arguments = "Modpath=" + Properties.Settings.Default.Mod_Directory.Replace(" ", "\\ ") + " Ignoreasserts";
                    }
                    else if (Mod_Name.All(char.IsDigit)) // If all characters are numbers, that means we target a Workshop mod
                    {   // So argument 1 targets now the Workshop dir            
                        // Arguments = @"Modpath=..\..\..\workshop\content\32470\" + Mod_Name; 
                        Arguments = @"Steammod=" + Mod_Name + " Ignoreasserts";
                    }

                    // Now, that we made sure Steam is running, we can launch the game           
                    if (Check_Process(Steam_Path)) { Check_Process(Program_Path, Arguments, Affinity, Priority); }
                } 
            }

        }



        public bool Check_Process(string Program_Path, string Arguments = "", bool Set_Affinity = false, bool High_Priority = false)
        {   
            string Program_Name = Path.GetFileNameWithoutExtension(Program_Path);
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
            Tag_List = @"# ====================== Settings ======================
# Show_Tooltip = true
# Store_Last_Settings = true
# Request_File_Approval = true
# Set_Launch_Affinity = true
# High_Launch_Priority = true
# Queried_Attribute = Name
# RGBA_Color = 100, 170, 170, 255 # Marine Blue
# Script_Directory = %AppData%\Local\Xml_Axe\Scripts
# Custom_Start_Parameters = false
# Disable_EAW_Mode = false
# Text_Format_Delimiter = ;
# Custom_Modpath = false



# ===================== Bool Values ======================

bool Planet_Surface_Accessible # Set to No and it will turn all GCs to space only because it sets all Planets to unaccessible. This operation is revertable: it checks if a planet has a ground.ted map to determine whether it is safe to set surface back to accessible.

bool Is_Targetable # Defines whether or not all Hardpoints in current selection or the Mod can be targeted.

bool Is_Destroyable # Defines whether or not all Hardpoints in current selection or the Mod can be destroyed.

bool Is_Named_Hero # Set to No and no more heroes will respawn. Most of them will also hide their hero Images.

Projectile_Does_Shield_Damage = bool # Set to Yes and apply to the whole mod, to disable all shield piercing effects.

Projectile_Does_Energy_Damage = bool # Careful, if set to yes Hitpoint damage will start once enemy energy level reaches 0.

Projectile_Does_Hitpoint_Damage = bool 

# ====================== Int Values ======================

Tactical_Health = 100

Shield_Points = 100

int Shield_Refresh_Rate = 5 # Usually about 30 for capital ships and less for weaker classes.

Int Select_Box_Scale = 100 # Set to 0 and all Ships and Troops will have their select box deactivated (not revertable). In percent mode this scales the size of all select box circles.

Layer_Z_Adjust = 100 # In percent mode this will scale the distance between all height layer values.

Space_Tactical_Unit_Cap = 10 # Sets Unit cap in space tactical battles, for all Factions in the Mod. Don't put too high or it will cause laggs.

Build_Cost_Credits = 100 # Set the price to 1, then you can build as many units as the population cap allows.

Tactical_Build_Cost_Multiplayer = 100 # Set the price to 1 for all Skirmish units.

Rebalance_Everything = Tactical_Health, Shield_Points, Shield_Refresh_Rate, Projectile_Damage, Damage # This balances the most important aspects of the Game: Tactical_Health, Shields, Shield_Refresh_Rate, Projectile_Damage. You can remove or add more Tag types to this tag in the settings! Then they will scale by the same % value.
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

                        Set_Checker(List_View_Selection);
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
                string[] Entries = new string[] {"All Types", "All in loaded Xml", "Faction Name Filter", "", "SpaceUnit", "UniqueUnit", 
                    "TransportUnit", "GroundInfantry", "GroundVehicle", "HeroUnit", "", "Squadron", "HeroCompany", "GroundCompany", "Planet",
                    "Faction", "HardPoint", "Projectile", "", "StarBase", "SpaceBuildable", "SpecialStructure", "TechBuilding", "GroundBase", 
                    "GroundStructure", "GroundBuildable"
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

            Set_Checker(List_View_Selection);
        }

        //===========================//
        private void Combo_Box_Entity_Name_TextChanged(object sender, EventArgs e)
        { Disable_Description(); }


        private void Combo_Box_Entity_Name_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Return))
            { Button_Search_Click(null, null); }
        }

 
        //===========================//
        private void Combo_Box_Type_Filter_TextChanged(object sender, EventArgs e)
        {
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

            if (Percent_Mode) { Scale_Factor = 10; } // Percentage Override


            if (Combo_Box_Type_Filter.Text == "Faction Name Filter") 
            {   Label_Entity_Name.Text = "Faction Name";
                Label_Type_Filter.Text = "Filter Type";
                Button_Search.Visible = false;
            }

            else if (!EAW_Mode)
            {   Label_Entity_Name.Text = "Attribute";
                Label_Type_Filter.Text = "Parent Name";
                Button_Search.Visible = true;
            } 
            else 
            {   Label_Entity_Name.Text = "Entity Name";
                Label_Type_Filter.Text = "Filter Type";
                Button_Search.Visible = true;
            }

        
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
     

        }


        //===========================//
        private void Combo_Box_Tag_Name_TextChanged(object sender, EventArgs e)
        {
            if (!User_Input) { return; }     
            bool Reset_Type_Filter = false;


            if (Last_Combo_Box_Tag_Name == "Rebalance_Everything") // Auto-Deactivating Percent mode
            { Button_Percentage_Click(null, null); Button_Percentage_MouseLeave(null, null); }


            switch (Combo_Box_Tag_Name.Text)
            {
                case "Planet_Surface_Accessible":
                    Combo_Box_Type_Filter.Text = "Planet";                 
                    break;
                case "Rebalance_Everything":
                    if (!Percent_Mode) { Button_Percentage_Click(null, null); Button_Percentage_MouseLeave(null, null); }               
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
            string Tag_Format = "";
            string[] Tag_Info = new string[] { };
            List<string> List_Of_Tags = new List<string>();


            // The + " #" makes sure the last entry comment stays empty
            foreach (string Phrase in (Input + " #").Split('\n'))
            {
                if (Phrase.StartsWith("//") || Phrase.StartsWith("#") || Phrase == "" || Phrase == "\n" || Phrase == "\r") { continue; } // Skip commented Lines
                else 
                {
                    string Tag_Name = Phrase.Replace(" ", "");
                    string Tag_Comment = ""; // Reset

                    try
                    {
                        if (Phrase.Contains("#"))
                        {   Tag_Info = Phrase.Split('#');
                            Tag_Name = Tag_Info[0].Replace(" ", "");
                            Tag_Comment = Tag_Info[1];
                        }



                        if (Tag_Name.StartsWith("Bool") | Tag_Name.StartsWith("bool"))
                        {   Tag_Format = "bool";
                            // Tag_Name = Tag_Name.Replace("Bool", "");
                            Tag_Name = Tag_Name.Substring(4, Tag_Name.Length - 4);
                        }

                        else if (Tag_Name.StartsWith("String") | Tag_Name.StartsWith("string"))
                        {   Tag_Format = "string";
                            Tag_Name = Tag_Name.Substring(6, Tag_Name.Length - 6);
                        }

                        else if (Tag_Name.StartsWith("Int") | Tag_Name.StartsWith("int"))
                        {   Tag_Format = "int";
                            Tag_Name = Tag_Name.Substring(3, Tag_Name.Length - 3);
                        }
                   


                        // This overwrites the "Tag_Format" from above - which is important for the range of int type Scale_Factor 
                        if (Tag_Name.Contains("=")) // Seperating the tag name and its expected format (int, bool or string)
                        {
                            Tag_Info = Tag_Name.Split('=');
                            Tag_Name = Tag_Info[0];
                            Tag_Format = Tag_Info[1];
                            // iConsole(400, 200, Tag_Name + " + " + Tag_Format);
                        }

                   


                        if (Tag_Name != "") { List_Of_Tags.Add(Tag_Name); };

                        // Loading the list of Tags, the values of all these tags are going to be scalled as group.
                        if (Tag_Name == "Rebalance_Everything") { Balancing_Tags = Tag_Format.Split(','); }



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
            }




            if (Text_Box_Tags.Visible) // Not doing the stuff below while in Settings mode
            {   User_Input = true;
                return List_Of_Tags;
            }
            else if (Tag_Format == "" | Is_Match(Tag_Format, "string"))
            {
                // Button_Operator.Visible = false;
                // Button_Percentage.Visible = false;
                Track_Bar_Tag_Value.Visible = false;
                // Combo_Box_Tag_Value.Items.Clear(); // Disabled

                Bool_Mode = false; Percent_Mode = false;
                Button_Operator_MouseLeave(null, null);
            }
            else if (Is_Match(Tag_Format, "bool"))
            {
                Bool_Mode = true; Percent_Mode = false;
                Button_Operator_MouseLeave(null, null);
                Button_Percentage.Visible = false;
                Track_Bar_Tag_Value.Visible = false;
                
                /* // Disabled because the tool does not know the type of tags the user types in
                if (Combo_Box_Tag_Value.Items.Count == 0)
                {   Combo_Box_Tag_Value.Items.Add("True");
                    Combo_Box_Tag_Value.Items.Add("False");
                    Combo_Box_Tag_Value.Items.Add("");
                    Combo_Box_Tag_Value.Items.Add("Yes");
                    Combo_Box_Tag_Value.Items.Add("No");
                }
                */


                string It = Combo_Box_Tag_Value.Text;

                if (!Is_Match(It, "True") & !Is_Match(It, "False") & !Is_Match(It, "Yes") & !Is_Match(It, "No"))
                { Combo_Box_Tag_Value.Text = ""; }

            }
            else // if (Is_Match(Tag_Format, "int")) // It will probably be int
            {
                if (Percent_Mode) { Scale_Factor = 10; }
                else 
                {
                    int.TryParse(Tag_Format, out Scale_Factor);
                    if (Scale_Factor == 0) { Scale_Factor = 100; } // Failsafe, default Scale Factor is 100
                }
                // iConsole(400, 200, "Scale is " + Scale_Factor);


                Bool_Mode = false;
                Button_Operator_MouseLeave(null, null);

                if (Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "True")| Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "False") 
                    | Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "Yes") | Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "No"))
                { Combo_Box_Tag_Value.Text = ""; }        


                // Resetting the right scale factor
                if (Scale_Factor == 10) { Combo_Box_Type_Filter_TextChanged(null, null); }

                Button_Percentage.Visible = true;
                Track_Bar_Tag_Value.Visible = true;
                // Combo_Box_Tag_Value.Items.Clear(); // Disabled
            }

            User_Input = true;
            return List_Of_Tags;
        }
   


        private void Combo_Box_Tag_Value_TextChanged(object sender, EventArgs e)
        { 
            if (User_Input) 
            {   Disable_Description();
                if (Percent_Mode & !Combo_Box_Tag_Value.Text.Contains("%")) { Combo_Box_Tag_Value.Text += "%"; }

                if (Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "True") | Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "False")
                    | Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "Yes") | Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "No"))
                { Bool_Mode = true; }
                else { Bool_Mode = false; }
            }

            Button_Operator_MouseLeave(null, null); // Check if bool and refresh
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
            foreach (string Line in Text.Split('\n'))
            { New_Text.Add("      " + Line); }

            Display.Text_Box_Caution_Window.Text = string.Join("\n", New_Text); // "\n      " + Text;
            Display.Show();

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


               

            if (Button_D_Text != "false")
            {
                // The first 2 buttons moves aside to free space for this one
                Display.Button_Caution_Box_1.Visible = true;
                Display.Button_Caution_Box_1.Text = Button_A_Text;
                Display.Button_Caution_Box_1.Location = new Point(30, Display.Size.Height - 96);

                Display.Button_Caution_Box_2.Visible = true;
                Display.Button_Caution_Box_2.Text = Button_B_Text;
                Display.Button_Caution_Box_2.Location = new Point(350, Display.Size.Height - 96);

                Display.Button_Caution_Box_3.Visible = true;
                Display.Button_Caution_Box_3.Text = Button_C_Text;
                Display.Button_Caution_Box_3.Location = new Point(190, Display.Size.Height - 96);

                Display.Button_Caution_Box_4.Visible = true;
                Display.Button_Caution_Box_4.Text = Button_D_Text;
                Display.Button_Caution_Box_4.Location = new Point(510, Display.Size.Height - 96);
            }
            else if (Button_C_Text != "false")
            {
                // The first 2 buttons moves aside to free space for this one
                Display.Button_Caution_Box_1.Visible = true;
                Display.Button_Caution_Box_1.Text = Button_A_Text;
                Display.Button_Caution_Box_1.Location = new Point(60, Display.Size.Height - 96);

                Display.Button_Caution_Box_2.Visible = true;
                Display.Button_Caution_Box_2.Text = Button_B_Text;
                Display.Button_Caution_Box_2.Location = new Point(380, Display.Size.Height - 96);

                Display.Button_Caution_Box_3.Visible = true;
                Display.Button_Caution_Box_3.Text = Button_C_Text;
                Display.Button_Caution_Box_3.Location = new Point(220, Display.Size.Height - 96);
            }

            else if (Button_B_Text != "false")
            {
                Display.Button_Caution_Box_2.Visible = true;
                Display.Button_Caution_Box_2.Text = Button_B_Text;
                Display.Button_Caution_Box_2.Location = new Point(280, Display.Size.Height - 96);
            }


            if (Button_A_Text != "false" & Button_C_Text == "false")
            {
                Display.Button_Caution_Box_1.Visible = true;
                Display.Button_Caution_Box_1.Text = Button_A_Text;
                Display.Button_Caution_Box_1.Location = new Point(120, Display.Size.Height - 96);
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
                Display.List_Exclusion_Mode = List_Exclusion_Mode;
                Display.List_View_Info.Visible = true;
                Display.List_View_Info.Items.Add(Text); // Text serves as Header here
                Display.List_View_Info.Items.Add("");
                Display.List_View_Info.Items.Add("");

                foreach (string Entry in The_List)
                { Display.List_View_Info.Items.Add(Entry); }

                Set_Checker(Display.List_View_Info);
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

            return null;
        }


        //===========================//
        public void Set_Checker(ListView The_Box)
        {
            // Sorting here instead of the default property because of timing issues with the loop below
            The_Box.Sort();

            foreach (ListViewItem Item in The_Box.Items)
            {   // Every second Tag should have this value in order to create a checkmate pattern with good contrast
                if (Item.Index % 2 == 0)
                {   Item.ForeColor = Color.White;
                    Item.BackColor = Theme_Color; 
                }
                else
                {   Item.ForeColor = Color.Black;
                    Item.BackColor = Color.LightGray;
                }
            }
        }


        //===========================//

        // Move_Unused_Files("models"); Move_Unused_Files("textures"); or just Move_Unused_Files(); to move all of them
        private void Move_Unused_Files(string Mode = "all")
        {                      
            string Model_Directory = Properties.Settings.Default.Mod_Directory + @"\Data\Art\Models";
            string Texture_Directory = Properties.Settings.Default.Mod_Directory + @"\Data\Art\Textures";
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
        private void Button_Search_Click(object sender, EventArgs e)
        {
            // iConsole(400, 100, Properties.Settings.Default.Xml_Directory + "\n" + Properties.Settings.Default.Mod_Directory); return;


            string Entity_Name = Wash_String(Combo_Box_Entity_Name.Text);
            if (Entity_Name == "" | Entity_Name == "None") { return; }


            // Matched in selected XML, so just show that one
            if (Found_In_Xml(Entity_Name)) { return; } 




            IEnumerable<XElement> Instances = null;


            foreach (var Xml in Get_All_Files(Xml_Directory, "xml"))
            {
                try
                {   // ===================== Opening Xml File =====================                            
                    XDocument Xml_File = XDocument.Load(Xml, LoadOptions.PreserveWhitespace);

                    Instances =
                      from All_Tags in Xml_File.Root.Descendants()
                      where (string)All_Tags.Attribute(Queried_Attribute) == Entity_Name // Fast Search                    
                      select All_Tags;


                    if (Instances.Any() & File.Exists(Xml)) { Set_Paths(Xml, false); Combo_Box_Entity_Name.Text = Entity_Name; break; }
                }
                catch { }
            }


            if (!Instances.Any())
            {
                foreach (var Xml in Get_All_Files(Xml_Directory, "xml"))
                {   try
                    {   XDocument Xml_File = XDocument.Load(Xml, LoadOptions.PreserveWhitespace);

                        Instances =
                          from All_Tags in Xml_File.Root.Descendants()
                          // Regex; This is damn slow - but it delivers results
                          where Is_Match((string)All_Tags.Attribute(Queried_Attribute), Entity_Name)
                          select All_Tags;


                        if (Instances.Any() & File.Exists(Xml)) { Set_Paths(Xml); break; } // Don't select Entity_Name here because its spelled wrong 
                    } catch {}
                }
            }

            Found_In_Xml(Entity_Name); // Just to select the found entity
        }


        private void Button_Search_MouseHover(object sender, EventArgs e)
        {
            Set_Resource_Button(Button_Search, Properties.Resources.Button_Search_Lit);
        }

        private void Button_Search_MouseLeave(object sender, EventArgs e)
        { Set_Resource_Button(Button_Search, Properties.Resources.Button_Search); }



        private bool Found_In_Xml(string Entity_Name)
        {
            if (In_Selected_Xml(Entity_Name))
            {
                List_View_Selection.Visible = true;

                if (List_View_Selection.Items.Count > 0) // Auto selecting the item
                {
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
            if (Percent_Mode) 
            {   Percent_Mode = false;
                User_Input = false; // Un-Percenting
                Combo_Box_Tag_Value.Text = Remove_Operators(Combo_Box_Tag_Value.Text);
                User_Input = true;

                if (Text_Box_Description.Text.StartsWith("While in Percent Mode")) { Text_Box_Description.Visible = false; }
            }
            else 
            {   Percent_Mode = true; // Needs to be set BEFORE Process_Tags()
                Combo_Box_Tag_Value.Text = ""; // Prepare for percent input
                Process_Tags(Text_Box_Tags.Text);

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
        {   if (Percent_Mode) { Set_Resource_Button(Button_Percentage, Properties.Resources.Button_Percent_Lit);  }
            else { Set_Resource_Button(Button_Percentage, Properties.Resources.Button_Percent); }
        }




        private void Button_Scripts_Click(object sender, EventArgs e)
        {
            if (Script_Mode) // Toggle between Script Mode
            {
                Script_Mode = false;
                Drop_Zone.Visible = true;
                Set_UI_Into_Script_Mode(!Script_Mode);

                List_View_Selection.Size = new Size(404, 164);
                List_View_Selection.Location = new Point(12, 12);
             
                Button_Browse.Location = new Point(1, 193);

                // Loading the Xml instead of the available scripts in script mode
                Load_Xml_Content(Properties.Settings.Default.Last_File);
                Set_Resource_Button(Button_Start, Properties.Resources.Button_Logs_Lit);
            }
            else
            {
                Script_Mode = true;
                Drop_Zone.Visible = false;
                List_View_Selection.Visible = true;
                Set_UI_Into_Script_Mode(!Script_Mode);

                List_View_Selection.Size = new Size(367, 482);
                List_View_Selection.Location = new Point(31, 29);

                Button_Browse.Location = new Point(1, 350);


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

            try { List_View_Selection.Columns[0].Width = List_View_Selection.Width - 8; } catch {}
                
        }


        private void Set_UI_Into_Script_Mode(bool Mode)
        {
            Control[] Controls = { Button_Start, Button_Run, Button_Search, 
                                   Button_Percentage, Button_Operator, Button_Toggle_Settings };
            foreach (Control Selectrion in Controls) { Selectrion.Visible = Mode; } // Hide or show all        
        }


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
                                if (Text_Format_Delimiter == "\\t") { Extension = "tsv"; }
                                else if (Text_Format_Delimiter == "," | Text_Format_Delimiter == ";") { Extension = "csv"; }

                                Execute(File_Path, "\"" + Properties.Settings.Default.Mod_Directory + "\\Data\" " + Text_Format_Delimiter + " " + Extension, Script_Directory);                               

                                // Alternate way: Specify path to the .json file as argument:
                                // Execute(File_Path, "\"" + Properties.Settings.Default.Mod_Directory + "\\Data\"" + " " + Script_Directory + "\\vanilla_dump.json");

                                // iConsole(400, 100, File_Path + " \"" + Properties.Settings.Default.Mod_Directory + "\\Data\"" + "\npause");  
                                return;                            
                            }
                        }
                        
                    
                                                                            


                        string Vanilla_Dir = Path.GetDirectoryName(Properties.Settings.Default.Steam_Exe_Path) + @"\steamapps\common\Star Wars Empire at War\Data";
                        Execute(File_Path, "\"" + Properties.Settings.Default.Mod_Directory + "\\Data\" \"" + Vanilla_Dir + "\"", Script_Directory);

                        Possible_Files = new string[] { "Mod_Cleanup.py", "01_mod_cleanup.py", "Test.py" };

                        foreach (string File_Name in Possible_Files)
                        {
                            if (Match_Without_Emptyspace_2(Selection, File_Name)) // Append the Modpath to the Cleanup Script                             
                            {

                                Temporal_E = File.ReadAllLines(Properties.Settings.Default.Mod_Directory + @"\Data\cleanup.txt").ToList();
                                int Line_Count = (Temporal_E.Count() * 30) + 160;
                                if (Line_Count > 680) { Line_Count = 680; }

                                iDialogue(680, Line_Count, "Yes, All", "Textures", "Models only", "Cancel", "Do you wish to move these into a \"Unused\" folder:", Temporal_E);

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


        private string Get_Scripts_Dir(bool check_string = false)
        {
            string Script_Directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Xml_Axe\Scripts";
            string New_Script_Directory = Script_Directory;

            // Overwride by User Setting
            if (!Match_Without_Emptyspace_2(Properties.Settings.Default.Tags, @"Script_Directory = %AppData%\Local\Xml_Axe\Scripts"))
            {
                foreach (string Line in Properties.Settings.Default.Tags.Split('\n'))
                { if (Line != "" & Line.Contains("Script_Directory")) { Script_Directory = Line.Split('=')[1]; break; } }


                if (Script_Directory == "") // Then the original path in Environment will be chosen
                { return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Xml_Axe\Scripts"; }

                else 
                {   // Removing empty space
                    if (Script_Directory[0] == ' ') { Script_Directory = Remove_Emptyspace_Prefix(Script_Directory); }

                    // Pretty stupid but this helps preventing errors lol

                    if (!Script_Directory.EndsWith("Scripts")) { New_Script_Directory = Script_Directory + "\\Scripts"; }
                    else { New_Script_Directory = Script_Directory; }
                }

                if (check_string)
                {   Properties.Settings.Default.Tags = Properties.Settings.Default.Tags.Replace(Script_Directory, New_Script_Directory);
                    Properties.Settings.Default.Save();

                    Tag_List = Properties.Settings.Default.Tags;
                    Text_Box_Tags.Text = Tag_List;
                }              

                // iConsole(600, 200, "\"" + Script_Directory + "\""); return;
            }

            return New_Script_Directory;
        }

        private void Button_Scripts_MouseHover(object sender, EventArgs e)
        { Set_Resource_Button(Button_Scripts, Properties.Resources.Button_Flash_Lit); }

        private void Button_Scripts_MouseLeave(object sender, EventArgs e)
        {   if (Script_Mode) { Set_Resource_Button(Button_Scripts, Properties.Resources.Button_Flash_Lit); }
            else { Set_Resource_Button(Button_Scripts, Properties.Resources.Button_Flash); }
        }



      
     

    



        private void Button_Operator_Click(object sender, EventArgs e)
        {   
            if (Bool_Mode)
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
            else if (Combo_Box_Tag_Value.Text != "") // & Percent_Mode)  // Toggle
            {
                string The_Value = Combo_Box_Tag_Value.Text;

                if (The_Value.Contains("-")) { Combo_Box_Tag_Value.Text = The_Value.Replace("-", "+"); }
                else if (The_Value.Contains("+")) { Combo_Box_Tag_Value.Text = The_Value.Replace("+", "-"); }
                else { Combo_Box_Tag_Value.Text = "-" + The_Value; }               
            }

        }

        private void Button_Operator_MouseHover(object sender, EventArgs e)
        {
            if (Bool_Mode)
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
            if (Bool_Mode)
            {   if (Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "True") | Match_Without_Emptyspace(Combo_Box_Tag_Value.Text, "Yes")) 
                { Set_Resource_Button(Button_Operator, Properties.Resources.Button_Bool_Lit); }
                else { Set_Resource_Button(Button_Operator, Properties.Resources.Button_Bool); }
            }
            else
            {   if (Combo_Box_Tag_Value.Text.Contains("-")) { Set_Resource_Button(Button_Operator, Properties.Resources.Button_Plus); }
                else { Set_Resource_Button(Button_Operator, Properties.Resources.Button_Minus); }
            }
        }




     



     


    
        //===========================//



    }
}
