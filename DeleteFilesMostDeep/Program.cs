using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DeleteFilesMostDeep
{
    class Program
    {
        static void Main()
        {
            var json = File.ReadAllText("config.json"); // ファイル内容をjson変数に格納
            // Deserialize
            var obj = JsonSerializer.Deserialize<Setting>(json);

            List<string> result = obj.Extend
                .Split(',')
                .ToList()
                .ConvertAll(a => a);

            foreach (var s in result)
            {
                string[] stFilePathes = GetFilesMostDeep(Regex.Escape(obj.Path), s);
                string stPrompt = string.Empty;

                // 取得したファイル名を列挙する
                foreach (string stFilePath in stFilePathes)
                {
                    stPrompt += stFilePath + Environment.NewLine;
                    File.Delete(stFilePath);
                }

                // 取得したすべてのファイルパスを表示する
                if (stPrompt != string.Empty)
                {
                    var logger = NLog.LogManager.GetCurrentClassLogger();
                    logger.Info(stPrompt + Environment.NewLine);

                }
            }
        }
        /// ---------------------------------------------------------------------------------------
        /// <summary>
        ///     指定した検索パターンに一致するファイルを最下層まで検索しすべて返します。</summary>
        /// <param name="stRootPath">
        ///     検索を開始する最上層のディレクトリへのパス。</param>
        /// <param name="stPattern">
        ///     パス内のファイル名と対応させる検索文字列。</param>
        /// <returns>
        ///     検索パターンに一致したすべてのファイルパス。</returns>
        /// ---------------------------------------------------------------------------------------
        public static string[] GetFilesMostDeep(string stRootPath, string stPattern)
        {
            System.Collections.Specialized.StringCollection hStringCollection = (
                new System.Collections.Specialized.StringCollection()
            );

            // このディレクトリ内のすべてのファイルを検索する
            foreach (string stFilePath in Directory.GetFiles(stRootPath, stPattern))
            {
                hStringCollection.Add(stFilePath);
            }

            // このディレクトリ内のすべてのサブディレクトリを検索する (再帰)
            foreach (string stDirPath in Directory.GetDirectories(stRootPath))
            {
                string[] stFilePaths = GetFilesMostDeep(stDirPath, stPattern);

                // 条件に合致したファイルがあった場合は、ArrayList に加える
                if (stFilePaths != null)
                {
                    hStringCollection.AddRange(stFilePaths);
                }
            }

            // StringCollection を 1 次元の String 配列にして返す
            string[] stReturns = new string[hStringCollection.Count];
            hStringCollection.CopyTo(stReturns, 0);

            return stReturns;
        }
    }
    public class Setting
    {
        public string Path { get; set; }
        public string Extend { get; set; }
    }
}
