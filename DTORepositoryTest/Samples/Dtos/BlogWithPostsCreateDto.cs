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
    class BlogWithPostsCreateDto : DtoBase<Blog, BlogWithPostsCreateDto>
    {
        public int BlogId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public List<PostDto> Posts { get; set; }
        public List<BlogTagDto> Tags { get; set; }
        protected override ActionFlags AllowedActions => ActionFlags.Create | ActionFlags.Update;
        protected override ISuccessOrErrors<Blog> UpdateDataFromDto(DbContext context, Blog entity, Blog oldEntity)
        {
            return base.UpdateDataFromDto(context, entity, oldEntity);
        }
        protected override Blog SetupRestOfEntity(DbContext context, Blog blog)
        {
            blog.CreatedDate = DateTime.MaxValue;
            return blog;
        }
    }
}
