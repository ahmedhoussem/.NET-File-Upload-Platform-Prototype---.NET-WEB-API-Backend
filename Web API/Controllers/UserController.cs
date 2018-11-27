using Dapper;
using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Web_API.CustomResponse;

namespace Web_API.Controllers
{
    [RoutePrefix("api/User")]
    [EnableCors("*", "*", "*")]
    public class UserController : ApiController
    {
        private IDbConnection db;

        public UserController()
        {
            db = new SqlConnection(Utils.GetConnectionString());
        }

        public UserController(IDbConnection db)
        {
            this.db = db;
        }

        // GET api/<controller>
        public async Task<IEnumerable<User>> Get()
        {
            var UserController = new UserService();

            return await UserController.GetAll();
        }

        // GET api/<controller>/5
        public async Task<ResponseBase> Get(int id)
        {
            var UserController = new UserService();


            var result = await UserController.GetById(id);

            if (result != null)
            {
                return new SuccessResponse { Data = result };
            }

            return new FailedResponse { Data = "User doesn't exist" };
        }

        


        [HttpPost]
        [Route("Login")]
        public async Task<ResponseBase> CheckUser(User User)
        {

            var UserController = new UserService();

            var result = await UserController.GetTokenFromUser(User);

            if (result != null)
            {
                return new SuccessResponse { Data = new { Token = result } };
            }

            return new FailedResponse { Data = "Invalid credentials , please try again"};
        }

        // POST api/<controller>
        public async Task<ResponseBase> Post(User User)
        {

            var UserController = new UserService();

            var result = await UserController.InsertAsyc(User);

            if (result != null)
            {
                return new SuccessResponse { Data = "User has been added successfully" };
            }

            return new FailedResponse { Data = "Username already exists , please try again" };
        }

    }
}