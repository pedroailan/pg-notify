using Commons;
using Microsoft.AspNetCore.Mvc;

namespace PgNotify_Producer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController(Producer producer) : ControllerBase
    {
        private readonly Producer _producer = producer;

        [HttpPost]
        public async Task<IActionResult> Notification([FromBody] Notification notification)
        {
            bool notify = await _producer.Notification(notification);
            return Ok(notify);
        }
    }
}
