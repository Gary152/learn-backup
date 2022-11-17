using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            #region 获取程序目录

            /*
            //1.获取程序的基目录
            var t1=AppDomain.CurrentDomain.BaseDirectory;
            MessageBox.Show(t1);
            //2.获取模块的完整路径
            var t2 = Process.GetCurrentProcess().MainModule.FileName;
            MessageBox.Show(t2);
            //3.获取和设置当前目录(该进程从中启动的目录)的完全限定目录
            var t3 = Environment.CurrentDirectory;
            MessageBox.Show(t3);
            //4.获取应用程序的当前工作目录
            var t4 = Directory.GetCurrentDirectory();
            MessageBox.Show(t4);
            //5.获取和设置包括该应用程序的目录的名称
            var t5 = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            MessageBox.Show(t5);
            */

            #endregion

            //重启应用程序
            //Process.Start(Assembly.GetExecutingAssembly().Location);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "文本文件|*.txt|图片文件|*.png|所有文件|*.*";

            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowDialog();
            var path = folderBrowserDialog.SelectedPath;

            Task.Run(() =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                var ps = Search(path, "*");
                if (ps != null && ps.Length > 0)
                {
                    foreach (var item in ps)
                    {
                        listBox1.Items.Add(item);
                    }
                }
                else
                {
                    MessageBox.Show("搜索结果为空！");
                }
                MessageBox.Show("搜索结束！");
            });
        }

        /// <summary>
        /// 递归遍历
        /// </summary>
        /// <param name="path">搜索目标</param>
        /// <param name="filter">过滤器</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string[] Search(string path, string filter)
        {
            if (Directory.Exists(path))
            {
                LinkedList<string> list = new LinkedList<string>();
                var ds = Directory.GetDirectories(path);
                foreach (var i in ds)
                {
                    var temp = Search(i, filter);
                    foreach (var item in temp)
                    {
                        list.AddLast(item);
                    }
                }

                var fs = Directory.GetFiles(path);
                foreach (var item in fs)
                {
                    string extName = Path.GetExtension(item);
                    var filters = filter.Split('|');
                    foreach (var f in filters)
                    {
                        if (extName == f)
                        {
                            list.AddLast(item);
                            break;
                        }
                    }
                }

                return list.ToArray();
            }
            else
            {
                throw new Exception("在进行搜索时,发现指定的地址不存在");
            }
        }
    }

    public class MyList<T>
    {
        private T[] ts = new T[10];
        private int length = 0;

        public T this[int index]
        {
            get { return ts[index]; }
            set { ts[index] = value; }
        }

        public int Length { get => length; private set => length = value; }

        public void Add(T t)
        {
            ts[Length] = t;
            Length++;

            if (Length == ts.Length)
            {
                T[] temp = new T[Length];
                for (int i = 0; i < Length; i++)
                {
                    temp[i] = ts[i];
                }
                ts = new T[Length * 2];

                for (int i = 0; i < Length; i++)
                {
                    ts[i] = temp[i];
                }
            }
        }
    }
}
