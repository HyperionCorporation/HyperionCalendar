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
            AddColorsToDropDown();
            lblServer.Text = persistence.GetMySQLServer();
            lblUser.Text = username;
            DateTime lastSync = persistence.GetLastSync();
            lblLastSync.Text = lastSync.ToShortDateString() + " " + lastSync.ToShortTimeString();
        }

        private void AddColorsToDropDown()
        {
            for (int r = 0; r <= 255; r++)
            {
                cmboCellBGR.Items.Add(r);
                cmboCellBGG.Items.Add(r);
                cmboCellBGB.Items.Add(r);

                cmboCellHighlightR.Items.Add(r);
                cmboCellHighlightG.Items.Add(r);
                cmboCellHighlightB.Items.Add(r);

                cmboCellOtherR.Items.Add(r);
                cmboCellOtherG.Items.Add(r);
                cmboCellOtherB.Items.Add(r);
            }

            cmboCellBGR.SelectedIndex = 0;
            cmboCellBGG.SelectedIndex = 0;
            cmboCellBGB.SelectedIndex = 0;


            cmboCellHighlightR.SelectedIndex = 0;
            cmboCellHighlightG.SelectedIndex = 0;
            cmboCellHighlightB.SelectedIndex = 0;

            cmboCellOtherR.SelectedIndex = 0;
            cmboCellOtherG.SelectedIndex = 0;
            cmboCellOtherB.SelectedIndex = 0;
        }
    }
}
