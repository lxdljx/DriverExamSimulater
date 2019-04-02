using System;
using System.Collections.Generic;
using System.Text;

namespace RedisStudy.BusinessLogic.Logger
{
    /// <summary>
    /// 日志接口
    /// </summary>
    public interface ILog
    {
        void Debug(Exception e);

        void Debug(string message);

        void Info(string message);

        void Error(Exception e);

        void Error(string message);
    }
}
