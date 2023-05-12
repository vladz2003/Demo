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
    /// Логика взаимодействия для AddEditProductWindow.xaml
    /// </summary>
    public partial class AddEditProductWindow : Window
    {
        private Product _product = new Product();
        private Trade123Entities _context = Trade123Entities.Context();
        private User _user;
        public AddEditProductWindow(User user, Product product)
        {
            InitializeComponent();

            _product= product;
            _user = user;

            if (_product.ProductID != 0)
            {
                tbProductArticleNumber.IsEnabled = false;
            }
            else
            {
                tbProductArticleNumber.IsEnabled = true;
            }

            DataContext = _product;

            comboCategory.ItemsSource = _context.ProductCategory.ToList();
            comboManufacturer.ItemsSource = _context.ProductManufacturer.ToList();
            comboSupplier.ItemsSource = _context.ProductSupplier.ToList();
            comboUnitType.ItemsSource = _context.UnitType.ToList();

            if (_product.ProductID != 0)
            {
                comboCategory.SelectedItem = _product.ProductCategory;
                comboManufacturer.SelectedItem = _product.ProductManufacturer;
                comboSupplier.SelectedItem = _product.ProductSupplier;
                comboUnitType.SelectedItem = _product.UnitType;
            }
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            var isMaxDiscountDigit = Int32.TryParse(tbProductMaxDiscountAmount.Text, out var maxDiscount);
            var isDiscountDigit = Int32.TryParse(tbProductDiscountAmount.Text, out var discount);

            if (maxDiscount < discount)
            {
                errors.AppendLine("Размер текущей скидки не должен превышать максимальную скидку");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            _product.ProductCategory = comboCategory.SelectedItem as ProductCategory;
            _product.ProductManufacturer = comboManufacturer.SelectedItem as ProductManufacturer;
            _product.ProductSupplier = comboSupplier.SelectedItem as ProductSupplier;
            _product.UnitType = comboUnitType.SelectedItem as UnitType;

            if (_product.ProductID == 0)
            {
                _context.Product.Add(_product);
            }

            try
            {
                _context.SaveChanges();
                MessageBox.Show("Информация сохранена");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_ReturnToProductPage_Click(object sender, RoutedEventArgs e)
        {
            if ((MessageBox.Show("Не сохраненные данные будут потеряны. Продолжить?", "Возврат на страницу", MessageBoxButton.YesNo, MessageBoxImage.Question)) == MessageBoxResult.Yes)
            {
                Administrator admin = new Administrator(_user);
                admin.Show();
                this.Close();
            }
        }
    }
}
