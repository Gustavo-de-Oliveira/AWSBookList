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

namespace AWSBookList.Database
{
    public class DynamoAbstract<T> where T : class, new() {
        public IDynamoDBContext context;
        private Amazon.DynamoDBv2.AmazonDynamoDBClient amazonDynamoDBClient { get; set; }
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
