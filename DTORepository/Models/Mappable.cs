using AutoMapper;
using DTORepository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTORepository.Models
{
    public abstract class Mappable
    {
        public abstract Profile GetMapperProfile();
        public class MappableProfile : Profile
        {
            public MappableProfile() : base()
            {

            }
            public MappableProfile(string profileName, Action<IProfileExpression> configurationAction) : base(profileName, configurationAction)
            {

            }
        }
    }
}
