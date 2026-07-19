// File: EnrollmentSystem.UI/ViewModels/LoginViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace EnrollmentSystem.UI.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Username or email is required.")]
    [Display(Name = "Username or Email")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}