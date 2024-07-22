namespace ApiUsers.Models.Dto.Request
{
    public class UserDto
    {
        public string FullName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        //verificar que exista el rol antes de insertar
        public int RolType { get; set; }

    }
}
