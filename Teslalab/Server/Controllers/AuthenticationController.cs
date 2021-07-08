using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teslalab.Server.Services;
using Teslalab.Shared;

namespace Teslalab.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _usersService;

        public AuthenticationController(IUserService usersSerivce)
        {
            _usersService = usersSerivce;
        }

        [ProducesResponseType(200, Type = typeof(LoginResponse))]
        [ProducesResponseType(400, Type = typeof(LoginResponse))]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            var result = await _usersService.GenerateTokenAsync(model);
            if (result == null)
                return BadRequest("Invalid Username or Password");

            return Ok(result);
        }

        [ProducesResponseType(200, Type = typeof(OperationResponse<string>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<string>))]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            var result = await _usersService.RegisterUserAsync(model);
            if (result == null)
                return BadRequest("Invalid Username or Password");

            return Ok(result);
        }
    }
}