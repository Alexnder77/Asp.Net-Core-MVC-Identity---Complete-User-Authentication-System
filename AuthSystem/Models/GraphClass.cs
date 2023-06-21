using System.Data.SqlClient;
using System.Data;

namespace NotBlocket2.Models {

    public class Graphmethods {

        public Graphmethods() { }


        public void GetLocationsWithDataSet(out List<string> locationNames, out List<int> profileCounts, out string errormsg) {

            //Skapa Sql connection
            SqlConnection dbConnection = new SqlConnection();
            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=NotBlocket;Integrated Security=True";

            String sqlstring = "SELECT [NotBlocket].[dbo].[Locations].Name, COUNT([NotBlocket].[dbo].[Profiles].Id) AS Profile_Count\r\nFROM [NotBlocket].[dbo].[Locations]\r\nJOIN [NotBlocket].[dbo].[Profiles] ON [NotBlocket].[dbo].[Locations].Location_Id = [NotBlocket].[dbo].[Profiles].Location_Id\r\nGROUP BY [NotBlocket].[dbo].[Locations].Name";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);
            SqlDataAdapter myAdapter = new SqlDataAdapter(dbCommand);
            DataSet myDS = new DataSet();
            locationNames = new List<string>();
            profileCounts = new List<int>();

            try {
                dbConnection.Open();

                myAdapter.Fill(myDS, "myPerson");

                int count = 0;
                int i = 0;
                count = myDS.Tables["myPerson"].Rows.Count;


                if (count > 0) {
                    while (i < count) {
                        locationNames.Add(myDS.Tables["myPerson"].Rows[i]["Name"].ToString());
                        profileCounts.Add(Convert.ToInt32(myDS.Tables["myPerson"].Rows[i]["Profile_Count"]));
                        i++;
                    }
                    errormsg = "";
                    return;
                }
                else { errormsg = "Det hämtas Ingen Location"; }
                return;
            }

            catch (Exception e) {
                errormsg = e.Message;
                return;
            }

            finally { dbConnection.Close(); }
        }
    }


}
    


