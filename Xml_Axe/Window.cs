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
        string Temporal_A, Temporal_B = "";
        public string Xml_Directory = Properties.Settings.Default.Xml_Directory;
        public List<string> File_Collection = new List<string>();
        string[] Tag_Info;

        private void Window_Load(object sender, EventArgs e)
        {
            Drop_Zone.AllowDrop = true;
            List_View_Selection.AllowDrop = true;
            Text_Box_Description.AllowDrop = true;
            

            Set_Resource_Button(Drop_Zone, Get_Start_Image());
            Set_Resource_Button(Button_Browse, Properties.Resources.Button_File);
            Set_Resource_Button(Button_Start, Properties.Resources.Button_Logs);
            Set_Resource_Button(Button_Run_Game, Properties.Resources.Button_Run);
            Set_Resource_Button(Button_Toggle_Settings, Properties.Resources.Button_Settings);
            Set_Resource_Button(Button_Reset_Blacklist, Properties.Resources.Button_Refresh);

            Button_Browse.BackColor = Color.Transparent;
            Button_Start.BackColor = Color.Transparent;
            Button_Run_Game.BackColor = Color.Transparent;           
            Button_Reset_Blacklist.BackColor = Color.Transparent;
            Button_Toggle_Settings.BackColor = Color.Transparent;
    
     


            // Loading Values
            Text_Box_Original_Path.Text = Properties.Settings.Default.Last_File;
            Track_Bar_Tag_Value.Value = Properties.Settings.Default.Trackbar_Value;



            Tag_List = Properties.Settings.Default.Tags;
            if (Tag_List == null | Tag_List == "") { Reset_Tag_List(); }
            Text_Box_Tags.Text = Tag_List;
            Reset_Tag_Box();
         
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
                        { MessageBox.Show("Error: the file needs to either be a folder or of .xml format."); }
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
        { Text_Box_Description.Visible = false; } // Hiding




        private void Text_Box_Description_Click(object sender, EventArgs e)
        {
            Text_Box_Description.Text = "";
            Text_Box_Description.Visible = false;
        }

        //===========================// 

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
                        { MessageBox.Show("Error: the file needs to either be a folder or of .xml format."); }
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

        private void Set_Paths(string The_Path)
        {
            Combo_Box_Entity_Name.Text = ""; // Resetting
            Text_Box_Original_Path.Text = The_Path;
            Properties.Settings.Default.Last_File = The_Path;
            Temporal_A = Path.GetDirectoryName(The_Path);

            for (int i = 0; i < 10; i++)
            {
                // Keep removing the last directory in the path,
                if (Regex.IsMatch(Temporal_A, "(?i).*?" + "xml$")) 
                {   List_View_Selection.Visible = true;
                    Load_Xml_Content(The_Path);
                }


                // Don't use else if here!
                if (!Regex.IsMatch(Temporal_A, "(?i).*?" + "xml$")) { Temporal_A = Path.GetDirectoryName(Temporal_A); }
                else if (Regex.IsMatch(Temporal_A, "(?i).*?" + "data$"))
                {
                    Xml_Directory = Temporal_A + @"\xml\"; // Updating 
                    Properties.Settings.Default.Xml_Directory = Temporal_A + @"\xml\";                 
                    // Leaping back for a directy, to get the name of the Modpath
                    Properties.Settings.Default.Mod_Directory = Path.GetDirectoryName(Temporal_A); ;

                    // MessageBox.Show(Path.GetDirectoryName(Path.GetDirectoryName(Temporal_A)));
                    break;
                }

                else //Until we got to the Xml directory
                {
                    Xml_Directory = Temporal_A + @"\"; // Updating 
                    Properties.Settings.Default.Xml_Directory = Temporal_A + @"\";


                    // Leaping back by 2 directoies, to get the name of the Modpath
                    Properties.Settings.Default.Mod_Directory = Path.GetDirectoryName(Path.GetDirectoryName(Temporal_A)); ;

                    // MessageBox.Show(Path.GetDirectoryName(Path.GetDirectoryName(Temporal_A)));
                    break;
                }
            }


            Set_Resource_Button(Button_Start, Properties.Resources.Button_Logs_Lit);
            Set_Checker(List_View_Selection);

            if (Text_Box_Description.Visible) // Hide
            {
                Text_Box_Description.Text = "";
                Text_Box_Description.Visible = false;
            }

         
            Properties.Settings.Default.Save();
        }


        //===========================//


        private void Track_Bar_Tag_Value_Scroll(object sender, EventArgs e)
        {
            Combo_Box_Tag_Value.Text = (Track_Bar_Tag_Value.Value * Scale_Factor).ToString();
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
        {   if (List_View_Selection.Visible) { List_View_Selection.Visible = false; }
            else 
            {   Load_Xml_Content(Properties.Settings.Default.Last_File); // Auto toggles to visible 
             
                if (Text_Box_Description.Visible)
                {   Text_Box_Description.Text = "";
                    Text_Box_Description.Visible = false;
                }
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
            if (Combo_Box_Tag_Name.Text == "") { return; }
            
            int Line_Count = 0;
            bool Warn_User = true;
            string The_Settings = Properties.Settings.Default.Tags;
            List<string> Related_Xmls = new List<string>();


            // if (The_Settings.Contains("Show_Files_That_Would_Change = true") | The_Settings.Contains("Show_Files_That_Would_Change=true"))
            if (Match_Without_Emptyspace(The_Settings, "Show_Files_That_Would_Change = true") & !In_Selected_Xml(Combo_Box_Entity_Name.Text))
            {
                Warn_User = false;                  
                Related_Xmls = Slice(false); // Means don't apply any changes
                Line_Count = (Related_Xmls.Count() * 30) + 160;
                if (Line_Count > 800) { Line_Count = 800; }

                iDialogue(580, Line_Count, "Yes", "Cancel", "Ignore", "Are you sure you wish to apply changes to:", Related_Xmls);
                 
  
                if (Caution_Window.Passed_Value_A.Text_Data == "false") { return; }          
            }


            Related_Xmls = Slice(true); // This line does the actual Job!

            if (Related_Xmls.Count() > 0 & Warn_User & !In_Selected_Xml(Combo_Box_Entity_Name.Text) 
              & Match_Without_Emptyspace(The_Settings, "Show_Changed_Files = true"))
            {
                Line_Count = (Related_Xmls.Count() * 30) + 90;
                if (Line_Count > 800) { Line_Count = 800; }
                iConsole(560, Line_Count, "\nApplied Changes to the following files: \n\n" + Temporal_B);
            }         
        }

        private void Button_Run_Game_MouseHover(object sender, EventArgs e)
        { Set_Resource_Button(Button_Run_Game, Properties.Resources.Button_Run_Lit); }

        private void Button_Run_Game_MouseLeave(object sender, EventArgs e)
        { Set_Resource_Button(Button_Run_Game, Properties.Resources.Button_Run); }




        private bool Match_Without_Emptyspace(string Entry_1, string Entry_2)
        {
            if (Regex.IsMatch(Entry_1.Replace(" ", ""), "(?i)" + Entry_2.Replace(" ", ""))) { return true; }
            return false;
        }


        // public List <string> Load_Xml_Content(string Xml_Path)
        public void Load_Xml_Content(string Xml_Path)
        {       
            // List<string> Found_Entities = null;
            IEnumerable<XElement> Instances = null;          
            if (!File.Exists(Xml_Path)) { MessageBox.Show("Can't find the Xml."); return; }
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
            //MessageBox.Show(Properties.Settings.Default.Xml_Directory);
            //MessageBox.Show(Properties.Settings.Default.Mod_Directory);
            int Changed_Entities = 0;

            string Selected_Xml = "";
            string Xml_Directory = Properties.Settings.Default.Xml_Directory;
            string Entity_Name = Wash_String(Combo_Box_Entity_Name.Text);
            string Selected_Tag = Regex.Replace(Combo_Box_Tag_Name.Text, "[\n\r\t </>]", ""); // Also removing </> tag values
            string Selected_Type = Wash_String(Combo_Box_Type_Filter.Text);

            // XElement Selected_Instance = null;
            IEnumerable<XElement> Instances = null;
            List <string> Changed_Xmls = new List<string>();



            
            if (!Directory.Exists(Xml_Directory))
            { MessageBox.Show("Can't find the Xml Directory."); return null; }



            if (Apply_Changes) // File_Collection is a global variable, feeded from the remaining filenames in the Caution_Window 
            {   if (File_Collection == null | File_Collection.Count == 0) { File_Collection = Get_Xmls(); } // Failsafe
                // iConsole(560, 600, Xml_Directory + string.Join("\n", File_Collection)); // return null; // Debug Code
            } else { File_Collection = Get_Xmls(); }



            foreach (var Xml in File_Collection)
            {   try
                {
                    Selected_Xml = Xml;      
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
                    { 
                        if (Instance.Descendants().Any())
                        {   try
                            {   // Selected_Instance = Instance;

                                if (Instance.Descendants(Selected_Tag).Any()) // Set the new tag value(s)
                                {
                                    Temporal_A = Selected_Xml.Replace(Xml_Directory, ""); // Removing Path
                                    if (!Changed_Xmls.Contains(Temporal_A))
                                    { Changed_Xmls.Add(Temporal_A); }


                                    if (Apply_Changes)
                                    {   Changed_Entities++;
                                        foreach (XElement Target in Instance.Descendants(Selected_Tag))
                                        {   Target.Value = Combo_Box_Tag_Value.Text;
                                            if (!Check_Box_All_Occurances.Checked) { break; } // Stop after first occurance
                                        }                                  
                                    }
                                }
                            } catch {}
                        }
                    }

                    if (Apply_Changes) { Xml_File.Save(Selected_Xml); } // MessageBox.Show("Saving to " + Xml); }
                    if (In_Selected_Xml(Entity_Name)) { return Changed_Xmls; } // Exiting after the first (and only) Xml File.
      
                } catch {}
            }

            File_Collection = new List<string>(); // Clearing for the next time
            return Changed_Xmls;      
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
            {   Text_Box_Tags.Visible = false;
                Label_Type_Filter.Visible = true;
                Label_Entity_Name.Text = "Entity Name";

                if (Tag_List != Text_Box_Tags.Text) // Then needs to update
                { Properties.Settings.Default.Tags = Text_Box_Tags.Text; }
                Properties.Settings.Default.Save();
                Reset_Tag_Box();

                return;
            }
            else
            {   Text_Box_Tags.Visible = true;
                Label_Type_Filter.Visible = false;
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
            Reset_Tag_List();
            Reset_Tag_Box();
            Text_Box_Tags.Text = Tag_List;
            if (Text_Box_Tags.Visible == false)
            { Text_Box_Tags.Visible = true; }
            MessageBox.Show("Resetted the List of Tags.");        
        }

        private void Button_Reset_Blacklist_MouseHover(object sender, EventArgs e)
        { Set_Resource_Button(Button_Reset_Blacklist, Properties.Resources.Button_Refresh_Lit); }

        private void Button_Reset_Blacklist_MouseLeave(object sender, EventArgs e)
        { Set_Resource_Button(Button_Reset_Blacklist, Properties.Resources.Button_Refresh); }

      

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
            if (Properties.Settings.Default.Star_Wars_Theme == false)
            {  return Properties.Resources.Shadow_Clone_01; }
            else {  return Properties.Resources.Starting_01; }
              
        }

        //===========================//


        public Bitmap Get_Compacted_Image()
        {
           Bitmap Result = null;
           Random rnd = new Random();
           int Value = rnd.Next(1, 2);  // creates a number between 1 and 4

            if (Properties.Settings.Default.Star_Wars_Theme == false)
            {                                          
                // Otherwise return a Star Wars based image:
                switch (Value)
                {
                    case 1:
                        Result = Properties.Resources.Rasengan_01;
                        break;
                    case 2:
                        Result = Properties.Resources.Shadow_Clone_02;
                        break;                

                    default:
                        Result = Properties.Resources.Rasengan_01;
                        break;
                }

                return Result;
            }



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

            return Result;

        }

      
        //===========================//
        public void Reset_Tag_List()
        {
            Tag_List = @"# I am a Setting (as parameter 1) or Comment (as parameter 2)
# Show_Tooltip = true
# Show_Files_That_Would_Change = true
# Show_Changed_Files = true


Planet_Surface_Accessible # Set to No and it will turn all GCs to space only because it sets all Planets to unaccessible.

Is_Targetable # Defines whether or not all Hardpoints in the Mod can be targeted.

Is_Destroyable # Defines whether or not all Hardpoints in the Mod can be destroyed.

Is_Named_Hero # Set to No and no more heroes will respawn.

Projectile_Does_Shield_Damage # Set to Yes and apply to the whole mod, to disable all shield piercing effects.

Projectile_Does_Energy_Damage # Careful, if set to yes Hitpoint damage will start once enemy energy level reaches 0.

Projectile_Does_Hitpoint_Damage

# ==================== Unit Values ====================

Tactical_Health

Shield_Points

Shield_Refresh_Rate # Usually about 30 for capital ships and less for weaker classes.

Select_Box_Scale # Set to 0 and all Ships and Troops will have their select box deactiated.

Layer_Z_Adjust

Space_Tactical_Unit_Cap # Sets Unit cap in space tactical battles, for all Factions in the Mod. Don't put too high or it will cause laggs.

Build_Cost_Credits # Set the price to 1, then you can build as many units as the population cap allows.

Tactical_Build_Cost_Multiplayer # Set the price to 1 for all Skirmish units.
";
        }

     
      
        //===========================//
        public void Reset_Tag_Box()
        {
            string[] Current_Tags = Text_Box_Tags.Text.Split('\n');
           
            Combo_Box_Tag_Name.Items.Clear();
            Combo_Box_Tag_Name.Text = "";
            Text_Box_Description.Text = "";
            Text_Box_Description.Visible = false;

            foreach (string Phrase in Current_Tags)
            {
                if (Phrase.StartsWith("//") || Phrase.StartsWith("#") || Phrase == "") { continue; } // Skip commented Lines
                Tag_Info = Phrase.Split('#');

                Combo_Box_Tag_Name.Items.Add(Tag_Info[0].Replace(" ", ""));                                                              
            }
        }



        //===========================//
        private void Combo_Box_Entity_Name_TextChanged(object sender, EventArgs e)
        { Text_Box_Description.Visible = false; }



 
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
                case "Faction":
                    Scale_Factor = 1;
                    break;
                    
                default:
                    Scale_Factor = 10;
                    break;
            }

            Text_Box_Description.Visible = false;


            if (Combo_Box_Type_Filter.Text == "All in loaded Xml")
            {   Combo_Box_Entity_Name.Text = "Multi";
                Combo_Box_Type_Filter.Text = ""; // Because then this can trigger a 2nd time in a row

                if (!List_View_Selection.Visible)
                {
                    Button_Start_Click(null, null); // This shows the List_View with Xml Entities of the loaded Xml
                    Set_Resource_Button(Button_Start, Properties.Resources.Button_Logs_Lit);
                }

                for (int i = List_View_Selection.Items.Count - 1; i >= 0; --i)
                {   // Selecting everything
                    if (List_View_Selection.Items[i].Text != "")
                    { List_View_Selection.Items[i].Selected = true; }
                }
                List_View_Selection.Focus();
            }
            //else if (Combo_Box_Entity_Name.Text ==  "Multi")
            //{ Combo_Box_Entity_Name.Text = ""; }
     

        }


        //===========================//
        private void Combo_Box_Tag_Name_TextChanged(object sender, EventArgs e)
        {   bool Expects_Bool = false;
            bool Reset_Type_Filter = false;


            switch (Combo_Box_Tag_Name.Text)
            {
                case "Planet_Surface_Accessible":
                    Combo_Box_Type_Filter.Text = "Planet";
                    Expects_Bool = true;
                    break;
                case "Is_Targetable":
                    Expects_Bool = true;
                    Combo_Box_Type_Filter.Text = "HardPoint";
                    break;
                case "Is_Destroyable":
                    Expects_Bool = true;
                    Combo_Box_Type_Filter.Text = "HardPoint";
                    break;
                case "Is_Named_Hero":
                    Expects_Bool = true;
                    Reset_Type_Filter = true;
                    break;


                case "Projectile_Does_Shield_Damage":
                    Expects_Bool = true;
                    Combo_Box_Type_Filter.Text = "Projectile";
                    break;
                case "Projectile_Does_Energy_Damage":
                    Expects_Bool = true;
                    Combo_Box_Type_Filter.Text = "Projectile";
                    break;
                case "Projectile_Does_Hitpoint_Damage":
                    Expects_Bool = true;
                    Combo_Box_Type_Filter.Text = "Projectile";
                    break;


                case "Tactical_Health":
                    Scale_Factor = 100;
                    Reset_Type_Filter = true;
                    break;
                case "Shield_Points":
                    Scale_Factor = 100;
                    Reset_Type_Filter = true;
                    break;
                case "Shield_Refresh_Rate":
                    Scale_Factor = 5;
                    Reset_Type_Filter = true;
                    break;
                case "Select_Box_Scale":
                    Scale_Factor = 100;
                    Reset_Type_Filter = true;
                    break;

                case "Space_Tactical_Unit_Cap":
                    Combo_Box_Type_Filter.Text = "Faction";
                    break;                 
                case "Build_Cost_Credits":
                    Scale_Factor = 100;
                    Reset_Type_Filter = true;
                    break;
                case "Tactical_Build_Cost_Multiplayer":
                    Scale_Factor = 100;
                    Reset_Type_Filter = true;
                    break;

                default:                  
                    Expects_Bool = false;
                    break;
            }
        

            // Resetting the right scale factor
            if (Scale_Factor == 10) { Combo_Box_Type_Filter_TextChanged(null, null); }



            if (Expects_Bool) { Track_Bar_Tag_Value.Visible = false; }
            else { Track_Bar_Tag_Value.Visible = true; }


            if (Reset_Type_Filter) 
            {
                string Probably_Wrong = "Planet,HardPoint,Faction,Projectile";

                foreach (string Entry in Probably_Wrong.Split(','))
                {   if (Entry == Combo_Box_Type_Filter.Text)
                    { Combo_Box_Type_Filter.Text = ""; break; }
                }
            }



            // ===============================================
            string Info = "";

            // The + " #" makes sure the last entry comment stays empty
            foreach (string Phrase in (Tag_List + " #").Split('\n'))
            {
                if (Phrase.StartsWith("//") || Phrase.StartsWith("#") || Phrase == "") { continue; } // Skip commented Lines
                else if (Phrase.Contains("#"))
                {   
                    try
                    {   Tag_Info = Phrase.Split('#');
                        Info = Tag_Info[1];


                        if (Tag_Info[0].Replace(" ", "") == Combo_Box_Tag_Name.Text)
                        {
                            // Removing empty space
                            if (Info[0] == ' ') { Info = Info.Substring(1, Info.Length - 1); } break;
                        }
                    } catch { Info = ""; }
                }
            }

                 
            if (Info == "")
            {   Text_Box_Description.Text = "";
                Text_Box_Description.Visible = false;              
            }
            else if (Match_Without_Emptyspace(Properties.Settings.Default.Tags, "Show_Tooltip = true"))           
            {   Text_Box_Description.Text = Info;
                Text_Box_Description.Visible = true;
            }  
        }
   

        private void Combo_Box_Tag_Value_TextChanged(object sender, EventArgs e)
        {
            Text_Box_Description.Visible = false;
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

        public void iDialogue(int Window_Size_X, int Window_Size_Y, string Button_A_Text, string Button_B_Text, string Button_C_Text, string Text, List<string> The_List = null)
        {
            //========== Displaying Error Messages to User   
            // Innitiating new Form
            Caution_Window Display = new Caution_Window();
            Display.Size = new Size(Window_Size_X, Window_Size_Y);

            // Using Theme colors for Text and Background
            Display.Text_Box_Caution_Window.BackColor = Color.Gray;
            Display.Text_Box_Caution_Window.ForeColor = Color.White;


            if (Button_A_Text != "false" & Button_C_Text == "false")
            {
                Display.Button_Caution_Box_1.Visible = true;
                Display.Button_Caution_Box_1.Text = Button_A_Text;
                Display.Button_Caution_Box_1.Location = new Point(120, Display.Size.Height - 96);
            }


            if (Button_B_Text != "false" & Button_C_Text == "false")
            {
                Display.Button_Caution_Box_2.Visible = true;
                Display.Button_Caution_Box_2.Text = Button_B_Text;
                Display.Button_Caution_Box_2.Location = new Point(280, Display.Size.Height - 96);
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
            string Error = "Please dragg and drop any target Xml, \nor the Xml directory of your mod into the Dropzone.";

            if (Xml_Directory == "" | Xml_Directory == null)
            { MessageBox.Show(Error); return null; }


            try
            {   if (Directory.Exists(Xml_Directory))
                {   foreach (string The_File in Directory.GetFiles(Xml_Directory, "*.xml", System.IO.SearchOption.AllDirectories))
                    { All_Xmls.Add(The_File); }                   
                } else { MessageBox.Show(Error); return null; }
                
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
                {
                    Item.ForeColor = Color.White;
                    Item.BackColor = Color.CadetBlue; ;
                }
                else
                {
                    Item.ForeColor = Color.Black;
                    Item.BackColor = Color.LightGray;
                }
            }
        }

    
        //===========================//



    }
}
