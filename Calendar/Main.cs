using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace Calendar
{
    public partial class MainForm : Form
    {
        Calendar currentCalendar;
        DateTime currentDate;
        DateTime currentDateFirstOfMonth;
        private Persistence persistence;
        private User user;
        private Thread syncThread;
        private Sync sync;
        private System.Threading.Timer timer;

        public MainForm()
        {
            InitializeComponent();
            //Get persistence
            persistence = new Persistence();

            sync = new Sync(persistence, null, this);
            //Get user login
            Form signIn = new SignIn(persistence,this,sync);
            DialogResult signInResult = signIn.ShowDialog();
            if (signInResult == DialogResult.OK)
            {
                currentDate = DateTime.Now;
                currentDateFirstOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                DoSettingsWork();
                //Calendar starts on todays date
                lblMonthYear.Text = getDateString(DateTime.Now);
                user = (User)signIn.Tag;
                currentCalendar = new Calendar(DateTime.Now, this, persistence, user, dataGridView1);
                currentCalendar.buildDataSet(dataGridView1);
                this.Text = "Calendar";
                //this.Width = 933;
                //this.Height = 811;
            }
            else if (signInResult == DialogResult.Cancel)
            {
                Application.Exit();
            }

            AutoResetEvent autoEvent = new AutoResetEvent(false);
            sync = new Sync(persistence, user,this);
            timer = new System.Threading.Timer(sync.CheckStatus, autoEvent, 120000, 120000);
            syncThread = new Thread(new ThreadStart(sync.startSync));
            syncThread.Name = "SyncThread";

            syncThread.IsBackground = true;
            syncThread.Start();

            SetCellBackground();
            SetSelectionColor();

        }

        public void SetCellBackground()
        {
            Color a = Color.FromArgb(255, Settings.CellBackground);
            dataGridView1.BackgroundColor = a;
        }

        public void SetSelectionColor()
        {
            dataGridView1.RowsDefaultCellStyle.SelectionBackColor = Settings.CellHighlight;
        }

        /// <summary>
        /// Checks to see if there is a settings file. Creates one if there isn't or loads from the existing one. 
        /// </summary>
        private void DoSettingsWork()
        {
            //Check to see if the existing settings file is there. 
            if (File.Exists("settings.xml"))
            {
                Settings.ReadSettings();
            }
            else
            {
                Settings.WriteDefaultSettings();
            }
        }

        public void refreshAllCells(bool loadEvents)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCalendarCell cell in row.Cells)
                {
                    if(loadEvents)
                        cell.LoadEvents();
                    cell.Invalidate();

                }
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
                currentCalendar = new Calendar(currentDate, this, persistence, user, dataGridView1);
            }

            else
            {
                currentCalendar = new Calendar(currentDateFirstOfMonth, this, persistence, user, dataGridView1);
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
                currentCalendar = new Calendar(currentDate, this, persistence, user, dataGridView1);
            }

            else
            {
                currentCalendar = new Calendar(currentDateFirstOfMonth, this, persistence, user, dataGridView1);
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
            
            if (currentCalendar != null)
            {
                currentCalendar.refreshSize(dataGridView1);
            }

            currentCalendar.refreshPosition(dataGridView1);

            refreshAllCells(false);

            dataGridView1.Height = this.Height - ((this.Height * 4) / 20);
            dataGridView1.Width = this.Width;
            dataGridView1.Location = new Point(0, (this.Height * 4) / 20);

            panel1.Width = this.Width;
            panel1.Height = (this.Height * 2) / 20;
            panel1.Location = new Point(0, (this.Height) / 20);
            
            btnPrevMonth.Location = new Point( (panel1.Width / 3) , panel1.Location.Y / 3);
            btnPrevMonth.Height = (panel1.Height * 8) / 10;

            btnNextMonth.Location = new Point( ((panel1.Width / 3) * 2 ), panel1.Location.Y / 3);
            btnNextMonth.Height = (panel1.Height * 8) / 10;

            lblMonthYear.Location = new Point((panel1.Width / 2) - (lblMonthYear.Width / 3), panel1.Location.Y / 3);
            lblMonthYear.Height = (panel1.Height * 8) / 10;
           
            menuStrip1.Location = new Point(0, 0);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            Form settings = new SettingsFormsGeneral(persistence, user.Name);
            DialogResult result = settings.ShowDialog();
            if (result == DialogResult.OK)
            {
                //update the colors
                SetCellBackground();
                SetSelectionColor();
                currentCalendar.UpdateColors(dataGridView1);//Updates the color for the current day
                refreshAllCells(false);
                Settings.WriteSettings();
            }
        }

        private void SyncButtonPressed(object sender, EventArgs e)
        {
            //Do a manual Sync
            if (!sync.IsSyncing && !DataGridViewCalendarCell.isReadingEventList)
            {
                Thread ManualSyncThread = new Thread(new ThreadStart(sync.DoSync));
                ManualSyncThread.Start();           
            }
        }

        private void btnPrevMonth_Resize(object sender, EventArgs e)
        {
            using (var gr = btnPrevMonth.CreateGraphics())
            {
                Font font = btnPrevMonth.Font;
                for (int size = (int)(btnPrevMonth.Height * 72 / gr.DpiY); size >= 8; --size)
                {
                    font = new Font(btnPrevMonth.Font.FontFamily, size, btnPrevMonth.Font.Style);
                    
                    if (TextRenderer.MeasureText(btnPrevMonth.Text, font).Width <= btnPrevMonth.ClientSize.Width) 
                        break;
                }
                btnPrevMonth.Font = font;
            }
        }

        private void HelpMenu_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if(item.Text == "About")
            {
                Form aboutMenu = new GenericHelpMenu("About", GenericHelpMenu.GenericHelpMenuType.ABOUT);
                aboutMenu.Show();
            }
            else if(item.Text == "Help")
            {
                Form helpMenu = new GenericHelpMenu("Help",GenericHelpMenu.GenericHelpMenuType.HELP);
                helpMenu.Show();
            }
        }

    }

    public class Sync
    {
        private bool doSync;
        private bool running;
        public static bool blockSync;
        private Persistence persistence;
        private User user;
        private MainForm main;

        public Sync(Persistence persistence,User user,MainForm main)
        {
            doSync = false;
            running = true;
            blockSync = false;
            this.persistence = persistence;
            this.user = user;
            this.main = main;
        }

        public User User
        {
            get { return user; }
            set { user = value; }
        }

        public bool IsSyncing
        {
            get { return doSync; }
        }

        /// <summary>
        /// Starts the synchronize loop
        /// </summary>
        public void startSync()
        {
            while (running)
            {
                if (doSync && !blockSync)
                {
                    //Sync
                    persistence.DoSync(user,main);                    
                    doSync = false;
                }
            }
        }

        /// <summary>
        /// Does the manual synchronization
        /// </summary>
        public void DoSync()
        {
            if (!doSync && !DataGridViewCalendarCell.isReadingEventList)
            {
                blockSync = true;
                //Make sure that there is no current sync event going on
                persistence.DoSync(user, main);
                blockSync = false;
            }
        }

        public void CheckStatus(Object stateInfo)
        {
            Console.WriteLine("Timer has ticked " + DateTime.Now.ToShortTimeString());
            doSync = true;
        }
    }
}
