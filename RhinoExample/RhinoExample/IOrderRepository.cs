namespace RhinoExample
{
    /// <summary>
    /// Repository responsible for storing orders
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Adds the order (item in stock) placed by the customer to the repository.
        /// </summary>
        /// <param name="customerThatOrders">The customer that orders.</param>
        /// <param name="productOrdered">The product ordered.</param>
        /// <param name="quatityOrderedInStock">The quatity ordered in stock.</param>
        /// <param name="orderAmount">The order amount.</param>
        void AddOrder(ICustomer customerThatOrders, IProduct productOrdered, int quatityOrderedInStock, int orderAmount);

        /// <summary>
        /// Adds the back order (items not in stock) placed by the customer to the repository.
        /// </summary>
        /// <param name="customerThatOrders">The customer that orders.</param>
        /// <param name="productOrdered">The product ordered.</param>
        /// <param name="quatityOrderedNotInStock">The quatity ordered not in stock.</param>
        void AddBackOrder(ICustomer customerThatOrders, IProduct productOrdered, int quatityOrderedNotInStock);
    }
}
