using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace Calendar
{
    public class Event
    {
        public string name;
        public DateTime begin;//Can't be less than the current day. 
        public DateTime end;//Can't be less than begin.
        public string location;
        public string description;
        public Rectangle rect;
        public bool drawn = false; //Keep track internally if the event has been drawn or not.
        private int key; //Used in the Dictionary in DataFridViewCalendarCell

        public Event(string name, DateTime begin, DateTime end, string location, string description, int key)
        {
            this.name = name;
            this.begin = begin;
            this.end = end;
            this.location = location;
            this.description = description;
            this.key = key;
        }

        public Event(int key)
        {
            this.key = key;
        }

        public int Key
        {
            get { return key; }
        }


    }

    static class EventUtilities
    {
        public static int TimePoint(DateTime time)
        {
            int hours = time.Hour;
            int minutes = time.Minute;

            return (hours + minutes) * 4;

        }
    }
}
