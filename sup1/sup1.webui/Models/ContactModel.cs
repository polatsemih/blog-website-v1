using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using sup1.entity;

namespace sup1.webui.Models.Admin
{
    public class ContactModel
    {
        [Required(ErrorMessage = "Please enter your message's subject.")]
        [MaxLength(50,ErrorMessage ="Max lenght is 50 word")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Please enter your message.")]
        public string Message { get; set; }
    }
    
    public class ContactAdmin
    {
        public List<ContactMessage> ContactMessages { get; set; }
    }

    public class ContactDetailModel
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}