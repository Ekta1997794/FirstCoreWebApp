using Microsoft.AspNetCore.Mvc;

namespace FirstCoreWebApp.Model
{
    public static class ApiResponse
    {
        public static IActionResult Success(object data, string message = "Success")
        {
            return new ObjectResult(new
            {
                message = message,
                data = data
            })
            {
                StatusCode = 200
            };
        }
        public static IActionResult Created(object data, string message = "Created Successfully")
        {
            return new ObjectResult(new
            {
                statusCode = 201,
                message = message,
                data = data
            })
            {
                StatusCode = 201
            };
        }

        public static IActionResult BadRequest(string message = "Bad Request")
        {
            return new ObjectResult(new
            {
                statusCode = 400,
                message = message
            })
            {
                StatusCode = 400
            };
        }

        public static IActionResult Unauthorized(string message = "Unauthorized")
        {
            return new ObjectResult(new
            {
                statusCode = 401,
                message = message
            })
            {
                StatusCode = 401
            };
        }

        public static IActionResult Conflict(string message = "Conflict")
        {
            return new ObjectResult(new
            {
                statusCode = 409,
                message = message
            })
            {
                StatusCode = 409
            };
        }
    }
}
