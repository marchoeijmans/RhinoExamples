namespace RhinoExample
{
    /// <summary>
    /// The Order CalculationService is responsible for calculating the order amount based on:
    /// the Customer (required for determine a discount)
    /// the Product ordered, 
    /// the Quantity ordered.
    /// </summary>
    public interface IOrderCalculationService
    {
        /// <summary>
        /// Calculates the order amount.
        /// </summary>
        /// <param name="customer">The customer that ordered.</param>
        /// <param name="product">The product ordered.</param>
        /// <param name="quantity">The quantity of the product ordered.</param>
        /// <returns>
        /// The order amount.
        /// </returns>
        int CalculateOrderAmount(ICustomer customer, IProduct product, int quantity);
    }
}
