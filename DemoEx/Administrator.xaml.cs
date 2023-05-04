using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DemoEx
{
    /// <summary>
    /// Логика взаимодействия для Administrator.xaml
    /// </summary>
    public partial class Administrator : Window
    {
        private List<Product> _products;
        public Administrator(User user)
        {
            InitializeComponent();
            _products = Trade123Entities.Context().Product.ToList();
            List_Products.ItemsSource = _products;
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
            textCurrent.Text = List_Products.ItemsSource.Cast<Product>().Count().ToString();
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

            List_Products.ItemsSource = products;
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = List_Products.SelectedItems.Cast<Product>().ToList();

            if (selectedProduct.Count == 0)
            {
                MessageBox.Show("Выберите товар, который желаете удалить нажатием на карточку");
                return;
            }
            if ((MessageBox.Show("Выбранный товар будет удален. Продолжить?", "Удаление товара", MessageBoxButton.YesNo, MessageBoxImage.Question)) == MessageBoxResult.Yes)
            {
                var db = Trade123Entities.Context();
                foreach (var item in selectedProduct)
                {
                    var deleted = db.OrderProduct.Where(p => p.ProductID == item.ProductID);
                    db.OrderProduct.RemoveRange(deleted);
                }
                db.SaveChanges();
                db.Product.RemoveRange(selectedProduct);
                db.SaveChanges();
                MessageBox.Show("Товар был успешно удален");
                _products = db.Product.ToList();
                List_Products.ItemsSource = _products;
                sortProducts();
            }
        }

        private void Button_Edit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {

        }
    }




}
