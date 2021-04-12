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



// ====================================================================================================
// **************************************** 03.2021 Imperial ******************************************
// ====================================================================================================



namespace Log_Compactor
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
        bool Bool_Mode = false;
        string Temporal_A, Temporal_B = "";
        string[] Balancing_Tags = null; // new string[] { };
        public Color Theme_Color = Color.CadetBlue;
        public string Xml_Directory = Properties.Settings.Default.Xml_Directory;
        public List<string> File_Collection = new List<string>();
        public List<string> Blacklisted_Xmls = null;


        private void Window_Load(object sender, EventArgs e)
        {
            Drop_Zone.AllowDrop = true;
            List_View_Selection.AllowDrop = true;
  

            Set_Resource_Button(Drop_Zone, Get_Start_Image()); 
            Set_Resource_Button(Button_Browse, Properties.Resources.Button_File);
            Set_Resource_Button(Button_Start, Properties.Resources.Button_Logs);
            Set_Resource_Button(Button_Search, Properties.Resources.Button_Search);
            Set_Resource_Button(Button_Percentage, Properties.Resources.Button_Percent);
            Set_Resource_Button(Button_Operator, Properties.Resources.Button_Minus);
            Set_Resource_Button(Button_Run_Game, Properties.Resources.Button_Axe);
            Set_Resource_Button(Button_Toggle_Settings, Properties.Resources.Button_Settings);
            Set_Resource_Button(Button_Reset_Blacklist, Properties.Resources.Button_Controller);

            Button_Browse.BackColor = Color.Transparent;
            Button_Start.BackColor = Color.Transparent;
            Button_Run_Game.BackColor = Color.Transparent;
            Button_Search.BackColor = Color.Transparent;
            Button_Percentage.BackColor = Color.Transparent; 
            Button_Operator.BackColor = Color.Transparent; 
            Button_Reset_Blacklist.BackColor = Color.Transparent;
            Button_Toggle_Settings.BackColor = Color.Transparent;
    
     

            if (File.Exists(Xml_Directory + "Axe_Blacklist.txt"))
            { Blacklisted_Xmls = File.ReadAllLines(Xml_Directory + "Axe_Blacklist.txt").ToList(); }

          


            Tag_List = Properties.Settings.Default.Tags;
            if (Tag_List == null | Tag_List == "") { Reset_Tag_List(); }
            Text_Box_Tags.Text = Tag_List;
            Reset_Tag_Box();



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
                        Load_Xml_Content(The_Path);
                    }


                    // Don't use else if here!
                    if (!Is_Match(Temporal_A, "xml$")) { Temporal_A = Path.GetDirectoryName(Temporal_A); }
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
            {   Load_Xml_Content(The_Path);
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
        }


        //===========================//
        private void Button_Browse_Click(object sender, EventArgs e)
        {    
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

        private void Button_Run_Game_Click(object sender, EventArgs e)
        {
            iConsole(600, 400, Check_for_Steam_Version());

            if (Combo_Box_Tag_Name.Text == "") { return; }
            
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

                iDialogue(680, Line_Count, "Yes", "Cancel", "Ignore", "Blacklist", "Are you sure you wish to apply changes to:", Related_Xmls);
                 
  
                if (Caution_Window.Passed_Value_A.Text_Data == "false") { return; }          
            }


            Related_Xmls = Slice(true); // This line does the actual Job!
            Set_Resource_Button(Drop_Zone, Get_Compacted_Image());
            if (List_View_Selection.Visible) { Button_Start_Click(null, null); } // Hiding open Xml

            if (Combo_Box_Entity_Name.Text == "None") { Properties.Settings.Default.Entity_Name = ""; }
            else { Properties.Settings.Default.Entity_Name = Wash_String(Combo_Box_Entity_Name.Text); }

            Properties.Settings.Default.Type_Filter = Combo_Box_Type_Filter.Text;
            Properties.Settings.Default.Tag_Name = Combo_Box_Tag_Name.Text;
            Properties.Settings.Default.Tag_Value = Combo_Box_Tag_Value.Text;                                
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

        private void Button_Run_Game_MouseHover(object sender, EventArgs e)
        { Set_Resource_Button(Button_Run_Game, Properties.Resources.Button_Axe_Lit); }

        private void Button_Run_Game_MouseLeave(object sender, EventArgs e)
        { Set_Resource_Button(Button_Run_Game, Properties.Resources.Button_Axe); }



        private bool Match_Setting(string Entry)
        {   if (Match_Without_Emptyspace(Properties.Settings.Default.Tags, Entry + " = true")) { return true; }
            return false;
        }


        private bool Match_Without_Emptyspace(string Entry_1, string Entry_2)
        {
            if (Regex.IsMatch(Entry_1.Replace(" ", ""), "(?i)" + Entry_2.Replace(" ", ""))) { return true; }
            return false;
        }

      
        private bool Is_Match(string Entry_1, string Entry_2)
        {
            if (Regex.IsMatch(Entry_1, "(?i)" + Entry_2)) { return true; }
            return false;
        }


        // public List <string> Load_Xml_Content(string Xml_Path)
        public void Load_Xml_Content(string Xml_Path)
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
                        string Entity_Name = (string)Instance.Attribute("Name");

                        if (Entity_Name != null)
                        {
                            if (!Match_Without_Emptyspace(Entity_Name, "Death_Clone") & !List_View_Matches(List_View_Selection, Entity_Name))
                            { List_View_Selection.Items.Add(Entity_Name); }
                        }
                    }
                }

                Set_Checker(List_View_Selection);
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
            { iConsole(200, 100, "\nCan't find the Xml."); return null; }



            if (Apply_Changes) // File_Collection is a global variable, feeded from the remaining filenames in the Caution_Window 
            {   if (File_Collection == null | File_Collection.Count == 0) { File_Collection = Get_Xmls(); } // Failsafe
                // iConsole(560, 600, Xml_Directory + string.Join("\n", File_Collection)); // return null; // Debug Code
            } else { File_Collection = Get_Xmls(); }



            foreach (var Xml in File_Collection)
            {   try
                {
                    Selected_Xml = Xml;
                    // Ignoring blacklisted Xmls
                    if (Blacklisted_Xmls != null) { if (Blacklisted_Xmls.Contains(Selected_Xml.Replace(Xml_Directory, ""))) { continue; } }
                    
                    

                    XDocument Xml_File = XDocument.Load(Selected_Xml, LoadOptions.PreserveWhitespace);
                    
                    // ===================== Opening Xml File =====================

                    if (In_Selected_Xml(Entity_Name)) // Select Multiple by Name
                    {
                        Selected_Xml = Properties.Settings.Default.Last_File; // Overwride what ever xml this loop selected before
                        Xml_File = XDocument.Load(Selected_Xml, LoadOptions.PreserveWhitespace);
                        List<string> Selected_Entities = Select_List_View_Items(List_View_Selection);

                       Instances =
                         from All_Tags in Xml_File.Root.Descendants()
                         where List_Matches(Selected_Entities, (string)All_Tags.Attribute("Name"))
                         select All_Tags;
                    }


                    else if (Combo_Box_Type_Filter.Text == "Faction Name Filter")
                    {
                        Instances =
                           from All_Tags in Xml_File.Root.Descendants() // Entity_Name means the Faction name here
                           where All_Tags.Descendants("Affiliation").Last().Value.Contains(Entity_Name)
                           select All_Tags; // Last() because it overwrites the first occurances ingame
                    }
                    else if (Entity_Name != "" & Entity_Name != "None") // Select a single Entity by Name
                    {
                        Instances =
                          from All_Tags in Xml_File.Root.Descendants()
                          where (string)All_Tags.Attribute("Name") == Entity_Name
                          select All_Tags;
                    }
                    else if (Combo_Box_Type_Filter.Text != "" & Combo_Box_Type_Filter.Text != "All Types") // By Entity Type
                    {
                        Instances =
                          from All_Tags in Xml_File.Root.Descendants()
                          // Selecting all non empty tags that have the Attribute "Name", null because we need all selected.
                          where All_Tags.Name == Selected_Type
                          select All_Tags;
                    }
                    else // Target all entities in the whole Mod!
                    {
                        Instances =
                          from All_Tags in Xml_File.Root.Descendants()
                          // Selecting all non empty tags that have the Attribute "Name", null because we need all selected.
                          where All_Tags != null
                          select All_Tags;
                    }





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
                            if (!Percent_Mode) { Target.Value = Combo_Box_Tag_Value.Text.Replace("+", ""); }

                            else try // Refactoring the old value!
                            {   int Percentage = 0;
                                decimal Original_Value = 0;

                                Decimal.TryParse(Target.Value, out Original_Value);
                                Int32.TryParse(Remove_Percentage(Combo_Box_Tag_Value.Text), out Percentage);

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
            {   if (Combo_Box_Type_Filter.Text != "Faction Name Filter") { Button_Search.Visible = true; }
                Text_Box_Tags.Visible = false;              
                Button_Run_Game.Visible = true; 
                Button_Percentage.Visible = true;
                Button_Operator.Visible = true; 
                Label_Type_Filter.Visible = true;
                Set_Resource_Button(Button_Reset_Blacklist, Properties.Resources.Button_Controller);

                if (Combo_Box_Type_Filter.Text == "Faction Name Filter") { Label_Entity_Name.Text = "Faction Name"; }
                else { Label_Entity_Name.Text = "Entity Name"; }

                if (Tag_List != Text_Box_Tags.Text) // Then needs to update
                {   Properties.Settings.Default.Tags = Text_Box_Tags.Text;
                    Tag_List = Text_Box_Tags.Text;
                }
                Properties.Settings.Default.Save();               
                Reset_Tag_Box();
                return;
            }
            else
            {   Button_Search.Visible = false; 
                Text_Box_Tags.Visible = true;                             
                Button_Run_Game.Visible = false;
                Button_Percentage.Visible = false;
                Button_Operator.Visible = false; 
                Label_Type_Filter.Visible = false;
                Set_Resource_Button(Button_Reset_Blacklist, Properties.Resources.Button_Refresh);
                Label_Entity_Name.Text = "List of Tags";            
                Text_Box_Tags.Focus(); // So the user can scroll
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
        private void Button_Reset_Blacklist_Click(object sender, EventArgs e)
        {
            if (Text_Box_Tags.Visible)
            {
                iDialogue(540, 200, "Do It", "Cancel", "false", "false", "\nAre you sure you wish reset the List of Tags?");
                if (Caution_Window.Passed_Value_A.Text_Data == "false") { return; }

                Reset_Tag_List();
                Reset_Tag_Box();
                Text_Box_Tags.Text = Tag_List;
                if (Text_Box_Tags.Visible == false)
                { Text_Box_Tags.Visible = true; }
            }
            else // Otherwised this button is used to launch the game
            {                              
                string Steam_Path = Properties.Settings.Default.Steam_Exe_Path; 
                string Program_Path = Path.GetDirectoryName(Steam_Path) + @"\steamapps\common\Star Wars Empire at War\corruption\StarWarsG.exe";
              
                string Mod_Name = Path.GetFileName(Properties.Settings.Default.Mod_Directory);
                string Arguments = @"Modpath=Mods\" + Mod_Name + " Ignoreasserts";


                // Overwride by User Setting
                if (!Match_Without_Emptyspace(Properties.Settings.Default.Tags, "Custom_Modpath = false"))
                {
                    string User_Path = "";

                    foreach (string Line in Properties.Settings.Default.Tags.Split('\n'))
                    { if (Line != "" & Line.Contains("Custom_Modpath")) { User_Path = Line.Split('=')[1]; } }

                    Arguments = "Modpath=" + User_Path + " Ignoreasserts";
                }
                else if (Mod_Name.All(char.IsDigit)) // If all characters are numbers, that means we target a Workshop mod
                {   // So argument 1 targets now the Workshop dir            
                    // Arguments = @"Modpath=..\..\..\workshop\content\32470\" + Mod_Name; 
                    Arguments = @"Steammod=" + Mod_Name + " Ignoreasserts";              
                }

        

                bool Affinity = false;
                bool Priority = false;

                if (Match_Setting("Set_Launch_Affinity")) {Affinity = true; }
                if (Match_Setting("High_Launch_Priority")) { Priority = true; }
                          
                // Now, that we made sure Steam is running, we can launch the game           
                if (Check_Process(Steam_Path)) { Check_Process(Program_Path, Arguments, Affinity, Priority); }        
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

                    iConsole(60, 100, Program_Path + " " + Arguments); // return true;
                    //The_Process[0] = Process.Start(Process_Info);
                    Process.Start(Process_Info);

                    The_Process = Process.GetProcessesByName(Program_Name); // Retrieve the app processes. 
                } catch { iConsole(60, 100, "\nFailed to find and launch " + Program_Name); return false; }
            }

            


            if (Set_Affinity | High_Priority) //Process[] The_Processes; 
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


        public Bitmap Get_Compacted_Image()
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


        //===========================// # Show_Changed_Files = true
        public void Reset_Tag_List()
        {
            Tag_List = @"# I am a Setting (as parameter 1) or Comment (as parameter 2 or 3)
# The optional @ sign introduces the expected type for a tag: bool, string or a int number as scrollbar factor


# Show_Tooltip = true
# Store_Last_Settings = true
# Request_File_Approval = true
# Set_Launch_Affinity = true
# High_Launch_Priority = true
# RGBA_Color = 100, 170, 170, 255 # Marine Blue
# Custom_Modpath = false


Planet_Surface_Accessible @ bool # Set to No and it will turn all GCs to space only because it sets all Planets to unaccessible. This operation is revertable: it checks if a planet has a ground.ted map to determine whether it is safe to set surface back to accessible.

Rebalance_Everything @ Tactical_Health, Shield_Points, Shield_Refresh_Rate # This balances the most important aspects of the Game: Tactical_Health, Shield, Shield_Refresh_Rate, Projectile Damage

Is_Targetable @ bool # Defines whether or not all Hardpoints in current selection or the Mod can be targeted.

Is_Destroyable @ bool # Defines whether or not all Hardpoints in current selection or the Mod can be destroyed.

Is_Named_Hero @ bool # Set to No and no more heroes will respawn.

Projectile_Does_Shield_Damage @ bool # Set to Yes and apply to the whole mod, to disable all shield piercing effects.

Projectile_Does_Energy_Damage @ bool # Careful, if set to yes Hitpoint damage will start once enemy energy level reaches 0.

Projectile_Does_Hitpoint_Damage @ bool 

# ==================== Int Values ====================

Tactical_Health @ 100

Shield_Points @ 100

Shield_Refresh_Rate @ 5 # Usually about 30 for capital ships and less for weaker classes.

Select_Box_Scale @ 100 # Set to 0 and all Ships and Troops will have their select box deactivated (not revertable). In percent mode this scales the size of all select box circles.

Layer_Z_Adjust @ 100 # In percent mode this will scale the distance between all height layer values.

Space_Tactical_Unit_Cap @ 10 # Sets Unit cap in space tactical battles, for all Factions in the Mod. Don't put too high or it will cause laggs.

Build_Cost_Credits @ 100 # Set the price to 1, then you can build as many units as the population cap allows.

Tactical_Build_Cost_Multiplayer @ 100 # Set the price to 1 for all Skirmish units.
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
                Button_Search.Visible = false;
            }
            else 
            {   Label_Entity_Name.Text = "Entity Name";
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
            
        }



        private List<string> Process_Tags(string Input)
        {
            User_Input = false;
            string Tag_Format = "";
            string[] Tag_Info = new string[] { };
            List<string> List_Of_Tags = new List<string>();


            // The + " #" makes sure the last entry comment stays empty
            foreach (string Phrase in (Input + " #").Split('\n'))
            {
                if (Phrase.StartsWith("//") || Phrase.StartsWith("#") || Phrase == "") { continue; } // Skip commented Lines
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

                        if (Tag_Name.Contains("@")) // Seperating the tag name and its expected format (int, bool or string)
                        {
                            Tag_Info = Tag_Name.Split('@');
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
                                // Removing empty space
                                if (Tag_Comment[0] == ' ') { Tag_Comment = Tag_Comment.Substring(1, Tag_Comment.Length - 1); }

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
                Combo_Box_Tag_Value.Items.Clear();

                Bool_Mode = false; Percent_Mode = false;
                Button_Operator_MouseLeave(null, null);
            }
            else if (Is_Match(Tag_Format, "bool"))
            {
                Bool_Mode = true; Percent_Mode = false;
                Button_Operator_MouseLeave(null, null);
                Button_Percentage.Visible = false;
                Track_Bar_Tag_Value.Visible = false;
                
                if (Combo_Box_Tag_Value.Items.Count == 0)
                {   Combo_Box_Tag_Value.Items.Add("True");
                    Combo_Box_Tag_Value.Items.Add("False");
                    Combo_Box_Tag_Value.Items.Add("");
                    Combo_Box_Tag_Value.Items.Add("Yes");
                    Combo_Box_Tag_Value.Items.Add("No");
                }


                string It = Combo_Box_Tag_Value.Text;

                if (!Is_Match(It, "True") & !Is_Match(It, "False") & !Is_Match(It, "Yes") & !Is_Match(It, "No"))
                { Combo_Box_Tag_Value.Text = ""; }

            }
            else // It will probably be int
            {
                if (Percent_Mode) { Scale_Factor = 10; }
                else { int.TryParse(Tag_Format, out Scale_Factor); }
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
                Combo_Box_Tag_Value.Items.Clear();
            }

            User_Input = true;
            return List_Of_Tags;
        }
   


        private void Combo_Box_Tag_Value_TextChanged(object sender, EventArgs e)
        { 
            if (User_Input) 
            {   Disable_Description();
                if (Percent_Mode & !Combo_Box_Tag_Value.Text.Contains("%")) { Combo_Box_Tag_Value.Text += "%"; }
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

        public void iDialogue(int Window_Size_X, int Window_Size_Y, string Button_A_Text, string Button_B_Text, string Button_C_Text, string Button_D_Text, string Text, List<string> The_List = null)
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
                Display.List_Exclusion_Mode = true;
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
        public List<string> Get_Xmls()
        {
            List<string> All_Xmls = new List<string>();          
            string Error = "\nPlease dragg and drop any target Xml, \nor the Xml directory of your mod into the Dropzone.";

            if (Xml_Directory == "" | Xml_Directory == null)
            { iConsole(600, 100, Error); return null; }

            
            try
            {   if (Directory.Exists(Xml_Directory))
                {   foreach (string The_File in Directory.GetFiles(Xml_Directory, "*.xml", System.IO.SearchOption.AllDirectories))
                    { All_Xmls.Add(The_File); }
                } else { iConsole(600, 100, Error); return null; }
                
            } catch {}


            return All_Xmls;
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



        private void Button_Percentage_Click(object sender, EventArgs e)
        {
            if (Percent_Mode) 
            {   Percent_Mode = false;
                User_Input = false; // Un-Percenting
                Combo_Box_Tag_Value.Text = Remove_Percentage(Combo_Box_Tag_Value.Text);
                User_Input = true;
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

        private string Remove_Percentage(string The_Text)
        { return The_Text.Replace("+", "").Replace("-", "").Replace("%", ""); }

        private void Button_Percentage_MouseHover(object sender, EventArgs e)
        { Set_Resource_Button(Button_Percentage, Properties.Resources.Button_Percent_Lit); }

        private void Button_Percentage_MouseLeave(object sender, EventArgs e)
        {   if (Percent_Mode) { Set_Resource_Button(Button_Percentage, Properties.Resources.Button_Percent_Lit);  }
            else { Set_Resource_Button(Button_Percentage, Properties.Resources.Button_Percent); }
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




        private void Button_Search_Click(object sender, EventArgs e)
        {
            string Entity_Name = Wash_String(Combo_Box_Entity_Name.Text);
            if (Entity_Name == "" | Entity_Name == "None") { return; }

           
            IEnumerable<XElement> Instances = null;
  
            
            foreach (var Xml in Get_Xmls())
            {   try
                {   // ===================== Opening Xml File =====================                            
                    XDocument Xml_File = XDocument.Load(Xml, LoadOptions.PreserveWhitespace);
                                                      
                    Instances =
                      from All_Tags in Xml_File.Root.Descendants()
                        where (string)All_Tags.Attribute("Name") == Entity_Name // Fast Search                    
                          select All_Tags;


                    if (Instances.Any() & File.Exists(Xml)) { Set_Paths(Xml, false); Combo_Box_Entity_Name.Text = Entity_Name; break; } 
                } catch {}
            }


            if (!Instances.Any())
            {   foreach (var Xml in Get_Xmls())
                {   try
                    {   XDocument Xml_File = XDocument.Load(Xml, LoadOptions.PreserveWhitespace);

                        Instances =
                          from All_Tags in Xml_File.Root.Descendants()
                          // Regex; This is damn slow - but it delivers results
                          where Is_Match((string)All_Tags.Attribute("Name"), Entity_Name)
                          select All_Tags;


                        if (Instances.Any() & File.Exists(Xml)) { Set_Paths(Xml); break; } // Don't select Entity_Name here because its spelled wrong 
                    } catch {}
                }
            }


              
        }


        private void Button_Search_MouseHover(object sender, EventArgs e)
        {
            Set_Resource_Button(Button_Search, Properties.Resources.Button_Search_Lit);
        }

        private void Button_Search_MouseLeave(object sender, EventArgs e)
        { Set_Resource_Button(Button_Search, Properties.Resources.Button_Search); }




    
        //===========================//



    }
}
