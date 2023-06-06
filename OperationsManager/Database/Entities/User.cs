using MongoDB.Bson.Serialization.Attributes;

namespace OperationsManager.Database.Entities
{
    public class User
    {
        [BsonId]
        public string Email { get; set; }
        public Guid WebPlatformId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public ISet<string> Modules { get; set; }
        public ISet<string> ActiveScenarios { get; set; }
        public int PermissionLevel { get; set; }


    }
}
