﻿using System.Text.RegularExpressions;

namespace Mesurement.UiLayer.Utility
{
    /// 字符串校验
    /// <summary>
    /// 字符串校验
    /// </summary>
    public class StringCheck
    {
        //        匹配中文字符的正则表达式： [\u4e00-\u9fa5]
        public static bool IsChinese(string stringInput)
        {
            return (!string.IsNullOrEmpty(stringInput) && new Regex(@"[\u4e00-\u9fa5]").IsMatch(stringInput));
        }

        //匹配双字节字符(包括汉字在内)：[^\x00-\xff]
        public static bool IsByte(string stringInput)
        {
            return (!string.IsNullOrEmpty(stringInput) && new Regex(@"[^\x00-\xff]").IsMatch(stringInput));
        }

        //匹配空行的正则表达式：\n[\s| ]*\r

        //匹配HTML标记的正则表达式：/<(.*)>.*<\/\1>|<(.*) \/>/ 
        public static bool IsHtml(string stringInput)
        {
            return (!string.IsNullOrEmpty(stringInput) && new Regex(@"/<(.*)>.*<\/\1>|<(.*) \/>/ ").IsMatch(stringInput));
        }

        //匹配IP地址的正则表达式：/(\d+)\.(\d+)\.(\d+)\.(\d+)/g //
        public static bool IsIP(string stringInput)
        {
            return (!string.IsNullOrEmpty(stringInput) && new Regex(@"/(\d+)\.(\d+)\.(\d+)\.(\d+)/g //").IsMatch(stringInput));
        }

        //匹配Email地址的正则表达式：\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*
        public static bool IsEmail(string stringInput)
        {
            return (!string.IsNullOrEmpty(stringInput) && new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*").IsMatch(stringInput));
        }

        //匹配网址URL的正则表达式：http://(/[\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?
        public static bool IsUrl(string stringInput)
        {
            return (!string.IsNullOrEmpty(stringInput) && new Regex(@"http://(/[\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?").IsMatch(stringInput));
        }

        //sql语句：^(select|drop|delete|create|update|insert).*$
        public static bool IsSql(string stringInput)
        {
            return (!string.IsNullOrEmpty(stringInput) && new Regex(@"^(select|drop|delete|create|update|insert).*$").IsMatch(stringInput));
        }

        //1、非负整数：^\d+$ 
        public static bool IsInteger(string stringInput)
        {
            return (!string.IsNullOrEmpty(stringInput) && new Regex(@"^\d+$ ").IsMatch(stringInput));
        }

        //2、正整数：^[0-9]*[1-9][0-9]*$ 
        public static bool IsPlus(string stringInput)
        {
            return (!string.IsNullOrEmpty(stringInput) && new Regex(@"^[0-9]*[1-9][0-9]*$ ").IsMatch(stringInput));
        }

        //4、负整数：^-[0-9]*[1-9][0-9]*$ 
        public static bool IsMinus(string stringInput)
        {
            return (!string.IsNullOrEmpty(stringInput) && new Regex(@"^-[0-9]*[1-9][0-9]*$").IsMatch(stringInput));
        }

        //6、非负浮点数：^\d+(\.\d+)?$ 
        //7、正浮点数：^((0-9)+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$ 
        //8、非正浮点数：^((-\d+\.\d+)?)|(0+(\.0+)?))$ 
        //9、负浮点数：^(-((正浮点数正则式)))$ 
        //10、英文字符串：^[A-Za-z]+$ 
        //11、英文大写串：^[A-Z]+$ 
        //12、英文小写串：^[a-z]+$ 
        //13、英文字符数字串：^[A-Za-z0-9]+$ 
        //14、英数字加下划线串：^\w+$ 
        //15、E-mail地址：^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$ 
        //16、URL：^[a-zA-Z]+://(\w+(-\w+)*)(\.(\w+(-\w+)*))*(\?\s*)?$ 
        //或：^http:\/\/[A-Za-z0-9]+\.[A-Za-z0-9]+[\/=\?%\-&_~`@[\]\':+!]*([^<>\"\"])*$
        //17、邮政编码：^[1-9]\d{5}$
        //18、中文：^[\u0391-\uFFE5]+$
        //19、电话号码：^((\(\d{2,3}\))|(\d{3}\-))?(\(0\d{2,3}\)|0\d{2,3}-)?[1-9]\d{6,7}(\-\d{1,4})?$
        //20、手机号码：^((\(\d{2,3}\))|(\d{3}\-))?13\d{9}$
        //21、双字节字符(包括汉字在内)：^\x00-\xff
        //22、匹配首尾空格：(^\s*)|(\s*$)（像vbscript那样的trim函数）
        //23、匹配HTML标记：<(.*)>.*<\/\1>|<(.*) \/> 
        //24、匹配空行：\n[\s| ]*\r
        //25、提取信息中的网络链接：(h|H)(r|R)(e|E)(f|F) *= *('|")?(\w|\\|\/|\.)+('|"| *|>)?
        //26、提取信息中的邮件地址：\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*
        //27、提取信息中的图片链接：(s|S)(r|R)(c|C) *= *('|")?(\w|\\|\/|\.)+('|"| *|>)?
        //28、提取信息中的IP地址：(\d+)\.(\d+)\.(\d+)\.(\d+)
        //29、提取信息中的中国手机号码：(86)*0*13\d{9}
        //30、提取信息中的中国固定电话号码：(\(\d{3,4}\)|\d{3,4}-|\s)?\d{8}
        //31、提取信息中的中国电话号码（包括移动和固定电话）：(\(\d{3,4}\)|\d{3,4}-|\s)?\d{7,14}
        //32、提取信息中的中国邮政编码：[1-9]{1}(\d+){5}
        //33、提取信息中的浮点数（即小数）：(-?\d*)\.?\d+
        //34、提取信息中的任何数字 ：(-?\d*)(\.\d+)? 
        //35、IP：(\d+)\.(\d+)\.(\d+)\.(\d+)
        //36、电话区号：/^0\d{2,3}$/
        //37、腾讯QQ号：^[1-9]*[1-9][0-9]*$
        //38、帐号(字母开头，允许5-16字节，允许字母数字下划线)：^[a-zA-Z][a-zA-Z0-9_]{4,15}$
        //39、中文、英文、数字及下划线：^[\u4e00-\u9fa5_a-zA-Z0-9]+$ 
        public static bool IsFileName(string stringInput)
        {
            return (!string.IsNullOrEmpty(stringInput) && new Regex("^[\u4e00-\u9fa5_a-zA-Z0-9]+$").IsMatch(stringInput));
        }
    }
}
