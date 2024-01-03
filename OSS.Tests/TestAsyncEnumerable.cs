/*
 * https://docs.microsoft.com/en-us/ef/ef6/fundamentals/testing/mocking?redirectedfrom=MSDN
 * https://stackoverflow.com/questions/40476233/how-to-mock-an-async-repository-with-entity-framework-core/40491640#40491640
*/

using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace OSS.Tests
{
    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            return new TestAsyncEnumerable<TResult>(expression);
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute<TResult>(expression));
        }

        TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

    }

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public TestAsyncEnumerable(Expression expression)
            : base(expression)
        { }

        public IAsyncEnumerator<T> GetEnumerator()
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider
        {
            get { return new TestAsyncQueryProvider<T>(this); }
        }
    }

    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public void Dispose()
        {
            _inner.Dispose();
        }

        public T Current
        {
            get
            {
                return _inner.Current;
            }
        }

        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            return Task.FromResult(_inner.MoveNext());
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(Task.FromResult(_inner.MoveNext()));
        }

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return new ValueTask(Task.CompletedTask);
        }
    }
}


//using System.Collections.Generic;
//using System.Data.Entity.Infrastructure;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Threading;
//using System.Threading.Tasks;

//namespace OSS.Tests
//{
//    internal class TestDbAsyncQueryProvider<TEntity> : IDbAsyncQueryProvider
//    {
//        private readonly IQueryProvider _inner;

//        internal TestDbAsyncQueryProvider(IQueryProvider inner)
//        {
//            _inner = inner;
//        }

//        public IQueryable CreateQuery(Expression expression)
//        {
//            return new TestDbAsyncEnumerable<TEntity>(expression);
//        }

//        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
//        {
//            return new TestDbAsyncEnumerable<TElement>(expression);
//        }

//        public object Execute(Expression expression)
//        {
//            return _inner.Execute(expression);
//        }

//        public TResult Execute<TResult>(Expression expression)
//        {
//            return _inner.Execute<TResult>(expression);
//        }

//        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
//        {
//            return Task.FromResult(Execute(expression));
//        }

//        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
//        {
//            return Task.FromResult(Execute<TResult>(expression));
//        }
//    }

//    internal class TestDbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
//    {
//        public TestDbAsyncEnumerable(IEnumerable<T> enumerable)
//            : base(enumerable)
//        { }

//        public TestDbAsyncEnumerable(Expression expression)
//            : base(expression)
//        { }

//        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
//        {
//            return new TestDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
//        }

//        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
//        {
//            return GetAsyncEnumerator();
//        }

//        IQueryProvider IQueryable.Provider
//        {
//            get { return new TestDbAsyncQueryProvider<T>(this); }
//        }
//    }

//    internal class TestDbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
//    {
//        private readonly IEnumerator<T> _inner;

//        public TestDbAsyncEnumerator(IEnumerator<T> inner)
//        {
//            _inner = inner;
//        }

//        public void Dispose()
//        {
//            _inner.Dispose();
//        }

//        public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
//        {
//            return Task.FromResult(_inner.MoveNext());
//        }

//        public T Current
//        {
//            get { return _inner.Current; }
//        }

//        object IDbAsyncEnumerator.Current
//        {
//            get { return Current; }
//        }
//    }
//}


// function need to be refactored
/*
         public SearchViewModel PrepareAddressSearchModel(Data.Models.Entities.Address model, string lang)
        {
            string streetType = string.Empty;
            if (string.IsNullOrWhiteSpace(lang)) lang = "EN";
            var lng = lang.ToUpper()[0];
            SearchViewModel search = new SearchViewModel
            {
                CivicNum = (model.CivicNumber ?? "").Trim().ToUpper(),
                Lang = lang,
                Place = (model.Municipality ?? "").Trim().RemoveDiacritics().ToUpper(),
                PostalCode = (model.PostalCode ?? "").Trim(),
                StreetDir = (model.StreetDirection ?? "").Trim().ToUpper(),
                StreetName = (model.StreetName ?? "").Trim().RemoveDiacritics().ToUpper(),
                StreetType = (model.StreetType ?? "").Trim().ToUpper(),
                UnitNumber = (model.UnitNumber ?? "").Trim(),
                UnitType = (model.UnitType ?? "").Trim(),
                CivicNumSuffix = string.Empty,
                UnitSuffix = string.Empty
            };

            //try
            //{
            // check for known directions and units
            if (!string.IsNullOrWhiteSpace(search.CivicNum) && search.CivicNum.Contains(' '))
            {
                var parts = search.CivicNum.Split(' ');
                var dir = Directions.FirstOrDefault(x => parts[0].Equals((x.ToUpper())));
                if (string.IsNullOrWhiteSpace(dir)) dir = Directions.FirstOrDefault(x => parts.Last().Equals((x.ToUpper())));
                if (!string.IsNullOrWhiteSpace(dir))
                {
                    search.StreetDir = dir;
                    parts[Array.IndexOf(parts, dir.ToUpper())] = string.Empty;
                }

                var unit = Units.FirstOrDefault(x => parts.Contains(x.ToUpper()));
                if (!string.IsNullOrWhiteSpace(unit))
                {
                    search.UnitType = unit;
                    var i = Array.IndexOf(parts, unit.ToUpper());
                    parts[i] = string.Empty;
                    // the following part after unit will be the unite number 
                    if (i > -1 && parts.Length > i + 1)
                    {
                        if (string.IsNullOrWhiteSpace(search.UnitNumber)) search.UnitNumber = parts[i + 1].Trim();
                        else search.UnitSuffix = parts[i + 1].Trim();
                        parts[i + 1] = string.Empty;
                    }
                }
                search.CivicNum = string.Join(' ', parts).Trim();
            }
            var suffix = search.CivicNum;
            //get civic number
            if (!string.IsNullOrWhiteSpace(search.CivicNum))
            {
                if (search.CivicNum.Contains('-'))
                {
                    string[] array2 = search.CivicNum.Split('-');
                    if (array2.Length > 1)
                    {
                        search.CivicNum = array2[1].Trim();
                        if (string.IsNullOrWhiteSpace(search.UnitNumber)) search.UnitNumber = array2[0].Trim();
                        else search.UnitSuffix += array2[0].Trim();
                        if (string.IsNullOrWhiteSpace(search.UnitType)) search.UnitType = lng == 'F' ? "Unité" : "UNIT";
                        else search.UnitSuffix += lng == 'F' ? "Unité" : "UNIT";
                    }
                }
                int.TryParse(search.CivicNum, out int cn);
                if (cn > 0) search.CivicNum = cn.ToString();
                else
                {
                    if (search.CivicNum.Contains(' '))
                    {
                        var parts = search.CivicNum.Split(' ');
                        for (int i = 0; i < parts.Length; i++)
                        {
                            if (int.TryParse(parts[i], out _))
                            {
                                search.CivicNum = parts[i];
                                parts[i] = string.Empty;
                                suffix = string.Join(' ', parts).Trim();
                                break;
                            }
                        }
                    }
                    else
                    {
                        search.CivicNum = Regex.Match(search.CivicNum, @"\d+").Value;
                    }
                }
            }

            if (suffix != search.CivicNum)
            {
                var i = suffix.IndexOf(search.CivicNum);
                if (i > -1 && i + search.CivicNum.Length < suffix.Length)
                {
                    search.CivicNumSuffix = suffix.Substring(i + search.CivicNum.Length).Trim();
                }
                if (search.CivicNumSuffix.Length == 1 && !Regex.IsMatch(search.CivicNumSuffix, @"[A-Za-z0-9]")) search.CivicNumSuffix = string.Empty;
            }
            //get street name and street type
            if (!string.IsNullOrWhiteSpace(model.Province) && !string.IsNullOrWhiteSpace(model.Municipality)
                && !string.IsNullOrWhiteSpace(search.StreetName))
            {
                var q = Addresses.Where(x => x.Province.Equals(model.Province.ToUpper())
                        && x.PlaceName.Equals(search.Place));
                var addresses = q.Where(x => x.StreetNameFull_F.Equals(search.StreetName)
                            || x.StreetNameFull_E.Equals(search.StreetName)
                            || x.StreetName.Equals(search.StreetName)).ToList();
                Address address = null;
                if (addresses != null && addresses.Count > 0)
                {
                    address = addresses[0];
                    if (addresses.Count > 1)
                    {
                        bool bFound = false;
                        if (!string.IsNullOrWhiteSpace(search.StreetDir))
                        {
                            var adr = addresses.FirstOrDefault(x => x.StreetDirCode.Equals(search.StreetDir) || x.StreetDirDescE.Equals(search.StreetDir)
                                    || x.StreetDirDescF.Equals(search.StreetDir));
                            if (adr != null)
                            {
                                address = adr;
                                bFound = true;
                            }
                        }
                        if (!bFound && !string.IsNullOrWhiteSpace(search.StreetType))
                        {
                            var adr = addresses.FirstOrDefault(x => x.StreetTypeCode.Equals(search.StreetType) || x.StreetTypeDesc.Equals(search.StreetType)
                                    || x.StreetTypePrefCode.Equals(search.StreetType) || x.StreetTypePrefDesc.Equals(search.StreetType));
                            if (adr != null) address = adr;
                        }
                    }
                }

                if (address == null)
                {
                    var streetNameParts = search.StreetName.Split(' ');
                    if (StreetTypes == null || StreetTypes.Count == 0)
                    {
                        StreetTypes = new List<StreetType>();
                        var lst = Addresses.Select(x => new
                        {
                            Code = x.StreetTypeCode,
                            Desc = x.StreetTypeDesc,
                            PrefCode = x.StreetTypePrefCode,
                            PrefDesc = x.StreetTypePrefDesc,
                            Lang = x.StreetTypeLang
                        }).Distinct();

                        StreetTypes.AddRange(lst.Select(x => new StreetType { Code = x.Code, Desc = x.Desc, Lang = x.Lang }));
                        StreetTypes.AddRange(lst.Select(x => new StreetType { Code = x.PrefCode, Desc = x.PrefDesc, Lang = x.Lang }));
                    }
                    if (streetNameParts.Length > 1 && StreetTypes.Count > 0)
                    {
                        //bool clearLast = false;
                        //var st = StreetTypes.FirstOrDefault(x => x.Code.Equals(streetNameParts[0], StringComparison.InvariantCultureIgnoreCase)
                        //        || x.Desc.Equals(streetNameParts[0], StringComparison.InvariantCultureIgnoreCase));
                        //if (st == null)
                        //{
                        //    st = StreetTypes.FirstOrDefault(x => x.Code.Equals(streetNameParts.Last(), StringComparison.InvariantCultureIgnoreCase)
                        //        || x.Desc.Equals(streetNameParts.Last(), StringComparison.InvariantCultureIgnoreCase));
                        //    clearLast = true;
                        //}
                        //if (st != null)
                        //{
                        //    if (string.IsNullOrWhiteSpace(search.StreetType)) search.StreetType = st.Code;
                        //    if (clearLast) streetNameParts[streetNameParts.Length - 1] = string.Empty;
                        //    else streetNameParts[0] = string.Empty;
                        //    search.StreetName = string.Join(' ', streetNameParts).Trim();
                        //}

                        //address = q.FirstOrDefault(x => x.StreetNameFull_F.Equals(search.StreetName)
                        //            || x.StreetNameFull_E.Equals(search.StreetName)
                        //            || x.StreetName.Equals(search.StreetName));

                        var st = StreetTypes.FirstOrDefault(x => x.Code.Equals(streetNameParts[0], StringComparison.InvariantCultureIgnoreCase)
                                    || x.Desc.Equals(streetNameParts[0], StringComparison.InvariantCultureIgnoreCase));
                        if (st != null)
                        {
                            string[] dummy = streetNameParts.Clone() as string[];
                            dummy[0] = string.Empty;
                            var sname = string.Join(' ', dummy).Trim();
                            address = q.FirstOrDefault(x => x.StreetNameFull_F.Equals(sname)
                                        || x.StreetNameFull_E.Equals(sname)
                                        || x.StreetName.Equals(sname));
                        }
                        if (address == null)
                        {
                            st = StreetTypes.FirstOrDefault(x => x.Code.Equals(streetNameParts.Last(), StringComparison.InvariantCultureIgnoreCase)
                                    || x.Desc.Equals(streetNameParts.Last(), StringComparison.InvariantCultureIgnoreCase));
                            if (st != null)
                            {
                                string[] dummy = streetNameParts.Clone() as string[];
                                dummy[streetNameParts.Length - 1] = string.Empty;
                                var sname = string.Join(' ', dummy).Trim();
                                address = q.FirstOrDefault(x => x.StreetNameFull_F.Equals(sname)
                                            || x.StreetNameFull_E.Equals(sname)
                                            || x.StreetName.Equals(sname));
                            }
                        }

                    }
                }

                //get street type
                if (address != null)
                {
                    search.AddressFound = true;
                    search.StreetDir = address.StreetDirCode;
                    search.StreetName = address.StreetName;
                    search.StreetType = address.StreetTypeCode;
                    //if (!address.StreetTypeLang.StartsWith(lng) && !string.IsNullOrWhiteSpace(address.StreetTypePrefCode))
                    //{ 
                    //    search.StreetType = address.StreetTypePrefCode;
                    //}
                    if (address.StreetName.Contains("route", StringComparison.InvariantCultureIgnoreCase) && string.IsNullOrWhiteSpace(search.StreetType))
                    {
                        search.StreetType = lng == 'F' ? "AUTOROUTE" : "HIGHWAY";
                    }
                }
            }
            //}
            //catch (Exception ex)
            //{
            //    Logger.LogError(ex, "Error in PrepareAddressSearchModel model:{@model}", model);
            //    //throw; 
            //}

            return search;
        }


 
 */