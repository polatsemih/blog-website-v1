using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using sup1.webui.Identity;

namespace sup1.webui.Models.Admin
{
    public class RoleModel
    {
        [Required(ErrorMessage = "Please enter a role name.")]
        public string Name { get; set; }
    }

    public class RoleEditModel
    {
        public IdentityRole Role { get; set; }
        public IEnumerable<User> Members { get; set; }
        public IEnumerable<User> NonMembers { get; set; }
        public string RoleId { get; set; }
        
        [Required(ErrorMessage = "Please enter a role name.")]
        public string RoleName { get; set; }
        public string[] IdsToAdd { get; set; }
        public string[] IdsToDelete { get; set; }
    }
}