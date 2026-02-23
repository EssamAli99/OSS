#nullable disable
using OSS.Services.AppServices;
using OSS.Services.DomainServices;
using OSS.Services.Models;

namespace OSS.Services.ExportImport
{
    /// <summary>
    /// Import manager
    /// </summary>
    public class ImportManager : IImportManager
    {


        private readonly ILocalizationService _localizationService;
        private readonly ILogService _logger;
        private readonly ITestTableService _service;





        public ImportManager(ILocalizationService localizationService,
            ILogService logger, ITestTableService service)
        {
            _localizationService = localizationService;
            _logger = logger;
            _service = service;
        }






        /// <summary>
        /// Import newsletter subscribers from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of imported subscribers
        /// </returns>
        public virtual async Task<int> ImportTestTablesFromTxtAsync(Stream stream)
        {
            var count = 0;
            using (var reader = new StreamReader(stream))
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    var tmp = line.Split(',');

                    if (tmp.Length > 3)
                        throw new Exception("Wrong file format");

                    var isActive = true;

                    //"active" field specified
                    if (tmp.Length >= 2)
                        isActive = bool.Parse(tmp[1].Trim());

                    //"storeId" field specified
                    int id = 0;
                    if (tmp.Length == 3)
                    {
                        if (int.TryParse(tmp[0], out id)) throw new Exception("Wrong format");
                    }

                    //import
                    var entity = await _service.PrepareMode(id);
                    if (entity != null)
                    {
                        entity.Text1 = tmp[1].Trim();
                        entity.Text2 = tmp[2].Trim();
                    }
                    else
                    {
                        entity = new TestTableModel
                        {
                            Id = int.Parse(tmp[0].Trim()),
                            Text1 = tmp[1].Trim(),
                            Text2 = tmp[2].Trim(),
                        };
                    }

                    await _service.Save(entity);
                    count++;
                }

            return count;
        }





        public class CategoryKey
        {
            public CategoryKey(string key, List<int> storesIds = null)
            {
                Key = key.Trim();
                StoresIds = storesIds ?? new List<int>();
            }

            public List<int> StoresIds { get; }

            public string Key { get; }

            public bool Equals(CategoryKey y)
            {
                if (y == null)
                    return false;

                if ((StoresIds.Any() || y.StoresIds.Any())
                    && (StoresIds.All(id => !y.StoresIds.Contains(id)) || y.StoresIds.All(id => !StoresIds.Contains(id))))
                    return false;

                return Key.Equals(y.Key);
            }

            public override int GetHashCode()
            {
                if (!StoresIds.Any())
                    return Key.GetHashCode();

                var storesIds = StoresIds.Select(id => id.ToString())
                    .Aggregate(string.Empty, (all, current) => all + current);

                return $"{storesIds}_{Key}".GetHashCode();
            }

            public override bool Equals(object obj)
            {
                var other = obj as CategoryKey;
                return other?.Equals(other) ?? false;
            }
        }


    }
}
