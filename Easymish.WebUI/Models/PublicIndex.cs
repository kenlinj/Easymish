using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easymish.WebUI.Models
{
    public class PublicIndex
    {
        [Display(ResourceType = typeof(Web.Resources.Account.Login), Name = "Username")]
        [Required(ErrorMessageResourceType = typeof(Web.Resources.Global.Message), ErrorMessageResourceName = "Require")]
        public string Username { get; set; }

        [Display(ResourceType = typeof(Web.Resources.Account.Login), Name = "Password")]
        [Required(ErrorMessageResourceType = typeof(Web.Resources.Global.Message), ErrorMessageResourceName = "Require")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(Web.Resources.Account.Login), Name = "RememberMe")]
        public bool RememberMe { get; set; }
    }
}
