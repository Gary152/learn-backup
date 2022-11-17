using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public static class Program
    {
        #region 数据结构

        /// <summary>
        /// 堆栈，先进后出
        /// </summary>
        public static Stack<object> stack = new Stack<object>();
        /// <summary>
        /// 队列，先进先出
        /// </summary>
        public static Queue<object> queue = new Queue<object>();

        #endregion

        public static void Main(string[] args)
        {

        }

        #region MyRegion

        /// <summary>
        /// 获取b站收藏夹
        /// </summary>
        public static void GetFavorite()
        {
            HttpClient client = new HttpClient(new HttpClientHandler() { UseCookies = false });
            client.DefaultRequestHeaders.Add("cookie", "SESSDATA=1930f099%2C1679970722%2C964e4%2A91;");
            Console.WriteLine(client.DefaultRequestHeaders.ToString());

            new Action(async () =>
            {
                string result = await client.GetStringAsync("https://api.bilibili.com/x/v3/fav/folder/list4navigate");

                Console.WriteLine(result);
            })();

            Console.ReadKey();
        }

        /// <summary>
        /// List数据结构
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class MyList<T> : IEnumerable<T> where T : class
        {
            private T[] dataList = new T[1];

            public int Count { get; private set; } = 0;
            public int Capacity
            {
                get => dataList.Length;
                set
                {
                    T[] temp = dataList;
                    dataList = new T[value];
                    temp.CopyTo(dataList, 0);
                }
            }

            public T this[int index]
            {
                get => dataList[index];
                set
                {
                    if (index >= Count)
                    {
                        throw new IndexOutOfRangeException("下标越界");
                    }

                    dataList[index] = value;
                }
            }
            private void Identity(int index)
            {
                if (index >= dataList.Length - 1)
                {
                    T[] temp = dataList;
                    dataList = new T[temp.Length * 2];
                    temp.CopyTo(dataList, 0);
                }
            }

            public void Add(T item)
            {
                Identity(Count);

                dataList[Count] = item;
                Count++;
            }

            public void Clear()
            {
                dataList = new T[Capacity];
                Count = 0;
            }
            public IEnumerator GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                {
                    yield return dataList[i];
                }
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                {
                    yield return dataList[i];
                }
            }
        }

        #endregion

        #region other

        /// <summary>
        /// 多任务，读取指定URL的字符数
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns></returns>
        public static async void GetLength(string url)
        {
            string result = await Task.Run(async () =>
            {
                Thread.Sleep(3000);
                HttpClient http = new HttpClient();
                http.DefaultRequestHeaders.Add("", "");
                string str = await http.GetStringAsync(url);

                return str;
            });

            Console.WriteLine(result);
        }

        /// <summary>
        /// 指针
        /// </summary>
        public static unsafe void Pointer()
        {
            unsafe
            {
                int[] a = new int[] { 1, 2, 3, 4, 5, 6 };
                fixed (int* iptr = a)
                {
                    Console.WriteLine(iptr->ToString());
                    Console.WriteLine((int)iptr);
                    Console.WriteLine((int)(iptr + 1));
                    Console.WriteLine((iptr + 1)->ToString());
                }
            }
        }

        /// <summary>
        /// Main函数参数args的使用
        /// </summary>
        /// <param name="args"></param>
        public static void UseArgs(string[] args)
        {
            if (args.Length < 1 || args[0] == "-help" || args[0] == "-?")
            {
                Console.WriteLine("有关某个命令的详细信息，请键入 HELP 命令名");
                Console.WriteLine("-? -help      输出此帮助消息");
                return;
            }

            Console.WriteLine("打印参数:");
            foreach (string i in args)
            {
                Console.WriteLine(i);
            }
        }
        #endregion
    }
}