using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaTunes.Models
{
    public static class Debug
    {
        public static void Write(string msg)
        {
            ((MainWindow)App.Current.MainWindow).txtDebug.Text += msg+"\n";
        }
    }
}
