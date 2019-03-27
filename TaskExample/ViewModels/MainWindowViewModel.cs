using Prism.Mvvm;
using Prism.Commands;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace TaskExample.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Task & Lock";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel()
        {

        }

        Object lockObj = new Object();

        private bool cancel1 = false;
        public bool Cancel1
        {
            get { return cancel1; }
            set { SetProperty(ref cancel1, value); }
        }

        private bool cancel2 = false;
        public bool Cancel2
        {
            get { return cancel2; }
            set { SetProperty(ref cancel2, value); }
        }

        private bool task1run = false;
        public bool Task1run
        {
            get { return task1run; }
            set { SetProperty(ref task1run, value);
                CommandTask1.RaiseCanExecuteChanged();
                CommandTask1Cancel.RaiseCanExecuteChanged();
            }
        }

        private bool task2run = false;
        public bool Task2run
        {
            get { return task2run; }
            set { SetProperty(ref task2run, value);
                CommandTask2.RaiseCanExecuteChanged();
                CommandTask2Cancel.RaiseCanExecuteChanged();
            }
        }

        private int tValue = 30;
        /// <summary>
        /// プログレスバー表示用
        /// </summary>
        public int TValue
        {
            get { return tValue; }
            set { SetProperty(ref tValue, value); }
        }

        public void Task1()
        {
            lock (lockObj)
            {
                while (TValue < 100)
                {
                    //ダミー負荷用ウエイトms スレッドを止める
                    Thread.Sleep(30);
                    //状況の報告
                    TValue++;
                    //キャンセルリクエストの確認
                    if (Cancel1)
                    {
                        break;
                    }
                }
            }
            Cancel1 = false;
        }

        public void Task2()
        {
            lock (lockObj)
            {
                while (TValue > 0)
                {
                    //ダミー負荷用ウエイトms スレッドを止める
                    Thread.Sleep(30);
                    //状況の報告
                    TValue--;
                    //キャンセルリクエストの確認
                    if (Cancel2)
                    {
                        break;
                    }
                }
            }
            Cancel2 = false;
        }

        #region CommandTask1
        private DelegateCommand commandTask1;
        public DelegateCommand CommandTask1 =>
            commandTask1 ?? (commandTask1 = new DelegateCommand(ExecuteCommandTask1, CanExecuteCommandTask1));

        async void ExecuteCommandTask1()
        {
            Task1run = true;
            await Task.Run(() => Task1());
            Task1run = false;
        }

        bool CanExecuteCommandTask1()
        {
            return !Task1run;
        }
        #endregion

        #region CommandTask2
        private DelegateCommand commandTask2;
        public DelegateCommand CommandTask2 =>
            commandTask2 ?? (commandTask2 = new DelegateCommand(ExecuteCommandTask2, CanExecuteCommandTask2));

        async void ExecuteCommandTask2()
        {
            Task2run = true;
            await Task.Run(() => Task2());
            Task2run = false;
        }

        bool CanExecuteCommandTask2()
        {
            return !Task2run;
        }
        #endregion

        #region CommandTask1Cancel
        private DelegateCommand commandTask1Cancel;
        public DelegateCommand CommandTask1Cancel =>
            commandTask1Cancel ?? (commandTask1Cancel = new DelegateCommand(ExecuteCommandTask1Cancel, CanExecuteCommandTask1Cancel));

        void ExecuteCommandTask1Cancel()
        {
            Cancel1 = true;
        }

        bool CanExecuteCommandTask1Cancel()
        {
            return Task1run;
        }
        #endregion

        #region CommandTask2Cancel
        private DelegateCommand commandTask2Cancel;
        public DelegateCommand CommandTask2Cancel =>
            commandTask2Cancel ?? (commandTask2Cancel = new DelegateCommand(ExecuteCommandTask2Cancel, CanExecuteCommandTask2Cancel));

        void ExecuteCommandTask2Cancel()
        {
            Cancel2 = true;
        }

        bool CanExecuteCommandTask2Cancel()
        {
            return Task2run;
        }
        #endregion
    }
}
