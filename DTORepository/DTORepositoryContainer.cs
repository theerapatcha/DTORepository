using AutoMapper;
using DTORepository.Attributes;
using DTORepository.Common;
using DTORepository.Internal;
using DTORepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DTORepository
{
    public static class DTORepositoryContainer
    {
        public static bool ThrowsOnError = false;
        private static IMapper _Mapper;
        public static IMapper Mapper {
            get{
                if(_Mapper == null)
                {
                    _Mapper = InitializeDtoMapper();
                }
                return _Mapper;
            }
        }
        public static IMapper InitializeDtoMapper()
        {
            MapperConfiguration mapperConfig = new MapperConfiguration(cfg =>
            {
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
            return mapperConfig.CreateMapper();
        }
    }
}
