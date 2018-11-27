using Dapper;
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
    public class PackageService : IRepository<Package>
    {
        private IDbConnection db;

        public PackageService()
        {
            db = new SqlConnection(Utils.GetConnectionString());
        }

        public PackageService(IDbConnection db)
        {
            this.db = db;
        }



        public async Task<IEnumerable<Package>> GetAll()
        {
            return await db.GetListAsync<Package>();
        }

        public async Task<IEnumerable<object>> GetPackagesPerUserWithDetails(string Token)
        {

            var us = new UserService();
            var userDecoded = us.TokenToUser(Token);

            string SearchQuery = "SELECT p.Id , p.Name , p.CreationTime , (SELECT COUNT(*) FROM Files f JOIN Packages pac ON f.PackageId = pac.Id WHERE pac.Id = p.Id) AS FilesCount FROM Packages p WHERE p.UserId = @User_ID";
            return await db.QueryAsync(SearchQuery, new { User_ID = userDecoded.Id });

        }

        public async Task<object> GetPackageWithDetails(int id)
        {

            string SearchQuery = "SELECT p.Id , p.Name , p.CreationTime , (SELECT COUNT(*) FROM Files f JOIN Packages pac ON f.PackageId = pac.Id WHERE pac.Id = p.Id) AS FilesCount FROM Packages p WHERE p.Id = @Id";
            return await db.QuerySingleAsync<object>(SearchQuery, new { Id = id });

        }

        public async Task<IEnumerable<Package>> GetPackagesPerUser(int User_id)
        {
            return await db.GetListAsync<Package>("where UserId = @UserId", new { UserId = User_id });
        }

        public async Task<Package> GetById(int id)
        {
            return await db.GetAsync<Package>(id);
        }

        public virtual Package Insert(Package Package)
        {
            var res = db.Insert(Package);
            if (res.HasValue == false)
                return null;
            Package.Id = res.Value;
            return Package;
        }

        public virtual async Task<Package> InsertAsyc(Package Package)
        {

            var id = await db.InsertAsync(Package);

            if (id == null)
            {
                return null;
            }

            Package.Id = id.Value;

            return Package;

        }


        public async Task<int> DeleteAsync(Package Package)
        {
            return await db.DeleteAsync(Package);
        }

        public virtual int Delete(Package Package)
        {
            return db.Delete(Package);
        }
    }
}
