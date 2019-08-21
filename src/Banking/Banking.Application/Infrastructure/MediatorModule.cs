using System.Reflection;
using Autofac;
using Banking.Application.Behaviors;
using Banking.Application.Commands;
using Banking.Application.Commands.Customers;
using Banking.Application.DomainEventHandlers.AccountClosed;
using Banking.Application.Validations;
using Banking.Domain.Events;
using FluentValidation;
using MediatR;

namespace Banking.Application.Infrastructure
{
    public class MediatorModule : Autofac.Module
    {

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
            .AsImplementedInterfaces();

        // builder.Register<ServiceFactory>(ctx =>
        // {
        //     var c = ctx.Resolve<IComponentContext>();
        //     return t => c.Resolve(t);
        // });


            builder.RegisterAssemblyTypes(typeof(AddAccountCommand).GetTypeInfo().Assembly)
                          .AsClosedTypesOf(typeof(IRequestHandler<,>));

            builder.RegisterAssemblyTypes(typeof(CloseAccountCommand).GetTypeInfo().Assembly)
            .AsClosedTypesOf(typeof(IRequestHandler<,>));

            // Register all the Command classes (they implement IRequestHandler) in assembly holding the Commands
            builder.RegisterAssemblyTypes(typeof(AddCustomerCommand).GetTypeInfo().Assembly)
            .AsClosedTypesOf(typeof(IRequestHandler<,>));


            // Register the DomainEventHandler classes (they implement INotificationHandler<>) in assembly holding the Domain Events
            builder.RegisterAssemblyTypes(typeof(AccountClosedDomainEventHandler).GetTypeInfo().Assembly)
            .AsClosedTypesOf(typeof(INotificationHandler<>));

            // Register the Command's Validators (Validators based on FluentValidation library)
            builder
                .RegisterAssemblyTypes(typeof(AddAccountCommandValidator).GetTypeInfo().Assembly)
                .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces();

            

            builder.Register<ServiceFactory>(context =>
                        {
                            var componentContext = context.Resolve<IComponentContext>();
                            return t => { object o; return componentContext.TryResolve(t, out o) ? o : null; };
                        });


            builder.RegisterGeneric(typeof(TransactionBehaviour<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(ValidatorBehavior<,>)).As(typeof(IPipelineBehavior<,>));

        }
}
}