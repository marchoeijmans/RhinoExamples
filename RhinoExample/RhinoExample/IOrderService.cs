namespace RhinoExample
{
    /// <summary>
    /// Service responsible for ordering a product selected by a customer
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Orders the product.
        /// </summary>
        /// <param name="customerThatOrders">The customer that orders.</param>
        /// <param name="productNameToOrder">The product name to order.</param>
        /// <param name="quantity">The quantity.</param>
        void OrderProduct(ICustomer customerThatOrders, string productNameToOrder, int quantity);
    }
}
