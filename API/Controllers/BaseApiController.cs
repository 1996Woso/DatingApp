using System;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[ServiceFilter(typeof(LogUserActivity))]//Utilize LogUserActivity service to update last seen(LastActive attribute)
[ApiController]
[Route("api/[controller]")]
public class BaseApiController: ControllerBase
{

}
