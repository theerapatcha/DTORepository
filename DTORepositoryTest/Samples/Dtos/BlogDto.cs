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
    class BlogDto : DtoBase<Blog, BlogDto>
    {
        public int BlogId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        [IgnoreWritingFor(ActionFlags.Update)]
        public Nullable<int> AuthorId { get; set; }
        [IgnoreWritingFor(ActionFlags.Update)]
        public AuthorDto Author { get; set;  }

        [IgnoreRetrievingFor(ActionFlags.List)]
        public virtual List<PostDto> Posts { get; set; }
        public virtual List<int> PostIds { get; set; }
        public virtual List<BlogTagDto> Tags { get; set; }
        public int NumberOfPosts { get; set; }

        protected override ActionFlags AllowedActions => ActionFlags.All;
        protected override Action<IMappingExpression<Blog, BlogDto>> EntityToDtoMapping
        {
            get
            {
                return m => m
                    .ForMember(d => d.NumberOfPosts,
                        opt => opt.MapFrom(s => s.Posts.Count()))
                    .ForMember(d => d.PostIds,
                        opt => opt.MapFrom(s => s.Posts.Select(x => x.PostId).ToList()));
            }
            
        }
        protected override ISuccessOrErrors<Blog> CreateDataFromDto(DbContext context, Blog newBlog)
        {
            var status = base.CreateDataFromDto(context, newBlog);
            var blog = status.Result;
            blog.CreatedDate = DateTime.MaxValue;
            return status;
        }
        protected override ISuccessOrErrors<Blog> UpdateDataFromDto(DbContext context, Blog newBlog, Blog oldBlog)
        {
            var status = base.UpdateDataFromDto(context, newBlog, oldBlog);
            var blog = status.Result;
            blog.UpdatedDate = DateTime.MaxValue;
            return status;
        }
    }
}
