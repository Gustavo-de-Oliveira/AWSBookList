using Amazon.DynamoDBv2.DataModel;
using System;

namespace AWSBookList.Model
{ 
    [DynamoDBTable("BookTable")]
    public class Book
    {
        [DynamoDBHashKey]

        public Guid IdBook { get; set; }

        [DynamoDBProperty]
        public string Author { get; set; }
        public string Genre { get; set; }
        public string Title { get; set; }
 
    }
}
