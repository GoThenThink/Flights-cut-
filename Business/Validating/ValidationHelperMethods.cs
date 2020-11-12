using RandomStartup.Extensions.Common.Types.Exceptions;
using FluentPatchValidation.Base;
using FluentValidation;
using FluentValidation.Internal;
using System.Linq;

namespace Flights.Business.Validating
{
    /// <summary>
    /// Class that stores helper methods for validation and exception handling in the Business Layer.
    /// </summary>
    /// <typeparam name="TBusinessModel">Type of business model object.</typeparam>
    internal static class ValidationHelperMethods<TBusinessModel, TValidator>
                where TValidator : IValidator<TBusinessModel>
    {
        /// <summary>
        /// Checks whether the property is allowed for editing.
        /// </summary>
        /// <param name="validator">Instance of a validator.</param>
        /// <param name="property">Name of a property to patch.</param>
        internal static void CheckPropertyForPatching(IPatchFieldProfile<TBusinessModel> validator, string property)
        {
            if (!validator.Validate(property.ToLower()))
            {
                throw new DenyPatchException(property);
            }
        }

        /// <summary>
        /// Validates obtained object and throws exception if it fails.
        /// </summary>
        /// <param name="instanceToValidate">Instance to validate.</param>
        /// <param name="validator">Validator.</param>
        /// <param name="ruleSet">Ruleset to use.</param>
        internal static void ValidateObject(TBusinessModel instanceToValidate, TValidator validator, string ruleSet = "default")
        {
            var validationResult = validator.Validate(instanceToValidate, op => op.IncludeRuleSets(ruleSet));
            if (!validationResult.IsValid)
                throw new Flights.Business.Exceptions.ValidationException(validationResult.Errors.ToList());
        }

        /// <summary>
        /// Validates specific property according to rules defined in default ruleset and throws exception if it fails.
        /// </summary>
        /// <param name="instanceToValidate">Instance to validate.</param>
        /// <param name="validator">Validator.</param>
        /// <param name="property">Property to validate.</param>
        internal static void ValidatePropertyForPatching(TBusinessModel instanceToValidate, TValidator validator, string property)
        {
            var ruleDescriptor = validator.CreateDescriptor();
            var rulesOfMember = ruleDescriptor.GetRulesForMember(property);
            var exactRule = rulesOfMember.FirstOrDefault(c => c.RuleSets.Length==0);

            if(!(exactRule is null))
            {
                var errorList = exactRule.Validate(new ValidationContext<TBusinessModel>(instanceToValidate, new PropertyChain(), new RulesetValidatorSelector())).ToList();
                if(errorList.Count > 0)
                    throw new Flights.Business.Exceptions.ValidationException(errorList);
            }
        }

        /// <summary>
        /// Validates specific property according to rules defined in custom ruleset and throws exception if it fails.
        /// </summary>
        /// <param name="instanceToValidate">Instance to validate.</param>
        /// <param name="validator">Validator.</param>
        /// <param name="ruleSet">Ruleset to use.</param>
        /// <param name="property">Property to validate.</param>
        internal static void ValidatePropertyForPatching(TBusinessModel instanceToValidate, TValidator validator, string ruleSet, string property)
        {
            var ruleDescriptor = validator.CreateDescriptor();
            var rulesOfMember = ruleDescriptor.GetRulesForMember(property);
            var exactRule = rulesOfMember.FirstOrDefault(c => c.RuleSets.Contains(ruleSet));

            if (!(exactRule is null))
            {
                var errorList = exactRule.Validate(new ValidationContext<TBusinessModel>(instanceToValidate, new PropertyChain(), new RulesetValidatorSelector(ruleSet))).ToList();
                if (errorList.Count > 0)
                    throw new Flights.Business.Exceptions.ValidationException(errorList);
            }
        }
    }
}
