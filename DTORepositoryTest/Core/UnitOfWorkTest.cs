using DTORepository.Core;
using DTORepositoryTest.Samples.Dtos;
using DTORepositoryTest.Samples.Models;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace DTORepositoryTest.Core
{
    public class UnitOfWorkTest : UnitTestBase
    {
        RepositoryFactory<BloggingContext> repositoryFactory;
        UnitOfWorkFactory<BloggingContext> unitOfWorkFactory;
        public UnitOfWorkTest(ITestOutputHelper output) : base(output) 
        {
            repositoryFactory = new RepositoryFactory<BloggingContext>(this._context);
            unitOfWorkFactory = new UnitOfWorkFactory<BloggingContext>(this._context);
        }
        [Fact]
        public async Task ExecuteAsync_Will_Rollback_Transaction_If_One_Failed()
        {
            var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();
            var blogRepository = repositoryFactory.CreateRepository<Blog>();

            // try update first to check it is still there after the rollback has happened
            var initialUpdate = blogRepository.CreateOrUpdate(new BlogUpdateDto
            {
                BlogId = 1, // exist id
                Name = "Update1"
            });
            Assert.True(initialUpdate.IsValid);
            Assert.Equal("Update1", initialUpdate.Result.Name);

            var status = await unitOfWork.ExecuteAsync(async (context) =>
            {
                // Try Update
                var innerStatus = await blogRepository.CreateOrUpdateAsync(new BlogUpdateDto
                {
                    BlogId = 1, // exist id
                    Name = "Cannot update"
                });
                Assert.True(innerStatus.IsValid);
                var tmpBlog = ((BloggingContext)context).Blogs.Find(1);
                Assert.Equal("Cannot update", tmpBlog.Name);

                // Try Create
                innerStatus.Combine(blogRepository.Create(new BlogUpdateDto
                {
                    BlogId = 9,
                    Name = "Cannot update"
                }));
                Assert.False(innerStatus.IsValid);

                return innerStatus;
            });
            
            // check rollback value
            Assert.False(status.IsValid);
            var blog = blogRepository.Get<BlogDto>(1).Result;
            Assert.Equal("Update1", blog.Name);
        }
        [Fact]
        public void Execute_Will_Rollback_Transaction_If_One_Failed()
        {
            var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();
            var blogRepository = repositoryFactory.CreateRepository<Blog>();

            // try update first to check it is still there after the rollback has happened
            var initialUpdate = blogRepository.CreateOrUpdate(new BlogUpdateDto
            {
                BlogId = 1, // exist id
                Name = "Update1"
            });
            Assert.True(initialUpdate.IsValid);
            Assert.Equal("Update1", initialUpdate.Result.Name);

            var status = unitOfWork.Execute((context) =>
            {
                // Try Update
                var innerStatus = blogRepository.CreateOrUpdate(new BlogUpdateDto
                {
                    BlogId = 1, // exist id
                    Name = "Cannot update"
                });
                Assert.True(innerStatus.IsValid);
                var tmpBlog = ((BloggingContext)context).Blogs.Find(1);
                Assert.Equal("Cannot update", tmpBlog.Name);

                // Try Create
                innerStatus.Combine(blogRepository.Create(new BlogUpdateDto
                {
                    BlogId = 9,
                    Name = "Cannot update"
                }));
                Assert.False(innerStatus.IsValid);

                return innerStatus;
            });
            
            // check rollback value
            Assert.False(status.IsValid);
            var blog = blogRepository.Get<BlogDto>(1).Result;
            Assert.Equal("Update1", blog.Name);
        }
    }
}
