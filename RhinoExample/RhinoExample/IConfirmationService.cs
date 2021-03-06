﻿namespace RhinoExample
{
    /// <summary>
    /// Service responsible for sending a confirmation message with the product ordered, the order amount to the customer
    /// </summary>
    public interface IConfirmationService
    {
        /// <summary>
        /// Sends the order confirmation (product ordered and order amount) to the customer.
        /// </summary>
        /// <param name="customerThatHasOrdered">The customer that has ordered.</param>
        /// <param name="productOrderByCustomer">The product order by customer.</param>
        /// <param name="orderAmount">The order amount.</param>
        void SendOrderConfirmationToCustomer(ICustomer customerThatHasOrdered, IProduct productOrderByCustomer, int orderAmount, int numberOfItemsOrdered);

        void SendBackOrderConfirmationToCustomer(ICustomer customerThatHasOrdered, IProduct productOrderedByCustomerNotInStock, int numberOfItemsPlacedInBackOrder);
    }
}
