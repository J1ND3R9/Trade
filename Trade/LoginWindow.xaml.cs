using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

namespace Trade
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public static TradeContext Trade = new();

        bool guestMode = true;
        bool isCaptcha = false;
        string CurrentCaptcha = "";
        Random r = new Random();
        char[] captchaSymbols = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        bool isBlocked = false;

        public LoginWindow()
        {
            InitializeComponent();
        }

        public void GenerateCaptcha()
        {
            CurrentCaptcha = "";
            CaptchaCanvas.Children.Clear();
            List<UIElement> listElements = new();
            for (int i = 0; i < 5; i++)
            {

                int randomSymbol = r.Next(0, captchaSymbols.Length);
                TextBlock tb = new()
                {
                    Text = captchaSymbols[randomSymbol].ToString(),
                    FontSize = 18,
                    Foreground = Brushes.Black,
                    TextDecorations = TextDecorations.Strikethrough
                };
                double left = 0;
                double top = r.Next(0, (int)CaptchaCanvas.ActualHeight - 30);
                if (listElements.Count == 0)
                    left = r.Next(0, 60);
                else
                {
                    int previousElement = (int)Canvas.GetLeft(listElements[i - 1]);
                    left = r.Next(previousElement, previousElement + 60);
                }
                Canvas.SetLeft(tb, left);
                Canvas.SetTop(tb, top);
                CurrentCaptcha += tb.Text;
                listElements.Add(tb);
                CaptchaCanvas.Children.Add(tb);
            }
        }

        private async void Login(object sender, RoutedEventArgs e)
        {
            if (isBlocked) return;
            
            isBlocked = true;

            try
            {
                string login = loginBox.Text;
                string password = passwordBox.Password;
                User account = Trade.Users.First(s => s.UserPassword == password && s.UserLogin == login);
                string captcha = "";
                if (isCaptcha)
                {
                    captcha = captchaBox.Text;
                }

                if (account == null)
                {
                    passwordBox.Clear();
                    gridCaptcha.Visibility = Visibility.Visible;
                    MessageBox.Show("Неверный логин или пароль!");
                    GenerateCaptcha();

                    if (isCaptcha)
                        await Block();

                    isCaptcha = true;
                    return;
                }
                bool conditionCaptcha = captcha == CurrentCaptcha;
                if (!conditionCaptcha) // Не прошел капчу
                {
                    MessageBox.Show("CAPTCHA не пройдена!");
                    captchaBox.Clear();
                    captchaBox.Focus();
                    GenerateCaptcha();
                    await Block();
                }
                ProductWindow window = new ProductWindow();
                window.AdminStatus.Text = account.UserRole > 2 ? "True" : "False";
                window.Show();
                this.Close();
            }
            finally
            {
                isBlocked = false;
            }
        }

        private async Task Block()
        {
            LoginButton.IsEnabled = false;
            await Task.Delay(10000);
            LoginButton.IsEnabled = true;
        }
    }
}
