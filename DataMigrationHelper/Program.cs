using Lib.Entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Creating Admin...");

            string ConnectionString = "Server=tcp:vasketid.database.windows.net,1433;Data Source=vasketid.database.windows.net;Initial Catalog=VasketidDB;Persist Security Info=False;User ID=masterchief;Password=Kame4463;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            string queryString = "INSERT INTO Resident(Name, password, Guid, IsAdmin, Deleted) VALUES ('Admin', @password, @guid, 1, 0);";
            string password = "123qaz";
            Guid guid = Guid.NewGuid();

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("password", HashSHA1(password + guid));
            parameters.Add("guid", guid);

            using (SqlConnection connection = new SqlConnection(ConnectionString))
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
                    Console.WriteLine("Admin not created");
                    Console.ReadLine();
                    return;
                }
            }
            Console.WriteLine("Admin created!");
            Console.ReadLine();
            return;
        }

        private static string HashSHA1(string value)
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
    }
}
