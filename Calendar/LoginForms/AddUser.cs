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
        
        public AddUser(Persistence persistence)
        {
            InitializeComponent();
            this.persistence = persistence;
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
                this.Tag = new User(txtName.Text, txtEmail.Text, HashedCredentials["hashedpassword"], HashedCredentials["salt"]);
                this.DialogResult = DialogResult.OK;
            }
        }

        //When you leave the txtEmail Box
        private void txtEmail_Leave(object sender, EventArgs e)
        {
            if (persistence.UserExists(txtEmail.Text.ToString()) != StorageLocation.NULL)
            {
                lblEmailWarn.Text = "Name Already Exists";
                lblEmailWarn.Visible = true;
                btnSubmit.Enabled = false;
            }

            else if (!isValidEmail(txtEmail.Text.ToString()))
            {
                lblEmailWarn.Text = "Invalid Email";
                lblEmailWarn.Visible = true;
                btnSubmit.Enabled = false;
            }

            else
            {
                lblEmailWarn.Visible = false;
                btnSubmit.Enabled = true;
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

            int atSignLocation = email.IndexOf('@');


            foreach (string domain in acceptableDomains)
            {
                string extension = email.Substring(email.Length - domain.Length);
                if (extension == domain && email[atSignLocation +1] != '.')
                    return true;
            }

            return false;
            
        }

        private void txtPasswordVerify_TextChanged(object sender, EventArgs e)
        {
            //Check for matching passwords
            if (txtPassword.Text != txtPasswordVerify.Text)
            {
                btnSubmit.Enabled = false;
                lblPasswdMessage.Visible = true;
            }

            else
            {
                btnSubmit.Enabled = true;
                lblPasswdMessage.Visible = false;
            }
        }

  

    }
}
