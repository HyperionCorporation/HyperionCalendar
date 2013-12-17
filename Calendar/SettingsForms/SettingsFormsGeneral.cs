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
        private bool complete = false;

        public SettingsFormsGeneral(Persistence persistence, string username)
        {
            InitializeComponent();
            AddColors();
            lblServer.Text = persistence.GetMySQLServer();
            lblUser.Text = username;
            DateTime lastSync = persistence.GetLastSync();
            lblLastSync.Text = lastSync.ToShortDateString() + " " + lastSync.ToShortTimeString();
        }

        private void AddColors()
        {
            //Background Color
            txtBGR.Text = Convert.ToString(Settings.CellBackground.R);
            txtBGG.Text = Convert.ToString(Settings.CellBackground.G);
            txtBGB.Text = Convert.ToString(Settings.CellBackground.B);

            //Cell Highlight
            txtCHR.Text = Convert.ToString(Settings.CellHighlight.R);
            txtCHG.Text = Convert.ToString(Settings.CellHighlight.G);
            txtCHB.Text = Convert.ToString(Settings.CellHighlight.B);

            //Other Month Color
            txtOMR.Text = Convert.ToString(Settings.OtherMonthColor.R);
            txtOMG.Text = Convert.ToString(Settings.OtherMonthColor.G);
            txtOMB.Text = Convert.ToString(Settings.OtherMonthColor.B);

            //Current Day Color
            txtCurrentDayR.Text = Convert.ToString(Settings.CurrentDayColor.R);
            txtCurrentDayG.Text = Convert.ToString(Settings.CurrentDayColor.G);
            txtCurrentDayB.Text = Convert.ToString(Settings.CurrentDayColor.B);

            pnlCellBGPreview.BackColor = Settings.CellBackground;
            pnlCellHighlightPreview.BackColor = Settings.CellHighlight;
            pnlCellOtherPreview.BackColor = Settings.OtherMonthColor;
            pnlCellDayPreview.BackColor = Settings.CurrentDayColor;

            complete = true;

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Settings.CellBackground = pnlCellBGPreview.BackColor;
            Settings.CellHighlight = pnlCellHighlightPreview.BackColor;
            Settings.OtherMonthColor = pnlCellOtherPreview.BackColor;
            Settings.CurrentDayColor = pnlCellDayPreview.BackColor;
            this.Tag = null;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Update the panel to preview the color. 
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void txt_TextChanged(object sender, EventArgs e)
        {
            TextBox box = sender as TextBox;
            if (ValidateInput(box) && complete)
            {

                pnlCellBGPreview.BackColor = Color.FromArgb(Convert.ToInt32(txtBGR.Text),
                                                            Convert.ToInt32(txtBGG.Text),
                                                            Convert.ToInt32(txtBGB.Text));
                pnlCellHighlightPreview.BackColor = Color.FromArgb(Convert.ToInt32(txtCHR.Text),
                                                                   Convert.ToInt32(txtCHG.Text),
                                                                   Convert.ToInt32(txtCHB.Text));
                pnlCellOtherPreview.BackColor = Color.FromArgb(Convert.ToInt32(txtOMR.Text),
                                                               Convert.ToInt32(txtOMG.Text),
                                                               Convert.ToInt32(txtOMB.Text));
                pnlCellDayPreview.BackColor = Color.FromArgb(Convert.ToInt32(txtCurrentDayR.Text),
                                                             Convert.ToInt32(txtCurrentDayG.Text),
                                                             Convert.ToInt32(txtCurrentDayB.Text));
            }
        }

        private bool ValidateInput(TextBox box)
        {
            int num;
            if (box.Text == String.Empty ||
               !Int32.TryParse(box.Text, out num) ||
                num > 255 || num < 0)
            {
                return false;
            }

            return true;
        }
    }
}
            