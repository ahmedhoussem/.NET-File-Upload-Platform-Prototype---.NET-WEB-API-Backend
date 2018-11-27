using MimeDetective;
using MimeDetective.Extensions;
using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Web_API.CustomResponse;

namespace Web_API.Controllers
{
    [RoutePrefix("api/Package")]
    [EnableCors("*", "*", "*")]
    public class PackageController : ApiController
    {
        private HttpRequestBase HttpRequest;

        private readonly FileService _FileService;
        private readonly PackageService _PackageService;
        private readonly UserService _UserService;

        public PackageController(FileService fs, PackageService ps, UserService us , HttpRequestBase Request)
        {
            HttpRequest = Request;

            _UserService = us;
            _FileService = fs;
            _PackageService = ps;
        }


        public PackageController()
        {
            _UserService = new UserService();
            _FileService = new FileService();
            _PackageService = new PackageService();
            HttpRequest = new HttpRequestWrapper(HttpContext.Current.Request);
        }

        const string PDF_MIME = "application/pdf";


        // GET api/<controller>/5
        [HttpGet]
        [Route("PerUser")]
        public async Task<ResponseBase> GetPackagesPerUser()
        {
            var Token = HttpRequest.Headers["Authorization"];


            var _PackageService = new PackageService();

            return new SuccessResponse { Data = await _PackageService.GetPackagesPerUserWithDetails(Token) };
        }

        public async Task<Package> Get(int id)
        {

            return await _PackageService.GetById(id);
        }

        [HttpGet]
        [Route("GetPackageDetails/{id}")]
        public async Task<object> GetPackageWithDetails(int id)
        {
            return await _PackageService.GetPackageWithDetails(id);
        }
        // POST api/<controller>
        [HttpPost]
        public ResponseBase Post()
        {
            //a flag used to determine if the upload was successul or not
            bool isValid = true;

           

            string PackageName = HttpRequest.Form.Get("Name");

            // Check if name exists
            if (string.IsNullOrEmpty(PackageName))
            {
                return new FailedResponse { Data = "Please provide a package name" };
            }

            var Files = HttpRequest.Files;

            // Check for file count
            if (Files.Count < 1)
            {
                return new FailedResponse { Data = "Please add atleast one file" };
            }

            // Check for Size and MIME type
            for (int i = 0; i < Files.Count; i++)
            {
                if (Files[i].ContentLength == 0)
                {
                    return new FailedResponse { Data = "One of the files uploaded seems to be corrupted" };
                }

                if (Files[i].ContentLength > 1000000)
                {
                    return new FailedResponse { Data = "One of the files uploaded exceeded the size limit of 1 MB per file" };
                }
                if (Files[0].ContentType != PDF_MIME)
                {
                    return new FailedResponse { Data = "One of the files uploaded is not a PDF file" };
                }
            }

            //
            var ServerPath = Path.Combine(HttpRequest.PhysicalApplicationPath, "Files");

            // Generate unique filnames based on timestamp and index to avoid file collision in the upload batch 
            // and keeping the original filename's for the download

            var files = Enumerable
                    .Range(0, Files.Count)
                    .Select(i =>
                    {
                        return new { FileData = Files[i], FileGeneratedName = Files[i].FileName.Replace(".pdf", "_" + i + "_" + DateTime.Now.Ticks + ".pdf") };
                    });


            var Token = HttpRequest.Headers.Get("Authorization");
            
            var User = _UserService.TokenToUser(Token);


            var CurrentPackage = _PackageService.Insert(new Package { Name = PackageName, UserId = User.Id, CreationTime = DateTime.Now });


            //A list to store the treated file for clean up incase of interruption
            List<Models.File> ListOfTreatedFiles = new List<Models.File>();



            // Check if path exists and create it in case it doesnt exist
            if (!Directory.Exists(ServerPath))
            {
                Directory.CreateDirectory(ServerPath);
            }


            //Use the TPL library to make the saving to disk more efficient
            Parallel.ForEach(files, (file) =>
            {
                try
                {
                    var TemprorairyFile = new Models.File { Filename = file.FileData.FileName, GeneratedFilename = file.FileGeneratedName, PackageId = CurrentPackage.Id };

                        //Add to list then add to DB
                        ListOfTreatedFiles.Add(TemprorairyFile);
                    _FileService.Insert(TemprorairyFile);

                        //Save the file to disk
                        file.FileData.SaveAs(Path.Combine(ServerPath, file.FileGeneratedName));
                }

                // Clean up in case of Error
                catch
                {
                        //Delete the files from DB
                        Parallel.ForEach(ListOfTreatedFiles, (f) =>
                    {
                        _FileService.Delete(f);
                    });

                        //Delete files from disk
                        Parallel.ForEach(files, (f) =>
                    {

                        if (System.IO.File.Exists(Path.Combine(ServerPath, f.FileGeneratedName)))
                        {
                            System.IO.File.Delete(Path.Combine(ServerPath, f.FileGeneratedName));

                        }
                    });

                    _PackageService.Delete(CurrentPackage);

                    isValid = false;
                }


            });

            //Ckeck if the upload was successful or not
            if (isValid)
                return new SuccessResponse { Data = new { Message = "File(s) uploaded successfully", PackageId = CurrentPackage.Id } };

            else
                return new FailedResponse { Data = "An occured while writing file to the disk " };

        }





    }
}