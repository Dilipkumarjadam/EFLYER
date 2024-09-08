using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFLYER.DTOs
{
    public class RegisteredUserDTO
    {
        public int RegId { get; set; }

        [Required(ErrorMessage = "Please Enter Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Please Enter Mobile No.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Please Enter Valid 10-Digit Mobile No.")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Please Select Country")]
        public int CountryRId { get; set; }

        [Required(ErrorMessage = "Please Enter EmailId")]
        [RegularExpression(@"^[^@\s]+@gmail[^@\s]+\.[^@\s]+$", ErrorMessage = "Please Enter Valid EmailId")]
        public string Email { get; set; }
        public string AdminEmail { get; set; }
        [Required(ErrorMessage = "Please Enter Password")]
        public string AdminPassword { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]
        public string Password { get; set; }
        public string ImagePath { get; set; }
        public IFormFile IMAGE { get; set; }
        public string Type {  get; set; }

      
    }
}
