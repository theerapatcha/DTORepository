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
    public class CreateOrUpdateServiceTest : UnitTestBase
    {   
        [Fact]
        public void TestUpdate_Blog_NameAndUrl_Only()
        {
            
            ICreateOrUpdateService<Blog> CreateOrUpdateService = new CreateOrUpdateService<Blog>(this._context);
            var result = CreateOrUpdateService.CreateOrUpdate(new BlogDto
            {
                BlogId = 1,
                Name = "Test Update",
                Url = "https://www.usc.edu",
            }, ActionFlags.Update);
            var entity = this._context.Blogs.Where(x => x.BlogId == 1).First();
            Assert.True(result.IsValid);
            Assert.Equal(1, result.Result.BlogId);
            
            Assert.Equal("Test Update", result.Result.Name);
            Assert.Equal("https://www.usc.edu", result.Result.Url);
            Assert.Equal(DateTime.MaxValue, entity.UpdatedDate);
            Assert.Equal(2, entity.Posts.Count);
            Assert.Equal(1, entity.Tags.Count);
            Assert.Equal(1, entity.Author.Id );
        }
        [Fact]
        public void TestUpdate_Blog_Should_Cant_Update_Author()
        {

            ICreateOrUpdateService<Blog> CreateOrUpdateService = new CreateOrUpdateService<Blog>(this._context);
            var result = CreateOrUpdateService.CreateOrUpdate(new BlogDto
            {
                BlogId = 1,
                Name = "Test Update",
                Url = "https://www.usc.edu",
                Author = new AuthorDto { Name = "invalid" }
            }, ActionFlags.Update);
            var entity = this._context.Blogs.Where(x => x.BlogId == 1).First();
            Assert.True(result.IsValid);
            Assert.Equal(1, result.Result.BlogId);

            Assert.Equal("Test Update", result.Result.Name);
            Assert.Equal("https://www.usc.edu", result.Result.Url);
            Assert.Equal(DateTime.MaxValue, entity.UpdatedDate);
            Assert.Equal(2, entity.Posts.Count);
            Assert.Equal(1, entity.Tags.Count);
            Assert.Equal(1, entity.Author.Id);
        }
        [Fact]
        public void TestUpdate_Blog_With_NotExistedId_Should_Fail()
        {
            ICreateOrUpdateService<Blog> CreateOrUpdateService = new CreateOrUpdateService<Blog>(this._context);
            var result = CreateOrUpdateService.CreateOrUpdate(new BlogDto
            {
                BlogId = 100,
                Name = "Test Update",
                Url = "https://www.usc.edu"
            }, ActionFlags.Update);
            Assert.False(result.IsValid);
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
                Posts = new List<PostDto>()
                {
                    new PostDto { PostId = 1, Title = "TestTitle1", Content = "TestContent1" },
                    new PostDto { PostId = 2, Title = "TestTitle2", Content = "TestContent2" },
                    new PostDto { Title = "TestTitle3", Content = "TestContent3" },
                }
            }, ActionFlags.Update);
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
        public void UpdateBlogWithPosts_RemoveAllPosts()
        {
            ICreateOrUpdateService<Blog> CreateOrUpdateService = new CreateOrUpdateService<Blog>(this._context);
            var result = CreateOrUpdateService.CreateOrUpdate(new BlogWithPostsCreateDto
            {
                BlogId = 1,
                Name = "Test Update2",
                Url = "https://www.usc.edu",
                Posts = new List<PostDto>()
                {

                }
            }, ActionFlags.Update);
            Assert.True(result.IsValid);
            Assert.Equal(1, result.Result.BlogId);
            var entity = this._context.Blogs.Where(x => x.BlogId == 1).First();
            Assert.Equal("Test Update2", result.Result.Name);
            Assert.Equal(0, entity.Posts.Count());
        }
        [Fact]
        public void UpdateBlogWithPosts_UpdateWithNewBlogTags()
        {
            ICreateOrUpdateService<Blog> CreateOrUpdateService = new CreateOrUpdateService<Blog>(this._context);
            var result = CreateOrUpdateService.CreateOrUpdate(new BlogWithPostsCreateDto
            {
                BlogId = 1,
                Name = "Test Update2",
                Url = "https://www.usc.edu",
                Posts = new List<PostDto> {
                    new PostDto { PostId = 3, Title = "T1", Content = "C1" },
                    new PostDto { PostId = 4, Title = "T2", Content = "C1" }
                },
                Tags = new List<BlogTagDto>()
                {
                    new BlogTagDto { Id = 1, BlogId=1, Name="Test2" },
                    new BlogTagDto { BlogId=1, Name="Test3" }
                }
            }, ActionFlags.Update);
            Assert.True(result.IsValid);
            Assert.Equal(1, result.Result.BlogId);
            var entity = this._context.Blogs.Where(x => x.BlogId == 1).First();
            Assert.Equal("Test Update2", result.Result.Name);
            Assert.Equal(2, entity.Tags.Count());
        }
        [Fact]
        public void UpdateBlogWithPosts_RemoveAllTags()
        {
            ICreateOrUpdateService<Blog> CreateOrUpdateService = new CreateOrUpdateService<Blog>(this._context);
            var result = CreateOrUpdateService.CreateOrUpdate(new BlogWithPostsCreateDto
            {
                BlogId = 1,
                Name = "Test Update2",
                Url = "https://www.usc.edu",
                Tags = new List<BlogTagDto>()
                {

                }
            }, ActionFlags.Update);
            Assert.True(result.IsValid);
            Assert.Equal(1, result.Result.BlogId);
            var entity = this._context.Blogs.Where(x => x.BlogId == 1).First();
            Assert.Equal("Test Update2", result.Result.Name);
            Assert.Equal(0, entity.Tags.Count());
        }
        [Fact]
        public void UpdateBlogWithPosts_UndefinedTags_Should_Not_Remove_Tags()
        {
            ICreateOrUpdateService<Blog> CreateOrUpdateService = new CreateOrUpdateService<Blog>(this._context);
            var result = CreateOrUpdateService.CreateOrUpdate(new BlogWithPostsCreateDto
            {
                BlogId = 1,
                Name = "Test Update2",
                Url = "https://www.usc.edu",
            }, ActionFlags.Update);
            Assert.True(result.IsValid);
            Assert.Equal(1, result.Result.BlogId);
            var entity = this._context.Blogs.Where(x => x.BlogId == 1).First();
            Assert.Equal("Test Update2", result.Result.Name);
            Assert.Equal(1, entity.Tags.Count());
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
                Posts = new List<PostDto>()
                {
                    new PostDto { Title = "TestTitle1", Content = "TestContent1" },
                    new PostDto { Title = "TestTitle2", Content = "TestContent2" },
                    new PostDto { Title = "TestTitle3", Content = "TestContent3" },
                }
            }, ActionFlags.Update);
            Assert.True(result.IsValid);
            Assert.Equal(1, result.Result.BlogId);
            var entity = this._context.Blogs.Where(x => x.BlogId == 1).First();
            Assert.Equal("Test Update2", result.Result.Name);
            Assert.NotEqual(0, result.Result.Posts.ToList()[0].PostId);
            Assert.NotEqual(0, result.Result.Posts.ToList()[1].PostId);
            Assert.Equal(3, entity.Posts.Count());
            Assert.Equal(7, _context.Posts.Count());
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
            }, ActionFlags.Update);
            Assert.True(result.IsValid);
            Assert.Equal(1, result.Result.BlogId);
            var entity = this._context.Blogs.Where(x => x.BlogId == 1).First();
            Assert.Equal("Test Update2", result.Result.Name);
            Assert.Equal(2, entity.Posts.Count());
        }
        [Fact]
        public void UpdateBlogTag_Only_Name_field()
        {
            ICreateOrUpdateService<BlogTag> CreateOrUpdateService = new CreateOrUpdateService<BlogTag>(this._context);
            var result = CreateOrUpdateService.CreateOrUpdate(new BlogTagDto {
                Id = 1,
                Name = "TestUpdate"
            }, ActionFlags.Update);
            Assert.True(result.IsValid);
            Assert.Equal("TestUpdate", result.Result.Name);
            Assert.Equal(1, result.Result.BlogId);
        }

        [Fact]
        public void UpdateBlogTag_Only_Name_Using_IgnoreMappingValue()
        {
            ICreateOrUpdateService<BlogTag> CreateOrUpdateService = new CreateOrUpdateService<BlogTag>(this._context);
            var result = CreateOrUpdateService.CreateOrUpdate(new BlogTagDto
            {
                Id = 1,
                Name = "Skip"
            }, ActionFlags.Update);
            Assert.True(result.IsValid);
            Assert.NotEqual("Skip", result.Result.Name);
            Assert.Equal(1, result.Result.BlogId);
        }
    }
    
}
