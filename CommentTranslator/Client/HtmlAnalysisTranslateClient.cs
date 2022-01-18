using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CommentTranlsator.Client;
using CommentTranslator.Util;
using Framework;

namespace CommentTranlsator.Client
{
    public class HtmlAnalysisTranslateClient : ITranslateClient
    {
        private Settings _settings;
        public HtmlAnalysisTranslateClient(Settings settings)
        {
            _settings = settings;
        }
        public async Task<IAPIResponse> Translate(string text)
        {
            var client = HttpClientFactory.Create();
            var fromLanguage = _settings.AutoDetect ? "auto" : _settings.TranslateFrom;
            var toLanguage = _settings.TranslateTo;
            //FormUrlEncodedContent
            var from = Uri.EscapeDataString(text);
            var apiResult = new ApiResponse();
            try
            {
                var data = await client.GetAsync($"https://translate.google.cn/m?q={from}&tl={toLanguage}&sl={fromLanguage}");
                apiResult.Code = (int)data.StatusCode;
                apiResult.Message = data.StatusCode.ToString();
                var html = await data.Content.ReadAsStringAsync();
                var reg = new System.Text.RegularExpressions.Regex("(?s)class=\"(?:t0|result-container)\">(.*?)<");

                var match = reg.Match(html);
                if (match.Groups.Count == 2)
                {
                    apiResult.Data = match.Groups[1].Value;
                }
                apiResult.Tags.Add("from-language", fromLanguage);
                apiResult.Tags.Add("to-language", toLanguage);
                apiResult.Tags.Add("translate-success", "true");
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
