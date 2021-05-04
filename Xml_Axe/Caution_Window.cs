using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Text.RegularExpressions;



namespace Xml_Axe
{
    public partial class Caution_Window : Form   
    {              
        Form The_Window = Application.OpenForms["Window"];
        Window Active_Window = null;
        public bool List_Exclusion_Mode = false;
        public bool Ying_Dominates = true;

        // Declaring static variable to pass it to the Main_Window Form:
        public static class Passed_Value_A
        { public static string Text_Data { get; set; } }


        
        public Caution_Window() 
        {        
            InitializeComponent(); 
        }


        private void Caution_Window_Load(object sender, EventArgs e)
        {
            Active_Window = (Window)The_Window;
            Passed_Value_A.Text_Data = "false";


            
            Control[] Controls = { Button_Caution_Box_1, Button_Caution_Box_2, Button_Caution_Box_3, Button_Caution_Box_4, Button_Invert_Selection };
            foreach (Control Selectrion in Controls)  // In order to have the right background colors
            {   // Selectrion.Parent = Text_Box_Caution_Window; // Nah they behave wrong then
                Selectrion.BackColor = Color.Transparent; 
            }
            

            Caution_Window_Resize(null, null);
            Active_Window.Set_Resource_Button(Button_Invert_Selection, Properties.Resources.Button_Ying);
           
        }
      
    
  


 


        public void Button_Caution_Box_1_Click(object sender, EventArgs e)
        {   
            // Setting the passed value to true (its a string because later we might need texts)
            Passed_Value_A.Text_Data = "true";

            if (List_Exclusion_Mode)
            {   foreach (ListViewItem Item in List_View_Info.Items)
                {   try 
                    {   if (Active_Window != null & Item != null)
                        {   if (Item.Text != "" & !Item.Text.Contains("Are you sure"))
                            { Active_Window.File_Collection.Add(Active_Window.Xml_Directory + Item.Text); }
                        }
                    } catch {}
                }              
            }

            this.Close();
        }


        public void Button_Caution_Box_2_Click(object sender, EventArgs e)
        {            
            Passed_Value_A.Text_Data = "false";
            this.Close();
        }

        private void Button_Caution_Box_3_Click(object sender, EventArgs e)
        {
            Passed_Value_A.Text_Data = "else";

            if (!List_Exclusion_Mode) { this.Close(); }
            else
            {   foreach (string Selection in Active_Window.Select_List_View_Items(List_View_Info))
                {
                    foreach (ListViewItem Item in List_View_Info.Items)
                    {
                        if (Item.Text == Selection)
                        { List_View_Info.Items.Remove(Item); }
                    }                  
                }

                Active_Window.Set_Checker(List_View_Info, Active_Window.Theme_Color);
            }

            Caution_Window_Resize(null, null); // Set the size at the right of the scrollbar
        }


        private void Button_Caution_Box_4_Click(object sender, EventArgs e)
        {
            List<string> The_Blacklist = Active_Window.Blacklisted_Xmls;

            Passed_Value_A.Text_Data = "other";

            if (!List_Exclusion_Mode) { this.Close(); }
            else
            {
                foreach (string Selection in Active_Window.Select_List_View_Items(List_View_Info))
                {
                    foreach (ListViewItem Item in List_View_Info.Items)
                    {   if (Item.Text == Selection)
                        {   The_Blacklist.Add(Item.Text); // Putting on my Hat ;)
                            List_View_Info.Items.Remove(Item); 
                        }
                    }
                }

                Active_Window.Set_Checker(List_View_Info, Active_Window.Theme_Color);
                Active_Window.Blacklisted_Xmls = The_Blacklist; // Refreshing

                string Blacklist = string.Join("\n", The_Blacklist);          
                File.WriteAllText(Active_Window.Xml_Directory + "Axe_Blacklist.txt", Blacklist);
            }

            Caution_Window_Resize(null, null); // Set the size at the right of the scrollbar
        }


        private void Button_Invert_Selection_Click(object sender, EventArgs e)
        {
            List<string> Selection = Active_Window.Select_List_View_Items(List_View_Info);

            //bool No_Selection = false;
            //if (Selection.Count() == 0) { No_Selection = true; } 

            foreach (ListViewItem Item in List_View_Info.Items)
            {
                if (Item.Text == "" || Item.Text.Contains("Are you sure")) { Item.Selected = false; continue; }
                // else if (No_Selection) { Item.Selected = true; } 

                else if (Active_Window.List_Matches(Selection, Item.Text)) { Item.Selected = false; }
                else { Item.Selected = true; }
            }

            if (Ying_Dominates) { Ying_Dominates = false; }
            else { Ying_Dominates = true; } // toggling
            Button_Invert_Selection_MouseHover(null, null);

            Caution_Window_Resize(null, null);
        }

        private void Button_Invert_Selection_MouseHover(object sender, EventArgs e)
        {
            if (Ying_Dominates) { Active_Window.Set_Resource_Button(Button_Invert_Selection, Properties.Resources.Button_Ying_Lit); }
            else if (!Ying_Dominates) { Active_Window.Set_Resource_Button(Button_Invert_Selection, Properties.Resources.Button_Yang_Lit); }
        }

        private void Button_Invert_Selection_MouseLeave(object sender, EventArgs e)
        {
            if (Ying_Dominates) { Active_Window.Set_Resource_Button(Button_Invert_Selection, Properties.Resources.Button_Ying); }
            else if (!Ying_Dominates) { Active_Window.Set_Resource_Button(Button_Invert_Selection, Properties.Resources.Button_Yang); }
        }



        private void Caution_Window_Resize(object sender, EventArgs e)
        {
            List_View_Info.Size = new Size(this.Size.Width - 10, this.Size.Height - 93);

            int Border = 14;
            int Item_Count = 0;

            foreach (ListViewItem Item in List_View_Info.Items)
            {
                if (Item.Text == "" || Item.Text.Contains("Are you sure")) { continue; }
                Item_Count++;

                if (Item_Count > 22) { Border = 32; break; }
            }

            List_View_Info.Columns[0].Width = this.Size.Width - Border;
        }


        private void Caution_Window_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Important or occasions that expect false to not continue would missunderstand the user here!
            // Also if the User just blacklisted or ignored entries before pressing X this must not trigger.
            if (Passed_Value_A.Text_Data != "true") { Passed_Value_A.Text_Data = "false"; }
        }


      
      
       
    }
}
