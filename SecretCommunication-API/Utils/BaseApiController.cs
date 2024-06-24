using Microsoft.AspNetCore.Mvc;
using SecretCommunication_API.Models.ImageSteganography;

namespace SecretCommunication_API.Utils
{
    public class BaseApiController : ControllerBase
    {
        protected IActionResult SetStatusCode<T>(T result) where T : class
        {
            if (result is ImageProcessingResult imageResult)
            {
                if (imageResult.Error != null)
                {
                    return BadRequest(imageResult.Error);
                }

                return File(imageResult.ImageBytes, "image/jpeg", "output_image.jpg");
            }

            if (result is MessageExtractionResult messageResult)
            {
                if (messageResult.Error != null)
                {
                    return BadRequest(messageResult.Error);
                }

                return Ok(messageResult.Message);
            }

            return BadRequest("Invalid result type.");
        }
    }
}
