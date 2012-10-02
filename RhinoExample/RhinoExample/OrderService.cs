namespace RhinoExample
{
    using System;

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
        /// The Order repository used to store Order details, for the Products Ordered by a Client
        /// </summary>
        private IOrderRepository orderRepository;

        /// <summary>
        /// The Confirmation Service used by the Order Service.
        /// </summary>
        private IConfirmationService confirmationService;

        /// <summary>
        /// The Order Calculation Service used by the Order Service.
        /// </summary>
        private IOrderCalculationService orderCalculationService;

        ICustomer customer;
        IProduct productOrdered = null;

        private int quantityOrdered = 0;
        
        private int itemsInstockOrdered = 0;
        private int itemsNotInStockForBackOrder = 0;
        
        private int orderAmount = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderService"/> class.
        /// </summary>
        /// <param name="productRepository">The product repository.</param>
        /// <param name="orderRepository">The order repository.</param>
        /// <param name="amountCalculator">The amount calculator.</param>
        /// <param name="confirmationService">The confirmation service.</param>
        public OrderService(
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            IOrderCalculationService amountCalculator, 
            IConfirmationService confirmationService)
        {
            this.producRepository = productRepository;
            this.orderRepository = orderRepository;
            this.orderCalculationService = amountCalculator;
            this.confirmationService = confirmationService;
        }

        /// <summary>
        /// Implements <see cref="IOrderService.OrderProduct(ICustomer, string, int)"/>.
        /// </summary>
        /// <param name="customer">The customer that orders.</param>
        /// <param name="productNameToOrder">The product name to order.</param>
        /// <param name="quantityOrdered">The quantityOrdered.</param>
        public void OrderProduct(
            ICustomer customerThatOrdered, 
            string productNameToOrder, 
            int quantityOrdered)
        {
            this.customer = customerThatOrdered;
            this.productOrdered = this.producRepository.GetProductByName(productNameToOrder);
            this.quantityOrdered = quantityOrdered;
            
            // Guard to make sure only a Product available in the ProductRepository can be ordered by a Customer
            if (this.productOrdered == null)
                throw new ApplicationException("Product does not exist in Repository.");

            this.CalculateItemsInAndOutStock();

            // all items are in stock, place order
            if (this.itemsInstockOrdered.Equals(this.quantityOrdered))
            {
                this.PlaceOrder();
            }
            // not all items are in stock, place Order for items in stock, place BackOrder for items not in stock
            else
            {
                this.PlaceOrder();
                this.PlaceBackOrder();
            }
        }

        /// <summary>
        /// Calculates the items in and out stock.
        /// </summary>
        private void CalculateItemsInAndOutStock()
        {
            int itemsInStock = this.producRepository.GetNumberOfItemsOfProductOrderedInStock(this.productOrdered, this.quantityOrdered);
            this.itemsInstockOrdered = (itemsInStock >= this.quantityOrdered) ? this.quantityOrdered : itemsInStock;
            this.itemsNotInStockForBackOrder = (itemsInStock < this.quantityOrdered) ? (this.quantityOrdered - itemsInStock) : 0;
        }

        /// <summary>
        /// Places the order.
        /// </summary>
        private void PlaceOrder()
        {
            this.orderAmount = this.orderCalculationService.CalculateOrderAmount(this.customer, this.productOrdered, this.quantityOrdered);
            this.orderRepository.AddOrder(this.customer, this.productOrdered, this.itemsInstockOrdered, this.orderAmount);
            this.producRepository.UpdateNumberOfItemsInStock(this.productOrdered, this.itemsInstockOrdered);
            this.confirmationService.SendOrderConfirmationToCustomer(this.customer, this.productOrdered, this.orderAmount, this.itemsInstockOrdered);
        }

        /// <summary>
        /// Places the back order.
        /// </summary>
        private void PlaceBackOrder()
        {
            this.orderRepository.AddBackOrder(this.customer, this.productOrdered, this.itemsNotInStockForBackOrder);
            this.confirmationService.SendBackOrderConfirmationToCustomer(this.customer, this.productOrdered, this.itemsNotInStockForBackOrder);
        }
    }
}
