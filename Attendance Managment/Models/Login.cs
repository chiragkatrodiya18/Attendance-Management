using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Attendance_Managment.Models
{
    public class Login
    {
        [Key]
        [Required]
        public string Id { get; set; }
        public string Password { get; set; }
    }
}