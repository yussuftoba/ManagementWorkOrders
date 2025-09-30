using DTO;
using Identity;
using IServices;
using ManagementWorkOrdersAPI.DTO;
using ManagementWorkOrdersAPI.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace ManagementWorkOrdersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : APIBaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJWTService _jwtService;
        private readonly EmailSender _emailSender;

        public AccountController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, IJWTService jwtService, EmailSender emailSender) : base(unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _emailSender = emailSender;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(registerDTO.Email);

                if (user == null)
                {
                    var applicationUser = new ApplicationUser()
                    {
                        Email = registerDTO.Email,
                        PhoneNumber = registerDTO.PhoneNumber,
                        UserName = registerDTO.Email,
                        PersonName = registerDTO.PersonName.Replace(" admin", "")
                    };

                    var result = await _userManager.CreateAsync(applicationUser, registerDTO.Password);

                    if (result.Succeeded)
                    {
                        if (registerDTO.PersonName.ToLower().Contains("admin"))
                        {
                            if (await _roleManager.FindByNameAsync("admin") is null)
                            {
                                var role = new ApplicationRole()
                                {
                                    Name = "admin"
                                };

                                await _roleManager.CreateAsync(role);
                            }

                            await _userManager.AddToRoleAsync(applicationUser, "admin");
                        }
                        else
                        {
                            if (await _roleManager.FindByNameAsync("viewer") is null)
                            {
                                var role = new ApplicationRole()
                                {
                                    Name = "viewer"
                                };

                                await _roleManager.CreateAsync(role);
                            }

                            await _userManager.AddToRoleAsync(applicationUser, "viewer");
                        }

                        //sign in
                        await _signInManager.SignInAsync(applicationUser, isPersistent: false);

                        var token = await _jwtService.CreateJwtToken(applicationUser);

                        return Ok(token);
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("Register", error.Description);
                        }
                    }
                }
                return BadRequest("This email is already existing");
            }

            return BadRequest(ModelState);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginDTO.Email);

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, isPersistent: false, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {

                        if (user == null)
                        {
                            return NotFound();
                        }

                        //sign in

                        await _signInManager.SignInAsync(user, isPersistent: false);

                        var token = await _jwtService.CreateJwtToken(user);

                        return Ok(token);
                    }
                }
                return BadRequest("Invalid Email or Password");
            }

            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                if(user == null)
                {
                    return Unauthorized();
                }

                var result = await _userManager.ChangePasswordAsync(user, changePasswordDTO.CurrentPassword, changePasswordDTO.NewPassword);

                if (result.Succeeded)
                {
                    return Ok(new { Message = "Password changed successfully!" });
                }
                return BadRequest(result.Errors);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordDTO forgetPasswordDTO)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(forgetPasswordDTO.Email);

                if (user == null)
                {
                    return BadRequest("User Not Found");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Send email
                _emailSender.SendEmail("Password Reset", forgetPasswordDTO.Email,token);

                return Ok(new { message = "Password reset link has been sent to your email." });
            }
            return BadRequest(ModelState);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);

                if(user == null)
                {
                    return BadRequest("User Not Found");
                }

                var result = await _userManager.ResetPasswordAsync(user, resetPasswordDTO.Token, resetPasswordDTO.NewPassword);

                if (result.Succeeded)
                {
                    return Ok(new { message = "Password has been reset successfully!" });
                }
                return BadRequest(result.Errors);
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpGet("LogOut")]
        public async Task<IActionResult> GetLogOut()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }


    }
}
