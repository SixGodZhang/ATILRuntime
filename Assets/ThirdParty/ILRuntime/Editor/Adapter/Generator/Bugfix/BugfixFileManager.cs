//-----------------------------------------------------------------------
// <filename>BugfixFileManager</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> ## </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/22 星期六 12:16:39# </time>
//-----------------------------------------------------------------------

using CodeGenerationTools.Generator.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace GameFramework.Taurus
{
    public class BugfixFileManager
    {
        private readonly Regex _regex = new Regex("\\{\\$(?:[a-z][a-z0-9_]*)\\}", RegexOptions.IgnoreCase | RegexOptions.Singleline);// 

        private string Template;

        private string GenTmpad;
        private HashSet<string> KeyWordList = new HashSet<string>();
        private Dictionary<string, object> KeyDictionary = new Dictionary<string, object>();

        /// <summary>
        /// 模板路径
        /// </summary>
        private string _filePath;

        public BugfixFileManager(string templatePath)
        {
            _filePath = templatePath;

            LoadTemplateFromFile(_filePath);
        }

        public bool LoadTemplate(string template)
        {
            Template = template;

            KeyWordList.Clear();
            var m = _regex.Match(Template);
            while (m.Success)
            {
                if (!KeyWordList.Contains(m.Value))
                {
                    KeyWordList.Add(m.Value);
                }
                m = m.NextMatch();
            }

            return !string.IsNullOrEmpty(Template);
        }

        public bool LoadTemplateFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                return false;
            LoadTemplate(File.ReadAllText(filePath));
            return true;
        }

        public void SetKeyValue(string key, object content)
        {
            if (!KeyWordList.Contains(key))
            {
                Console.WriteLine("Invalid key word");
                return;
            }

            if (KeyDictionary.ContainsKey(key))
                KeyDictionary[key] = content;
            else
                KeyDictionary.Add(key, content);
        }

        private string GetContent(string key)
        {
            return KeyDictionary[key] as string;
        }

        private void Replace(string keyword, string content)
        {
            GenTmpad = GenTmpad.Replace(keyword, content);
        }

        public string Generate()
        {
            if (string.IsNullOrEmpty(Template))
            {
                Console.WriteLine("{0}'s Template  is null,please use LoadTemplate to init template", GetType().Name);
                return null;
            }

            GenTmpad = Template;

            foreach (var key in KeyWordList)
            {
                Replace(key, GetContent(key));
            }

            return GenTmpad;
        }
    }
}
