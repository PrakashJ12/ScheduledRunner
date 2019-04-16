﻿using System;
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
        Timer timer = new Timer(); // name space(using System.Timers;)  
        public Service1()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            WriteToLogFile("Service is started at " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            int schedulerInterval = int.Parse(ConfigurationManager.AppSettings.Get("SchedulerIntervalMinutes")); //number in minutes  
            timer.Interval = schedulerInterval * 60 * 1000; //number in milisecinds  
            timer.Enabled = true;
        }
        protected override void OnStop()
        {
            WriteToLogFile("Service is stopped at " + DateTime.Now);
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            WriteToLogFile("Service is recall at " + DateTime.Now);
            try
            {
                Process myProcess = new Process();
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = ConfigurationManager.AppSettings.Get("ExecutableLocation");
                myProcess.StartInfo.Arguments = ConfigurationManager.AppSettings.Get("Parameters");
                myProcess.Start();
            }catch(Exception ex)
            {
                WriteToLogFile(ex.ToString());
                WriteToLogFile(ConfigurationManager.AppSettings.Get("ExecutableLocation"));
            }
            

            WriteToLogFile("Service executed at " + DateTime.Now);
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
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
