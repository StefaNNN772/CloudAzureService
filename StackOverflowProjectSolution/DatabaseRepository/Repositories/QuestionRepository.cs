using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseRepository.Repositories
{
    public class QuestionRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;
        public QuestionRepository()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(
                new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("QuestionTable");
            _table.CreateIfNotExists();
        }
        public IQueryable<Question> RetrieveAllQuestions()
        {
            var results = from g in _table.CreateQuery<Question>()
                          where g.PartitionKey == "Question"
                          select g;
            return results;
        }
        public void AddQuestion(Question question)
        {
            // Samostalni rad: izmestiti tableName u konfiguraciju servisa.
            TableOperation insertOperation = TableOperation.Insert(question);
            _table.Execute(insertOperation);
        }

        public void UpdateQuestion(Question question)
        {
            try
            {
                TableOperation updateOperation = TableOperation.Replace(question);
                _table.Execute(updateOperation);
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == 412)
                {
                    System.Diagnostics.Debug.WriteLine($"Concurrent update detected for question {question.RowKey}");
                    throw new Exception("Someone else modified this question. Please refresh and try again.", ex);
                }
                throw new Exception("Error updating question: " + ex.Message, ex);
            }
        }

        public Question GetQuestionByRowKey(string rowKey)
        {
            var result = (from g in _table.CreateQuery<Question>()
                          where g.PartitionKey == "Question" && g.RowKey == rowKey
                          select g).FirstOrDefault();
            return result;
        }

        public void DeleteQuestion(string rowKey)
        {
            try
            {
                var question = GetQuestionByRowKey(rowKey);
                TableOperation deleteOperation = TableOperation.Delete(question);
                _table.Execute(deleteOperation);
            }
            catch (StorageException ex)
            {
                throw new Exception("Error deleting question: " + ex.Message, ex);
            }
        }
    }
}
