using System.Text.RegularExpressions;

namespace UserProvider.Helpers;

public class Validation
{
    public static bool ValidateEmail(string email) => Regex.IsMatch("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$", email);
}
