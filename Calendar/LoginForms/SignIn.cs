using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Threading;
using StorageLocation = Calendar.Persistence.Location; //no typdef exists. 


namespace Calendar
{
    public partial class SignIn : Form
    {
        private Persistence persistence;
        private User user;

        public SignIn(Persistence persistence)
        {
            InitializeComponent();
            this.persistence = persistence;
            this.CenterToScreen();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.txtPassword.Clear();
            this.txtUserLogin.Clear();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            

            StorageLocation loc = persistence.UserExists(txtUserLogin.Text.ToLower());

            if (loc != StorageLocation.NULL)
            {
                if (validateLogin(loc))
                {
                    this.DialogResult = DialogResult.OK;
                    this.Tag = user;
                }
                else
                {
                    MessageBox.Show("Wrong Password", "Login Error");
                }
            }

            else
            {
                MessageBox.Show("User doesn't exist", "Login Error");
            }
        }

        //Attempts to validate the user's login
        private bool validateLogin(StorageLocation loc)
        {
            //If user is cached, check SQLite. 
            if (loc == StorageLocation.SQLITE)
            {
                user = persistence.GetCachedUser(txtUserLogin.Text);
                HashAlgorithm algo = HashAlgorithm.Create("SHA512");
                String enteredHashedPassword = User.hashPassword(txtPassword.Text, user.Salt)["hashedpassword"];

                if (user.HashedPassword == enteredHashedPassword)
                {
                    MessageBox.Show("Login Succesful", "Login");
                    return true;
                }
            }

            else
            {
                user = persistence.GetUser(txtUserLogin.Text);
                HashAlgorithm algo = HashAlgorithm.Create("SHA512");
                String enteredHashedPassword = User.hashPassword(txtPassword.Text, user.Salt)["hashedpassword"];

                if (user.HashedPassword == enteredHashedPassword)
                {
                    MessageBox.Show("Login Succesful", "Login");
                    //Cache the user
                    persistence.FlushCache();
                    persistence.CacheUser(user);
                    return true;
                }
            }

            return false; //Change this if MySQL isn't working
        }

        private void btnSignup_Click(object sender, EventArgs e)
        {
            if (persistence.CheckRemoteConnection())
            {
                Form addUserForm = new AddUser(persistence);
                DialogResult addUserFormResult = addUserForm.ShowDialog();
                if (addUserFormResult == DialogResult.OK)
                {
                    //Create the user
                    user = (User)addUserForm.Tag;
                    persistence.SaveUser(user);
                    this.txtUserLogin.Text = user.Email;
                }
            }

            else
            {
                MessageBox.Show("Can't Connect to Remote","Connection Error");
                }
        }

    }
}
