using ACS.SPiiPlusNET;
using System;
using System.Diagnostics;
using System.Windows;

namespace ACSTimeUse
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Api acs;
        int m_axis = 4;//控制的轴号
        int m_Dis = 4;//用于控制PTP步进距离
        public MainWindow()
        {
            InitializeComponent();
            acs = new Api();

        }

        private void SetAcc_Button_Click(object sender, RoutedEventArgs e)
        {
            double[] AccArray = new double[100];
            ACC_Box.Text = "";
            for (int i = 0; i < 100; i++)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();
                if (UP_mode.IsChecked==true)
                {
                    stopwatch.Start(); //  开始监视代码运行时间
                    acs.SetAcceleration((Axis)m_axis, 100);                   //  需要测试的代码 ....
                    stopwatch.Stop(); //  停止监视
                }
                else if (Down_mode.IsChecked==true)
                {
                    stopwatch.Start(); //  开始监视代码运行时间
                    acs.ReadVariable("I0", ProgramBuffer.ACSC_NONE, i, i); //读
                    //acs.WriteVariable(i, "M_ACC",ProgramBuffer.ACSC_NONE,i,i);//写
                    stopwatch.Stop(); //  停止监视
                }
                TimeSpan timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间
                double milliseconds =Convert.ToDouble(timespan.Ticks) / 10000;  //  总毫秒数
                AccArray[i] = milliseconds;
                ACC_Box.Text += $"[{DateTime.Now}] ACC:{milliseconds}\n";
            }
            Array.Sort(AccArray);
            ACC_Calc_Box.Text = $"MinTime:{AccArray[0]}    MaxTime:{AccArray[99]}   MedianTime:{AccArray[50]} ";
        }

        private void SetJerk_Button_Click(object sender, RoutedEventArgs e)
        {
            double[] JerkArray = new double[100];
            Jerk_Box.Text = "";

            for (int i = 0; i < 100; i++)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();


                if (UP_mode.IsChecked == true)
                {
                    stopwatch.Start(); //  开始监视代码运行时间
                    acs.SetJerk((Axis)m_axis, 1000);                   //  需要测试的代码 ....
                    stopwatch.Stop(); //  停止监视
                }
                else if (Down_mode.IsChecked == true)
                {
                    stopwatch.Start(); //  开始监视代码运行时间
                    acs.WriteVariable(i, "M_Jerk", ProgramBuffer.ACSC_NONE, i, i);
                    stopwatch.Stop(); //  停止监视
                }
                TimeSpan timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间
                double milliseconds = Convert.ToDouble(timespan.Ticks) / 10000;  //  总毫秒数
                JerkArray[i] = milliseconds;
                Jerk_Box.Text += $"[{DateTime.Now}] Jerk:{milliseconds}\n";
            }
            Array.Sort(JerkArray);
            Jerk_Calc_Box.Text = $"MinTime:{JerkArray[0]}    MaxTime:{JerkArray[99]}   MedianTime:{JerkArray[50]} ";
        }

        private void PTP_Button_Click(object sender, RoutedEventArgs e)
        {
            double[] PTPArray = new double[100];
            PTP_Box.Text = "";

            acs.Enable((Axis)m_axis);
            for (int i = 0; i < 50; i++)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();


                if (UP_mode.IsChecked == true)
                {
                    stopwatch.Start(); //  开始监视代码运行时间
                    acs.ToPoint(MotionFlags.ACSC_AMF_RELATIVE, (Axis)m_axis, m_Dis);                   //  需要测试的代码 ....
                    stopwatch.Stop(); //  停止监视
                }
                else if (Down_mode.IsChecked == true)
                {
                    stopwatch.Start(); //  开始监视代码运行时间
                    acs.WriteVariable(i, "M_Point", ProgramBuffer.ACSC_NONE);
                    stopwatch.Stop(); //  停止监视
                }
                TimeSpan timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间
                double milliseconds = Convert.ToDouble(timespan.Ticks) / 10000;  //  总毫秒数
                PTPArray[i] = milliseconds;

                PTP_Box.Text += $"[{DateTime.Now}] PTP:{milliseconds}\n";
                acs.WaitMotionEnd((Axis)m_axis,-1);
            }
            for (int i = 50; i < 100; i++)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();
                if (UP_mode.IsChecked == true)
                {
                    stopwatch.Start(); //  开始监视代码运行时间
                    acs.ToPoint(MotionFlags.ACSC_AMF_RELATIVE, (Axis)m_axis, m_Dis);                   //  需要测试的代码 ....
                    stopwatch.Stop(); //  停止监视
                }
                else if (Down_mode.IsChecked == true)
                {
                    stopwatch.Start(); //  开始监视代码运行时间
                    acs.WriteVariable(i, "M_Point", ProgramBuffer.ACSC_NONE);
                    stopwatch.Stop(); //  停止监视
                }
                TimeSpan timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间
                double milliseconds = Convert.ToDouble(timespan.Ticks) / 10000;  //  总毫秒数
                PTPArray[i] = milliseconds;

                PTP_Box.Text += $"[{DateTime.Now}] PTP:{milliseconds}\n";
                acs.WaitMotionEnd((Axis)m_axis, -1);
            }
            Array.Sort(PTPArray);
            PTP_Calc_Box.Text = $"MinTime:{PTPArray[0]}     MaxTime:{PTPArray[99]}   MedianTime:{PTPArray[50]} ";
        }

        private void Connect_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                acs.OpenCommEthernetTCP("10.0.0.100", 701);
                ConnectState();
            }
            catch (ACSException)
            {

            }
        }

        private void SimConnect_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                acs.OpenCommSimulator();
                ConnectState();
            }
            catch (ACSException)
            {

            }
        }
        private void ConnectState()
        {
            if (acs.IsConnected)
            {
                State_led.Background= System.Windows.Media.Brushes.Green;
            }
            else
            {
                State_led.Background = System.Windows.Media.Brushes.Gray;

            }

        }
    }
}
