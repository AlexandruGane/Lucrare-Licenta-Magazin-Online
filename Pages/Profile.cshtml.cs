using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;  // Asigurați-vă că această bibliotecă este adăugată
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace GaneShop.Pages
{
    [RequireAuth]
    public class ProfileModel : PageModel
    {
        [Required(ErrorMessage = "Campul destinat numelui este necesar")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Campul destinat prenumelui este necesar")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Campul destinat email-ului este necesar"), EmailAddress]
        public string Email { get; set; } = "";

        public string? Phone { get; set; } = "";

        [Required(ErrorMessage = "Campul destinat adresei este necesar")]
        public string Address { get; set; } = "";

        public string? Password { get; set; } = "";
        public string? Confirm_Password { get; set; } = "";

        public string MesajEronat = "";
        public string MesajBun = "";

        private readonly IConfiguration _configuration;  // Adăugați acest câmp

        public ProfileModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            FirstName = HttpContext.Session.GetString("firstname") ?? "";
            LastName = HttpContext.Session.GetString("lastname") ?? "";
            Email = HttpContext.Session.GetString("email") ?? "";
            Phone = HttpContext.Session.GetString("phone");
            Address = HttpContext.Session.GetString("address") ?? "";
        }

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                MesajEronat = "Din pacate, validarea datelor nu a reusit.";
                return;
            }

            if (Phone == null) Phone = "";

            string submitButton = Request.Form["action"];

            string connectionString = _configuration.GetConnectionString("DefaultConnection");  // Modificare aici

            if (submitButton.Equals("profile"))
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string sql = "UPDATE users SET firstname=@firstname,lastname=@lastname," +
                            "email=@email,phone=@phone,address=@address WHERE id=@id";

                        int? id = HttpContext.Session.GetInt32("id");

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@firstname", FirstName);
                            command.Parameters.AddWithValue("@lastname", LastName);
                            command.Parameters.AddWithValue("@email", Email);
                            command.Parameters.AddWithValue("@phone", Phone);
                            command.Parameters.AddWithValue("@address", Address);
                            command.Parameters.AddWithValue("@id", id);

                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MesajEronat = ex.Message;
                    return;
                }

                HttpContext.Session.SetString("firstname", FirstName);
                HttpContext.Session.SetString("lastname", LastName);
                HttpContext.Session.SetString("email", Email);
                HttpContext.Session.SetString("phone", Phone);
                HttpContext.Session.SetString("address", Address);

                MesajBun = "Profilul a fost modificat cu succes!";
            }
            else if (submitButton.Equals("password"))
            {
                if (Password == null || Password.Length < 3 || Password.Length > 30)
                {
                    MesajEronat = "Parola trebuie să conțină între 3 și 30 de caractere.";
                    return;
                }

                if (Confirm_Password == null || !Confirm_Password.Equals(Password))
                {
                    MesajEronat = "Parola și Confirmarea Parolei nu se potrivesc.";
                    return;
                }

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "UPDATE users SET password=@password WHERE id=@id AND email=@email";

                        int? id = HttpContext.Session.GetInt32("id");
                        string userEmail = HttpContext.Session.GetString("email");

                        if (id != null && userEmail != null)
                        {
                            var passwordHasher = new PasswordHasher<IdentityUser>();
                            string hashedPassword = passwordHasher.HashPassword(new IdentityUser(), Password);

                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {
                                command.Parameters.AddWithValue("@password", hashedPassword);
                                command.Parameters.AddWithValue("@id", id);
                                command.Parameters.AddWithValue("@email", userEmail);

                                command.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            MesajEronat = "Nu s-a putut identifica utilizatorul.";
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MesajEronat = ex.Message;
                    return;
                }

                MesajBun = "Parola a fost modificată cu succes!";
            }
        }
    }
}