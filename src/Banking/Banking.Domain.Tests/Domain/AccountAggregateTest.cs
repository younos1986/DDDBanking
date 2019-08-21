using System;
using Banking.Domain.Accounts;
using Banking.Domain.DomainExceptions;
using Banking.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Banking.Domain.Tests.Domain
{
    public class AccountAggregateTest
    {


        [Fact]
        public void New_account_balance_equals_zero()
        {
            // Arrange
            var customerId = 1;
            Account account = new Account(customerId, "account description");

            // Act

            // Assert
            Assert.Equal(account.GetBalance(), 0);

        }

        [Fact]
        public void Withdraw_deposit_1000_balance_equals_zero()
        {
            // Arrange
            var customerId = 1;
            Account account  = new Account(customerId , "account description");

            // Act
            account.Deposit(1000); 
            account.Withdraw(1000);

            // Assert
            Assert.Equal(account.GetBalance() , 0);

        }

        [Fact]
        public void Deposit_1000_balance_equals_1000()
        {
            // Arrange
            var customerId = 1;
            Account account = new Account(customerId, "account description");

            // Act
            account.Deposit(1000);

            // Assert
            Assert.Equal(account.GetBalance(), 1000);

        }

        [Fact]
        public void When_closing_account_then_status_is_closed()
        {
            // Arrange
            var customerId = 1;
            string description = "account is opened";
            Account account  = new Account(customerId , description);

            // Act
            account.Close();

            // Assert
            Assert.Equal(account.GetAccountStatus , AccountStatus.Closed);
            account.GetAccountStatus.Should().Be(AccountStatus.Closed);

        }

        [Fact]
        public void Closing_account_with_positive_balance_throws_accountCannotBeClosedException()
        {
            // Arrange
            var customerId = 1;
            string description = "account is opened";
            Account account = new Account(customerId,  description);

            account.Deposit(100);

            // Act & Assert
            Assert.Throws<AccountCannotBeClosedException>(() => account.Close());
        }

        [Fact]
        public void Withdraw_account_with_lessBalance_throws_noSufficientBalanceException()
        {
            // Arrange
            var customerId = 1;
            string description = "account is opened";
            Account account = new Account(customerId, description);


            // Act & Assert
            Assert.Throws<NoSufficientBalanceException>(() => account.Withdraw(100));
        }


        [Fact]
        public void Closing_account_raises_accountClosedDomainEvent()
        {
            // Arrange
            var customerId = 1;
            string description = "account is opened";
            Account account = new Account(customerId, description);

            // act
            account.Close();

            // Act & Assert
            Assert.True(account.GetRecentlyAddedDomainEvent() is Banking.Domain.Events.AccountClosedDomainEvent);
        }

        [Fact]
        public void Adding_account_raises_accountAddedDomainEvent()
        {
            // Arrange
            var customerId = 1;
            string description = "account is opened";
            Account account = new Account(customerId, description);

            // Act & Assert
            Assert.True(account.GetRecentlyAddedDomainEvent() is Banking.Domain.Events.AccountAddedDomainEvent);
        }

    }
}
