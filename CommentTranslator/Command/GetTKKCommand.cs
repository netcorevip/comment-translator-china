using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using CommentTranslator.Option;
using Microsoft.VisualStudio.Shell.Interop;

namespace CommentTranslator
{
    /// <summary>
    /// 获取TKK按钮事件处理
    /// </summary>
    internal sealed class GetTKKCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0111;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("284ee4d1-edc6-4ed0-bf84-d45aeebf593c");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;


        private GetTKKCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }


        public static GetTKKCommand Instance
        {
            get;
            private set;
        }

        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        public static void Initialize(Package package)
        {
            Instance = new GetTKKCommand(package);
        }




        private void MenuItemCallback(object sender, EventArgs e)
        {
            try
            {
                var baseResultHtml = GetResultHtml("https://translate.google.cn/");
                Regex re = new Regex(@"(tkk:')(.*?)(?=')"); //此正则返回：tkk:'431119.315913250
                var tkks = re.Match(baseResultHtml).ToString().Split('\'');
                var TKK = string.IsNullOrWhiteSpace(tkks[1]) ? "" : tkks[1].Trim(); //在返回的HTML中正则匹配TKK的值

                if (!string.IsNullOrWhiteSpace(TKK))
                {
                    try
                    {
                        //处理权限不足无法写入问题
                        Clipboard.SetDataObject(TKK.Trim(), true); //关闭程序保留剪贴板内容
                    }
                    catch (Exception exception)
                    {

                    }

                    OptionPageGrid page = (OptionPageGrid)this.package.GetDialogPage(typeof(OptionPageGrid));
                    page.TKK = TKK;
                    page.SaveSettingsToStorage();
                    page.SaveToSetting();
                    OutputString(Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.GeneralPane_guid, string.Format("从https://translate.google.cn/获取的TKK为：{0}", TKK));
                }
                else
                {
                    ShowMessageBox("获取TKK值失败，请打开浏览器手动获取");
                }

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }


        }

        public string GetResultHtml(string url)
        {
            var html = "";
            string referer = url;
            CookieContainer cookie = new CookieContainer();
            var webRequest = WebRequest.Create(url) as HttpWebRequest;

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
                    var Headers = webResponse.Headers;
                    html = reader.ReadToEnd();
                    reader.Close();
                    webResponse.Close();
                }
            }
            return html;
        }

        /// <summary>
        /// 输出内容到输出窗口
        /// </summary>
        /// <param name="guidPane"></param>
        /// <param name="text"></param>
        private void OutputString(Guid guidPane, string text)
        {
            const int VISIBLE = 1;
            const int DO_NOT_CLEAR_WITH_SOLUTION = 0;
            IVsOutputWindow outputWindow;
            IVsOutputWindowPane outputWindowPane = null;
            int hr;
            // Get the output window
            outputWindow = ServiceProvider.GetService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            //默认情况下不会创建“常规”窗格。 我们必须强迫它的创造
            if (guidPane == Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.GeneralPane_guid)
            {
                hr = outputWindow.CreatePane(guidPane, "General", VISIBLE, DO_NOT_CLEAR_WITH_SOLUTION);
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
            }
            // Get the pane
            hr = outputWindow.GetPane(guidPane, out outputWindowPane);
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
            // Output the text
            if (outputWindowPane != null)
            {
                outputWindowPane.Activate();
                outputWindowPane.OutputString(text);
            }
        }
        public void ShowMessageBox(string mes)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string message = string.Format(CultureInfo.CurrentCulture, mes);
            string title = "系统提示";
            VsShellUtilities.ShowMessageBox(package, message, title,
                OLEMSGICON.OLEMSGICON_WARNING, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        /*
         private void LogCalculationToOutput(string firstArg, string secondArg, string operation, string result)
        {
            string message = String.Format("Calculation executed: {0} {1} {2} = {3} ", firstArg, operation, secondArg, result);

            IVsOutputWindow outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            Guid generalWindowGuid = VSConstants.GUID_OutWindowGeneralPane;
            IVsOutputWindowPane windowPane;
            outWindow.GetPane(ref generalWindowGuid, out windowPane);
            windowPane.OutputString(message);
        }
        */




    }
}
