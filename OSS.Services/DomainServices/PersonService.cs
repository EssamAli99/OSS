using Microsoft.EntityFrameworkCore;
using OSS.Data.Entities;
using OSS.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSS.Services.DomainServices
{
    public class PersonService : IPersonService
    {
        private readonly ApplicationDbContext _ctx;
        public PersonService(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<int> GetTotal(Func<PersonModel, bool> where)
        {
            return await _ctx.Person.CountAsync();
        }
        public async Task<IEnumerable<PersonModel>> PrepareModeListAsync(Func<Person, bool>? where)
        {
            var query = _ctx.Person.AsNoTracking();
            if (where != null) query = query.Where(where).AsQueryable();
            return await query.Select(x => new PersonModel
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                EncrypedId = x.Id.ToString()
            }).ToListAsync();
        }

        public IList<PersonModel> PrepareModePagedList(IEnumerable<KeyValuePair<string, string>> param)
        {
            int start = 0;
            int pageSize = 5;
            string orderBy = "";
            string orderDir = "asc";
            var query = _ctx.Person.AsQueryable().AsNoTracking();
            if (param != null && param.Any())
            {
                if (param.Any(x => x.Key == "start"))
                {
                    var aaa = param.FirstOrDefault(x => x.Key == "start");
                    _ = int.TryParse(aaa.Value, out start);
                }

                if (param.Any(x => x.Key == "length"))
                {
                    var aaa = param.FirstOrDefault(x => x.Key == "length");
                    _ = int.TryParse(aaa.Value, out pageSize);
                }
                if (param.Any(x => x.Key == "order[0][column]"))
                {
                    var aaa = param.FirstOrDefault(x => x.Key == "order[0][column]");
                    if (aaa.Value != "0")
                    {
                        orderBy = aaa.Value;
                        if (param.Any(x => x.Key == "order[0][column]"))
                            orderDir = param.FirstOrDefault(x => x.Key == "order[0][dir]").Value;
                    }
                }
                if (param.Any(x => x.Key == "search[value]"))
                {
                    var aaa = param.FirstOrDefault(x => x.Key == "search[value]").Value;
                    if (!string.IsNullOrEmpty(aaa)) query = query.Where(x => x.FirstName.Contains(aaa) || x.LastName.Contains(aaa));
                }
            }

            //query = query.Skip(start).Take(pageSize);
            if (!string.IsNullOrEmpty(orderBy))
            {
                if (orderDir == "asc") query = query.OrderBy(x => x.FirstName);
                else query = query.OrderByDescending(x => x.LastName);
            }
            return query.Select(x => new PersonModel
            {
                EncrypedId = x.Id.ToString(),
                ModelMode = ModelActions.List,
                FirstName = x.FirstName,
                LastName = x.LastName
            }).ToList();
        }

        private Person GetEntity(string encryptedId)
        {
            //TODO: add data protection encrypt and decrypt
            int Id = Int32.Parse(encryptedId);
            return _ctx.Person.Find(Id);
        }
        public PersonModel PrepareMode(string encryptedId)
        {

            if (string.IsNullOrEmpty(encryptedId)) return null;
            var entity = GetEntity(encryptedId);
            //TODO: add auto mapper to use ToModel function
            if (entity != null)
            {
                var model = new PersonModel
                {
                    EncrypedId = encryptedId,
                    FirstName = entity.FirstName,
                    LastName = entity.LastName
                };
                return model;
            }
            return null;
        }

        public async Task<PersonModel> Save(PersonModel model)
        {

            if (model == null) return null;
            Person entity = null;
            if (string.IsNullOrEmpty(model.EncrypedId))
            {
                entity = new Person();
            }
            else
            {
                entity = GetEntity(model.EncrypedId);
            }
            if (entity == null) throw new NullReferenceException(nameof(entity));

            entity.FirstName = model.FirstName;
            entity.LastName = model.LastName;

            if (string.IsNullOrEmpty(model.EncrypedId))
            {
                _ctx.Person.Add(entity);
                model.EncrypedId = entity.Id.ToString();
            }
            else
            {
                _ctx.Person.Update(entity);
            }
            await _ctx.SaveChangesAsync();
            return model;

        }
        public async Task Delete(string encryptedId)
        {
            var entity = GetEntity(encryptedId);
            _ctx.Person.Remove(entity);
            await _ctx.SaveChangesAsync();
        }

    }
}

