using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using sib_api_v3_sdk.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GaneShop.Pages.Admin.Parfumuri
{
    public class IndexModel : PageModel
    {
        public List<ParfumuriInfo> listParfumuri = new List<ParfumuriInfo>();
        public string search = "";
        public int page = 1;
        public int totalPages = 0;
        private readonly int pageSize = 3; 

        public void OnGet()
        {
            search = Request.Query["search"];
            if (search == null) search = "";

            page = 1;
            string requestPage = Request.Query["page"];
            if (requestPage != null)
            {
                try
                {
                    page = int.Parse(requestPage);
                }
                catch (Exception ex)
                {
                    page = 1;
                }
            }

            try
            {
                string connectionString = "Data Source=.\\sqlexpress01;Initial Catalog=bestshop;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlCount = "SELECT COUNT(*) FROM Parfumuri";
                    if (search.Length > 0)
                    {
                        sqlCount += " WHERE Title LIKE @search OR categorie LIKE @search";
                    }

                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        command.Parameters.AddWithValue("@search", "%" + search + "%");
                        decimal count = (int)command.ExecuteScalar();
                        totalPages = (int)Math.Ceiling(count / pageSize);
                    }

                    string sql = "SELECT * FROM Parfumuri";
                    if (search.Length > 0)
                    {
                        sql += " WHERE Title LIKE @search OR categorie LIKE @search";
                    }
                    sql += " ORDER BY ID DESC OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@search", "%" + search + "%");
                        command.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@pageSize", pageSize);

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
                                parfumuriInfo.created_at = reader.GetDateTime(7).ToString("MM/dd/yyyy");
                                listParfumuri.Add(parfumuriInfo);
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

    public class ParfumuriInfo
    {
        public int ID { get; set; }
        public string Title { get; set; } = "";
        public string Cod_Parfum { get; set; } = "";
        public decimal Pret { get; set; }
        public string Descriere { get; set; } = "";
        public string categorie { get; set; } = "";
        public string imagine { get; set; } = "";
        public string created_at { get; set; } = "";
    }


}