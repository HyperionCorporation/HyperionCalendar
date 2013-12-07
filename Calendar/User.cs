using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Globalization;


namespace Calendar
{

    /**
     * Defines a user object.
     * Never store a raw password in the user. Always call the hashPassword method first,
     * then pass in the password.
     * */
    public class User
    {
        private string name;
        private string email;
        private string hashedPasswd; //DO NOT STORE PLAIN TEXT PASSWORD!
        private string salt;
        private int? uid;

        public User()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="name">The name of the user.</param>
        /// <param name="email">The email of the user.</param>
        /// <param name="hashedPasswd">The hashed password.</param>
        /// <param name="salt">The salt associated with the user</param>
        public User(string name, string email, string hashedPasswd, string salt = null, int? uid = null)
        {
            this.name = name;
            this.email = email.ToLower();
            this.hashedPasswd = hashedPasswd;
            this.salt = salt;
            this.uid = uid;
         }


        /// <summary>
        /// Hashes the password.
        /// </summary>
        /// <param name="rawPassword">The raw password.</param>
        /// <returns>A dictionary with the hashedPassword and salt</returns>
        public static Dictionary<String,String> hashPassword(string rawPassword, string salt = null)
        {
            Random rand = new Random();
            UnicodeEncoding encoding = new UnicodeEncoding();
            HashAlgorithm algo = HashAlgorithm.Create("SHA512");
            int saltValueSize = rand.Next(100);
            
            if(salt == null)//It's a new user
                salt = User.getHash(saltValueSize,rand);

            if (rawPassword != null && salt != null && algo != null && encoding != null)
            {
                
                byte[] prehashed = System.Text.Encoding.Unicode.GetBytes(rawPassword + salt);
                byte[] hashed = algo.ComputeHash(prehashed);
                
                string hashedPassword = string.Empty;
                foreach (byte hexdigit in hashed)
                {
                    hashedPassword += hexdigit.ToString("X2", CultureInfo.InvariantCulture.NumberFormat);
                }

                return new Dictionary<string, string>{
                    {"hashedpassword", hashedPassword},
                    {"salt",salt}
                };
            }

            return null;
        }

        /// <summary>
        /// Generates a salt
        /// </summary>
        /// <param name="saltValueSize">Size of the salt value.</param>
        /// <returns></returns>
        private static string getHash(int saltValueSize, Random rand)
        {
            const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            char[] buffer = new char[saltValueSize];
            for (int i = 0; i < saltValueSize; i++)
            {
                buffer[i] = _chars[rand.Next(_chars.Length)];
            }
            return new string(buffer);
        }

        public string Name
        {
            get { return name; }
        }

        public string Email
        {
            get { return email.ToLower(); }
        }

        public string HashedPassword
        {
            get { return hashedPasswd; }
        }

        public string Salt
        {
            get { return salt; }
            set { salt = value; }
        }

        public int? UID
        {
            get { return uid; }
            set { uid = value; }
        }

        public static bool operator == (User left, User right)
        {
            if(Object.Equals(left,null))
                if(Object.Equals(right,null))
                    return true;
                else
                    return false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(User left, User right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
 	        if(obj == null)
                return false;
            User user = (User)obj;
            if(this.name == user.name &&
               this.hashedPasswd == user.hashedPasswd &&
               this.salt == user.salt &&
               this.email == user.email)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
 	        string hashString = this.name + this.hashedPasswd + this.salt + this.email;
            return hashString.GetHashCode();
        }

    }
}
