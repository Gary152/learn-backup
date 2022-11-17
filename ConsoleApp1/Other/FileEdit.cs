using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Data.OleDb;

namespace FileEdit
{
    /// <summary>
    /// 对FileEdit各个模块的整合与管理
    /// </summary>
    public static class FileEditManager
    {
        public struct CmdOperate
        {

        }
        public struct SQLiteHelper
        {

        }
        public struct ExcelHelper
        {

        }
        public struct TxtHelper
        {

        }
    }

    #region 对命令提示符进行操作

    /// <summary>
    /// 对命令提示符进行操作
    /// </summary>
    public class CmdOperate
    {
        /// <summary>
        /// 执行cmd命令
        /// </summary>
        /// <param name="cmdtxt">命令行参数;如：shutdown /s</param>
        /// <returns>返回cmd行数据</returns>
        public static string RunCmd(string cmdtxt)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = "cmd";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                p.StandardInput.WriteLine(cmdtxt);
                p.StandardInput.WriteLine("exit");
                string result = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                p.Close();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    #endregion

    #region 对SQLite数据库进行操作

    /// <summary>
    /// 对SQLite数据库进行操作
    /// </summary>
    public class SQLiteHelper
    {
        private static SQLiteConnection conn;

        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="dbName">数据库的文件名</param>
        /// <param name="tableSql">初始表的字段配置,如: UserInfo (ID int, Name varchar(20))</param>
        public static void CrateDB(string dbName, string tableSql)
        {
            try
            {
                string path = Directory.GetCurrentDirectory() + @"\DataBase";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                conn = new SQLiteConnection($@"Data Source={path + @"\" + dbName}.db;Version=3;");

                string sql = $"create table if not exists {tableSql}";
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 打开现有数据库
        /// </summary>
        /// <param name="filePath">数据库文件路径</param>
        public static void OpenDB(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    conn = new SQLiteConnection($@"Data Source={filePath};Version=3;");
                }
                else
                {
                    throw new Exception("无法打开数据库,指定的数据库文件不存在！");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 修改表数据
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public static int Edit(string sql)
        {
            try
            {
                conn.Open();

                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                int result = cmd.ExecuteNonQuery();

                conn.Close();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 查询表数据
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public static DataTable Select(string sql)
        {
            try
            {
                conn.Open();

                DataTable dt = new DataTable();
                new SQLiteDataAdapter(sql, conn).Fill(dt);

                conn.Close();

                return dt;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }

    #endregion

    #region 对Excel表格进行操作

    /// <summary>
    /// 对Excel表进行操作
    /// </summary>
    public class ExcelHelper
    {
        private static OleDbConnection conn;

        /// <summary>
        /// 创建Excel表
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="tableSql">初始表的字段配置,如: UserInfo (ID int, Name varchar(20))</param>
        [Obsolete("该方法因缺少ISAM,暂无法使用", true)]
        public static void Crate(string fileName, string tableSql)
        {
            try
            {
                string path = Directory.GetCurrentDirectory() + @"\ExcelFile";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                conn = new OleDbConnection($@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={path + @"\" + fileName}.xls;HDR=Yes;IMEX=2;");

                string sql = $"create table if not exists {tableSql}";
                conn.Open();
                OleDbCommand cmd = new OleDbCommand(sql, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 打开现有Excel表
        /// </summary>
        /// <param name="filePath">文件位置</param>
        public static void Open(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string fileType = Path.GetExtension(filePath).ToLower();
                    if (fileType == ".xls")
                    {
                        conn = new OleDbConnection($@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={filePath};Extended Properties=Excel 8.0;HDR=Yes;IMEX=2");
                    }
                    else if (fileType == ".xlsx")
                    {
                        //conn = new OleDbConnection($@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties=Excel 12.0;HDR=Yes;IMEX=2");
                        throw new Exception("无法读取.xlsx文件,请另存为.xls");
                    }
                    else
                    {
                        throw new Exception("文件类型错误,指定的文件不是Excel文件");
                    }
                }
                else
                {
                    throw new Exception("无法打开数据库,指定的数据库文件不存在！");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 修改Excel表数据
        /// </summary>
        /// <param name="sql">sql语句。提示:此处表名的写法为[Sheet1$]</param>
        /// <returns></returns>
        public static int Edit(string sql)
        {
            try
            {
                conn.Open();

                OleDbCommand cmd = new OleDbCommand(sql, conn);
                int result = cmd.ExecuteNonQuery();

                conn.Close();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 查询Excel表数据
        /// </summary>
        /// <param name="sql">sql语句。提示:此处表名的写法为[Sheet1$]</param>
        /// <returns></returns>
        public static DataTable Select(string sql)
        {
            try
            {
                DataTable dt = new DataTable();
                new OleDbDataAdapter(sql, conn).Fill(dt);
                conn.Close();
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    #endregion

    #region 文件流对txt进行操作

    public class TxtHelper
    {
        /// <summary>
        /// 读取指定的txt文件
        /// </summary>
        /// <param name="path">文件位置</param>
        /// <returns></returns>
        public static string Read(string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read);
                    StreamReader sr = new StreamReader(fs);
                    string txt = sr.ReadToEnd();
                    sr.Close();
                    fs.Close();

                    return txt;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 将指定内容写入txt文件
        /// </summary>
        /// <param name="path">文件位置</param>
        /// <param name="content">要写入的内容</param>
        /// <returns></returns>
        public static bool Write(string path, string content)
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(content))
                {
                    FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);

                    sw.Write(content);
                    sw.Close();
                    fs.Close();

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 将指定内容追加写入txt文件
        /// </summary>
        /// <param name="path">文件位置</param>
        /// <param name="content">要写入的内容</param>
        /// <returns></returns>
        public static bool AppendWrite(string path, string content)
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(content))
                {
                    StreamWriter sw = new StreamWriter(path, true);
                    sw.WriteLine(content);
                    sw.Close();

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }

    #endregion
}
