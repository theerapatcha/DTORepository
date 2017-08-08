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
using Xunit.Abstractions;

namespace DTORepositoryTest.Services
{
    public class ListServiceTest : UnitTestBase
    {
        public ListServiceTest(ITestOutputHelper output) : base(output) { }
        [Fact]
        public void TestList_Query_By_Predicate_Blog()
        {
            IListService<Blog> listService = new ListService<Blog>(this._context);
            var result = listService.Query<BlogDto>(x => x.BlogId == 1);
            Assert.True(result.IsValid);
            Assert.Equal(1, result.Result.Count);
            Assert.Equal(1, result.Result[0].BlogId);
            Assert.Equal("Dummy Blog #1", result.Result[0].Name);
        }

        [Fact]
        public void TestList_Query_By_Predicate_Blog_Should_Not_Return_Posts()
        {
            IListService<Blog> listService = new ListService<Blog>(this._context);
            var result = listService.Query<BlogDto>(x => x.BlogId == 1);
            Assert.True(result.IsValid);
            Assert.Equal(1, result.Result.Count);
            Assert.Equal(1, result.Result[0].BlogId);
            Assert.Equal("Dummy Blog #1", result.Result[0].Name);
            Assert.Null(result.Result[0].Posts);
        }
        [Fact]
        public async Task TestList_QueryAsync_By_Predicate_Blog()
        {
            IListService<Blog> listService = new ListService<Blog>(this._context);
            var result = await listService.QueryAsync<BlogDto>(x => x.BlogId == 1);
            Assert.True(result.IsValid);
            Assert.Equal(1, result.Result.Count);
            Assert.Equal(1, result.Result[0].BlogId);
            Assert.Equal("Dummy Blog #1", result.Result[0].Name);
        }

        [Fact]
        public void TestList_List_Using_Queryable_Blog()
        {
            IListService<Blog> listService = new ListService<Blog>(this._context);
            var queryable = listService.List<BlogDto>();
            var result = queryable.Where(x => x.BlogId == 1).ToList();
            Assert.Equal(1, result.Count);
            Assert.Equal(1, result[0].BlogId);
            Assert.Equal("Dummy Blog #1", result[0].Name);
        }
        [Fact]
        public void TestList_List_Post_ByPostId_And_NumberofBlogs_1()
        {
            IListService<Post> listService = new ListService<Post>(this._context);
            var queryable = listService.List<PostDto>().Where(x => x.PostId == 1).Where(x=>x.NumberOfBlogsBelongTo == 0);
             var result = queryable.ToList();
            Assert.Equal(0, result.Count);
        }
        [Fact]
        public void TestList_List_Post_ByPostId_And_NumberofBlogs_2()
        {
            IListService<Post> listService = new ListService<Post>(this._context);
            var queryable = listService.List<PostDto>().Where(x => x.PostId == 1 && x.NumberOfBlogsBelongTo == 1 && x.PostId != 2);
            var result = queryable.ToList();
            Assert.Equal(1, result.Count);
            Assert.Equal(1, result[0].NumberOfBlogsBelongTo);
        }
        [Fact]
        public void TestList_Query_Post()
        {
            IListService<Post> listService = new ListService<Post>(this._context);
            var result = listService.Query<PostDto>(x => x.PostId == 1);
            
            Assert.Equal(1, result.Result.Count);
            Assert.Equal(1, result.Result[0].NumberOfBlogsBelongTo);
        }
    }
    
}
