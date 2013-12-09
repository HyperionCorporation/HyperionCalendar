using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calendar
{
    public partial class DataGridViewCalendarCell : DataGridViewTextBoxCell
    {
        //Rectangle[] rectangles; //Gonna have a list of events instead of rectangles
        Dictionary<int, Event> events;
        int rectCount = 0;
        int rectIndex = 0;
        Point cursorPosition;
        SolidBrush brush;
        public DateTime date;
        private Persistence persistence;
        private User user;

        public DataGridViewCalendarCell(DateTime date, Object Value, Persistence persistence, User user)
        {
            InitializeComponent();
            //rectangles = new Rectangle[5];
            brush = new SolidBrush(Color.FromArgb(128,Color.Blue));
            events = new Dictionary<int,Event>();
            this.Value = Value;
            this.date = date;
            this.persistence = persistence;
            this.user = user;
            LoadEvents();
        }

        private void LoadEvents()
        {
            events = persistence.GetEvents(user, date);
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds,Rectangle cellBounds,
            int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue,
            string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
            {
            // Call the base class method to paint the default cell appearance.
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState,
                value, formattedValue, errorText, cellStyle,
                advancedBorderStyle, paintParts);

           
                    foreach(KeyValuePair<int,Event> calendarEvent in events)
                    {
                        if (!calendarEvent.Value.drawn)
                        {
                            calendarEvent.Value.rect.X = cellBounds.X + 1;
                            
                            
                            //if (cursorPosition.X == 0 && cursorPosition.Y == 0)
                            calendarEvent.Value.rect.Y = cellBounds.Y + EventUtilities.TimePoint(calendarEvent.Value.begin);
                            //else
                            //    calendarEvent.Value.rect.Y = cursorPosition.Y + 1;
                            calendarEvent.Value.rect.Width = cellBounds.Width - 4;
                            calendarEvent.Value.rect.Height = cellBounds.Height - 90;
                            calendarEvent.Value.drawn = true;
                        }
                    }
                    

            foreach (KeyValuePair<int,Event> entry in events)
            {
                graphics.DrawRectangle(Pens.Blue, entry.Value.rect);
                graphics.FillRectangle(brush, entry.Value.rect);
            }
            
        }

        protected override void OnMouseEnter(int rowIndex)
        {
            //this.DataGridView.InvalidateCell(this);
        }

        protected override void OnMouseLeave(int rowIndex)
        {
            //this.DataGridView.InvalidateCell(this);
            
        }

        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            base.OnMouseDown(e);
            
            if (e.Button == MouseButtons.Right)
            {
                this.ContextMenuStrip = new ContextMenuStrip();
                Dictionary<int, Event> events;
                events = persistence.GetEvents(user, date);
                List<ToolStripItem> tsItems = new List<ToolStripItem>();
                foreach(KeyValuePair<int, Event> myEvent in events)
                {
                    ToolStripItem myTSItem = new ToolStripMenuItem();
                    myTSItem.Text = myEvent.Value.name;
                    myTSItem.Tag = myEvent.Value;
                    myTSItem.Click += new System.EventHandler(item_Click);
                    tsItems.Add(myTSItem);
                    this.ContextMenuStrip.Items.Add(myTSItem);
                }
                

                this.ContextMenuStrip.Items.Add(new ToolStripSeparator());

                this.ContextMenuStrip.Items.Add("Another Setting");

                ContextMenuStrip.Show();
            }
        }

        void item_Click(object sender, EventArgs e)
        {
            ToolStripItem tsItem = sender as ToolStripItem;
            Event myEvent = tsItem.Tag as Event;
            Form editEvent = new EventModifier(myEvent, true);
            DialogResult editEventResult = editEvent.ShowDialog();
            if (editEventResult == DialogResult.No)
            {
                //Delete the event
                Event deleteEvent = (Event)editEvent.Tag;
                events.Remove(deleteEvent.Key);
                persistence.DeleteEvent(deleteEvent);
                this.DataGridView.InvalidateCell(this);
            }
            else if (editEventResult == DialogResult.OK)
            {
                Event modifyEvent = (Event)editEvent.Tag;
                persistence.EditEvent(modifyEvent, user);
                modifyEvent.drawn = false;
                this.DataGridView.InvalidateCell(this);
            }
        }

        protected override void OnDoubleClick(DataGridViewCellEventArgs e)
        {
            base.OnDoubleClick(e);
            Rectangle rect = new Rectangle();

           
            cursorPosition = this.DataGridView.PointToClient(Cursor.Position);
            //addRectangle(rect);

            Event exisitingEvent = checkEvent(cursorPosition);
            if (exisitingEvent == null)
            {
                Form addEvent = new EventModifier(date,false);
                DialogResult addEventResult = addEvent.ShowDialog();
                if (addEventResult == DialogResult.OK)
                {
                    //This entire thing is just a clusterfuck. Clean it
                    if(addEvent.Tag != null)
                    {
                        Event myGoingToRemoveEvent = (Event)addEvent.Tag;
                       /* MessageBox.Show(myGoingToRemoveEvent.name + "\n" +
                        *    myGoingToRemoveEvent.begin + "\n" +
                        *    myGoingToRemoveEvent.end + "\n" +
                        *    myGoingToRemoveEvent.location + "\n" +
                        *    myGoingToRemoveEvent.description, "Event Information");
                        */
                        myGoingToRemoveEvent.rect = rect;
                        events.Add(myGoingToRemoveEvent.Key, myGoingToRemoveEvent);
                        this.DataGridView.InvalidateCell(this);
                        persistence.SaveEvent(myGoingToRemoveEvent, user);
                     }
                }
            }
            else
            {
                Form editEvent = new EventModifier(exisitingEvent, true);
                DialogResult editEventResult = editEvent.ShowDialog();
                if (editEventResult == DialogResult.No)//
                {
                    //Delete the event
                    Event deleteEvent = (Event)editEvent.Tag;
                    events.Remove(deleteEvent.Key);
                    persistence.DeleteEvent(deleteEvent);
                    this.DataGridView.InvalidateCell(this);
                }

                else if (editEventResult == DialogResult.OK)
                {
                    Event modifyEvent = (Event)editEvent.Tag;
                    persistence.EditEvent(modifyEvent, user);
                    modifyEvent.drawn = false;
                    this.DataGridView.InvalidateCell(this);
                }
            }
            
        }

        private Event checkEvent(Point cursorPosition)
        {
            foreach (KeyValuePair<int,Event> calendarEvent in events)
            {
                if (calendarEvent.Value.rect.Contains(cursorPosition))
                {
                    return calendarEvent.Value;
                }
            }

            return null;
        }

        private void addRectangle(Rectangle rect)
        {
            rect.Y = cursorPosition.Y;
            //rectangles[rectIndex] = rect;
            
            if (rectIndex < 4)
                rectIndex++;
            else
                rectIndex = 0;

            if (rectCount < 4)
                rectCount++;

        }
    }
}
