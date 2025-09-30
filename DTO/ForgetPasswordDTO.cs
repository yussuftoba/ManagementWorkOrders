using System.ComponentModel.DataAnnotations;

namespace DTO;

public class ForgetPasswordDTO
{
    [Required(ErrorMessage = "Email can't be blank")]
    [EmailAddress(ErrorMessage = "Email should be in a proper email address format")]
    public string Email {  get; set; }
}
