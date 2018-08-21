﻿using System;
using System.Net;

namespace Tars.Csharp.Hosting.Configurations
{
    public class HostConfiguration
    {
        public string Ip { get; set; } = "127.0.0.1";

        public IPAddress IPAddress => IPAddress.Parse(Ip);

        public int Port { get; set; } = 8989;

        public int QuietPeriodSeconds { get; set; } = 1;

        public TimeSpan QuietPeriodTimeSpan => TimeSpan.FromSeconds(QuietPeriodSeconds);

        public int ShutdownTimeoutSeconds { get; set; } = 3;

        public TimeSpan ShutdownTimeoutTimeSpan => TimeSpan.FromSeconds(ShutdownTimeoutSeconds);

        public int SoBacklog { get; set; } = 8192;
    }
}