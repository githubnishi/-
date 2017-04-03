using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin_interface;

namespace Analyse_C
{
    public class Analyse : Plugin
    {
        static string table = "C";
        //区切り文字
        private static string[] kugiri = { " ", ";", "" };

        /// <summary>
        /// 文字列を指定文字で区切る
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static string hoge(string arg)
        {
            coment_delete(arg);
            space_insert(arg);
            int[] data = new int[3];
            string[] stArrayData = arg.Split(kugiri, StringSplitOptions.RemoveEmptyEntries);
            Array.Resize(ref data, stArrayData.Length);
            if (data.Length == 0)
                //return "";
            for (int first_roop = 0; first_roop < data.Length; first_roop++)
            {
                data[first_roop] = hogee(stArrayData[first_roop]);
            }

            switch (data[0])
            {
                //変数宣言 関数宣言 returnとか
                case 0:
                    if(data.Length == 1)
                    {
                        //returnかbreak
                    }
                    switch(data[1])
                    {
                        //式の場合
                        case 1:
                        case 3:
                            for (int second_roop = 2; second_roop <= data.Length; second_roop++)
                            {
                                if (data[second_roop] == 2)
                                    return Databace.Databace.Serch(table, surplus_delete(stArrayData[second_roop])) +  "して" + stArrayData[0] + "に代入";
                                    //関数
                                if (data[second_roop] == 4)
                                    return Databace.Databace.Serch(table, stArrayData[second_roop - 1]) + "して" + stArrayData[0] + "に代入";
                                ;
                            }
                            return stArrayData[0] + "に代入";

                        
                    }
                    break;

                //関数の使用(確定)
                case 2:
                    return Databace.Databace.Serch(table, surplus_delete(stArrayData[0])) + "する関数の使用";

                //ポインタの中身変更(確定)
                case 3:
                    return "ポインタの中身を操作";

                //構文エラー説
                default:
                    return "";
            }

            return "";
        }

        /// <summary>
        /// 文字列に含まれているキーワードから番号を返す
        /// </summary>
        /// <param name="arg">文字列</param>
        /// <returns>
        /// 0=一致なし
        /// 1=計算式
        /// 2=関数
        /// 3=*
        /// 4=先頭に(
        /// </returns>
        public static int hogee(string arg)
        {
            string work = "";
            int type_work = 0;
            foreach (char c in arg)
            {
                work += c;

                switch (c)
                {

                    case '(':
                        //2文字目以降
                        if (work == "")
                            type_work = 4;
                        else
                            type_work = 2;
                        break;

                    case '*':
                        type_work = 3;
                        break;

                    case '+':
                    case '-':
                    case '/':
                    case '%':
                    case '=':
                        type_work = 1;
                        break;

                    default:
                        break;
                }
            }
            return type_work;
        }


        /// <summary>
        /// ///*コメントアウト*/や"文字列"を削除する
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static string coment_delete(string arg)
        {
            int delete_front = 0, delete_back = 0;

            // " ～ "を削除
            delete_front = arg.IndexOf("\"");
            delete_back = arg.IndexOf("\"", delete_front + 1);
            if (delete_front != -1)
            {
                if (delete_back != -1)
                    arg = arg.Remove(delete_front, 0);
                arg = arg.Remove(delete_front, (delete_back - delete_front));
            }

            // /* ～ */を削除
            delete_front = arg.IndexOf("/*");
            delete_back = arg.IndexOf("*/");
            if (delete_front != -1)
            {
                if (delete_back != -1)
                    arg = arg.Remove(delete_front, 0);
                arg = arg.Remove(delete_front, (delete_back - delete_front));
            }          

            // //のあとをすべて削除
            delete_front = arg.IndexOf("//");
            if (delete_front != -1)
            {
                arg = arg.Remove(delete_front, 0);
            }
            return arg;
        }

        /// <summary>
        /// 判別用に+-*/とかの前後にスペース挿入
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static string space_insert(string arg)
        {
            arg = arg.Replace("+", " + ");
            arg = arg.Replace("-", " - ");
            arg = arg.Replace("*", " * ");
            arg = arg.Replace("/", " / ");
            arg = arg.Replace("%", " % ");
            arg = arg.Replace("=", " = ");
            return arg;
        }

        /// <summary>
        /// ()や;を取り除いて文字列だけにする
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static string surplus_delete(string arg)
        {
            arg = arg.Remove(arg.IndexOf("("), 0);
            arg = arg.Remove(arg.IndexOf(";"), 0);

            return arg;
        }
    }
}
