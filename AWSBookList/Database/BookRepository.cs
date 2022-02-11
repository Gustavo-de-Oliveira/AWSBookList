using Amazon.DynamoDBv2.DataModel;
using AWSBookList.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AWSBookList.Database
{
    public class BookRepository : DynamoAbstract<Book>
    {
        public BookRepository() : base()
        {
        }

        public async Task<List<Book>> GetAll() => await Scan();
        public async Task<List<Book>> GetById(Guid id) => await this.QueryByHash(id);
        
        public new async Task Save(Book obj) => await base.Save(obj);
        public new async Task Edit(Book obj) => await base.Edit(obj);

        public new async Task Delete(Book obj) => await base.Delete(obj);
    }
}
