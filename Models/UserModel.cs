using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace Auction_house {

    public class UserModel { 
        public string? email {get; set;}
        public string? jelszó1 {get; set;}
        public string? jelszó2 {get; set;}
    }
}