using FluentValidation;

namespace QiECommerceAPI.Models.Validation
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(100);

            RuleFor(p => p.PriceIQD)
                .GreaterThanOrEqualTo(0).WithMessage("PriceIQD cannot be negative.");

            RuleFor(p => p.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");
        }
    }
}
