using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace GaneShop.Pages.Auth
{
    [BindProperties]
    public class RegisterModel : PageModel
    {

        [Required(ErrorMessage="Numele nu a fost introdus")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Prenumele nu a fost introdus")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Email-ul nu a fost introdus"), EmailAddress]
        public string Email { get; set; } = "";
        public string? Phone { get; set; } = "";

        [Required(ErrorMessage = "Adresa  nu a fost introdusa")]
        public string Address { get; set; } = "";

        [Required(ErrorMessage = "Parola nu a fost introdusa")]
        [StringLength(20,ErrorMessage ="Parola trebuie sa contina maxim de 20 de caractere")]
        [MinLength(5, ErrorMessage = "Parola trebuie să conțină minim 5 caractere")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage ="Confirmarea parolei este necesara")]
        [Compare("Password",ErrorMessage = "Campurile pentru parola si confirmarea parolei nu se potrivesc")]
        public string ConfirmPassword { get; set; } = "";

        public string errorMessage = "";
        public string successMessage = "";

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
            if (!ModelState.IsValid) 
            {

                errorMessage = "Datele nu au fost validate";
                return;
            }
            if (Phone == null) Phone = "";


            string connectionString = "Data Source=.\\sqlexpress01;Initial Catalog=bestshop;Integrated Security=True";

            try 
            {

                using(SqlConnection connection = new SqlConnection(connectionString)) 
                {

                    connection.Open();
                    string sql = "INSERT INTO users" +
                        "(firstname,lastname,email,phone,address,password,role)  VALUES " +
                        "(@firstname,@lastname,@email,@phone,@address,@password,'client'); ";

                    var passwordHasher = new PasswordHasher<IdentityUser>();
                    string hashedPassword = passwordHasher.HashPassword(new IdentityUser(), Password);

                    using (SqlCommand command = new SqlCommand(sql, connection)) 
                    {
                        command.Parameters.AddWithValue("@firstname", FirstName);
                        command.Parameters.AddWithValue("@lastname", LastName);
                        command.Parameters.AddWithValue("email", Email);
                        command.Parameters.AddWithValue("@phone", Phone);
                        command.Parameters.AddWithValue("@address", Address);
                        command.Parameters.AddWithValue("@password", hashedPassword);

                        command.ExecuteNonQuery();
                                                  
                    }

                }
            }

            catch (Exception ex) 
            {

                errorMessage = ex.Message;
                return;
            
            }

            // initializare de autentificare


            try 
            {
                using(SqlConnection connection= new SqlConnection(connectionString)) 
                {
                    connection.Open();
                    string sql = "SELECT * FROM users WHERE email=@email";

                    using(SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", Email);

                        using(SqlDataReader reader= command.ExecuteReader()) 
                        { 
                            if(reader.Read()) 
                            {

                                int id = reader.GetInt32(0);
                                string firstname=reader.GetString(1);
                                string lastname=reader.GetString(2);
                                string email=reader.GetString(3);
                                string phone=reader.GetString(4);
                                string address=reader.GetString(5);
                                //string hashedPassword=reader.GetString(6);
                                string role=reader.GetString(7);
                                string created_at = reader.GetDateTime(8).ToString("MM/dd/yyyy");

                                HttpContext.Session.SetInt32("id", id);
                                HttpContext.Session.SetString("firstname", firstname);
                                HttpContext.Session.SetString("lastname", lastname);
                                HttpContext.Session.SetString("email", email);
                                HttpContext.Session.SetString("phone", phone);
                                HttpContext.Session.SetString("address", address);
                                HttpContext.Session.SetString("role", role);
                                HttpContext.Session.SetString("created_at", created_at);

                            }
                        }

                    }
                }

            }
            catch (Exception ex) 
            {
            
                errorMessage = ex.Message;
                return;
            }

                successMessage = "Contul a fost creat cu succes!";

            Response.Redirect("/");

        }

    }
}
