using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Services
{
    public static class Utils
    {
        const string JWTConfigKey = "JWTSecret";

        public static string GetConnectionString(string name = "DapperDB")
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        public static string GetJWTSecret()
        {
            var secret = ConfigurationManager.AppSettings.Get(JWTConfigKey);

            if(secret == null)
                throw new Exception("Error in the login parameters , report bug");

            return secret;
        }

        public static string HashPassword(string Password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Password));

                // Get the hashed string.  
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}