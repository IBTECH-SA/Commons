using System;
using System.Configuration;

namespace UniversalMessagingClient.Models
{
    public class ConnectionConfiguration
    {
        public string RNAME { get; set; }
        public string TopicName { get; set; }
        public int LogLevel { get; set; }
        public string UserName { get; set; }

        public ConnectionConfiguration()
        {
            RNAME = ConfigurationManager.AppSettings["RNAME"];
            TopicName = ConfigurationManager.AppSettings["TOPIC_NAME"];
            LogLevel = Convert.ToInt32(ConfigurationManager.AppSettings["LOG_LEVEL"]);
            UserName = ConfigurationManager.AppSettings["UM_USERNAME"];

        }
    }

}
