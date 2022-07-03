using System.Reflection;
using System;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Mappings;
using MediatR;
using Ordering.Application.Behaviors;

namespace Ordering.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //Use reflection to check all the mapping profiles which inherit from auto mapper profile
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            //Use reflection to check all the validators which inherit from AbstractValidation<T> (which implements IValidator<T> interface)
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            //Use reflecttion to check all the MediatR handlers which implement IRequestHandler<,>
            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            
            return services;
        }
        
    }
}
