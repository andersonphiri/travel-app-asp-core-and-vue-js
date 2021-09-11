using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Travel.Application.Common.Behaviors
{
    internal class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger _logger;
        private readonly Stopwatch _timer;
        public PerformanceBehavior(ILogger<TRequest> logger)
        {
            _logger = logger;
            _timer = new Stopwatch();
        }
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _timer.Start();
            var response = await next();
            _timer.Stop();
            var elapsedMilliseconds = _timer.ElapsedMilliseconds;
            if (elapsedMilliseconds <= 500) return response;
            var requestname = typeof(TRequest).Name;
            _logger.LogWarning("Travel Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}", requestname, elapsedMilliseconds, request);
            return response;
        }
    }
}
