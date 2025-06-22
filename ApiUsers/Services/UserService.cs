namespace ApiUsers.Services
{
    public sealed class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherHelper _passwordHashHelper;
        private readonly IRolRepository _rolRepository;
        private readonly IWebRootFilesHelper _webRootFilesHelper;
        private readonly IExcelHelper _excelHelper;

        public UserService(IUserRepository userRepository, 
            IPasswordHasherHelper passwordHashHelper, 
            IRolRepository rolRepository, 
            IWebRootFilesHelper webRootFilesHelper, 
            IExcelHelper excelHelper)
        {
            _userRepository = userRepository;
            _passwordHashHelper = passwordHashHelper;
            _rolRepository = rolRepository;
            _webRootFilesHelper = webRootFilesHelper;
            _excelHelper = excelHelper;
        }

        public async Task<bool> SignUpAsync(SignUpRequest request, CancellationToken cancellationToken)
        {
            bool existUser = await _userRepository.AnyAsync(u => !string.IsNullOrEmpty(u.Email) && u.Email.Equals(request.Email), cancellationToken);

            if (existUser) throw new Exception("The given email is being used.");

            var guestRol = await _rolRepository.GetAsync(r => !string.IsNullOrEmpty(r.Code) && r.Code.Equals("GST"), cancellationToken);

            await _userRepository.InsertAsync(new User()
            {
                Email = request.Email,
                Password = _passwordHashHelper.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsActive = true,
                RolId = guestRol?.Id ?? 0
            }, cancellationToken);

            return true;
        }

        public async Task<UserDto?> GetAsync(int id, CancellationToken cancellationToken)
        { 
            var user = await _userRepository.GetAsync(id, cancellationToken);
            if (user is null) return default;

            var rol = await _rolRepository.GetAsync(user.RolId, cancellationToken);

            var response = user.ToDto();
            response.Rol = rol is null ? default : rol.ToDto();
            return response;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync(GetAllRequest request, CancellationToken cancellationToken)
        {
            
            var (filter, parameters) = GetFilterFromGetAllRequest(request);

            var users = parameters.Any()
                ? await _userRepository.GetAllAsync(filter.ToString(), parameters.ToArray(), cancellationToken)
                : await _userRepository.GetAllAsync(cancellationToken);

            var roles = await _rolRepository.GetAllAsync(cancellationToken);

            return users.Join(roles, 
                user => user.RolId, 
                rol => rol.Id, 
                (user, rol) => new UserDto()
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        IsActive = user.IsActive,
                        CreatedOn = user.CreatedOn,
                        UpdatedOn = user.UpdatedOn,
                        Rol = new RolDto()
                        {
                            Id = rol.Id,
                            Code = rol.Code,
                            Name = rol.Name
                        }
                    });
        }

        private (string, List<object?>) GetFilterFromGetAllRequest(GetAllRequest request)
        {
            int index = 0;
            var filter = new StringBuilder();
            var parameters = new List<object?>();

            if (request.RolId > 0)
                BuildFilter("RolId == @{1}", request.RolId);

            if (!string.IsNullOrEmpty(request.Email))
                BuildFilter("Email.Contains(@{1})", request.Email);

            if (request.CreatedOn > DateTime.MinValue)
                BuildFilter("CreatedOn >= @{1}", request.CreatedOn);

            if (string.IsNullOrEmpty(request.Status) && !string.IsNullOrEmpty(request.Status))
                BuildFilter("IsActive == @{1}", request.Status.Equals("active", StringComparison.OrdinalIgnoreCase));

            void BuildFilter(string single_filter, object value)
            {
                filter.AppendFormat("{0}" + single_filter, index > 0 ? " and " : "", index++);
                parameters.Add(value);
            }

            return (filter.ToString(), parameters);
        }

        public async Task<int> InsertAsync(InsertRequest request, CancellationToken cancellationToken)
        {
            var user = new User()
            {
                Email = request.Email,
                Password = _passwordHashHelper.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsActive = request.IsActive,
                RolId = request.RolId
            };

            return await _userRepository.InsertAsync(user, cancellationToken);
        }

        public async Task<int> UpdateAsync(int id, UpdateRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(id, cancellationToken);

            if (user is null) return 0;

            var setUser = new User()
            {
                Id = user.Id,
                Email = request.Email,
                Password = user.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsActive = request.IsActive,
                RolId = request.RolId,
                CreatedBy = user.CreatedBy,
                CreatedOn = user.CreatedOn,
                UpdatedBy = user.UpdatedBy,
                UpdatedOn = user.UpdatedOn
            };

            return await _userRepository.UpdateAsync(setUser, cancellationToken);
        }

        public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(id, cancellationToken);

            if (user is not null)
                return await _userRepository.DeleteAsync(user, cancellationToken);

            // Validate if user is default not allowed delete action

            return 0;
        }

        public async Task<int> ImportFromExcelAsync(IFormFile layoutRequest, CancellationToken cancellationToken)
        {
            string fullFilePath = string.Empty;
            using (var data = new MemoryStream())
            {
                await layoutRequest.CopyToAsync(data, cancellationToken);
                fullFilePath = await _webRootFilesHelper.SaveFileAsync(data, layoutRequest.FileName, cancellationToken);
                fullFilePath.Guard(nameof(fullFilePath));
            }

            var users = await _excelHelper.ReadWorkSheetAsync<User>(fullFilePath, isExpendedModelType: true, cancellationToken: cancellationToken);

            if (File.Exists(fullFilePath)) File.Delete(fullFilePath);

            var bulkResult = await _userRepository.BulkInsert(users, cancellationToken);

            return bulkResult;
        }

        public async Task<FileStreamResult> ExportToExcelAsync(CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync(cancellationToken);

            var fileData = await _excelHelper.CreateWorkBookAsync(users, cancellationToken: cancellationToken);

            return new FileStreamResult(new MemoryStream(fileData), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "Users.xlsx"
            };
        }
    }
}
