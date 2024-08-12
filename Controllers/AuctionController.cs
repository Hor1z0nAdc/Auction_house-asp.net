using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Auction_house {
    [ApiController]
    [Route("")]

    public class AuctionController : Controller {
        Database databaseObject = new Database();
        const string határidő = "2022-01-16";

        [HttpGet]
        public ActionResult home() {
            return Redirect("aukciok");
        }


        [HttpGet("aukciok")]
        public ActionResult aukciok() {
            //Retriving all auctions
            databaseObject.OpenConnection();
            String query = "SELECT id, leírás, kikiáltási_ár, aktuális_ár, határidő, kép, név FROM árverés;";
            SQLiteCommand command = new SQLiteCommand(query, databaseObject.connection);
            SQLiteDataReader reader = command.ExecuteReader();

            //Creating objects from the auctions and passing it to view
            List<Árverés> árverésList = new List<Árverés>();
            while(reader.Read()) {
                Árverés árverésObject = new Árverés();
                árverésObject.id = reader.GetInt32(0);
                árverésObject.kikiáltási_ár = Convert.ToDouble(reader["kikiáltási_ár"]);
                árverésObject.aktuális_ár = Convert.ToDouble(reader["aktuális_ár"]);
                árverésObject.határidő = reader["határidő"].ToString();
                árverésObject.kép = reader["kép"].ToString();
                árverésObject.név = reader["név"].ToString();
                árverésObject.leírás = reader["leírás"].ToString();

                árverésList.Add(árverésObject);
            }
            databaseObject.CloseConnection();

            ViewData["árverések"] = árverésList;
            return View();
        }
        
        [Authorize]
        [HttpGet("ujArveres")]
        public ActionResult ujArveres() {
            return View();
        }

        [Authorize]
        [HttpPost("ujArveres")]
        public ActionResult postUjArveres([FromForm] string név,
                                          [FromForm] string ár,
                                          [FromForm] string leírás,
                                          [FromForm] IFormFile kép ) {
            //Validation of form inputs
            Boolean isError = false;
            int kikiáltási_ár;
            if(string.IsNullOrWhiteSpace(név)) {
                TempData["névError"] = "Adja meg az árverés nevét!";
                isError = true;
            }   
            if(string.IsNullOrWhiteSpace(ár)) {
                TempData["árError"] = "Adja meg az árverés kikiáltási árát!";
                isError = true;
            }    
            if(string.IsNullOrWhiteSpace(leírás)) {
                TempData["leírásError"] = "Adja meg az árverés leírását!";
                isError = true;
            }  
            if(kép == null) {
                TempData["képError"] = "Töltsön fel egy képet a tárgyról!";
                isError = true;
            }         
            try{
                kikiáltási_ár = Int32.Parse(ár);
            }
            catch (Exception e){
                TempData["árError"] = "Adja meg az árverés kikiáltási árát (érvényes szám formátumban)!";
                isError = true;
            }

            if(isError) {
                return View("ujarveres");
            }  

            //Processing form input data                  
            string dateTime = DateTime.Now.ToString();
            dateTime = dateTime.Replace(" ", "");
            dateTime = dateTime.Replace(".", "");
            dateTime = dateTime.Replace(":", "");
            string[] imageInfo = kép.FileName.Split(".");
            string image = imageInfo[0] + dateTime + "." +imageInfo[1];
            string path = "wwwroot/images/";

            //Upload image from form
            FileStream fileStream = new FileStream(path + image, FileMode.Create, FileAccess.Write);
            kép.CopyTo(fileStream);  

            //Save item into database 
            string query = "INSERT INTO árverés ('leírás', 'kikiáltási_ár',	'aktuális_ár', 'határidő', 'kép', 'név') VALUES " +
                            "(@leírás, @kikiáltási_ár, @aktuális_ár, @határidő, @kép, @név)";
            SQLiteCommand command = new SQLiteCommand(query, databaseObject.connection);
            databaseObject.OpenConnection();
            command.Parameters.AddWithValue("@leírás", leírás);
            command.Parameters.AddWithValue("@kikiáltási_ár", ár);
            command.Parameters.AddWithValue("@aktuális_ár", 0);
            command.Parameters.AddWithValue("@határidő", határidő);
            command.Parameters.AddWithValue("@kép", image);
            command.Parameters.AddWithValue("@név", név);
            command.ExecuteNonQuery();
            databaseObject.CloseConnection();   

            return Redirect("aukciok");
        }
    }
}   