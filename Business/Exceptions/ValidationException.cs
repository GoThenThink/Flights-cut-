using RandomStartup.Extensions.Common.Types.Exceptions.Base;
using RandomStartup.Extensions.Common.Types.Exceptions.Common;
using FluentValidation.Results;
using System.Collections.Generic;
using System.Linq;

namespace Flights.Business.Exceptions
{
    /// <summary>
    /// Exception occurs while validating obtained object
    /// </summary>
    public class ValidationException : BaseException, IValidationException
    {
        /// <summary>
        /// Exception info
        /// </summary>
        public IDictionary<string, string[]> Errors { get; }

        /// <summary/>
        private ValidationException()
            : base("One or more validation errors",
                "link to related site",
                 400)
        {
            Errors = new Dictionary<string, string[]>();
        }

        /// <summary/>
        public ValidationException(IReadOnlyList<ValidationFailure> failures)
            : this()
        {
            Errors = failures
                .GroupBy(x => x.PropertyName)
                .Distinct()
                .ToDictionary(x => x.Key, x => x.Select(y => y.ErrorMessage).ToArray());
        }
    }
}