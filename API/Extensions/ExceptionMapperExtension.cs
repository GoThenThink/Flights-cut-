using RandomStartup.Extensions.AspNetCore.Middleware;
using RandomStartup.Extensions.Common.Types.Exceptions.Base;
using Flights.Business.Exceptions;

namespace Flights.Extensions
{
    internal static class ExceptionMapperExtension
    {
        public static IExceptionMapper RegisterMapper(this IExceptionMapper mapper)
        {
            return mapper
                .AddMap<IValidationException, ValidationException>();
        }
    }
}
