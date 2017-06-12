using DtoHelperLib.Services;
using DtoHelperLibTest.Dtos;
using DtoHelperLibTest.Samples.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DtoHelperLibTest.Services
{
    public class CreateOrUpdateServiceAsyncTest : UnitTestBase
    {   
        [Fact]
        public void TestUpdate_Blog()
        {
            var entity = this._context.Blogs.Where(x => x.BlogId == 1).First();
            ICreateOrUpdateService<Blog> CreateOrUpdateService = new CreateOrUpdateService<Blog>(this._context);
            var result = CreateOrUpdateService.CreateOrUpdate(new BlogCreateDto
            {
                BlogId = 1,
                Name = "Test Update",
                Url = "https://www.usc.edu"
            });
            Assert.True(result.IsValid);
            Assert.Equal(1, result.Result.BlogId);
            
            Assert.Equal("Test Update", result.Result.Name);
            Assert.Equal("https://www.usc.edu", result.Result.Url);
            //Assert.Equal(DateTime.MaxValue, entity.UpdatedDate);
        }

        [Fact]
        public void UpdateBlogWithPosts_WithExistingPosts()
        {
            ICreateOrUpdateService<Blog> CreateOrUpdateService = new CreateOrUpdateService<Blog>(this._context);
            var result = CreateOrUpdateService.CreateOrUpdate(new BlogWithPostsCreateDto
            {
                BlogId = 1,
                Name = "Test Update2",
                Url = "https://www.usc.edu",
                Posts = new List<PostCreateDto>()
                {
                    new PostCreateDto { PostId = 1, Title = "TestTitle1", Content = "TestContent1" },
                    new PostCreateDto { PostId = 2, Title = "TestTitle2", Content = "TestContent2" },
                    new PostCreateDto { Title = "TestTitle3", Content = "TestContent3" },
                }
            });
            Assert.True(result.IsValid);
            Assert.Equal(1, result.Result.BlogId);
            var entity = this._context.Blogs.Where(x => x.BlogId == 1).First();
            Assert.Equal("Test Update2", result.Result.Name);
            Assert.NotEqual(0, result.Result.Posts.ToList()[0].PostId);
            Assert.NotEqual(0, result.Result.Posts.ToList()[1].PostId);
            Assert.NotEqual(0, result.Result.Posts.ToList()[2].PostId);
            Assert.Equal(DateTime.MaxValue, entity.Posts.ToList()[0].UpdatedDate);
            Assert.Equal(DateTime.MaxValue, entity.Posts.ToList()[1].UpdatedDate);
            Assert.Equal(DateTime.MinValue, entity.Posts.ToList()[2].UpdatedDate);
            Assert.Equal(3, entity.Posts.Count());
        }
        [Fact]
        public void UpdateBlogWithPosts_WithoutExistingPosts_Should_DeleteExistingPosts()
        {
            ICreateOrUpdateService<Blog> CreateOrUpdateService = new CreateOrUpdateService<Blog>(this._context);
            var result = CreateOrUpdateService.CreateOrUpdate(new BlogWithPostsCreateDto
            {
                BlogId = 1,
                Name = "Test Update2",
                Url = "https://www.usc.edu",
                Posts = new List<PostCreateDto>()
                {
                    new PostCreateDto { Title = "TestTitle1", Content = "TestContent1" },
                    new PostCreateDto { Title = "TestTitle2", Content = "TestContent2" },
                    new PostCreateDto { Title = "TestTitle3", Content = "TestContent3" },
                }
            });
            Assert.True(result.IsValid);
            Assert.Equal(1, result.Result.BlogId);
            var entity = this._context.Blogs.Where(x => x.BlogId == 1).First();
            Assert.Equal("Test Update2", result.Result.Name);
            Assert.NotEqual(0, result.Result.Posts.ToList()[0].PostId);
            Assert.NotEqual(0, result.Result.Posts.ToList()[1].PostId);
            Assert.Equal(3, entity.Posts.Count());
        }
        [Fact]
        public void UpdateBlogWithPosts_WithExistingPostIds_Should()
        {
            ICreateOrUpdateService<Blog> CreateOrUpdateService = new CreateOrUpdateService<Blog>(this._context);
            var result = CreateOrUpdateService.CreateOrUpdate(new BlogWithPostIdsCreateDto
            {
                BlogId = 1,
                Name = "Test Update2",
                Url = "https://www.usc.edu",
                PostIds = new List<int>() { 3, 4 }
            });
            Assert.True(result.IsValid);
            Assert.Equal(1, result.Result.BlogId);
            var entity = this._context.Blogs.Where(x => x.BlogId == 1).First();
            Assert.Equal("Test Update2", result.Result.Name);
            Assert.Equal(2, entity.Posts.Count());
        }
    }
    
}
