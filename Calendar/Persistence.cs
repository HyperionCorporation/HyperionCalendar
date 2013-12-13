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
        public Dictionary<long,Event> GetEvents(User user, DateTime date)
        {
            List<Event> BuildAList = sqlitePersitance.getEvents(user.UID);
            Dictionary<long, Event> eventDict = new Dictionary<long, Event>();
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
        private void DeleteEvent(Event deleteEvent)
        {
            //Don't delete from Local DB here. 
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

        public bool DoSync(int uid)
        {
            List<Event> eventList = sqlitePersitance.getEvents(uid);
            return mysqlPersitance.DoSync(eventList, uid);
        }

    }


    /// <summary>
    /// SQLite Persistence Object
    /// </summary>
    class SQLitePersistance
    {
        public static readonly string localDatabase = "calendar.sqlite";
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
            long key = myEvent.Key; //Key is generated with original event creation.

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
                cmd.Parameters.Add(new SQLiteParameter("@timelastedit",DateTime.Now.Ticks));
                cmd.Parameters.Add(new SQLiteParameter("@uid", key));
                cmd.Parameters.Add(new SQLiteParameter("@delete",myEvent.DeleteEvent));
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                localDBConnection.Close();
            }
        }

        /// <summary>
        /// Deletes an Event from the Local DB.
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
                MessageBox.Show("Error Deleting Event " + e.Message, "Delete Error");
            }
            finally
            {
                localDBConnection.Close();
                }
        }

        /// <summary>
        /// Gets the event for a User from the SQLite DB
        /// </summary>
        /// <param name="user">The user id.</param>
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
                    long timeLastModified = (long)reader["timelastedit"];
                    long key = (long)reader["uid"];
                    bool deleteEvent = Convert.ToBoolean(reader["deleteEvent"]);
                    if(!deleteEvent)
                        eventList.Add(new Event(name, new DateTime(begintime), new DateTime(endtime), location, description, key, new DateTime(timeLastModified), deleteEvent));
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
                cmd.Parameters.AddWithValue("@timelastedit", myEvent.LastModified.Ticks);
                cmd.Parameters.AddWithValue("@delete", myEvent.DeleteEvent);
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
                cmd.Parameters.Add(new SQLiteParameter("@id",user.UID));
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
            if (connection.State != System.Data.ConnectionState.Closed ||
                connection.State != System.Data.ConnectionState.Broken ||
                connection.State != System.Data.ConnectionState.Executing ||
                connection.State != System.Data.ConnectionState.Fetching)
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch (MySqlException e)
                {
                    System.Diagnostics.Debug.WriteLine("Error Opening Connection " + e.Message);
                    return false;
                }
                catch (Exception e)
                {
                    if (e.Message == "The connection is already open.")
                        return true;
                    return false;
                }
            }

            return false;
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
            long key = myEvent.Key; //Get the key and save it to db.
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
                cmd.Parameters.AddWithValue("@timelastedit", DateTime.Now.Ticks);
                cmd.Parameters.AddWithValue("@uid", key);
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


        /// <summary>
        /// Does the synchronize between MySQL and SQLite
        /// </summary>
        /// <param name="eventList">The event list.</param>
        public bool DoSync(List<Event> eventList, int uid)
        {
            Console.WriteLine("Syncing The Database");
            string querySync = PermanentSettings.SYNC_REMOTE_WITH_LOCAL;
            string queryInsert = PermanentSettings.SAVE_EVENT_MYSQL;
            string queryGetRemoteLastModifiedTime = PermanentSettings.GET_LAST_MODIFIED_TIME_MYSQL;
            string getEventQuery = PermanentSettings.GET_EVENT_MYSQL;
            List<Tuple<bool, DateTime>> lastModifiedTimes = new List<Tuple<bool, DateTime>>(); //Store if it exists in rdb or not as well as the last updated Time.


            foreach (Event myEvent in eventList)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(queryGetRemoteLastModifiedTime, connection);
                cmd.Parameters.AddWithValue("@uid", myEvent.Key);
                if (this.OpenConnection() == true)
                {
                    //Execute command
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MySqlDataReader dataReader = cmd.ExecuteReader();

                        if (dataReader.Read())
                        {
                            long lastModifeidRaw = Convert.ToInt64(dataReader["timelastedit"]);
                            lastModifiedTimes.Add(Tuple.Create(true, new DateTime(lastModifeidRaw))); //A result was returned, it exists. 
                        }

                        else
                        {
                            lastModifiedTimes.Add(Tuple.Create(false, new DateTime(0)));
                        }
                    }

                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Error Persistence.GetLastEditedTime " + e.Message);
                        return false;
                    }

                    finally
                    {
                        this.CloseConnection();
                    }
                }

                else
                {
                    System.Diagnostics.Debug.WriteLine("Error Connecting to DB", "Connection Error");
                }
            }



            int index = 0;
            foreach (Event myEvent in eventList)
            {
                //Update the remote if it is behind the local.
                if (this.OpenConnection() == true)
                {
                    if (lastModifiedTimes[index].Item1 && myEvent.LastModified > lastModifiedTimes[index].Item2)
                    {
                        //Event Exists in DB and is older than the local DB
                        index++;
                        MySqlCommand cmd = new MySqlCommand(querySync, connection);
                        cmd.Parameters.AddWithValue("@name", myEvent.name);
                        cmd.Parameters.AddWithValue("@begintime", myEvent.begin.Ticks);
                        cmd.Parameters.AddWithValue("@endtime", myEvent.end.Ticks);
                        cmd.Parameters.AddWithValue("@location", myEvent.location);
                        cmd.Parameters.AddWithValue("@description", myEvent.description);
                        cmd.Parameters.AddWithValue("@timelastedit", myEvent.LastModified.Ticks);
                        cmd.Parameters.AddWithValue("@uid", myEvent.Key);

                        //Execute command
                        try
                        {
                            cmd.ExecuteNonQuery();
                        }

                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine("Error Persistence.DoSync " + e.Message);
                            return false;
                        }

                        finally
                        {
                            this.CloseConnection();
                        }
                    }
                    else if (!lastModifiedTimes[index].Item1)
                    {
                        //Event doesn't exist at remote DB
                        MySqlCommand cmd = new MySqlCommand(queryInsert, connection);
                        cmd.Parameters.AddWithValue("@name", myEvent.name);
                        cmd.Parameters.AddWithValue("@begintime", myEvent.begin.Ticks);
                        cmd.Parameters.AddWithValue("@endtime", myEvent.end.Ticks);
                        cmd.Parameters.AddWithValue("@location", myEvent.location);
                        cmd.Parameters.AddWithValue("@description", myEvent.description);
                        cmd.Parameters.AddWithValue("@userid", uid);
                        cmd.Parameters.AddWithValue("@timelastedit", myEvent.LastModified.Ticks);
                        cmd.Parameters.AddWithValue("@uid", myEvent.Key);
                        index++;
                        //Execute command
                        try
                        {
                            cmd.ExecuteNonQuery();
                        }

                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine("Error Persistence.DoSync " + e.Message);
                            return false;
                        }
                        //close connection
                        finally
                        {
                            this.CloseConnection();
                        }

                    }
                }

                else
                {
                    return false;
                }
            }

            GetEventsInRemote(eventList,uid);
            Console.WriteLine("Done Syncing The Database");

            return true;
        }

        private void GetEventsInRemote(List<Event> eventList,int uid)
        {
               
            List<Event> returnedEvents = new List<Event>();
            //GET_ALL_EVENTS_MYSQL
            //Get all the events in the Remote DB
            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(PermanentSettings.GET_ALL_EVENTS_MYSQL, connection);
                MySqlDataReader dataReader = null;
                //Create a data reader and Execute the command
                try
                {
                    dataReader = cmd.ExecuteReader();
                    //Read the data and store them in the list
                    if (dataReader.Read())
                    {
                        string name = Convert.ToString(dataReader["name"]);
                        long beginTime = Convert.ToInt64(dataReader["begintime"]);
                        long endtime = Convert.ToInt64(dataReader["endtime"]);
                        string location = Convert.ToString(dataReader["location"]);
                        string description = Convert.ToString(dataReader["description"]);
                        int userID = Convert.ToInt32(dataReader["user"]);
                        long timelastedit = Convert.ToInt64(dataReader["timelastedit"]);
                        long uniqueID = Convert.ToInt64(dataReader["uid"]);

                        returnedEvents.Add(new Event(name, new DateTime(beginTime), new DateTime(endtime), location, description, uniqueID, new DateTime(timelastedit), false));
                    }

            
                }
                catch (Exception e)
                {
                    MessageBox.Show("Exception " + e.Message, "MySQL Error");
                }
                finally
                {
                    //close Data Reader
                    if (dataReader != null)
                        dataReader.Close();
                    //close Connection
                    this.CloseConnection();
                }
            }

            //Remove any Events that are are on both the remote and local. 

            for (int i = 0; i < returnedEvents.Count; i++)
            {
                foreach(Event myEvent  in eventList)
                {
                    if (returnedEvents[i].Key == myEvent.Key)
                        returnedEvents.RemoveAt(i);
                }
            }

            SQLiteConnection localDBConnection = new SQLiteConnection("Data Source=" + SQLitePersistance.localDatabase + ";Version=3;");

            //Save the events that remain in the local DB
            foreach (Event myEvent in returnedEvents)
            {
                try
                {
                    localDBConnection.Open();
                   
                    SQLiteCommand cmdLocal = localDBConnection.CreateCommand();
                    cmdLocal.CommandType = System.Data.CommandType.Text;
                    cmdLocal.CommandText = PermanentSettings.SAVE_EVENT_SQLITE;
                    cmdLocal.Parameters.AddWithValue("@name", myEvent.name);
                    cmdLocal.Parameters.AddWithValue("@begintime", myEvent.begin.Ticks);
                    cmdLocal.Parameters.AddWithValue("@endtime", myEvent.end.Ticks);
                    cmdLocal.Parameters.AddWithValue("@location", myEvent.location);
                    cmdLocal.Parameters.AddWithValue("@description", myEvent.description);
                    cmdLocal.Parameters.AddWithValue("@uid", myEvent.Key);
                    cmdLocal.Parameters.AddWithValue("@userid", uid);
                    cmdLocal.Parameters.AddWithValue("@timelastedit", myEvent.LastModified.Ticks);
                    cmdLocal.Parameters.AddWithValue("@delete", false);
                    int affected = cmdLocal.ExecuteNonQuery();
                    Console.Write("");
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error Pulling Events: " + e.Message);
                }
                finally
                {
                    localDBConnection.Close();
                }
            }
        }
    }
}
