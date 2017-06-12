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
    class BlogUpdateDto : DtoBase<Blog, BlogUpdateDto>
    {
        public int BlogId { get; set; }
        public string Name { get; set; }
        protected override ActionFlags AllowedActions => ActionFlags.Update;
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
