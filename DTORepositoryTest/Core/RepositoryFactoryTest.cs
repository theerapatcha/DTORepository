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
        RepositoryFactory _factory;
        
        public RepositoryFactoryTest()
        {
            DbContext context = new Mock<DbContext>().Object;
            _factory = new RepositoryFactory(context);
        }
        [Fact]
        public void Create_Repository_With_Correct_GenericType()
        {
            // when
            var repository = _factory.CreateRepository<Blog>();
            // then
            Assert.IsType<Repository<Blog>>(repository);
        }
    }
}
