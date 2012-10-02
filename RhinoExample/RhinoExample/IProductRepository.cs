namespace RhinoExample
{
    using System.Collections.Generic;
    
    /// <summary>
    /// Repository that contains Products that can be ordered by a customer
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Gets the product by name.
        /// </summary>
        /// <param name="productName">Name of the product.</param>
        /// <returns>When the product exist, the products, otherwise null.</returns>
        IProduct GetProductByName(string productName);

        /// <summary>
        /// Gets the number of items of the product ordered in stock.
        /// </summary>
        /// <param name="productOrdered">The product ordered.</param>
        /// <param name="quantityOrdered">The quantity ordered.</param>
        /// <returns>
        /// The number of items of the product ordered in stock.
        /// </returns>
        int GetNumberOfItemsOfProductOrderedInStock(IProduct productOrdered, int quantityOrdered);

        /// <summary>
        /// Updates the number of items in stock.
        /// </summary>
        /// <param name="productOrdered">The product ordered.</param>
        /// <param name="quantityOrdered">The quantity ordered.</param>
        void UpdateNumberOfItemsInStock(IProduct productOrdered, int quantityOrdered);
    }
}
