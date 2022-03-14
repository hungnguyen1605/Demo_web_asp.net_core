using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;

namespace webDemo.Areas.Admin.Models
{
    public class ChangePasswordviewmodel
    {
        [Key]
        public int AccountId { get; set; }
        [Display(Name = "Mật khẩu hiện tại")]
        public string PasswordNow { get; set; }
        [Display(Name = "Mật khẩu mới")]
        [Required(ErrorMessage = "Vui lòng nhập Mật khẩu mới")]
        [MinLength(5, ErrorMessage = "Mật khẩu phải nhập trên 5 ký tự")]
        public string Password { get; set; }
        [MinLength(5, ErrorMessage = "Mật khẩu phải nhập trên 5 ký tự")]
        [Display(Name = "vui lòng nhập lại Mật khẩu mới")]
        [Compare("Password",ErrorMessage ="Mật khẩu không giống nhau")]
        public string ConfirmPassword { get; set; }
    }


}
