﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewsGator.Install.Common.Output;
using NewsGator.Install.Common.Utilities;
using System.Globalization;

namespace NewsGator.Install.Common.Tasks.Debugger
{
    /// <summary>
    /// Task to disable the Just-In-Time Debugger
    /// </summary>
    internal class DisableDebugger : IInstallTask
    {
        public string Script
        {
            get
            {
                return "Disable-SocialSitesDebugger";
            }
        }

        private Guid instanceId = Guid.Empty;
        public Guid InstanceId
        {
            get
            {
                if (this.instanceId == Guid.Empty)
                    this.instanceId = Guid.NewGuid();
                return this.instanceId;
            }
        }

        public DisableDebugger()
        {
            this.Timeout = 600000;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public OutputQueue Run()
        {
            var output = new OutputQueue();
            this.Status = TaskStatus.InProgress;

            try
            {
                output.Add(NewsGator.Install.Resources.Tasks.TaskDetails_DisableDebugger);

                output.Add(DebuggerRegistry.DisableJustInTimeDebugger());

                output.Add(NewsGator.Install.Resources.Tasks.TaskDetails_DisableDebugger_Complete);
                this.Status = TaskStatus.CompleteSuccess;
            }
            catch (Exception exception)
            {
                output.Add(string.Format(CultureInfo.CurrentUICulture, NewsGator.Install.Resources.Exceptions.UpdateRegistryException, exception.Message), OutputType.Error, exception.ToString(), exception);
                this.Status = TaskStatus.CompleteFailed;
            }

            return output;
        }

        public string DisplayName
        {
            get { return NewsGator.Install.Resources.Tasks.TaskName_DisableDebugger; }
        }

        public TaskStatus Status { get; set; }

        public int Timeout { get; set; }
    }
}
