using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly ServerContext server;

        public ArticlesController(ServerContext server)
        {
            this.server = server;
        }

        [HttpGet("get-article-by-category")]
        public async Task<ActionResult<IEnumerable<Articles>>> GetArticleByCategory([FromQuery] string category, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                page = page < 1 ? 1 : page;
                int skip = (page - 1) * pageSize;

                var result = await server.Articles
                    .Where(item => item.category_id == category)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, e.ToString());
            }
        }

        [HttpGet("get-article-by-id")]
        public async Task<ActionResult<Articles>> GetArticleByID([FromQuery] string id)
        {
            try
            {
                var result = await server.Articles.FirstOrDefaultAsync(item => item.articles_id == id);
                if (result == null)
                {
                    return NotFound("Article not found.");
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, e.ToString());
            }
        }

        [HttpGet("get-latest-article")]
        public async Task<ActionResult<IEnumerable<Articles>>> GetLatestArticle()
        {
            try
            {
                var result = await server.Articles.OrderByDescending(item => item.createAt).ToListAsync();
                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, e.ToString());
            }
        }

        [HttpGet("search-article")]
        public async Task<ActionResult<IEnumerable<Articles>>> SearchArticles([FromQuery] string content)
        {
            try
            {
                var result = await server.Articles.Where(item => item.title.Contains(content)).ToListAsync();

                if (result == null || !result.Any())
                {
                    return NotFound("No articles found with the given title.");
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, e.ToString());
            }
        }

        [HttpPut("edit-article")]
        public async Task<IActionResult> EditArticle([FromBody] Articles updatedArticle)
        {
            try
            {
        
                if (updatedArticle == null)
                {
                    return BadRequest("Lỗi data.");
                }

                var existingArticle = await server.Articles.FindAsync(updatedArticle);
                if (existingArticle == null)
                {
                    return NotFound("Không tìm thấy bài viết.");
                }

                existingArticle.title = updatedArticle.title;
                existingArticle.content = updatedArticle.content;
                existingArticle.category_id = updatedArticle.category_id;
                await server.SaveChangesAsync();
                return Ok(existingArticle);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, e.ToString());
            }
        }

        [HttpPost("new-article")]
        public async Task<IActionResult> NewArticle([FromBody] Articles newArticle)
        {
            try
            {
                if (newArticle == null)
                {
                    return BadRequest("Lỗi data.");
                }
                await server.Articles.AddAsync(newArticle);
                await server.SaveChangesAsync();
                return CreatedAtAction(nameof(GetArticleByID), new { id = newArticle.articles_id }, newArticle);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, e.ToString());
            }
        }



        [HttpDelete("delete-article/{id}")]
        public async Task<IActionResult> NewArticle(string id)
        {
            try
            {
                var article = await server.Articles.FindAsync(id);
                if (article == null)
                {
                    return BadRequest("Lỗi data.");
                }

                server.Articles.Remove(article);
                await server.SaveChangesAsync();
                return Ok("success");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, e.ToString());
            }
        }

        [HttpPut("submit-comment")]
        public async Task<IActionResult> SubmitComment()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                var userId = jsonToken?.Claims.First(claim => claim.Type == "user_id").Value;
                if (userId != null)
                {

                    using (var reader = new StreamReader(Request.Body))
                    {
                        var body = await reader.ReadToEndAsync();
                        var commentData = JsonConvert.DeserializeObject<Comments>(body);

                        var article = await server.Articles
                            .Include(a => a.Comments)
                            .FirstOrDefaultAsync(a => a.articles_id == commentData.articles_id);

                        if (article == null)
                        {
                            return BadRequest("Lỗi data.");
                        }

                        var newComment = new Comments
                        {

                            user_id = userId,
                            content = commentData.content,
                        };

                        article.Comments.Add(newComment);
                        await server.SaveChangesAsync();
                        return Ok("success");
                    }

                }
                else
                {
                    return NotFound("Lỗi trong truy cập người dùng");
                }


              
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, e.ToString());
            }
        }
        [HttpPut("like-aritcle")]
        public async Task<IActionResult> likeAritcle()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                var userId = jsonToken?.Claims.First(claim => claim.Type == "user_id").Value;
                if (userId != null)
                {
                    using (var reader = new StreamReader(Request.Body))
                    {
                        var body = await reader.ReadToEndAsync();
                        var likeData = JsonConvert.DeserializeObject<Likes>(body);

                        var article = await server.Articles
                            .Include(a => a.Likes)
                            .FirstOrDefaultAsync(a => a.articles_id == likeData.articles_id);

                        if (article == null)
                        {
                            return BadRequest("Lỗi data.");
                        }

                        var newSubmitLike = new Likes
                        {
                            user_id = likeData.user_id,

                        };

                        article.Likes.Add(newSubmitLike);
                        await server.SaveChangesAsync();
                        return Ok("success");
                    }


                }
                else
                {

                    return NotFound("Lỗi trong truy cập người dùng");

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, e.ToString());
            }
        }







    }
}
