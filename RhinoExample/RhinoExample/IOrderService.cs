namespace RhinoExample
{
    /// <summary>
    /// Service responsible for ordering a Product selected by a Customer, by:
    /// Calculating the Order Amnount (based on the Order Price and amount Ordered)
    /// Storing the Order to the OrderRepository
    /// Sending a confirmation message to the Customer
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Orders the product.
        /// </summary>
        /// <param name="customer">The customer that orders.</param>
        /// <param name="productNameToOrder">The product name to order.</param>
        /// <param name="quantityOrdered">The quantityOrdered.</param>
        void OrderProduct(ICustomer customerThatOrders, string productNameToOrder, int quantity);
    }
}
