using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DemoEx
{
    /// <summary>
    /// Логика взаимодействия для MainWindow1.xaml
    /// </summary>
    public partial class MainWindow1 : Window
    {
        private List<Product> _products;
        private User _user;
        private Order _order = new Order();
        private List<OrderProduct> _orderedProducts = new List<OrderProduct>();
        public MainWindow1(User user)
        {
            InitializeComponent();
            _user = user;
            Trade123Entities db = new Trade123Entities();
            _products = db.Product.ToList();
            ListView_Products.ItemsSource = _products;
            textAll.Text = _products.Count.ToString();

            _order.UserID = _user.UserID;
            _order.OrderStatusID = 1;

            ComboBoxFilterProductDiscountAmount.ItemsSource = new List<string>
            {
                "Все", "0-10%", "10-15%", "15-∞%"
            };
            ComboBoxFilterProductByPrice.ItemsSource = new List<string>
            {
                "По умолчанию", "По возрастанию", "По убыванию"
            };

            ComboBoxFilterProductDiscountAmount.SelectedIndex = 0;
            ComboBoxFilterProductByPrice.SelectedIndex = 0;
        }

        private void ComboBoxFilterProductDiscountAmount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sortProducts();
            refreshCurrentCountProducts();
        }

        private void ComboBoxFilterProductByPrice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sortProducts();
            refreshCurrentCountProducts();
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            sortProducts();
            refreshCurrentCountProducts();
        }

        private void refreshCurrentCountProducts()
        {
            textCurrent.Text = ListView_Products.ItemsSource.Cast<Product>().Count().ToString();
        }

        private void sortProducts()
        {
            var products = _products;

            switch (ComboBoxFilterProductDiscountAmount.SelectedIndex)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {
                        products = products.Where(p => p.ProductDiscountAmount < 10).ToList();
                        break;
                    }
                case 2:
                    {
                        products = products.Where(p => p.ProductDiscountAmount > 10 && p.ProductDiscountAmount < 15).ToList();
                        break;
                    }
                case 3:
                    {
                        products = products.Where(p => p.ProductDiscountAmount > 15).ToList();
                        break;
                    }
            }

            switch (ComboBoxFilterProductByPrice.SelectedIndex)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {
                        products = products.OrderBy(p => p.ProductCost).ToList();
                        break;
                    }
                case 2:
                    {
                        products = products.OrderByDescending(p => p.ProductCost).ToList();
                        break;
                    }
            }

            if (tbSearch.Text != "" && !(string.IsNullOrWhiteSpace(tbSearch.Text)))
            {
                products = products.FindAll(p => p.ProductName.Contains(tbSearch.Text));
            }

            ListView_Products.ItemsSource = products;
        }

        private void Button_AddToCart_Click(object sender, RoutedEventArgs e)
        {
            var selectedProducts = ListView_Products.SelectedItems.Cast<Product>().ToList();

            if (selectedProducts.Count == 0)
            {
                MessageBox.Show("Выберите товар, который желаете добавить в корзину нажатием на карточку");
                return;
            }

            foreach (var item in selectedProducts)
            {
                
                var productToAdd = _orderedProducts.Find(o => o.ProductID == item.ProductID);
                if (productToAdd == null)
                {
                    OrderProduct newOrderProduct = new OrderProduct
                    {
                        OrderID = _order.OrderID,
                        Product = item,
                        ProductID = item.ProductID,
                        Count = 1
                    };
                    _orderedProducts.Add(newOrderProduct);
                }
                else
                {
                    _orderedProducts.Find(o => o.ProductID == item.ProductID).Count++;
                }
            }

            MessageBox.Show("Продукт был добавлен в коризну");
            Button_GoToCart.Visibility = Visibility.Visible;
        }

        private void Button_GoToCart_Click(object sender, RoutedEventArgs e)
        {
            CartWindow cartWindow = new CartWindow(_orderedProducts, _order, _user);
            cartWindow.Show();
            this.Close();
        }
    }
}
