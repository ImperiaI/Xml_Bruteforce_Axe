﻿using System;
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

            Caution_Window_Resize(null, null);
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

                Active_Window.Set_Checker(List_View_Info);
            }    
  
        }


        private void Caution_Window_Resize(object sender, EventArgs e)
        {
            List_View_Info.Size = new Size(this.Size.Width - 10, this.Size.Height - 93);
            List_View_Info.Columns[0].Width = this.Size.Width - 18;
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

                Active_Window.Set_Checker(List_View_Info);
                Active_Window.Blacklisted_Xmls = The_Blacklist; // Refreshing

                string Blacklist = string.Join("\n", The_Blacklist);          
                File.WriteAllText(Active_Window.Xml_Directory + "Axe_Blacklist.txt", Blacklist);
            }                  
        }

        private void Caution_Window_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Important or occasions that expect false to not continue would missunderstand the user here!
            // Also if the User just blacklisted or ignored entries before pressing X this must not trigger.
            if (Passed_Value_A.Text_Data != "true") { Passed_Value_A.Text_Data = "false"; }
        }


      
      
       
    }
}
