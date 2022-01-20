﻿//------------------------------------------------------------------------------
// <copyright file="CommentTranslator.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using CommentTranslator.Ardonment;
using CommentTranslator.Util;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using CommentTranslator.Option;
using Microsoft.VisualStudio.Shell.Interop;

namespace CommentTranslator
{
    /// <summary>
    /// 翻译按钮事件处理
    /// </summary>
    internal sealed class TranslateCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("284ee4d1-edc6-4ed0-bf84-d45aeebf593c");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslateCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private TranslateCommand(Package package)
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

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static TranslateCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// 从所有者包中获取服务提供者。
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }



        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new TranslateCommand(package);
        }



        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Settings settings = new Settings();
            //获取配置值，赋值验证TKK
            settings.ReloadSetting((OptionPageGrid)package.GetDialogPage(typeof(OptionPageGrid)));
            if (string.IsNullOrWhiteSpace(settings.TKK))
            {
                ShowMessageBox("请先设置TKK值");
                return;
            }

            //获取DTE对象
            var dte = (DTE2)ServiceProvider.GetService(typeof(DTE));
            //  EnvDTE80.DTE2 _dte2 = (EnvDTE80.DTE2)ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE));

            if (dte.ActiveDocument != null)
            {
                var selection = (TextSelection)dte.ActiveDocument.Selection;

                //如果不选择文本，自动选择光标所在行
                if (string.IsNullOrEmpty(selection.Text))
                {
                    selection.SelectLine();
                }

                //修剪所选文本
                var parser = CommentParserHelper.GetCommentParser(dte.ActiveDocument.Language);
                var text = selection.Text.Trim();

                if (parser != null)
                {
                    var regions = parser.GetCommentRegions(text, 0);
                    if (regions.Count() > 0 && regions.First().Start == 0 && regions.Last().End == text.Length)
                    {
                        text = string.Join(Environment.NewLine, regions.Select(r => parser.GetComment(text.Substring(r.Start, r.Length)).Content.Trim()));
                    }
                }

                //Check if selection text is still empty
                if (!string.IsNullOrEmpty(text))
                {
                    TranslatePopupConnector.Translate(GetWpfView(), text);
                }
            }
        }

        private IWpfTextView GetWpfView()
        {
            var textManager = (IVsTextManager)ServiceProvider.GetService(typeof(SVsTextManager));
            var componentModel = (IComponentModel)this.ServiceProvider.GetService(typeof(SComponentModel));
            var editor = componentModel.GetService<IVsEditorAdaptersFactoryService>();

            textManager.GetActiveView(1, null, out IVsTextView textViewCurrent);
            return editor.GetWpfTextView(textViewCurrent);
        }



        public void ShowMessageBox(string mes)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string message = string.Format(CultureInfo.CurrentCulture, mes);
            string title = "系统提示";
            VsShellUtilities.ShowMessageBox(package, message, title,
                OLEMSGICON.OLEMSGICON_WARNING, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }






    }
}
