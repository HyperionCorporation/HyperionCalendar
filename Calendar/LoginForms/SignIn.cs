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
        public static Persistence persistence;
        public static SignIn signIn;
        public static User user;
        public static BackgroundWorker bw;
        public static MainForm mainForm;

        public SignIn(Persistence persistence,MainForm mainForm)
        {
            InitializeComponent();
            SignIn.persistence = persistence;
            SignIn.signIn = this;
            this.CenterToScreen();
            SignIn.mainForm = mainForm;
            bw = new BackgroundWorker
            {
                WorkerReportsProgress = true,
            };
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.txtPassword.Clear();
            this.txtUserLogin.Clear();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

            if (txtUserLogin.Text == "")
            {
                MessageBox.Show("Please enter a Username", "Login Error");
            }
            else if (txtPassword.Text == "")
            {
                MessageBox.Show("Please enter a Password", "Login Error");
            }
            else
            {
                //Threaded to prevent UI lockup
                bw.DoWork += Login;
                bw.ProgressChanged += bw_ProgressChanged;
                bw.RunWorkerAsync();                
            }
        }

        /// <summary>
        /// Passed to the Thread Start as a paramter.
        /// </summary>
        /// <param name="info">The information.</param>
        public static void Login(object sender, DoWorkEventArgs e)
        {
            string email = SignIn.signIn.txtUserLogin.Text;
            SignIn.user = SignIn.persistence.GetUser(email);


            StorageLocation loc = persistence.UserExists(SignIn.signIn.txtUserLogin.Text.ToLower());
            bw.ReportProgress(50);
            if (loc != StorageLocation.NULL)
            {
                if (SignIn.signIn.validateLogin(loc))
                {
                    bw.ReportProgress(100);
                    SignIn.signIn.DialogResult = DialogResult.OK;
                    SignIn.signIn.Tag = user;
                }
            
            }

            else
            {
                MessageBox.Show("User doesn't exist", "Login Error");
            }
        }

        /// <summary>
        /// Handles the ProgressChanged event of the bw control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ProgressChangedEventArgs"/> instance containing the event data.</param>
        static void bw_ProgressChanged(object sender,ProgressChangedEventArgs e)
        {
            SignIn.signIn.prgsLogin.Value = e.ProgressPercentage;
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

                HashAlgorithm algo = HashAlgorithm.Create("SHA512");
                String enteredHashedPassword = User.hashPassword(signIn.txtPassword.Text, SignIn.user.Salt)["hashedpassword"];
                if (SignIn.user.HashedPassword == enteredHashedPassword)
                {
                    MessageBox.Show("Login Succesful", "Login");
                    //Cache the user
                    SignIn.persistence.FlushCache();
                    SignIn.persistence.CacheUser(SignIn.user);
                    //Tell the app that it's ok to let them in. They're cool.
                    signIn.DialogResult = DialogResult.OK;
                    signIn.Tag = SignIn.user;
                }

                else
                {
                    MessageBox.Show("Incorrect Username/Password", "Login Error");
                }
                return false; //We need to wait for the thread to call a method

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
