using CommentTranslator.Option;

namespace CommentTranslator.Util
{
    /// <summary>
    /// 获取工具菜单设置的值
    /// </summary>
    public class Settings
    {
        //public const string SettingCollection = @"InstalledProducts\Comment Translator";

        //public const string TranslateUrlProperty = @"TranslateUrl";
        //public const string TranslateFromProperty = @"TranslateFrom";
        //public const string TranslateToProperty = @"TranslateTo";
        //public const string AutoDetectProperty = @"AutoDetect";

        public string TKK { get; set; }
        public string TranslateUrl { get; set; }
        public string TranslateFrom { get; set; }
        public string TranslateTo { get; set; }
        public bool AutoDetect { get; set; }
        public bool AutoTranslateComment { get; set; }

        public bool AutoTextCopy { get; set; }

        /// <summary>
        /// 刷新设置的值
        /// </summary>
        /// <param name="page"></param>
        public void ReloadSetting(OptionPageGrid page)
        {
            TranslateUrl = "";//page.TranslateUrl;
            TranslateFrom = page.TranslateFrom;
            TranslateTo = page.TranslatetTo;
            AutoDetect = page.AutoDetect;
            AutoTranslateComment = page.AutoTranslateComment;
            TKK = page.TKK;
            AutoTextCopy = page.AutoTextCopy;
        }
    }
}
