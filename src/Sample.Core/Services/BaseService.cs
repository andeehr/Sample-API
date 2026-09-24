using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Sample.Core.Services
{
    public abstract class BaseService<TService>
    {
        protected readonly ILogger<TService> _logger;
        protected readonly IMapper _mapper;

        public BaseService(ILogger<TService> logger, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
    }
}