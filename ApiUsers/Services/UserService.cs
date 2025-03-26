namespace ApiUsers.Services
{
    //TODO. Change all ct to cancelationToken param name
    //TODO. Update password endpoint only enable for current user logged and only change password with security code and the last correct password validate
    //TODO. Map to GetAllResponse, dont send encoded/decoded password, manage password in other endpoints
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

        public async Task<bool> SignUpAsync(SignUpRequest request, CancellationToken ct)
        {
            bool existUser = await _userRepository.AnyAsync(u => !string.IsNullOrEmpty(u.Email) && u.Email.Equals(request.Email), ct);

            if (existUser) throw new Exception("The given email is being used.");

            var guestRol = await _rolRepository.GetAsync(r => !string.IsNullOrEmpty(r.Code) && r.Code.Equals("GST"), ct);

            await _userRepository.InsertAsync(new User()
            {
                Email = request.Email,
                Password = _passwordHashHelper.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsActive = true,
                RolId = guestRol?.Id ?? 0
            }, ct);

            return true;
        }

        public async Task<UserDto?> GetAsync(int id, CancellationToken ct)
        { 
            var user = await _userRepository.GetAsync(id, ct);
            if (user is null) return default;

            var rol = await _rolRepository.GetAsync(user.RolId, ct);

            var response = user.ToDto();
            response.Rol = rol is null ? default : rol.ToDto();
            return response;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync(GetAllRequest request, CancellationToken ct)
        {
            
            var (filter, parameters) = GetFilterFromGetAllRequest(request);

            var users = parameters.Any()
                ? await _userRepository.GetAllAsync(filter.ToString(), parameters.ToArray(), ct)
                : await _userRepository.GetAllAsync(ct);

            var roles = await _rolRepository.GetAllAsync(ct);

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

            if (string.IsNullOrEmpty(request.Status) && !request.Status.Equals("All", StringComparison.OrdinalIgnoreCase))
                BuildFilter("IsActive == @{1}", request.Status.Equals("Active", StringComparison.OrdinalIgnoreCase));

            void BuildFilter(string single_filter, object value)
            {
                filter.AppendFormat("{0}" + single_filter, index > 0 ? " and " : "", index++);
                parameters.Add(value);
            }

            return (filter.ToString(), parameters);
        }

        public async Task<int> InsertAsync(InsertRequest request, CancellationToken ct)
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

            return await _userRepository.InsertAsync(user, ct);
        }

        public async Task<int> UpdateAsync(UpdateRequest request, CancellationToken ct)
        {
            var user = await _userRepository.GetAsync(request.Id, ct);

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

            return await _userRepository.UpdateAsync(setUser, ct);
        }

        public async Task<int> DeleteAsync(int id, CancellationToken ct)
        {
            var user = await _userRepository.GetAsync(id, ct);

            if (user is not null)
                return await _userRepository.DeleteAsync(user, ct);

            return 0;
        }

        public async Task<int> ImportFromExcelAsync(IFormFile layoutRequest, CancellationToken ct)
        {
            string fullFilePath = string.Empty;
            using (var data = new MemoryStream())
            {
                await layoutRequest.CopyToAsync(data, ct);
                fullFilePath = await _webRootFilesHelper.SaveFileAsync(data, layoutRequest.FileName, ct);
                fullFilePath.Guard(nameof(fullFilePath));
            }

            var users = await _excelHelper.ReadWorkSheetAsync<User>(fullFilePath, isExpendedModelType: true, ct: ct);

            if (File.Exists(fullFilePath)) File.Delete(fullFilePath);

            var bulkResult = await _userRepository.BulkInsert(users, ct);

            return bulkResult;
        }

        public async Task<FileStreamResult> ExportToExcelAsync(CancellationToken ct)
        {
            var users = await _userRepository.GetAllAsync(ct);

            var fileData = await _excelHelper.CreateWorkBookAsync(users, ct: ct);

            return new FileStreamResult(new MemoryStream(fileData), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "Users.xlsx"
            };
        }
    }
}
