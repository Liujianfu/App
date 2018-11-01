﻿using System;

namespace CareVisit.Core.Exceptions
{
    public class ServiceAuthenticationException : Exception
    {
        public string Content { get; }

        public ServiceAuthenticationException()
        {

        }

        public ServiceAuthenticationException(string content)
        {
            Content = content;
        }

    }
}