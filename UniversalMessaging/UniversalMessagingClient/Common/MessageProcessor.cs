using com.pcbsys.nirvana.client;
using System;

namespace UniversalMessagingClient.Common
{
    public class MessageProcessor
    {
        public void Process(nConsumeEvent consumeEvent)
        {
            byte[] buffer = consumeEvent.getEventData();
            if (buffer != null)
            {
                //Add its length to the byte counter

                var output = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                Console.WriteLine("Message Received -- :  " + output);
            }
        }

    }
}
