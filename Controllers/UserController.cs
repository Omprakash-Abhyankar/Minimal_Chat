//using AngularAuthAPI.Helpers;
using Minimal_Chat_App.Helpers;
using Minimal_Chat_App.Models;
using Minimal_Chat_App.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System;
using Microsoft.AspNetCore.Authorization;


namespace AngularAuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _authContext;
        public UserController(AppDbContext appDbContext)
        {
            _authContext = appDbContext;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] Login userObj)
        {
            if (userObj == null)
                return BadRequest();
            // && x.Password == userObj.Password
            var user = await _authContext.Users.FirstOrDefaultAsync(x=>x.UserName == userObj.UserName);
         
            if (user == null)
                return NotFound(new {Message = "User Not Found!" });
            //these is check the encrypted password.
            if(!PasswordHasher.VerifyPassword(userObj.Password, user.Password))
            {
                return BadRequest(new { Message = "Password is Incorrect" });
            }

            user.Token = CreateJwt(user);

            return Ok(new
            { 
                Token = user.Token,
                Message = "Login Success!"
            });
        }


        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] Users userObj)
        {
            if (userObj == null)
                return BadRequest();
            //Check username
            if(await ChecckUserNameExistAsync(userObj.UserName))
                return BadRequest(new {Message = "Username is Already Exist!."});


            //Check Email
            if (await ChecckEmailExistAsync(userObj.Email))
                return BadRequest(new { Message = "Email Id is Already Exist!." });


            //Check Password Strenghth
            var pass = CheckPasswordStrength(userObj.Password);
            if(!string.IsNullOrEmpty(pass))
                return BadRequest(new { Message = pass.ToString() });


            userObj.Password = PasswordHasher.HashPassword(userObj.Password);     //these will help to hash the password.
            //userObj.Role = "User";
            userObj.Token = "";
            await _authContext.Users.AddAsync(userObj); //these line will help to send data in database
            await _authContext.SaveChangesAsync();
            return Ok(new
            {
                Message = "User Registered!"
            });
        }

        private Task<bool> ChecckUserNameExistAsync(string userName)        
            => _authContext.Users.AnyAsync(x => x.UserName == userName);

        private Task<bool> ChecckEmailExistAsync(string email)
           => _authContext.Users.AnyAsync(x => x.Email == email);

        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();
            if (password.Length < 8)

                sb.Append("Minimum password length should be 8" + Environment.NewLine);
            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]")
                && Regex.IsMatch(password, "[0-9]")))
                sb.Append("Password Should be Alphanumeric" + Environment.NewLine);
            if (!Regex.IsMatch(password, "[<,>,0,!,#,$,%,^,&,*,(,),_,+,\\[,\\],{,},?,:,;,|,',\\,.,/,~,`,-,=,@]"))
                sb.Append("Password Should Contain Special Charater" + Environment.NewLine);
            return sb.ToString();
        }
            //these token is made up of three things 1. header 2. payload 3. signiture
            private string CreateJwt(Users user)
            {
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("veryverysecret.....");
                var identity = new ClaimsIdentity(new Claim[]
                {
                   //new Claim(ClaimTypes.Role, user.Role),
                   new Claim(ClaimTypes.Name,$"{user.FirstName} {user.LastName}")
                });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
                var token = jwtTokenHandler.CreateToken(tokenDescriptor);
                return jwtTokenHandler.WriteToken(token);
            }



        private Task<bool> CheckTitleExistAsync(string title)
        {
            throw new NotImplementedException();
        }

      /*  [HttpPost("InsertData")]
        public async Task<IActionResult> InsertData([FromBody] Task taskObj)
        {
            if (taskObj == null)
                return BadRequest();
            return Ok(new
            {
                Message = "To-Do List Created Sussessfully!."
            });
        }*/


        //using these user can only login when it with has token men
        //[Authorize]
        //ctreate a API to get all users
        [HttpGet]
        public async Task<ActionResult<Users>> GetAllUsers()
        {
            return Ok(await _authContext.Users.ToListAsync());
        }

    }



}

