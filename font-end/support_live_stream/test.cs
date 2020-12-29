public async Task UpdateOneAsync(Func<FilterDefinition<PageModel>> funcFilter, Func<UpdateDefinition<PageModel>> funcUpdate)
{
    await _pageRepository.UpdateOneAsync(funcFilter, funcUpdate);
}