﻿// <copyright file="CreateQueueRequest.cs" company="Michael Silver">
// Copyright (c) Michael Silver. All rights reserved.
// </copyright>

using System.Runtime.Serialization;

namespace NWorkQueue.Common.Models
{
    [DataContract]
    public class CreateQueueRequest
    {
        [DataMember(Order = 1)]
        public string QueueName { get; set; }
    }
}