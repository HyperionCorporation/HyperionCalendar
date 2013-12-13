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
    //Allows you to create, edit and delete events
    public partial class EventModifier : Form
    {
        private DateTime date;
        private Event newEvent;
        private bool editing;
        private Random random;

        public EventModifier(DateTime date, bool editing)
        {
            this.date = date;
            InitializeComponent();
            this.editing = editing;
            random = new Random();
            newEvent = new Event(DateTime.Now.Ticks + random.Next());

            if (!editing)
            {
                this.btnDelete.Enabled = false;
                //Fill in the DateTime values with things that make sense. 
                dtpBeginDate.Value = date;
                dtpEndDate.Value = DateTime.Today;
                dtpBeginTime.Value = date;
                dtpEndTime.Value = DateTime.Now;
            }
            
        }

        public EventModifier(Event existingEvent, bool editing)
       {
            InitializeComponent();
            this.editing = editing;
            newEvent = existingEvent;
      
            if (editing)
            {
                txtBoxName.Text = newEvent.name;
                txtBoxLocation.Text = newEvent.location;
                txtBoxDescription.Text = newEvent.description;
                dtpBeginDate.Value = newEvent.begin;
                dtpBeginTime.Value = newEvent.begin;
                dtpEndDate.Value = newEvent.end;
                dtpEndTime.Value = newEvent.end;
            }
       }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //Validate Here
            if (validate())
            {
                newEvent.name = txtBoxName.Text;
                newEvent.location = txtBoxLocation.Text;
                newEvent.description = txtBoxDescription.Text;
                newEvent.begin = new DateTime(dtpBeginDate.Value.Year,dtpBeginDate.Value.Month,dtpBeginDate.Value.Day,dtpBeginTime.Value.Hour,dtpBeginTime.Value.Minute,dtpBeginTime.Value.Second); //Take the date and the time to make a correct DateTime obj
                newEvent.end = new DateTime(dtpEndDate.Value.Year, dtpEndDate.Value.Month, dtpEndDate.Value.Day, dtpEndTime.Value.Hour, dtpEndTime.Value.Minute, dtpEndTime.Value.Second);
                newEvent.LastModified = DateTime.Now;
                this.Tag = newEvent;
                this.DialogResult = DialogResult.OK;
            }
         }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //Delete the event
            newEvent.DeleteEvent = true;
            this.Tag = newEvent;
            this.DialogResult = DialogResult.No;
        }

        private bool validate()
        {
            if (txtBoxName.Text == String.Empty) 
            {
                MessageBox.Show("Please enter a name", "Input Error"); 
                return false;
            }

            else if (txtBoxLocation.Text == String.Empty)
            {
                MessageBox.Show("PLease enter a location","Input Error");
                return false;
            }

            else if (dtpEndDate.Value.Month < dtpBeginDate.Value.Month ||
                    dtpEndDate.Value.Day < dtpBeginDate.Value.Day ||
                    dtpEndDate.Value.Year < dtpBeginDate.Value.Year)
            {
                MessageBox.Show("Please enter an end date that is in the future", "Input Error");
                return false;
            }

            else if (dtpEndDate.Value.Month == dtpBeginDate.Value.Month &&
                    dtpBeginDate.Value.Day == dtpEndDate.Value.Day &&
                    dtpBeginDate.Value.Year == dtpEndDate.Value.Year)
            {
                //Event is on the same date. Check times. 
                if (dtpEndTime.Value <= dtpBeginTime.Value)
                {
                    MessageBox.Show("Please enter a time that is in the future", "Entry Error");
                    return false;
                }

                return true;
            }
            return true;
        }


        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Tag = null;
            this.DialogResult = DialogResult.OK;
        }

    }

}
