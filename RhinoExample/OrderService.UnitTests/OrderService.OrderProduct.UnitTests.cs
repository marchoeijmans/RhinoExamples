namespace OrderService.UnitTests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rhino.Mocks;
    using RhinoExample;
    
    /// <summary>
    ///This is a test class for OrderServiceTest and contains all OrderService.Order Unit Tests
    ///</summary>
    [TestClass()]
    public class OrderServiceTest
    {
        private TestContext testContextInstance;

        MockRepository mockRepository = new MockRepository();
        IOrderService orderService;

        // Mocks
        IProductRepository productRepositoryMock;
        IOrderRepository orderRepositoryMock;
        IConfirmationService confirmationServiceMock;
        IOrderCalculationService orderAmountCalculatorMock;
        
        // Stubs
        IProductRepository productRepositoryStub;
        IOrderCalculationService calculationServiceStub;
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
            this.calculationServiceStub = mockRepository.Stub<IOrderCalculationService>();
            
            this.customerStub = MockRepository.GenerateStub<ICustomer>();
            this.customerStub.Name = "CustomerName";
            this.customerStub.Address = "CustomerAddress";

            this.productStub = MockRepository.GenerateStub<IProduct>();
            this.productStub.Name = "ProductName";
            this.productStub.Description = "ProductDescription";
            this.productStub.Price = 10;

            this.productRepositoryMock = mockRepository.DynamicMock<IProductRepository>();
            this.orderRepositoryMock = mockRepository.DynamicMock<IOrderRepository>();
            this.orderAmountCalculatorMock = mockRepository.DynamicMock<IOrderCalculationService>();
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
        [ExpectedException(typeof(ApplicationException), "Product does not exist in Repository.")]
        public void OrderProduct_Should_Not_Order_When_OrderSelected_ByCustomer_DoesNot_Exists()
        {
            this.orderService = new OrderService(
                this.productRepositoryStub,
                this.orderRepositoryMock,
                this.orderAmountCalculatorMock, 
                this.confirmationServiceMock);

            // Record expectations
            using (mockRepository.Record())
            {
                // Expectation
                SetupResult
                    .For(productRepositoryStub.GetProductByName(this.productSelectedByCustomer))
                    .Return(null);
            }

            this.orderService.OrderProduct(this.customerStub, productSelectedByCustomer, this.quantity);
        }

        [TestMethod()]
        public void OrderProduct_Should_Retrieve_Product_Selected_By_Customer_From_ProductRepository()
        {
            this.orderService = new OrderService(
                this.productRepositoryMock, 
                this.orderRepositoryMock,
                this.calculationServiceStub, 
                this.confirmationServiceMock);

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

        [TestMethod]
        public void OrderProduct_Should_Place_Order_For_Items_In_Stock()
        {
            this.orderService = new OrderService(
                this.productRepositoryStub,
                this.orderRepositoryMock,
                this.calculationServiceStub,
                this.confirmationServiceMock);

            // Record expectations
            using (mockRepository.Record())
            {
                SetupResult
                    .For(productRepositoryStub.GetProductByName(this.productSelectedByCustomer))
                    .Return(this.productStub);

                SetupResult
                    .For(productRepositoryStub.GetNumberOfItemsOfProductOrderedInStock(this.productStub, this.quantity))
                    .Return(this.quantity + 1);

                SetupResult
                    .For(calculationServiceStub.CalculateOrderAmount(this.customerStub, this.productStub, this.quantity))
                    .Return(this.orderAmount);

                // Expectation that AddOrder is called exactly once with right arguments
                this.orderRepositoryMock.Expect(t => t.AddOrder(this.customerStub, this.productStub, this.quantity, this.orderAmount))
                    .Repeat.Times(1);
            }

            // Verify expectations
            using (mockRepository.Playback())
            {
                this.orderService.OrderProduct(this.customerStub, productSelectedByCustomer, this.quantity);
            }
        }

        [TestMethod]
        public void OrderProdcut_Should_Send_Confirmation_To_Customer_For_Products_Ordered()
        {
            this.orderService = new OrderService(
                this.productRepositoryStub,
                this.orderRepositoryMock,
                this.calculationServiceStub,
                this.confirmationServiceMock);

            // Record expectations
            using (mockRepository.Record())
            {
                SetupResult
                    .For(productRepositoryStub.GetProductByName(this.productSelectedByCustomer))
                    .Return(this.productStub);

                SetupResult
                    .For(productRepositoryStub.GetNumberOfItemsOfProductOrderedInStock(this.productStub, this.quantity))
                    .Return(this.quantity + 1);

                SetupResult
                    .For(calculationServiceStub.CalculateOrderAmount(
                        this.customerStub,
                        this.productStub,
                        this.quantity))
                    .Return(this.orderAmount);

                // Expectation that SentConfirmationToCustomer is called exactly once with right arguments
                this.confirmationServiceMock.Expect(t => t.SendOrderConfirmationToCustomer(this.customerStub, this.productStub, this.orderAmount, this.quantity))
                    .Repeat.Times(1);
            }

            // Verify expectations
            using (mockRepository.Playback())
            {
                this.orderService.OrderProduct(this.customerStub, productSelectedByCustomer, this.quantity);
            }
        }

        [TestMethod]
        public void OrderProduct_Should_Update_ProductRepository_For_Products_Ordered_When_All_Items_Ordered_Are_In_Stock()
        {
            this.orderService = new OrderService(
                this.productRepositoryStub,
                this.orderRepositoryMock,
                this.calculationServiceStub,
                this.confirmationServiceMock);

            // Record expectations
            using (mockRepository.Record())
            {
                SetupResult
                    .For(productRepositoryStub.GetProductByName(this.productSelectedByCustomer))
                    .Return(this.productStub);

                SetupResult
                    .For(productRepositoryStub.GetNumberOfItemsOfProductOrderedInStock(this.productStub, this.quantity))
                    .Return(this.quantity + 1);

                // Expectation that UpdateNumberOfItemsInStock is called exactly once with right arguments
                this.productRepositoryStub.Expect(t => t.UpdateNumberOfItemsInStock(this.productStub, this.quantity))
                    .Repeat.Times(1);
            }

            // Verify expectations
            using (mockRepository.Playback())
            {
                this.orderService.OrderProduct(this.customerStub, productSelectedByCustomer, this.quantity);
            }
        }

        [TestMethod]
        public void OrderProduct_Should_Place_BackOrder_For_Items_Not_In_Stock()
        {
            this.orderService = new OrderService(
                this.productRepositoryStub,
                this.orderRepositoryMock,
                this.calculationServiceStub,
                this.confirmationServiceMock);

            // Record expectations
            using (mockRepository.Record())
            {
                SetupResult
                    .For(productRepositoryStub.GetProductByName(this.productSelectedByCustomer))
                    .Return(this.productStub);

                SetupResult
                    .For(productRepositoryStub.GetNumberOfItemsOfProductOrderedInStock(this.productStub, this.quantity))
                    .Return(this.quantity - 1);

                // Expectation that AddOrder is called exactly once with right arguments
                this.orderRepositoryMock.Expect(t => t.AddBackOrder(this.customerStub, this.productStub, 1))
                    .Repeat.Times(1);
            }

            // Verify expectations
            using (mockRepository.Playback())
            {
                this.orderService.OrderProduct(this.customerStub, productSelectedByCustomer, this.quantity);
            }
        }

        [TestMethod]
        public void OrderProduct_Should_Update_ProductRepository_For_Products_Ordered_When_Not_All_Items_Ordered_Are_In_Stock()
        {
            this.orderService = new OrderService(
                this.productRepositoryStub,
                this.orderRepositoryMock,
                this.calculationServiceStub,
                this.confirmationServiceMock);

            // Record expectations
            using (mockRepository.Record())
            {
                SetupResult
                    .For(productRepositoryStub.GetProductByName(this.productSelectedByCustomer))
                    .Return(this.productStub);

                SetupResult
                    .For(productRepositoryStub.GetNumberOfItemsOfProductOrderedInStock(this.productStub, this.quantity))
                    .Return(this.quantity - 1);

                // Expectation that UpdateNumberOfItemsInStock is called exactly once with right arguments
                this.productRepositoryStub.Expect(t => t.UpdateNumberOfItemsInStock(this.productStub, this.quantity - 1))
                    .Repeat.Times(1);
            }

            // Verify expectations
            using (mockRepository.Playback())
            {
                this.orderService.OrderProduct(this.customerStub, productSelectedByCustomer, this.quantity);
            }
        }

        [TestMethod()]
        public void OrderProduct_Should_Calulate_OrderAmount_For_Product_Selected_For_Quantity_Entered_By_Customer()
        {
            this.orderService = new OrderService(
                this.productRepositoryStub, 
                this.orderRepositoryMock,
                this.orderAmountCalculatorMock, 
                this.confirmationServiceMock);

            // Record expectations
            using (mockRepository.Record())
            {
                // Expectation that stub returns the product based on the Selected ProductName
                SetupResult
                    .For(this.productRepositoryStub.GetProductByName(this.productSelectedByCustomer))
                    .Return(this.productStub);

                // Expectation that Order Amount Calculator is called exactly once with right arguments.
                this.orderAmountCalculatorMock.Expect(
                    t => t.CalculateOrderAmount(
                        this.customerStub,
                        this.productStub, 
                        this.quantity))
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
        public void OrderProduct_Should_Store_Order_In_OrderRepository()
        {
            this.orderService = new OrderService(
                this.productRepositoryStub, 
                this.orderRepositoryMock,
                this.calculationServiceStub, 
                this.confirmationServiceMock);

            // Record expectations
            using (mockRepository.Record())
            {
                SetupResult
                    .For(productRepositoryStub.GetProductByName(this.productSelectedByCustomer))
                    .Return(this.productStub);

                SetupResult
                    .For(productRepositoryStub.GetNumberOfItemsOfProductOrderedInStock(this.productStub, this.quantity))
                    .Return(this.quantity + 1);

                SetupResult
                    .For(calculationServiceStub.CalculateOrderAmount(
                    this.customerStub,
                    this.productStub, 
                    this.quantity))
                    .Return(orderAmount);

                // Expectation that the AddOrder is called exactly once with the right arguments
                this.orderRepositoryMock.Expect(t => t.AddOrder(this.customerStub, this.productStub, this.quantity, this.orderAmount))
                    .Repeat.Times(1);
            }

            // Verify expectations
            using (mockRepository.Playback())
            {
                this.orderService.OrderProduct(this.customerStub, productSelectedByCustomer, this.quantity);
            }
        }

      [TestMethod]
      public void OrderProduct_Should_Send_Confirmation_To_Customer_For_Items_Placed_In_BackOrder()
      {
          this.orderService = new OrderService(
              this.productRepositoryStub,
              this.orderRepositoryMock,
              this.calculationServiceStub,
              this.confirmationServiceMock);

          // Record expectations
          using (mockRepository.Record())
          {
              SetupResult
                  .For(productRepositoryStub.GetProductByName(this.productSelectedByCustomer))
                  .Return(this.productStub);

              SetupResult
                  .For(productRepositoryStub.GetNumberOfItemsOfProductOrderedInStock(this.productStub, this.quantity))
                  .Return(this.quantity - 1);

              // Expectation that SendBackOrderConfirmationToCustomer is called exactly once with right arguments
              this.confirmationServiceMock.Expect(t => t.SendBackOrderConfirmationToCustomer(this.customerStub, this.productStub, 1))
                  .Repeat.Times(1);
          }

          // Verify expectations
          using (mockRepository.Playback())
          {
              this.orderService.OrderProduct(this.customerStub, productSelectedByCustomer, this.quantity);
          }

      }
    }
}
