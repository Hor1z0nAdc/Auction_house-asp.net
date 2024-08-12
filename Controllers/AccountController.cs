using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Auction_house {
    [ApiController]
    [Route("fiok")]

    public class AccountController : Controller {
        Database databaseObject = new Database();

        [HttpGet("bejelentkezes")]
        public ActionResult bejelentkezes() {
            if(User.Identity.IsAuthenticated) return Redirect("../aukciok");
            return View();
        }

        [HttpGet("regisztracio")]
        public ActionResult regisztracio() {
            if(User.Identity.IsAuthenticated) return Redirect("../aukciok");
            return View();
        }

        [HttpPost("bejelentkezes")]
        public ActionResult postBejelentkezes([FromForm] string email, 
                                              [FromForm] string jelszó ) {
            //Looking for user with given email                                        
            String query = "SELECT id, email, password, salt FROM felhasználó WHERE email=@email;";
            SQLiteCommand command = new SQLiteCommand(query, databaseObject.connection);
            databaseObject.OpenConnection();
            command.Parameters.AddWithValue("@email", email);
            SQLiteDataReader reader = command.ExecuteReader();

            //User exists
            if(reader.HasRows) {
                //Check password
                string? id = null;
                string? salt = null;
                string? databasePassword = null;

                while(reader.Read()) {
                    id = reader["id"].ToString();
                    salt = reader["salt"].ToString();
                    databasePassword = reader["password"].ToString();    
                }
                databaseObject.CloseConnection();
                string hashedJelszó = PasswordHasher.generateHash(jelszó + salt);

                //Valid password
                if(!string.IsNullOrEmpty(databasePassword) && hashedJelszó == databasePassword) {
                    //Creating user identity for sending cookie with ticket
                    var claims = new List<Claim>() {
                        new Claim(ClaimTypes.Email, email),
                        new Claim(ClaimTypes.Hash, hashedJelszó),
                        new Claim(ClaimTypes.Sid, id)
                    };
                    var identity = new ClaimsIdentity(claims, "userIdentity");
                    var userPrincipal = new ClaimsPrincipal(identity);
                     HttpContext.SignInAsync(userPrincipal);

                    return Redirect("../aukciok");
                }
                //Invalid password
                TempData["pwError"] = "Hibás jelszót adott meg.";
       
                return View("bejelentkezes");
            }

            //Invalid email
            TempData["emailError"] = "Nem létezik felhasználó ezzel az emailcímmel.";
            return View("bejelentkezes");
        }

        [HttpPost("regisztracio")]
        public ActionResult postRegisztracio([FromForm] string email,
                                             [FromForm] string jelszó1,
                                             [FromForm] string jelszó2)
        {   
            //Validating passwords
            Boolean isError = false;
            if(string.IsNullOrEmpty(jelszó1)) {
                TempData["pw1Error"] = "A jelszót kötelező megadnia.";
                isError = true;
            }
            else if(jelszó1.Length < 4 || jelszó1.Length > 16) {
                TempData["pw1Error"] = "A jelszónak 4-16 karakter között kell lennie.";
                isError = true;
            }
            else if(jelszó1 != jelszó2) {
                TempData["pw1Error"] = "A megadott jelszavak nem egyeznek.";
                isError = true;
            }

            if(string.IsNullOrEmpty(jelszó2)) {
                TempData["pw2Error"] = "Ismét adja meg a jelszavát!.";
                isError = true;
            }

            //Checking if user with given email exists
            String query = "SELECT EXISTS(SELECT 1 FROM felhasználó WHERE email=@email);";
            SQLiteCommand command1 = new SQLiteCommand(query, databaseObject.connection);
            databaseObject.OpenConnection();
            command1.Parameters.AddWithValue("@email", email);
            Int64 userCount = (Int64)command1.ExecuteScalar();

            if(userCount > 0 ) {
                TempData["emailError"] = "A megadott emailcím foglalt.";
                isError = true;
            }

            if(isError){
                TempData["email"] = email;
                return View("regisztracio");
            }

            //Storing validated user in the database
            string salt = PasswordHasher.generateSalt();
            string password = PasswordHasher.generateHash(jelszó1 + salt);

            query = "INSERT Into felhasználó ('email', 'password', 'salt') "+
                                        "VALUES" +
                            "(@email, @password, @salt)";
            SQLiteCommand command2 = new SQLiteCommand(query, databaseObject.connection);
            command2.Parameters.AddWithValue("@email", email);
            command2.Parameters.AddWithValue("@password", password);
            command2.Parameters.AddWithValue("@salt", salt);
            command2.ExecuteNonQuery();
            databaseObject.CloseConnection();
            return Redirect("bejelentkezes");
        }

        [HttpGet("kijelentkezes")]
        public ActionResult kijelentkezes() {
            HttpContext.SignOutAsync();
            return Redirect("bejelentkezes");
        }

        
    }
}    