using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OperationsManager.Helpers;
using OperationsManager.Helpers;
using System.Text.Json.Nodes;

namespace OperationsManager.Database.Entities
{
    public class Module
    {
        public int ModuleType { get; set; }
        [BsonId]
        public string Id { get; set; }
        public string Data { get; set; }
        public DateTime Timestamp { get; set; }
        public string Checksum => ChecksumHelper.ComputeMD5(Data);
    }
}
