using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

#region Author

// Name   : System Awaker
// Auther : Prabhas Harlapur
// Tool   : A Free Tool.
// Reason : Fun Programming "Not for miss Use"
// 

#endregion

namespace SystemAwaker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}
