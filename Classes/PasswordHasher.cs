using System;
using System.Security.Cryptography;
using System.Text;

namespace Auction_house {

    public class PasswordHasher {
        private static Random random = new Random();
        
        public static string generateHash(string password) {
            byte[] passwordArray = Encoding.Unicode.GetBytes(password);  
            SHA256 hash = SHA256.Create();
            byte[] hashValue = hash.ComputeHash(passwordArray);
            return Convert.ToBase64String(hashValue);
        }

        public static string generateSalt() {
            byte[] bytes = new byte[10];
            random.NextBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}