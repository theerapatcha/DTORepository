﻿using DTORepository.Common;
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
    public class UpdateServiceAsyncTest : UnitTestBase
    {
        public UpdateServiceAsyncTest(ITestOutputHelper output) : base(output) { }
        [Fact]
        public async Task TestUpdate_Blog()
        {
            var entity = this._context.Blogs.Where(x => x.BlogId == 1).First();
            ICreateOrUpdateService<BloggingContext, Blog> CreateOrUpdateService = new CreateOrUpdateService<BloggingContext, Blog>(this._context);
            var result = await CreateOrUpdateService.CreateOrUpdateAsync(new BlogDto
            {
                BlogId = 1,
                Name = "Test Update",
                Url = "https://www.usc.edu"
            }, ActionFlags.Update);
            Assert.True(result.IsValid);
            Assert.Equal(1, result.Result.BlogId);
            
            Assert.Equal("Test Update", result.Result.Name);
            Assert.Equal("https://www.usc.edu", result.Result.Url);
            //Assert.Equal(DateTime.MaxValue, entity.UpdatedDate);
        }

        [Fact]
        public async Task UpdateBlogWithPosts_WithExistingPosts()
        {
            ICreateOrUpdateService<BloggingContext, Blog> CreateOrUpdateService = new CreateOrUpdateService<BloggingContext, Blog>(this._context);
            var result = await CreateOrUpdateService.CreateOrUpdateAsync(new BlogWithPostsCreateDto
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
        public async Task UpdateBlogWithPosts_WithoutExistingPosts_Should_DeleteExistingPosts()
        {
            ICreateOrUpdateService<BloggingContext, Blog> CreateOrUpdateService = new CreateOrUpdateService<BloggingContext, Blog>(this._context);
            var result = await CreateOrUpdateService.CreateOrUpdateAsync(new BlogWithPostsCreateDto
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
        }
        [Fact]
        public async Task UpdateBlogWithPosts_WithExistingPostIds_Should()
        {
            ICreateOrUpdateService<BloggingContext, Blog> CreateOrUpdateService = new CreateOrUpdateService<BloggingContext, Blog>(this._context);
            var result = await CreateOrUpdateService.CreateOrUpdateAsync(new BlogWithPostIdsCreateDto
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
        public async Task UpdateBlogWithPosts_RemoveAllTags()
        {
            ICreateOrUpdateService<BloggingContext, Blog> CreateOrUpdateService = new CreateOrUpdateService<BloggingContext, Blog>(this._context);
            var result = await CreateOrUpdateService.CreateOrUpdateAsync(new BlogWithPostsCreateDto
            {
                BlogId = 1,
                Name = "Test Update2",
                Url = "https://www.usc.edu",
                Tags = new List<BlogTagDto>()
                {
                    new BlogTagDto {
                        Name = "Test",
                        BlogId = 1
                    }
                }
            }, ActionFlags.Update);
            Assert.Equal(new List<object>() { }, result.Errors);
            Assert.True(result.IsValid);
            Assert.Equal(1, result.Result.BlogId);
            var entity = this._context.Blogs.Where(x => x.BlogId == 1).First();
            Assert.Equal("Test Update2", result.Result.Name);
            Assert.Equal(1, entity.Tags.Count());
        }
    }
    
}
