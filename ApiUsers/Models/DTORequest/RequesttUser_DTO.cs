namespace ApiUsers.Models.DTORequest
{
    public class RequestUser_DTO
    {
        public string FullName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        //verificar que exista el rol antes de insertar
        //public int RolType { get; set; } = 0;

    }
}
