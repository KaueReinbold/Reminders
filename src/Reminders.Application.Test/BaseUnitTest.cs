using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Reminders.Application.Mapper.Extensions;
using Reminders.Application.Test.MockData;
using Reminders.Domain.Contracts;
using Reminders.Domain.Contracts.Repositories;
using Reminders.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reminders.Application.Test
{
    [TestClass]
    public class BaseUnitTest
    {
        protected Mock<IRemindersRepository> repositoryMock;
        protected Mock<IUnitOfWork> unitOfWorkMock;
        protected IMapper mapperMock;
        protected static IQueryable<Reminder> reminders;

        [TestInitialize]
        public void TestInitialize()
        {
            repositoryMock = new Mock<IRemindersRepository>();
            unitOfWorkMock = new Mock<IUnitOfWork>();
            mapperMock = AutoMapperConfiguration.CreateMapper();

            unitOfWorkMock.Setup(u => u.Commit()).Returns(true);

            reminders = ReminderMock.Reminders;
        }
    }
}
