using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.RegularExpressions;

namespace ApiUsers.Classes
{
    public class CustomValidator
    {
        public static bool ValidatePassword(string _password)
        {
            //Realizar if individual para poder mostrar que caracter falta
            try
            {
                if (string.IsNullOrEmpty(_password.Trim())) return false;
                if (_password.Length != 10) return false;
                if (Regex.Count(_password, @"[a-z]") != 3) return false;
                if (Regex.Count(_password, @"[A-Z]") != 3) return false;
                if (Regex.Count(_password, @"[0-9]") != 2) return false;
                if (Regex.Count(_password, @"[^A-Za-z0-9]") != 2) return false;

                return true;
            }
            catch { return false;  }
        }

        public static bool ValidateEmail(string _email) 
        {
            if (string.IsNullOrWhiteSpace(_email)) return false;
            String expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(_email, expresion)) return (Regex.Replace(_email, expresion, String.Empty).Length == 0);
            else return false;
        }
    }
}
