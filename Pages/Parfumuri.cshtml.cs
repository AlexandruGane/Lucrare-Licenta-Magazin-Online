using GaneShop.Pages.Admin.Parfumuri;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace GaneShop.Pages
{
    public class ParfumuriModel : PageModel
    {
        [BindProperty(SupportsGet =true)]
        public string Cauta { get; set; }

        public List<ParfumuriInfo> listParfumuri=new List<ParfumuriInfo>();

        public void OnGet()
        {

            try
            {
                string connectionString = "Data Source=.\\sqlexpress01;Initial Catalog=bestshop;Integrated Security=True";
           
                 using(SqlConnection connection = new SqlConnection(connectionString)) 
                {
                    
                    connection.Open();

                    string sql = "SELECT * FROM Parfumuri WHERE (Title LIKE @search)";

                    using (SqlCommand command=new SqlCommand(sql, connection)) 
                    {
                        command.Parameters.AddWithValue("@search", "%" + Cauta + "%");
                    
                    using(SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {

                                ParfumuriInfo parfumuriInfo = new ParfumuriInfo();
                                parfumuriInfo.ID= reader.GetInt32(0);
                                parfumuriInfo.Title= reader.GetString(1);
                                parfumuriInfo.Cod_Parfum = reader.GetString(2);
                                parfumuriInfo.Pret=reader.GetDecimal(3);
                                parfumuriInfo.Descriere=reader.GetString(4);
                                parfumuriInfo.categorie=reader.GetString(5);
                                parfumuriInfo.imagine=reader.GetString(6);
                                parfumuriInfo.created_at=reader.GetDateTime(7).ToString("MM/dd/yyyyy");

                                listParfumuri.Add(parfumuriInfo);
                            }
                        }
                    }
                
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
        }
    }
}
