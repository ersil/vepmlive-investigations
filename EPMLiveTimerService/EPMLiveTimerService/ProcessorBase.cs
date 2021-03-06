﻿using System;
using Microsoft.SharePoint.Administration;
using System.Data;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace TimerService
{
    public abstract class ProcessorBase
    {
        protected static SPWebApplicationCollection GetWebApplications()
        {
            return SPWebService.ContentService.WebApplications;
        }
        private Object thisLock = new Object();

        protected struct RunnerData
        {
            public string cn;
            public DataRow dr;
        }
        protected int RunningThreads
        {
            get
            {
                return threadList.Count;
            }
        }
        private static object threadsLock = new object();
        private int _maxThreads;
        protected int MaxThreads {
            get {
                lock (threadsLock)
                {
                    for (int i = threadList.Count - 1; i >= 0; i--)
                    {
                        if (!threadList[i].Item1.IsAlive)
                        {
                            threadList.RemoveAt(i);

                        }
                        else if (Timeout > 0 && DateTime.Now - threadList[i].Item2 > new TimeSpan(0, Timeout * threadList[i].Item3, 0))
                        {
                            
                                try
                                {
                                    threadList[i].Item1.Abort();
                                    threadList[i].Item1.Join();
                                }
                                catch { }

                                threadList.RemoveAt(i);

                           
                        }

                    }
                    return _maxThreads - threadList.Count;
                }
            }
        }
        
        List<Tuple<Thread, DateTime,int>> threadList = new List<Tuple<Thread, DateTime, int>>();
        protected bool startProcess(RunnerData rd, int trialNumber = 1)
        {
            try
            {
                Thread newThread = new Thread(new ParameterizedThreadStart(DoWork));
                newThread.Start(rd);
                lock (threadsLock)
                {
                    threadList.Add(new Tuple<Thread,DateTime, int>(newThread,DateTime.Now, trialNumber));
                }
                return true;
            }
            catch (Exception ex)
            {
                LogMessage("ERR", "STPR", ex.Message);
                return false;
            }
        }

        public void LogMessage(string type, string module, string message)
        {
            lock (thisLock)
            {
                DateTime dt = DateTime.Now;

                using (StreamWriter swLog = new StreamWriter(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\LOGS\\" + LogName + "_" + dt.Year + dt.Month + dt.Day + ".log", true))
                {

                    swLog.WriteLine(DateTime.Now.ToString() + "\t" + type + "\t" + module + "\t" + message);

                }
            }
        }

        protected CancellationToken token;
        public virtual void InitializeTask(CancellationToken token)
        {
            InitializeTask(true, token);
        }

        public virtual void InitializeTask(bool initializeThreads, CancellationToken token)
        {
            this.token = token;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\LOGS");
            }
            catch { }

            LogMessage("INIT", "STMR", "Starting Timer Service");
            if (!initializeThreads)
            {
                initialized = true;
                return;
            }
            int maxThreads = 0;
            try
            {
                maxThreads = int.Parse(EPMLiveCore.CoreFunctions.getFarmSetting(ThreadsProperty));
            }
            catch (Exception e)
            {
                LogMessage("INIT", "GTERR", "Unable to read default thread value from Farm Settings");
            }
            if (maxThreads < 1)
                return;

            LogMessage("INIT", "STMR", "Setting threads to: " + maxThreads);
            _maxThreads = maxThreads;
            initialized = true;
        }
        public void HeartBeat()
        {
            LogMessage("HTBT", "MNTH", "Dispatcher alive, running threads: " + RunningThreads + ", free threads: " + MaxThreads);
        }
        public virtual void Cancel()
        {
            lock (threadsLock)
            {
                foreach (Tuple<Thread, DateTime, int> tuple in threadList)
                {
                    tuple.Item1.Abort();
                }
            }
            LogMessage("STOP", "STMR", "Stopped Timer Service");
        }
        protected abstract string LogName {
            get;
        }
        protected virtual int Timeout
        {
            get
            {
                return 0;
            }
        }
        public abstract void ProcessJobs();

        public virtual string PollingProperty
        {
            get
            {
                return "PollingInterval";
            }

        }
        bool initialized = false;
        public bool Initialized
        {
            get
            {
                return initialized;
            }


        }
           

        protected abstract void DoWork(object rd);

        protected abstract string ThreadsProperty {
            get;
        }

    }
}
