using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using unvell.ReoGrid;
using unvell.ReoGrid.IO;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Diagnostics;

namespace Test1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            StartThread();

            InitScottPlot();
            InitReoGrid();

            LoadPara();
        }

        ~MainWindow() 
        {
            m_bEndThreadTest = true;
            DispatcherHelper.DoEvents();
            Thread.Sleep(3000);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void CartesianChart_Loaded(object sender, RoutedEventArgs e)
        {

        }


        private void InitScottPlot()
        {
            //var plt = new ScottPlot.Plot(600, 400);
            var plt = plot.Plot;
            plt.Clear();

            // 模拟真实数据 参考值为10，公差为1
            Random rand = new(0);
            double[] heights = ScottPlot.DataGen.RandomNormal(rand, pointCount: 1000, mean: 10, stdDev: 0.2);
            // 最大值和最小值在参考值附近，包括了公差范围
            double mini = 9;
            double maxi = 11;
            int bin = 8;

            ScottPlot.Statistics.Histogram hist = new(min: mini, max: maxi, binCount: bin);
            hist.AddRange(heights);
            double[] probabilities = hist.GetProbability();

            var bar = plt.AddBar(values: probabilities, positions: hist.Bins);

            // 标签位置，和最大值、最小值有关
            double[] positions = new double[bin];
            for (int i = 0; i < bin; i++)
            {
                positions[i] = (maxi - mini) * i / (bin - 1) + mini;
            }
            string[] labels = { "-1010", "1", "2", "-0.1", "0.1", "5", "6", "1010" };


            plt.XTicks(positions, labels);

            // 柱状图宽度和最大值、最小值、份数有关
            bar.BarWidth = (maxi - mini) / bin;
            bar.FillColor = ColorTranslator.FromHtml("#9bc3eb");
            bar.BorderColor = ColorTranslator.FromHtml("#82add9");

            plt.AddFunction(hist.GetProbabilityCurve(heights, true), System.Drawing.Color.Black, 2, ScottPlot.LineStyle.Dash);
            plt.SetAxisLimits(yMin: 0, yMax: 0.5);

            plt.XAxis.AxisTicks.MajorGridVisible = false;
            plt.YAxis.AxisTicks.MajorGridVisible = false;
            plt.XAxis.AxisTicks.MinorTickVisible = false;
            plt.XAxis.AxisTicks.MajorTickVisible = false;

            var vLine = plt.AddVerticalLine(9.2);
            vLine.DragEnabled = true;
            vLine.DragLimitMin = 9;
            vLine.DragLimitMax = 10;

            //plt.SaveFig("stats_histogramProbability.png");
            plot.Refresh();
        }

        private void InitReoGrid()
        {
            // get current active worksheet instance
            var sheet = grid.CurrentWorksheet;
           
            // set cells data
            sheet["A1"] = "hello world";
            sheet[2, 1] = 10;

            sheet.SetCellData(new CellPosition(4, 1), "hello world");

            sheet.AddOutline(RowOrColumn.Column, 5, 2).Collapse();
            sheet.FreezeToCell(2, 7, FreezeArea.LeftTop);


            var dataRange = sheet.Ranges["A21:F35"];

            dataRange.Data = new object[,]
            {
                {"[23423423]", "Product ABC", 15, 150},
                {"[45645645]", "Product DEF", 1, 75},
                {"[78978978]", "Product GHI", 2, 30},
            };

            // set subtotal formula
            sheet.Cells["G21"].Formula = "E21*F21";

            // auto fill other subtotals
            sheet.AutoFillSerial("G21", "G22:G35");

            LoadData();
        }
        /// <summary>
        /// 载入数据
        /// </summary>
        private void LoadData()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var workbook = grid;
                try
                {
                    string path2 = "doc/表格1.xlsx";// "C:\\Users\\qiuzexin\\Desktop\\G#Git#20230331\\1\\GitKu001\\Test1\\Test1\\doc\\表格1.xlsx";
                    using (Stream stream = Application.GetResourceStream(new Uri(path2, UriKind.Relative)).Stream)
                    {
                        workbook.Load(stream, FileFormat.Excel2007);
                    }
                }
                catch (Exception ex)
                {
                    //_VM.ShowInfo($" 载入用户权限表异常：{ex}");
                }
            }));
        }


        string m_sMsgShow = "产生数据：";
        public bool m_bEndThreadTest = false;
        // 初始化线程/////////////////////////////////////////////////////////////////////////////////
        public void StartThread()
        {
            m_bEndThreadTest = false;
            // IsBackground=true是后台线程，进程结束自动结束，false是前台线程，前台线程不结束，主线程会一直暂停不退出进程
            Thread cabService = new Thread(new ThreadStart(ThreadTest)) { IsBackground = true };   
            cabService.Start();
        }

        public void ThreadTest()
        {
            //DateTime tmCurrent = DateTime.Now; ;
            while (true)
            {
                if (m_bEndThreadTest)
                {
                    break;
                }


                int x = new Random((int)(System.DateTime.Now.Second)).Next(3, 50);
                m_sMsgShow = "产生数据：" + x;

                Debug.WriteLine(m_sMsgShow);
                Console.WriteLine(m_sMsgShow);
                //MessageBox.Show(sText);
                UpdateMsgShow();

                DispatcherHelper.DoEvents();    // 实时响应，但会降低效率
                Thread.Sleep(1000);
            }
        }

        // 事件委托
        public void UpdateMsgShow()
        {
            //Action MsgShowAction = new Action(() => 
            //{
            //    MsgShow.Text = m_sMsgShow;
            //});
            Dispatcher.Invoke(new Action(() =>
            {
                MsgShow.Text = m_sMsgShow;
            }));
        }




        ///十字路口2/////////////////////////////////////////////////////////////////////////////////////////////////
        int m_nMode = 1;
        SolidColorBrush brushGreen = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 128, 0));
        SolidColorBrush brushRed = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 0));
        SolidColorBrush brushYellow = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 180, 66));
        bool m_bEndThreadGoLeft = false;
        bool m_bEndThreadGoRight = false;
        bool m_bEndThreadGoTop = false;
        bool m_bEndThreadGoBottom = false;
        public static INI INI = new();
        public ConnectSQLite m_sqlCross;

        void LoadPara()
        {
            string sValue = INI.IniReadValue("十字路口配置", "模式", "1");
            m_nMode = int.Parse(sValue);

            buttonLeft1.Background = brushGreen;
            buttonLeft2.Background = brushGreen;
            buttonLeft3.Background = brushGreen;

            buttonRight1.Background = brushGreen;
            buttonRight2.Background = brushGreen;
            buttonRight3.Background = brushGreen;

            buttonUp1.Background = brushRed;
            buttonUp2.Background = brushRed;
            buttonUp3.Background = brushGreen;

            buttonDown1.Background = brushRed;
            buttonDown2.Background = brushRed;
            buttonDown3.Background = brushGreen;

            InitPara();
        }
        void SavePara()
        {
            INI.IniWriteValue("十字路口配置", "模式", m_nMode.ToString());
        }

        public void InitPara()
        {
            m_sqlCross = new ConnectSQLite("CrossDataBase.sqlite");


            m_bEndThreadGoLeft = false;
            Thread thGoLeft = new Thread(new ThreadStart(ThreadGoLeft));
            thGoLeft.IsBackground = true;
            thGoLeft.Start();

            m_bEndThreadGoRight = false;
            Thread thGoRight = new Thread(new ThreadStart(ThreadGoRight));
            thGoRight.IsBackground = true;
            thGoRight.Start();

            m_bEndThreadGoRight = false;
            Thread thGoUp = new Thread(new ThreadStart(ThreadGoUp)) { IsBackground = true };
            thGoUp.Start();

            m_bEndThreadGoRight = false;
            Thread thGoDown = new Thread(new ThreadStart(ThreadGoDown)) { IsBackground = true };
            thGoDown.Start();
            
        }

        public void Rule1(Button btLeft, Button btStraight, string sFlag="左")
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                DateTime tmCurrent = DateTime.Now;

                SolidColorBrush brushTmp;//= new();
                string sMsg = sFlag;
                if (btLeft.Background.Equals(brushGreen))
                {
                    brushTmp = brushRed;
                    sMsg = sFlag + "绿变红";
                }
                else
                {
                    brushTmp = brushGreen;
                    sMsg = sFlag + "红变绿";
                }
                btLeft.Background = brushYellow;
                btStraight.Background = brushYellow;
                //DispatcherHelper.DoEvents();
                int nAdd = 10;
                for (int i = 0; tmCurrent.AddMilliseconds(1000) > DateTime.Now; i += nAdd)
                {
                    DispatcherHelper.DoEvents();
                    Thread.Sleep(nAdd);
                }

                btLeft.Background = brushTmp;
                btStraight.Background = brushTmp;

                m_sqlCross.AddTableData(tmCurrent.ToString(), sMsg);
            }));
        }

        public void ThreadGoLeft()
        {
            DateTime now = DateTime.Now;
            double dSeconds = now.TimeOfDay.TotalSeconds;
            double milliseconds = now.TimeOfDay.TotalMilliseconds;
            while (true)
            {
                if(m_bEndThreadGoLeft)
                {
                    break;
                }

                DateTime now2 = DateTime.Now;
                double dSeconds2 = now2.TimeOfDay.TotalSeconds;
                int nDiff = (int)(dSeconds2 - dSeconds);
                int nFlag = nDiff % 5;
                if (nDiff > 5)
                {
                    dSeconds = dSeconds2;
                    // 委托：刷新控件
                    Rule1(buttonLeft1, buttonLeft2);
                }

                DispatcherHelper.DoEvents();
                Thread.Sleep(10);
            }
        }

        public void ThreadGoRight()
        {
            DateTime now = DateTime.Now;
            double dSeconds = now.TimeOfDay.TotalSeconds;
            while (true)
            {
                if (m_bEndThreadGoRight)
                {
                    break;
                }

                DateTime now2 = DateTime.Now;
                double dSeconds2 = now2.TimeOfDay.TotalSeconds;
                int nDiff = (int)(dSeconds2 - dSeconds);
                int nFlag = nDiff % 5;
                if (nDiff > 5)
                {
                    dSeconds = dSeconds2;
                    Rule1(buttonRight1, buttonRight2, "右");
                }

                DispatcherHelper.DoEvents();
                Thread.Sleep(10);
            }
        }
        public void ThreadGoUp()
        {
            DateTime now = DateTime.Now;
            double dSeconds = now.TimeOfDay.TotalSeconds;
            while (true)
            {
                if (m_bEndThreadGoRight)
                {
                    break;
                }

                DateTime now2 = DateTime.Now;
                double dSeconds2 = now2.TimeOfDay.TotalSeconds;
                int nDiff = (int)(dSeconds2 - dSeconds);
                int nFlag = nDiff % 5;
                if (nDiff > 5)
                {
                    dSeconds = dSeconds2;
                    Rule1(buttonUp1, buttonUp2, "前");
                }

                DispatcherHelper.DoEvents();
                Thread.Sleep(10);
            }
        }
        public void ThreadGoDown()
        {
            DateTime now = DateTime.Now;
            double dSeconds = now.TimeOfDay.TotalSeconds;
            while (true)
            {
                if (m_bEndThreadGoRight)
                {
                    break;
                }

                DateTime now2 = DateTime.Now;
                double dSeconds2 = now2.TimeOfDay.TotalSeconds;
                int nDiff = (int)(dSeconds2 - dSeconds);
                int nFlag = nDiff % 5;
                if (nDiff > 5)
                {
                    dSeconds = dSeconds2;
                    Rule1(buttonDown1, buttonDown2, "后");
                }

                DispatcherHelper.DoEvents();
                Thread.Sleep(10);
            }
        }

        private void ChangeMode(object sender, EventArgs eventArgs)
        {

        }
    }


}
