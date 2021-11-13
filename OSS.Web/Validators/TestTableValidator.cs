using FluentValidation;
using OSS.Services.AppServices;
using OSS.Services.Models;
using OSS.Web.Framework;

namespace OSS.Web.Validators
{
    public class TestTableValidator : AbstractValidator<TestTableModel>
    {
        public TestTableValidator(ILocalizationService localizationService, IWorkContext workContext)
        {
            int langId = workContext.WorkingLanguageId;
            RuleFor(x => x.Text1)
                .NotEmpty().WithMessage(localizationService.GetResource("required", langId))
                .EmailAddress().WithMessage(localizationService.GetResource("emial is required", langId));

            RuleFor(x => x.Text2)
                .NotNull().WithMessage(localizationService.GetResource("this is required", langId))
                .Length(1, 5).WithMessage(localizationService.GetResource("from 1 to 5", langId));
        }
    }
}
