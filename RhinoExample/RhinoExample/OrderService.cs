using System;
namespace RhinoExample
{
    /// <summary>
    /// Implements <see cref="IOrderService.cs"/>.
    /// </summary>
    public class OrderService : IOrderService
    {
        /// <summary>
        /// The Product Repository used by the Order Service.
        /// </summary>
        private IProductRepository producRepository;

        /// <summary>
        /// The Confirmation Service used by the Order Service.
        /// </summary>
        private IConfirmationService confirmationService;

        /// <summary>
        /// The Order Amount Calculator used by the Order Service.
        /// </summary>
        private IOrderAmountCalculator orderAmountCalculator;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderService"/> class.
        /// </summary>
        /// <param name="productRepository">The product repository.</param>
        /// <param name="amountCalculator">The amount calculator.</param>
        /// <param name="confirmationService">The confirmation service.</param>
        public OrderService(IProductRepository productRepository, IOrderAmountCalculator amountCalculator, IConfirmationService confirmationService)
        {
            this.producRepository = productRepository;
            this.orderAmountCalculator = amountCalculator;
            this.confirmationService = confirmationService;
        }

        /// <summary>
        /// Implements <see cref="IOrderService.OrderProduct(ICustomer, string, int)"/>.
        /// </summary>
        /// <param name="customerThatOrders">The customer that orders.</param>
        /// <param name="productNameToOrder">The product name to order.</param>
        /// <param name="quantity">The quantity.</param>
        public void OrderProduct(ICustomer customerThatOrders, string productNameToOrder, int quantity)
        {
            IProduct productOrdered;
            int orderAmount;

            productOrdered = this.producRepository.GetProductByName(productNameToOrder);
            
            // Guard to make sure only Product available in the ProductRepository can be ordered by a Customer
            if (productOrdered == null)
                return;

            orderAmount = this.orderAmountCalculator.CalculateOrderAmount(productOrdered, quantity);
            this.confirmationService.SendConfirmationToCustomer(customerThatOrders, productOrdered, orderAmount);
        }
    }
}
