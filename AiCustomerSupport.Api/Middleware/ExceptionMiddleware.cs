using AiCustomerSupport.Api.Models;
using AiCustomerSupport.Application.Exceptions;
using System.Net;

namespace AiCustomerSupport.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            CustomProblemDetails problemDetails = new();

            switch (ex)
            {
                case BadRequestException badRequestException:

                    statusCode = HttpStatusCode.BadRequest;
                    problemDetails = new CustomProblemDetails
                    {
                        Title = badRequestException.Message,
                        Status = (int)statusCode,
                        Detail = badRequestException.InnerException?.Message,
                        Type = nameof(BadRequestException),
                        Errors = badRequestException.ValidationErrors
                    };

                    break;


                case NotFoundException notFoundException:

                    statusCode = HttpStatusCode.NotFound;
                    problemDetails = new CustomProblemDetails
                    {
                        Title = notFoundException.Message,
                        Status = (int)statusCode,
                        Detail = notFoundException.InnerException?.Message,
                        Type = nameof(NotFoundException)
                    };

                    break;



                case ForbiddenException forbiddenException:

                    statusCode = HttpStatusCode.Forbidden;
                    problemDetails = new CustomProblemDetails
                    {
                        Title = forbiddenException.Message,
                        Status = (int)statusCode,
                        Detail = forbiddenException.InnerException?.Message,
                        Type = nameof(ForbiddenException)
                    };

                    break;


                default:
                    problemDetails = new CustomProblemDetails
                    {
                        Title = ex.Message,
                        Status = (int)statusCode,
                        Detail = ex.StackTrace,
                        Type = nameof(HttpStatusCode.InternalServerError)
                    };

                    break;
            }

            httpContext.Response.StatusCode = (int)statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
