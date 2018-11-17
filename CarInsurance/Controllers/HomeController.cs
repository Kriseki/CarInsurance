using CarInsurance.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CarInsurance.Controllers
{
    public class HomeController : Controller
    {
        private readonly string connectionString = @"Data Source=STUDENT-PC\SQLEXPRESS;Initial Catalog=Quotes;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(string firstName, string lastName, string emailAddress, string dateofBirth,
                                   string carYear, string carMake, string carModel, string dui, string tickets, string coverage)
        {
            
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(emailAddress) ||
                string.IsNullOrEmpty(dateofBirth) ||string.IsNullOrEmpty(carYear) ||string.IsNullOrEmpty(carMake) || 
                string.IsNullOrEmpty(carModel) || string.IsNullOrEmpty(dui) || string.IsNullOrEmpty(tickets) ||string.IsNullOrEmpty(coverage))
            {
                return View("~/Views/Shared/Error.cshtml");
            }
            else
            {
                DateTime dob = DateTime.Parse(dateofBirth);
                Int32 yearoCar = Convert.ToInt32(carYear);
                Int32 tix = Convert.ToInt32(tickets);
                decimal result = 50m;
                DateTime today = DateTime.Today;
                Int32 age = today.Year - dob.Year;

                if (age < 25 && age > 18)
                {
                    result += 25;
                }
                if (age < 18)
                {
                    result += 100;
                }
                if (age > 100)
                {
                    result += 25;
                }
                if (yearoCar < 2000)
                {
                    result += 25;
                }
                if (yearoCar > 2015)
                {
                    result += 25;
                }
                if (carMake.ToLower() == "porsche")
                {
                    result += 25;
                }
                if (carMake.ToLower() == "porsche" && carModel == "911")
                {
                    result += 25;
                }
                if (tix > 0)
                {
                    Int32 ticket = tix * 10;
                    result += ticket;
                }
                if (dui.ToLower() == "yes" || dui.ToLower() == "y" || dui.ToLower() == "yeah" || dui.ToLower() == "ya")
                {
                    result = (result * .25m) + result;
                }
                if (coverage.ToLower() == "full")
                {
                    result = (result * .5m) + result;
                }

                string queryString = @"INSERT INTO AutoQuotes (FirstName, LastName, EmailAddress, DateofBirth,
                                       CarYear, CarMake, CarModel, Dui, Tickets, Coverage, Total) VALUES
                                       (@FirstName, @LastName, @EmailAddress, @DateofBirth, @CarYear, @CarMake,
                                        @CarModel, @Dui, @Tickets, @Coverage, @Total)";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Parameters.Add("@FirstName", SqlDbType.VarChar);
                    command.Parameters.Add("@LastName", SqlDbType.VarChar);
                    command.Parameters.Add("@EmailAddress", SqlDbType.VarChar);
                    command.Parameters.Add("@DateofBirth", SqlDbType.Date);
                    command.Parameters.Add("@CarYear", SqlDbType.Int);
                    command.Parameters.Add("@CarMake", SqlDbType.VarChar);
                    command.Parameters.Add("@CarModel", SqlDbType.VarChar);
                    command.Parameters.Add("@Dui", SqlDbType.VarChar);
                    command.Parameters.Add("@Tickets", SqlDbType.Int);
                    command.Parameters.Add("@Coverage", SqlDbType.VarChar);
                    command.Parameters.Add("@Total", SqlDbType.Decimal);

                    command.Parameters["@FirstName"].Value = firstName;
                    command.Parameters["@LastName"].Value = lastName;
                    command.Parameters["@EmailAddress"].Value = emailAddress;
                    command.Parameters["@DateofBirth"].Value = dateofBirth;
                    command.Parameters["@CarYear"].Value = carYear;
                    command.Parameters["@CarMake"].Value = carMake;
                    command.Parameters["@CarModel"].Value = carModel;
                    command.Parameters["@Dui"].Value = dui;
                    command.Parameters["@Tickets"].Value = tickets;
                    command.Parameters["@Coverage"].Value = coverage;
                    command.Parameters["@Total"].Value = result;

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                    
                }
                QuoteResults n = new QuoteResults()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Total = result
                };
                    return View("Results", n);
            }
        }

        public ActionResult Admin()
        {
            string queryString = @"SELECT Id, FirstName, LastName, EmailAddress, DateofBirth, CarYear, CarMake,
                                   CarModel, Dui, Tickets, Coverage, Total from AutoQuotes";
            List<InsuranceSignUp> quotes = new List<InsuranceSignUp>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var signup = new InsuranceSignUp();
                    signup.FirstName = reader["FirstName"].ToString();
                    signup.LastName = reader["LastName"].ToString();
                    signup.EmailAddress = reader["EmailAddress"].ToString();
                    signup.DateofBirth = Convert.ToDateTime(reader["DateofBirth"]);
                    signup.CarYear = Convert.ToInt32(reader["CarYear"]);
                    signup.CarMake = reader["CarMake"].ToString();
                    signup.CarModel = reader["CarModel"].ToString();
                    signup.Dui = reader["Dui"].ToString();
                    signup.Tickets = Convert.ToInt32(reader["Tickets"]);
                    signup.Coverage = reader["Coverage"].ToString();
                    signup.Total = Convert.ToDecimal(reader["Total"]);
                    quotes.Add(signup);

                }
            }
                return View(quotes);
        }
    }
}