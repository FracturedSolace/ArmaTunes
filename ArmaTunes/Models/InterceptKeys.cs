using System;
using System.Diagnostics;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Windows;
using ArmaTunes;
using ArmaTunes.Models.WindowsLowLevel;

public static class InterceptKeys
{
    private const int WM_KEYDOWN = 0x0100;

    private static HookProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;

    public static void SetHook()
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            //_hookID = SetWindowsHookEx(WH_KEYBOARD_LL, _proc,
            //    GetModuleHandle(curModule.ModuleName), 0);
            _hookID = SetWindowsHookEx(HookType.WH_KEYBOARD_LL, _proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    public static void UnSetHook()
    {
        UnhookWindowsHookEx(_hookID);
    }

    private static IntPtr HookCallback(
        int nCode, IntPtr wParam, IntPtr lParam)
    {
        int vkCode = Marshal.ReadInt32(lParam);

        ArmaTunes.Models.Debug.Write($"Detected keypress [KeyCode: {vkCode}]");

        if(vkCode == 20)//CAPS Lock: Used to toggle Music over Mic
        {
            ((MainWindow)Application.Current.MainWindow).ToggleMusic();
        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
        IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
}
