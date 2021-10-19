using FluentValidation;
using OSS.Services.AppServices;
using OSS.Services.Models;
using OSS.Web.Framework;

namespace OSS.Web.Validators
{
    public class EmailAccountValidator : AbstractValidator<EmailAccountModel>
    {
        public EmailAccountValidator(ILocalizationService localizationService, IWorkContext workContext)
        {
            int langId = workContext.WorkingLanguageId;
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(localizationService.GetResource("required", langId))
                .Length(1, 255).WithMessage(localizationService.GetResource("maximum255", langId))
                .EmailAddress().WithMessage(localizationService.GetResource("emial is required", langId));

            RuleFor(x => x.DisplayName)
                .NotNull().WithMessage(localizationService.GetResource("this is required", langId))
                .Length(1, 255).WithMessage(localizationService.GetResource("maximum255", langId));

            RuleFor(x => x.Host)
                .NotNull().WithMessage(localizationService.GetResource("this is required", langId))
                .Length(1, 255).WithMessage(localizationService.GetResource("maximum255", langId));

            RuleFor(x => x.Password)
                .NotNull().WithMessage(localizationService.GetResource("this is required", langId))
                .Length(1, 255).WithMessage(localizationService.GetResource("maximum255", langId));

            RuleFor(x => x.Username)
                .NotNull().WithMessage(localizationService.GetResource("this is required", langId))
                .Length(1, 255).WithMessage(localizationService.GetResource("maximum255", langId));

        }
    }
}
