using System;
using System.Collections.Generic;
using System.Text;

namespace Algorithm
{
    public static class MyMath
    {

        //冒泡排序
        public static void Sort(int[] x)
        {
            for (int i = 0; i < x.Length; i++)
            {
                for (int j = 0; j < x.Length - i - 1; j++)
                {
                    if (x[j] > x[j + 1])
                    {
                        int temp = x[j];
                        x[j] = x[j + 1];
                        x[j + 1] = temp;
                    }
                }
            }
        }

        //反转数组
        public static void Reverse(int[] x)
        {
            for (int i = 0; i < x.Length / 2; i++)
            {
                int temp = x[i];
                x[i] = x[x.Length - i - 1];
                x[x.Length - i - 1] = temp;
            }
        }

        //翻转字符串(递归)
        public static string Reverse(string x)
        {
            if (x.Length == 1)
            {
                return x;
            }
            return x[x.Length - 1] + Reverse(x.Substring(0, x.Length - 1));
        }
    }

    public class Algorithm
    {
        //递归斐波那契数列
        public static int Fbnq(int x)
        {
            if (x < 2)
            {
                if (x == 0)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }

            return Fbnq(x - 1) + Fbnq(x - 2);

            //简化版
            //return x < 2 ? x == 0 ? 0 : 1 : DG(x - 1) + DG(x - 2);
        }

        //递推斐波那契数列
        public static int Fbnq(int a, int b, int c, int d)
        {
            if (c == d)
            {
                return a + b;
            }
            return Fbnq(b, a + b, c + 1, d);

        }
        //循环斐波那契数列
        public static int FbnqX(int x)
        {
            int a = 1;
            int b = 1;
            for (int i = 3; i <= x; i++)
            {
                int temp = a + b;
                a = b;
                b = temp;
            }
            return b;
        }


        //阶乘
        public static int JC(int x)
        {
            if (x <= 0)
            {
                return 0;
            }
            else if (x == 1)
            {
                return 1;
            }

            return JC(x - 1) * x;
        }

        //阶乘Pro
        public static long JC(long x)
        {
            if (x <= 0)
            {
                return 0;
            }
            else if (x == 1)
            {
                return 1;
            }

            return JC(x - 1) * x;
        }


        //递推绳子(绳子截取n段，任意三段不为三角形，求最绳子小长度)
        public static int SZ(int x)
        {
            //长，宽，当前求的段数，总段数，当前的绳子长
            return SZ(1, 1, 3, x, 2);
        }

        public static int SZ(int a, int b, int c, int d, int e)
        {
            if (c == d)
            {
                return e + a + b;
            }
            return SZ(b, a + b, c + 1, d, e + a + b);
        }

        //循环绳子
        public static int SZX(int x)
        {
            int a = 1;
            int b = 1;
            int c = 2;
            for (int i = 3; i <= x; i++)
            {
                int temp = a + b;
                a = b;
                b = temp;
                c += b;
            }
            return c;
        }


        //分治上台阶
        static int count = 0;
        public static void TJF(int n)
        {
            if (n == 0)
            {
                count++;
                return;
            }
            TJF(n - 1);
            if (n > 1)
            {
                TJF(n - 2);
            }
        }

        //递归上台阶
        public static int TJD(int n)
        {
            if (n == 0)
            {
                return 1;
            }
            if (n == 1)
            {
                return 1;
            }
            return TJD(n - 1) + TJD(n - 2);
        }

        //动态规划上台阶
        public static int TJDG(int n, int[] jie)
        {
            if (n < 1)
            {
                return 0;
            }
            if (n == 1)
            {
                return 1;
            }
            if (n == 2)
            {
                return 2;
            }
            if (jie[n - 1] > 0)
            {
                return jie[n - 1];
            }

            int temp = TJDG(n - 1, jie) + TJDG(n - 2, jie);
            if (jie[n - 1] == 0)
            {
                jie[n - 1] = temp;
            }
            return temp;
        }

        //动态规划上台阶循环解
        public static int F(int n)
        {
            if (n < 1)
            {
                return 0;
            }
            if (n == 1)
            {
                return 1;
            }
            if (n == 2)
            {
                return 2;
            }

            int a = 1;
            int b = 2;
            int temp = 0;
            for (int i = 3; i < n + 1; i++)
            {
                temp = a + b;
                a = b;
                b = temp;
            }
            return temp;
        }

        //分治汉诺塔
        public static void HNT(string x, string y, string z, int n)
        {
            //当前柱，目标柱，中转柱，当前数量
            if (n == 1)
            {
                Console.WriteLine(x + "->" + y);
                return;
            }
            HNT(x, z, y, n - 1);
            HNT(x, y, z, 1);
            HNT(z, y, x, n - 1);
        }


        //分支限界电路板加载数据及调用
        public static void DLB()
        {
            /*
            7
            7
            3
            2
            4
            6
            14
            1
            3
            2
            3
            2
            4
            3
            5
            4
            4
            4
            5
            5
            1
            5
            5
            6
            1
            6
            2
            6
            3
            7
            1
            7
            2
            7
            3
             */

            int x = int.Parse(Console.ReadLine());
            int y = int.Parse(Console.ReadLine());
            int[,] map = new int[x, y];

            Point a = new Point
            {
                x = int.Parse(Console.ReadLine()) - 1,
                y = int.Parse(Console.ReadLine()) - 1
            };
            map[a.x, a.y] = -1;

            Point b = new Point
            {
                x = int.Parse(Console.ReadLine()) - 1,
                y = int.Parse(Console.ReadLine()) - 1
            };

            int non = int.Parse(Console.ReadLine());
            for (int i = 0; i < non; i++)
            {
                int m = int.Parse(Console.ReadLine()) - 1;
                int n = int.Parse(Console.ReadLine()) - 1;
                map[m, n] = -1;
            }
            LinkedList<Point> list = new LinkedList<Point>();
            list.AddLast(a);

            //当前要搜素的点(初始时只有a)，当前层数，目标点，电路板地图
            int temp = DLB(list, 1, b, map);
            Console.WriteLine(temp);
        }

        //分支限界所用结构体
        public struct Point
        {
            public int x;
            public int y;
            public Point(int a, int b)
            {
                x = a;
                y = b;
            }
        }
        //分支界限电路板
        public static int DLB(LinkedList<Point> list, int n, Point b, int[,] map)
        {
            LinkedList<Point> nlist = new LinkedList<Point>();
            foreach (var p in list)
            {
                if (p.y > 0)
                {
                    if (p.x == b.x && p.y - 1 == b.y)
                    {
                        return n;
                    }
                    if (map[p.x, p.y - 1] == 0)
                    {
                        nlist.AddLast(new Point(p.x, p.y - 1));
                    }
                }
                if (p.x < map.GetLength(0) - 1)
                {
                    if (p.x + 1 == b.x && p.y == b.y)
                    {
                        return n;
                    }
                    if (map[p.x + 1, p.y] == 0)
                    {
                        nlist.AddLast(new Point(p.x + 1, p.y));
                    }
                }
                if (p.y < map.GetLength(1) - 1)
                {
                    if (p.x == b.x && p.y + 1 == b.y)
                    {
                        return n;
                    }
                    if (map[p.x, p.y + 1] == 0)
                    {
                        nlist.AddLast(new Point(p.x, p.y + 1));
                    }
                }
                if (p.x > 0)
                {
                    if (p.x - 1 == b.x && p.y == b.y)
                    {
                        return n;
                    }
                    if (map[p.x - 1, p.y] == 0)
                    {
                        nlist.AddLast(new Point(p.x - 1, p.y));
                    }
                }
            }
            return DLB(nlist, n + 1, b, map);
        }

        //回溯背包加载及调用
        public static void HSBB()
        {
            /*
            5
            100
            50
            5
            200
            25
            180
            30
            225
            45
            200
            50
            */
            int n = int.Parse(Console.ReadLine());
            int m = int.Parse(Console.ReadLine());
            int[] jz = new int[n];
            int[] zl = new int[n];
            for (int i = 0; i < jz.Length; i++)
            {
                jz[i] = int.Parse(Console.ReadLine());
                zl[i] = int.Parse(Console.ReadLine());
            }
            //总价值，总重量，当前个数，总个数，背包容量，价值数组，重量数组
            Console.WriteLine(HSBB(0, 0, 1, n, m, jz, zl));
        }

        //回溯背包问题
        public static int HSBB(int zj, int zz, int ds, int zs, int bz, int[] jz, int[] zl)
        {
            if (ds == zs)
            {
                if (zz + zl[ds - 1] <= bz)
                {
                    return zj + jz[ds - 1];
                }
                return zj;
            }
            int x = HSBB(zj, zz, ds + 1, zs, bz, jz, zl);
            if (zz + zl[ds - 1] <= bz)
            {
                int y = HSBB(zj + jz[ds - 1], zz + zl[ds - 1], ds + 1, zs, bz, jz, zl);
                if (y > x)
                {
                    return y;
                }
            }
            return x;
        }


        //贪心集装箱
        public static void TXJZX()
        {
            int n = int.Parse(Console.ReadLine());
            int m = int.Parse(Console.ReadLine());
            int[] jz = new int[n];
            int[] zl = new int[n];
            for (int i = 0; i < jz.Length; i++)
            {
                jz[i] = int.Parse(Console.ReadLine());
                zl[i] = int.Parse(Console.ReadLine());
            }

            //冒泡排序
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (jz[j] / (double)zl[j] > jz[j + 1] / (double)zl[j + 1])
                    {
                        int temp = jz[j];
                        jz[j] = jz[j + 1];
                        jz[j + 1] = temp;

                        temp = zl[j];
                        zl[j] = zl[j + 1];
                        zl[j + 1] = temp;
                    }
                }
            }

            //装包
            double zjz = 0;
            int zzl = 0;
            for (int i = 0; i < n; i++)
            {
                if (zzl + zl[i] <= m)
                {
                    zjz += jz[i];
                    zzl += zl[i];
                    Console.WriteLine(jz[i] + " " + zl[i]);
                }
                else
                {
                    zjz += (m - zzl) * (jz[i] / (double)zl[i]);
                    Console.WriteLine("拆包" + " " + jz[i] + " " + zl[i]);
                    break;
                }
            }
            Console.WriteLine(zjz);
        }


        //背包问题
        public static void BB()
        {
            int a = 0;
            string b = "";
            for (int i1 = 0; i1 <= 1; i1++)
            {
                for (int i2 = 0; i2 <= 1; i2++)
                {
                    for (int i3 = 0; i3 <= 1; i3++)
                    {
                        for (int i4 = 0; i4 <= 1; i4++)
                        {
                            for (int i5 = 0; i5 <= 1; i5++)
                            {
                                int y = i1 * 5 + i2 * 25 + i3 * 30 + i4 * 45 + i5 * 50;
                                if (y > 100)
                                {
                                    continue;
                                }
                                int x = i1 * 50 + i2 * 200 + i3 * 180 + i4 * 225 + i5 * 200;
                                if (x > a)
                                {
                                    a = x;
                                    b = (i1 > 0 ? "1" : "") + (i2 > 0 ? "2" : "") + (i3 > 0 ? "3" : "") + (i4 > 0 ? "4" : "")
                                        + (i5 > 0 ? "5" : "");
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine(a);
            Console.WriteLine(b);
        }
    }
}
