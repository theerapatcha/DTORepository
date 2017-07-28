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
    class PostDto : DtoBase<Post, PostDto>
    {

        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        [IgnoreWritingFor(ActionFlags.Create)]
        public string EdittedContent { get; set; }
        [IgnoreRetrievingFor(ActionFlags.Get)]
        public Nullable<int> NumberOfBlogsBelongTo { get; set; }
        protected override ActionFlags AllowedActions => ActionFlags.All;
        //protected override Action<Post, PostDto> EntityToDtoMapping
        //{
        //    get
        //    {
        //        return (s, d) => d.NumberOfBlogsBelongTo = s.Blogs.Count();
        //    }
        //}
        protected override Action<IMappingExpression<Post, PostDto>> EntityToDtoMapping
        {
            get
            {
                return m => m.ForMember(d => d.NumberOfBlogsBelongTo,
                    opt => opt.MapFrom(s => s.Blogs.Count()));
            }

        }
        //protected override Action<Post, PostDto> EntityToDtoMapping2
        //{
        //    get
        //    {
        //        return (s, d) => d.NumberOfBlogsBelongTo = s.Blogs.Count();
        //    }
        //}
        protected override ISuccessOrErrors<Post> CreateDataFromDto(DbContext context, Post newPost)
        {
            var status = base.CreateDataFromDto(context, newPost);
            var post = status.Result;
            post.CreatedDate = DateTime.MaxValue;
            return status;
        }
        protected override Post SetupRestOfEntity(DbContext context, Post entity)
        {
            entity.NumberOfEditted = entity.NumberOfEditted+1;
            return base.SetupRestOfEntity(context, entity);
        }
        protected override ISuccessOrErrors<Post> UpdateDataFromDto(DbContext context, Post newPost, Post oldPost)
        {
            var status = base.UpdateDataFromDto(context, newPost, oldPost);
            var post = status.Result;
            post.UpdatedDate = DateTime.MaxValue;
            return status;
        }
    }
}
