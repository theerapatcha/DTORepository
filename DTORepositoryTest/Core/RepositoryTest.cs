using DTORepository.Common;
using DTORepository.Core;
using DTORepository.Services;
using DTORepositoryTest.Samples.Dtos;
using DTORepositoryTest.Samples.Models;
using Moq;
using System.Data.Entity;
using Xunit;

namespace DTORepositoryTest.Core
{
    public class RepositoryTest
    {
        IRepository<Blog> _repository;
        Mock<ICreateOrUpdateService<Blog>> m_CreateOrUpdateService;
        Mock<IDetailService<Blog>> m_DetailService;
        Mock<IListService<Blog>> m_ListService;
        Mock<IDeleteService<Blog>> m_DeleteService;
        public RepositoryTest()
        {
            DbContext context = new Mock<DbContext>().Object;
            m_CreateOrUpdateService = new Mock<ICreateOrUpdateService<Blog>>();
            m_DetailService = new Mock<IDetailService<Blog>>();
            m_ListService = new Mock<IListService<Blog>>();
            m_DeleteService = new Mock<IDeleteService<Blog>>();
            _repository = new Repository<Blog>(m_CreateOrUpdateService.Object, m_DetailService.Object, m_ListService.Object, m_DeleteService.Object);
        }
        [Fact]
        public void WhenCall_Get_Will_Trigger_DetailService_Get()
        {
            // when
            _repository.Get<BlogDto>(1);
            // then
            m_DetailService.Verify(service => service.Get<BlogDto>(1), Times.Once);
        }
        [Fact]
        public void WhenCall_List_Will_Trigger_ListService_List()
        {
            // when
            _repository.List<BlogDto>(x=>x.BlogId == 1);
            // then
            m_ListService.Verify(service => service.List<BlogDto>(x => x.BlogId == 1), Times.Once);
        }
        [Fact]
        public void WhenCall_List_Will_Trigger_ListService_List_No_Predicate()
        {
            // when
            _repository.List<BlogDto>();
            // then
            m_ListService.Verify(service => service.List<BlogDto>(null), Times.Once);
        }
        [Fact]
        public void WhenCall_Query_Will_Trigger_ListService_Query()
        {
            // when
            _repository.Query<BlogDto>(x => x.BlogId == 1);
            // then
            m_ListService.Verify(service => service.Query<BlogDto>(x => x.BlogId == 1), Times.Once);
        }
        [Fact]
        public void WhenCall_Query_Will_Trigger_ListService_Query_No_Predicate()
        {
            // when
            _repository.Query<BlogDto>();
            // then
            m_ListService.Verify(service => service.Query<BlogDto>(null), Times.Once);
        }
        [Fact]
        public void WhenCall_Insert_Will_Trigger_CreateOrUpdateService_CreateOrUpdate_FalseUpsert()
        {
            // given
            var dto = new BlogDto();
            // when
            _repository.Create(dto);
            // then
            m_CreateOrUpdateService.Verify(service => service.CreateOrUpdate<BlogDto>(dto, ActionFlags.Create), Times.Once);
        }
        [Fact]
        public void WhenCall_Update_Will_Trigger_CreateOrUpdateService_CreateOrUpdate_TrueUpsert()
        {
            // given
            var dto = new BlogDto();
            // when
            _repository.Update(dto);
            // then
            m_CreateOrUpdateService.Verify(service => service.CreateOrUpdate<BlogDto>(dto, ActionFlags.Update), Times.Once);
        }
        [Fact]
        public void WhenCall_CreateOrUpdate_Will_Trigger_CreateOrUpdateService_CreateOrUpdate_TrueUpsert()
        {
            // given
            var dto = new BlogDto();
            // when
            _repository.CreateOrUpdate(dto);
            // then
            m_CreateOrUpdateService.Verify(service => service.CreateOrUpdate<BlogDto>(dto, ActionFlags.Create | ActionFlags.Update), Times.Once);
        }
    }
}
