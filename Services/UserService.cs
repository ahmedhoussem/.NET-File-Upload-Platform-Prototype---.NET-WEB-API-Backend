using Dapper;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserService : IRepository<User>
    {
        private IDbConnection db;

        public UserService()
        {
            db = new SqlConnection(Utils.GetConnectionString());
        }

        public UserService(IDbConnection db)
        {
            this.db = db;
        }


        public async Task<IEnumerable<User>> GetAll()
        {

            var Result = await db.GetListAsync<User>();

            return Result;
        }

        public async Task<User> GetById(int id)
        {
            var Result = await db.GetAsync<User>(id);

            return Result;
        }

        ////GetUser() Overload : Check with Username and Password
        public async Task<User> GetUser(User User)
        {
            User.Password = Utils.HashPassword(User.Password);

            var Result = await db.GetListAsync<User>("where Username = @Username and Password = @Password ",
                new { Username = User.Username, Password = User.Password });

            if (Result.FirstOrDefault() == null)
            {
                return null;
            }

            return Result.FirstOrDefault();

        }

        public async Task<string> GetTokenFromUser(User User)
        {

            User.Password = Utils.HashPassword(User.Password);

            var Result = await db.GetListAsync<User>("where Username = @Username and Password = @Password ",
                new { Username = User.Username, Password = User.Password });

            if (Result.FirstOrDefault() == null)
            {
                return null;
            }

            return UserToToken(Result.FirstOrDefault());
        }

        public async Task<User> InsertAsyc(User User)
        {
            var res = await db.GetListAsync<User>("where Username = @Username", new { Username = User.Username });

            if (res.Count() == 0)
            {
                //Hash the password before adding to DB
                User.Password = Utils.HashPassword(User.Password);

                var id = await db.InsertAsync(new User { Password = User.Password, Username = User.Username });

                User.Id = id.Value;

                return User;
            }


            return null;
        }

        public virtual User TokenToUser(string token)
        {

            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

                var json = decoder.DecodeToObject<IDictionary<string, string>>(token, Utils.GetJWTSecret(), verify: false);

                var User = new User { Username = json["Username"], Password = json["Password"] };

                return db.GetList<User>("where Username = @Username and Password = @Password ", new { Username = json["Username"], Password = json["Password"] }).FirstOrDefault();
            }
            catch (SignatureVerificationException)
            {
                return null;
            }
        }

        private string UserToToken(User u)
        {
            var payload = new Dictionary<string, object>
            {
                { "Username", u.Username },
                { "Password", u.Password }
            };

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            return encoder.Encode(payload, Utils.GetJWTSecret());
        }

        public async Task<int> DeleteAsync(User User)
        {
            return await db.DeleteAsync(User);
        }

        public User Insert(User t)
        {
            var res = db.GetList<User>("where Username = @Username", new { Username = t.Username });

            if(res.Count() == 0)
            {
                db.Insert(t);
                return t;
            }

            return null;
            
        }

        public virtual int Delete(User t)
        {
            return db.Delete(t);
        }
    }
}
