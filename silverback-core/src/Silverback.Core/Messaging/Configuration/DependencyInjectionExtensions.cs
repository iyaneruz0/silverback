﻿// Copyright (c) 2018 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System;
using Silverback.Messaging.Configuration;
using Silverback.Messaging.Publishing;
using Silverback.Messaging.Subscribers;
using Silverback.Messaging.Subscribers.ArgumentResolvers;
using Silverback.Messaging.Subscribers.ReturnValueHandlers;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddBus(this IServiceCollection services, Action<BusPluginOptions> optionsAction = null)
        {

            var pluginOptions = new BusPluginOptions(services);
            optionsAction?.Invoke(pluginOptions);

            return services
                .AddScoped<IPublisher, Publisher>()
                .AddSingleton<BusOptions>()
                .AddSingleton<SubscribedMethodInvoker>()
                .AddSingleton<SubscribedMethodArgumentsResolver>()
                .AddSingleton<IArgumentResolver, EnumerableMessageArgumentResolver>()
                .AddSingleton<IArgumentResolver, SingleMessageArgumentResolver>()
                .AddSingleton<IArgumentResolver, ServiceProviderAdditionalArgumentResolver>()
                .AddSingleton<ReturnValueHandler>()
                .AddSingleton<IReturnValueHandler, EnumerableMessagesReturnValueHandler>()
                .AddSingleton<IReturnValueHandler, SingleMessageReturnValueHandler>()
                // TODO: Move to another package
                .AddScoped<IEventPublisher, EventPublisher>()
                .AddScoped<ICommandPublisher, CommandPublisher>()
                .AddScoped<IRequestPublisher, RequestPublisher>()
                .AddScoped<IQueryPublisher, QueryPublisher>();
        }
    }
}