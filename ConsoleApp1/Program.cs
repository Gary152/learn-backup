using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Other.FileEdit;

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
            var a = CmdOperate.RunCmd();
            Console.WriteLine(a);
        }

        #region MyRegion

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
    }
}