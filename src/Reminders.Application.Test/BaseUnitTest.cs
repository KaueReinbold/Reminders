using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Reminders.Application.Mapper.Extensions;
using Reminders.Application.Test.Fake;
using Reminders.Domain.Contracts;
using Reminders.Domain.Contracts.Repositories;

namespace Reminders.Application.Test
{
    [TestClass]
    public class BaseUnitTest
    {
        protected IRemindersRepository repository;
        protected Mock<IUnitOfWork> unitOfWorkMock;
        protected IMapper mapperMock;

        [TestInitialize]
        public void TestInitialize()
        {
            repository = new RemindersRepositoryFake();
            unitOfWorkMock = new Mock<IUnitOfWork>();
            mapperMock = AutoMapperConfiguration.CreateMapper();

            unitOfWorkMock
                .Setup(u => u.Commit())
                .Returns(true);
        }
    }
}
