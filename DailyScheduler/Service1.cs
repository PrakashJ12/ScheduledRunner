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
using System.Globalization;

namespace ScheduledRunner
{
    public partial class Service1 : ServiceBase
    {
        const int ConsoleReadWaitTime = 60; //In Seconds
        Timer _timer;
        int dailyRunTime_HH;
        int dailyRunTime_MM;
        string logFilePath;

        public Service1()
        {
            InitializeComponent();
            logFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToString("yyyy_MM_dd", CultureInfo.InvariantCulture) + ".txt";
            _timer = new System.Timers.Timer();
            dailyRunTime_HH = int.Parse(ConfigurationManager.AppSettings.Get("SchedulerTimeHour"));
            dailyRunTime_MM = int.Parse(ConfigurationManager.AppSettings.Get("SchedulerTimeMinute"));
        }
        protected override void OnStart(string[] args)
        {
            WriteToLogFile("Service is started at " + DateTime.Now);
            
            
            _timer.Interval = getInterval();
            _timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            _timer.Enabled = true;

            WriteToLogFile("First Run Scheduled in " + (int)((((_timer.Interval) /1000))/60) + " Minutes");
        }

        private double getInterval()
        {
            double tillNextInterval = DateTime.Today.AddDays(0).AddHours(dailyRunTime_HH).AddMinutes(dailyRunTime_MM).Subtract(DateTime.Now).TotalSeconds * 1000;
            if (tillNextInterval < 0)
            {
                tillNextInterval += new TimeSpan(24, 0, 0).TotalSeconds * 1000;
            }
            return tillNextInterval;
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
                
                WriteToLogFile("Scheduled run executed successfully at " + DateTime.Now);
            }
            catch(Exception ex)
            {
                WriteToLogFile("ERROR001: An Error Occured During Scheduled Run");
                WriteToLogFile(ex.ToString());
                WriteToLogFile(ConfigurationManager.AppSettings.Get("ExecutableLocation"));
            }

            _timer.Interval = getInterval();
            
            WriteToLogFile("Next Run Scheduled in " + (int)((((_timer.Interval) / 1000)) / 60) + " Minutes");
        }
        public void WriteToLogFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = logFilePath;
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Encoding.UTF8.GetString(Convert.FromBase64String("LS0tLS0tLURhaWx5IFNjaGVkdWxlZCBSdW5uZXIgU2VydmljZSBmb3IgV2luZG93cyBNYWNoaW5lcyguTkVUIHY0KyktLS0tLS0tDQoNCkRldmVsb3BlZCBCeSBQcmFrYXNoIEpvc2VwaCA8UHJha2FzaC5Kb3NlcGggYXQgZ2RzLmV5LmNvbT4gb2YgRXZlbnQgQ29uZmlndXJhdGlvbiAtIEFwcGxpY2F0aW9uIEluc3RydW1lbnRhdGlvbiBUZWFtDQpTb3VyY2UgUmVwbzogaHR0cHM6Ly9naXRodWIuY29tL1ByYWthc2hKMTIvU2NoZWR1bGVkUnVubmVyDQoNCg==")));
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) + ": " + Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) + ": " + Message);
                }
            }
        }
    }
}
