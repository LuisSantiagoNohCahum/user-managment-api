using ApiUsers.Models.Constants;
using ApiUsers.Models.Enums;
using MiniExcelLibs;

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

        public async Task<bool> InsertGuestAsync(SignUpRequest request, CancellationToken cancellationToken)
        {
            bool existUser = await _userRepository.AnyAsync(u => !string.IsNullOrEmpty(u.Email) && u.Email.Equals(request.Email), cancellationToken);

            if (existUser)
            {
                throw new ApiException($"Ya existe un usuario registrado con el correo {request.Email}");
            } 

            var guestRol = await _rolRepository.GetAsync(r => r.Code.Equals(RolCode.Guest), cancellationToken);

            string passwordHash = _passwordHashHelper.HashPassword(request.Password);

            var newGuestUser = new User()
            {
                Email = request.Email,
                Password = passwordHash,
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsActive = true,
                RolId = guestRol?.Id ?? 0
            };

            await _userRepository.InsertAsync(newGuestUser, cancellationToken);

            return true;
        }

        public async Task<UserDto> GetAsync(int id, CancellationToken cancellationToken)
        { 
            var user = await _userRepository.GetAsync(id, cancellationToken);

            if (user is null)
            {
                throw new ApiException($"Usuario {id} no encontrado.");
            }

            var rol = await _rolRepository.GetAsync(user.RolId, cancellationToken);

            return user.ToDto(rol?.ToDto());
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

        private (string, List<object>) GetFilterFromGetAllRequest(GetAllRequest request)
        {
            int index = 0;
            var filter = new StringBuilder();
            var parameters = new List<object>();

            if (request.RolId > 0)
            {
                BuildFilter("RolId == @{1}", request.RolId);
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                BuildFilter("Email.Contains(@{1})", request.Email);
            }

            if (request.CreatedOn > DateTime.MinValue)
            {
                BuildFilter("CreatedOn >= @{1}", request.CreatedOn);
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                bool isActive = request.Status.ToLower() switch
                {
                    "active" => true,
                    "inactive" => false,
                    _ => throw new ApiException($"El status {request.Status} no esta permitido.")
                };

                BuildFilter("IsActive == @{1}", isActive);
            } 

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

            if (user is null) 
            {
                throw new ApiException($"Usuario {id} no encontrado.");
            };

            user.Id = user.Id;
            user.Email = request.Email;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.IsActive = request.IsActive;
            user.RolId = request.RolId;

            return await _userRepository.UpdateAsync(user, cancellationToken);
        }

        public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(id, cancellationToken);

            if (user is null) return 0;
            
            bool isInternal = await _rolRepository.AnyAsync(r => r.Id == user.RolId && r.Code.Equals(RolCode.Internal), cancellationToken);

            if (isInternal)
            {
                throw new ApiException($"The user {user.Email} is internal and cannot be deleted.");
            }  

            return await _userRepository.DeleteAsync(user, cancellationToken);
        }

        public async Task<int> ImportFromFileAsync(ImportFromFileRequest request, CancellationToken cancellationToken)
        {
            // TODO. Move the read process to repository.
            string filePath = string.Empty;

            try
            {
                // TODO. First validate request and the save the file and to aggroup cases use or (|) clausule.
                (filePath, string extension) = await SaveLayoutFileAsync(request, cancellationToken);

                var users = request.Type switch
                {
                    FileType.TXT when IsValidExtension("txt") => await ReadUsersFromTxtAsync(filePath, cancellationToken),
                    FileType.EXCEL when IsValidExtension("xlsx", "xls") => await ReadUsersFromExcelOrCsvAsync(filePath, ExcelType.XLSX, cancellationToken),
                    FileType.CSV when IsValidExtension("csv") => await ReadUsersFromExcelOrCsvAsync(filePath, ExcelType.CSV, cancellationToken),
                    _ => throw new ApiException("Formato de archivo no soportado para el tipo de importacion.")
                };

                users = users.Select(u =>
                {
                    u.Password = _passwordHashHelper.HashPassword(u.Password);
                    return u;
                });

                bool IsValidExtension(params string[] extensions)
                    => extensions is not null && extensions.Any() && extensions.Contains(extension);

                // TODO. Auto generate password or allow set hashed password.
                return await _userRepository.BulkInsert(users, cancellationToken);
            }
            finally
            {
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        public async Task<(string FilePath, string Extension)> SaveLayoutFileAsync(ImportFromFileRequest request, CancellationToken cancellationToken)
        {
            var fileParts = request.LayoutFile.FileName.Split(".", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            if (fileParts.Length <= 1)
                throw new ApiException("El archivo cargado no es valido");

            var extension = fileParts.LastOrDefault();

            using (var data = new MemoryStream())
            {
                await request.LayoutFile.CopyToAsync(data, cancellationToken);

                string filePath = await _webRootFilesHelper.SaveFileAsync(data, $"{Guid.NewGuid().ToString()}.{extension}", cancellationToken);

                filePath.Guard(nameof(filePath));

                return (filePath, extension);
            }
        }

        public async Task<IEnumerable<User>> ReadUsersFromTxtAsync(string filePath, CancellationToken cancellationToken)
        {
            if (!File.Exists(filePath)) return Enumerable.Empty<User>();

            var lines = await File.ReadAllLinesAsync(filePath, cancellationToken);

            var headers = new[] { "Correo", "Contraseña", "Nombres", "Apellidos", "Activo", "Rol Id"};

            return lines.Select((line, index) =>
            {
                var fields = line.Split("|", StringSplitOptions.TrimEntries);

                for (int i = 0; i < fields.Length; i++)
                {
                    if (string.IsNullOrEmpty(fields[i]))
                    {
                        string message = $"Formato incorrecto en la fila {index + 1} columna {i + 1} ({headers[i]})";
                        throw new ApiException(message);
                    }
                }

                // TODO. Validate with the insert validator, set the data into insert request and validate (use ServiceProvider or the concrete class), and mapper the all request to user
                return new User()
                {
                    Email = fields[0],
                    Password = fields[1],
                    FirstName = fields[2],
                    LastName = fields[3],
                    IsActive = Convert.ToBoolean(Convert.ToInt32(fields[4])),
                    RolId = Convert.ToInt32(fields[5])
                };
            });
        }

        public async Task<IEnumerable<User>> ReadUsersFromExcelOrCsvAsync(string filePath, ExcelType excelType, CancellationToken cancellationToken)
        {
            MiniExcelLibs.IConfiguration configuration = default;

            Dictionary<string, string> columns = new()
            {
                { nameof(User.Email), "CORREO" },
                { nameof(User.Password), "CONTRASENA" },
                { nameof(User.FirstName), "NOMBRES" },
                { nameof(User.LastName), "APELLIDOS" },
                { nameof(User.IsActive), "ACTIVO" },
                { nameof(User.RolId), "ROLID" },
            };

            if (excelType == ExcelType.CSV)
            {
                configuration = new MiniExcelLibs.Csv.CsvConfiguration()
                {
                    Seperator = ',',
                    StreamReaderFunc = (stream) => new StreamReader(stream, Encoding.GetEncoding("ISO-8859-1"))
                };
            }

            return await _excelHelper.ReadWorkSheetAsync<User>(filePath, excelType, columns, configuration, "UsersLayout", cancellationToken);
        }
        
        public async Task<string> ExportToExcelAsync([FromQuery] GetAllRequest request, CancellationToken cancellationToken)
        {
            string filePath = string.Empty;

            try
            {
                var users = await GetAllAsync(request, cancellationToken);

                var linealUsersInfo = users.Select(user => new
                {
                    Clave = user.Id,
                    Correo = user.Email,
                    Nombres = user.FirstName,
                    Apellidos = user.LastName,
                    Estatus = user.IsActive ? "ACTIVO" : "INACTIVO",
                    Fecha_Creacion = user.CreatedOn,
                    Clave_Rol = user.Rol?.Code,
                    Nombre_Rol = user.Rol?.Name
                });

                filePath = await _excelHelper.CreateWorkBookAsync(linealUsersInfo, cancellationToken: cancellationToken);

                var fileData = await File.ReadAllBytesAsync(filePath, cancellationToken);

                return await _webRootFilesHelper.GetFileUrlAsync(fileData, $"{Guid.NewGuid().ToString()}.xlsx", cancellationToken);
            }
            finally
            {
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
           
        }
    }
}
