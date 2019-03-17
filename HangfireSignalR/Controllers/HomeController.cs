using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HangfireSignalR.Models;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using HangfireSignalR.Hubs;
using Microsoft.AspNetCore.Authorization;
using System;

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
            var userName = User.Identity.Name;
            var jobId = BackgroundJob.Enqueue(() => BackgroundCounterAsync(userName));
            //Register Transfer account job type in cache/db, and clear once above job is finished.
            //BackgroundJob.ContinueWith(
            //                jobId,
            //                () => Console.WriteLine("Continuation!"));
            return RedirectToAction("Progress");
        }

        [NonAction]
        [AutomaticRetry(Attempts = 0)]
        public async Task BackgroundCounterAsync(string user)
        {
            try
            {
                for (int i = 0; i <= 100; i += 5)
                {
                    await _hubContext.Clients.Group(user).SendAsync("progress", i);
                    await Task.Delay(500);
                }
            }
            catch (Exception)
            {
                //Rollback changes
                throw;
            }
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
