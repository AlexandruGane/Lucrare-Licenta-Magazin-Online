using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace GaneShop.Pages.Auth
{
    [BindProperties]
    public class LoginModel : PageModel
    {

        [Required(ErrorMessage="Email-ul este necesar"),EmailAddress]
        public string Email { get; set; } = "";

        [Required(ErrorMessage ="Campul pentru parola este necesar")]
        public string Password { get; set; } = "";

        public string MesajEronat = "";
        public string MesajValidare = "";

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            base.OnPageHandlerExecuting(context);

            if(HttpContext.Session.GetString("role")!=null) 
            {

                context.Result = new RedirectResult("/");
            }
        }

        public void OnGet()
        {
        }

        public void OnPost() 
        {
            if(!ModelState.IsValid) 
            {
                MesajEronat = "Datele nu au fost introduse";
                return;
            }

            try
            {

                string connectionString = "Data Source=.\\sqlexpress01;Initial Catalog=bestshop;Integrated Security=True";
                using(SqlConnection connection = new SqlConnection(connectionString)) 
                {
                
                    connection.Open();
                    string sql = "SELECT * FROM  users WHERE email=@email";
                    using(SqlCommand command =new SqlCommand(sql, connection)) 
                    {

                        command.Parameters.AddWithValue("@email", Email);

                    using(SqlDataReader reader = command.ExecuteReader())
                        {
                            if(reader.Read())
                            {

                                int id = reader.GetInt32(0);
                                string firstname=reader.GetString(1);
                                string lastname=reader.GetString(2);
                                string email=reader.GetString(3);
                                string phone=reader.GetString(4);
                                string address=reader.GetString(5);
                                string hashedPassword=reader.GetString(6);
                                string role=reader.GetString(7);
                                string created_at=reader.GetDateTime(8).ToString("MM/dd/yyyyy");

                                var passwordHasher = new PasswordHasher<IdentityUser>();
                                var result = passwordHasher.VerifyHashedPassword(new IdentityUser(),
                                    hashedPassword, Password);

                                if (result == PasswordVerificationResult.Success
                                    || result == PasswordVerificationResult.SuccessRehashNeeded)
                                {
                                    
                                    HttpContext.Session.SetInt32("id", id);
                                    HttpContext.Session.SetString("firstname", firstname);
                                    HttpContext.Session.SetString("lastname", lastname);
                                    HttpContext.Session.SetString("email", email);
                                    HttpContext.Session.SetString("phone", phone);
                                    HttpContext.Session.SetString("address", address);
                                    HttpContext.Session.SetString("role", role);
                                    HttpContext.Session.SetString("created_at", created_at);

                                   
                                    Response.Redirect("/");
                                }

                            }
                        }

                    }
                }

            }

            catch(Exception ex) 
            { 
                MesajEronat= ex.Message;
                return;
            
            }

            MesajEronat = "Email-ul sau parola au fost introduse intr-un mod GRESIT";

        }
    }
}
