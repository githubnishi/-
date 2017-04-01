using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;

namespace Databace
{
    public class Databace
    {
        private static SQLiteConnection connect = new SQLiteConnection();

        /// <summary>
        /// 接続文字列のセット
        /// </summary>
        /// <param name="path"></param>
        public void ConnectionOpen(string path)
        {
            connect.ConnectionString = "Data Source=" + path;
            connect.Open();
            SQLiteCommand command = connect.CreateCommand();
            command.CommandText = "CREATE TEMPORARY TABLE temp(ID integer primary key AUTOINCREMENT, language text, target text, translate text, syori integer)";
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// データベースを閉じる
        /// </summary>
        public void Close()
        {
            if (connect.State == System.Data.ConnectionState.Open)
                connect.Close();
        }

        /// <summary>
        /// テーブルを返す
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public DataTable GetTable(string table)
        {
            DataTable data = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter("SELECT * FROM " + table, connect);
            adapter.Fill(data);

            return data;
        }

        /// <summary>
        /// データベースとの照合
        /// </summary>
        /// <param name="arg">検索文字</param>
        /// <returns></returns>
        public static string Serch(string table, string arg)
        {
            arg = arg.Replace("\t", "");
            string work = "";
            int id_work = 0;
            SQLiteCommand command = connect.CreateCommand();

            command.CommandText = "SELECT * FROM " + table + " WHERE target = '" + arg + "'";

            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    id_work = Int32.Parse(reader[4].ToString());
                    switch (id_work)
                    {
                        case 1:
                            work += reader[3].ToString();
                            break;
                        case 2:
                            work += reader[3].ToString();
                            break;
                        case 3:
                            work += reader[2].ToString();
                            break;
                        case 4:
                        case 5:
                        case 6:
                            work = reader[3].ToString();
                            break;
                    }
                    id_work = Int32.Parse(reader[4].ToString());
                }
            }

            command.CommandText = "SELECT * FROM syori WHERE id = '" + id_work + "'";
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    switch (id_work)
                    {
                        case 1:
                            work += reader[1].ToString();
                            break;
                        case 2:
                            work += reader[1].ToString();
                            break;
                        case 3:
                            break;
                        case 4:
                        case 5:
                        case 6:
                            break;
                    }
                }
            }
            if (table == "C" && work == "")
                return Serch("temp", arg);
            else
                return work;
        }

        /// <summary>
        /// 自作関数をテンポラリテーブルに追加
        /// </summary>
        /// <param name="name"></param>
        /// <param name="target"></param>
        /// <param name="translate"></param>
        public static void Insert_temp(string target, string translate)
        {
            SQLiteCommand command = connect.CreateCommand();
            command.CommandText = @"INSERT OR REPLACE INTO temp(language,target,translate,syori) VALUES (@language,@target,@translate,@syori)";

            // パラメータのセット
            command.Parameters.Add("language", System.Data.DbType.String);
            command.Parameters.Add("target", System.Data.DbType.String);
            command.Parameters.Add("translate", System.Data.DbType.String);
            command.Parameters.Add("syori", System.Data.DbType.Int32);

            // データの追加
            command.Parameters["language"].Value = "C";
            command.Parameters["target"].Value = target;
            command.Parameters["translate"].Value = translate;
            command.Parameters["syori"].Value = 4;
            command.ExecuteNonQuery();
        }
    }
}
