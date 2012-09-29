namespace OrderService.UnitTests
{
    using RhinoExample;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Rhino.Mocks;
    
    
    /// <summary>
    ///This is a test class for OrderServiceTest and is intended
    ///to contain all OrderServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class OrderServiceTest
    {
        private TestContext testContextInstance;

        MockRepository mockRepository = new MockRepository();
        IOrderService orderService;

        // Mocks
        IConfirmationService confirmationServiceMock;
        IProductRepository productRepositoryMock;
        IOrderAmountCalculator orderAmountCalculatorMock;
        
        // Stubs
        IProductRepository productRepositoryStub;
        IOrderAmountCalculator orderAmountCalculatorStub;
        private ICustomer customerStub;
        private IProduct productStub;

        // Setup and expectations
        private string productSelectedByCustomer;
        private int quantity;
        private int orderAmount;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
        }
        
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
        }
        
        [TestInitialize()]
        public void MyTestInitialize()
        {
            this.mockRepository = new MockRepository();

            this.productRepositoryStub = mockRepository.Stub<IProductRepository>();
            this.orderAmountCalculatorStub = mockRepository.Stub<IOrderAmountCalculator>();
            
            this.customerStub = MockRepository.GenerateStub<ICustomer>();
            this.customerStub.Name = "CustomerName";
            this.customerStub.Address = "CustomerAddress";

            this.productStub = MockRepository.GenerateStub<IProduct>();
            this.productStub.Name = "ProductName";
            this.productStub.Description = "ProductDescription";
            this.productStub.Price = 10;

            this.productRepositoryMock = mockRepository.DynamicMock<IProductRepository>();
            this.orderAmountCalculatorMock = mockRepository.DynamicMock<IOrderAmountCalculator>();
            this.confirmationServiceMock = mockRepository.DynamicMock<IConfirmationService>();

            this.productSelectedByCustomer = "ProductName";
            this.quantity = 100;
            this.orderAmount = 1000;
        }
        
        [TestCleanup()]
        public void MyTestCleanup()
        {
        }
        
        #endregion

        [TestMethod()]
        public void OrderProduct_Should_Not_Order_When_OrderSelected_ByCustomer_DoesNot_Exists()
        {
            this.orderService = new OrderService(this.productRepositoryStub, this.orderAmountCalculatorMock, this.confirmationServiceMock);

            // Record expectations
            using (mockRepository.Record())
            {
                // Expectation
                SetupResult
                    .For(productRepositoryStub.GetProductByName(this.productSelectedByCustomer))
                    .Return(null);

                // Expectation that Order Amount Calculator is not called due the Product is not available in the ProductRepository.
                this.orderAmountCalculatorMock.Expect(t => t.CalculateOrderAmount(this.productStub, this.quantity))
                    .WhenCalled(x => x.ReturnValue = 0)
                    .Return(0)
                    .Repeat.Times(0);
            }

            // Verify expectations
            using (mockRepository.Playback())
            {
                this.orderService.OrderProduct(this.customerStub, productSelectedByCustomer, this.quantity);
            }


        }


        [TestMethod()]
        public void OrderProduct_Should_Call_OrderRepository_To_Return_Order_Selected_By_Customer()
        {
            this.orderService = new OrderService(this.productRepositoryMock, this.orderAmountCalculatorStub, this.confirmationServiceMock);

            // Record expectations
            using (mockRepository.Record())
            {
                // Expectation that GetProductByName is called exactly once with right arguments
                this.productRepositoryMock.Expect(t => t.GetProductByName(this.productSelectedByCustomer))
                    .WhenCalled(x => x.ReturnValue = this.productStub)
                    .Return(this.productStub)
                    .Repeat.Times(1);
            }

            // Verify expectations
            using (mockRepository.Playback())
            {
                this.orderService.OrderProduct(this.customerStub, productSelectedByCustomer, this.quantity);
            }
        }

        [TestMethod()]
        public void OrderProduct_Should_Call_AmmountCalculator_To_Calculate_Order_Amount_Once()
        {
            this.orderService = new OrderService(this.productRepositoryStub, this.orderAmountCalculatorMock, this.confirmationServiceMock);

            // Record expectations
            using (mockRepository.Record())
            {
                // Expectation that stub returns the product based on the Selected ProductName
                SetupResult
                    .For(this.productRepositoryStub.GetProductByName(this.productSelectedByCustomer))
                    .Return(this.productStub);

                // Expectation that Order Amount Calculator is called exactlu once with right arguments.
                this.orderAmountCalculatorMock.Expect(t => t.CalculateOrderAmount(this.productStub, this.quantity))
                    .WhenCalled(x => x.ReturnValue = this.orderAmount)
                    .Return(this.orderAmount)
                    .Repeat.Times(1);
            }

            // Verify expectations
            using (mockRepository.Playback())
            {
                this.orderService.OrderProduct(this.customerStub, productSelectedByCustomer, this.quantity);
            }
        }

        [TestMethod()]
        public void OrderProduct_Should_Call_ConfirmationServer_To_Send_OrderConfirmation_Of_ProductSelected_To_Customer_Once()
        {
            this.orderService = new OrderService(this.productRepositoryStub, this.orderAmountCalculatorStub, this.confirmationServiceMock);

            // Record expectations
            using (mockRepository.Record())
            {
                SetupResult
                    .For(productRepositoryStub.GetProductByName(this.productSelectedByCustomer))
                    .Return(this.productStub);

                SetupResult
                    .For(orderAmountCalculatorStub.CalculateOrderAmount(this.productStub, this.quantity))
                    .Return(orderAmount);

                // This call records if the right confirmation is send to the customer
                this.confirmationServiceMock.SendConfirmationToCustomer(this.customerStub, this.productStub, orderAmount);
            }

            // Verify expectations
            using (mockRepository.Playback())
            {
                this.orderService.OrderProduct(this.customerStub, productSelectedByCustomer, this.quantity);
            }
        }
    }
}
