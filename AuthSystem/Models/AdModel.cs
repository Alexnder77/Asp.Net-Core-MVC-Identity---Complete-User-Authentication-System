using System.Data.SqlClient;
using System.Data;
using NotBlocket2.Models;
using Microsoft.Build.Framework;

using Microsoft.AspNetCore.Mvc;

namespace NotBlocket2.Models {
    public class Ad {

        public Ad() { }

        public int Id { get; set; }
        //Publika egenskaper
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }


        [Required]
        public int Price { get; set; }

        public int? Profile_Id { get; set; }
        public string? ImagePath { get; set; }
    }

    public class AdMethods {

        public AdMethods() { }
        
		private bool IsValidImageFormat(string fileName) {
			var extension = Path.GetExtension(fileName).ToLowerInvariant();
			return extension == ".jpg" || extension == ".jpeg" || extension == ".png";
		}

        public static async Task<string> SaveFileAsync(IFormFile file) {
            // Check if the file is null
            if (file == null || file.Length == 0) {
                return "File is empty or missing";
            }

            // Get the file stream and read its contents
            var stream = file.OpenReadStream();
            var fileBytes = new byte[file.Length];
            await stream.ReadAsync(fileBytes, 0, (int)file.Length);

            AdMethods am = new AdMethods();
            // Validate the file format
            if (!am.IsValidImageFormat(file.FileName)) {
                return "Invalid file format";
            }

            // Generate a unique file name
            var fileName = $"{DateTime.Now.ToString("yyyyMMddhhmmss")}_{new Random().Next(1000, 9999)}_{file.FileName}";

            // Save the file to the file system
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create)) {
                await fileStream.WriteAsync(fileBytes);
            }

            return Path.Combine("/images", fileName);
        }
        


        public Ad GetAdById(int id, out string errormsg) {
			SqlConnection dbConnection = new SqlConnection();
			dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=NotBlocket;Integrated Security=True";
			String sqlstring = "SELECT * FROM [NotBlocket].[dbo].[Ads] WHERE Id = @Id";
			SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);
			dbCommand.Parameters.Add("@Id", SqlDbType.Int).Value = id;
			SqlDataAdapter myAdapter = new SqlDataAdapter(dbCommand);
			DataSet myDS = new DataSet();

			try {
				dbConnection.Open();
				myAdapter.Fill(myDS, "Ad");
				int count = myDS.Tables["Ad"].Rows.Count;
				if (count > 0) {
					Ad profile = new Ad();

					profile.Name = myDS.Tables["Ad"].Rows[0]["Name"].ToString();
					
                    if (myDS.Tables["Ad"].Rows[0]["Description"] != DBNull.Value) {
                        profile.Description = myDS.Tables["Ad"].Rows[0]["Description"].ToString();
                    }

                    if (myDS.Tables["Ad"].Rows[0]["Category"] != DBNull.Value) {
                        profile.Category = myDS.Tables["Ad"].Rows[0]["Category"].ToString();
                    }
                    profile.Price = Convert.ToInt32(myDS.Tables["Ad"].Rows[0]["Price"]);
					
                    if (myDS.Tables["Ad"].Rows[0]["Profile_Id"] != DBNull.Value) {
                        profile.Profile_Id = Convert.ToInt32(myDS.Tables["Ad"].Rows[0]["Profile_Id"]);
                    }

                    if (myDS.Tables["Ad"].Rows[0]["ImagePath"] != DBNull.Value) {
                        profile.ImagePath = myDS.Tables["Ad"].Rows[0]["ImagePath"].ToString();
                    }

                    errormsg = "";
					return profile;
				}
				else {
					errormsg = "No ad was found with the given ID.";
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


		public int UpdateAd(Ad pd, out string errormsg) {
			SqlConnection dbConnection = new SqlConnection();
			dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=NotBlocket;Integrated Security=True;Pooling=False";
            //String sqlstring = "UPDATE [NotBlocket].[dbo].[Ads] SET Name = @Name WHERE Id = @Id";
            String sqlstring = "UPDATE [NotBlocket].[dbo].[Ads] SET Name = @Name, Description = @Description, Category = @Category, Price = @Price, Profile_Id = @Profile_Id  WHERE Id = @Id";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection); 

            
            dbCommand.Parameters.Add("@Id", SqlDbType.Int).Value = pd.Id;
			dbCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 30).Value = pd.Name;
			dbCommand.Parameters.Add("@Description", SqlDbType.NVarChar, 50).Value = pd.Description ?? (object)DBNull.Value; 
			dbCommand.Parameters.Add("@Category", SqlDbType.NVarChar, 30).Value = pd.Category ?? (object)DBNull.Value;
			dbCommand.Parameters.Add("@Price", SqlDbType.Int).Value = pd.Price;
			dbCommand.Parameters.Add("@Profile_Id", SqlDbType.Int).Value = pd.Profile_Id ?? (object)DBNull.Value;
			//dbCommand.Parameters.Add("@ImagePath", SqlDbType.NVarChar, 50).Value = pd.ImagePath ?? (object)DBNull.Value;
			

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

		public int DeleteAd(int id, out string errormsg) {
			SqlConnection dbConnection = new SqlConnection();
			dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=NotBlocket;Integrated Security=True;Pooling=False";
			String sqlstring = "DELETE FROM [NotBlocket].[dbo].[Ads] WHERE Id = @Id";
			SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);
			dbCommand.Parameters.AddWithValue("@Id", id);
			try {
				dbConnection.Open();
				int i = 0;
				i = dbCommand.ExecuteNonQuery();
				if (i == 1) { errormsg = ""; }
				else { errormsg = "The ad was not deleted from the database."; }
				return (i);
			}
			catch (Exception e) {
				errormsg = e.Message;
				return 0;
			}
			finally { dbConnection.Close(); }
		}

		public int InsertAd(Ad ad, out string errormsg) {
            //Create SQL Connection
            SqlConnection dbConnection = new SqlConnection();

            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=NotBlocket;Integrated Security=True;Pooling=False";
            String sqlstring = "INSERT INTO [NotBlocket].[dbo].[Ads] (Name, Description, Category, Price, Profile_Id, ImagePath) VALUES (@Name, @Description, @Category, @Price, @Profile_Id, @ImagePath)";

            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            dbCommand.Parameters.Add("Name", SqlDbType.NVarChar, 30).Value = ad.Name;
            dbCommand.Parameters.Add("Description", SqlDbType.NVarChar, 50).Value = ad.Description ?? (object)DBNull.Value;
            dbCommand.Parameters.Add("Category", SqlDbType.NVarChar, 30).Value = ad.Category ?? (object)DBNull.Value;
            dbCommand.Parameters.Add("Price", SqlDbType.Int).Value = ad.Price;

            dbCommand.Parameters.Add("ImagePath", SqlDbType.NVarChar, 50).Value = ad.ImagePath ?? (object)DBNull.Value;
            dbCommand.Parameters.Add("Profile_Id", SqlDbType.Int).Value = ad.Profile_Id ?? (object)DBNull.Value;



            try {
                dbConnection.Open();
                int i = 0;
                i = dbCommand.ExecuteNonQuery();
                if (i == 1) { errormsg = ""; }
                else { errormsg = "The ad was not inserted into the database."; }
                return (i);
            }

            catch (Exception e) {
                errormsg = e.Message;
                return 0;
            }
            finally { dbConnection.Close(); }
        }


        public List<Ad> GetAdsWithDataSet(out string errormsg, in string filterByCategorystring = null) {

            //Skapa Sql connection
            SqlConnection dbConnection = new SqlConnection();
            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=NotBlocket;Integrated Security=True";

            String sqlstring = "SELECT * FROM [NotBlocket].[dbo].[Ads]";

            if (filterByCategorystring != null) {
                sqlstring = sqlstring + "WHERE [NotBlocket].[dbo].[Ads].[Category] = '" + filterByCategorystring + "'";
            };

            errormsg = sqlstring;
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            SqlDataAdapter myAdapter = new SqlDataAdapter(dbCommand);
            DataSet myDS = new DataSet();
            List<Ad> AdList = new List<Ad>();

            try {
                dbConnection.Open();

                myAdapter.Fill(myDS, "myAd");

                int count = 0;
                int i = 0;
                count = myDS.Tables["myAd"].Rows.Count;


                if (count > 0) {
                    while (i < count) {
                        Ad pd = new Ad();

                        pd.Name = myDS.Tables["myAd"].Rows[i]["Name"].ToString();
                        pd.Description = myDS.Tables["myAd"].Rows[i]["Description"].ToString();
                        pd.ImagePath = myDS.Tables["myAd"].Rows[i]["ImagePath"].ToString();
                        pd.Category = myDS.Tables["myAd"].Rows[i]["Category"].ToString();
                        pd.Price = Convert.ToInt32(myDS.Tables["myAd"].Rows[i]["Price"]);
                        pd.Id = Convert.ToInt32(myDS.Tables["myAd"].Rows[i]["Id"]);
                        i++;
                        AdList.Add(pd);
                    }
                    errormsg = "";
                    return AdList;
                }
                else { errormsg = errormsg + "Det hämtas Ingen ad"; }
                return AdList;
            }

            catch (Exception e) {
                errormsg = e.Message;
                return null;
            }

            finally { dbConnection.Close(); }

        }

        public List<Ad> GetAdsWithDataSet2(string sortString, string searchstring, out string errormsg) {

            //Skapa Sql connection
            SqlConnection dbConnection = new SqlConnection();
            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=NotBlocket;Integrated Security=True";


            //add ASC vs DESC?
        
            String sqlstring = @"
                                SELECT *
                                FROM [NotBlocket].[dbo].[Ads]
                                WHERE [NotBlocket].[dbo].[Ads].[Name] LIKE '%'+'" + searchstring + @"'+'%'
                                ORDER BY 
                                    [NotBlocket].[dbo].[Ads].[" + sortString + "] ASC";



            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            SqlDataAdapter myAdapter = new SqlDataAdapter(dbCommand);
            DataSet myDS = new DataSet();
            List<Ad> AdList = new List<Ad>();

            try {
                dbConnection.Open();

                myAdapter.Fill(myDS, "myAd");

                int count = 0;
                int i = 0;
                count = myDS.Tables["myAd"].Rows.Count;


                if (count > 0) {
                    while (i < count) {
                        Ad pd = new Ad();

                        pd.Name = myDS.Tables["myAd"].Rows[i]["Name"].ToString();
                        pd.Description = myDS.Tables["myAd"].Rows[i]["Description"].ToString();
                        pd.Category = myDS.Tables["myAd"].Rows[i]["Category"].ToString();
                        pd.ImagePath = myDS.Tables["myAd"].Rows[i]["ImagePath"].ToString();
                        pd.Price = Convert.ToInt32(myDS.Tables["myAd"].Rows[i]["Price"]);
                        pd.Id = Convert.ToInt32(myDS.Tables["myAd"].Rows[i]["Id"]);
                        i++;
                        AdList.Add(pd);
                    }
                    errormsg = "";
                    return AdList;
                }
                else { errormsg = "There are no results for your search"; }
                return AdList;
            }

            catch (Exception e) {
                errormsg = e.Message;
                return null;
            }

            finally { dbConnection.Close(); }





        }
    }
}


