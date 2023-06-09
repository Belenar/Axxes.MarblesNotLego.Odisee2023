﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Axxes.ToyCollector.Core.Contracts.DependencyResolution;
using Microsoft.Extensions.DependencyInjection;

namespace Axxes.ToyCollector.DependencyResolution
{
    public static class DependencyLoaderExtensions
    {
        /// <summary>
        /// Loads the DLL's and scans their types using Reflection. If a class implementing
        /// <see cref="ITypeRegistrar"/> is found, it is executed, thus adding these types to
        /// the DI container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to register the types in.</param>
        /// <param name="inheritedTypesRegistry">The <see cref="IInheritedTypesRegistry"/>> to resolve inherited types.</param>
        /// <param name="dllFiles">The paths of all dll files to scan.</param>
        public static void LoadConfiguredTypesFromFiles(
            this IServiceCollection services, IInheritedTypesRegistry inheritedTypesRegistry, IEnumerable<string> dllFiles)
        {
            var container = new TypeRegistrationContainer(services);

            foreach (var dllFile in dllFiles)
            {
                LoadAssembly(container, inheritedTypesRegistry, dllFile);
            }
        }

        private static void LoadAssembly(ITypeRegistrationContainer container, IInheritedTypesRegistry inheritedTypesRegistry, string dllFile)
        { 
            var assembly = Assembly.LoadFile(dllFile);

            var types = assembly.GetTypes();

            foreach (var registrarType in types
                .Where(t => typeof(ITypeRegistrar).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract))       
            {
                RunRegistrar(container, registrarType);
            }

            foreach (var typeRegistrarType in types
                .Where(t => typeof(IInheritedTypeRegistrar).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract))
            {
                RunInheritedTypesRegistrar(inheritedTypesRegistry, typeRegistrarType);
            }
        }

        private static void RunRegistrar(ITypeRegistrationContainer container, Type registrarType)
        {
            var registrar = (ITypeRegistrar)Activator.CreateInstance(registrarType);

            registrar.RegisterServices(container);
        }

        private static void RunInheritedTypesRegistrar(IInheritedTypesRegistry typeRegistry, Type typeRegistrarType)
        {
            var registrar = (IInheritedTypeRegistrar)Activator.CreateInstance(typeRegistrarType);

            registrar.RegisterInheritedTypes(typeRegistry);
        }
    }
}
