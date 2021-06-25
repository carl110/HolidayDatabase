using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayDatabase
{
    class DataAccess
    {
        private static OleDbConnection createConnection(string database = "Microsoft.ACE.OLEDB.12.0", string dataSource = "Travel.accdb")
        {
            OleDbConnectionStringBuilder connBuilder = new OleDbConnectionStringBuilder();
            connBuilder.Add("Provider", database);
            connBuilder.Add("Data Source", dataSource);
            OleDbConnection conn = new OleDbConnection(connBuilder.ConnectionString);
            return conn;
        }
        public static List<Holiday> getAllHolidays()
        {
            List<Holiday> holidayList = new List<Holiday>();
            try
            {
                OleDbConnection conn = createConnection();
                OleDbCommand cmd = conn.CreateCommand();
                conn.Open();
                //OleDb command
                OleDbCommand command = new OleDbCommand("select * from tblHoliday", conn);
                //Used to store the result from an OleDb statement
                OleDbDataReader dataReader = null;
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    Holiday tempHoliday = new Holiday();
                    tempHoliday.HolidayNo = Convert.ToInt32(dataReader[0]);
                    tempHoliday.Destination = dataReader[1].ToString();
                    tempHoliday.Cost = Convert.ToDouble(dataReader[2]);
                    tempHoliday.Departure = Convert.ToDateTime(dataReader[3].ToString());
                    tempHoliday.NoOfDays = Convert.ToInt32(dataReader[4]);
                    tempHoliday.Available = Convert.ToBoolean(dataReader[5].ToString());
                    holidayList.Add(tempHoliday);
                }
                dataReader.Close();
                command.Dispose();
                conn.Close();
            }
            catch (Exception e)
            {
                throw new Exception("Error***" + e.Message);
            }
            return holidayList;
        }
        public static void DeleteHoliday(int holidayNo)
        {
            try
            {
                OleDbConnection conn = createConnection();
                OleDbDataAdapter da = new OleDbDataAdapter();
                da.DeleteCommand = new OleDbCommand("DELETE FROM tblHoliday WHERE HolidayNo =" + holidayNo, conn);
                conn.Open();
                da.DeleteCommand.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static void UpdateHoliday(Holiday holToUpdate)
        {
            try
            {
                OleDbConnection conn = createConnection();
                conn.Open();
                string query = @"UPDATE tblHoliday SET Destination = @Dest, 
                                 Cost = @Cost, 
                                 DepartureDate = @DepDate, 
                                 NoOfDays = @NoDays, 
                                 Available = @Available 
                                 WHERE HolidayNo = @HolNo";
                OleDbCommand command = new OleDbCommand(query, conn);
                command.Parameters.AddWithValue("@Dest", holToUpdate.Destination);
                command.Parameters.AddWithValue("@Cost", holToUpdate.Cost);
                command.Parameters.AddWithValue("@DepDate", holToUpdate.Departure);
                command.Parameters.AddWithValue("@NoDays", holToUpdate.NoOfDays);
                command.Parameters.AddWithValue("@Available", holToUpdate.Available);
                command.Parameters.AddWithValue("@HolNo", holToUpdate.HolidayNo);
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR**** " + e.Message);
            }
        }
        public static void AddHoliday(string holidayNo, string destination, string cost, string departureDate, string noOfDays, bool available)
        {
            try
            {
                OleDbConnection conn = createConnection();
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand("INSERT INTO tblHoliday VALUES" +
                        "(@HolNo, @Dest, @Cost, @DepDate, @NoDays, @Available)", conn);
                    cmd.Parameters.Add(new OleDbParameter("@HolNo", OleDbType.Numeric) { Value = holidayNo });
                    cmd.Parameters.Add(new OleDbParameter("@Dest", OleDbType.VarChar, 20) { Value = destination });
                    cmd.Parameters.Add(new OleDbParameter("@Cost", OleDbType.Currency) { Value = cost });
                    cmd.Parameters.Add(new OleDbParameter("@DepDate", OleDbType.Date) { Value = departureDate });
                    cmd.Parameters.Add(new OleDbParameter("@NoDays", OleDbType.Numeric) { Value = noOfDays });
                    cmd.Parameters.Add(new OleDbParameter("@Available", OleDbType.Boolean) { Value = available });
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
