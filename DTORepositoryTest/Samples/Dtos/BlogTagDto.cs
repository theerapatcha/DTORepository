using AutoMapper;
using DTORepository.Attributes;
using DTORepository.Common;
using DTORepository.Models;
using DTORepositoryTest.Samples.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTORepositoryTest.Samples.Dtos
{
    class BlogTagDto : DtoBase<BlogTag, BlogTagDto>
    {

        public int Id { get; set; }
        
        public int? BlogId { get; set; }
        [IgnoreMappingIfValue("Skip")]
        public string Name { get; set; }        
        public int BlogId2 { get; set; }
        protected override ActionFlags AllowedActions => ActionFlags.All;
        protected override Action<IMappingExpression<BlogTag, BlogTagDto>> EntityToDtoMapping
        {
            get
            {
                return m => m.ForMember(d => d.BlogId2,
                    opt => opt.MapFrom(s => s.Blog.BlogId));
            }

        }
    }

}
