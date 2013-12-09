﻿using System;
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
        Dictionary<int, Event> events = null;
        int rectCount = 0;
        int rectIndex = 0;
        Point cursorPosition;
        SolidBrush brush;
        public DateTime date;
        private Persistence persistence;
        private User user;
        private DataGridView parent;

        public DataGridViewCalendarCell(DateTime date, Object Value, Persistence persistence, User user)
        {
            InitializeComponent();
            this.parent = this.DataGridView;
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

           
                    foreach(KeyValuePair<int,Event> calendarEvent in events)
                    {
                        if (!calendarEvent.Value.drawn)
                        {
                            calendarEvent.Value.rect.X = cellBounds.X + 1;
                            calendarEvent.Value.rect.Y = cellBounds.Y + EventUtilities.TimePoint(calendarEvent.Value.begin);
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

        /// <summary>
        /// Generates the right click menu.
        /// </summary>
        private void generateRightClickMenu()
        {
            this.ContextMenuStrip = new ContextMenuStrip();
            LoadEvents();
            List<ToolStripItem> tsItems = new List<ToolStripItem>();
            foreach (KeyValuePair<int, Event> myEvent in events)
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
            else
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
            foreach (KeyValuePair<int,Event> calendarEvent in events)
            {
                if (calendarEvent.Value.rect.Contains(cursorPosition))
                {
                    return calendarEvent.Value;
                }
            }

            return null;
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
                persistence.DeleteEvent(deleteEvent);
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
    }
}
