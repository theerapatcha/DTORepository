using DTORepository.Core;
using DTORepositoryTest.Samples.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DTORepositoryTest.Core
{
    public class RepositoryFactoryTest
    {
        RepositoryFactory<BloggingContext> _factory;
        
        public RepositoryFactoryTest()
        {
            BloggingContext context = new Mock<BloggingContext>().Object;
            _factory = new RepositoryFactory<BloggingContext>(context);
        }
        [Fact]
        public void Create_Repository_With_Correct_GenericType()
        {
            // when
            var repository = _factory.CreateRepository<Blog>();
            // then
            Assert.IsType<Repository<BloggingContext, Blog>>(repository);
        }
    }
}
