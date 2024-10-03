using backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BCrypt.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace backend.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ServerContext server;
        public readonly IConfiguration _config;
        private readonly Helper helper;

        public UserController(ServerContext server, IConfiguration config)
        {
            this.server = server;
            _config = config;  
            helper = new Helper(_config);
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp()
        {
            try
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    var body = await reader.ReadToEndAsync();
                    var user = JsonConvert.DeserializeObject<User>(body);

                    var isAlready = await server.Users.FirstOrDefaultAsync(u => u.username == user.username);

                    if (isAlready == null)
                    {
                        var passwordHash =  BCrypt.Net.BCrypt.EnhancedHashPassword(user.password, 13);
                        user.password = passwordHash;

                        await server.Users.AddAsync(user);
                        await server.SaveChangesAsync();
                        return Ok("Đăng ký thành công");
                    }
                    else
                    {
                        return BadRequest("Tài khoản đã tồn tại");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi");
            }
        }
        [HttpGet("getall")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUser()
        {
            try
            {
               
                var users = await server.Users.ToListAsync();
                return Ok(users);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi khi lấy danh sách người dùng");
            }
        }


        [HttpPost("signin")]
        public async Task<IActionResult> SignIn()
        {
            try
            {
                using (var reader = new StreamReader(Request.Body))
                {
          
                    var body = await reader.ReadToEndAsync();
                    var user = JsonConvert.DeserializeObject<User>(body);
                    var isAlready = await server.Users.FirstOrDefaultAsync(u => u.username == user.username);
                    if (isAlready != null)
                    {
                        bool auth = BCrypt.Net.BCrypt.Verify(user.password, isAlready.password);
                        if (auth)
                        { 
                                string token = helper.generateJWTToken(isAlready);
                                return Ok(token);

                        }
                        else
                        {

                            return BadRequest("Sai mật khẩu");

                        }


                    }
                    else
                    {
                        return BadRequest("Tài khoản không tồn tại");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError,e.ToString());
            }
        }

      





    }
}
