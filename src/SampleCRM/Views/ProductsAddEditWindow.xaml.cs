﻿using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SampleCRM.Web.Views
{
    public partial class ProductsAddEditWindow : ChildWindow
    {
        private const double windowSizeMult = .85;

        private ProductsContext _context;

        public static async Task<bool> Show(Models.Products product, ProductsContext context)
        {
            var window = new ProductsAddEditWindow(product, context);
            window.Width = Application.Current.MainWindow.ActualWidth * windowSizeMult;
            window.Height = Application.Current.MainWindow.ActualHeight * windowSizeMult;
            await window.ShowAndWait();
            return window.DialogResult.GetValueOrDefault(false);
        }

        public ProductsAddEditWindow()
        {
            InitializeComponent();
        }

        public ProductsAddEditWindow(Models.Products product, ProductsContext context)
            : this()
        {
            _context = context;
            productAddEditView.ProductViewModel = product;
            Title = product.IsNew ? "Add Product" : "Edit Product";
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            productAddEditView.Save(_context);
            //DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _context.RejectChanges();
            DialogResult = false;
        }

        private void productAddEditView_ProductDeleted(object sender, System.EventArgs e)
        {
            DialogResult = true;
        }

        private void productAddEditView_ProductAdded(object sender, System.EventArgs e)
        {
            DialogResult = true;
        }
    }
}