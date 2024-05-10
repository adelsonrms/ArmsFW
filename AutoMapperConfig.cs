using ArmsFW.Core;
using ArmsFW.Services.Shared.Settings;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;



namespace ArmsFW.AutoMapperExtensions
{
    public static class AutoMapperConfiguration
    {
        public static IServiceCollection RegistrarAutoMapper(this IServiceCollection services, params Type[] profileAssemblyMarkerTypes)
        {
            services.AddAutoMapper(profileAssemblyMarkerTypes);

            var ass = new List<System.Reflection.Assembly>() { Aplicacao.Assembly };

            if (profileAssemblyMarkerTypes != null)
            {
                ass.AddRange(profileAssemblyMarkerTypes.ToList().Select(a => a.Assembly));
            }

            //Configura os perfies de mapeamentos deste assembly
            AutoMapperConfiguration.Initialize(ass);

            return services;
        }

        private static readonly object ConcurrentLock = new object();

        public static IMapper Mapper { get; private set; }

        public static List<Assembly> AssembliesProfile { get; private set; }

        public static MapperConfiguration MapperConfiguration { get; private set; }

        private static bool _initialized;

        public static void Initialize(List<Assembly> AssembliesProfile)
        {
            List<Type> perfis;
            lock (ConcurrentLock)

                if (!_initialized)
                {
                    MapperConfiguration = new MapperConfiguration(cfg => {

                        //Carrega todos os perfis de automapper configurados Neste Assembly. (classes que herdam da classe base Profile do AutoMapper)
                        perfis = App.CarregarClasses<Profile>(Aplicacao.Assembly);

                        //Se informado, carrega os type de outros assemblyes
                        if (AssembliesProfile != null) AssembliesProfile.ForEach(x => perfis.AddRange(App.CarregarClasses<Profile>(x)));

                        if (perfis.Any())
                        {
                            perfis.ForEach(p => cfg.AddProfile(App.Instanciar<Profile>(p)));
                        }

                    });
                    _initialized = true;
                }

            Mapper = MapperConfiguration.CreateMapper();

        }
    }

    public static class Mapper
    {
        public static SourceType Map<SourceType>(this SourceType obj)
        {
            var entity = AutoMapperConfiguration.Mapper.Map<SourceType>(obj);
            return entity;
        }

        public static DestinationType Map<SourceType, DestinationType>(this SourceType obj)
        {
            var entity = AutoMapperConfiguration.Mapper.Map<DestinationType>(obj);
            return entity;
        }

        public static List<DestinationType> Map<SourceType, DestinationType>(this List<SourceType> obj)
        {
            var entity = AutoMapperConfiguration.Mapper.Map<List<DestinationType>>(obj);

            return entity;
        }

        public static IEnumerable<DestinationType> Map<SourceType, DestinationType>(this IEnumerable<SourceType> obj)
        {
            var entity = AutoMapperConfiguration.Mapper.Map<IEnumerable<DestinationType>>(obj);

            return entity;
        }

        public static DestinationType Map<SourceType, DestinationType>(this SourceType source, DestinationType dest)
        {
            var entity = AutoMapperConfiguration.Mapper.Map(source, dest);
            return entity;
        }
    }
}
