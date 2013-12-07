using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar
{
    static class PermanentSettings
    {
        public static readonly string CREATE_EVENTS_TABLE_SQLITE = "create table events (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT," +
                                                                   "begintime INTEGER," +
                                                                   "endtime INTEGER," +
                                                                   "location TEXT," +
                                                                   "description TEXT," +
                                                                   "repeat TEXT," +
                                                                   "reminder TEXT," +
                                                                   "user INTEGER)";
        
        public static readonly string CREATE_USER_CACHE_TABLE_SQLITE = "create table user (id INTEGER PRIMARY KEY AUTOINCREMENT,"+ 
                                                                       "name TEXT," +
                                                                       "email TEXT UNIQUE ON CONFLICT ABORT," +
                                                                       "password TEXT," +
                                                                       "salt TEXT," +
                                                                       "timelastedit INTEGER)";

        public static readonly string GET_UID_MYSQL = "Select uid FROM users where email = @email limit 1";

        public static readonly string SAVE_USER_MYSQL = "INSERT INTO users (name, email, password, salt, timelastedit) VALUES(@name, @email, @hashedPass, @salt, @now)";

        public static readonly string GET_USER_MYSQL = "SELECT * FROM users where email = @email limit 1";

        public static readonly string SAVE_EVENT_MYSQL = "INSERT INTO events (name,begintime,endtime,location,description,user) VALUES (@name,@begintime,@endtime,@location,@description,@userid)";

        public static readonly string SAVE_EVENT_SQLITE = "INSERT INTO events (name,begintime,endtime,location,description,user) VALUES(@name,@begintime,@endtime,@location,@description,@userid)";

        public static readonly string GET_EVENTS_MYSQL = "SELECT * FROM events INNER JOIN users ON events.user = @userid";

        public static readonly string GET_EVENTS_SQLITE = "SELECT * FROM events where user = @uid";

        public static readonly string DELETE_EVENT_SQLITE = "DELETE FROM events where id = @key";

        public static readonly string CACHE_USER_SQLITE = "INSERT or REPLACE into user (name,email,password,salt,timelastedit) VALUES (@name,@email,@password,@salt,@currentTime)";

        public static readonly string FLUSH_CACHE = "DELETE FROM user";

        public static readonly string GET_CACHED_USER = "Select * from user where email = @email LIMIT 1";

        public static readonly string UPDATE_EVENT_SQLITE = "UPDATE events SET name=@name,begintime=@begintime,endtime=@endtime,location=@location,description=@description,user=@user where id=@id";
    }

}
