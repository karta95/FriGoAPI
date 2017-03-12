﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using FriGo.DAL;
using FriGo.Db.Models;
using FriGo.Interfaces.Dependencies;
using FriGo.Services;

namespace FriGo.Api
{
    public class Bootstrapper
    {        
        public static void Run()
        {
            Assembly[] otherAssemblies = GetAssemblies();            
        
            var builder = new ContainerBuilder();
            HttpConfiguration config = GlobalConfiguration.Configuration;

            builder.RegisterApiControllers(otherAssemblies.First());
            builder.RegisterWebApiFilterProvider(config);

            RegisterTypes(otherAssemblies, builder);

            IContainer container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private static Assembly[] GetAssemblies()
        {
            return new[]
            {
                Assembly.GetExecutingAssembly(),

                Assembly.GetAssembly(typeof(BaseService)),
                Assembly.GetAssembly(typeof(Entity)),
                Assembly.GetAssembly(typeof(UnitOfWork)),
            };
        }

        private static void RegisterTypes(IEnumerable<Assembly> assemblies, ContainerBuilder builder)
        {
            foreach (Assembly assembly in assemblies)
            {
                RegisterRequestDependencies(builder, assembly);
                RegisterLifeTimeDependencies(builder, assembly);
                RegisterSingleInstanceDependencies(builder, assembly);
                RegisterMatchingLifeTimeDependency(builder, assembly);
                RegisterSelfDependency(builder, assembly);
            }
        }

        private static void RegisterRequestDependencies(ContainerBuilder builder, Assembly assembly)
        {
            IEnumerable<Type> types = assembly.GetTypes().Where(t => typeof(IRequestDependency).IsAssignableFrom(t)).ToList();

            builder.RegisterTypes(types.ToArray()).AsImplementedInterfaces().InstancePerRequest();
        }

        private static void RegisterLifeTimeDependencies(ContainerBuilder builder, Assembly assembly)
        {
            IEnumerable<Type> types = assembly.GetTypes().Where(t => typeof(ILifeTimeDependency).IsAssignableFrom(t)).ToList();

            builder.RegisterTypes(types.ToArray()).AsImplementedInterfaces().InstancePerLifetimeScope();
        }

        private static void RegisterSingleInstanceDependencies(ContainerBuilder builder, Assembly assembly)
        {
            IEnumerable<Type> types = assembly.GetTypes().Where(t => typeof(ISingleInstanceDependency).IsAssignableFrom(t)).ToList();

            builder.RegisterTypes(types.ToArray()).AsImplementedInterfaces().SingleInstance();
        }

        private static void RegisterMatchingLifeTimeDependency(ContainerBuilder builder, Assembly assembly)
        {
            IEnumerable<Type> types = assembly.GetTypes().Where(t => typeof(IMatchingLifeTimeDependency).IsAssignableFrom(t)).ToList();

            builder.RegisterTypes(types.ToArray()).AsImplementedInterfaces().InstancePerMatchingLifetimeScope();
        }

        private static void RegisterSelfDependency(ContainerBuilder builder, Assembly assembly)
        {
            IEnumerable<Type> types = assembly.GetTypes().Where(t => typeof(ISelfRequestDependency).IsAssignableFrom(t)).ToList();

            builder.RegisterTypes(types.ToArray()).AsSelf().InstancePerRequest();
        }        
    }
}