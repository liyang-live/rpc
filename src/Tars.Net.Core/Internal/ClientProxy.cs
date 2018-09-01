﻿using System;
using System.Linq;
using System.Reflection;
using AspectCore.Extensions.Reflection;
using Tars.Net.Attributes;
using Tars.Net.Clients;

namespace Tars.Net.Internal
{
    public class ClientProxy : DispatchProxy
    {
        private IRpcClientFactory clientFactory;

        protected override object Invoke(MethodInfo method, object[] parameters)
        {
            var attribute = method.DeclaringType.GetCustomAttribute<RpcAttribute>();
            var isOneway = method.GetReflector().IsDefined<OnewayAttribute>();
            var outParameters = method.GetParameters().Where(i => i.IsOut).ToArray();
            return clientFactory.SendAsync(attribute.ServantName, method.Name, outParameters, method.ReturnParameter, isOneway, attribute.Codec, parameters).Result;
        }

        private void SetClientFactory(IRpcClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public static object Create(Type type, IRpcClientFactory rpcClientFactory)
        {
            MethodInfo mi = typeof(DispatchProxy).GetMethod(nameof(DispatchProxy.Create), new Type[] { });
            mi = mi.MakeGenericMethod(type, typeof(ClientProxy));
            var clientProxy = mi.Invoke(null, null);

            ((ClientProxy)clientProxy).SetClientFactory(rpcClientFactory);
            return clientProxy;
        }
    }
}