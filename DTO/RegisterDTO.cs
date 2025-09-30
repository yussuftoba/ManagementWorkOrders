﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ManagementWorkOrdersAPI.DTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Person Name can't be blank")]
        public string PersonName { get; set; } = string.Empty;


        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = "Email should be in a proper email address format")]
        public string Email { get; set; } = string.Empty;


        [Required(ErrorMessage = "Phone number can't be blank")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number should contain digits only")]
        public string PhoneNumber { get; set; } = string.Empty;


        [Required(ErrorMessage = "Password can't be blank")]
        public string Password { get; set; } = string.Empty;


        [Required(ErrorMessage = "Confirm Password can't be blank")]
        [Compare("Password", ErrorMessage = "Password and confirm password do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
