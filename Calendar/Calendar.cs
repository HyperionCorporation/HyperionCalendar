using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calendar
{
    class Calendar
    {
        private DateTime date;
        private const int numRowsInCalendarView = 6; //Number of Rows (7 days * 6 Rows = 42 boxes) in a month view
        private const int numColumnsInCalendarView = 7; //7 Days in a week.
        private const int numDaysInCalendarView = 42; //A calendar will always have room for 42 days.
        private MainForm mainForm;
        private Persistence persistence;
        private User user;

        public Calendar(DateTime date, MainForm mainForm, Persistence persistence, User user)
        {
            this.date = date;
            this.mainForm = mainForm;
            this.persistence = persistence;
            this.user = user;
        }


        public List<Event> GetEventsForMonth(DataGridView calendarView)
        {
            List<Event> theBigOne = new List<Event>();
            foreach(DataGridViewRow row in calendarView.Rows)
            {
                foreach (DataGridViewCalendarCell cell in row.Cells)
                {
                    if (cell.date.Month == date.Month)
                    {
                        List<Event> eventsInCell = cell.GetEventsFromCell();
                        foreach (Event myEvent in eventsInCell)
                        {
                            theBigOne.Add(myEvent);
                        }
                    }
                }
            }

            return theBigOne;
        }

        //Generate the rows and columns for the Calendar
        public void buildDataSet(DataGridView calendarView)
        {
            setColumnHeaders(calendarView);
            int monthLength = DateTime.DaysInMonth(date.Year, date.Month);
            bool endOfMonthReached = false;
            //Build the calendar based on the current date
            
            DateTime firstOfCurrentMonth = new DateTime(date.Year, date.Month, 1);
            int start = generateFirstWeek(calendarView, firstOfCurrentMonth);
            DateTime startDate = new DateTime(date.Year, date.Month, start); //Gets the date that the week generation starts on after the first week
            for (int i = 0; i <= numRowsInCalendarView - 3; i++)
            {
               //Generate everything in between here.
                DataGridViewRow row = new DataGridViewRow();
               
                for (int j = 1; j <= 7; j++)
                {
                    if (start > DateTime.DaysInMonth(firstOfCurrentMonth.Year, firstOfCurrentMonth.Month))
                    {
                        start = 1;
                        endOfMonthReached = true;
                    }

                   addCellToRow(row, start.ToString(), startDate);
                   startDate = startDate.AddDays(1);
                   start++;
                }

                calendarView.Rows.Add(row);
             }
            //Generate last week here   
            DateTime lastOfCurrentMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
            generateLastWeek(calendarView, lastOfCurrentMonth, start, endOfMonthReached);

            for (int i = 0; i < calendarView.Rows.Count; i++)
            {
                calendarView.Rows[i].Height = mainForm.Height / 7;
            }

            shadeCalendar(calendarView);

        }

        private void generateLastWeek(DataGridView calendarView, DateTime lastOfCurrentMonth, int currentDate, bool endOfMonthReached)
        {
            DataGridViewRow row = new DataGridViewRow();
            int count = currentDate;
            int nextMonthCount = 1;

            if (lastOfCurrentMonth.DayOfWeek != DayOfWeek.Sunday && !endOfMonthReached)
            {
                int nextMonth = lastOfCurrentMonth.AddMonths(1).Month;
                bool[] fallsOnNextMonth = generateNextMonth(lastOfCurrentMonth.AddMonths(1));

                for (int i = 0; i < 7; i++)
                {
                    if (fallsOnNextMonth[i])
                    {
                        DateTime cellDate = new DateTime(date.AddMonths(1).Year,date.AddMonths(1).Month,nextMonthCount);
                        addCellToRow(row, nextMonthCount.ToString(),cellDate);
                        nextMonthCount++;
                    }
                    else
                    {
                       DateTime cellDate = new DateTime(date.Year, date.Month, count);
                       addCellToRow(row, count.ToString(),cellDate);
                       count++;
                    }

                }

                calendarView.Rows.Add(row);
         
            }

            else
            {
                DateTime cellDate;
                for (int i = 0; i < 7; i++)
                {
                    if (currentDate != DateTime.DaysInMonth(date.Year,date.Month) && currentDate > DateTime.DaysInMonth(date.AddMonths(1).Year, date.AddMonths(1).Month))
                    {
                        currentDate = 1; //Reset the date to 1 if we go over the current month's max
                    }

                    if (currentDate <= DateTime.DaysInMonth(date.Year, date.Month))
                    {
                        cellDate = new DateTime(date.Year, date.Month, currentDate);
                    }
                    else
                    {
                        cellDate = new DateTime(date.AddMonths(1).Year, date.AddMonths(1).Month, currentDate);
                    }
                    addCellToRow(row, currentDate.ToString(), cellDate);
                    currentDate++;      
                }

                calendarView.Rows.Add(row);
            
            }
 
        }

        //Generates the first week. Returns the last numerical day of that week
        private int generateFirstWeek(DataGridView calendarView, DateTime firstOfCurrentMonth)
        {
            //All of this works for the first week of the month
            DataGridViewRow row = new DataGridViewRow();
            int lastMonth = firstOfCurrentMonth.AddMonths(-1).Month; //Numerical representation of the previous month
            int lastNumDayOfLastMonth = DateTime.DaysInMonth(date.Year, lastMonth);
            int count = 1;

            //If the new month begins on a day other than Sunday, then the previous month will have days shown
            if (firstOfCurrentMonth.DayOfWeek != DayOfWeek.Sunday)
            {
               
                bool[] fallOnPreviousMonth = generateLastMonth(firstOfCurrentMonth.AddMonths(-1));
                DateTime currentDate = new DateTime(date.Year, date.Month, 1);
                for (int i = 0; i < 7; i++)
                {
                    if (fallOnPreviousMonth[i])
                    {
                        string value =  (lastNumDayOfLastMonth - ((int)firstOfCurrentMonth.DayOfWeek - (i+1))).ToString();
                        DateTime lastMontDate = new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, Convert.ToInt32(value));
                        addCellToRow(row, value, lastMontDate);
                    }
                    else
                    {
                        addCellToRow(row, count.ToString(),currentDate);
                        currentDate = currentDate.AddDays(1);
                        count++;
                    }
                }
                
                calendarView.Rows.Add(row);
                return count;
            }
            
            //The week starts on a Sunday
            else
            {
                int lastMonthStart = lastNumDayOfLastMonth - 6;
                for (int i = 0; i < 7; i++)
                {
                    //The previous month starts on the first week of the calendar display
                    if (lastMonthStart != 1)
                    {
                        DateTime cellDate = new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, lastMonthStart);
                        addCellToRow(row, lastMonthStart.ToString(), cellDate);
                        lastMonthStart++;
                    }
                    else
                    {
                        DateTime cellDate = new DateTime(date.Year, date.Month, lastMonthStart);
                        addCellToRow(row, lastMonthStart.ToString(), cellDate);
                        lastMonthStart++;
                    }
                }
                
                calendarView.Rows.Add(row);
                return 1;

            }
        }
        
        //Determines if the day falls on the next month or not
        private bool[] generateNextMonth(DateTime nextMonth)
        {
            //Determine what day of the week the first of the next month falls on
            DayOfWeek firstDayOfMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1).DayOfWeek;
            bool[] fallOnNextMonth = new bool[7];

            for (int i = (int)firstDayOfMonth; i < 7; i++)
            {
                fallOnNextMonth[i] = true;
            }

            for (int i = 0; i < (int)firstDayOfMonth; i++)
            {
                fallOnNextMonth[i] = false;
            }

            return fallOnNextMonth;
        }

        /// <summary>
        /// Determines if the day falls on the previous month or not
        /// </summary>
        /// <param name="lastMonth">The last month.</param>
        /// <returns></returns>
        private bool[] generateLastMonth(DateTime lastMonth)
        {
            int lastNumDayOfMonth = DateTime.DaysInMonth(lastMonth.Year,lastMonth.Month);
            DayOfWeek lastDayOfMonth = new DateTime(lastMonth.Year, lastMonth.Month, lastNumDayOfMonth).DayOfWeek;


            bool[] fallOnPreviousMonth = new bool[7];

            //Go backwards. Everything < lastDayOfMonth is in the previous Month
            for (int i = (int)lastDayOfMonth; i >= 0; i--)
            {
                fallOnPreviousMonth[i] = true;
            }

            for (int i = (int)lastDayOfMonth + 1; i < 7; i++)
            {
                fallOnPreviousMonth[i] = false;
            }

            return fallOnPreviousMonth;

        }

        //Adds the cell to the row
        private void addCellToRow(DataGridViewRow row, string value, DateTime cellDate)
        {
            DataGridViewCalendarCell newCell = new DataGridViewCalendarCell(cellDate, value,persistence,user);
            row.Cells.Add(newCell);

        }

        /// <summary>
        /// Sets the column headers.
        /// </summary>
        /// <param name="calendarView">The calendar view.</param>
        private void setColumnHeaders(DataGridView calendarView)
        {
            calendarView.ColumnCount = numColumnsInCalendarView;
            //Add the ability to change them to shorthand. i.e We
            calendarView.Columns[0].Name = "Sunday";
            calendarView.Columns[1].Name = "Monday";
            calendarView.Columns[2].Name = "Tuesday";
            calendarView.Columns[3].Name = "Wednesday";
            calendarView.Columns[4].Name = "Thursday";
            calendarView.Columns[5].Name = "Friday";
            calendarView.Columns[6].Name = "Saturday";
            calendarView.RowHeadersVisible = false;
           

            for (int i = 0; i <= 6; i++)
            {
                calendarView.Columns[i].Width = mainForm.Width / 7;
                calendarView.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;                
            }

        }

        /// <summary>
        /// Selects the current date.
        /// </summary>
        /// <param name="calendarView">The calendar view.</param>
        public void selectCurrentDate(DataGridView calendarView)
        {
            foreach (DataGridViewRow row in calendarView.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    DataGridViewCalendarCell calCell = cell as DataGridViewCalendarCell;
                    DateTime dateInCell = calCell.date;
                    if (dateInCell.Day == date.Day && dateInCell.Month == DateTime.Now.Month && dateInCell.Year == DateTime.Now.Year)
                    {
                        //cell.Selected = true;
                        calCell.IsCurrentDay = true;
                        calCell.CurrentDayColor = Settings.CurrentDayColor;
                        //cell.Style.BackColor = Settings.CurrentDayColor;
                        break; //In case today is the first, don't want to highlight it twice
                    }
                }
            }

        }

        /// <summary>
        /// Updates the color of the cells
        /// </summary>
        /// <param name="calendarView">The calendar view.</param>
        public void UpdateColors(DataGridView calendarView)
        {
            foreach (DataGridViewRow row in calendarView.Rows)
            {
                foreach (DataGridViewCalendarCell cell in row.Cells)
                {
                    if (cell.IsCurrentDay)
                        cell.CurrentDayColor = Settings.CurrentDayColor;
                    else if (cell.IsOtherMonth)
                        cell.OtherMonthColor = Settings.OtherMonthColor;
                    else
                        cell.Style.BackColor = Settings.CellBackground;
                }
            }

        }

        //Shades all the cells of the previous and next month
        private void shadeCalendar(DataGridView calendarView)
        {
            int rowIndex = 1;
            int numDaysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
            int cellCount = 0;

            foreach (DataGridViewRow row in calendarView.Rows)
            {
                int dateInFirstCell = Convert.ToInt32((string)row.Cells[0].FormattedValue);

                if (dateInFirstCell != 1 && rowIndex == 1)
                {
                    //The first date in the calendar is the previous month

                    DataGridViewCalendarCell cell = (DataGridViewCalendarCell)row.Cells[0];
                    int currentCellDate = Convert.ToInt32((string)cell.FormattedValue);
                    int index = 0;

                    while (currentCellDate != 1)
                    {
                        cell.OtherMonthColor = Settings.OtherMonthColor;
                        cell.IsOtherMonth = true;
                        index++;
                        if (index > 6)
                            break;
                        cell = (DataGridViewCalendarCell)row.Cells[index];
                        currentCellDate = Convert.ToInt32((string)cell.FormattedValue);
                    }
                }

                else if(rowIndex > 4)
                {
                    foreach (DataGridViewCalendarCell cell in row.Cells)
                    {
                        cellCount++;
                        int dateInCell = Convert.ToInt32((string)cell.FormattedValue);
                        if (dateInCell < 15)
                        {
                            cell.IsOtherMonth = true;
                            cell.OtherMonthColor = Settings.OtherMonthColor;
                        }
                    }
                }


                rowIndex++;
            }
        }

        public string getWidhtHeight(DataGridView calendarView)
        {
            return calendarView.Width + " " + calendarView.Height;
        }

        public void refreshSize(DataGridView calendarView)
        {
            for (int i = 0; i < calendarView.Rows.Count; i++)
            {
                calendarView.Rows[i].Height = mainForm.Height / 7;
            }
            for (int i = 0; i <= 6; i++)
            {
                calendarView.Columns[i].Width = mainForm.Width / 7;
                calendarView.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

        }
        
    }


}
