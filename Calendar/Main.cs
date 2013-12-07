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
    public partial class MainForm : Form
    {
        Calendar currentCalendar;
        DateTime currentDate;
        DateTime currentDateFirstOfMonth;
        private Persistence persistence;
        private User user;
        Dictionary<int, Event> events;
        

        public MainForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            //Get persistence
            persistence = new Persistence();


            //Get user login
            Form signIn = new SignIn(persistence);
            DialogResult signInResult = signIn.ShowDialog();
            if(signInResult == DialogResult.OK)
            {
                currentDate = DateTime.Now;
                currentDateFirstOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

                //Calendar starts on todays date
                lblMonthYear.Text = getDateString(DateTime.Now);
                user = (User)signIn.Tag;
                currentCalendar = new Calendar(DateTime.Now, this, persistence, user);
                currentCalendar.buildDataSet(dataGridView1);
                this.Text = "Calendar";
                //this.Width = 933;
                //this.Height = 811;
            }
            else if (signInResult == DialogResult.Cancel)
            {
                currentDate = DateTime.Now;
                currentDateFirstOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

                lblMonthYear.Text = getDateString(DateTime.Now);
                user = new User("Bryan", "Wnt2bsleepin@gmail.com", "sdsdsds", "sdsdsds", 1);
                currentCalendar = new Calendar(DateTime.Now, this, persistence, user);
                currentCalendar.buildDataSet(dataGridView1);
                this.Text = "Calendar";
               //Application.Exit();
            }

           

        }


        private String getDateString(DateTime date)
        {
            string month = string.Empty;
            switch (date.Month)
            {
                case 1:
                    month = "January";
                    break;
                case 2:
                    month = "February";
                    break;
                case 3:
                    month = "March";
                    break;
                case 4:
                    month = "April";
                    break;
                case 5:
                    month = "May";
                    break;
                case 6:
                    month = "June";
                    break;
                case 7:
                    month = "July";
                    break;
                case 8:
                    month = "August";
                    break;
                case 9:
                    month = "September";
                    break;
                case 10:
                    month = "October";
                    break;
                case 11:
                    month = "November";
                    break;
                case 12:
                    month = "December";
                    break;
            }
            return month + " " + date.Year;
        }

        //Moves the calendar display to the next month
        private void btnNextMonth_Click(object sender, EventArgs e)
        {
            currentDate = currentDate.AddMonths(1);
            currentDateFirstOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            if (currentDate.Month == DateTime.Now.Month &&
                           currentDate.Year == DateTime.Now.Year)
            {
                //Put the actual date through, that way correct date is highlighted
                currentCalendar = new Calendar(currentDate, this,persistence,user);
            }

            else
            {
                currentCalendar = new Calendar(currentDateFirstOfMonth, this,persistence,user);
            } 
            
            dataGridView1.Rows.Clear();
            currentCalendar.buildDataSet(dataGridView1);
            lblMonthYear.Text = getDateString(currentDateFirstOfMonth);
            if (currentDate.Month != DateTime.Now.Month)
                dataGridView1.ClearSelection();
            else
            {
                dataGridView1.ClearSelection();
                currentCalendar.selectCurrentDate(dataGridView1);
            }

            Console.WriteLine(currentCalendar.getWidhtHeight(dataGridView1));
        }

        private void btnPrevMonth_Click(object sender, EventArgs e)
        {
            currentDate = currentDate.AddMonths(-1);
            currentDateFirstOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

            if (currentDate.Month == DateTime.Now.Month &&
                currentDate.Year == DateTime.Now.Year)
            {
                //Put the actual date through, that way correct date is highlighted
                currentCalendar = new Calendar(currentDate, this,persistence,user);
            }

            else
            {
                currentCalendar = new Calendar(currentDateFirstOfMonth, this,persistence,user);
            }

            dataGridView1.Rows.Clear();
            currentCalendar.buildDataSet(dataGridView1);
            lblMonthYear.Text = getDateString(currentDateFirstOfMonth);
            dataGridView1.ClearSelection();

            if (currentDate.Month == DateTime.Now.Month)
                currentCalendar.selectCurrentDate(dataGridView1);
            
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            if (currentCalendar == null)
                Application.Exit();
            else   
                currentCalendar.selectCurrentDate(dataGridView1);
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sa = persistence.GetMySQLServer();
            Form settings = new SettingsFormsGeneral(sa, user.Name);
            DialogResult result = settings.ShowDialog();
        }

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            String test;
            test = dataGridView1.CurrentCell.Value.ToString();
        }
    }
}
