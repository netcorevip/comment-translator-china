namespace Framework
{
    using System;
    using System.Linq;

    /// <summary>
    /// 计算谷歌翻译TK值
    /// </summary>
    public static class GetTKHelper
    {
        /*
        C# 谷歌翻译TKK计算tk 百度翻译gtk计算sign 通用算法 - Mr_Cc - CSDN博客
            https://blog.csdn.net/u013070165/article/details/85096834
*/






        /// <summary>
        /// 实现js的charAt方法
        /// </summary>
        /// <param name="obj">The obj<see cref="object"/></param>
        /// <param name="index">The index<see cref="int"/></param>
        /// <returns>The <see cref="char"/></returns>
        public static char charAt(this object obj, int index)
        {
            char[] chars = obj.ToString().ToCharArray();
            return chars[index];
        }


        /// <summary>
        /// 实现js的charCodeAt方法
        /// </summary>
        /// <param name="obj">The obj<see cref="object"/></param>
        /// <param name="index">The index<see cref="int"/></param>
        /// <returns>The <see cref="int"/></returns>
        public static int charCodeAt(this object obj, int index)
        {
            char[] chars = obj.ToString().ToCharArray();
            return (int)chars[index];
        }


        /// <summary>
        /// 实现js的Number方法
        /// </summary>
        /// <param name="cc">The cc<see cref="object"/></param>
        /// <returns>The <see cref="int"/></returns>
        public static int Number(object cc)
        {
            try
            {
                long a = Convert.ToInt64(cc.ToString());
                int b = a > 2147483647 ? (int)(a - 4294967296) : a < -2147483647 ? (int)(a + 4294967296) : (int)a;
                return b;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// The b
        /// </summary>
        /// <param name="a">The a<see cref="long"/></param>
        /// <param name="b">The b<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string b(long a, string b)
        {
            for (int d = 0; d < b.Length - 2; d += 3)
            {
                char c = b.charAt(d + 2);
                int c0 = 'a' <= c ? c.charCodeAt(0) - 87 : GetTKHelper.Number(c);
                long c1 = '+' == b.charAt(d + 1) ? a >> c0 : a << c0;
                a = '+' == b.charAt(d) ? a + c1 & 4294967295 : a ^ c1;
            }
            a = GetTKHelper.Number(a);
            return a.ToString();
        }

        /// <summary>
        /// 计算TK值
        /// </summary>
        /// <param name="a">待翻译字符串<see cref="string"/></param>
        /// <param name="TKK">谷歌返回TKK<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string GetTK(this string a, string TKK)
        {
            string[] e = TKK.Split('.');
            int d = 0;
            int h = 0;
            int[] g = new int[a.Length * 3];
            h = GetTKHelper.Number(e[0]);
            for (int f = 0; f < a.Length; f++)
            {
                int c = a.charCodeAt(f);
                if (128 > c)
                {
                    g[d++] = c;
                }
                else
                {
                    if (2048 > c)
                    {
                        g[d++] = c >> 6 | 192;
                    }
                    else
                    {
                        if (55296 == (c & 64512) && f + 1 < a.Length && 56320 == (a.charCodeAt(f + 1) & 64512))
                        {
                            c = 65536 + ((c & 1023) << 10) + a.charCodeAt(++f) & 1023;
                            g[d++] = c >> 18 | 240;
                            g[d++] = c >> 12 & 63 | 128;
                        }
                        else
                        {
                            g[d++] = c >> 12 | 224;
                            g[d++] = c >> 6 & 63 | 128;
                            g[d++] = c & 63 | 128;
                        }
                    }
                }
            }
            int[] g0 = g.Where(x => x != 0).ToArray();
            long aa = h;
            for (d = 0; d < g0.Length; d++)
            {
                aa += g0[d];
                aa = Convert.ToInt64(b(aa, "+-a^+6"));
            }
            aa = Convert.ToInt64(b(aa, "+-3^+b+-f"));
            long bb = aa ^ GetTKHelper.Number(e[1]);
            aa = bb;
            aa = aa + bb;
            bb = aa - bb;
            aa = aa - bb;
            if (0 > aa)
            {
                aa = (aa & 2147483647) + 2147483648;
            }
            aa %= (long)1e6;
            return aa.ToString() + "." + (aa ^ h);
        }
    }
}
