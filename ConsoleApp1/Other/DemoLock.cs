using System;
using System.Threading;

namespace DemoLock
{
    #region 关键字Lock演示
    public class Account
    {
        int balance;
        Random r = new Random();

        public Account(int initial)
        {
            balance = initial;
        }

        //执行存取操作
        public int Withdraw(int amount)
        {
            if (balance < 0)
            {
                throw new Exception("Balance小于零");
            }

            //如果有进程在执行存取操作，则阻止其他进程执行
            lock (this)
            {
                Console.WriteLine("当前进程名" + Thread.CurrentThread.Name);

                if (balance >= amount)
                {
                    //睡眠5毫秒
                    Thread.Sleep(5);
                    balance -= amount;
                    return amount;
                }
                else
                {
                    return 0;
                }
            }
        }
        //多进程的入口函数
        public void DoTransactions()
        {
            for (int i = 0; i < 100; i++)
            {
                //随机存钱取钱
                Withdraw(r.Next(-50, 100));
            }
        }
    }
    /// <summary>
    /// 演示
    /// </summary>
    public class Demo
    {
        //开十个线程不断存取Balance,验证Lock
        static Thread[] threads = new Thread[10];
        public void Start()
        {
            Account account = new Account(0);

            for (int i = 0; i < 10; i++)
            {
                Thread t = new Thread(new ThreadStart(account.DoTransactions));
                threads[i] = t;
            }
            for (int i = 0; i < 10; i++)
            {
                threads[i].Name = "第" + i + "线程";
            }
            for (int i = 0; i < 10; i++)
            {
                threads[i].Start();
            }
            Console.ReadLine();
        }
    }
    #endregion

    #region 结构体Vector3
    public struct Vector3 : IEquatable<Vector3>, IFormattable
    {
        public float x;
        public float y;
        public float z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    default:
                        throw new Exception("下标越界,Vector3的索引为0-2");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    default:
                        throw new Exception("下标越界,Vector3的索引为0-2");
                }
            }
        }
        public static Vector3 one { get { return new Vector3(1, 1, 1); } }
        public static Vector3 zero { get { return new Vector3(0, 0, 0); } }
        public static Vector3 forward { get { return new Vector3(0, 0, 1); } }
        public static Vector3 backward { get { return new Vector3(0, 0, -1); } }

        [Obsolete("请使用forward")]
        public static Vector3 fwd { get { return new Vector3(0, 0, 1); } }
        public bool Equals(Vector3 other)
        {
            if (other.x == x && other.y == y && other.z == z)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return "(" + x.ToString() + "," + y.ToString() + "," + z.ToString() + ")";
        }
        public static Vector3 operator *(float d, Vector3 a)
        {
            a.x *= d;
            a.y *= d;
            a.z *= d;

            return a;
        }
        public static Vector3 operator *(Vector3 a, float d)
        {
            a.x *= d;
            a.y *= d;
            a.z *= d;

            return a;
        }
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            a.x -= b.x;
            a.y -= b.y;
            a.z -= b.z;

            return a;
        }
    }
    #endregion
}
