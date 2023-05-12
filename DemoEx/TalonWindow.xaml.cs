using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
using System.Xml.Linq;
using Path = System.IO.Path;

namespace DemoEx
{
    /// <summary>
    /// Логика взаимодействия для TalonWindow.xaml
    /// </summary>
    public partial class TalonWindow : Window
    {
        private Order _order;
        private List<OrderProduct> _orderProducts;
        private decimal? _totalSum;
        private decimal? _totalDiscountSum;
        private User _user;
        public TalonWindow(Order order, List<OrderProduct> orderProducts, decimal? totalSum, decimal? totalDiscountSum, User user)
        {
            InitializeComponent();

            _user = user;
            _order = order;
            _orderProducts = orderProducts;
            _totalSum = totalSum;
            _totalDiscountSum = totalDiscountSum;

            gridTalon.DataContext = _order;
            tbTotalSum.Text = totalSum.ToString();
            tbTotalDicountSum.Text = totalDiscountSum.ToString();
            tbDiscount.Text = (totalSum - totalDiscountSum).ToString();
            dataGridOrderedProducts.ItemsSource = _orderProducts;
        }

        private void Button_SaveToPdf_Click(object sender, RoutedEventArgs e)
        {
            PdfDocument document = new PdfDocument();

            document.Info.Title = "Заказ №" + _order.OrderID.ToString();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            XFont fontRegular = new XFont("Comic Sans MS", 20, XFontStyle.Regular);
            XFont fontBold = new XFont("Comic Sans MS", 20, XFontStyle.Bold);
            XFont fontBoldLarge = new XFont("Comic Sans MS", 26, XFontStyle.Bold);
            double margin = -180;
            gfx.DrawString(
                "Номер заказа: " + _order.OrderID.ToString(),
            fontBold,
                XBrushes.Black,
                new XRect(0, margin, page.Width, page.Height),
                XStringFormats.Center
                );
            margin += 40;
            gfx.DrawString(
                "Дата заказа: " + _order.OrderCreateDate.ToString(),
            fontRegular,
            XBrushes.Black,
                new XRect(0, margin, page.Width, page.Height),
                XStringFormats.Center
                );
            margin += 20;
            gfx.DrawString(
                "Дата доставки: " + _order.OrderDeliveryDate.ToString(),
            fontRegular,
            XBrushes.Black,
                new XRect(0, margin, page.Width, page.Height),
                XStringFormats.Center
                );
            margin += 40;

            gfx.DrawString(
                "Состав заказа:",
            fontRegular,
            XBrushes.Black,
                new XRect(0, margin, page.Width, page.Height),
                XStringFormats.Center
                );
            margin += 40;
            gfx.DrawString(
                "Наименование",
                fontBold,
            XBrushes.Black,
                new XRect(-200, margin, page.Width, page.Height),
                XStringFormats.Center
                );
            gfx.DrawString(
                "Цена",
                fontBold,
            XBrushes.Black,
                new XRect(0, margin, page.Width, page.Height),
                XStringFormats.Center
                );
            gfx.DrawString(
                "Количество",
            fontBold,
            XBrushes.Black,
                new XRect(200, margin, page.Width, page.Height),
                XStringFormats.Center
                );
            margin += 20;
            foreach (var item in _orderProducts)
            {
                gfx.DrawString(
                   item.Product.ProductName,
                fontRegular,
                XBrushes.Black,
                   new XRect(-200, margin, page.Width, page.Height),
                   XStringFormats.Center
                   );
                gfx.DrawString(
                   item.Product.ProductDiscountCost.ToString(),
                fontRegular,
                   XBrushes.Black,
                   new XRect(0, margin, page.Width, page.Height),
                   XStringFormats.Center
                   );
                gfx.DrawString(
                   item.Count.ToString(),
                fontRegular,
                   XBrushes.Black,
                   new XRect(200, margin, page.Width, page.Height),
                   XStringFormats.Center
                   );
                margin += 20;
            }

            margin += 20;
            gfx.DrawString(
                "Сумма: " + _totalSum.ToString(),
                fontRegular,
                XBrushes.Black,
                new XRect(0, margin, page.Width, page.Height),
                XStringFormats.Center
                );
            margin += 20;
            gfx.DrawString(
                "Скидка: " + (_totalSum - _totalDiscountSum).ToString(),
                fontRegular,
                XBrushes.Black,
                new XRect(0, margin, page.Width, page.Height),
                XStringFormats.Center
                );
            margin += 20;
            gfx.DrawString(
                "Сумма со скидкой: " + _totalDiscountSum.ToString(),
                fontRegular,
                XBrushes.Black,
                new XRect(0, margin, page.Width, page.Height),
                XStringFormats.Center
                );
            margin += 40;
            gfx.DrawString(
                "Адрес пункта выдачи: " + _order.PickupPoint.Address,
                fontRegular,
                XBrushes.Black,
                new XRect(0, margin, page.Width, page.Height),
                XStringFormats.Center
                );
            margin += 40;
            gfx.DrawString(
               "Код получения: " + _order.OrderGetCode.ToString(),
               fontBoldLarge,
               XBrushes.Black,
               new XRect(0, margin, page.Width, page.Height),
               XStringFormats.Center
               );


            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "talon.pdf");

            document.Save(path);

            var process = new Process();
            process.StartInfo = new ProcessStartInfo(path)
            {
                UseShellExecute = true
            };
            process.Start();

            MessageBox.Show("Сохранено в Документы!");
        }

        private void Button_GoToMain_Click(object sender, RoutedEventArgs e)
        {
            MainWindow1 mainWindow1 = new MainWindow1(_user);
            mainWindow1.Show();
            this.Close();
        }
    }
}
