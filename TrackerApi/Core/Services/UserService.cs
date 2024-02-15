using AutoMapper;
using TrackerApi.Common.Constants;
using TrackerApi.Common.Helper;
using TrackerApi.Common.Model.Dtos;
using TrackerApi.Common.Model.RequestModels.User;
using TrackerApi.Common.Model.ViewModels.User;
using TrackerApi.Core.Interfaces.Repositories;
using TrackerApi.Core.Interfaces.Services;
using TrackerApi.Data.Entity;

namespace TrackerApi.Core.Services
{
    public class UserService : IUserServices
    {

        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;


        public UserService(IUsersRepository usersRepository, IMapper mapper)
        {
            _usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
            _mapper = mapper;
        }

        #region CRUD

        public async Task<ServiceResultDto<CreateUserViewModel>> CreateUser(CreateUserRequestModel requestModel)
        {

            ServiceResultDto<CreateUserViewModel> result = new();

            string encryptKey = HelperMethods.CreateEncryptKey(8);

            if (encryptKey is null)
                return result;

            string encryptPassword = HelperMethods.CreateEncryptHash(requestModel.Password, encryptKey);


            var createUser = new Users
            {
                Id = Guid.NewGuid(),
                Password = encryptPassword,
                Salt = encryptKey,
                Email = requestModel.Email ?? string.Empty,
                CreateDate = DateTime.Now,
            };

            var createdUser = await _usersRepository.InsertGetEntityAsync(createUser);

            if (createdUser is not null)
            {
                result.Status = true;
                result.Message = CommonConstants.Success;
                result.Data = _mapper.Map<CreateUserViewModel>(createdUser);
            }
            else
            {
                result.Status = false;
                result.Message = CommonConstants.Failed;
            }

            return result;
        }


        #endregion
    }
}
