using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class FileService : IRepository<Models.File>
    {
        private IDbConnection db;

        public FileService()
        {
            db = new SqlConnection(Utils.GetConnectionString());
        }

        public FileService(IDbConnection db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<Models.File>> GetAll()
        {
            return await db.GetListAsync<Models.File>();
        }

        public async Task<KeyValuePair<string, FileStream>> SendPDFData(string ServerPath, int id)
        {
            var FileInfo = await db.GetAsync<Models.File>(id);

            var path = Path.Combine(ServerPath, FileInfo.GeneratedFilename);
            var fileStream = System.IO.File.OpenRead(path);
            long fileLength = fileStream.Length;

            return new KeyValuePair<string, FileStream>(FileInfo.Filename, System.IO.File.OpenRead(path));

        }

        public async Task<Models.File> GetById(int id)
        {
            return await db.GetAsync<Models.File>(id);
        }

        public async Task<IEnumerable<Models.File>> GetFilesPerPackage(int Package_id)
        {

            return await db.GetListAsync<Models.File>("where PackageId = @PackageId", new { PackageId = Package_id });

        }

        public virtual async Task<Models.File> InsertAsyc(Models.File File)
        {
            await db.InsertAsync(File);

            return File;

        }


        public async Task<int> DeleteAsync(Models.File File)
        {

            return await db.DeleteAsync(File);

        }

        public virtual int Delete(Models.File File)
        {
            // Using a separate disposable DB connection since its going to be used in parallel and DB connection isn't thread-safe
            using (var dbCopy = new SqlConnection(db.ConnectionString))
            { 
                return dbCopy.Delete(File);
            }
        }

        public virtual Models.File Insert(Models.File t)
        {
            // Using a separate disposable DB connection since its going to be used in parallel and DB connection isn't thread-safe
            using (var dbCopy = new SqlConnection(db.ConnectionString))
            {
                dbCopy.Insert(t);

                return t;
            }
        }
    }
}
