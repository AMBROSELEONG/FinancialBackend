using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using FinancialApi.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinancialApi.Models;

namespace FinancialApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public UserController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost("getUser")]
        public async Task<IActionResult> GetUser([FromBody] GetUserRequest request)
        {
            if (string.IsNullOrEmpty(request.UserName) && string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.Password) && string.IsNullOrEmpty(request.Token))
            {
                return BadRequest(new { success = false });
            }

            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                Password = request.Password,
                Token = request.Token
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        [HttpPost("CheckUserName")]
        public async Task<IActionResult> CheckUserName([FromBody] CheckUserNameRequest request)
        {
            if (string.IsNullOrEmpty(request.UserName))
            {
                return BadRequest("UserName is required");
            }

            var checkUserName = await _context.Users
            .Where(v => v.UserName == request.UserName)
            .FirstOrDefaultAsync();

            if (checkUserName != null)
            {
                return BadRequest(new { success = false });
            }

            return Ok(new { success = true });
        }

        [HttpPost("CheckEmail")]
        public async Task<IActionResult> CheckEmail([FromBody] CheckEmailRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest("Email is required");
            }

            var checkEmail = await _context.Users
           .Where(v => v.Email == request.Email)
           .FirstOrDefaultAsync();

            if (checkEmail != null)
            {
                return BadRequest(new { success = false });
            }

            return Ok(new { success = true });
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Data is required");
            }

            var checkUserData = await _context.Users
            .Where(v => v.Email == request.Email && v.Password == request.Password)
            .FirstOrDefaultAsync();

            if (checkUserData == null)
            {
                return BadRequest(new { success = false });
            }

            var malaysiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
            checkUserData.LastLogin = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, malaysiaTimeZone);
            _context.Users.Update(checkUserData);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, userID = checkUserData.UserID, userName = checkUserData.UserName });
        }

        [HttpPost("Reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest("Email is required.");
            }

            if (string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Password is required.");
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(v => v.Email == request.Email);

            if (existingUser == null)
            {
                return NotFound("Email not found.");
            }

            existingUser.Password = request.Password;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while resetting the password.");
            }
        }
    }

    public class GetUserRequest
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Token { get; set; }
    }

    public class CheckUserNameRequest
    {
        public required string UserName { get; set; }
    }

    public class CheckEmailRequest
    {
        public required string Email { get; set; }
    }

    public class SignInRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class ResetRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}