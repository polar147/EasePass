﻿using EasePass.Settings;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;

namespace EasePass.Helper
{
    internal class ClipboardHelper
    {
        static Queue<ClipboardHistoryItem> history = new Queue<ClipboardHistoryItem>();

        public static void Copy(string text, bool removeFromClipboard = false)
        {
            var package = new DataPackage();
            package.SetText(text);
            Clipboard.SetContent(package);

            //remove from the clipboard:
            if (removeFromClipboard)
                RemoveLastClipboardItem(text);
        }

        public static void RemoveLastClipboardItem(string text)
        {
            // Needs to be a short delay here
            var dp = new DispatcherTimer();
            dp.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            dp.Start();
            dp.Tick += async (s, e) =>
            {
                var items = (await Clipboard.GetHistoryItemsAsync()).Items;
                if (items.Count > 0)
                {
                    for(int i = 0; i < items.Count; i++)
                    {
                        try
                        {
                            if (await items[i].Content.GetTextAsync() == text)
                            {
                                history.Enqueue(items[i]);
                                break;
                            }
                        }
                        catch { }
                    }
                }
                dp.Stop();
            };

            var dp1 = new DispatcherTimer();
            dp1.Interval = new TimeSpan(0, 0, AppSettings.GetSettingsAsInt(AppSettingsValues.clipboardClearTimeoutSec, DefaultSettingsValues.ClipboardClearTimeoutSec));
            dp1.Start();
            dp1.Tick += async (s, e) =>
            {
                dp1.Stop();
                var formats = Clipboard.GetContent().AvailableFormats;
                if (formats == null)
                {
                    return;
                }
                if (await Clipboard.GetContent().GetTextAsync() == text)
                {
                    Clipboard.SetContent(null);
                }
                var items = (await Clipboard.GetHistoryItemsAsync()).Items;
                if (items.Count > 0)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        try
                        {
                            if (await items[i].Content.GetTextAsync() == text)
                            {
                                history.Enqueue(items[i]);
                                break;
                            }
                        }
                        catch { }
                    }
                }
            };
        }
    }
}
