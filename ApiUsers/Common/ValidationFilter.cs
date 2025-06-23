using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiUsers.Common
{
    public class ValidationFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments)
            {
                var argumentType = argument.Value?.GetType();
                if (argumentType is null) continue;

                var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
                var validator = _serviceProvider.GetService(validatorType) as IValidator;

                if (validator is not null)
                {
                    var validationContext = new ValidationContext<object>(argument.Value);

                    var result = await validator.ValidateAsync(validationContext);
                    if (!result.IsValid)
                    {
                        context.Result = new BadRequestObjectResult(new ApiResponse<string>() 
                        { 
                            Success = false,
                            Data = "An error occurred while validating the request.",
                            Errors = result.Errors.Select(x => $"{x.PropertyName}: {x.ErrorMessage}" ).ToArray()
                        });

                        return;
                    }
                }
            }

            await next();
        }
    }
}

