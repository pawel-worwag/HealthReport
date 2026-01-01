using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using HealthReport.Identity.Application;
using HealthReport.Identity.Contracts;

namespace HealthReport.Web.Components.Pages.Identity;

public partial class Register(NavigationManager navigation, IRegisterUserHandler registerHandler)
	: ComponentBase
{


	protected RegisterModel model = new();
	protected bool isSubmitting;
	protected string[]? errors;

	protected async Task HandleValidSubmit()
	{
		isSubmitting = true;
		errors = null;
		var dto = new RegisterUserDto(model.Email ?? string.Empty, model.Password ?? string.Empty);
		var result = await registerHandler.RegisterAsync(dto);
		isSubmitting = false;

		if (result.Succeeded)
		{
			navigation.NavigateTo("/identity/login");
		}
		else
		{
			errors = result.Errors;
		}
	}

	protected class RegisterModel
	{
		[Required]
		[EmailAddress]
		public string? Email { get; set; }

		[Required]
		[MinLength(6)]
		public string? Password { get; set; }

		[Required]
		[Compare("Password", ErrorMessage = "Passwords do not match")]
		public string? ConfirmPassword { get; set; }
	}
}