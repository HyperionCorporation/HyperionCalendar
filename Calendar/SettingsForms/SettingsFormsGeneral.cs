using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calendar
{
    public partial class SettingsFormsGeneral : Form
    {
        public SettingsFormsGeneral(Persistence persistence, string username)
        {
            InitializeComponent();
            lblServer.Text = persistence.GetMySQLServer();
            lblUser.Text = username;
            DateTime lastSync = persistence.GetLastSync();
            lblLastSync.Text = lastSync.ToShortDateString() + " " + lastSync.ToShortTimeString();
        }
    }
}
