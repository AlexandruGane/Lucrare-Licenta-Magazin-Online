using GaneShop.Pages.Admin.Parfumuri;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace GaneShop.Pages
{
    public class DetaliiParfumuriModel : PageModel
    {
        public ParfumuriInfo ParfumuriInfo=new ParfumuriInfo();
        public void OnGet(int? id)
        {
            if (id == null)
            {
                Response.Redirect("/");
                return;

            }

            try 
            {

                string connectionString = "Data Source=.\\sqlexpress01;Initial Catalog=bestshop;Integrated Security=True";
                using(SqlConnection connection = new SqlConnection(connectionString)) 
                {
                
                 connection.Open();
                    string sql = "SELECT * FROM Parfumuri WHERE id=@id";
                    using(SqlCommand command = new SqlCommand(sql, connection)) 
                    {
                    
                        command.Parameters.AddWithValue("@id", id);

                        using(SqlDataReader reader = command.ExecuteReader()) 
                        {

                            if (reader.Read()) 
                            {
                                ParfumuriInfo.ID = reader.GetInt32(0);
                                ParfumuriInfo.Title = reader.GetString(1);
                                ParfumuriInfo.Cod_Parfum = reader.GetString(2);
                                ParfumuriInfo.Pret = reader.GetDecimal(3);
                                ParfumuriInfo.Descriere = reader.GetString(4);
                                ParfumuriInfo.categorie = reader.GetString(5);
                                ParfumuriInfo.imagine = reader.GetString(6);
                                ParfumuriInfo.created_at = reader.GetDateTime(7).ToString("MM/dd/YYYY");

                            }
                            else
                            {
                                Response.Redirect("/");
                                return;
                            }
                        
                        
                        
                        }
                    }
                }
            }

            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
                Response.Redirect("/");
                return;


            }
        }
    }
}
