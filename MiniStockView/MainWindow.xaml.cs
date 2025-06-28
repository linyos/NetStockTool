using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MiniStockView.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MiniStockView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // 設定ViewModel將在App.xaml.cs中進行
        }

        /// <summary>
        /// 設定ViewModel
        /// </summary>
        public void SetViewModel(MainViewModel viewModel)
        {
            DataContext = viewModel;
        }

        /// <summary>
        /// 窗口拖拽移動
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        /// <summary>
        /// 窗口載入完成
        /// </summary>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // 設定窗口位置到右上角
            var workingArea = SystemParameters.WorkArea;
            Left = workingArea.Right - Width - 50;
            Top = workingArea.Top + 50;
        }
    }
}