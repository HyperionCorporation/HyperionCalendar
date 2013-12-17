using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace Calendar
{

    static class Settings
    {
        private static Color cellBackground = System.Drawing.Color.White;
        private static Color cellHighlight = System.Drawing.Color.Blue;
        private static Color currentDayColor = System.Drawing.Color.DarkSlateGray;
        private static Color otherMonthColor = System.Drawing.Color.LightGoldenrodYellow;


        private static int syncInterval = 30; //In Minutes
        private static string address = "bryantp.com";
        private static int port = 3306;
        private static string userSQL = "calendarUser";
        private static string password = "EVeA53UptWrW3ehN";
        private static string database = "calendar";

        private static Tuple<int,int, int, int> GetRGB(string RGB)
        {
            //Color [A=255, R=0, G=0, B=255]
            string[] broken = RGB.Split(',',' ','=','[',']');
            int Alpha = Convert.ToInt32(broken[3]);
            int Red = Convert.ToInt32(broken[6]);
            int Green = Convert.ToInt32(broken[9]);
            int Blue = Convert.ToInt32(broken[12]);
            return Tuple.Create<int,int,int,int>(Alpha,Red,Green,Blue);
        }

        public static void ReadSettings()
        {
            using (XmlReader reader = XmlReader.Create("settings.xml"))
            {
                while (reader.Read())
                {
                    // Only detect start elements.
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        // Get element name and switch on it.
                        Tuple<int, int, int, int> returnedValues;
                        switch (reader.Name)
                        {
                            case "SyncInterval":
                                SyncInterval = Convert.ToInt64(reader.ReadString());
                                break;
                            case "Address":
                                address = reader.ReadString();
                                break;
                            case "Port":
                                port = Convert.ToInt32(reader.ReadString());
                                break;
                            case "Password":
                                password = reader.ReadString();
                                break;
                            case "CellBackground":
                                //returnedValues = GetRGB(reader.ReadString());
                                cellBackground = Color.FromArgb(Convert.ToInt32(reader.ReadString()));
                                break;
                            case "CellHighlight":
                                //returnedValues = GetRGB(reader.ReadString());
                                cellHighlight = Color.FromArgb(Convert.ToInt32(reader.ReadString()));
                                break;
                            case "CurrentDayColor":
                                //returnedValues = GetRGB(reader.ReadString());
                                currentDayColor = Color.FromArgb(Convert.ToInt32(reader.ReadString()));
                                break;
                            case "OtherMonthColor":
                                //returnedValues = GetRGB(reader.ReadString());
                                otherMonthColor = Color.FromArgb(Convert.ToInt32(reader.ReadString()));
                                break;
                        }
                    }
                }
            }
            

        }

        public static Color CellBackground
        {
            get { return cellBackground; }
            set { cellBackground = value; }
        }

        public static Color CellHighlight
        {
            get { return cellHighlight; }
            set { cellHighlight = value; }
        }

        public static Color CurrentDayColor
        {
            get { return currentDayColor; }
            set { currentDayColor = value; }
        }

        public static Color OtherMonthColor
        {
            get { return otherMonthColor; }
            set { otherMonthColor = value; }
        }

        public static string Address
        {
            get { return address; }
            set { address = value; }
        }

        public static int Port
        {
            get { return port; }
            set { port = value; }
        }

        public static string ServerUser
        {
            get { return userSQL; }
            set { userSQL = value; }
        }

        public static string Password
        {
            get { return password; }
            set { password = value; }
        }

        public static string DataBase
        {
            get { return database; }
            set { database = value; }
        }

        public static void WriteSettings()
        {
            File.Delete("settings.xml");
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create("settings.xml", settings))
            {

                writer.WriteStartDocument();
                writer.WriteStartElement("Settings");

                //Server settings
                writer.WriteStartElement("Server");
                writer.WriteElementString("SyncInterval", Convert.ToString(SyncInterval));
                writer.WriteElementString("Address", address);
                writer.WriteElementString("Port", Convert.ToString(port));
                writer.WriteElementString("Password", password);
                writer.WriteEndElement();

                writer.WriteStartElement("Colors");
                writer.WriteElementString("CellBackground", cellBackground.ToArgb().ToString());
                writer.WriteElementString("CellHighlight", cellHighlight.ToArgb().ToString());
                writer.WriteElementString("CurrentDayColor", currentDayColor.ToArgb().ToString());
                writer.WriteElementString("OtherMonthColor", otherMonthColor.ToArgb().ToString());
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }


        public static void WriteDefaultSettings()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create("settings.xml",settings))
            {
            
                writer.WriteStartDocument();
                writer.WriteStartElement("Settings");
                
                //Server settings
                writer.WriteStartElement("Server");
                writer.WriteElementString("SyncInterval", Convert.ToString(SyncInterval));
                writer.WriteElementString("Address", address);
                writer.WriteElementString("Port", Convert.ToString(port));
                writer.WriteElementString("Password", password);
                writer.WriteEndElement();

                //Color settings
                writer.WriteStartElement("Colors");
                writer.WriteElementString("CellBackground", cellBackground.ToArgb().ToString());
                writer.WriteElementString("CellHighlight", cellHighlight.ToArgb().ToString());
                writer.WriteElementString("CurrentDayColor", currentDayColor.ToArgb().ToString());
                writer.WriteElementString("OtherMonthColor", otherMonthColor.ToArgb().ToString());
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public static long SyncInterval
        {
            get{return syncInterval * 30 * 100;}
            set
            {  
                syncInterval = (int)(value/(30L*100L));
            }
        }
   
    }

    static class PermanentSettings
    {
        public static readonly string CREATE_EVENTS_TABLE_SQLITE = "create table events (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT," +
                                                                   "begintime INTEGER," +
                                                                   "endtime INTEGER," +
                                                                   "location TEXT," +
                                                                   "description TEXT," +
                                                                   "repeat TEXT," +
                                                                   "reminder TEXT," +
                                                                   "user INTEGER," +
                                                                   "timelastedit INTEGER," +
                                                                   "uid INTEGER," +
                                                                   "deleteEvent INTEGER)";
        
        public static readonly string CREATE_USER_CACHE_TABLE_SQLITE = "create table user (id INTEGER PRIMARY KEY AUTOINCREMENT,"+ 
                                                                       "name TEXT," +
                                                                       "email TEXT UNIQUE ON CONFLICT ABORT," +
                                                                       "password TEXT," +
                                                                       "salt TEXT," +
                                                                       "timelastedit INTEGER)";

        public static readonly string GET_UID_MYSQL = "Select uid FROM users where email = @email limit 1";

        public static readonly string SAVE_USER_MYSQL = "INSERT INTO users (name, email, password, salt, timelastedit) VALUES(@name, @email, @hashedPass, @salt, @now)";

        public static readonly string GET_USER_MYSQL = "SELECT * FROM users where email = @email limit 1";

        public static readonly string SAVE_EVENT_MYSQL = "INSERT INTO events (name,begintime,endtime,location,description,user,timelastedit,uid,deleteEvent) VALUES (@name,@begintime,@endtime,@location,@description,@userid,@timelastedit,@uid,@delete)";

        public static readonly string SAVE_EVENT_SQLITE = "INSERT INTO events (name,begintime,endtime,location,description,user,timelastedit,uid,deleteEvent) VALUES(@name,@begintime,@endtime,@location,@description,@userid,@timelastedit,@uid,@delete)";

        public static readonly string GET_EVENTS_MYSQL = "SELECT * FROM events WHERE user=@user";//"SELECT * FROM events INNER JOIN users ON events.uid = @eventID WHERE user.uid = @userid";
        
        public static readonly string GET_EVENTS_SQLITE = "SELECT * FROM events where user = @uid";

        public static readonly string DELETE_EVENT_SQLITE = "DELETE FROM events where uid = @key";

        public static readonly string DELETE_EVENT_MYSQL = "DELETE FROM events where uid = @key";

        public static readonly string CACHE_USER_SQLITE = "INSERT or REPLACE into user (id,name,email,password,salt,timelastedit) VALUES (@id,@name,@email,@password,@salt,@currentTime)";

        public static readonly string FLUSH_CACHE = "DELETE FROM user";

        public static readonly string FLUSH_EVENT_CACHE = "DELETE FROM events";

        public static readonly string GET_CACHED_USER = "Select * from user where email = @email LIMIT 1";

        public static readonly string UPDATE_EVENT_SQLITE = "UPDATE events SET name=@name,begintime=@begintime,endtime=@endtime,location=@location,description=@description,user=@user,timelastedit=@timelastedit,deleteEvent=@delete where uid=@id";

        public static readonly string SYNC_REMOTE_WITH_LOCAL = "Update events set name=@name,begintime=@begintime,endtime=@endtime,location=@location,description=@description,timelastedit=@timelastedit,deleteEvent=@delete where uid=@uid";
        
        public static readonly string GET_LAST_MODIFIED_TIME_MYSQL = "SELECT timelastedit FROM events WHERE uid=@uid AND user=@userID";
 
    }
}
