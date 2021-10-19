using FluentValidation;
using OSS.Services.AppServices;
using OSS.Services.Models;
using OSS.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSS.Web.Validators
{
    public class PersonValidator : AbstractValidator<PersonModel>
    {
        public PersonValidator(ILocalizationService localizationService, IWorkContext workContext)
        {
            int langId = workContext.WorkingLanguageId;
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(localizationService.GetResource("required", langId))
                .EmailAddress().WithMessage(localizationService.GetResource("emial is required", langId));

            RuleFor(x => x.LastName)
                .NotNull().WithMessage(localizationService.GetResource("this is required", langId))
                .Length(1, 5).WithMessage(localizationService.GetResource("from 1 to 5", langId));
        }

    }
}
