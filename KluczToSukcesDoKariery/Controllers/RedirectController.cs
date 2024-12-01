using Microsoft.AspNetCore.Mvc;

namespace KluczToSukcesDoKariery.Controllers
{
	public class RedirectController : Controller
	{
		public IActionResult ToIdentityAccount(string catchAll)
		{
			return RedirectPermanent($"/Identity/Account/{catchAll}");
		}
	}
}
