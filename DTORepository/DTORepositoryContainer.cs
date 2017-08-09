using AutoMapper;
using AutoMapper.EntityFramework;
using AutoMapper.EquivalencyExpression;
using DTORepository.Attributes;
using DTORepository.Common;
using DTORepository.Internal;
using DTORepository.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DTORepository
{
    public static class DTORepositoryContainer
    {
        public static bool ThrowsOnDatabaseError = false;
        private static IMapper _Mapper;
        public static IMapper Mapper {
            get{
                if(_Mapper == null)
                {
                    throw new InvalidOperationException("Please initialize DTORepository before using it");
                    //_Mapper = InitializeDtoMapper();
                }
                return _Mapper;
            }
        }
        public static void InitializeDtoMapper<TDbContext>() 
            where TDbContext : DbContext, new()
        {
            MapperConfiguration mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddCollectionMappers();
                cfg.SetGeneratePropertyMaps<GenerateEntityFrameworkPrimaryKeyPropertyMaps<TDbContext>>();
                cfg.AllowNullCollections = true;
                cfg.IgnoreRetrievingAttributeForActionProperty();
                cfg.IgnoreWritingAttributeForActionProperty();
                cfg.IgnoreMappingOnNullOrIgnoredValue();
                
                foreach (Type type in AppDomain.CurrentDomain.GetAllMappableTypes())
                {
                    var obj = (Mappable)Activator.CreateInstance(type);
                    cfg.AddProfile(obj.GetMapperProfile());
                }
            });
            _Mapper = mapperConfig.CreateMapper();
        }
    }
}
