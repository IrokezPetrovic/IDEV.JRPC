﻿using System;
using System.Configuration;
using Consul;
using JRPC.Client;
using JRPC.Service;
using JRPC.Service.Host.Kestrel;
using JRPC.Service.Registry;

namespace Tests {

    public class ServiceRunner {

        private const string DEFAULT_IP_ADRESS = "localhost";
        private const string DEFAULT_PORT = "58795";

        public static Tuple<JRpcService, T> StartService<T>(string serviceName, JRpcModule jRpcModule, string ipAdress = null, string port = null) where T : class {
            if (string.IsNullOrEmpty(ipAdress)) {
                ipAdress = DEFAULT_IP_ADRESS;
            }
            if (string.IsNullOrEmpty(port)) {
                port = DEFAULT_PORT;
            }

            ConfigurationManager.AppSettings.Set("ServiceAddress", ipAdress);
            ConfigurationManager.AppSettings.Set("ServicePort", port);
            var consulClient = new ConsulClient();
            var registry = new DefaultModulesRegistry();

            var jrpServerHost = new KestrelJRpcServerHost();

            
            registry.AddJRpcModule(jRpcModule);
            
            var service = new JRpcService(registry, consulClient, jrpServerHost);
            service.Start();
            var path = $"http://{ipAdress}:{port}";
            var client = new JRpcClient(path);
            var clientProxy = client.GetProxy<T>(serviceName);
            return Tuple.Create(service, clientProxy);
        }

    }

}