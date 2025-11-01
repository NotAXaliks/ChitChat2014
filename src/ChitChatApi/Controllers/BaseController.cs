using ChitChatApi.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ChitChatApi.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult SendError(string error)
        {
            var response = ApiResponse<object>.FromError(error);
            return Ok(response);
        }

        protected IActionResult SendData<T>(T data)
        {
            var response = ApiResponse<T>.FromData(data);
            return Ok(response);
        }

        protected string GenerateSession(int employeeId)
        {
            var sessionToken = Guid.NewGuid().ToString();

            HttpContext.Session.SetString("SessionToken", sessionToken);
            HttpContext.Session.SetInt32("UserId", employeeId);

            return sessionToken;
        }
    }
}