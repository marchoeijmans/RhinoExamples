namespace RhinoExample
{
    /// <summary>
    /// A Customer that can order a Product,
    /// </summary>
    public interface ICustomer
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        string Address { get; set; }
    }
}
