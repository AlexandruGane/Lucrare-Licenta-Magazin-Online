using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace GaneShop.Pages.Admin.Parfumuri
{
    public class EditModel : PageModel
    {
        [BindProperty]
        public int Id { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Titlul este necesar")]
        [MaxLength(100, ErrorMessage = "Titlul nu poate contine mai mult de 100 de caractere")]
        public string Title { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Codul pentru parfum este necesar")]
        [MaxLength(75, ErrorMessage = "Codul pentru parfum nu poate contine mai mult de 75 de caractere")]
        public string Cod_Parfum { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Codul pentru pret este necesar")]
        public decimal Pret { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Codul pentru descriere este necesar")]
        [MaxLength(50, ErrorMessage = "Descrierea nu poate contine mai mult de 50 de caractere")]
        public string Descriere { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Codul pentru categorie este necesar")]
        [MaxLength(50, ErrorMessage = "Categoria nu poate contine mai mult de 50 de caractere")]
        public string categorie { get; set; } = "";

        [BindProperty]
        public string ImagineFileName { get; set; } = "";

        [BindProperty]
        public IFormFile? imagine { get; set; }

        public string errorMessage = "";
        public string succesMessage = "";


        public void OnGet()
        {
            string requestId = Request.Query["id"];

            try 
            {
                string connectionString = "Data Source=.\\sqlexpress01;Initial Catalog=bestshop;Integrated Security=True";

                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Parfumuri WHERE id=@Id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        command.Parameters.AddWithValue("Id", requestId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            if (reader.Read())
                            {

                                Id = reader.GetInt32(0);
                                Title = reader.GetString(1);
                                Cod_Parfum = reader.GetString(2);
                                Pret = reader.GetDecimal(3);
                                Descriere = reader.GetString(4);
                                categorie = reader.GetString(5);
                                ImagineFileName = reader.GetString(6);


                            }
                            else
                            {

                                Response.Redirect("/Admin/Parfumuri/Index");
                            }
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
                Response.Redirect("/Admin/Parfumuri/Index");
            }
        }
        public void OnPost() 
        {
            if (!ModelState.IsValid) 
            {
                errorMessage = "Datele nu au fost introduse";
                return;
            }

            if (Descriere == null) Descriere = "";


            succesMessage = "Datele au fost introduse";


        }
    }
}
