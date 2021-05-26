using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace insta_scrape.Authentication
{
    public static class PasswordHelper
    {
        public static string GetHash(string password)
        {
            return Convert.ToBase64String(
                System.Security.Cryptography.SHA256.Create()
                .ComputeHash(Encoding.UTF8.GetBytes(password))
                );
        }
    }
}
