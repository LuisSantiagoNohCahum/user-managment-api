﻿namespace ApiUsers.Models.Requests.Uers
{
    public class InsertRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public int RolId { get; set; }
    }
}
