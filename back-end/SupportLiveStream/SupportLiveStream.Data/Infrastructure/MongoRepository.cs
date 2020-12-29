using MongoDB.Bson;
using MongoDB.Driver;
using SupportLiveStream.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SupportLiveStream.Data
{
    public abstract class MongoRepository<TDocument> : IRepository<TDocument>
        where TDocument : IMongoModelBase, new()
    {
        protected readonly IMongoContext _mongoContext;
        protected readonly IMongoCollection<TDocument> _collection;

        public MongoRepository(IMongoContext context, string collectionName)
        {
            _mongoContext = context;
            try
            {
                if (!IsExistCollection(collectionName))
                {
                    _mongoContext.Database.CreateCollection(collectionName);
                }
                _collection = _mongoContext.Database.GetCollection<TDocument>(collectionName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task InsertOneAsync(TDocument obj)
        {
            try
            {
                await _collection.InsertOneAsync(obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task InsertManyAsync(List<TDocument> lst)
        {
            try
            {
                await _collection.InsertManyAsync(lst);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteOneAsync(FilterDefinition<TDocument> filter)
        {
            try
            {
                await _collection.DeleteOneAsync(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<TDocument>> FindAsync(Func<FilterDefinition<TDocument>> funcFilter = null, Func<SortDefinition<TDocument>> funcSort = null)
        {
            try
            {
                FilterDefinition<TDocument> filter;
                SortDefinition<TDocument> sort;
                if (funcFilter == null)
                {
                    filter = Builders<TDocument>.Filter.Empty;
                }
                else
                {
                    filter = funcFilter();
                }
                if(funcSort == null)
                {
                    sort = Builders<TDocument>.Sort.Descending("_id");
                }
                else
                {
                    sort = funcSort();
                }
                var res = await _collection.FindAsync<TDocument>(filter, new FindOptions<TDocument, TDocument>()
                {
                    Sort = sort
                });
                var data = res.ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateOneAsync(Func<FilterDefinition<TDocument>> funcFilter, Func<UpdateDefinition<TDocument>> funcUpdate)
        {
            try
            {
                var filter = funcFilter();
                var update = funcUpdate();
                await _collection.UpdateOneAsync(filter, update);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool IsExistCollection(string collectionName)
        {
            bool isExist = _mongoContext.Database.ListCollectionsAsync(new ListCollectionsOptions() { Filter = new BsonDocument("name", collectionName) }).Result.ToList().Count == 0 ? false : true;
            return isExist;
        }
    }
}
