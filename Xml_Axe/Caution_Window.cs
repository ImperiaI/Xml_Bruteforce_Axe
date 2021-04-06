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



namespace Log_Compactor
{
    public partial class Caution_Window : Form   
    {
        public Caution_Window() { InitializeComponent(); }

        public bool List_Exclusion_Mode = false;

        
        // Declaring static variable to pass it to the Main_Window Form:
        public static class Passed_Value_A
        {
            public static string Text_Data { get; set; }        
        }

        public static class Passed_Table
        {
            public static List<string> Content { get; set; }
        }
 

        private void Caution_Window_Load(object sender, EventArgs e)
        {
            Passed_Value_A.Text_Data = "false";

            List_View_Info.Size = new Size(this.Size.Width - 10, this.Size.Height - 10);
        }
      



        public void Button_Caution_Box_1_Click(object sender, EventArgs e)
        {   
            // Setting the passed value to true (its a string because later we might need texts)
            Passed_Value_A.Text_Data = "true";
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
            {
                Window Main_Class = new Window();
                
                foreach (string Selection in Main_Class.Select_List_View_Items(List_View_Info))
                {
                    foreach (ListViewItem Item in List_View_Info.Items)
                    {
                        if (Item.Text == Selection)
                        { List_View_Info.Items.Remove(Item); }
                    }                  
                }
            }


            foreach (ListViewItem Item in List_View_Info.Items)
            {
                if (Item.Text != "" & Item != null) { Passed_Table.Content.Add(Item.Text); }
            }  

            
        }


        private void Caution_Window_Resize(object sender, EventArgs e)
        {
            List_View_Info.Size = new Size(this.Size.Width - 10, this.Size.Height - 10);
        }


      
      
       
    }
}
