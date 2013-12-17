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
        Dictionary<long, Event> events = null;
        Point cursorPosition;
        SolidBrush brush;
        public DateTime date;
        private Persistence persistence;
        private User user;
        private DataGridView parent;
        private bool hasOverlapped;

        public DataGridViewCalendarCell(DateTime date, Object Value, Persistence persistence, User user)
        {
            InitializeComponent();
            this.parent = this.DataGridView;
            brush = new SolidBrush(Color.FromArgb(128,Color.Blue));
            events = new Dictionary<long,Event>();
            this.Value = Value;
            this.date = date;
            this.persistence = persistence;
            this.user = user;
            this.hasOverlapped = false;
            foreach (DataGridViewColumn column in DataGridView.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            LoadEvents();
        }

        /// <summary>
        /// Loads the events from the local DB
        /// </summary>
        public void LoadEvents()
        {
            events.Clear();
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

           
                    foreach(KeyValuePair<long,Event> calendarEvent in events)
                    {
                        if (!calendarEvent.Value.DeleteEvent && !calendarEvent.Value.drawn)
                        {
                            calendarEvent.Value.rect.X = cellBounds.X + 1;
                            calendarEvent.Value.rect.Y = cellBounds.Y + EventUtilities.TimePoint(calendarEvent.Value.begin);
                            calendarEvent.Value.rect.Width = cellBounds.Width - 4;
                            calendarEvent.Value.rect.Height = cellBounds.Height - 90;
                            calendarEvent.Value.drawn = true;

                            checkBoxOverlap();
                        }
                    }
                    

            foreach (KeyValuePair<long,Event> entry in events)
            {
                graphics.DrawRectangle(Pens.Blue, entry.Value.rect);
                graphics.FillRectangle(brush, entry.Value.rect);
            }
            
        }

        /// <summary>
        /// Generates the right click menu.
        /// </summary>
        private void generateRightClickMenu()
        {
            this.ContextMenuStrip = new ContextMenuStrip();
            LoadEvents();
            List<ToolStripItem> tsItems = new List<ToolStripItem>();
            foreach (KeyValuePair<long, Event> myEvent in events)
            {
                ToolStripItem myTSItem = new ToolStripMenuItem();
                myTSItem.Text = myEvent.Value.name;
                myTSItem.Tag = myEvent.Value;
                myTSItem.Click += new System.EventHandler(EventListClick);
                tsItems.Add(myTSItem);
                this.ContextMenuStrip.Items.Add(myTSItem);
            }


            this.ContextMenuStrip.Items.Add(new ToolStripSeparator());

            this.ContextMenuStrip.Items.Add("Another Setting");

            ContextMenuStrip.Show();
        }

        /// <summary>
        /// Called when the user holds down a mouse button while the pointer is on a cell. Shows the menu
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.DataGridViewCellMouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            base.OnMouseDown(e);
            
            if (e.Button == MouseButtons.Right)
            {
                generateRightClickMenu();
            }
        }


        /// <summary>
        /// Event listener for the right button menu
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        void EventListClick(object sender, EventArgs e)
        {
            ToolStripItem tsItem = sender as ToolStripItem;
            Event myEvent = tsItem.Tag as Event;
            updateEvent(myEvent);
           // generateRightClickMenu(); //Not needed, keep just in case. 
        }

        /// <summary>
        /// Called when the cell is double-clicked. Used to show the event that is clicked. 
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.DataGridViewCellEventArgs" /> that contains the event data.</param>
        protected override void OnDoubleClick(DataGridViewCellEventArgs e)
        {
            base.OnDoubleClick(e);
            Rectangle rect = new Rectangle();
            checkBoxOverlap();
           
            cursorPosition = this.DataGridView.PointToClient(Cursor.Position);
            //addRectangle(rect);

            Event exisitingEvent = checkEvent(cursorPosition);
            if (exisitingEvent == null)
            {
                Form addEvent = new EventModifier(date,false);
                DialogResult addEventResult = addEvent.ShowDialog();
                if (addEventResult == DialogResult.OK)
                {
                    if(addEvent.Tag != null)
                    {
                        Event myGoingToRemoveEvent = (Event)addEvent.Tag;
                        myGoingToRemoveEvent.rect = rect;
                        events.Add(myGoingToRemoveEvent.Key, myGoingToRemoveEvent);
                        this.DataGridView.InvalidateCell(this);
                        persistence.SaveEvent(myGoingToRemoveEvent, user);
                     }
                }
            }
            else if (exisitingEvent.OverLapped == true)
            {
                List<Event> eventList = GetEventsFromCell();
                Form modifyOverlap = new OverlapListViewer(eventList);
                DialogResult modifyOverlapResult = modifyOverlap.ShowDialog();
                if ((Event)modifyOverlap.Tag != null)
                {
                    updateEvent((Event)modifyOverlap.Tag);
                }
            }
            else if (exisitingEvent.OverLapped == false)
            {
                updateEvent(exisitingEvent);
            }
            
        }

        /// <summary>
        /// Returns the event that was clicked. 
        /// </summary>
        /// <param name="cursorPosition">The cursor position.</param>
        /// <returns></returns>
        private Event checkEvent(Point cursorPosition)
        {
            foreach (KeyValuePair<long,Event> calendarEvent in events)
            {
                if (calendarEvent.Value.rect.Contains(cursorPosition))
                {
                    return calendarEvent.Value;
                }
            }

            return null;
        }

        public List<Event> GetEventsFromCell()
        {
            List<Event> eventList = new List<Event>();
            foreach (KeyValuePair<long, Event> pair in events)
            {
                eventList.Add(pair.Value);
            }

            return eventList;
        }

        /// <summary>
        /// Updates the event
        /// </summary>
        /// <param name="exisitingEvent">The exisiting event.</param>
        private void updateEvent(Event exisitingEvent)
        {
            Form editEvent = new EventModifier(exisitingEvent, true);
            DialogResult editEventResult = editEvent.ShowDialog();
            if (editEventResult == DialogResult.No)//
            {
                //Delete the event
                Event deleteEvent = (Event)editEvent.Tag;
                events.Remove(deleteEvent.Key);
                persistence.EditEvent(deleteEvent, user);
                //persistence.DeleteEvent(deleteEvent);
                this.DataGridView.InvalidateCell(this);
            }

            else if (editEventResult == DialogResult.OK)
            {
                Event modifyEvent = (Event)editEvent.Tag;
                persistence.EditEvent(modifyEvent, user);
                events.Remove(modifyEvent.Key);
                events.Add(modifyEvent.Key,modifyEvent);
                modifyEvent.drawn = false;
                LoadEvents();
                foreach (DataGridViewRow row in this.DataGridView.Rows)
                {
                    foreach (DataGridViewCalendarCell cell in row.Cells)
                    {
                        DataGridView.InvalidateCell(cell);
                        cell.LoadEvents();
                    }
                }
                
            }
            
        }
        public void checkBoxOverlap()
        {
            List<Event> eventList = GetEventsFromCell();
            foreach (Event events in eventList)
            {
                foreach (Event myEvent in eventList)
                {
                    if (events.Key != myEvent.Key && events.rect.IntersectsWith(myEvent.rect))
                    {
                        events.OverLapped = true;
                        myEvent.OverLapped = true;
                        this.hasOverlapped = true;
                    }


                    else if (events.Key != myEvent.Key && events.OverLapped == false)
                    {
                        events.OverLapped = false;
                        this.hasOverlapped = false;
                    }
                }
            }
        }

        public void Invalidate()
        {
            DataGridView.InvalidateCell(this);
        }

    }
}
