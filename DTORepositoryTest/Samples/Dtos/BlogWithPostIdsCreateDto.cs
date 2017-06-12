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
    class BlogWithPostIdsCreateDto : DtoBase<Blog, BlogWithPostIdsCreateDto>
    {
        public int BlogId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public List<int> PostIds { get; set; }
        protected override ActionFlags AllowedActions => ActionFlags.Create | ActionFlags.Update;
        protected override ISuccessOrErrors<Blog> CreateDataFromDto(DbContext context, Blog newBlog)
        {
            var status = base.CreateDataFromDto(context, newBlog);
            var blog = status.Result;
            blog.Posts = context.Set<Post>().Where(x => PostIds.Contains(x.PostId)).ToList();
            return status;
        }
        protected override ISuccessOrErrors<Blog> UpdateDataFromDto(DbContext context, Blog newBlog, Blog oldBlog)
        {
            var status = base.UpdateDataFromDto(context, newBlog, oldBlog);
            var blog = status.Result;
            blog.Posts = context.Set<Post>().Where(x => PostIds.Contains(x.PostId)).ToList();
            return status;
        }
    }
}
