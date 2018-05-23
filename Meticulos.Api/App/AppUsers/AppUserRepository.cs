using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meticulos.Api.App.AppUsers
{
    public class AppUserRepository : IAppUserRepository
    {
        private readonly AppUserContext _context = null;

        public AppUserRepository(IOptions<Settings> settings)
        {
            _context = new AppUserContext(settings);
        }

        public async Task<AppUser> Get(ObjectId id)
        {
            var filter = Builders<AppUser>.Filter.Eq("_id", id);

            try
            {
                return await _context.AppUsers.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public async Task<AppUser> Authenticate(AppUser user)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(user.Email)
        //            || string.IsNullOrEmpty(user.Password))
        //            throw new ApplicationException("Invalid credentials provided.");

        //        List<FilterDefinition<AppUser>> filters = new List<FilterDefinition<AppUser>>();
        //        filters.Add(Builders<AppUser>.Filter.Eq("EmailAddress", new ObjectId(user.Email)));
        //        filters.Add(Builders<AppUser>.Filter.Eq("Password", new ObjectId(user.Password)));

        //        var filterConcat = Builders<AppUser>.Filter.And(filters);
        //        var findResults = await _context.AppUsers.Find(filterConcat).ToListAsync();

        //        if (findResults == null || findResults.Count < 1)
        //            throw new ApplicationException("Invalid credentials provided.");

        //        return findResults.FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public async Task<List<AppUser>> Search(AppUserSearchRequest request)
        {
            try
            {
                List<FilterDefinition<AppUser>> filters = new List<FilterDefinition<AppUser>>();
                if (!string.IsNullOrEmpty(request.EmailAddress))
                    filters.Add(Builders<AppUser>.Filter.Eq("EmailAddress", new ObjectId(request.EmailAddress)));
            
                var filterConcat = Builders<AppUser>.Filter.And(filters);
                return await _context.AppUsers.Find(filterConcat).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public async Task<AppUser> Add(AppUser user)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(user.Email)
        //            || string.IsNullOrEmpty(user.Password))
        //            throw new ApplicationException("Required fields not supplied.");

        //        if (string.IsNullOrEmpty(user.Name))
        //            user.Name = user.Email;

        //        var searchResult = await Search(new AppUserSearchRequest() { EmailAddress = user.Email });
        //        if (searchResult.Count > 0)
        //            throw new ApplicationException("User account already exists with that email address.");

        //        await _context.AppUsers.InsertOneAsync(user);
        //        return await Get(user.Id);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public async Task<AppUser> Update(AppUser user)
        {
            var filter = Builders<AppUser>.Filter.Eq("Id", user.Id);

            try
            {
                await _context.AppUsers.ReplaceOneAsync(filter, user);
                return await Get(user.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Delete(ObjectId id)
        {
            var filter = Builders<AppUser>.Filter.Eq("Id", id);

            try
            {
                await _context.AppUsers.DeleteOneAsync(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
