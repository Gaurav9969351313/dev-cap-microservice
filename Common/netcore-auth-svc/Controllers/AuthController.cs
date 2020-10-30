using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using netcore_auth_svc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using OpenTracing;

namespace netcore_auth_svc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IHttpClientFactory _factory;

        public AuthController(
         IConfiguration configuration,
         ITracer tracer,
         IHttpClientFactory factory,
         UserManager<IdentityUser> userManager,
         SignInManager<IdentityUser> signInManager,
         RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            this._signInManager = signInManager;
            this._roleManager = roleManager;

            this._factory = factory;

            var operationName = "AuthController/";
            var builder = tracer.BuildSpan(operationName);

            using (var scope = builder.StartActive(true))
            {
                var span = scope.Span;

                var log = $"Endpoint Called ";
                span.Log(log + " " + span.ToString());
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("getByAdminOnly")]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "Admin action results 1", "Admin action results 2" };
        }

        [Authorize(Policy = "MemberDep")] // role User
        [HttpGet("getByMemberDepPolicy")]
        public ActionResult<IEnumerable<string>> getByDepPolicy()
        {
            return new string[] { "Member Dep Policy action results 1", "Member  Dep Policy Action results 2" };
        }


        [Authorize(Policy = "AdminDep")] // role Admin
        [HttpGet("getByAdminDepPolicy")]
        public ActionResult<IEnumerable<string>> getByAdminDepPolicy()
        {
            return new string[] { "Admin Dep Policy action results 1", "Admin Dep Policy Action results 2" };
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> GetNormally()
        {
        // public async Task<IEnumerable<string>> GetAsync()  
        // {  
            var data = GetSomeThingFromOrderApi();  
            return new string[] { "value1", "value2" };
        }


        [HttpPost("signup")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<IdentityResult>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Signup(SignupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                // if ((await _userManager.FindByEmailAsync(model.Email)) != null)
                // {

                if (!(await _roleManager.RoleExistsAsync(model.Role)))
                {
                    var role = new IdentityRole { Name = model.Role };
                    var roleResult = await _roleManager.CreateAsync(role);

                    if (!roleResult.Succeeded)
                    {
                        var errors = roleResult.Errors.Select(s => s.Description);
                        ModelState.AddModelError("Role", string.Join(",", errors));
                        return BadRequest(ModelState);
                    }
                }

                var user = new IdentityUser
                {
                    Email = model.Email,
                    UserName = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                user = await _userManager.FindByEmailAsync(model.Email);

                if (result.Succeeded)
                {
                    var claim = new Claim("Department", model.Department);
                    await _userManager.AddClaimAsync(user, claim);
                    await _userManager.AddToRoleAsync(user, model.Role);

                    Ok(result);
                }
                // }
            }
            return Ok();
        }


        [HttpPost("signin")]
        public async Task<IActionResult> Signin(SigninViewModel model)
        {
            var issuer = _configuration["Tokens:Issuer"];
            var audience = _configuration["Tokens:Audience"];
            var key = _configuration["Tokens:Key"];
            var tokenExpiryInMin = _configuration["Tokens:ExpiryInMin"];

            if (ModelState.IsValid)
            {
                var signinResult = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

                if (signinResult.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Username);
                    if (user != null)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Email , user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti , user.Id),
                        };

                        var keyBytes = Encoding.UTF8.GetBytes(key);
                        var theKey = new SymmetricSecurityKey(keyBytes);
                        var creds = new SigningCredentials(theKey, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.Now.AddMinutes(Convert.ToDouble(tokenExpiryInMin)), signingCredentials: creds);

                        return Ok(new { sucess = true, token = new JwtSecurityTokenHandler().WriteToken(token) });
                    }
                }
                else
                {
                    // ToDo: Send ErrorResponse Model 
                    ModelState.AddModelError("responseDescription", "Cannot login.");
                    ModelState.AddModelError("sucess", "0");
                    return BadRequest(ModelState);
                }
            }
            return Ok(model);
        }

        private async Task<string> GetSomeThingFromOrderApi()
        {
            var client = _factory.CreateClient("logging-api");
            var requestMsg = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5000/gateway/logging-svc/values");
            var responseMsg = await client.SendAsync(requestMsg);
            var data = await responseMsg.Content.ReadAsStringAsync();
            return data;
        }

        [HttpPost("signout")]
        public async Task<IActionResult> Signout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
    }
}