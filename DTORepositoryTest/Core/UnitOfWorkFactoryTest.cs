using DTORepository.Core;
using DTORepositoryTest.Samples.Models;
using Moq;
using System.Data.Entity;
using Xunit;
using Xunit.Abstractions;

namespace DTORepositoryTest.Core
{
    public class UnitOfWorkFactoryTest : UnitTestBase
    {
        UnitOfWorkFactory<BloggingContext> _factory;
        
        public UnitOfWorkFactoryTest(ITestOutputHelper output) : base(output)
        {
            BloggingContext context = new Mock<BloggingContext>().Object;
            _factory = new UnitOfWorkFactory<BloggingContext>(context);
        }
        [Fact]
        public void Create_UnitOfWork_With_Correct_GenericType()
        {
            // when
            var UnitOfWork = _factory.CreateUnitOfWork();
            // then
            Assert.IsType<UnitOfWork<BloggingContext>>(UnitOfWork);
        }
    }
}
