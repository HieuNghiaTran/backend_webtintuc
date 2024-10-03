using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace backend.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ServerContext server;
        public CategoryController(ServerContext server)
        {
            this.server = server;


        }
        [HttpGet("getall")]
        public async Task<ActionResult<IEnumerable<Categories>>> getAll()
        {


            try
            {
                var result = await server.Categories.ToListAsync();
                return Ok(result);
            }
            catch (Exception ex)

            {
                Console.WriteLine(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToString());
            }
        }

        [HttpPost("new-category")]
        public async Task<ActionResult> addCategory()
        {
            try
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    var body = await reader.ReadToEndAsync();
                    var category = JsonConvert.DeserializeObject<Categories>(body);
                    await server.Categories.AddAsync(category);

                    await server.SaveChangesAsync();

                    return Ok("success");

                }
            }
            catch (Exception ex)
            {
               
                Console.WriteLine(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToString());
            }
        }

        [HttpPut("edit-category")]
        public async Task<ActionResult> editCategory()
        {
            try
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    var body = await reader.ReadToEndAsync();
                    var category = JsonConvert.DeserializeObject<Categories>(body);
                    var exitsingCatgory = await server.Categories.FindAsync(category.category_id);
                    exitsingCatgory.category_name = category.category_name;
                    await server.SaveChangesAsync();
                    return Ok("succes");

                }
            }
            catch (Exception ex)

            {
                Console.WriteLine(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToString());
            }
        }

        [HttpDelete("delete-category")]
        public async Task<ActionResult> deleteCategory([FromBody] Categories category)
        {
            try
            {
                var exitsingCatgory = await server.Categories.FindAsync(category.category_id);
                server.Categories.Remove(exitsingCatgory);
                await server.SaveChangesAsync();
                return Ok("succes");
            }
            catch (Exception ex)

            {
                Console.WriteLine(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToString());
            }
        }




    }

}