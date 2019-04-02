using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedisStudy.BusinessLogic.Logger
{
    public class Logger : ILog
    {
        static  NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        public Logger()
        {
            
        }

        public void Debug(Exception e)
        {
            _logger.Debug(e);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Error(Exception e)
        {
            _logger.Error(e);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }
    }
}
