using System;
using System.Collections.Generic;
using System.Text;

namespace RedisStudy.ViewModel.EventObject
{
    public class EventObject
    {
        public string Message { get; set; }

        public EventType Type { get; set; }

        public  string  EventModelJson { get; set; }
    }

    public enum EventType
    {
        ExamBegin = 0,

        ProjectBegin = 1,

        projectEnd = 2,

        LostPoint = 3,

        ExamEnd = 4,

        AllEnd = 5
    }

}
