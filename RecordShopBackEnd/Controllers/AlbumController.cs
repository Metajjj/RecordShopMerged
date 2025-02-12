using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

using RecordShop_BE.Services;
using RecordShop_BE.Tables;

namespace RecordShop_BE.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlbumController : ControllerBase
    {
        private IAlbumService service;
        public AlbumController(IAlbumService s)
        {
            service = s;
        }

        [HttpGet]
        public IActionResult GetallAlbums()
        {
            return Ok(service.GetAllAlbums());
        }

        [HttpGet("{id}")]
        public IActionResult GetAlbumById(string id)
        {
            try
            {
                var album = service.GetAlbumById(id);

                return (album == null) ? NotFound("Given ID is not found!") : Ok(JsonSerializer.Serialize(album, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch (FormatException ex)
            {
                return BadRequest("Param is NaN! (not a number)");
            }
        }


        [HttpPost]
        public IActionResult PostAlbum(Albums a)
        {
            //Only accept album object post, nothing else! i.e. if fails to json.Deserialize, throw err

            //returns 400 if ierror

            return Ok("Successfully added "+a.Title+" with ID of "+service.PostAlbum(a).Id);
        }

            //  HttpContext not accepted for actual env!
        /*[HttpPut]
        public IActionResult PutAlbum(HttpContext htpc) //cant test without parsing!
        {
            try
            {
                htpc.Request.Body.Position = 0; //Ensure is at 0 so readable

                var album = JsonSerializer.Deserialize<Albums>(new StreamReader(htpc.Request.Body).ReadToEndAsync().Result);

                var success = service.PutAlbum(album);

                //TODO err nullable obj must have val
                return success ? Ok(JsonSerializer.Serialize(album, new JsonSerializerOptions { WriteIndented = true })) : Accepted("No changes detected");
            }
            catch (JsonException ex)
            {
                return BadRequest("Body is not in a valid format! (not a Album!)\n" + ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return NotFound("Given ID is not found!");
            }            
        }*/

        [HttpPut]   //HttpContext htpc conflicts with actual env
        public IActionResult PutAlbum([FromBody] Albums A) //cant test without parsing!
        {
            try
            {
                var success = service.PutAlbum(A);

                return success ? Ok(JsonSerializer.Serialize(A, new JsonSerializerOptions { WriteIndented = true })) : Accepted("No changes detected");
            }
            catch (ArgumentNullException ex)
            {
                return NotFound("Given ID is not found!");
            }
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteAlbumId(string id)
        {
            try
            {
                var album = service.DeleteAlbumById(id);

                return (album == null) ? NotFound("Given ID is not found!") : Ok("DELETED:\n "+JsonSerializer.Serialize(album, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch (FormatException ex)
            {
                return BadRequest("Param is NaN! (not a number)");
            }
        }
    }
}
