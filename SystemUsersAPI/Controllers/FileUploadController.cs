using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace SystemUsersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] string uniqName)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file was uploaded.");

                string profilePicsPath = Path.Combine(Directory.GetCurrentDirectory(), "ProfilePics");

                // Ensure the ProfilePics directory exists
                if (!Directory.Exists(profilePicsPath))
                {
                    try
                    {
                        Directory.CreateDirectory(profilePicsPath);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Unable to create ProfilePics directory: {ex.Message}");
                    }
                }

                // Generate a unique filename to prevent overwrites
                string uniqueFileName = $"{uniqName}_{file.FileName}";
                string filePath = Path.Combine(profilePicsPath, uniqueFileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok(new { fileName = uniqueFileName, path = filePath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
   
    [HttpGet("{fileName}")]
    public IActionResult GetUploadedImage(string fileName)
    {
        try
        {
            string profilePicsPath = Path.Combine(Directory.GetCurrentDirectory(), "ProfilePics");
            string filePath = Path.Combine(profilePicsPath, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound($"Image {fileName} not found.");
            }

            // Get the file's content type
            string contentType = "image/jpeg"; // Default to JPEG
            string extension = Path.GetExtension(fileName).ToLower();
            switch (extension)
            {
                case ".png":
                    contentType = "image/png";
                    break;
                case ".gif":
                    contentType = "image/gif";
                    break;
                case ".bmp":
                    contentType = "image/bmp";
                    break;
            }

            // Return the file stream
            var stream = System.IO.File.OpenRead(filePath);
            return File(stream, contentType);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    }
}