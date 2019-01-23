using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


#region Auther

// Name   : System Awaker
// Auther : Prabhas Harlapur
// Tool   : A Free Tool.
// Reason : Fun Programming "Not for miss Use"
// 

#endregion

namespace SystemAwaker
{
    public partial class frmMain : Form
    {

        // The mouse's target location.
        public Point m_Target = new Point(200, 150);

        static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        static bool exitFlag = false;



        public frmMain()
        {
            InitializeComponent();
        }

        // Draw a target.
        private void frmMain_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            for (int i = 5; i <= 20; i += 5)
            {
                e.Graphics.DrawEllipse(Pens.Red,
                    m_Target.X - i - 1, m_Target.Y - i - 1, 2 * i, 2 * i);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //Enable the awake mode
            PowerHelper.ForceSystemAwake();


            btnStart.Enabled = false;

            

        }

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);


        private void btnEnd_Click(object sender, EventArgs e)
        {
            PowerHelper.ResetSystemDefault();
            btnStart.Enabled = true;

            btnStartWithClick.Enabled = true;


            myTimer.Stop();
            exitFlag = true;

            // Displays a message box asking whether to continue running the timer.
            //if (MessageBox.Show("Continue running?", "Count is: " + alarmCounter,
            //   MessageBoxButtons.YesNo) == DialogResult.Yes)
            //{
            //    // Restarts the timer and increments the counter.
            //    alarmCounter += 1;
            //    myTimer.Enabled = true;
            //}
            //else
            //{
            //    // Stops the timer.
            //    exitFlag = true;
            //}

        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            PowerHelper.ResetSystemDefault();
            btnStart.Enabled = true;
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            PowerHelper.ResetSystemDefault();

           
        }

        private void frmMain_Click(object sender, EventArgs e)
        {
            // Get the mouse position.
            Point pt = MousePosition;

            // Convert to screen coordinates.
            pt = this.PointToClient(pt);

            using (Graphics gr = this.CreateGraphics())
            {
                gr.SmoothingMode = SmoothingMode.AntiAlias;
                gr.DrawLine(Pens.Blue, pt.X - 5, pt.Y - 5, pt.X + 5, pt.Y + 5);
                gr.DrawLine(Pens.Blue, pt.X + 5, pt.Y - 5, pt.X - 5, pt.Y + 5);
            }
        }

        // This is the method to run when the timer is raised.
        private void TimerEventProcessor(Object myObject,
                                                EventArgs myEventArgs)
        {


            // Convert the target to absolute screen coordinates.
            Point pt = PointToScreen(m_Target);

            // mouse_event moves in a coordinate system where
            // (0, 0) is in the upper left corner and
            // (65535,65535) is in the lower right corner.
            // Convert the coordinates.
            Rectangle screen_bounds = Screen.GetBounds(pt);
            uint x = (uint)(pt.X * 65535 / screen_bounds.Width);
            uint y = (uint)(pt.Y * 65535 / screen_bounds.Height);

            // Click there.
            mouse_event(
                (uint)(MouseEventFlags.ABSOLUTE | MouseEventFlags.MOVE |
                    MouseEventFlags.LEFTDOWN | MouseEventFlags.LEFTUP),
                x, y, 0, 0);


            
        }

        private void btnStartWithClick_Click(object sender, EventArgs e)
        {



            //Enable the awake mode
            PowerHelper.ForceSystemAwake();


            btnStartWithClick.Enabled = false;

            // Convert the target to absolute screen coordinates.
            Point pt = this.PointToScreen(m_Target);

            // mouse_event moves in a coordinate system where
            // (0, 0) is in the upper left corner and
            // (65535,65535) is in the lower right corner.
            // Convert the coordinates.
            Rectangle screen_bounds = Screen.GetBounds(pt);
            uint x = (uint)(pt.X * 65535 / screen_bounds.Width);
            uint y = (uint)(pt.Y * 65535 / screen_bounds.Height);

            // Move the mouse.
            mouse_event(
                (uint)(MouseEventFlags.ABSOLUTE | MouseEventFlags.MOVE),
                x, y, 0, 0);

            // Click there.
            mouse_event(
                (uint)(MouseEventFlags.ABSOLUTE | MouseEventFlags.MOVE |
                    MouseEventFlags.LEFTDOWN | MouseEventFlags.LEFTUP),
                x, y, 0, 0);


            /* Adds the event and the event handler for the method that will 
          process the timer event to the timer. */
            myTimer.Tick += new EventHandler(TimerEventProcessor);

            // Sets the timer interval to 5 seconds.
            myTimer.Interval = 5000;
            myTimer.Start();

            // Runs the timer, and raises the event.
            while (exitFlag == false)
            {
                // Processes all the events in the queue.
                Application.DoEvents();
            }

        }
    }


    public class PowerHelper
    {
        public static void ForceSystemAwake()
        {
            NativeMethods.SetThreadExecutionState(NativeMethods.EXECUTION_STATE.ES_CONTINUOUS |
                                                  NativeMethods.EXECUTION_STATE.ES_DISPLAY_REQUIRED |
                                                  NativeMethods.EXECUTION_STATE.ES_SYSTEM_REQUIRED |
                                                  NativeMethods.EXECUTION_STATE.ES_AWAYMODE_REQUIRED);
        }

        public static void ResetSystemDefault()
        {
            NativeMethods.SetThreadExecutionState(NativeMethods.EXECUTION_STATE.ES_CONTINUOUS);
        }
    }

    internal static partial class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [FlagsAttribute]
        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001

            // Legacy flag, should not be used.
            // ES_USER_PRESENT = 0x00000004
        }
    }

    // Mouse event flags.
    [Flags]
    public enum MouseEventFlags : uint
    {
        LEFTDOWN = 0x00000002,
        LEFTUP = 0x00000004,
        MIDDLEDOWN = 0x00000020,
        MIDDLEUP = 0x00000040,
        MOVE = 0x00000001,
        ABSOLUTE = 0x00008000,
        RIGHTDOWN = 0x00000008,
        RIGHTUP = 0x00000010,
        WHEEL = 0x00000800,
        XDOWN = 0x00000080,
        XUP = 0x00000100
    }
}
