using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BookingSalon.Data.Validation
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            return value.GetType()
                .GetMember(value.ToString())[0]
                .GetCustomAttribute<DisplayAttribute>()?
                .Name ?? value.ToString();
        }
    }
}
