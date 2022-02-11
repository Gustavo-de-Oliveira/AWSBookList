using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace AWSBookList.Database
{
    public class DynamoAbstract<T> where T : class, new() {
        private readonly IDynamoDBContext context;
        private Amazon.DynamoDBv2.AmazonDynamoDBClient amazonDynamoDBClient { get; set; }
        protected DynamoAbstract() { 

            amazonDynamoDBClient = new Amazon.DynamoDBv2.AmazonDynamoDBClient(awsAccessKeyId: "AKIASCBEI6XRO2VRL2UE", awsSecretAccessKey: "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", Amazon.RegionEndpoint.USEast1);

            context = new DynamoDBContext(amazonDynamoDBClient);
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
