using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;


namespace ThreeWorkTool
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            //Checks for arguments and opens arc file if valid.
            //if (args.Length >= 1)
            //{
            //    string fileName = args[0];
            //    //Check file exists
            //    if (File.Exists(fileName))
            //    {
            //        //OpenFromStart(fileName);
            //    }
            //}
            //else
            //{
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            FrmMainThree ThreeForm = new FrmMainThree();
            Application.Run(ThreeForm);
            //}
        }

        //Puts up an error message box, writes the exception to the log, and closes the program.
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"An exception occured. If you report this, make sure they can see this next part:\n {e.ExceptionObject?.GetType()}", "Fatal Error!",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            //Writes to log file.
            string ProperPath = "";
            ProperPath = Globals.ToolPath + "Log.txt";
            using (StreamWriter sw = File.AppendText(ProperPath))
            {
                sw.WriteLine("\n=====EXCEPTION OCCURED!=====\n");
                sw.WriteLine(e.ToString());
                sw.WriteLine(e.ExceptionObject?.GetType());
            }



            Application.Exit();


        }


    }
}
