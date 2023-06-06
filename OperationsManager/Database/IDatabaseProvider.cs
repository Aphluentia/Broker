namespace OperationsManager.Database
{
    public interface IDatabaseProvider
    {

        public Task<(bool, string?)> Get(DatabaseControllers controller, string? id = null, bool isModule = false);
        public Task<(bool, string?)> Post(object body, DatabaseControllers controller, string? id = null, bool isModule = false);
        public Task<(bool, string?)> Put(object body,DatabaseControllers controller, string? id = null, bool isModule = false);
        public Task<(bool, string?)> Delete(DatabaseControllers controller, string? id = null, bool isModule = false, string? ModuleId = null);
    }
}
