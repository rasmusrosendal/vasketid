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
    public class BookingManager
    {
        private static BookingManager mCurrent;
        private BookingManager() { }

        public static BookingManager Current
        {
            get
            {
                if (mCurrent == null)
                {
                    mCurrent = new BookingManager();
                }
                return mCurrent;
            }
        }

        #region public methods

        public bool CheckAvailability(DateTime starttime, DateTime endtime)
        {
            bool result = true;
            if (GetBookingsByDate(starttime).Where(x => x.Time >= starttime && x.Time < endtime).Any())
            {
                result = false;
            }
            return result;
        }

        public List<Booking> GetBookingsByDate(DateTime date)
        {
            DateTime newDate = new DateTime(date.Year, date.Month, date.Day);
            string queryString = @"SELECT Id, Time, UserId, Note FROM Booking WHERE Time > @lower AND Time < @upper AND Deleted = 0";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("lower", newDate);
            parameters.Add("upper", newDate.AddDays(1));

            return GetBooking(queryString, parameters);
        }

        public Booking GetBookingById(int id)
        {
            string queryString = @"SELECT Id, Time, UserId, Note FROM Booking WHERE Id=@id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("Id", id);

            return GetBooking(queryString, parameters).FirstOrDefault();
        }

        public Booking CreateBooking(DateTime starttime, DateTime endtime, int userid, string note)
        {
            Booking result;
            int createdId = -1;

            string queryString = "IF NOT EXISTS(SELECT Id FROM Booking WHERE Time >= @starttime AND Time < @endtime AND Deleted = 0) BEGIN ";
            int counter = 1;
            DateTime low = starttime;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("starttime", starttime);
            parameters.Add("endtime", endtime);
            parameters.Add("note", note);
            for (int i = starttime.Hour; i < endtime.Hour; i += 2)
            {
                parameters.Add("time" + counter, low);
                parameters.Add("user" + counter, userid);

                queryString += "INSERT INTO Booking (Time, UserId, Note) VALUES( @time" + counter + ", @user" + counter + ", @note) ";

                low = low.AddHours(2);
                counter++;
            }
            queryString += " END";
            try
            {
                //TODO, fix return value.
                createdId = 1;
                CreateDeleteBooking(queryString, parameters);
            }
            catch (Exception ex)
            {
                result = null;
            }

            result = GetBookingById(createdId);
            return result;

        }

        public bool DeleteBooking(DateTime starttime, DateTime endtime, int userid)
        {
            bool result = true;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("starttime", starttime);
            parameters.Add("endtime", endtime);
            parameters.Add("user", userid);

            string queryString = "UPDATE Booking SET Deleted = 1 WHERE Time >= @starttime AND Time < @endtime AND UserId = @user";

            try
            {
                CreateDeleteBooking(queryString, parameters);
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        #endregion

        #region private methods


        private List<Booking> GetBooking(string queryString, Dictionary<string, object> parameters)
        {
            List<Booking> result = new List<Booking>();
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
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Booking b = new Booking();
                        b.Id = (int)reader[0];
                        b.Time = (DateTime)reader[1];
                        b.UserId = (int)reader[2];
                        try
                        {
                            b.Note = (string)reader[3];
                        }
                        catch (Exception ex)
                        {
                            b.Note = "";
                        }
                        result.Add(b);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                }
            }
            return result;
        }

        private bool CreateDeleteBooking(string queryString, Dictionary<string, object> parameters)
        {
            queryString += " SELECT SCOPE_IDENTITY()";
            bool result = false;
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
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" ERROR ----------- BookingManager, CreateBooking()");
                }

                return result;
            }
        }

        #endregion
    }
}
