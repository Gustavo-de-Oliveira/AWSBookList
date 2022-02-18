using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Amazon.Runtime;
using Amazon.DynamoDBv2.DocumentModel;
using AWSBookList.Model;

namespace AWSBookList.Database
{
    public class DynamoAbstract<T> where T : class, new() {
        public IDynamoDBContext context;
        private Amazon.DynamoDBv2.AmazonDynamoDBClient amazonDynamoDBClient { get; set; }
        public object DynamoDbClientService { get; private set; }

        protected DynamoAbstract() { 
            Credentials c = GetTemporaryCredentialsAsync().GetAwaiter().GetResult();

            amazonDynamoDBClient = new Amazon.DynamoDBv2.AmazonDynamoDBClient(c.AccessKeyId, c.SecretAccessKey,c.SessionToken, Amazon.RegionEndpoint.USEast1);

            context = new DynamoDBContext(amazonDynamoDBClient);
        }

        public static async Task<Credentials> GetTemporaryCredentialsAsync()
        {
            using (var stsClient = new AmazonSecurityTokenServiceClient())
            {
                var getSessionTokenRequest = new GetSessionTokenRequest
                {
                    DurationSeconds = 900 // seconds
                };

                GetSessionTokenResponse sessionTokenResponse =  await stsClient.GetSessionTokenAsync(getSessionTokenRequest);

                Credentials credentials = sessionTokenResponse.Credentials;

                var sessionCredentials =
                    new SessionAWSCredentials(credentials.AccessKeyId,
                                              credentials.SecretAccessKey,
                                              credentials.SessionToken);

                return credentials;
            }
        }

        protected async Task<List<T>> Scan() {

            return await context.ScanAsync<T>(new List<ScanCondition>()).GetRemainingAsync();
        }

        protected async Task<List<T>> QueryByHash(object hashKey)
        {
            return await context.QueryAsync<T>(hashKey).GetRemainingAsync();
        }
        protected async Task<List<Book>> QueryByAuthor(String author)
        {
            QueryRequest queryRequest = new QueryRequest
            {
                TableName = "BookTable",
                IndexName = "Author-index",
                KeyConditionExpression = "#Author = :v_aut",
                ExpressionAttributeNames = new Dictionary<String, String> {
                    {"#Author", "Author"}
                },
    
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    {":v_aut", new AttributeValue { S =  author }},
                },
                ScanIndexForward = true
            };

            var result = await amazonDynamoDBClient.QueryAsync(queryRequest);

            var items = result.Items;

            var listaLivros = new List<Book>();
            foreach (var currentItem in items)
            {
                Book currentBook = new Book();
                foreach (string attr in currentItem.Keys)
                {
                   if(attr == "Title") currentBook.Title = currentItem[attr].S;
                   else if(attr == "Author") currentBook.Author = currentItem[attr].S;
                   else if(attr == "IdBook") currentBook.IdBook = Guid.Parse(currentItem[attr].S);
                   else if(attr == "Genre") currentBook.Genre = currentItem[attr].S;
                }

                listaLivros.Add(currentBook);
            }

            return listaLivros;
        }
        protected async Task<List<T>> QueryByGenre(String genre)
        {
            List<ScanCondition> conditions = new List<ScanCondition>();
            conditions.Add(new ScanCondition("Genre", ScanOperator.Contains, genre));

            var response = await context.ScanAsync<T>(conditions).GetRemainingAsync();

            return response;
        }

        protected async Task<List<T>> QueryByTitle(String title)
        {
            List<ScanCondition> conditions = new List<ScanCondition>();
            conditions.Add(new ScanCondition("Title", ScanOperator.Contains, title));

            var response = await context.ScanAsync<T>(conditions).GetRemainingAsync();

            return response;
        }
        
        protected async Task Save(T obj) 
        {
            await context.SaveAsync<T>(obj);   
        }
        protected async Task Edit(T obj)
        {
            await context.SaveAsync<T>(obj);
        }
        protected async Task Delete(T obj)
        {
            await context.DeleteAsync<T>(obj);
        }
    }
}
