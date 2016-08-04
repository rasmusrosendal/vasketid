using Lib.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Managers
{
    public class UserManager
    {
        private static UserManager mCurrent;
        private UserManager() { }

        public static UserManager Current
        {
            get
            {
                if (mCurrent == null)
                {
                    mCurrent = new UserManager();
                }
                return mCurrent;
            }
        }

        #region public methods
        public User ValidateLogin(string userName, string password)
        {
            string queryString = @"SELECT * FROM [User] WHERE Name=@name AND Deleted=@deleted";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("name", userName);
            parameters.Add("deleted", false);

            User currentUser = GetUser(queryString, parameters);
            if (currentUser != null)
            {
                string hashedEnteredPassword = HashSHA1(password + currentUser.Guid);
                if (hashedEnteredPassword.Equals(currentUser.Password))
                {
                    return currentUser;
                }
            }
            return null;
        }

        public bool IsUsernameTaken(string username)
        {
            string querystring = "SELECT TOP 1 * FROM [User] WHERE Name = @name AND Deleted=@deleted";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("name", username);
            parameters.Add("deleted", false);

            User user = GetUser(querystring, parameters);
            return user != null;
        }

        public User GetUserById(int id)
        {
            string queryString = @"SELECT * FROM [User] WHERE Id=@id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("id", id);

            return GetUser(queryString, parameters);
        }

        public User GetUserByName(string name)
        {
            string queryString = @"SELECT * FROM [User] WHERE Name=@name";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("name", name);

            return GetUser(queryString, parameters);
        }

        public List<User> GetAllUsers()
        {
            string queryString = @"SELECT * FROM [User] WHERE Deleted = @deleted";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("deleted", 0);

            return GetUsers(queryString, parameters);
        }

        public User CreateUser(string name, string password, bool isAdmin)
        {
            User result;
            Guid guid = Guid.NewGuid();
            string hashedPassword = HashSHA1(password + guid);
            string queryString = "INSERT INTO [User] (Name, Password, Guid, IsAdmin) VALUES (@name, @password, @guid, @isAdmin)";

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("name", name);
            parameters.Add("password", hashedPassword);
            parameters.Add("guid", guid);
            parameters.Add("isAdmin", isAdmin ? 1 : 0);

            try
            {
                CreateUpdateDeleteUser(queryString, parameters);
            }
            catch (Exception)
            {
                result = null;
            }

            result = GetUserByName(name);
            return result;
        }

        public bool UpdateUser(User user)
        {
            bool result = true;
            string queryString = "UPDATE [User] SET Name = @name, IsAdmin = @isAdmin, Deleted = @deleted WHERE Id = @id";

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("id", user.Id);
            parameters.Add("name", user.Name);
            parameters.Add("isAdmin", user.IsAdmin ? 1 : 0);
            parameters.Add("deleted", user.Deleted);

            try
            {
                CreateUpdateDeleteUser(queryString, parameters);
            }
            catch (Exception e)
            {
                result = false;
            }

            return result;
        }
        #endregion

        #region private methods
        private User GetUser(string queryString, Dictionary<string, object> parameters)
        {
            User result = null;
            string qString = queryString;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["VasketidDB"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                foreach (KeyValuePair<string, object> pair in parameters)
                {
                    command.Parameters.AddWithValue(pair.Key, pair.Value);
                }

                try
                {
                    User user = null;
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        user = new User();
                        user.Id = (int)reader[0];
                        user.Name = (string)reader[1];
                        user.Password = (string)reader[2];
                        user.Guid = (Guid)reader[3];
                        user.IsAdmin = (bool)reader[4];
                        var temp = reader[5];
                        user.Deleted = (bool)reader[5];
                    }
                    result = user;
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" ERROR ----------- UserManager, GetUser()");
                    throw ex;
                }
            }
            return result;
        }

        private List<User> GetUsers(string queryString, Dictionary<string, object> parameters)
        {
            List<User> result = new List<User>();
            string qString = queryString;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["VasketidDB"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                foreach (KeyValuePair<string, object> pair in parameters)
                {
                    command.Parameters.AddWithValue(pair.Key, pair.Value);
                }

                try
                {
                    User user = null;
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        user = new User();
                        user.Id = (int)reader[0];
                        user.Name = (string)reader[1];
                        user.Password = (string)reader[2];
                        user.Guid = (Guid)reader[3];
                        user.IsAdmin = (bool)reader[4];
                        user.Deleted = (bool)reader[5];
                        result.Add(user);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" ERROR ----------- UserManager, GetUsers()");
                    throw ex;
                }
            }
            return result;
        }

        private void CreateUpdateDeleteUser(string queryString, Dictionary<string, object> parameters)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["VasketidDB"].ConnectionString))
            {

                SqlTransaction transaction;
                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    SqlCommand command = new SqlCommand(queryString, connection, transaction);
                    foreach (KeyValuePair<string, object> pair in parameters)
                    {
                        command.Parameters.AddWithValue(pair.Key, pair.Value);
                    }

                    try
                    {
                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" ERROR ----------- UserManager, CreateDeleteUser()");
                    throw ex;
                }
            }
        }

        private string HashSHA1(string value)
        {
            var sha1 = System.Security.Cryptography.SHA1.Create();
            var inputBytes = Encoding.ASCII.GetBytes(value);
            var hash = sha1.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            for (var i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
        #endregion
    }
}
