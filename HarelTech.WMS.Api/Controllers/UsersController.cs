using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarelTech.WMS.Api.Models;
using HarelTech.WMS.Common.Models;
using HarelTech.WMS.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace HarelTech.WMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class UsersController : ControllerBase
    {
        private readonly IPrioritySystem _prioritySystem;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        public UsersController(IPrioritySystem prioritySystem, IUserService userService, IConfiguration configuration)
        {
            _prioritySystem = prioritySystem;
            _userService = userService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("apiAuth")]
        public async Task<IActionResult> ApiAuthenticate([FromBody]AuthenticateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Username or password is incorrect" });

            var user = await _userService.Authenticate(model.UserName, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }

        //[Authorize]
        [HttpPost("appAuth")]
        public async Task<IActionResult> AppAuthentication(string userLogin)
        {
            if (string.IsNullOrEmpty(userLogin))
                return BadRequest(new RequestResponseDto  { Success = false, Message = "Incorrect user login" });

            //get user
            var user = await _prioritySystem.GetSystemUser(userLogin);
            if(user == null)
                return BadRequest(new { message = "Incorrect user login" });
            //check license
            var lic = _configuration.GetValue<int>("AppSettings:Users");
            var activeUsers = await _prioritySystem.GetCurrentActiveUsers();
            if(activeUsers > lic)
                return Ok(new RequestResponseDto { Success = false, Error = "License Violation! Please contact system admin." });

            var isExists = await _prioritySystem.IsUserExists(user.USERID);

            if (isExists) return Ok(user);

            return Ok(new RequestResponseDto { Success = false, Error = "Your user not allowed to Quick WMS app. Please contact system admin." });
        }
    }
}