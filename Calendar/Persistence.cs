using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Threading;

namespace Calendar
{
    public class Persistence
    {
        private SQLitePersistance sqlitePersitance;
        private MySQLPersistence mysqlPersitance;

        public enum Location
        {
            SQLITE,
            MYSQL,
            NULL
        }

        public Persistence()
        {
            //Create an SQLite Persistence Object
            sqlitePersitance = new SQLitePersistance();
            mysqlPersitance = new MySQLPersistence();
        }

        /// <summary>
        /// Saves the for a user. 
        /// </summary>
        /// <param name="myEvent">The event to be saved</param>
        /// <param name="user">The user to save the event under.</param>
        /// <returns></returns>
        public int SaveEvent(Event myEvent, User user)
        {
            return sqlitePersitance.SaveEvent(myEvent, user);
        }

        public void EditEvent(Event myEvent,User myUser)
        {
            sqlitePersitance.EditEvent(myEvent, myUser);

        }
        /// <summary>
        /// Gets the events for a user on a date. 
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public Dictionary<int,Event> GetEvents(User user, DateTime date)
        {
            List<Event> BuildAList = sqlitePersitance.getEvents(user.UID);
            Dictionary<int, Event> eventDict = new Dictionary<int, Event>();
            foreach(Event myEvent in BuildAList)
            {
                if (myEvent.begin.Month == date.Month &&
                    myEvent.begin.Day == date.Day &&
                    myEvent.begin.Year == date.Year)
                {
                    eventDict.Add(myEvent.Key, myEvent);
                }
            }

            return eventDict;
        }

        /// <summary>
        /// Checks if the user exists. 
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>The location of the user, remote or cached locally</returns>
        public Location UserExists(string email)
        {
            //Check the SQLite first to see if the user is cached. 
            User userCached = sqlitePersitance.GetCachedUser(email);

            if (userCached == null)
            {
                User user = mysqlPersitance.GetUser(email);

                if (user != null)
                    return Location.MYSQL;
                else
                {
                    return Location.NULL;
                }
            }

            return Location.SQLITE; 
        }

        /// <summary>
        /// Saves the user n the remote DB
        /// </summary>
        /// <param name="user">The user.</param>
        public void SaveUser(User user)
        {
            bool usersSaved = mysqlPersitance.SaveUser(user);
            if (!usersSaved)
                MessageBox.Show("Error saving user", "Save Error");
        }

        /// <summary>
        /// Caches the user in the local DB
        /// </summary>
        /// <param name="user">The user.</param>
        public void CacheUser(User user)
        {
            bool userSaved = sqlitePersitance.CacheUser(user);
            if(!userSaved)
                MessageBox.Show("Error Caching User","Save Error");
        }

        /// <summary>
        /// Returns the user in the cache. 
        /// </summary>
        /// <returns></returns>
        public User GetCachedUser(string email)
        {
            return sqlitePersitance.GetCachedUser(email);
        }

        /// <summary>
        /// Flushes the cache.
        /// </summary>
        public void FlushCache()
        {
            sqlitePersitance.FlushCache();
        }

        /// <summary>
        /// Gets the user from the remote DB
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public User GetUser(string email)
        {
            return mysqlPersitance.GetUser(email);
        }

        /// <summary>
        /// Deletes the event from the local DB. It should reflect to the remote when it updates. 
        /// </summary>
        /// <param name="deleteEvent">The delete event.</param>
        public void DeleteEvent(Event deleteEvent)
        {
            sqlitePersitance.DeleteEvent(deleteEvent);
        }

        public string GetMySQLServer()
        {
            return mysqlPersitance.Server;
        }

        public bool CheckRemoteConnection()
        {
            return mysqlPersitance.CheckConnection();
        }

    }


    /// <summary>
    /// SQLite Persistence Object
    /// </summary>
    class SQLitePersistance
    {
        private const string localDatabase = "calendar.sqlite";
        private SQLiteConnection localDBConnection;

        public SQLitePersistance()
        {
            Preperations();
        }

        /*
        * Checks to see if a database already exists. If it is
        * then it will make the database and tables.
        * */
        private void Preperations()
        {
            if (File.Exists(localDatabase))
                return;


            SQLiteConnection.CreateFile(localDatabase); //Create the database
            SQLiteCommand command;
            //Create the two tables. 
            localDBConnection = new SQLiteConnection("Data Source=" + localDatabase + ";Version=3;");
            localDBConnection.Open();


            command = new SQLiteCommand(PermanentSettings.CREATE_USER_CACHE_TABLE_SQLITE, localDBConnection);
            command.ExecuteNonQuery();

            string eventTableCreateQuery = PermanentSettings.CREATE_EVENTS_TABLE_SQLITE;
            command = new SQLiteCommand(eventTableCreateQuery, localDBConnection);
            command.ExecuteNonQuery();
            localDBConnection.Close();
        }

        public int SaveEvent(Event myEvent,User myUser)
        {
            string name = myEvent.name;
            DateTime begin = myEvent.begin;
            DateTime end = myEvent.end;
            string location = myEvent.location;
            string description = myEvent.description;

            try
            {
                localDBConnection = new SQLiteConnection("Data Source=" + localDatabase + ";Version=3;");
                localDBConnection.Open();

                SQLiteCommand cmd = localDBConnection.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = PermanentSettings.SAVE_EVENT_SQLITE;
                cmd.Parameters.Add(new SQLiteParameter("@name", name));
                cmd.Parameters.Add(new SQLiteParameter("@begintime", begin.Ticks));
                cmd.Parameters.Add(new SQLiteParameter("@endtime", end.Ticks));
                cmd.Parameters.Add(new SQLiteParameter("@location", location));
                cmd.Parameters.Add(new SQLiteParameter("@description", description));
                cmd.Parameters.Add(new SQLiteParameter("@userid", myUser.UID));
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                localDBConnection.Close();
            }
        }

        /// <summary>
        /// Deletes an Event from the Local DB
        /// </summary>
        /// <param name="deleteEvent">The delete event.</param>
        public void DeleteEvent(Event deleteEvent)
        {
            localDBConnection = new SQLiteConnection("Data Source=" + localDatabase + ";Version=3;");
            localDBConnection.Open();

            try
            {
                SQLiteCommand cmd = localDBConnection.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = PermanentSettings.DELETE_EVENT_SQLITE;
                cmd.Parameters.Add(new SQLiteParameter("@key", deleteEvent.Key));
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {

            }
            finally
            {
                localDBConnection.Close();
                }
        }

        /// <summary>
        /// Gets the event for a User from the SQLite DB
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public List<Event> getEvents(int? uid)
        {
            if (uid == null)
                return null;

            localDBConnection = new SQLiteConnection("Data Source=" + localDatabase + ";Version=3;");
            localDBConnection.Open();
            SQLiteCommand cmd = localDBConnection.CreateCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = PermanentSettings.GET_EVENTS_SQLITE;
            cmd.Parameters.AddWithValue("@uid", uid);
            SQLiteDataReader reader = cmd.ExecuteReader();
            List<Event> eventList = new List<Event>();
            try
            {
                while (reader.Read())
                {
                    String name = (string)reader["name"];
                    long begintime = (long)reader["begintime"];
                    long endtime = (long)reader["endtime"];
                    String location = (string)reader["location"];
                    String description = (string)reader["description"];
                    var key = reader.GetInt32(0);
                    eventList.Add(new Event(name, new DateTime(begintime), new DateTime(endtime), location, description,(int)key));
                }

            }
            catch (InvalidCastException)
            {
                //There were no rows returned, columns are null.
                return null;
            }
            finally
            {
                localDBConnection.Close();
            }

            return eventList;
        }

        /// <summary>
        /// Gets the cached user. 
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public User GetCachedUser(string email)
        {
            localDBConnection = new SQLiteConnection("Data Source=" + localDatabase + ";Version=3;");
            localDBConnection.Open();

            SQLiteCommand cmd = localDBConnection.CreateCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = PermanentSettings.GET_CACHED_USER;
            cmd.Parameters.Add(new SQLiteParameter("@email", email.ToLower()));
            SQLiteDataReader reader = cmd.ExecuteReader();

            try
            {
                string name = (string)reader["name"];
                string DBEmail = (string)reader["email"];
                string hashedPassword = (string)reader["password"];
                string salt = (string)reader["salt"];
                int key = Convert.ToInt32((long)reader["id"]);
                return new User(name, DBEmail, hashedPassword, salt, key);
            }
            catch (InvalidCastException)
            {
                //There were no rows returned, columns are null.
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            finally
            {
                localDBConnection.Close();
            }

        }


        public void EditEvent(Event myEvent, User myUser)
        {
            try
            {
                localDBConnection = new SQLiteConnection("Data Source=" + localDatabase + ";Version=3;");
                localDBConnection.Open();

                SQLiteCommand cmd = localDBConnection.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = PermanentSettings.UPDATE_EVENT_SQLITE;
                cmd.Parameters.AddWithValue("@name", myEvent.name);
                cmd.Parameters.AddWithValue("@begintime", myEvent.begin.Ticks);
                cmd.Parameters.AddWithValue("@endtime", myEvent.end.Ticks);
                cmd.Parameters.AddWithValue("@location", myEvent.location);
                cmd.Parameters.AddWithValue("@description", myEvent.description);
                cmd.Parameters.AddWithValue("@user", myUser.UID);
                cmd.Parameters.AddWithValue("@id", myEvent.Key);

                int affected = cmd.ExecuteNonQuery();
                Console.Write("");
            }
            catch (Exception e)
            {
                MessageBox.Show("There Was An Error Updating Event " + e.Message,"Update Error");
            }
            finally
            {
                localDBConnection.Close();
            }
        }

        public void FlushCache()
        {
            try
            {
                localDBConnection = new SQLiteConnection("Data Source=" + localDatabase + ";Version=3;");
                localDBConnection.Open();

                SQLiteCommand cmd = localDBConnection.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = PermanentSettings.FLUSH_CACHE;
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error Flushing Cache: " + e.Message);
            }
            finally
            {
                localDBConnection.Close();
            }
        }

        /// <summary>
        /// Caches the in the local DB
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public bool CacheUser(User user)
        {
            string name = user.Name;
            string email = user.Email;
            string hashedPass = user.HashedPassword;
            string salt = user.Salt;

            try
            {
                localDBConnection = new SQLiteConnection("Data Source=" + localDatabase + ";Version=3;");
                localDBConnection.Open();

                SQLiteCommand cmd = localDBConnection.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = PermanentSettings.CACHE_USER_SQLITE;
                cmd.Parameters.Add(new SQLiteParameter("@name", name));
                cmd.Parameters.Add(new SQLiteParameter("@email", email.ToLower()));
                cmd.Parameters.Add(new SQLiteParameter("@password", hashedPass));
                cmd.Parameters.Add(new SQLiteParameter("@salt", salt));
                cmd.Parameters.Add(new SQLiteParameter("@currentTime", DateTime.Now.Ticks));
                int rows = (int)cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error Caching User: " + e.Message);
                return false;
            }
            finally
            {
                localDBConnection.Close();
            }
        }
    }

    class MySQLPersistence
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        public MySQLPersistence()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes the Database
        /// </summary>
        private void Initialize()
        {
            server = "bryantp.com";
            database = "calendar";
            uid = "calendarUser";
            password = "EVeA53UptWrW3ehN";

            string connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        /// <summary>
        /// Opens the connection.
        /// </summary>
        /// <returns></returns>
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException  e)
            {
                return false;
            }
            catch (Exception e)
            {
                if (e.Message == "The connection is already open.")
                    return true;
                return false;
            }
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        /// <returns></returns>
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets the event for user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public List<Event> getEventForUser(User user)
        {
            string query = PermanentSettings.GET_EVENTS_MYSQL;
            connection.Close();
            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@userid", user.UID);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();
                List<Event> eventList = new List<Event>();
                Random rand = new Random();

                //Read the data and store them in the list
                if (dataReader.Read())
                {
                    String name = (string)dataReader["name"];
                    long begintime = (long)dataReader["begintime"];
                    long endtime = (long)dataReader["endtime"];
                    String location = (string)dataReader["location"];
                    String description = (string)dataReader["description"];

                    eventList.Add(new Event(name, new DateTime(begintime), new DateTime(endtime), location, description, rand.Next(100)));
                }



                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return eventList;
            }

            else
            {
                MessageBox.Show("Error Connecting to Server", "Connection Error");
                return null;
            }
        }

        /// <summary>
        /// Saves the event to the MySQL Database
        /// </summary>
        /// <param name="myEvent">My event.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public bool SaveEvent(Event myEvent, User user)
        {
            string name = myEvent.name;
            DateTime begin = myEvent.begin;
            DateTime end = myEvent.end;
            string location = myEvent.location;
            string description = myEvent.description;
            string query = PermanentSettings.SAVE_EVENT_MYSQL;

            if (this.OpenConnection())
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@begintime", begin.Ticks);
                cmd.Parameters.AddWithValue("@endtime", end.Ticks);
                cmd.Parameters.AddWithValue("@location", location);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@userid", user.UID);
                //Execute command
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
                return true;
            }


            return false;
        }



        /// <summary>
        /// Saves the user in a MySQL Database
        /// </summary>
        /// <param name="user">The user.</param>
        public bool SaveUser(User user)
        {
            string name = user.Name;
            string email = user.Email;
            string hashedPass = user.HashedPassword;
            string salt = user.Salt;
            string query = PermanentSettings.SAVE_USER_MYSQL;
            string getuidquery = PermanentSettings.GET_UID_MYSQL;

            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@email", email.ToLower());
                cmd.Parameters.AddWithValue("@hashedPass", hashedPass);
                cmd.Parameters.AddWithValue("@salt", salt);
                cmd.Parameters.AddWithValue("@now", DateTime.Now.Ticks);
                //Execute command
                try
                {
                    cmd.ExecuteNonQuery();

                    query = PermanentSettings.GET_UID_MYSQL;
                    cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@email", email.ToLower());
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    if (dataReader.Read())
                    {
                        int uid = (int)dataReader["uid"];
                        user.UID = uid; //Save the UID for the user.
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                //close connection
                finally
                {
                    this.CloseConnection();
                }
            }
            else
            {
                MessageBox.Show("Error Connecting to Server", "Connection Error");
                return false;
            }

            return false;
        }

        public bool CheckConnection()
        {
            return OpenConnection();
        }


        /// <summary>
        /// Gets the user from the Remote DB
        /// </summary>
        /// <param name="userEmail">The user email.</param>
        /// <returns></returns>
        public User GetUser(string userEmail)
        {
            string query = PermanentSettings.GET_USER_MYSQL;

            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@email",userEmail.ToLower());
                MySqlDataReader dataReader =  null;
                //Create a data reader and Execute the command
                try
                {
                    dataReader = cmd.ExecuteReader();
                    User returnUser = null;

                    //Read the data and store them in the list
                    if (dataReader.Read())
                    {

                        String name = (string)dataReader["name"];
                        String email = (string)dataReader["email"];
                        String hashedPassword = (string)dataReader["password"];
                        String salt = (string)dataReader["salt"];
                        int uid = (int)dataReader["uid"];
                        //Read in last changed. 
                        returnUser = new User(name, email, hashedPassword, salt, uid);
                    }
                    
                    //return list to be displayed
                    return returnUser;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Exception " + e.Message, "Login Error");
                    return null;
                }
                finally
                {
                    //close Data Reader
                    if(dataReader != null)
                        dataReader.Close();
                    //close Connection
                    this.CloseConnection();
                }
            }
            else
            {
                MessageBox.Show("Error Connecting to Server", "Connection Error");
                return null;
            }

        }

        public String Server
        {
            get{return server;}
        }


    }
}
