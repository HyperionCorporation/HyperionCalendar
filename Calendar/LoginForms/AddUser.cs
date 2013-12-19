using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StorageLocation = Calendar.Persistence.Location; //no typdef exists. 


namespace Calendar
{
   
    public partial class AddUser : Form
    {
        private Persistence persistence;
        public static BackgroundWorker bw;
        private bool isCheckingEmail;
        private bool userExists;
        
        public AddUser(Persistence persistence)
        {
            InitializeComponent();
            bw = new BackgroundWorker();
            this.persistence = persistence;
            this.btnSubmit.Enabled = false;
            isCheckingEmail = false;
            userExists = true; //We will assume the user exists until proven otherwise.
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (txtName.Text == String.Empty)
                MessageBox.Show("Please enter a Name", "Entry Error");
            else if (txtPassword.Text == String.Empty)
                MessageBox.Show("Please enter a Password", "Entry Error");
            else if (txtPasswordVerify.Text == String.Empty)
                MessageBox.Show("Please enter your password twice", "Entry Error");

            else if (txtName.Text != String.Empty && txtPassword.Text != String.Empty)
            {
                Dictionary<String, String> HashedCredentials = User.hashPassword(txtPassword.Text);
                this.Tag = new User(txtName.Text, txtEmail.Text, HashedCredentials["hashedpassword"],true, HashedCredentials["salt"]);
                this.DialogResult = DialogResult.OK;
            }
        }


        /// <summary>
        /// Handles the RunWorkerCompleted event of the bw control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Tuple<string, bool, bool> resultSet = (Tuple<string, bool, bool>)e.Result;
            lblEmailWarn.Text = resultSet.Item1;
            lblEmailWarn.Visible = resultSet.Item2;
            btnSubmit.Enabled = resultSet.Item3;
            isCheckingEmail = false;
        }

        /// <summary>
        /// Handles the ProgressChanged event of the bw control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ProgressChangedEventArgs"/> instance containing the event data.</param>
        static void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        private void CheckExists(object sender, DoWorkEventArgs e)
        {

            if (persistence.UserExists(txtEmail.Text.ToString()) != StorageLocation.NULL)
            {
                e.Result = Tuple.Create<string, bool, bool>("Name Already Exists", true, false);
                userExists = true;
                //lblEmailWarn.Text = "Name Already Exists";
                //lblEmailWarn.Visible = true;
                //btnSubmit.Enabled = false;
            }

            else if (!isValidEmail(txtEmail.Text.ToString()))
            {
                e.Result = Tuple.Create<string, bool, bool>("Invalid Email", true, false);
                //lblEmailWarn.Text = "Invalid Email";
                //lblEmailWarn.Visible = true;
                //btnSubmit.Enabled = false;
            }

            else
            {
                e.Result = Tuple.Create<string, bool, bool>("", false, true);
                //lblEmailWarn.Visible = false;
                //btnSubmit.Enabled = true;
            }
        }

        //When you leave the txtEmail Box
        private void txtEmail_Leave(object sender, EventArgs e)
        {
            if (!isCheckingEmail)
            {
                isCheckingEmail = true;
                bw.DoWork += CheckExists;
                bw.WorkerReportsProgress = true;
                bw.ProgressChanged += bw_ProgressChanged;
                bw.WorkerSupportsCancellation = true;
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                bw.RunWorkerAsync();
            }
        }

        //Validate the email address
        private bool isValidEmail(string email)
        {
            int atSymbolCount = 0;
            
            string[] acceptableDomains = { ".com", ".org", ".net", ".edu", ".info" };

            if (email == string.Empty || email[0] == '@')
                return false;

            foreach (char c in email)
            {
                if (c == '@')
                {
                    atSymbolCount++;
                }
            }

            if (atSymbolCount != 1)
                return false;

            if(email.Contains(' '))
                return false;

            int atSignLocation = email.IndexOf('@');


            foreach (string domain in acceptableDomains)
            {
                string extension = email.Substring(email.Length - domain.Length);
                if (extension == domain && email[atSignLocation +1] != '.')
                    return true;
            }

            return false;
            
        }

        /// <summary>
        /// Handles the TextChanged event of the txtPasswordVerify control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void txtPasswordVerify_TextChanged(object sender, EventArgs e)
        {
            //Check for matching passwords
            if (txtPassword.Text != txtPasswordVerify.Text)
            {
                btnSubmit.Enabled = false;
                lblPasswdMessage.Visible = true;
            }

            else if(!userExists)
            {
                btnSubmit.Enabled = true;
                lblPasswdMessage.Visible = false;
            }

            else
            {
                lblPasswdMessage.Visible = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

  

    }
}
