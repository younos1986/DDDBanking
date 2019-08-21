using System;
using System.Threading;
using System.Threading.Tasks;
using Banking.Application.Commands;
using Banking.Application.Services;
using Banking.Domain.Accounts;
using Banking.Domain.DomainExceptions;
using Banking.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using Xunit;

namespace Banking.Domain.Tests.Application
{
    public class AddAccountCommandHandlerTest
    {

        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<IIntegrationEventLogService> _integrationEventLogServiceMock;

        public AddAccountCommandHandlerTest()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _integrationEventLogServiceMock = new Mock<IIntegrationEventLogService>();
        }


        [Fact]
        public async Task Handle_return_true()
        {
           // setup
            _accountRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Account>()))
                           .Returns(Task.FromResult<Account>(FakeAccount()));

            _accountRepositoryMock.Setup(repo => repo.UnitOfWork.SaveEntitiesAsync(default(CancellationToken)))
            .Returns(Task.FromResult<bool>(true));


            // action
            var handler = new AddAccountCommandHandler(_accountRepositoryMock.Object , _integrationEventLogServiceMock.Object);
            var cltToken = new System.Threading.CancellationToken();
            var result = await handler.Handle(new AddAccountCommand(1 , "account description") , cltToken);

            // assert
            Assert.True(result);
        }

        private Account FakeAccount()
        {
            return new Account(1 , "account description");
        }
    }
}
