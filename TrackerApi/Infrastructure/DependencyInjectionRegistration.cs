using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data.Common;
using TrackerApi.Core.Interfaces.Context;
using TrackerApi.Core.Interfaces.Repositories;
using TrackerApi.Infrastructure.Context;
using TrackerApi.Infrastructure.Repository;

namespace TrackerApi.Infrastructure
{
    public static class DependencyInjectionRegistration
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddScoped<DbConnection>(con =>  new NpgsqlConnection(configuration.GetConnectionString("TrackerDb")))
                .AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped(typeof(IRepositoryDb<>), typeof(RepositoryDb<>))
                .AddScoped<IUsersRepository, UsersRepository>();


            return services;
        }
    }
}

