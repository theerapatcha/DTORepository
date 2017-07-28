using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTORepositoryTest.Samples.Models
{
    public class BloggingContext : DbContext
    {
        public BloggingContext()
        {
        }
        public BloggingContext(DbConnection connection) : base(connection, true)
        {
        }
        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<BlogTag> BlogTags { get; set; }
    }
    public class Blog
    {
        public Blog()
        {
            this.Posts = new HashSet<Post>();
            this.Tags = new HashSet<BlogTag>();
        }
        public int BlogId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? AuthorId { get; set; }
        public virtual Author Author { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<BlogTag> Tags { get; set; }
    }

    public class Post
    {
        public Post()
        {
            this.Blogs = new HashSet<Blog>();
        }
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string EdittedContent { get; set; }
        public int NumberOfEditted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public virtual ICollection<Blog> Blogs { get; set; }
    }
    public class BlogTag
    {
        public int Id { get; set; }
        public int BlogId { get; set; }
        public string Name { get; set; }
        public virtual Blog Blog { get; set; }
        
    }
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
