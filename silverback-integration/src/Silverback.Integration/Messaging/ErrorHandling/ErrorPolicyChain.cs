﻿// Copyright (c) 2018 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Silverback.Messaging.Messages;

namespace Silverback.Messaging.ErrorHandling
{
    /// <summary>
    /// A chain of error policies to be applied one after another.
    /// </summary>
    public class ErrorPolicyChain : ErrorPolicyBase
    {
        private readonly ILogger<ErrorPolicyChain> _logger;
        private readonly ErrorPolicyBase[] _policies;

        public ErrorPolicyChain(ILogger<ErrorPolicyChain> logger, params ErrorPolicyBase[] policies)
            : this (policies.AsEnumerable(), logger)
        {
        }

        public ErrorPolicyChain(IEnumerable<ErrorPolicyBase> policies, ILogger<ErrorPolicyChain> logger)
            : base(logger)
        {
            _logger = logger;

            if (policies == null) throw new ArgumentNullException(nameof(policies));
            _policies = policies.ToArray();

            if (_policies.Any(p => p == null)) throw new ArgumentNullException(nameof(policies), "One or more policies in the chain have a null value.");
        }

        public override ErrorAction HandleError(IMessage failedMessage, int retryCount, Exception exception)
        {
            foreach (var policy in _policies)
            {
                if (policy.CanHandle(failedMessage, retryCount, exception))
                    return policy.HandleError(failedMessage, retryCount, exception);
            }

            _logger.LogTrace("All policies have been applied but the message still couldn't be successfully processed. The consumer will be stopped.");
            return ErrorAction.StopConsuming;
        }
    }
}