﻿using EasePass.Dialogs;
using EasePass.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Helper
{
    internal static class PrinterHelper
    {
        private static ObservableCollection<PasswordManagerItem> items;

        private static int PageIndex = 0;
        private static int ItemIndex = 0;

        private const float LeftSpacing = 1.5f; // cm
        private const float TopSpacing = 1.5f; // cm
        private const float RightSpacing = 1.5f; // cm
        private const float BottomSpacing = 1.5f; // cm

        private static readonly Brush Brush = Brushes.Black;

        private static readonly Font HeadingFont = new Font(FontFamily.GenericSansSerif, 72, FontStyle.Bold);
        private static readonly Font DisplayNameFont = new Font(FontFamily.GenericSansSerif, 13, FontStyle.Bold);
        private static readonly Font BodyFont = new Font(FontFamily.GenericSansSerif, 13);

        public static async void Print(ObservableCollection<PasswordManagerItem> items, string printer)
        {
            await Task.Run(() =>
            {
                PrinterHelper.items = items;
                PrintDocument pd = new PrintDocument();
                pd.PrinterSettings.PrinterName = printer;
                pd.PrintPage += Pd_PrintPage;
                pd.EndPrint += Pd_EndPrint;
                PageIndex = 0;
                ItemIndex = 0;
                pd.Print();
            });
        }

        private static void Pd_EndPrint(object sender, PrintEventArgs e)
        {
            items = null;
        }

        private static void Pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            float dpix = e.Graphics.DpiX / 6;
            float dpiy = e.Graphics.DpiY / 6;
            float width = e.PageBounds.Width / 100 * dpix;
            float height = e.PageBounds.Height / 100 * dpiy;
            float yOffset = Math.Max(CmToInch(TopSpacing) * dpiy, 0/*e.MarginBounds.Top*/);
            float bottomSpace = Math.Max(CmToInch(BottomSpacing) * dpiy, 0/*e.MarginBounds.Bottom*/);
            float leftSpace = Math.Max(CmToInch(LeftSpacing) * dpix, 0/*e.MarginBounds.Left*/);
            if (PageIndex == 0)
            {
                SizeF size = e.Graphics.MeasureString("Ease Pass", HeadingFont);
                e.Graphics.DrawString("Ease Pass", HeadingFont, Brush, new PointF(width / 2 - size.Width / 2, yOffset));
                yOffset += size.Height + dpiy * 0.75f; // add 3/4 inch spacing
            }
            
            while (ItemIndex < items.Count)
            {
                var item = BuildServiceString(items[ItemIndex]);
                float textHeight = 0;
                SizeF displayNameSize = e.Graphics.MeasureString(item.name, DisplayNameFont);
                textHeight += displayNameSize.Height;
                textHeight += 0.1f * dpiy;
                SizeF bodySize = e.Graphics.MeasureString(item.body, BodyFont);
                textHeight += bodySize.Height;
                if (textHeight > height)
                {
                    InfoMessages.PrinterItemSkipped(item.name);
                    ItemIndex++;
                    continue;
                }
                if (yOffset + textHeight + bottomSpace > height) break;
                e.Graphics.DrawString(item.name, DisplayNameFont, Brush, new PointF(leftSpace, yOffset));
                yOffset += displayNameSize.Height;
                yOffset += 0.1f * dpiy;
                e.Graphics.DrawString(item.body, BodyFont, Brush, new PointF(leftSpace, yOffset));
                yOffset += bodySize.Height;
                yOffset += 0.5f * dpiy; // 1/2 inch spacing between services
                ItemIndex++;
            }

            PageIndex++;
            e.HasMorePages = ItemIndex < items.Count;
        }

        private static float CmToInch(float cm)
        {
            return cm / 2.54f;
        }

        private static (string name, string body) BuildServiceString(PasswordManagerItem item)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(item.Username)) sb.AppendLine("Username: " + item.Username);
            if (!string.IsNullOrEmpty(item.Email)) sb.AppendLine("E-Mail: " + item.Email);
            if (!string.IsNullOrEmpty(item.Password)) sb.AppendLine("Password: " + item.Password);
            if (!string.IsNullOrEmpty(item.Website)) sb.AppendLine("Website: " + item.Website);
            if (!string.IsNullOrEmpty(item.Secret))
            {
                sb.AppendLine("TOTP-Secret: " + item.Secret);
                sb.AppendLine("TOTP-Algorithm: " + item.Algorithm);
                sb.AppendLine("TOTP-Interval: " + item.Interval);
            }
            if (!string.IsNullOrEmpty(item.Notes))
            {
                sb.AppendLine("Notes:");
                sb.AppendLine(item.Notes);
            }
            return (item.DisplayName, sb.ToString());
        }
    }
}
