using OperationsApi;
using System.Text.Json.Nodes;

namespace OperationsApi.Database.Entities
{
    public class Module
    {
        public int ModuleType { get; set; }
        public string Id { get; set; }
        public string Data { get; set; }
    }
}
