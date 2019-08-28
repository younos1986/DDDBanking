# DDDBanking 

    My knowledge about DDD

# domain-driven-design

How to Enrich Domain Models?
I begin following TDD practices, it gives me confidence to enrich the model in different places incrementally. I know two TDD approaches, the inside-out and the outside-in. And to be honest I prefer the inside-out approach, with the guidance of DDD building blocks. The DDD building blocks guides me in the correct path. I start thinking on Entities, Value Objects and Aggregates then I move outside to the Use Cases and Repositories. I am able to discover a lot of domain, design the model without working on database and UI. Next, a short description of what we gonna need from DDD.

Value Objects: its immutable custom types that are distinguishable only by the state of its properties.
Entities: its custom types that are distinguishable by an identity property, it has data and behaviors.
Aggregate roots: a kind on entity that maintain the object graph in consistent state and is associated to a repository.
Use Cases: coordinates the operations with the domain objects and services.
Repositories and Services: provides access to external resources.

reference: https://paulovich.net/rich-domain-model-with-ddd-tdd-reviewed/

<img src='https://raw.githubusercontent.com/younos1986/DDDBanking/master/images/ddd.png' />

# domain-driven-design-language

Account
A bank account allows us to send and receive money and has its unique number. Anytime we tell about an account in a bank, an account is always a bank account. In the other hand, an account in an information system is used to authorize a user. We have the term "account" meaning something absolutely different in two different domains. Domain has an impact on what we imagine when someone says a concrete term. So we have to learn and specify domain terms first.

Price
Let's speak about e-shop domain. What is a price? For us, as customers, it is how much we pay. A manager can think about price as an amount that his company pays to the supplier. For an accountant, a price is just a number. And e-shop programmer is now confused.

Language is crucial because customers and experts are telling their stories in their language. But it is also natural language, inaccurate, ambiguous, context-aware. And as we can see, language can be tricky even within one domain. 

<img src='https://raw.githubusercontent.com/younos1986/DDDBanking/master/images/domain-driven-design-language.png' />

reference: https://pehapkari.cz/blog/2017/12/05/domain-driven-design-language/


# What is a domain event?

An event is something that has happened in the past. A domain event is, something that happened in the domain that you want other parts of the same domain (in-process) to be aware of. The notified parts usually react somehow to the events.

An important benefit of domain events is that side effects can be expressed explicitly.

For example, if you're just using Entity Framework and there has to be a reaction to some event, you would probably code whatever you need close to what triggers the event. So the rule gets coupled, implicitly, to the code, and you have to look into the code to, hopefully, realize the rule is implemented there.

On the other hand, using domain events makes the concept explicit, because there is a DomainEvent and at least one DomainEventHandler involved.

reference: https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation


# Integration events

As described earlier, when you use event-based communication, a microservice publishes an event when something notable happens, such as when it updates a business entity. Other microservices subscribe to those events. When a microservice receives an event, it can update its own business entities, which might lead to more events being published. This is the essence of the eventual consistency concept. This publish/subscribe system is usually performed by using an implementation of an event bus. The event bus can be designed as an interface with the API needed to subscribe and unsubscribe to events and to publish events. It can also have one or more implementations based on any inter-process or messaging communication, such as a messaging queue or a service bus that supports asynchronous communication and a publish/subscribe model.

You can use events to implement business transactions that span multiple services, which gives you eventual consistency between those services. An eventually consistent transaction consists of a series of distributed actions. At each action, the microservice updates a business entity and publishes an event that triggers the next action.

The Catalog microservice using event driven communication through an event bus, to achieve eventual consistency with Basket and additional microservices.

<img src='https://docs.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/media/image19.png'/>


reference: https://docs.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/integration-event-based-microservice-communications


# Design a microservice domain model

reference: https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/microservice-domain-model


# Layers in DDD microservices

reference: https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/ddd-oriented-microservice#layers-in-ddd-microservices

# Banking Sample

<img src='https://raw.githubusercontent.com/younos1986/DDDBanking/master/images/Aggregate.png' />


* Aggregate Roots: Customer and Account.
* Entities: Credit and Debit.
* Value Objects: Amount.
* Use Cases: Add, Deposit, Withdraw, Close, Get Account Status.


### Features
*   DDD
*   CQRS
*   BackgroundTasks
*   APPIGateWay
*   NServiceBus using SQL Server Persistence
*   Dockerized
*   Unit-Test
*   Functional-Test
*   EF-Core2.2

### AccountController.cs

```

[Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        readonly IMediator _mediator;
        public AccountController(IMediator mediator)
        {
            _mediator=mediator;
        }


        [HttpPost, Route("[action]")]
        public async Task<ActionResult> AddAccount([FromBody] AddAccountCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost, Route("[action]")]
        public async Task<ActionResult> Deposit([FromBody] DepositCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost, Route("[action]")]
        public async Task<ActionResult> Withdraw([FromBody] WithdrawCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost , Route("[action]")]
        public async Task<ActionResult> CloseAccount([FromBody] CloseAccountCommand command)
        {
            var result =  await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost, Route("[action]")]
        public async Task<ActionResult> GetAccountStatus([FromBody] GetAccountStatusQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }

```




### AddAccountCommand.cs

```

public class AddAccountCommand : IRequest<bool>
    {
        public AddAccountCommand(long customerId,  string description)
        {
            this.CustomerId = customerId;
            this.Description = description;
        }
        public long CustomerId { get; private set; }
        public string Description { get; private set; }
    }

```



### AddAccountCommandHandler.cs

```

public class AddAccountCommandHandler : IRequestHandler<AddAccountCommand, bool>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IIntegrationEventLogService _integrationEventLogService;
        public AddAccountCommandHandler(
            IAccountRepository accountRepository,
        IIntegrationEventLogService integrationEventLogService)
        {
            _accountRepository= accountRepository;
            _integrationEventLogService=integrationEventLogService;
        }

        public async Task<bool> Handle(AddAccountCommand req, CancellationToken cancellationToken)
        {
            var accountCreatedIntegrationEvent = new AccountCreatedIntegrationEvent(req.CustomerId);
            await _integrationEventLogService.AddAndSaveEventAsync(accountCreatedIntegrationEvent);

            Account account = new Account(req.CustomerId , "the account is opened");

            await _accountRepository.CreateAsync(account);
            return await _accountRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }

```





### Account.cs
```

public class Account : Entity, IAggregateRoot
    {

        protected Account()
        {
            _credits = new List<Credit>();
            _debits = new List<Debit>();
        }

        public Account(long customerId, string description)
        {
            // this.Id = 1;
            this.CustomerId = customerId;
            this._accountStatusId = AccountStatus.Opened.Id;
            this.Description=description;

            _credits = new List<Credit>();
            _debits = new List<Debit>();

            this.AddDomainEvent(new AccountAddedDomainEvent(Id, CustomerId));

        }
        public long CustomerId { get; private set; }

        public string Description { get; private set; }



        public virtual AccountStatus AccountStatus {get; private set;}
        
        public AccountStatus GetAccountStatus => AccountStatus.From(_accountStatusId);

        private  int _accountStatusId;

        private readonly List<Credit> _credits;
        private readonly List<Debit> _debits;
        public virtual IReadOnlyCollection<Credit> Credits => _credits;
        public virtual IReadOnlyCollection<Debit> Debits => _debits;

        public Credit Deposit(Amount amount)
        {
            Credit c = new Credit(Id , amount, DateTime.Now , "description");
            _credits.Add(c);
            return c;
        }

        public Debit Withdraw(Amount amount)
        {
            Amount balance = GetBalance();

            if (amount > balance)
                throw new NoSufficientBalanceException($"The account {Id} doesn't have sufficient balance. Its balance is {balance}");

            Debit d = new Debit(Id, amount, DateTime.Now, "description"); ;
            _debits.Add(d);
            return d;
        }

        public void Close()
        {
            Amount balance = GetBalance();

            if (balance > 0)
                throw new AccountCannotBeClosedException($"The account {Id} cannot be closed.Current balance is {balance} ");

            //this.AccountStatus = AccountStatus.Closed;
            this._accountStatusId = AccountStatus.Closed.Id;
            this.Description = "the account is closed";
            
            this.AddDomainEvent(new AccountClosedDomainEvent(Id , CustomerId));

        }

        public void CheckIfAccountIsAllowedToGetLoan(Amount amount)
        {
            Amount balance = GetBalance();

            if (balance < 10_000)
                throw new YouAreNotAllowedTogetLoanException("");

            //todo: some other stuff
        }

        public Amount GetBalance()
        {
            Amount balance = 0;
            foreach (var c in Credits)
            {
                balance = balance + c.Amount;
            }
            
            foreach (var d in Debits)
            {
                balance = balance - d.Amount;
            }
            return balance;
        }

    }

```



