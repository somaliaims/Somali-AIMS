using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace AIMS.Services.Helpers
{
    public interface ISecurityHelper
    {
        /// <summary>
        /// Gets password hash
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        string GetPasswordHash(string password);
    }

    public class SecurityHelper : ISecurityHelper
    {
        public string GetPasswordHash(string password)
        {
            var crypt = new SHA256Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(password));
            foreach (byte theByte in crypto)
            {
                hash += theByte.ToString("x2");
            }
            return hash;
        }
    }
}
