using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GaneShop.Pages.Admin.Comenzi
{
    public class IndexModel : PageModel
    {
        public List<ComenziItemInfo> listComenziItems { get; set; }

        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=.\\sqlexpress01; Initial Catalog=bestshop; Integrated Security=True";

                listComenziItems = new List<ComenziItemInfo>(); // Inițializăm lista aici

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM comenzi_items ORDER BY id desc";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ComenziItemInfo comenziItemInfo = new ComenziItemInfo();
                                comenziItemInfo.Id = reader.GetInt32(0);
                                comenziItemInfo.ComenziId = reader.GetInt32(1);
                                comenziItemInfo.ParfumuriId = reader.GetInt32(2);
                                comenziItemInfo.Cantitate = reader.GetInt32(3);
                                comenziItemInfo.PretUnitate = reader.GetDecimal(4);

                                listComenziItems.Add(comenziItemInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Aici poți adăuga o gestionare a erorilor, dacă este necesar
            }
        }
    }

    public class ComenziItemInfo
    {
        public int Id { get; set; }
        public int ComenziId { get; set; }
        public int ParfumuriId { get; set; }
        public int Cantitate { get; set; }
        public decimal PretUnitate { get; set; }
    }
}
