using Microsoft.EntityFrameworkCore;
using OSS.Data.Entities;
using OSS.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using LinqKit;
//using System.Linq.Dynamic.Core;

namespace OSS.Services.DomainServices
{
    public class TestTableService : ITestTableService
    {
        private readonly ApplicationDbContext _ctx;
        public TestTableService(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<int> GetTotal(Func<TestTableModel, bool> where)
        {
            return await _ctx.TestTable.CountAsync();
        }
        public async Task<IEnumerable<TestTableModel>> PrepareModeListAsync(Func<TestTable, bool>? where)
        {
            var query = _ctx.TestTable.AsNoTracking();
            if (where != null) query = query.Where(where).AsQueryable();
            return await query.Select(x => x.ToModel<TestTableModel>()).ToListAsync();
        }

        public async Task<IList<TestTableModel>> PrepareModePagedList(Dictionary<string, string> param)
        {
            int start = 0;
            int pageSize = 5;
            string orderBy = "";
            string orderDir = "asc";
            var query = _ctx.TestTable.AsQueryable().AsNoTracking();
            if (param != null && param.Any())
            {
                if (param.Any(x=> x.Key == "start"))
                {
                    var aaa = param.FirstOrDefault(x => x.Key == "start");
                     _ = int.TryParse(aaa.Value, out start);
                }

                if (param.Any(x => x.Key == "length"))
                {
                    var aaa = param.FirstOrDefault(x => x.Key == "length");
                    _ = int.TryParse(aaa.Value, out pageSize);
                }
                if (param.Any(x=> x.Key == "order[0][column]"))
                {
                    var aaa = param.FirstOrDefault(x => x.Key == "order[0][column]");
                    if (aaa.Value != "0")
                    {
                        orderBy = aaa.Value;
                        if (param.Any(x=> x.Key == "order[0][dir]"))
                            orderDir = param.FirstOrDefault(x => x.Key == "order[0][dir]").Value;
                    }
                }
                if (param.Any(x=> x.Key == "search[value]"))
                {
                    var aaa = param.FirstOrDefault(x => x.Key == "search[value]").Value;
                    if (!string.IsNullOrEmpty(aaa)) query = query.Where(x => x.Text1.Contains(aaa) || x.Text2.Contains(aaa));
                }
            }

            //query = query.Skip(start).Take(pageSize);
            if (!string.IsNullOrEmpty(orderBy))
            {
                if (orderDir == "asc") query = query.OrderBy(x => x.Text1);
                else query = query.OrderByDescending(x => x.Text1);
            }
            return await query.Select(x=> x.ToModel<TestTableModel>()).ToListAsync();

            //--------------------------------------------------- 
            ///
            /// write your expression
            /// 
            //Expression<Func<TestTable, bool>> whereClause;
            //if (param.ContainsKey("Test1")) whereClause = (w => w.Text1.Contains(param["Test1"]));
            //if (param.ContainsKey("Test2")) whereClause = (w => w.Text1.Contains(param["Test2"]));
            //if (whereClause != null) query = query.Where(whereClause);
            //-----------------------------------
            ///
            /// LinqKit package
            /// 
            //Expression<Func<TestTable, bool>>? expr = PredicateBuilder.New<TestTable>(false);
            //var original = expr;
            //if (param.ContainsKey("Test1"))
            //{
            //    expr = (x => x.Text1.Contains(param["Test1"]));
            //}
            //if (param.ContainsKey("Test2"))
            //{
            //    expr = expr.Or(x => x.Text1.Contains(param["Test2"]));
            //}
            //-----------------------------------------------
            ///
            ///using dynamci linq package
            //query = query.Where("text1=1");
        }

        private async Task<TestTable> GetEntity(string encryptedId)
        {
            //TODO: add data protection encrypt and decrypt
            int Id = Int32.Parse(encryptedId);
            return await _ctx.TestTable.FindAsync(Id);
        }
        public async Task<TestTableModel> PrepareMode(string encryptedId)
        {

            if (string.IsNullOrEmpty(encryptedId)) return null;
            var entity = await GetEntity(encryptedId);
            //TODO: add auto mapper to use ToModel function
            if (entity != null)
            {
                return entity.ToModel<TestTableModel>();
            }
            return null;
        }

        public async Task<TestTableModel> Save(TestTableModel model)
        {

            if (model == null) return null;
            TestTable entity = null;
            if (string.IsNullOrEmpty(model.EncrypedId))
            {
                entity = new TestTable();
            }
            else
            {
                entity = await GetEntity(model.EncrypedId);
            }
            if (entity == null) throw new NullReferenceException(nameof(entity));

            entity.Text1 = model.Text1;
            entity.Text2 = model.Text2;

            if (string.IsNullOrEmpty(model.EncrypedId))
            {
                _ctx.TestTable.Add(entity);
                model.EncrypedId = entity.Id.ToString();
            }
            else
            {
                _ctx.TestTable.Update(entity);
            }
            await _ctx.SaveChangesAsync();
            return model;

        }
        public async Task Delete(string encryptedId)
        {
            var entity = await GetEntity(encryptedId);
            _ctx.TestTable.Remove(entity);
            await _ctx.SaveChangesAsync();
        }

    }
}


/*
 DataTable returns query string contains 

draw-->1
columns[0][data]-->Text1
columns[0][name]-->
columns[0][searchable]-->true
columns[0][orderable]-->true
columns[0][search][value]-->
columns[0][search][regex]-->false
columns[1][data]-->Text2
columns[1][name]-->
columns[1][searchable]-->true
columns[1][orderable]-->true
columns[1][search][value]-->
columns[1][search][regex]-->false
order[0][column]-->0
order[0][dir]-->asc
start-->0
length-->5
search[value]-->
search[regex]-->false
_-->1608513197222
 
 */