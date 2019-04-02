using System;
using System.Collections.Generic;
using System.Text;

namespace RedisStudy.BusinessLogic.Logger
{
    public class ConsoleLog : ILog
    {
        public void Debug(Exception e)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "发生了异常，消息为：" + e.GetBaseException().Message);
            //Console.WriteLine(e.StackTrace);
            
        }

        public void Debug(string message)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "发生了异常，消息为：" + message);
            
        }

        public void Error(Exception e)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "发生了异常，消息为：" + e.GetBaseException().Message);
            
        }

        public void Error(string message)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "发生了异常，消息为：" + message);
            
        }

        public void Info(string message)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "系统消息：" + message);
            
        }
    }
}
