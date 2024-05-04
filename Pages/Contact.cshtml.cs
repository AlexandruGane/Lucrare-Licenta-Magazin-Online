using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations; /*biblioteca utilizata pentru a folosi [Required] */
using System.Data.SqlClient;
using System.Numerics;



namespace GaneShop.Pages
{
    public class ContactModel : PageModel
    {
        public void OnGet()
        {
        }

        [BindProperty]
        [Required(ErrorMessage = "Campul aferent numelui nu a fost completat")]
        [Display(Name ="Numele*")]
        public string Numele { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "Campul aferent prenumelui nu a fost completat")]
        [Display(Name="Prenumele*")]
        public string Prenumele { get; set; } = "";
        [BindProperty, Required(ErrorMessage = "Campul aferent pentru email nu a fost completat")]
        [EmailAddress]
        [Display(Name ="Email*")]
        public string Email { get; set; } = ""; 

        [BindProperty]
        public string? Telefon { get; set; } = "";

        [BindProperty, Required]
        public string Subiectul { get; set; } = "";
        [BindProperty, Required(ErrorMessage = "Campul aferent mesajului nu a fost completat")]
        [MinLength(2,ErrorMessage ="Ati introdus mult prea putine caractere ca mesajul dvs sa fie luat in considerare!")]
        [MaxLength(150,ErrorMessage ="Nu puteti sa introduceti un text mai mare de 150 de caractere!")]
        [Display(Name="Subiectul")]
        public string Mesaj { get; set; } = "";

        public List<SelectListItem> ListaSubiect { get; } = new List<SelectListItem>

{
    new SelectListItem { Value = "Starea comenzii", Text = "Starea comenzii" },
    new SelectListItem { Value = "Cerere pentru rambursare", Text = "Cerere pentru rambursare" },
    new SelectListItem { Value = "Zona de cariere", Text = "Zona de cariere" },
    new SelectListItem { Value = "Altele", Text = "Altele" }
};



        public string MesajBun { get; set; } = "";
        public string MesajEronat { get; set; } = "";


        public void  OnPost()
        {
           
            //Cream o metoda simpla prin care daca campurile din formular sunt goale sa ne returneze un mesaj de eroare */
        
            if(!ModelState.IsValid )

            {
                //Afisare mesaj de eroare corespunzator

                MesajEronat = "Am rugamintea sa completezi toate campurile!";
                    return;
        
            }
                 MesajBun = "Am receptionat cerinta dumneavoastra";





            if (Telefon == null)

            {
                Telefon = "null";
            }

            /*adaugare de mesaje intr-o baza de date */
           


            try
            {
                string connectionString = "Data Source=.\\sqlexpress01;Initial Catalog=bestshop;Integrated Security=True ";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO messages" +
                        "(firstname,lastname,email,phone,subject,message) VALUES" +
                        "(@Numele,@Prenumele,@Email,@Telefon,@Subiectul,@Mesaj);";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        command.Parameters.AddWithValue("@Numele", Numele);
                        command.Parameters.AddWithValue("@Prenumele", Prenumele);
                        command.Parameters.AddWithValue("@Email",Email);
                        command.Parameters.AddWithValue("@Telefon", Telefon);
                        command.Parameters.AddWithValue("@Subiectul", Subiectul);
                        command.Parameters.AddWithValue("@Mesaj", Mesaj);

                        command.ExecuteNonQuery();
                    }
                }
            }

            catch (Exception ex)
            {
                /* Afisarea unui mesaj de eroare in cazul in care intervin probleme tehnice */

                MesajEronat = ex.Message;
                return;

            }


            Numele = "";
            Prenumele = "";
            Email = "";
            Telefon = "";
            Subiectul = "";
            Mesaj = "";

            ModelState.Clear();


         }

        }
        

}
