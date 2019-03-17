using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HangfireSignalR.Models;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using HangfireSignalR.Hubs;
using Microsoft.AspNetCore.Authorization;

namespace HangfireSignalR.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHubContext<JobProgressHub> _hubContext;

        public HomeController(IHubContext<JobProgressHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public IActionResult Progress()
        {
            return View();
        }

        [Authorize]
        public IActionResult BackgroundCounter()
        {
            BackgroundJob.Enqueue(() => BackgroundCounterAsync());
            return RedirectToAction("Progress");
        }

        [NonAction]
        public async Task BackgroundCounterAsync()
        {
            for (int i = 0; i <= 100; i += 5)
            {
                await _hubContext.Clients.All.SendAsync("progress", i);

                await Task.Delay(500);
            }
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
