﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Naruto.Infrastructure.Exceptions
{
    /// <summary>
    /// 个人的应用程序的异常
    /// </summary>
    public class MyExceptions : ApplicationException
    {
        public MyExceptions(string message) : base(message) { }
    }
}
