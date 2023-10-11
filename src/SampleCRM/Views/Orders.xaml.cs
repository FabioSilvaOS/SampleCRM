﻿using OpenRiaServices.DomainServices.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SampleCRM.Web.Views
{
    public partial class Orders : BasePage
    {
        private OrderStatusContext _orderStatusContext = new OrderStatusContext();
        private CountryCodesContext _countryCodesContext = new CountryCodesContext();
        private OrderContext _orderContext = new OrderContext();
        private OrderItemsContext _orderItemsContext = new OrderItemsContext();
        private CustomersContext _customersContext = new CustomersContext();
        private ProductsContext _productsContext = new ProductsContext();
        private TaxTypesContext _taxTypesContext = new TaxTypesContext();
        private ShippersContext _shippersContext = new ShippersContext();
        private PaymentTypeContext _paymentTypesContext = new PaymentTypeContext();

        private bool _orderItemsTabSelected;

        private IEnumerable<Models.Orders> _ordersCollection;
        public IEnumerable<Models.Orders> OrdersCollection
        {
            get { return _ordersCollection; }
            set
            {
                if (_ordersCollection != value)
                {
                    _ordersCollection = value;
                    OnPropertyChanged();
                    OnPropertyChanged("FilteredOrdersCollection");
                    SelectedOrder = FilteredOrdersCollection.FirstOrDefault();
                }
            }
        }

        public IEnumerable<Models.Orders> FilteredOrdersCollection
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_searchText))
                {
                    return _ordersCollection;
                }
                else
                {
                    return _ordersCollection.Where(x => x.OrderID.ToString().Contains(_searchText.ToLowerInvariant()));
                }
            }
        }

        private IEnumerable<Models.CountryCodes> _countryCodes;
        public IEnumerable<Models.CountryCodes> CountryCodes
        {
            get { return _countryCodes; }
            set
            {
                if (_countryCodes != value)
                {
                    _countryCodes = value;
                    OnPropertyChanged();
                }
            }
        }

        private IEnumerable<Models.OrderStatus> _statuses;
        public IEnumerable<Models.OrderStatus> Statuses
        {
            get { return _statuses; }
            set
            {
                if (_statuses != value)
                {
                    _statuses = value;
                    OnPropertyChanged();
                }
            }
        }

        private Models.Orders _selectedOrder = new Models.Orders();
        public Models.Orders SelectedOrder
        {
            get { return _selectedOrder; }
            set
            {
                if (_selectedOrder != value)
                {
                    _selectedOrder = value;
                    OnPropertyChanged();
                    OnPropertyChanged("AnySelectedOrder");
                    LoadCustomer();
                    LoadOrderItemsOfOrder();
#if DEBUG
                    Console.WriteLine($"Orders, Order: {value.OrderID} selected");
#endif
                }
            }
        }

        public bool AnySelectedOrder => _selectedOrder != null;

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    OnPropertyChanged("FilteredOrdersCollection");
                    SelectedOrder = FilteredOrdersCollection.FirstOrDefault();
                    //OnPropertyChanged("SelectedOrder");
                    //OnPropertyChanged("AnySelectedOrder");
                }
            }
        }


        private IEnumerable<Models.OrderItems> _orderItemsCollection;
        public IEnumerable<Models.OrderItems> OrderItemsCollection
        {
            get { return _orderItemsCollection; }
            set
            {
                if (_orderItemsCollection != value)
                {
                    _orderItemsCollection = value;
                    OnPropertyChanged();
                    OnPropertyChanged("FilteredOrderItemsCollection");
                    SelectedOrderItem = FilteredOrderItemsCollection.FirstOrDefault();

                }
            }
        }

        public IEnumerable<Models.OrderItems> FilteredOrderItemsCollection
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_searchOrderItemText))
                {
                    return _orderItemsCollection;
                }
                else
                {
                    return _orderItemsCollection.Where(x => x.OrderID.ToString().Contains(_searchOrderItemText));
                }
            }
        }

        private Models.OrderItems _selectedOrderItem;
        public Models.OrderItems SelectedOrderItem
        {
            get { return _selectedOrderItem; }
            set
            {
                if (_selectedOrderItem != value)
                {
                    _selectedOrderItem = value;
                    OnPropertyChanged();
                    //LoadProduct();
#if DEBUG
                    Console.WriteLine($"OrderItems, OrderItem: {value.OrderLine} selected");
#endif
                }
            }
        }

        private string _searchOrderItemText;
        public string SearchOrderItemText
        {
            get { return _searchOrderItemText; }
            set
            {
                if (_searchOrderItemText != value)
                {
                    _searchOrderItemText = value;
                    OnPropertyChanged();
                    OnPropertyChanged("FilteredOrderItemsCollection");
                    SelectedOrderItem = FilteredOrderItemsCollection.FirstOrDefault();
                    //OnPropertyChanged("SelectedOrder");
                }
            }
        }


        public Orders()
        {
            InitializeComponent();
            DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadElements();
        }

        private async void LoadElements()
        {
            var query = _orderContext.GetOrdersQuery();
            var op = await _orderContext.LoadAsync(query);
            OrdersCollection = op.Entities;

            await LoadCountryCodes();
            await LoadStatuses();
#if DEBUG
            Console.WriteLine("Orders Collection:" + OrdersCollection.Count());
            foreach (var item in OrdersCollection)
            {
                Console.WriteLine("Order Id:" + item.OrderID);
                Console.WriteLine("Customer Id:" + item.CustomerID);
            }
#endif
        }

        private async Task LoadCountryCodes()
        {
            var query = _countryCodesContext.GetCountriesQuery();
            var op = await _countryCodesContext.LoadAsync(query);
            CountryCodes = op.Entities;

            foreach (var o in OrdersCollection)
            {
                o.CountryCodes = CountryCodes;
                o.ShipCountryName = CountryCodes.SingleOrDefault(x => x.CountryCodeID == o.ShipCountryCode).Name;
            }
        }

        private async Task LoadStatuses()
        {
            var query = _orderStatusContext.GetOrderStatusQuery();
            var op = await _orderStatusContext.LoadAsync(query);
            Statuses = op.Entities;

            foreach (var o in OrdersCollection)
            {
                o.Statuses = Statuses;
                o.StatusDesc = Statuses.FirstOrDefault(x => x.Status == o.Status)?.Name;
            }
        }

        private async void LoadCustomer()
        {
            if (SelectedOrder == null)
                return;

            if (SelectedOrder.CustomersCombo != null && SelectedOrder.CustomersCombo.Any())
                return;

            if (SelectedOrder.Customer == null || SelectedOrder.Customer.CustomerID != SelectedOrder.CustomerID)
            {
                var query = _customersContext.GetCustomersQuery().Where(x => x.CustomerID == SelectedOrder.CustomerID);
                var op = await _customersContext.LoadAsync(query);
                var customer = op.Entities.FirstOrDefault();
                SelectedOrder.Customer = customer;
            }
        }

        private async void LoadProduct(Models.OrderItems orderItem)
        {
            if (orderItem == null)
                return;

            if (orderItem.Product == null)
            {
                var query = _productsContext.GetProductsQuery().Where(x => x.ProductID == orderItem.ProductID);
                var op = await _productsContext.LoadAsync(query);
                var product = op.Entities.FirstOrDefault();
                orderItem.Product = product;
            }
        }

        private async void LoadTaxRate(Models.OrderItems orderItem)

        {
            if (orderItem == null)
                return;

            if (orderItem.TaxRate == 0)
            {
                var query = _taxTypesContext.GetTaxTypesQuery().Where(x => x.TaxTypeID == orderItem.TaxType);
                var op = await _taxTypesContext.LoadAsync(query);
                var taxRate = Convert.ToDecimal(op.Entities.FirstOrDefault()?.Rate, CultureInfo.InvariantCulture);
                orderItem.TaxRate = taxRate;
            }
        }

        private async void LoadOrderItemsOfOrder()
        {
            if (SelectedOrder == null)
                return;

            if (!_orderItemsTabSelected)
                return;

            var orderId = SelectedOrder.OrderID;
            var query = _orderItemsContext.GetOrderItemsOfOrderQuery(SelectedOrder.OrderID);
            var op = await _orderItemsContext.LoadAsync(query);
            OrderItemsCollection = op.Entities;

            foreach (var orderItem in OrderItemsCollection)
            {
                LoadProduct(orderItem);
                LoadTaxRate(orderItem);
            }

#if DEBUG
            Console.WriteLine("Order Items Collection:" + OrderItemsCollection.Count());
            foreach (var item in OrderItemsCollection)
            {
                Console.WriteLine("Order Item Id:" + item.OrderLine);
            }
#endif
        }

        private void btnSearchCancel_Click(object sender, RoutedEventArgs e)
        {
            SearchText = string.Empty;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void grdOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if DEBUG
            Console.WriteLine("grdOrders_SelectionChanged, {0} Items Added", e.AddedItems.Count);
#endif
        }

        private async void btnNew_Click(object sender, RoutedEventArgs e)
        {
            var result = await OrderAddEditWindow.Show(new Models.Orders
            {
                IsEditMode = true,
                CustomersCombo = (await _customersContext.LoadAsync(_customersContext.GetCustomersQuery())).Entities,
                Statuses = (await _orderStatusContext.LoadAsync(_orderStatusContext.GetOrderStatusQuery())).Entities,
                CountryCodes = (await _countryCodesContext.LoadAsync(_countryCodesContext.GetCountriesQuery())).Entities,
                Shippers = (await _shippersContext.LoadAsync(_shippersContext.GetShippersQuery())).Entities,
                PaymentTypes = (await _paymentTypesContext.LoadAsync(_paymentTypesContext.GetPaymentTypesQuery())).Entities
            }, _orderContext);

            if (result)
            {
                NavigationService.Refresh();
            }
        }

        private void tcDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var anySelected = e.AddedItems.Count > 0;
            if (anySelected)
            {
                _orderItemsTabSelected = e.AddedItems.Contains(tbOrderItems);
                if (_orderItemsTabSelected)
                {
                    LoadOrderItemsOfOrder();
                }
            }
            else
            {
                _orderItemsTabSelected = false;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure to delete the order and all items belong that order?", MessageBoxButton.OKCancel);
            if (result != MessageBoxResult.OK)
                return;

            if (_orderContext.Orders.CanRemove)
            {
                _orderContext.Orders.Remove(SelectedOrder);
                _orderContext.SubmitChanges(OnDeleteSubmitCompleted, null);
            }
            else
            {
                throw new AccessViolationException("RIA Service Delete Entity for Customer Context is denied");
            }
        }

        private void OnDeleteSubmitCompleted(SubmitOperation so)
        {
            if (so.HasError)
            {
                if (so.Error.Message.StartsWith("Submit operation failed. Access to operation"))
                {
                    ErrorWindow.Show("Access Denied", "Insuficient User Role", so.Error.Message);
                }
                else
                {
                    ErrorWindow.Show("Access Denied", so.Error.Message, "");
                }
                so.MarkErrorAsHandled();
            }
            else
            {
                NavigationService.Refresh();
            }
        }

        private void formOrder_EditEnded(object sender, DataFormEditEndedEventArgs e)
        {
            if (e.EditAction == DataFormEditAction.Commit)
            {
                _customersContext.SubmitChanges(OnFormOrderSubmitCompleted, null);
            }
            else if (e.EditAction == DataFormEditAction.Cancel)
            {
                SelectedOrder.IsEditMode = false;
            }
        }

        private void OnFormOrderSubmitCompleted(SubmitOperation so)
        {
            if (so.HasError)
            {
                if (so.Error.Message.StartsWith("Submit operation failed. Access to operation"))
                {
                    ErrorWindow.Show("Access Denied", "Insuficient User Role", so.Error.Message);
                }
                else
                {
                    ErrorWindow.Show("Access Denied", so.Error.Message, "");
                }
                so.MarkErrorAsHandled();
            }
            else
            {
                OnPropertyChanged("SelectedOrder");
                OnPropertyChanged("OrdersCollection");
                OnPropertyChanged("FilteredOrdersCollection");
            }
        }

        private void btnOrderItemSearchCancel_Click(object sender, RoutedEventArgs e)
        {
            SearchOrderItemText = string.Empty;
        }

        private void grdOrderItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if DEBUG
            Console.WriteLine("grdOrderItems_SelectionChanged, {0} Items Added", e.AddedItems.Count);
#endif
        }

        private async void btnShowOrderItem_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedOrderItem == null)
            {
                throw (new ArgumentNullException("Selected Order Item can't be null"));
            }

            if (SelectedOrderItem.ProductsCombo == null)
            {
                SelectedOrderItem.ProductsCombo = (await _productsContext.LoadAsync(_productsContext.GetProductsQuery())).Entities;
            }

            if (SelectedOrderItem.TaxTypes == null)
            {
                SelectedOrderItem.TaxTypes = (await _taxTypesContext.LoadAsync(_taxTypesContext.GetTaxTypesQuery())).Entities;
            }

            var result = await OrderItemAddEditWindow.Show(SelectedOrderItem, _orderItemsContext);
            if (result)
            {
                NavigationService.Refresh();
            }
        }

        private async void btnNewOrderItem_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedOrder == null)
            {
                throw (new ArgumentNullException("Selected Order can't be null"));
            }

            var result = await OrderItemAddEditWindow.Show(new Models.OrderItems
            {
                IsEditMode = true,
                OrderID = SelectedOrder.OrderID,
                OrderLine = 0,
                ProductsCombo = (await _productsContext.LoadAsync(_productsContext.GetProductsQuery())).Entities,
                TaxTypes = (await _taxTypesContext.LoadAsync(_taxTypesContext.GetTaxTypesQuery())).Entities
            }, _orderItemsContext);

            if (result)
            {
                NavigationService.Refresh();
            }
        }
    }
}