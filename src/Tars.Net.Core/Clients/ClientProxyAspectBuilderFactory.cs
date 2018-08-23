﻿using AspectCore.DynamicProxy;
using System;
using System.Reflection;

namespace Tars.Net.Clients.Proxy
{
    [NonAspect]
    public class ClientProxyAspectBuilderFactory : IAspectBuilderFactory
    {
        private readonly IInterceptorCollector interceptorCollector;
        private readonly IRpcClientInvokeBuilder invokeBuilder;
        private readonly IAspectCaching aspectCaching;

        public ClientProxyAspectBuilderFactory(IInterceptorCollector interceptorCollector,
            IAspectCachingProvider aspectCachingProvider, IRpcClientInvokeBuilder invokeBuilder)
        {
            if (aspectCachingProvider == null)
            {
                throw new ArgumentNullException(nameof(aspectCachingProvider));
            }
            this.interceptorCollector =
                interceptorCollector ?? throw new ArgumentNullException(nameof(interceptorCollector));
            this.invokeBuilder = invokeBuilder;
            aspectCaching = aspectCachingProvider.GetAspectCaching(nameof(ClientProxyAspectBuilderFactory));
        }

        public IAspectBuilder Create(AspectContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            return (IAspectBuilder)aspectCaching.GetOrAdd(GetKey(context.ServiceMethod, context.ImplementationMethod), key => Create((Tuple<MethodInfo, MethodInfo>)key));
        }

        private object GetKey(MethodInfo serviceMethod, MethodInfo implementationMethod)
        {
            return Tuple.Create(serviceMethod, implementationMethod);
        }

        private IAspectBuilder Create(Tuple<MethodInfo, MethodInfo> tuple)
        {
            var aspectBuilder = new AspectBuilder(context => context.Complete(), null);
            aspectBuilder.AddAspectDelegate(invokeBuilder.Build(tuple.Item2));
            foreach (var interceptor in interceptorCollector.Collect(tuple.Item1, tuple.Item2))
            {
                aspectBuilder.AddAspectDelegate(interceptor.Invoke);
            }

            return aspectBuilder;
        }
    }
}