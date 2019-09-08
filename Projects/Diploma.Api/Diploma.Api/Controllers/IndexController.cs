using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Diploma.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class IndexController : ControllerBase
	{
		[HttpPost]
		public void Post([FromForm] string value)
		{
		}
	}
}
