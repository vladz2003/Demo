using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DemoEx
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isAuthError = false;
        private int ticksLeft = 10;
        private DispatcherTimer _timer;
        private List<User> users;
        // функция входа 

        public MainWindow()
        {
            InitializeComponent();
            Trade123Entities db = new Trade123Entities();
            users = db.User.ToList();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var user = users.Find(u => u.UserLogin == TextBox_Login.Text && u.UserPassword == TextBox_Password.Text);

            if (isAuthError)
            {
                if (Text_Captcha.Text == TextBox_Captcha.Text && user != null)
                {
                }
                else
                {
                    MessageBox.Show("Вы не прошли проверку на робота или ввели неверный логин или пароль. Подождите 10 секунд и попробуйте снова");
                    Button_Autorization.IsEnabled = false;
                    Button_Guest.IsEnabled = false;
                    var timer = new DispatcherTimer();
                    timer.Tick += timer_Tick;
                    timer.Interval = TimeSpan.FromSeconds(1);
                    timer.Start();

                    _timer = timer;

                    Text_Captcha.Text = generateCaptcha(4);

                    return;
                }
            }

            if (user != null)
            {
                MessageBox.Show("Авторизация прошла успешно!");
                if (user.Role.RoleName == "Администратор")
                {
                    Administrator admin = new Administrator(user);
                    admin.Show();
                }
                else
                {
                    MainWindow1 wind = new MainWindow1(user);
                    wind.Show();
                }

                this.Close();
            }
            else
            {
                MessageBox.Show("Вы ввели неверный логин или пароль");
                Stack_Captcha.Visibility = Visibility.Visible;
                Text_Captcha.Text = generateCaptcha(4);

                isAuthError = true;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            ticksLeft--;
            if (ticksLeft == 0)
            {
                _timer.Stop();
                Button_Autorization.IsEnabled = true;
                Button_Guest.IsEnabled = true;
                ticksLeft = 10;
            }
        }

        private string generateCaptcha(int count)
        {
            string captcha = "";
            Random rand = new Random();

            for (int i = 0; i < count; i++)
            {
                switch (rand.Next(1, 3))
                {
                    case 1:
                        captcha += Convert.ToChar(rand.Next(65, 90));
                        break;
                    case 2:
                        captcha += Convert.ToChar(rand.Next(48, 57));
                        break;
                }
            }

            return captcha;
        }

        private void TextBox_TextChanged_1(object sender, object e)
        {

        }

        private void Button_Click(object sender, object e)
        {

        }

        private void Button_Guest_Click(object sender, RoutedEventArgs e)
        {
            User user = new User();
            user.UserName = "Гость";
            MainWindow1 wind = new MainWindow1(user);
            wind.Show();
            this.Close();
        }
    }
}
