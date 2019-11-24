using Humanizer.Bytes;
using Microsoft.AspNetCore.Mvc;
using Portal.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Portal.Controllers
{
    public class HomeController : Controller
    {
        private static HttpClient _httpClient = new HttpClient();
        private static PortalViewModel _viewModel = new PortalViewModel();
        public static int RequestsCounter;

        public IActionResult Index()
        {
            var watch = Stopwatch.StartNew();

            RequestsCounter++;

            if (!Program.CheckIfThereIsThreadAvailable())
                return BadRequest(Messages.THREAD_IS_NO_THREAD_AVAILABLE);

            //Adiciona a requisição à fila do Pool de execuções. 
            //A requisição será processada assim que um espaço de execução for sendo liberado.
            ThreadPool.QueueUserWorkItem(new WaitCallback(t =>
            {
                _viewModel.PrepareViewModel(_httpClient, RequestsCounter);
            }));

            return View(_viewModel);
        }

        public IActionResult Information()
        {
            return View();
        }
    }
}