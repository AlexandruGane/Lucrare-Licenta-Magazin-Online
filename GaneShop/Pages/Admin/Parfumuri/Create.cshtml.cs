using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace GaneShop.Pages.Admin.Parfumuri
{
    public class CreateModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage="Titlul este necesar")]
        [MaxLength(100,ErrorMessage ="Titlul nu poate contine mai mult de 100 de caractere")]
        public string Title { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage ="Codul pentru parfum este necesar")]
        [MaxLength(75,ErrorMessage ="Codul pentru parfum nu poate contine mai mult de 75 de caractere")]
        public string Cod_Parfum { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage ="Codul pentru pret este necesar")]
        public decimal Pret { get; set; }

        [BindProperty]
        [Required(ErrorMessage ="Codul pentru descriere este necesar")]
        [MaxLength(50,ErrorMessage ="Descrierea nu poate contine mai mult de 50 de caractere")]
        public string Descriere { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage ="Codul pentru categorie este necesar")]
        [MaxLength(50, ErrorMessage ="Categoria nu poate contine mai mult de 50 de caractere")]
        public string categorie { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage ="Codul pentru imagine este necesar")]
        public IFormFile imagine { get; set; }

        public string errorMessage = "";
        public string succesMessage = "";


        private IWebHostEnvironment webHostEnvironment;
        public CreateModel(IWebHostEnvironment env)
        {
            webHostEnvironment = env;

        }

        public void OnGet()
        {

        }

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                errorMessage = "Validarea de data a esuat";
                return;

            }

            if (Descriere == null) Descriere = "";


            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(imagine.FileName);
            string imageFolder = webHostEnvironment.WebRootPath + "/images/Parfumuri/";
            string imageFullPath = Path.Combine(imageFolder, newFileName);

            using (var stream = System.IO.File.Create(imageFullPath))
            {
                imagine.CopyTo(stream);
            }
            try
            {
                string connectionString = "Data Source=.\\sqlexpress01;Initial Catalog=bestshop;Integrated Security=True";
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO Parfumuri" +
                        "(Title,Cod_parfum,Pret,Descriere,categorie,imagine) VALUES" +
                        "(@title,@cod_parfum,@pret,@descriere,@categorie,@imagine);";


                    using(SqlCommand command=new SqlCommand(sql, connection))
                    {


                        command.Parameters.AddWithValue("@title", Title);
                        command.Parameters.AddWithValue("@cod_parfum", Cod_Parfum);
                        command.Parameters.AddWithValue("@pret", Pret);
                        command.Parameters.AddWithValue("@descriere", Descriere);
                        command.Parameters.AddWithValue("@categorie", categorie);
                        command.Parameters.AddWithValue("@imagine", newFileName);

                        command.ExecuteNonQuery();


                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage=ex.Message;
                return;

            }


            succesMessage = "Datele au fost salvate corect";
            Response.Redirect("/Admin/Parfumuri/Index");


        }

         
    }
}
