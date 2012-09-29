namespace RhinoExample
{
    using System.Collections.Generic;
    
    /// <summary>
    /// Repository that contains Products that can be ordered by a customer
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Gets all products.
        /// </summary>
        /// <returns>All products that can be ordered.</returns>
        List<IProduct> GetAllProducts();

        /// <summary>
        /// Gets the product by name.
        /// </summary>
        /// <param name="productName">Name of the product.</param>
        /// <returns>When the product exist, the products, otherwise null.</returns>
        IProduct GetProductByName(string productName);
    }
}
