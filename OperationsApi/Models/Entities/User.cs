

namespace OperationsApi.Database.Entities
{
    public class User
    {
        public string Email { get; set; }
        public Guid WebPlatformId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public ISet<string> Modules { get; set; }
        public ISet<string> ActiveScenarios { get; set; }
        public int PermissionLevel { get; set; }


    }
}
