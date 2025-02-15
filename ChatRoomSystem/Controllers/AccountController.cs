using ChatRoomSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace ChatRoomSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ChatDbContext _context;

        public AccountController(ChatDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public JsonResult SignUp(User obj)
        {
            var userExist = _context.Users.Any(x => x.UserName == obj.UserName|| x.Email==obj.Email);
            if (userExist) {
                return Json(new { success = false, message = "username/Email already exists" });

            }
            var user = new User
            {
                UserName = obj.UserName,
                Email = obj.Email,
                Name = obj.Name,
                Password = HashPassword(obj.Password),
                Active = true,



            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return Json(new { success = true, message = "user created succefully" });

        }

        [HttpPost]
        public JsonResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserName == username);

            if (user == null || !VerifyPassword(password, user.Password))
            {
                return Json(new { success = false, message = "Invalid credentials." });
            }

            return Json(new
            {
                success = true,
                id = user.Id,
                username = user.UserName,
                name = user.Name
            });
        }

        #region PasswordHashing
        private const int SaltSize = 16; // Size of the salt in bytes
        private const int HashSize = 20; // Size of the hash in bytes
        private const int Iterations = 10000; // Number of iterations for PBKDF2
        // Method to hash a password
        private string HashPassword(string password)
        {
            // Generate a salt
            byte[] salt = GenerateSalt();

            // Hash the password with the salt
            byte[] hash = GenerateHash(password, salt);

            // Combine salt and hash into a single byte array
            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            // Convert to Base64 string for storage
            return Convert.ToBase64String(hashBytes);
        }

        // Method to generate a random salt
        private byte[] GenerateSalt()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[SaltSize];
                rng.GetBytes(salt);
                return salt;
            }
        }

        // Method to hash the password with a given salt
        private byte[] GenerateHash(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                return pbkdf2.GetBytes(HashSize);
            }
        }

        // Method to verify a password against a stored hash
        private bool VerifyPassword(string password, string storedHash)
        {
            // Convert stored hash from Base64 to byte array
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // Extract salt and hash from stored hash
            byte[] salt = new byte[SaltSize];
            byte[] hash = new byte[HashSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);
            Array.Copy(hashBytes, SaltSize, hash, 0, HashSize);

            // Hash the password with the extracted salt
            byte[] testHash = GenerateHash(password, salt);

            // Compare the hashes
            for (int i = 0; i < HashSize; i++)
            {
                if (hash[i] != testHash[i])
                    return false;
            }

            return true;
        }

        [HttpPost]
        public JsonResult TestHash(string password)
        {
            string x = HashPassword(password);

            return Json(new
            {
                message = x
            });
        }

        [HttpPost]
        public JsonResult TestPasswordVerification(string password, string storedHash)
        {
            bool isValid = VerifyPassword(password, storedHash);

            return Json(new
            {
                success = isValid,
                message = isValid ? "Password is valid." : "Invalid password."
            });
        }
        #endregion
    }
}
