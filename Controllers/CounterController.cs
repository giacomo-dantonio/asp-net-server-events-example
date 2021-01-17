using CounterApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CounterApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CounterController : ControllerBase
    {
        private readonly ILogger<CounterController> _logger;

        private readonly PersistentCounter _counter;

        private readonly JsonSerializerSettings _jsonSettings;

        public CounterController(
            ILogger<CounterController> logger,
            PersistentCounter persistentCounter)
        {
            _logger = logger;
            _counter = persistentCounter;
            _jsonSettings = new JsonSerializerSettings();
            _jsonSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
        }

        [HttpGet]
        public Counter GetCounter()
        {
            return new Counter
            {
                Value = _counter.Value
            };
        }

        [HttpPut]
        public ActionResult SetCounter(Counter counter)
        {
            _logger.LogInformation($"Setting counter to {counter.Value}.");
            _counter.Value = counter.Value;
            return new OkResult();
        }

        [HttpPut]
        [Route("Stop")]
        public ActionResult StopCounter()
        {
            _logger.LogInformation($"Stop increasing counter.");

            _counter.Running = false;
            return new OkResult();
        }

        [HttpPut]
        [Route("Start")]
        public ActionResult StartCounter()
        {
            _logger.LogInformation($"Start increasing counter.");

            _counter.Running = true;
            return new OkResult();
        }

        [HttpGet]
        [Route("Stream")]
        public async Task StreamCounter(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start counter streaming.");

            Response.StatusCode = 200;
            Response.Headers.Add("Content-Type", "text/event-stream");

            _counter.CounterUpdated += StreamValue;

            while (!cancellationToken.IsCancellationRequested) {
                await Task.Delay(1000);
            }

            _counter.CounterUpdated -= StreamValue;
            _logger.LogInformation("Stop counter streaming.");

            async void StreamValue(object sender, int value)
            {
                var messageJson = JsonConvert.SerializeObject(_counter, _jsonSettings);
                await Response.WriteAsync($"data:{messageJson}\n\n");
                await Response.Body.FlushAsync();
            }
        }
    }
}