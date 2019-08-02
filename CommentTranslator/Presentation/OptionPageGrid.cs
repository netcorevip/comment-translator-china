using System.ComponentModel;

using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace CommentTranslator.Option
{
    /// <summary>
    /// 工具选项窗体
    /// </summary>
    public class OptionPageGrid : DialogPage
    {
        /*[Category("Server")]
        [DisplayName("翻译服务器URL")]
        [Description("翻译服务器的网址")]
        public string TranslateUrl { get; set; } = "https://translate.google.cn/";*/

        /// <summary>
        /// 谷歌TKK
        /// </summary>
        [Category("Server")]
        [DisplayName("谷歌TKK")]
        [Description("设置谷歌TKK参数")]
        public string TKK { get; set; }

        /// <summary>
        /// 待翻译语言
        /// </summary>
        [Category("Translate")]
        [DisplayName("待翻译语言")]
        [Description("待翻译语言类型")]
        public string TranslateFrom { get; set; } = "en";

        /// <summary>
        /// 目标语言
        /// </summary>
        [Category("Translate")]
        [DisplayName("翻译成语言")]
        [Description("翻译为目标语言类型")]
        public string TranslatetTo { get; set; } = "zh-CN";

        /// <summary>
        /// 自动检测语言
        /// </summary>
        [Category("Translate")]
        [DisplayName("自动检测类型")]
        [Description("自动检测待翻译语言类型")]
        public bool AutoDetect { get; set; } = false;

        [Category("Translate")]
        [DisplayName("打开文件自动翻译")]
        [Description("打开文件时自动翻译注释")]
        public bool AutoTranslateComment { get; set; } = false;

        [Category("Translate")]
        [DisplayName("手动翻译自动复制")]
        [Description("手动翻译自动复制内容到剪切板")]
        public bool AutoTextCopy { get; set; } = false;




        protected override void OnApply(PageApplyEventArgs e)
        {
            base.OnApply(e);

            if (e.ApplyBehavior == ApplyKind.Apply)
            {
                SaveToSetting();
            }
        }


        /// <summary>
        /// 保存设置
        /// </summary>
        public void SaveToSetting()
        {
            // C#中MessageBox用法大全（附效果图）
            //https://www.cnblogs.com/rooly/articles/1910063.html

            if (string.IsNullOrWhiteSpace(TKK))
            {

                MessageBox.Show("请先设置TKK值！", "系统提示");
                //   return;
            }
            //刷新值
            CommentTranslatorPackage.Settings.ReloadSetting(this);
        }

        //【小试插件开发】给Visual Studio装上自己定制的功能来提高代码调试效率
        //https://www.cnblogs.com/hohoa/p/6617619.html?utm_source=gold_browser_extension



    }
}
