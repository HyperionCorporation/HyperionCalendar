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
        public static Sync sync;

        public SignIn(Persistence persistence,MainForm mainForm,Sync sync)
        {
            InitializeComponent();
            SignIn.persistence = persistence;
            SignIn.signIn = this;
            this.CenterToScreen();
            SignIn.mainForm = mainForm;
            bw = new BackgroundWorker();
            SignIn.sync = sync;
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
                if (bw != null)
                {
                    bw = new BackgroundWorker();
                }

                if (!bw.IsBusy)
                {
                    prgsLogin.Value = 0;
                    bw.DoWork += Login;
                    bw.WorkerReportsProgress = true;
                    bw.ProgressChanged += bw_ProgressChanged;
                    bw.WorkerSupportsCancellation = true;
                    bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                    bw.RunWorkerAsync();
                    btnLogin.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Passed to the Thread Start as a paramter.
        /// </summary>
        /// <param name="info">The information.</param>
        public static void Login(object sender, DoWorkEventArgs e)
        {
            bw.ReportProgress(25);
            string email = SignIn.signIn.txtUserLogin.Text;
            StorageLocation loc = persistence.UserExists(SignIn.signIn.txtUserLogin.Text.ToLower());
            if (loc != StorageLocation.ERRCONN)
            {
                bw.ReportProgress(50);
                if (loc != StorageLocation.SQLITE)
                {
                    Tuple<bool, User> result = SignIn.persistence.GetUser(email);
                    user = result.Item2;
                }

                bw.ReportProgress(75);
                if (loc != StorageLocation.NULL)
                {
                    if (SignIn.signIn.validateLogin(loc))
                    {
                        sync.User = SignIn.user;
                        sync.DoSync();
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
        }

        /// <summary>
        /// Handles the RunWorkerCompleted event of the bw control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnLogin.Enabled = true;
            prgsLogin.Value = 0;
        }

        /// <summary>
        /// Handles the ProgressChanged event of the bw control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ProgressChangedEventArgs" /> instance containing the event data.</param>
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
                else
                {
                    MessageBox.Show("Incorrect Username/Password", "Login Error");
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
                    sync.User = SignIn.user;
                    sync.DoSync();
                    //Tell the app that it's ok to let them in. They're cool.
                    signIn.DialogResult = DialogResult.OK;
                    signIn.Tag = SignIn.user;
                }

                else
                {
                    MessageBox.Show("Incorrect Username/Password", "Login Error");
                }
                return false;

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
