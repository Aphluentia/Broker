
using OperationsManager.Database;
using OperationsManager.Models.Enums;
using System.ComponentModel;
using Newtonsoft.Json;
using OperationsManager.Models.BrokerMessageDataField.Users;
using OperationsManager.Models.BrokerMessageDataField.Modules;
using OperationsManager.Database.Entities;

namespace OperationsManager.BackgroundServices.Workers
{
    public class TMessageHandler
    {
        private BackgroundWorker _worker;
        private bool _StopFlag;
        private readonly IKafkaMessageHandler _kafkaMonitor;
        private readonly IDatabaseProvider _database;
        public TMessageHandler(KafkaQueue mKafka, IDatabaseProvider database)
        {
            _kafkaMonitor = mKafka;
            _database = database;
            _StopFlag = false;
            _worker = new BackgroundWorker();
            _worker.DoWork += RunAsync;
        }

        public void Start()
        {
            _worker.RunWorkerAsync();
        }

        private async void RunAsync(object sender, DoWorkEventArgs e)
        {
            while (!_StopFlag)
            {
                var message = this._kafkaMonitor.FetchIncomingMessage();
                
                if (message != null)
                {
                    
                    switch (message.OperationCode)
                    {
                        case Operation.CREATE_USER:
                            var UserToCreate = JsonConvert.DeserializeObject<CreateUser>(message.Data.ToString());
                            if (UserToCreate!= null) await _database.Post(UserToCreate, DatabaseControllers.User);
                            break;

                        case Operation.UPDATE_USER:
                            var UserToUpdate = JsonConvert.DeserializeObject<UpdateUser>(message.Data.ToString());
                            if (UserToUpdate != null) await _database.Put(UserToUpdate.User, DatabaseControllers.User, UserToUpdate.Email);
                            break;

                        case Operation.DELETE_USER:
                            var UserToDelete = JsonConvert.DeserializeObject<DeleteUser>(message.Data.ToString());
                            if (UserToDelete != null) await _database.Delete(DatabaseControllers.User, UserToDelete.Email);
                            break;

                        case Operation.CREATE_MODULE:
                            var ModuleToCreate = JsonConvert.DeserializeObject<CreateModule>(message.Data.ToString());
                            if (ModuleToCreate != null) await _database.Post(ModuleToCreate, DatabaseControllers.Modules);
                            break;

                        case Operation.UPDATE_MODULE:
                            var ModuleToUpdate = JsonConvert.DeserializeObject<UpdateModule>(message.Data.ToString());
                            if (ModuleToUpdate != null) await _database.Put(ModuleToUpdate.Module, DatabaseControllers.User, ModuleToUpdate.ModuleId);
                            break;

                        case Operation.DELETE_MODULE:
                            var ModuleToDelete = JsonConvert.DeserializeObject<DeleteModule>(message.Data.ToString());
                            if (ModuleToDelete != null) await _database.Delete(DatabaseControllers.Modules, ModuleToDelete.ModuleId);
                            break;

                        case Operation.CREATE_CONNECTION:
                            var ConnectionToCreate = JsonConvert.DeserializeObject<ModuleConnection>(message.Data.ToString());
                            if (ConnectionToCreate != null) await _database.Post(ConnectionToCreate, DatabaseControllers.User, null, true);
                            break;

                        case Operation.DELETE_CONNECTION:
                            var ConnectionToDelete = JsonConvert.DeserializeObject<ModuleConnection>(message.Data.ToString());
                            if (ConnectionToDelete != null) await _database.Delete(DatabaseControllers.User, ConnectionToDelete.WebPlatformId,true, ConnectionToDelete.ModuleId );
                            break;

                        default:
                            // Handle any other cases or throw an exception
                            throw new ArgumentOutOfRangeException(nameof(message.OperationCode), message.OperationCode, "Invalid operation");
                    }

                    
                    
                }
                

            }
            

        }

      
    }
}
