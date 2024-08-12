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
using Microsoft.AspNetCore.SignalR;

namespace Auction_house {
    [ApiController]
    [Route("licit")]

    public class BidController : Controller {
         Database databaseObject = new Database();
         IHubContext<StateHub> HubContext;
         string userId;


        public BidController(IHttpContextAccessor httpContextAccessor) {
            userId = httpContextAccessor.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.Sid).Value ;
        }  

        [Authorize]
        [HttpGet("{id}")]
        public ActionResult licitalas(string id) {
            //Retriving auction with specific id
            databaseObject.OpenConnection();
            String query = "SELECT id, név, leírás, kikiáltási_ár, aktuális_ár, határidő, kép, győztes " +
                           "FROM árverés  WHERE id=@id";
            SQLiteCommand command = new SQLiteCommand(query, databaseObject.connection);
            command.Parameters.AddWithValue("@id", id);
            SQLiteDataReader reader = command.ExecuteReader();
            
            //Create and send árverés object to view
            Árverés item = new Árverés();
            while(reader.Read()) {
               item.id = reader.GetInt32(0);
               item.kikiáltási_ár = Convert.ToDouble(reader["kikiáltási_ár"]);
               item.aktuális_ár = Convert.ToDouble(reader["aktuális_ár"]);
               item.határidő = reader["határidő"].ToString();
               item.kép = reader["kép"].ToString();
               item.név = reader["név"].ToString();
               item.győztes = reader["győztes"].ToString();
               item.leírás = reader["leírás"].ToString();
            }
            ViewData["item"] = item;

            //Send number of bids and actual price on the item to view 
            query = "SELECT COUNT(*) FROM licit WHERE árverés_id=@id";
            SQLiteCommand command2 = new SQLiteCommand(query, databaseObject.connection);
            command2.Parameters.AddWithValue("@id", id);
            Int64 numOfBids = (Int64)command2.ExecuteScalar();

            ViewData["numOfBids"] = numOfBids;
            databaseObject.CloseConnection();
            
            return View();
        }

        [Authorize]
        [HttpPost("{id}")]
        public async Task<IActionResult> postLicitalas([FromForm] string ajánlat,
                                          [FromForm] string numOfBids,
                                          [FromForm] string aktuális_ár,
                                          [FromForm] string kikiáltási_ár,
                                                     string id)
        {   
            //Register licit into database
            databaseObject.OpenConnection();
            String query = "INSERT INTO licit ('ajánlat', 'felhasználó_id', 'árverés_id') VALUES " + 
                           "(@ajánlat, @felhasználó_id, @árverés_id)";
            SQLiteCommand command = new SQLiteCommand(query, databaseObject.connection);
            command.Parameters.AddWithValue("@ajánlat", ajánlat);
            command.Parameters.AddWithValue("@felhasználó_id", userId);
            command.Parameters.AddWithValue("@árverés_id", id);
            command.ExecuteNonQuery();

            /*Calculate item price - current number of bids is one value smaller, 
              because the data from form is outdated, it was set before inserting new licit 
              into database by the code above
            */
            int intNumOfBids = int.Parse(numOfBids);
            int termék_ára = int.Parse(kikiáltási_ár);

            if (intNumOfBids > 0) {
                    //Find second highest bid and add 1 euro to it
                    query = "SELECT MAX(ajánlat) FROM licit WHERE árverés_id=@id AND ajánlat < (SELECT MAX(ajánlat) FROM licit WHERE árverés_id=@id)";
                    SQLiteCommand command2 = new SQLiteCommand(query, databaseObject.connection);
                    command2.Parameters.AddWithValue("@id", id);
                    SQLiteDataReader reader = command2.ExecuteReader();

                    int secondHighestAjánlat = 0;
                    while(reader.Read()) {
                        secondHighestAjánlat = reader.GetInt32(0);
                    }

                    termék_ára = secondHighestAjánlat + 1;
            }
            
            //Find user with the highest bid
            query = "SELECT f.email, MAX(ajánlat) FROM licit l " +
                    "JOIN felhasználó f ON l.felhasználó_id = f.id";
            SQLiteCommand command3 = new SQLiteCommand(query, databaseObject.connection);
            SQLiteDataReader reader2 = command3.ExecuteReader();

            string felhasználó_email = null;
            while(reader2.Read()) {
                felhasználó_email = reader2["email"].ToString();
            }

            //Update aktuális_ár with new value
            query = "UPDATE árverés SET aktuális_ár=@aktuális_ár, győztes=@győztes  WHERE id=@id";
            SQLiteCommand command4 = new SQLiteCommand(query, databaseObject.connection);
            command4.Parameters.AddWithValue("@aktuális_ár", termék_ára);
            command4.Parameters.AddWithValue("@győztes", felhasználó_email);
            command4.Parameters.AddWithValue("@id", id);
            command4.ExecuteNonQuery();
            databaseObject.CloseConnection();

            return Redirect("../aukciok");
        }
    }

}    