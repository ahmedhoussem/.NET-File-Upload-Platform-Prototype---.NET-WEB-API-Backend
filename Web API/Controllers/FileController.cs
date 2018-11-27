using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Web_API.Controllers
{
    [RoutePrefix("api/File")]
    [EnableCors("*", "*", "*")]
    public class FileController : ApiController
    {
        private IDbConnection db;

        public FileController()
        {
            db = new SqlConnection(Utils.GetConnectionString());
        }

        public FileController(IDbConnection db)
        {
            this.db = db;
        }

        // GET api/<controller>
        [HttpGet]
        public async Task<IEnumerable<Models.File>> Get(int id)
        {
            var _FileService = new FileService(db);

            return await _FileService.GetFilesPerPackage(id);
        }

        [HttpGet]
        [Route("Download/{id}")]
        [EnableCors("*","*","*")]
        public async Task<HttpResponseMessage> SendFileStream([FromUri] int id)
        {
            try
            {
                var _FileService = new FileService();

                var FileInfo = await _FileService.SendPDFData(HttpContext.Current.Server.MapPath("~/Files"), id);

                var response = new HttpResponseMessage();
                response.Content = new StreamContent(FileInfo.Value);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = FileInfo.Key ;
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                response.Content.Headers.ContentLength = FileInfo.Value.Length;

                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
    
}
