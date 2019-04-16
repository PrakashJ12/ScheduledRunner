using System;
using System.Configuration;
using System.Reflection;
using System.ServiceProcess;

namespace ScheduledRunner
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller = new System.ServiceProcess.ServiceInstaller();


            Assembly service = Assembly.GetAssembly(typeof(ProjectInstaller));
            string assemblyPath = service.Location;
            Configuration config = ConfigurationManager.OpenExeConfiguration(assemblyPath);
            KeyValueConfigurationCollection mySettings = config.AppSettings.Settings;
            // 
            // serviceProcessInstaller
            // 
            // System.ServiceProcess.ServiceAccount.LocalSystem;
            //this.serviceProcessInstaller.Account = (ServiceAccount)Enum.Parse(typeof(ServiceAccount), mySettings["UserAccount"].Value);
            if ("LocalSystem".Equals(mySettings["UserAccount"].Value))
            {
                this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
                this.serviceProcessInstaller.Password = null;
                this.serviceProcessInstaller.Username = null;
            }
            else
            {
                this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.User;
                this.serviceProcessInstaller.Password = mySettings["UserAccount"].Value;
                this.serviceProcessInstaller.Username = mySettings["Password"].Value;
            }

            
            
            // 
            // serviceInstaller
            // 
            this.serviceInstaller.DisplayName = mySettings["ServiceName"].Value;
            this.serviceInstaller.ServiceName = mySettings["ServiceName"].Value;
            this.serviceInstaller.Description = mySettings["ServiceDescription"].Value;

            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller,
            this.serviceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller serviceInstaller;
    }
}