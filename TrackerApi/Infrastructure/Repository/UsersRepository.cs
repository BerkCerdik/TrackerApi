using TrackerApi.Core.Interfaces.Context;
using TrackerApi.Core.Interfaces.Repositories;
using TrackerApi.Data.Entity;

namespace TrackerApi.Infrastructure.Repository
{
    public class UsersRepository : RepositoryDb<Users>, IUsersRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsersRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}