using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DemoEx
{
    /// <summary>
    /// Логика взаимодействия для MainWindow1.xaml
    /// </summary>
    public partial class MainWindow1 : Window
    {
        private List<Product> _products;
        public MainWindow1(User user)
        {
            InitializeComponent();
            Trade123Entities db = new Trade123Entities();
            _products = db.Product.ToList();
            lvProducts.ItemsSource = _products;
            textAll.Text = _products.Count.ToString();

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
            textCurrent.Text = lvProducts.ItemsSource.Cast<Product>().Count().ToString();
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

            lvProducts.ItemsSource = products;
        }        
    }
}
