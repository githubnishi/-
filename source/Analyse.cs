using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analyse_C
{
    public class Analyse
    {
        static string table = "C";
        static string formula_work = "";
        /// <summary>
        /// 文字列から何の処理かを返す
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static string hantei(string arg)
        {
            int space_flg = 0, skip_count = 0, point_flg = 0,return_flg = 0;
            int delete_front = 0, delete_back = 0;
            string work = "";
            delete_front = arg.IndexOf("/*");
            delete_back = arg.IndexOf("*/");
            if (delete_front != -1)
            {
                if (delete_back != -1)
                    arg = arg.Remove(delete_front, 0);
                arg = arg.Remove(delete_front, (delete_back - delete_front));
            }
            arg = space_insert(arg);
            foreach (char c in arg)
            {
                switch (c)
                {
                    //関数
                    case '(':
                        if (Databace.Databace.Serch(table,work) == "")
                            return work + "関数の呼び出し";
                        if (arg.Substring((work.Length + skip_count), 1) != "(")
                        {
                            return Databace.Databace.Serch(table, work) + "する関数の使用";
                        }
                        else
                        {
                            return Databace.Databace.Serch(table, work);
                        }


                    //計算
                    case '*':
                    case '+':
                    case '-':
                    case '/':
                    case '%':
                        skip_count++;
                        point_flg = 1;
                        break;
                    case '=':
                        skip_count++;
                        //if (space_flg == 1)
                        //{
                            formula_work = work;
                            return kansuu(arg.Substring(work.Length + skip_count)) + formula_work + "に代入";
                        //}

                        break;

                    //構造体フラグ
                    /*case '.':
                        type_flg = 1;
                        break;*/

                    //空白
                    case ' ':
                        skip_count++;
                        if (work != "")
                            space_flg = 1;
                        continue;

                    //終端
                    case ';':
                        skip_count++;
                        if (Databace.Databace.Serch(table, work) == "")
                            return "";
                        return Databace.Databace.Serch(table, work);

                    case '[':
                        if(space_flg == 0)
                        {
                            if (work == "")
                                return "";
                            return "配列" + work + "に代入";
                        }
                        break;
                    //その他
                    default:
                        if(point_flg == 1)
                        {
                            if (Databace.Databace.Serch(table, work) == "")
                                return "";
                            return Databace.Databace.Serch(table, work).Replace("変数","ポインタ");
                        }

                        if (space_flg == 1)
                        {
                            if (work == "return")
                                return_flg = 1;
                            if (return_flg == 1)
                            {
                                if (function_check(arg.Substring(work.Length)) == 0)
                                {
                                    if (Databace.Databace.Serch(table, work) == "")
                                        return "";
                                    return "関数を呼んで戻る";
                                }
                            }
                            else
                            {
                                if (function_check(arg.Substring(work.Length)) == 0)
                                {
                                    if (Databace.Databace.Serch(table, work) == "")
                                        return "";
                                    //Databace.Databace.Insert_temp(, work);
                                    return "戻り値が" + Databace.Databace.Serch(table, work).Replace("変数", "関数");
                                }
                            }
                            if (Databace.Databace.Serch(table, work) == "")
                                return "";
                            return Databace.Databace.Serch(table, work);
                        }

                        switch (c)
                        {
                            case '{':
                            case '}':
                                skip_count++;
                                break;

                            case ')':
                            case ',':
                                return "";

                            default:
                                work += c;
                                break;
                        }
                        break;
                }
            }

            return "";
        }

        /// <summary>
        /// 文字列の最初に来る関数を判定
        /// </summary>
        /// <param name="args">文字列</param>
        /// <returns></returns>
        private static string kansuu(string arg)
        {
            string work = "";
            foreach (char c in arg)
            {
                switch (c)
                {
                    //関数
                    case '(':
                        if (work != "")
                        {
                            if (Databace.Databace.Serch(table, work) == "")
                                return "関数を使用して";
                            return Databace.Databace.Serch(table, work) + "して";
                        }
                        return "";


                    //空白
                    case ' ':
                        continue;

                    //その他
                    default:
                        switch (c)
                        {
                            case '{':
                            case '}':
                                break;

                            case ')':
                            case ',':
                                return "";

                            default:
                                work += c;
                                break;
                        }
                        break;
                }
            }

            return "";
        }

        /// <summary>
        /// 関数かチェックする時用
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private static int function_check(string arg)
        {
            foreach (char c in arg)
            {
                switch (c)
                {
                    case ';':
                    case '*':
                    case '+':
                    case '-':
                    case '/':
                    case '%':
                    case '=':
                    case '[':
                        return -1;

                    case '(':
                        return 0;

                }
            }
            return -1;
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
            arg = arg.Replace("<", " < ");
            arg = arg.Replace(">", " > ");
            arg = arg.Replace("=", " = ");
            return arg;
        }
    }
}
