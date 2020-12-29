using MongoDB.Driver;
using SupportLiveStream.Model;
using System;
using System.Threading.Tasks;

namespace SupportLiveStream.Data
{
    public interface IPageRepository : IRepository<PageModel>
    {

    }

    public class PageRepository : MongoRepository<PageModel>, IPageRepository
    {
        public PageRepository(IMongoContext context) : base(context, "PAGE")
        {

        }

        //public async Task<IEnumerable<PagesModel>> GetByQueryAsync(string filter)
        //{
        //    try
        //    {
        //        //var bQuery = "{'Videos.Comments.FbId': '1', 'PageId': '3', 'Videos.IsLive': true}";
        //        var data = await _collection.FindAsync(filter);
        //        return data.ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public override async Task<bool> UpdateAsync(string strPageId, PagesModel obj)
        //{
        //    try
        //    {
        //        var arrayFilter = Builders<PagesModel>.Filter.Eq("PageId", strPageId) & Builders<PagesModel>.Filter.Eq("scores.type", "quiz");
        //        var arrayUpdate = Builders<PagesModel>.Update.Set("scores.$.score", 84.92381029342834);
        //        var data = await _collection.UpdateOneAsync(arrayFilter, arrayUpdate);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public async Task<bool> AddOrUpdateAsync(string filter, PagesModel model)
        //{
        //    try
        //    {
        //        var lstFilterd = await GetByQueryAsync(filter);
        //        PagesModel pagesModel = lstFilterd.FirstOrDefault();
        //        if (pagesModel == null)
        //        {
        //            await AddAsync(model);
        //        }
        //        else
        //        {
        //            await UpdateAsync(pagesModel.PageId, model);
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
    }
}
