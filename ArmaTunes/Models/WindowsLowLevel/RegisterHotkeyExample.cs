﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace ArmaTunes.Models.WindowsLowLevel
{
    public class Hotkey
    {
        private Window window = null;

        private HwndSource _source;
        private int HOTKEY_ID;

        public EventHandler OnHotKeyPressed;

        public Hotkey(Window window, int HOTKEY_ID)
        {
            this.window = window;
            this.HOTKEY_ID = HOTKEY_ID;

            //Generate a WindowInteroperability helper and add the hook
            var helper = new WindowInteropHelper(window);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);

            window.Closed += OnClosed;

            RegisterHotKey();
        }

        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey(
            [In] IntPtr hWnd,
            [In] int id,
            [In] uint fsModifiers,
            [In] uint vk);

        [DllImport("User32.dll")]
        private static extern bool UnregisterHotKey(
            [In] IntPtr hWnd,
            [In] int id);
        

        public void OnClosed(object sender, EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            _source = null;
            UnregisterHotKey();
        }

        private void RegisterHotKey()
        {
            var helper = new WindowInteropHelper(window);
            const uint VK_F10 = 0x79;
            const uint MOD_CTRL = 0x0002;
            if (!RegisterHotKey(helper.Handle, HOTKEY_ID, MOD_CTRL, VK_F10))
            {
                // handle error
            }
        }

        private void UnregisterHotKey()
        {
            var helper = new WindowInteropHelper(window);
            UnregisterHotKey(helper.Handle, HOTKEY_ID);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int MSG_TYPE_HOTKEY = 0x0312;
            switch (msg)
            {
                case MSG_TYPE_HOTKEY:
                    if (wParam.ToInt32() == HOTKEY_ID)
                    {
                        OnHotKeyPressed(this, new EventArgs());
                        handled = true;
                    }
                    break;
            }
            return IntPtr.Zero;
        }
    }
}
