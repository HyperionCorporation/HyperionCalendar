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
    public partial class OverlapListViewer : Form
    {
        List<Event> eventList;
        int selectedIndex;

        public OverlapListViewer(List<Event> eventList)
        {
            this.eventList = eventList;
            InitializeComponent();

        }

        private void OverlapListViewer_Load(object sender, EventArgs e)
        {
            int count = 0;
            foreach (Event myEvent in eventList)
            {
                if (myEvent.OverLapped == true)
                {
                    ListViewItem itm;
                    string[] arr = new string[6];
                    arr[0] = count.ToString();
                    arr[1] = myEvent.name;
                    arr[2] = myEvent.location;
                    arr[3] = myEvent.begin.ToString();
                    arr[4] = myEvent.end.ToString();
                    arr[5] = myEvent.OverLapped.ToString();
                    itm = new ListViewItem(arr);
                    listView1.Items.Add(itm);
                    count++;
                }
            }

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                this.Tag = eventList[Convert.ToInt32(listView1.SelectedItems[0].Text)];
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select an event", "Input Error");
            }
            
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                this.Tag = eventList[Convert.ToInt32(listView1.SelectedItems[0].Text)];
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select an event", "Input Error");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
