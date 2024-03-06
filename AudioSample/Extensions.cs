using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioSample
{
    internal static class Extensions
    {
        [DllImport("user32")]
        private static extern int GetScrollInfo(IntPtr hWnd, int nBar, ref SCROLLINFO lpsi);

        [StructLayout(LayoutKind.Sequential)]
        private struct SCROLLINFO
        {
            public int cbSize;
            public int fMask;
            public int min;
            public int max;
            public int nPage;
            public int nPos;
            public int nTrackPos;
        }

        public static bool ReachedBottom(this RichTextBox rtb)
        {
            var info = new SCROLLINFO
            {
                cbSize = Marshal.SizeOf(typeof(SCROLLINFO)),
                fMask = 0b_0001_0011,
            };
            GetScrollInfo(rtb.Handle, 1, ref info);
            return info.max <= info.nTrackPos + info.nPage + 30;
        }

        public static void AddText(this RichTextBox rtb, string text)
        {
            Action action = () =>
            {
                if (rtb.TextLength == 0)
                    rtb.Text = text;
                else
                    rtb.AppendText($"\n{text}");

                if (rtb.ReachedBottom())
                {
                    rtb.SelectionStart = rtb.TextLength;
                    rtb.ScrollToCaret();
                }
            };

            if (rtb.InvokeRequired)
                rtb.BeginInvoke(action);
            else
                action();
        }
    }
}
