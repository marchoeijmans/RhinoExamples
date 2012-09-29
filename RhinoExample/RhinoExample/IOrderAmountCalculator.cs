namespace RhinoExample
{
    /// <summary>
    /// The Order amount Calculator is responsible for calculating the order amount based on the product ordered and the quantity ordered
    /// </summary>
    public interface IOrderAmountCalculator
    {
        /// <summary>
        /// Calculates the order amount.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns>The order amount.</returns>
        int CalculateOrderAmount(IProduct product, int quantity);
    }
}
