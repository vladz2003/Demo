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
    /// Логика взаимодействия для CartWindow.xaml
    /// </summary>
    public partial class CartWindow : Window
    {
        private List<OrderProduct> _orderedProducts;
        private decimal? _totalSum;
        private decimal? _totalDiscountSum;
        private Order _order;
        private User _user;
        public CartWindow(List<OrderProduct> orderedProducts, Order order, User user)
        {
            InitializeComponent();

            _user = user;
            _orderedProducts = orderedProducts;
            _order = order;
            dataGridCart.ItemsSource = _orderedProducts;

            SetTotalSum();

            tbTotalSum.Text = _totalSum.ToString();
            tbTotalDiscountSum.Text = _totalDiscountSum.ToString();

            ComboBox_PickupPoints.ItemsSource = Trade123Entities.Context().PickupPoint.ToList();
        }
        private void SetTotalSum()
        {
            _totalSum = 0;
            _totalDiscountSum = 0;
            foreach (var item in _orderedProducts)
            {
                _totalSum += item.Product.ProductCost;
                _totalDiscountSum += item.Product.ProductDiscountCost;
            }
        }

        private void Button_MakeOrder_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBox_PickupPoints.SelectedItem as PickupPoint == null)
            {
                MessageBox.Show("Выберете пункт выдачи");
                return;
            }

            var countProducts = 0;
            var db = Trade123Entities.Context();

            _order.PickupPoint = ComboBox_PickupPoints.SelectedItem as PickupPoint;
            _order.OrderCreateDate = DateTime.Now;

            Random rand = new Random();
            _order.OrderGetCode = rand.Next(100, 999);

            int orderId;

            foreach (var item in _orderedProducts)
            {
                countProducts++;
                if (item.Product.ProductQuantityInStock < 1)
                {
                    _order.OrderDeliveryDate = DateTime.Now.AddDays(6);

                    db.Order.Add(_order);
                    db.SaveChanges();
                    foreach (var product in _orderedProducts)
                    {
                        product.OrderID = _order.OrderID;
                    }
                    db.OrderProduct.AddRange(_orderedProducts);
                    db.SaveChanges();
                    TalonWindow talonWindow1 = new TalonWindow(_order, _orderedProducts, _totalSum, _totalDiscountSum, _user);
                    talonWindow1.Show();
                    this.Close();
                    return;
                }
            }

            if (countProducts > 3)
            {
                _order.OrderDeliveryDate = DateTime.Now.AddDays(3);
            }
            else
            {
                _order.OrderDeliveryDate = DateTime.Now.AddDays(6);
            }


            db.Order.Add(_order);
            db.SaveChanges();
            foreach (var product in _orderedProducts)
            {
                product.OrderID = _order.OrderID;
            }
            foreach(var item in _orderedProducts)
            {
               db.OrderProduct.Add(item);
               db.SaveChanges();             
            }
           

            TalonWindow talonWindow = new TalonWindow(_order, _orderedProducts, _totalSum, _totalDiscountSum, _user);
            talonWindow.Show();
            this.Close();
        }

        private void Button_GoBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow1 mainWindow1 = new MainWindow1(_user);
            mainWindow1.Show();
            this.Close();
        }
    }
}
