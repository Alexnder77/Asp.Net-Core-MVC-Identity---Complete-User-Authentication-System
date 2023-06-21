using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;


namespace NotBlocket2.Models {
    public class ProfileMethods {

        public ProfileMethods() { }

        public Profile GetProfileById(int id, out string errormsg) {
            SqlConnection dbConnection = new SqlConnection();
            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=NotBlocket;Integrated Security=True";
            String sqlstring = "SELECT * FROM [NotBlocket].[dbo].[Profiles] WHERE Id = @Id";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);
            dbCommand.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            SqlDataAdapter myAdapter = new SqlDataAdapter(dbCommand);
            DataSet myDS = new DataSet();
            try {
                dbConnection.Open();
                myAdapter.Fill(myDS, "Profile");
                int count = myDS.Tables["Profile"].Rows.Count;
                if (count > 0) {
                    Profile profile = new Profile();
                    profile.Name = myDS.Tables["Profile"].Rows[0]["Name"].ToString();
                    profile.Email = myDS.Tables["Profile"].Rows[0]["Email"].ToString();
                    profile.Password = myDS.Tables["Profile"].Rows[0]["Password"].ToString();
                    profile.Location_Id = Convert.ToInt16(myDS.Tables["Profile"].Rows[0]["Location_Id"]);

                    profile.Id = Convert.ToInt16(myDS.Tables["Profile"].Rows[0]["Id"]);
                    errormsg = "";
                    return profile;
                }
                else {
                    errormsg = "No profile was found with the given ID.";
                    return null;
                }
            }
            catch (Exception e) {
                errormsg = e.Message;
                return null;
            }
            finally {
                dbConnection.Close();
            }
        }


        public int UpdateProfile(Profile pd, out string errormsg) {
            SqlConnection dbConnection = new SqlConnection();
            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=NotBlocket;Integrated Security=True;Pooling=False";
            String sqlstring = "UPDATE [NotBlocket].[dbo].[Profiles] SET Name = @Name, Email = @Email, Password = @Password, Location_Id = @Location_Id WHERE Id = @Id";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);
            dbCommand.Parameters.Add("@Id", SqlDbType.Int).Value = pd.Id;
            dbCommand.Parameters.Add("@Location_Id", SqlDbType.Int).Value = pd.Location_Id;
            dbCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 30).Value = pd.Name;
            dbCommand.Parameters.Add("@Email", SqlDbType.NVarChar, 50).Value = pd.Email;
            dbCommand.Parameters.Add("@Password", SqlDbType.NVarChar, 50).Value = pd.Password;
            try {
                dbConnection.Open();
                int i = 0;
                i = dbCommand.ExecuteNonQuery();
                if (i == 1) { errormsg = ""; }
                else { errormsg = "The profile was not updated in the database."; }
                return (i);
            }
            catch (Exception e) {
                errormsg = e.Message;
                return 0;
            }
            finally { dbConnection.Close(); }
        }


        public int DeleteProfile(int id, out string errormsg) {
            SqlConnection dbConnection = new SqlConnection();
            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=NotBlocket;Integrated Security=True;Pooling=False";
            String sqlstring = "DELETE FROM [NotBlocket].[dbo].[Profiles] WHERE Id = @Id";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);
            dbCommand.Parameters.AddWithValue("@Id", id);
            try {
                dbConnection.Open();
                int i = 0;
                i = dbCommand.ExecuteNonQuery();
                if (i == 1) { errormsg = ""; }
                else { errormsg = "The profile was not deleted from the database."; }
                return (i);
            }
            catch (Exception e) {
                errormsg = e.Message;
                return 0;
            }
            finally { dbConnection.Close(); }
        }


        public int InsertProfile(Profile pd, out string errormsg) {
            //Skapa Sql connection
            SqlConnection dbConnection = new SqlConnection();

            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=NotBlocket;Integrated Security=True;Pooling=False";
            String sqlstring = "INSERT INTO [NotBlocket].[dbo].[Profiles] (Name, Email, Password, Location_Id) VALUES (@Name, @Email, @Password, @Location_Id)";
            // For now no way of adding location, 

            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            dbCommand.Parameters.Add("Name", SqlDbType.NVarChar, 30).Value = pd.Name;
            dbCommand.Parameters.Add("Email", SqlDbType.NVarChar, 50).Value = pd.Email;
            dbCommand.Parameters.Add("Password", SqlDbType.NVarChar, 50).Value = pd.Password;
            dbCommand.Parameters.Add("Location_Id", SqlDbType.Int).Value = pd.Location_Id;

            try {
                dbConnection.Open();
                int i = 0;
                i = dbCommand.ExecuteNonQuery();
                if (i == 1) { errormsg = ""; }
                else { errormsg = "Det skapas inte en person i databasen."; }
                return (i);
            }

            catch (Exception e) {
                errormsg = e.Message;
                return 0;
            }
            finally { dbConnection.Close(); }
        }


        public List<Profile> GetPersonWithDataSet(out string errormsg) {

            //Skapa Sql connection
            SqlConnection dbConnection = new SqlConnection();
            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=NotBlocket;Integrated Security=True";

            String sqlstring = "SELECT * FROM [NotBlocket].[dbo].[Profiles]";

            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);
            SqlDataAdapter myAdapter = new SqlDataAdapter(dbCommand);
            DataSet myDS = new DataSet();
            List<Profile> PersonList = new List<Profile>();

            try {
                dbConnection.Open();

                myAdapter.Fill(myDS, "myPerson");

                int count = 0;
                int i = 0;
                count = myDS.Tables["myPerson"].Rows.Count;


                if (count > 0) {
                    while (i < count) {
                        Profile pd = new Profile();

                        pd.Name = myDS.Tables["myPerson"].Rows[i]["Name"].ToString();
                        pd.Email = myDS.Tables["myPerson"].Rows[i]["Email"].ToString();
                        pd.Password = myDS.Tables["myPerson"].Rows[i]["Password"].ToString();
                        pd.Location_Id = Convert.ToInt16(myDS.Tables["myPerson"].Rows[i]["Location_Id"]);
                        pd.Id = Convert.ToInt16(myDS.Tables["myPerson"].Rows[i]["Id"]);
                        i++;
                        PersonList.Add(pd);
                    }
                    errormsg = "";
                    return PersonList;
                }
                else { errormsg = "Det hämtas Ingen person"; }
                return PersonList;
            }

            catch (Exception e) {
                errormsg = e.Message;
                return null;
            }

            finally { dbConnection.Close(); }
        }


    }

}
