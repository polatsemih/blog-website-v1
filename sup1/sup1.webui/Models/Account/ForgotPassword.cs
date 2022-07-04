using System.ComponentModel.DataAnnotations;

namespace sup1.webui.Models.Account
{
    public class ForgotPassword
    {
        [Required(ErrorMessage = "Please enter a valid email address.")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Please enter a valid email address.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}