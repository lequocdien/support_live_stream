using MongoDB.Driver;
using SupportLiveStream.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupportLiveStream.Data
{
    public interface IRepository<TDocument> where TDocument : IMongoModelBase
    {
        Task InsertOneAsync(TDocument obj);
        Task InsertManyAsync(List<TDocument> lst);
        Task DeleteOneAsync(FilterDefinition<TDocument> filter);
        Task<IEnumerable<TDocument>> FindAsync(Func<FilterDefinition<TDocument>> funcFilter = null, Func<SortDefinition<TDocument>> funcSort = null);
        Task UpdateOneAsync(Func<FilterDefinition<TDocument>> funcFilter, Func<UpdateDefinition<TDocument>> funcUpdate);
        bool IsExistCollection(string collectionName);
    }
}
