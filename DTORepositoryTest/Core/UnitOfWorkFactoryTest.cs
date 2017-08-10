using DTORepository.Core;
using Moq;
using System.Data.Entity;
using Xunit;
using Xunit.Abstractions;

namespace DTORepositoryTest.Core
{
    public class UnitOfWorkFactoryTest : UnitTestBase
    {
        UnitOfWorkFactory _factory;
        
        public UnitOfWorkFactoryTest(ITestOutputHelper output) : base(output)
        {
            DbContext context = new Mock<DbContext>().Object;
            _factory = new UnitOfWorkFactory(context);
        }
        [Fact]
        public void Create_UnitOfWork_With_Correct_GenericType()
        {
            // when
            var UnitOfWork = _factory.CreateUnitOfWork();
            // then
            Assert.IsType<UnitOfWork>(UnitOfWork);
        }
    }
}
