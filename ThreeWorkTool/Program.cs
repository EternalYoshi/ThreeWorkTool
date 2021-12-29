using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;


namespace ThreeWorkTool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.Run(new FrmMainThree());
        }

        //Puts up an error message box, writes the exception to the log, and closes the program.
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"An exception occured. If you report this, make sure they can see this next part:\n {e.ExceptionObject?.GetType()}", "Fatal Error!",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            //Writes to log file.
            using (StreamWriter sw = File.AppendText("Log.txt"))
            {
                sw.WriteLine("\n=====EXCEPTION OCCURED!=====\n");
                sw.WriteLine(e.ToString());
                sw.WriteLine(e.ExceptionObject?.GetType());
            }



            Application.Exit();


        }


    }
}
