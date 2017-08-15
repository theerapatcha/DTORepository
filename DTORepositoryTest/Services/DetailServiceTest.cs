using DTORepository.Services;
using DTORepositoryTest.Samples.Dtos;
using DTORepositoryTest.Samples.Models;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace DTORepositoryTest.Services
{
    public class DetailServiceTest : UnitTestBase
    {
        public DetailServiceTest(ITestOutputHelper output) : base(output) { }
        [Fact]
        public void TestDetail_Blog()
        {
            IDetailService<BloggingContext, Blog> detailService = new DetailService<BloggingContext, Blog>(this._context);
            var result = detailService.Get<BlogDto>(2);
            Assert.True(result.IsValid);
            Assert.Equal("Dummy Blog #2", result.Result.Name);
            Assert.Equal("http://google.com", result.Result.Url);
            Assert.Equal(2, result.Result.Posts.Count());
            Assert.Equal(2, result.Result.NumberOfPosts);
            Assert.Equal(3, result.Result.PostIds[0]);
            Assert.Equal(4, result.Result.PostIds[1]);
        }
        [Fact]
        public void TestDetail_Blog_Check_CanCall_Reference()
        {
            IDetailService<BloggingContext, Blog> detailService = new DetailService<BloggingContext, Blog>(this._context);
            var result = detailService.Get<BlogDto>(1);
            Assert.True(result.IsValid);
            Assert.Equal("Dummy Blog #1", result.Result.Name);
            Assert.Equal("John Doe", result.Result.Author.Name);
            Assert.Equal(1, result.Result.Tags.Count);
            Assert.Equal(1, result.Result.Tags.First().BlogId2);
            Assert.Null(result.Result.Posts.First().NumberOfBlogsBelongTo);
        }
        [Fact]
        public void TestDetail_Get_Post()
        {
            IDetailService<BloggingContext, Post> detailService = new DetailService<BloggingContext, Post>(this._context);
            var result = detailService.Get<PostDto>(1);
            Assert.Equal(1, result.Result.PostId);
            Assert.Null(result.Result.NumberOfBlogsBelongTo);
        }
    }
    
}
