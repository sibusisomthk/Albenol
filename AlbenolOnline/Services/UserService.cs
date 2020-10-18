using AlbenolOnline.Entities;
using AlbenolOnline.Helpers;
using AlbenolOnline.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AlbenolOnline.Services
{
        public interface IUserService
        {
            Task<User> Authenticate(string username, string password);
            IEnumerable<User> GetAll();
            bool UsernameExist(string username);
            bool EmailExist(String email);
            string testConnection();
            string testService();
            Task<User> Register(NewUserModel model);
        }

        public class UserService : IUserService
        {
            private readonly ModelsContext _context;
            // users hardcoded for simplicity, store in a db with hashed passwords in production applications
            private List<User> _users = new List<User>
            {
                new User { Id = 1, FirstName = "Test", LastName = "User", Username = "test", Password = "test" }
            };

            private readonly AppSettings _appSettings;

            public UserService(IOptions<AppSettings> appSettings, ModelsContext context)
            {
                _appSettings = appSettings.Value;
                _context = context;
            }
            public string testConnection()
            {
                try
                {
                    List<User> users = _context.Users.ToList<User>();
                    return "users count: " + users.Count;
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
            public string testService()
            {
                return "default users count" + _users.Count;
            }
            public async Task<User> Authenticate(string username, string password)
            {
                var user = _context.Users.SingleOrDefault(x => x.Username.ToLower().Equals(username.ToLower()));
                // return null if user not found
                if (user == null || user.Password == null)
                    return null;
                if (!Security.CompareHashedData(user.Password, password))
                    return null;

                // authentication successful so generate jwt token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                user.Token = tokenHandler.WriteToken(token);
                user.Password = null;
                return user;
            }
            public async Task<User> Register(NewUserModel model)
            {
                User user = new User()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Username = model.Username,
                    Email = model.Email,
                    Phone = model.Phone,
                };
                user.Password = Security.HashSensitiveData(model.Password);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                user = _context.Users.Where(u => u.Username.Equals(user.Username)).FirstOrDefault();
                
                //generate jwt token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                user.Token = tokenHandler.WriteToken(token);
                user.Password = null;

                return user;
            }

            public IEnumerable<User> GetAll()
            {
                return _users.WithoutPasswords();
            }
            private bool UserExists(int id)
            {
                return _context.Users.Any(e => e.Id == id);
            }
            public bool UsernameExist(string username)
            {
                return _context.Users.Any(e => e.Username == username);
            }
            public bool EmailExist(string email)
            {
                return _context.Users.Any(e => e.Email == email);
            }
        }
}
