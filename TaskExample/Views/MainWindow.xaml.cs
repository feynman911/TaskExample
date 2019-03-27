using System;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;

namespace TaskExample.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            task1_cancel_button.IsEnabled = false;
            task2_cancel_button.IsEnabled = false;
        }

        private CancellationTokenSource cancelTokensource1;//キャンセル判定用
        private CancellationTokenSource cancelTokensource2;//キャンセル判定用
        Object lockObj = new Object();
        int percent = 0;

        //Task実行用にIProgressとCancellationTokent追加
        //複数変数戻したい時には独自クラスを定義
        public bool Task1(IProgress<int> p, CancellationToken cancelToken)
        {
            bool ret = true;
            lock(lockObj)
            {
                while (percent < 100)
                {
                    //ダミー負荷用ウエイトms スレッドを止める
                    Thread.Sleep(30);
                    //状況の報告
                    percent++;
                    p.Report(percent);
                    //キャンセルリクエストの確認
                    if (cancelToken.IsCancellationRequested)
                    {
                        ret = false;
                        break;
                    }
                }
            }
            return ret;
        }

        public bool Task2(IProgress<int> p, CancellationToken cancelToken)
        {
            bool ret = true;
            lock(lockObj)
            {
                while (percent > 0)
                {
                    //ダミー負荷用ウエイトms スレッドを止める
                    Thread.Sleep(30);
                    //状況の報告
                    percent--;
                    p.Report(percent);
                    //キャンセルリクエストの確認
                    if (cancelToken.IsCancellationRequested)
                    {
                        ret = false;
                        break;
                    }
                }
            }
            return ret;
        }

        //textbox1 & ProgressBar1 の更新
        public void SetText(int percent)
        {
            textBox1.Text = percent.ToString();
            progressBar1.Value = percent;
        }

        private void Task1_cancel_button_Click(object sender, RoutedEventArgs e)
        {
            if (cancelTokensource1 != null) cancelTokensource1.Cancel();
        }

        private void Task2_cancel_button_Click(object sender, RoutedEventArgs e)
        {
            if (cancelTokensource2 != null) cancelTokensource2.Cancel();
        }

        private async void Task1_button_Click(object sender, RoutedEventArgs e)
        {
            task1_button.IsEnabled = false;
            task1_cancel_button.IsEnabled = true;
            var p = new Progress<int>(SetText);
            cancelTokensource1 = new CancellationTokenSource();
            var cToken = cancelTokensource1.Token;
            bool ret = await Task.Run(() => Task1(p, cToken));
            task1_button.IsEnabled = true;
            task1_cancel_button.IsEnabled = false;
            cancelTokensource1.Dispose();
            cancelTokensource1 = null;
        }

        private async void Task2_button_Click(object sender, RoutedEventArgs e)
        {
            task2_button.IsEnabled = false;
            task2_cancel_button.IsEnabled = true;
            var p = new Progress<int>(SetText);
            cancelTokensource2 = new CancellationTokenSource();
            var cToken = cancelTokensource2.Token;
            bool ret = await Task.Run(() => Task2(p, cToken));
            task2_button.IsEnabled = true;
            task2_cancel_button.IsEnabled = false;
            cancelTokensource2.Dispose();
            cancelTokensource2 = null;
        }

    }
}
