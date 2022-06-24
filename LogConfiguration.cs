﻿namespace ASRR.Log
{
    public class NLogBasedLogConfiguration
    {
        public bool OpenOnStartUp { get; set; }
        public bool OpenOnButton { get; set; }
        public string LogName { get; set; }
        public string LogFilePath { get; set; }
        public string LogLayout { get; set; }
        public string NameFilter { get; set; }
        public string MinLevel { get; set; }
    }
}