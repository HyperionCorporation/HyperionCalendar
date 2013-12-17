using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace Calendar
{
    public partial class GenericHelpMenu : Form
    {
        private GenericHelpMenuType type;

        private Assembly assembly = Assembly.GetExecutingAssembly();
        private string resourceName = "Calendar.Text.about.rtf";

        public enum GenericHelpMenuType
        {
            ABOUT,
            HELP
        }


        public GenericHelpMenu(string title, GenericHelpMenuType type)
        {
            InitializeComponent();
            this.Name = title;
            lblTitle.Text = title;
            this.type = type;
            SetText();
        }


        private void SetText()
        {
            if (type == GenericHelpMenuType.ABOUT)
            {
                string aboutText;
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    aboutText = reader.ReadToEnd();
                }
               
                rtxContent.Rtf = aboutText;

            }

            else if (type == GenericHelpMenuType.HELP)
            {
                string helpText = "Help Information Goes Here";
                rtxContent.Text = helpText;
            }

            else
            {
                rtxContent.Text = "Well this is embaressing\nI have no idea " +
                                  "why this is showing. Please submit a bug report on the " +
                                  "Github repo";
            }

        }
    }
}
