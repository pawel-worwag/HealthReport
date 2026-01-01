using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using HealthReport.Identity.Contracts;
using HealthReport.Identity.Application;
using HealthReport.Identity.Domain;
using Microsoft.AspNetCore.Identity;

namespace HealthReport.Web.Components.Pages.Auth;

public partial class Login(NavigationManager navigation, ILoginUserHandler loginHandler, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) : ComponentBase
{
    protected LoginModel model = new();
    protected bool isSubmitting;
    protected string[]? errors;

    protected async Task HandleValidSubmit()
    {
        isSubmitting = true;
        errors = null;
        var dto = new LoginUserDto(model.Email ?? string.Empty, model.Password ?? string.Empty);
        var result = await loginHandler.LoginAsync(dto);
        isSubmitting = false;

        if (result.Succeeded)
        {
            // sign in the user on the web layer
            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user != null)
            {
                await signInManager.SignInAsync(user, isPersistent: false);
            }

            navigation.NavigateTo("/");
        }
        else
        {
            errors = result.Errors;
        }
    }

    protected class LoginModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
