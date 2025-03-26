namespace ApiUsers.Extensions
{
    public static class ModelMapperExtensions
    {
        public static UserDto ToDto(this User user)
        =>  new UserDto()
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                CreatedOn = user.CreatedOn,
                UpdatedOn = user.UpdatedOn,
                Rol = default
            };
        
        public static RolDto ToDto(this Rol rol)
            => new RolDto()
            {
                Id = rol.Id,
                Code = rol.Code,
                Name = rol.Name,
            };
    }
}
