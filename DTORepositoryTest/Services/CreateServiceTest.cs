using DTORepository.Common;
using DTORepository.Services;
using DTORepositoryTest.Samples.Dtos;
using DTORepositoryTest.Samples.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DTORepositoryTest.Services
{
    public class CreateServiceTest : UnitTestBase
    {   
        [Fact]
        public void TestCreate_Blog()
        {
            ICreateOrUpdateService<Blog> createOrUpdateService = new CreateOrUpdateService<Blog>(this._context);
            var result = createOrUpdateService.CreateOrUpdate(new BlogDto
            {
                Name = "Test Create",
                Url = "https://www.usc.edu",
                AuthorId = 1
            }, ActionFlags.Create);
            Assert.True(result.IsValid);
            Assert.NotEqual(0, result.Result.BlogId);
            var entity = this._context.Blogs.Where(x => x.BlogId == result.Result.BlogId).First();
            Assert.Equal("Test Create", result.Result.Name);
            Assert.Equal("https://www.usc.edu", result.Result.Url);
            Assert.Equal(DateTime.MaxValue, entity.CreatedDate);
            Assert.Equal(0, entity.Posts.Count);
            Assert.Equal(1, entity.Author.Id);
        }

        [Fact]
        public void CreateBlogWith_NewPosts()
        {
            ICreateOrUpdateService<Blog> createOrUpdateService = new CreateOrUpdateService<Blog>(this._context);
            var result = createOrUpdateService.CreateOrUpdate(new BlogWithPostsCreateDto
            {
                Name = "Test Create2",
                Url = "https://www.usc.edu",
                Posts = new List<PostDto>()
                {
                    new PostDto { Title = "TestTitle1", Content = "TestContent1" },
                    new PostDto { Title = "TestTitle2", Content = "TestContent2" }
                }
            }, ActionFlags.Create);
            Assert.True(result.IsValid);
            Assert.NotEqual(0, result.Result.BlogId);
            var entity = this._context.Blogs.Where(x => x.BlogId == result.Result.BlogId).First();
            Assert.Equal("Test Create2", result.Result.Name);
            Assert.NotEqual(0, result.Result.Posts[0].PostId);
            Assert.NotEqual(0, result.Result.Posts[1].PostId);
            Assert.Equal(DateTime.MaxValue, entity.Posts.ToList()[0].CreatedDate);
            Assert.Equal(DateTime.MaxValue, entity.Posts.ToList()[1].CreatedDate);
        }
        [Fact]
        public void CreateBlogWith_ExistingPosts()
        {
            ICreateOrUpdateService<Blog> createOrUpdateService = new CreateOrUpdateService<Blog>(this._context);
            var result = createOrUpdateService.CreateOrUpdate(new BlogWithPostsCreateDto
            {
                Name = "Test Create2",
                Url = "https://www.usc.edu",
                Posts = new List<PostDto>()
                {
                    new PostDto { PostId=1, Title = "TestTitle1", Content = "TestContent1" },
                }
            }, ActionFlags.Create);
            Assert.True(result.IsValid);
            Assert.NotEqual(0, result.Result.BlogId);
            var entity = this._context.Blogs.Where(x => x.BlogId == result.Result.BlogId).First();
            Assert.Equal("Test Create2", result.Result.Name);
            Assert.Equal(1, result.Result.Posts[0].PostId);
            Assert.Equal(DateTime.MaxValue, entity.Posts.ToList()[0].UpdatedDate);
            Assert.Equal(1, entity.Posts.Count);
        }
        [Fact]
        public void CreateBlogWith_ExisitingId_Should_Failed()
        {
            ICreateOrUpdateService<Blog> createOrUpdateService = new CreateOrUpdateService<Blog>(this._context);
            var result = createOrUpdateService.CreateOrUpdate(new BlogWithPostsCreateDto
            {
                BlogId = 1,
                Name = "Test Create2",
                Url = "https://www.usc.edu",
                Posts = new List<PostDto>()
                {
                    new PostDto { PostId=1, Title = "TestTitle1", Content = "TestContent1" },
                }
            }, ActionFlags.Create);
            Assert.False(result.IsValid);
        }
        [Fact]
        public void CreatePost()
        {
            ICreateOrUpdateService<Post> createOrUpdateService = new CreateOrUpdateService<Post>(this._context);
            var result = createOrUpdateService.CreateOrUpdate(new PostDto
            {
                Title = "TestTitle2",
                Content = "TestContent2",
                EdittedContent = "TestEdittedContent3"
            }, ActionFlags.Create);
            Assert.True(result.IsValid);
            Assert.Equal("TestContent2", result.Result.Content);
            Assert.Null(result.Result.EdittedContent);
        }
        [Fact]
        public void Cannot_CreateOrUpdateBlog_With_BlogUpdateDto()
        {
            ICreateOrUpdateService<Blog> createOrUpdateService = new CreateOrUpdateService<Blog>(this._context);
            var result = createOrUpdateService.CreateOrUpdate(new BlogUpdateDto
            {
                BlogId = 9, // not exist id
                Name = "Cannot update"
            }, ActionFlags.Create);
            Assert.False(result.IsValid);
        }
        [Fact]
        public void Cannot_CreateBlog_With_BlogUpdateDto()
        {
            ICreateOrUpdateService<Blog> createOrUpdateService = new CreateOrUpdateService<Blog>(this._context);
            var result = createOrUpdateService.CreateOrUpdate(new BlogUpdateDto
            {
                BlogId = 9,
                Name = "Cannot update"
            }, ActionFlags.Create);
            Assert.False(result.IsValid);
        }
        [Fact]
        public void Can_UpdateBlog_With_BlogUpdateDto()
        {
            ICreateOrUpdateService<Blog> createOrUpdateService = new CreateOrUpdateService<Blog>(this._context);
            var result = createOrUpdateService.CreateOrUpdate(new BlogUpdateDto
            {
                BlogId = 1, // exist id
                Name = "Cannot update"
            });
            Assert.True(result.IsValid);
        }
    }
    
}
