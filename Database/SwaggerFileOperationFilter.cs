using finance_management.DTOs;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class SwaggerFileOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasFile = context.MethodInfo
            .GetParameters()
            .Any(p => p.ParameterType == typeof(IFormFile) ||
                      p.ParameterType == typeof(ImportTransactionsRequest) &&
                      p.ParameterType.GetProperties().Any(pi => pi.PropertyType == typeof(IFormFile)));

        if (!hasFile)
            return;

        operation.RequestBody = new OpenApiRequestBody
        {
            Content =
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties =
                        {
                            ["file"] = new OpenApiSchema { Type = "string", Format = "binary" }
                        },
                        Required = new HashSet<string> { "file" }
                    }
                }
            }
        };
    }
}