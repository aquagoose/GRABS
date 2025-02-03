﻿using grabs;
using grabs.Core;

GrabsLog.LogMessage += (severity, source, message, _, _) => Console.WriteLine($"{severity} - {source}: {message}"); 

InstanceInfo info = new InstanceInfo(Backend.Vulkan, "grabs.Tests", true);

Instance instance = Instance.Create(info);

instance.Dispose();