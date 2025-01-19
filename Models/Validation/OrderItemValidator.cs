using FluentValidation;

namespace QiECommerceAPI.Models.Validation
{
    public class OrderItemValidator : AbstractValidator<OrderItem>
    {
        public OrderItemValidator()
        {
            RuleFor(oi => oi.ProductId)
                .GreaterThan(0).WithMessage("Invalid Product ID.");

            RuleFor(oi => oi.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        }
    }
}
