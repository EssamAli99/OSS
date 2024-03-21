using AutoMapper;
using OSS.Data;
using OSS.Data.Entities;
using OSS.Services.Models;

namespace OSS.Services
{
    public class AllMapperConfiguration : Profile
    {
        public AllMapperConfiguration()
        {
            CreateMap<TestTable, TestTableModel>();
            //    .ForMember(model => model.EncrypedId, op => op.MapFrom(x => x.Id.ToString()));
            CreateMap<TestTableModel, TestTable>();
            //    .ForMember(entity => entity.Id, op => op.MapFrom(x => int.Parse(x.EncrypedId)));

            CreateMap<Log, LogModel>();
            CreateMap<AppPage, AppPageModel>();
            CreateMap<EmailAccount, EmailAccountModel>();
            CreateMap<Language, LanguageModel>();
            CreateMap<QueuedEmail, QueuedEmailModel>();

            ////moved to startup as part of upgrade automapper
            ////add some generic mapping rules
            //ForAllMaps((mapConfiguration, map) =>
            //{
            //    if (typeof(BaseModel).IsAssignableFrom(mapConfiguration.DestinationType))
            //    {
            //        map.ForMember(nameof(BaseModel.ModelMode), options => options.Ignore());
            //        map.ForMember(nameof(BaseModel.EncrypedId), options => options.MapFrom(entity => ((BaseEntity)entity).Id.ToString()));
            //    }

            //    if (typeof(BaseEntity).IsAssignableFrom(mapConfiguration.DestinationType))
            //    {
            //        map.ForMember(nameof(BaseEntity.Id), options => options.MapFrom(entity => int.Parse(((BaseModel)entity).EncrypedId)));
            //    }
            //});


        }
    }
}
