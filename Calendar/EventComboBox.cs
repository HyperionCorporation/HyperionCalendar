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
    public partial class EventComboBox : System.Windows.Forms.ComboBox
    {
        List<Event> events;

        public EventComboBox()
        {
            InitializeComponent();
            events = new List<Event>();
        }

        public void addEvent(Event myEvent)
        {
            events.Add(myEvent);
        }

     

        private void EventComboBox_Click(object sender, EventArgs e)
        {
            EventComboBox box = sender as EventComboBox;
            if (box.SelectedIndex >= 0)
            {
                Event ev = events[box.SelectedIndex];
                Console.WriteLine("STALL");
            }
        }
    }
}
