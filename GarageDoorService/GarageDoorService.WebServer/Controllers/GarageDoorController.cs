namespace GarageDoorService.WebServer.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Shared;

    [Route("api/[controller]")]
    [Route("api/[controller]/[action]")]
    [Route("api/[controller]/[action]/{id}")]
    [AllowAnonymous]
    class GarageDoorController : Controller
    {
        private readonly IGarageDoorService _service;

        public GarageDoorController(IGarageDoorService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult> OpenLeftDoor([FromBody] string key)
        {
            //Might want to add some type of authentication code?
            await _service.OpenLeftGarageDoor();
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> OpenRightDoor([FromBody] string key)
        {
            //Might want to add some type of authentication code?
            await _service.OpenRightGarageDoor();
            return Ok();
        }
    }
}
