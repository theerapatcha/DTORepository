using DTORepository.Core;
using Moq;
using System.Data.Entity;
using Xunit;

namespace DTORepositoryTest.Core
{
    public class UnitOfWorkFactoryTest
    {
        UnitOfWorkFactory _factory;
        
        public UnitOfWorkFactoryTest()
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
