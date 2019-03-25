using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Framework
{
    public abstract class ApiClient
    {

        protected virtual async Task<IAPIResponse> Execute(IApiRequest request)
        {

            CookieContainer cookie = new CookieContainer();
            string text = Encoding.UTF8.GetString(request.Body);
            string tkk = request.TKK;    
            string fromLanguage = request.Headers["from-language"].ToString();
            string toLanguage = request.Headers["to-language"].ToString();
            string referer = "https://translate.google.cn/";
            var html = "";
            var tk = GetTKHelper.GetTK(text, tkk);
            StringBuilder url = new StringBuilder();
            url.Append("https://translate.google.cn/translate_a/single?client=webapp");
            url.AppendFormat("&sl={0}", fromLanguage);
            url.AppendFormat("&tl={0}", toLanguage);
            url.Append("&hl=zh-CN&dt=at&dt=bd&dt=ex&dt=ld&dt=md&dt=qca&dt=rw&dt=rm&dt=ss&dt=t&otf=1&ssel=0&tsel=0&kc=1");
            url.AppendFormat("&tk={0}", tk);
            url.AppendFormat("&q={0}", HttpUtility.UrlEncode(text));

            //Create empty result
            var apiResult = new ApiResponse();
            try
            {
                var webRequest = WebRequest.Create(url.ToString()) as HttpWebRequest;
                webRequest.Method = "GET";
                webRequest.CookieContainer = cookie;
                webRequest.Referer = referer;
                webRequest.Timeout = 20000;
                webRequest.Headers.Add("X-Requested-With:XMLHttpRequest");
                webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                webRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.121 Safari/537.36";

                using (var webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    using (var reader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
                    {
                        html = reader.ReadToEnd();

                        dynamic tempResult = Newtonsoft.Json.JsonConvert.DeserializeObject(html);
                        //解决“企业\r\n学习”多行文本没有显示全部翻译
                        var resarry = Newtonsoft.Json.JsonConvert.DeserializeObject(tempResult[0].ToString());
                        var length = (resarry.Count) - 1;
                        StringBuilder str = new StringBuilder();
                        for (int i = 0; i < length; i++)
                        {
                            var res = Newtonsoft.Json.JsonConvert.DeserializeObject(resarry[i].ToString());
                            str.Append(res[0].ToString());
                        }

                        apiResult.Code = (int)webResponse.StatusCode;
                        apiResult.Message = webResponse.StatusCode.ToString();
                        apiResult.Data = str.ToString();
                        apiResult.Tags.Add("from-language", fromLanguage);
                        apiResult.Tags.Add("to-language", toLanguage);
                        apiResult.Tags.Add("translate-success", "true");

                        reader.Close();
                        webResponse.Close();
                    }
                }
            }
            catch (Exception e)
            {
                apiResult.Code = -1;
                apiResult.Message = e.Message;
                apiResult.Data = "";
            }
            return apiResult;
        }
    }
}
