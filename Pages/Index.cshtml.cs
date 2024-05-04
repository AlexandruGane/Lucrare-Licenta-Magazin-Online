using GaneShop.Pages.Admin.Parfumuri;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace GaneShop.Pages
{
    public class IndexModel : PageModel
    {
        public List<ParfumuriInfo> listnoiParfumuri = new List<ParfumuriInfo>();
        public List<ParfumuriInfo> listGamaLux = new List<ParfumuriInfo>();

        public void OnGet()
        {


            try
            {
                string connectionString = "Data Source=.\\sqlexpress01;Initial Catalog=bestshop;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT TOP 4 * FROM Parfumuri ORDER BY created_at DESC";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                ParfumuriInfo parfumuriInfo = new ParfumuriInfo();
                                parfumuriInfo.ID = reader.GetInt32(0);
                                parfumuriInfo.Title = reader.GetString(1);
                                parfumuriInfo.Cod_Parfum = reader.GetString(2);
                                parfumuriInfo.Pret = reader.GetDecimal(3);
                                parfumuriInfo.Descriere = reader.GetString(4);
                                parfumuriInfo.categorie = reader.GetString(5);
                                parfumuriInfo.imagine = reader.GetString(6);
                                parfumuriInfo.created_at = reader.GetDateTime(7).ToString("MM/dd/YYYY");

                                listnoiParfumuri.Add(parfumuriInfo);
                            }
                        }
                    }

         sql = "SELECT TOP 4 Parfumuri.*, (" +
       "SELECT SUM(comenzi_items.cantitate) FROM comenzi_items WHERE Parfumuri.id=comenzi_items.parfumuri_id" +
       ") AS TOTAL_VANZARI " +
       "FROM Parfumuri " +
       "ORDER BY TOTAL_VANZARI DESC";




                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                ParfumuriInfo parfumuriInfo = new ParfumuriInfo();
                                parfumuriInfo.ID = reader.GetInt32(0);
                                parfumuriInfo.Title = reader.GetString(1);
                                parfumuriInfo.Cod_Parfum = reader.GetString(2);
                                parfumuriInfo.Pret = reader.GetDecimal(3);
                                parfumuriInfo.Descriere = reader.GetString(4);
                                parfumuriInfo.categorie = reader.GetString(5);
                                parfumuriInfo.imagine = reader.GetString(6);
                                parfumuriInfo.created_at = reader.GetDateTime(7).ToString("MM/dd/YYYY");

                                listGamaLux.Add(parfumuriInfo);

                            }

                        }
                    }
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);

            }
        }
    }
}