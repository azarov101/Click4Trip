using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Click4Trip.Classes
{
    public class Encryption
    {
        public const int SALT_SIZE = 24;
        public const int HASH_SIZE = 24;
        public const int PBKDF2_ITT = 500;


        public string CreateHash(string password)
        {
            //Generate Random salt
            RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[SALT_SIZE];
            csprng.GetBytes(salt);

            //Generate the Hashed password
            byte[] hash = PBKDF2(password, salt, PBKDF2_ITT, HASH_SIZE);
            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);

        }

        private byte[] PBKDF2(string password, byte[] salt, int pBKDF2_ITT, int outputBytes)
        {
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt);
            pbkdf2.IterationCount = pBKDF2_ITT;
            return pbkdf2.GetBytes(outputBytes);
        }

        private bool SlowEqual(byte[] dbHash, byte[] passHash)
        {
            //Check length
            uint diff = (uint)dbHash.Length ^ (uint)passHash.Length;

            for (int i = 0; i < dbHash.Length && i < passHash.Length; i++)
                diff |= (uint)dbHash[i] ^ (uint)passHash[i];

            return diff == 0;
        }

        public bool ValidatePassword(string password, string dbHash, string dbSalt)
        {
            byte[] salt = Convert.FromBase64String(dbSalt);
            byte[] hash = Convert.FromBase64String(dbHash);

            byte[] hashToValidate = PBKDF2(password, salt, PBKDF2_ITT, hash.Length);

            return SlowEqual(hash, hashToValidate);

        }
    }
}