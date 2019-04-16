using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration;

namespace ScheduledRunner
{
    public partial class Service1 : ServiceBase
    {
        const int ConsoleReadWaitTime = 60; //In Seconds
        Timer _timer;
        DateTime _scheduleTime;
        int dailyRunTime_HH;
        int dailyRunTime_MM;

        public Service1()
        {
            InitializeComponent();

            _timer = new System.Timers.Timer();
            dailyRunTime_HH = int.Parse(ConfigurationManager.AppSettings.Get("SchedulerTimeHour"));
            dailyRunTime_MM = int.Parse(ConfigurationManager.AppSettings.Get("SchedulerTimeMinute"));
            _scheduleTime = DateTime.Today.AddDays(0).AddHours(dailyRunTime_HH).AddMinutes(dailyRunTime_MM); // Schedule to run once a day at 7:00 a.m.

        }
        protected override void OnStart(string[] args)
        {
            WriteToLogFile("Service is started at " + DateTime.Now);
            
            double tillNextInterval = _scheduleTime.Subtract(DateTime.Now).TotalSeconds * 1000;
            if (tillNextInterval < 0)
            {
                tillNextInterval += new TimeSpan(24, 0, 0).TotalSeconds * 1000;
            }
            _timer.Interval = tillNextInterval;
            _timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            _timer.Enabled = true;

            WriteToLogFile("First Run Scheduled in " + (((tillNextInterval)/1000))/60 + " Minutes");
        }
        protected override void OnStop()
        {
            WriteToLogFile("Service is stopped at " + DateTime.Now);
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            WriteToLogFile("Scheduled run started at " + DateTime.Now);
            try
            {
                Process myProcess = new Process();
                //myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.RedirectStandardOutput = true;
                myProcess.OutputDataReceived += (sender, args) => WriteToLogFile(args.Data);
                myProcess.StartInfo.FileName = ConfigurationManager.AppSettings.Get("ExecutableLocation");
                myProcess.StartInfo.Arguments = ConfigurationManager.AppSettings.Get("Parameters");
                myProcess.Start();
                myProcess.BeginOutputReadLine();

                WriteToLogFile("Scheduled run executed successfully at " + DateTime.Now +". Initiating "+ ConsoleReadWaitTime + " second delay.");
            }
            catch(Exception ex)
            {
                WriteToLogFile("ERROR001: An Error Occured During Scheduled Run");
                WriteToLogFile(ex.ToString());
                WriteToLogFile(ConfigurationManager.AppSettings.Get("ExecutableLocation"));
            }

            if (_timer.Interval != 24 * 60 * 60 * 1000)
            {
                _timer.Interval = 24 * 60 * 60 * 1000;
            }



            WriteToLogFile("Next Run Scheduled in " + (int)((((_timer.Interval) / 1000)) / 60) + " Minutes");


        }
        public void WriteToLogFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    Console.WriteLine(System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("LS0tLS0tLURhaWx5IFNjaGVkdWxlZCBSdW5uZXIgU2VydmljZSBmb3IgV2luZG93cyBNYWNoaW5lcyguTkVUIHY0KyktLS0tLS0tDQoNCkRldmVsb3BlZCBCeSBQcmFrYXNoIEpvc2VwaCA8UHJha2FzaC5Kb3NlcGggYXQgZ2RzLmV5LmNvbT4NClNvdXJjZSBSZXBvOiBodHRwczovL2dpdGh1Yi5jb20vUHJha2FzaEoxMi9TY2hlZHVsZWRSdW5uZXINCg0K")));
                    sw.WriteLine(DateTime.Now.ToString() + ": " + Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(DateTime.Now.ToString() + ": " + Message);
                }
            }
        }
    }
}
