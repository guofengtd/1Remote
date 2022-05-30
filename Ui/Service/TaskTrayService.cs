﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PRM.Model;
using PRM.View;
using Shawn.Utils.Interface;
using Shawn.Utils.Wpf;
using Shawn.Utils.Wpf.Controls;
using Ui;

namespace PRM.Service
{
    public class TaskTrayService
    {
        ~TaskTrayService()
        {
            TaskTrayDispose();
        }

        #region TaskTray

        private System.Windows.Forms.NotifyIcon? _taskTrayIcon = null;
        public void TaskTrayInit()
        {
            TaskTrayDispose();
            Debug.Assert(Application.GetResourceStream(ResourceUriHelper.GetUriFromCurrentAssembly("LOGO.ico"))?.Stream != null);
            _taskTrayIcon = new System.Windows.Forms.NotifyIcon
            {
                Text = AppPathHelper.APP_DISPLAY_NAME,
                Icon = new System.Drawing.Icon(Application.GetResourceStream(ResourceUriHelper.GetUriFromCurrentAssembly("LOGO.ico")).Stream),
                BalloonTipText = "",
                Visible = true
            };
            ReloadTaskTrayContextMenu();
            GlobalEventHelper.OnLanguageChanged += ReloadTaskTrayContextMenu;
            _taskTrayIcon.MouseDoubleClick += (sender, e) =>
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    IoC.Get<MainWindowViewModel>()?.ShowMe();
                }
            };
        }

        public void TaskTrayDispose()
        {
            if (_taskTrayIcon != null)
            {
                _taskTrayIcon.Visible = false;
                _taskTrayIcon.Dispose();
                _taskTrayIcon = null;
            }
        }


        private void ReloadTaskTrayContextMenu()
        {
            // rebuild TaskTrayContextMenu while language changed
            if (_taskTrayIcon == null) return;

            var title = new System.Windows.Forms.ToolStripMenuItem(AppPathHelper.APP_DISPLAY_NAME);
            title.Click += (sender, args) =>
            {
                HyperlinkHelper.OpenUriBySystem("https://github.com/1Remote/PRemoteM");
            };
            var linkHowToUse = new System.Windows.Forms.ToolStripMenuItem(IoC.Get<ILanguageService>().Translate("about_page_how_to_use"));
            linkHowToUse.Click += (sender, args) =>
            {
                HyperlinkHelper.OpenUriBySystem("https://github.com/1Remote/PRemoteM/wiki");
            };
            var linkFeedback = new System.Windows.Forms.ToolStripMenuItem(IoC.Get<ILanguageService>().Translate("about_page_feedback"));
            linkFeedback.Click += (sender, args) =>
            {
                HyperlinkHelper.OpenUriBySystem("https://github.com/1Remote/PRemoteM/issues");
            };
            var exit = new System.Windows.Forms.ToolStripMenuItem(IoC.Get<ILanguageService>().Translate("Exit"));
            exit.Click += (sender, args) => App.Close();
            _taskTrayIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            _taskTrayIcon.ContextMenuStrip.Items.Add(title);
            _taskTrayIcon.ContextMenuStrip.Items.Add("-");
            _taskTrayIcon.ContextMenuStrip.Items.Add(linkHowToUse);
            _taskTrayIcon.ContextMenuStrip.Items.Add(linkFeedback);
            _taskTrayIcon.ContextMenuStrip.Items.Add(exit);

            // After startup and initalizing our application and when closing our window and minimize the application to tray we free memory with the following line:
            System.Diagnostics.Process.GetCurrentProcess().MinWorkingSet = System.Diagnostics.Process.GetCurrentProcess().MinWorkingSet;
        }
        #endregion

    }
}
