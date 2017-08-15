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
    class PostDto : DtoBase<BloggingContext, Post, PostDto>
    {

        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        [IgnoreWritingFor(ActionFlags.Create)]
        public string EdittedContent { get; set; }
        [IgnoreRetrievingFor(ActionFlags.Get)]
        public Nullable<int> NumberOfBlogsBelongTo { get; set; }
        protected override ActionFlags AllowedActions => ActionFlags.All;
        protected override Action<IMappingExpression<Post, PostDto>> EntityToDtoProjection
        {
            get
            {
                return (opt) => opt.ForMember(d => d.NumberOfBlogsBelongTo,
                    m => m.MapFrom(s => (int?) s.Blogs.Count()));
            }
        }
        //protected override PostDto SetupRestOfDto(DbContext context, Post entity)
        //{
        //    this.NumberOfBlogsBelongTo = entity.Blogs.Count();
        //    return this;
        //}
        protected override ISuccessOrErrors<Post> CreateDataFromDto(BloggingContext context, Post newPost)
        {
            var status = base.CreateDataFromDto(context, newPost);
            var post = status.Result;
            post.CreatedDate = DateTime.MaxValue;
            return status;
        }
        protected override Post SetupRestOfEntity(BloggingContext context, Post entity)
        {
            entity.NumberOfEditted = entity.NumberOfEditted+1;
            return base.SetupRestOfEntity(context, entity);
        }
        protected override ISuccessOrErrors<Post> UpdateDataFromDto(BloggingContext context, Post newPost, Post oldPost)
        {
            var status = base.UpdateDataFromDto(context, newPost, oldPost);
            var post = status.Result;
            post.UpdatedDate = DateTime.MaxValue;
            return status;
        }
    }
}
