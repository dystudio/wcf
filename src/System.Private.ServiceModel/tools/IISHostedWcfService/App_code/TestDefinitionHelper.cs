﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace WcfService
{
    internal class TestDefinitionHelper
    {
        private static IDictionary<ServiceSchema, string> s_baseAddresses = null;
        private const int DefaultHttpPort = 8081;
        private const int DefaultHttpsPort = 44285;
        private const int DefaultTcpPort = 809;
        private const int DefaultWebSocketPort = 8083;
        private const int DefaultWebSocketSPort = 8084;

        private static IDictionary<ServiceSchema, string> BaseAddresses
        {
            get
            {
                if (s_baseAddresses == null)
                {
                    IDictionary<ServiceSchema, string> dict = new Dictionary<ServiceSchema, string>();
                    var httpPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("httpPort")) ? DefaultHttpPort : int.Parse(Environment.GetEnvironmentVariable("httpPort"));
                    dict[ServiceSchema.HTTP] = string.Format(@"http://localhost:{0}", httpPort);
                    var httpsPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("httpsPort")) ? DefaultHttpsPort : int.Parse(Environment.GetEnvironmentVariable("httpsPort"));
                    dict[ServiceSchema.HTTPS]= string.Format(@"https://localhost:{0}", httpsPort);
                    var tcpPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("tcpPort")) ? DefaultTcpPort : int.Parse(Environment.GetEnvironmentVariable("tcpPort"));
                    dict[ServiceSchema.NETTCP] = string.Format(@"net.tcp://localhost:{0}", tcpPort);
                    var websocketPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("websocketPort")) ? DefaultWebSocketPort : int.Parse(Environment.GetEnvironmentVariable("websocketPort"));
                    dict[ServiceSchema.WS] = string.Format(@"http://localhost:{0}", websocketPort);
                    var websocketsPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("websocketsPort")) ? DefaultWebSocketSPort : int.Parse(Environment.GetEnvironmentVariable("websocketsPort"));
                    dict[ServiceSchema.WSS] = string.Format(@"https://localhost:{0}", websocketsPort);
                    s_baseAddresses = dict;
                    Console.WriteLine("Using base addresses:");
                    foreach(var ba in dict.Values)
                    {
                        Console.WriteLine("\t" + ba);
                    }
                }

                return s_baseAddresses;
            }
        }

        internal static void StartHosts()
        {
            foreach (var sht in GetAttributedServiceHostTypes())
            {
                foreach(TestServiceDefinitionAttribute attr in sht.GetCustomAttributes(typeof(TestServiceDefinitionAttribute), false))
                {
                    string serviceBaseAddress = string.Empty;
                    try
                    {
                        serviceBaseAddress = string.Format("{0}/{1}", BaseAddresses[attr.Schema], attr.BasePath);
                        var serviceHost = (ServiceHostBase)Activator.CreateInstance(sht, new Uri[] { new Uri(serviceBaseAddress) });
                        Console.WriteLine("  {0} at {1}", sht.Name, serviceBaseAddress);
                        serviceHost.Open();
                    }
                    catch (Exception e)
                    {
                        ConsoleColor bg = Console.BackgroundColor;
                        ConsoleColor fg = Console.ForegroundColor;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Problem starting servicehost type " + sht.Name + " with schema " + attr.Schema + " and address " + serviceBaseAddress);
                        Console.WriteLine(e);
                        Console.BackgroundColor = bg;
                        Console.ForegroundColor = fg;
                    }
                }
            }
        }

        internal static IEnumerable<Type> GetAttributedServiceHostTypes()
        {
            var allTypes = typeof(TestDefinitionHelper).Assembly.GetTypes();
            var serviceHostTypes = from t in allTypes where (typeof(ServiceHostBase).IsAssignableFrom(t)) select t;
            return from sht in serviceHostTypes where (sht.GetCustomAttributes(typeof(TestServiceDefinitionAttribute), false).Length > 0) select sht;
        }
    }
}
