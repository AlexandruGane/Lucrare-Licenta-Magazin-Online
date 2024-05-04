using GaneShop.Pages.Admin.Parfumuri;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Net;
using System.Windows.Input;

namespace GaneShop.Pages
{
    [BindProperties]
    public class CardModel : PageModel
    {

        [Required(ErrorMessage = "Adresa dvs nu a fost introdusa!")]
        public string Address { get; set; } = "";

        [Required]
        public string Metoda_Plata { get; set; } = "";

        public List<OrderItem> listOrderItems= new List<OrderItem>();
        public decimal Pret_Produse = 0;
        public decimal Transport = 5;
        public decimal Total_Plata = 0;


        private Dictionary<String, int> getParfumuriDictionary()
        {
            var ParfumuriDictionary =new Dictionary<String, int>();
            string cookieValue = Request.Cookies["magazin_cart"] ?? "";
            if (cookieValue.Length > 0)
            {
                string[] parfumuriIdArray = cookieValue.Split('-');
                

                for(int i = 0; i< parfumuriIdArray.Length; i++)
                {
                    string parfumuriID = parfumuriIdArray[i];
                    if(ParfumuriDictionary.ContainsKey(parfumuriID))
                    {
                        ParfumuriDictionary[parfumuriID] += 1;

                    }
                    else
                    {
                        ParfumuriDictionary.Add(parfumuriID, 1);
                    }
                }
            }
            return ParfumuriDictionary;
        }


        public void OnGet()
        {
            var ParfumuriDictionary=getParfumuriDictionary();

            //adaugare de insert update si delete in cosul de parfum
            string? action = Request.Query["action"];
            string? id= Request.Query["id"];

            

           
            if (action != null && id != null && ParfumuriDictionary.ContainsKey(id))
            {
                if (action.Equals("add"))
                {
                    ParfumuriDictionary[id] += 1;
                }
                else if (action.Equals("sub"))
                {
                    if (ParfumuriDictionary[id] > 1)
                        ParfumuriDictionary[id] -= 1;

                }
                else if (action.Equals("delete"))
                {
                    ParfumuriDictionary.Remove(id);
                }

                string newCookieValue = "";

                foreach (var keyValuePair in ParfumuriDictionary)
                {
                    for (int i = 0; i < keyValuePair.Value; i++)
                    {
                        newCookieValue += "-" + keyValuePair.Key;
                    }
                }

                if (newCookieValue.Length > 0)
                {
                    newCookieValue = newCookieValue.Substring(1);

                }

                var cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.Now.AddDays(365);
                cookieOptions.Path = "/";

                Response.Cookies.Append("magazin_cart", newCookieValue, cookieOptions);

            }

            try
            {
                string connectionString = "Data Source=.\\sqlexpress01;Initial Catalog=bestshop;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    string sql = "SELECT * FROM Parfumuri WHERE id=@id";
                    foreach (var KeyValuePair in ParfumuriDictionary)
                    {
                        string parfumuriID = KeyValuePair.Key;
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {

                            command.Parameters.AddWithValue("@id", parfumuriID);
                            using(SqlDataReader reader = command.ExecuteReader()) 
                            { 
                                if(reader.Read())
                                {
                                    OrderItem item = new OrderItem();
                                    item.ParfumuriInfo.ID = reader.GetInt32(0);
                                    item.ParfumuriInfo.Title = reader.GetString(1);
                                    item.ParfumuriInfo.Cod_Parfum = reader.GetString(2);
                                    item.ParfumuriInfo.Pret = reader.GetDecimal(3);
                                    item.ParfumuriInfo.Descriere = reader.GetString(4);
                                    item.ParfumuriInfo.categorie = reader.GetString(5);
                                    item.ParfumuriInfo.imagine = reader.GetString(6);
                                    item.ParfumuriInfo.created_at = reader.GetDateTime(7).ToString("MM/dd/YYYY");

                                    item.numCopies=KeyValuePair.Value;
                                    item.totalPrice = item.numCopies * item.ParfumuriInfo.Pret;
                                 

                                    listOrderItems.Add(item);

                                    Pret_Produse += item.totalPrice;


                                }
                                

                                Total_Plata = Pret_Produse + Transport;

                            }

                        }
                    }

                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Address = HttpContext.Session.GetString("address") ?? "";



        }


        public string errorMessage = "";
        public string successMessage = "";
        private string sqlComanda;

        public void OnPost()
        {
            int client_id = HttpContext.Session.GetInt32("id") ?? 0;
            if(client_id <1) 
            {
                Response.Redirect("/Auth/Login");
                return;

            }

            if (!ModelState.IsValid)
            {
                errorMessage = "Datele introduse nu sunt corecte";
                return;
            }

            var ParfumuriDictionary = getParfumuriDictionary();
            if (ParfumuriDictionary.Count < 1)
            {
                errorMessage = "Cosul dvs de cumparaturi este gol";
                return;
            }
            try
            {
                string connectionString = "Data Source=.\\sqlexpress01;Initial Catalog=bestshop;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    int noulIdComanda = 0;

                    string sqlComanda = "INSERT INTO comenzi (client_id, data_comanda, taxa_expediere, adresa_livrare, metoda_plata, status_plata, status_comanda) " +
                    "OUTPUT INSERTED.id " +
                    "VALUES (@client_id, CURRENT_TIMESTAMP, @Transport, @Address, @Metoda_plata, 'acceptata', 'expediata')";

                    using (SqlCommand command = new SqlCommand(sqlComanda, connection))
                    {

                        command.Parameters.AddWithValue("@client_id", client_id);
                        command.Parameters.AddWithValue("@Address", Address);
                        command.Parameters.AddWithValue("@Metoda_plata", Metoda_Plata);
                        command.Parameters.AddWithValue("@Transport", Transport);

                        noulIdComanda =(int) command.ExecuteScalar();
                    }

                    string sqlItems = "INSERT INTO comenzi_items (comenzi_id, parfumuri_id, cantitate, pret_unitate) VALUES (@comenzi_id, @parfumuri_id, @cantitate, @pret_unitate)";

                    foreach (var keyValuePair in ParfumuriDictionary)
                    {
                        string ParfumuriID=keyValuePair.Key;
                        int cantitate=keyValuePair.Value;
                        decimal pretUnitate = getParfumuriPrice(ParfumuriID);

                        using (SqlCommand command = new SqlCommand(sqlItems, connection))
                        {
                            command.Parameters.AddWithValue("@comenzi_id", noulIdComanda);
                            command.Parameters.AddWithValue("@parfumuri_id", ParfumuriID);
                            command.Parameters.AddWithValue("@cantitate", cantitate);
                            command.Parameters.AddWithValue("@pret_unitate", pretUnitate);

                            command.ExecuteNonQuery();

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;

            }

            Response.Cookies.Delete("magazin_cart");

            successMessage = "Comanda a fost procesata cu succes! Un operator o sa preia comanda dvs.";
        }

        private decimal getParfumuriPrice(string ParfumuriID)
        {
            decimal pret = 0;
            try
            {
                string connectionString= "Data Source=.\\sqlexpress01;Initial Catalog=bestshop;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT Pret from Parfumuri WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", ParfumuriID);

                        using(SqlDataReader reader = command.ExecuteReader())
                        {
                            if(reader.Read())
                            {
                                pret = reader.GetDecimal(0);
                            }
                        }



                    }
                }

            }
            catch (Exception ex)
            {

            }
            return pret;

        }

    }

    public class OrderItem
    {
        public ParfumuriInfo ParfumuriInfo = new ParfumuriInfo();
        public int numCopies = 0;
        public decimal totalPrice = 0;

    }

}
