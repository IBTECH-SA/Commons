using System;
using com.pcbsys.nirvana.client;
using UniversalMessagingClient.Common;
using UniversalMessagingClient.Models;

namespace UniversalMessagingClient
{
    public class Subscriber : SubscriberBase, nEventListener
    {

        private static System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

        private long lastEID = 0;
        private DateTime startTime;
        private long byteCount = 0;


        private int logLevel = 0;
        private int count = -1;
        private int totalMsgs = 0;
        private int reportCount = 10000;

        private nChannel myChannel;
        private static Subscriber mySelf = null;


        public static void Main(String[] args)
        {

            //Create an instance for this class
            mySelf = new Subscriber();
            var start = -1;
            var report = 1000;

            ConnectionConfiguration connectionConfiguration = new ConnectionConfiguration();

            mySelf.SubscribteToChannel(parseRealmProperties(connectionConfiguration.RNAME),
                connectionConfiguration.TopicName, null, start, connectionConfiguration.LogLevel, report);
        }

        /**
         * This method demonstrates the Nirvana API calls necessary to subscribe to
         * a channel.
         * It is called after all command line arguments have been received and
         * validated
         *
         * @param realmDetails a String[] containing the possible RNAME values
         * @param achannelName the channel name to create
         * @param selector the subscription selector filter
         * @param startEid the eid to start subscribing from
         * @param loglvl the specified log level
         * @param repCount the specified report count
         */
        private void SubscribteToChannel(String[] realmDetails, String achannelName, String sel, long start, int loglvl, int repCount)
        {
            mySelf.constructSession(realmDetails);

            logLevel = loglvl;
            reportCount = repCount;

            //Subscribes to the specified channel
            try
            {
                //Create a channel attributes object
                nChannelAttributes nca = new nChannelAttributes();
                nca.setName(achannelName);

                //Obtain the channel reference
                myChannel = mySession.findChannel(nca);

                //if the latest event has been implied (by specifying -1)
                if (start == -1)
                {
                    //Get the last eid on the channel and reset the start eid with that value
                    start = myChannel.getLastEID();
                }
                //Add this object as a subscribe to the channel with the specified message selector
                // and start eid
                myChannel.addSubscriber(this, sel, start);

                //Stay subscribed until the user presses any key
                Console.WriteLine("Press any key to quit !");
                try
                {
                    Console.Read();
                }
                catch (Exception) { } //Ignore this

                Console.WriteLine("Finished. Consumed total of " + totalMsgs);
                //Remove this subscriber
                myChannel.removeSubscriber(this);
                //Close the session we opened
                try
                {
                    nSessionFactory.close(mySession);
                }
                catch (Exception) { }
                //Close any other sessions so that we can exit
                nSessionFactory.shutdown();

            }
            catch (nSessionPausedException)
            {
                Console.WriteLine("Session has been paused, please resume the session");
                Environment.Exit(1);
            }
            catch (nSecurityException)
            {
                Console.WriteLine("Unsufficient permissions for the requested operation.");
                Console.WriteLine("Please check the ACL settings on the server.");
                Environment.Exit(1);
            }
            catch (nSessionNotConnectedException)
            {
                Console.WriteLine("The session object used is not physically connected to the Nirvana realm.");
                Console.WriteLine("Please ensure the realm is up and check your RNAME value.");
                Environment.Exit(1);
            }
            catch (nUnexpectedResponseException)
            {
                Console.WriteLine("The Nirvana REALM has returned an unexpected response.");
                Console.WriteLine("Please ensure the Nirvana REALM and client API used are compatible.");
                Environment.Exit(1);
            }
            catch (nRequestTimedOutException)
            {
                Console.WriteLine("The requested operation has timed out waiting for a response from the REALM.");
                Console.WriteLine("If this is a very busy REALM ask your administrator to increase the client timeout values.");
                Environment.Exit(1);
            }
        }


        protected override void processArgs(String[] args)
        {

        }




        /**
         * Prints the usage message for this class
         */
        private static void Usage()
        {
            Console.WriteLine("Usage ...\n");
            Console.WriteLine("subscriber <rname> <channel name> [start eid] [debug] [count] [selector] \n");

            Console.WriteLine(
              "<Required Arguments> \n");
            Console.WriteLine(
              "<rname> - the rname of the server to connect to");
            Console.WriteLine(
              "<channel name> - Channel name parameter for the channel to subscribe to");
            Console.WriteLine(
              "\n[Optional Arguments] \n");
            Console.WriteLine(
              "[start eid] - The Event ID to start subscribing from");
            Console.WriteLine(
              "[debug] - The level of output from each event, 0 - none, 1 - summary, 2 - EIDs, 3 - All");
            Console.WriteLine(
              "[count] - The number of events to wait before printing out summary information");
            Console.WriteLine(
              "[selector] - The event filter string to use");
        }

        /**
         * A callback is received by the API to this method each time an event is received from
         * the nirvana channel. Be carefull not to spend too much time processing the message
         * inside this method, as until it exits the next message can not be pushed.
         *
         * @param evt An nConsumeEvent object containing the message received from the channel
         */
        public void go(nConsumeEvent evt)
        {
            ProcessReportAndLogging(evt);

            MessageProcessor messageProcessor = new MessageProcessor();
            messageProcessor.Process(evt);
        }

        public void ProcessReportAndLogging(nConsumeEvent evt)
        {

            MessageProcessor messageProcessor = new MessageProcessor();
            //If this is the first message we receive
            if (count == -1)
            {
                //Get a timestamp to be used for message rate calculations
                startTime = DateTime.Now;
                count = 0;
            }

            //Increment he counter
            count++;
            totalMsgs++;

            //Have we reached the point where we need to report the rates?
            if (count == reportCount)
            {

                //Reset the counter
                count = 0;

                //Get a timestampt to calculate the rates
                DateTime end = DateTime.Now;

                // Does the specified log level permits us to print on the screen?
                if (logLevel >= 1)
                {

                    //Dump the rates on the screen
                    if (!end.Equals(startTime))
                    {
                        TimeSpan ts = end - startTime;
                        double milli = ts.TotalMilliseconds;
                        Console.WriteLine("Received " + reportCount + " in " + (milli) + " Evt/Sec = " + ((reportCount * 1000) / (milli)) + " Bytes/sec=" + ((byteCount * 1000) / (milli)));
                        Console.WriteLine("Bandwidth data : Bytes Tx [" + mySession.getOutputByteCount() + "] Bytes Rx [" + mySession.getInputByteCount() + "]");
                    }
                    else
                    {
                        Console.WriteLine("Received " + reportCount + " faster than the system millisecond counter");
                    }
                }
                //Set the startTime for the next report equal to the end timestamp for the previous one
                startTime = end;

                //Reset the byte counter
                byteCount = 0;
            }

            //If the last EID counter is not equal to the current event ID
            if (lastEID != evt.getEventID())
            {
                //If yes, maybe we have missed an event, so print a message on the screen.
                //This message could be printed for a number of other reasons.
                //One of them would be someone purging a range creating an 'eid gap'.
                //As eids are never reused within a channel you could have a situation
                //where this gets printed but nothing is missed.
                Console.WriteLine("Expired event range " + (lastEID) + " - " + (evt.getEventID() - 1));
                //Reset the last eid counter
                lastEID = evt.getEventID() + 1;
            }
            else
            {

                //Increment the last eid counter
                lastEID++;
            }



            //Get the data of the message
            byte[] buffer = evt.getEventData();
            if (buffer != null)
            {
                //Add its length to the byte counter
                byteCount += buffer.Length;
            }
            //If the loglevel permits printing on the screen
            if (logLevel >= 2)
            {
                //Print the eid
                Console.WriteLine("Event id : " + evt.getEventID());
                if (evt.isEndOfChannel())
                {
                    Console.WriteLine("End of channel reached");
                }
                //If the loglevel permits printing on the screen
                if (logLevel >= 3)
                {
                    //Print the message tag
                    Console.WriteLine("Event tag : " + evt.getEventTag());
                    //Print the message data
                    Console.WriteLine("Event data : " + encoding.GetString(evt.getEventData()));
                    if (evt.hasAttributes())
                    {
                        displayEventAttributes(evt.getAttributes());
                    }

                    nEventProperties prop = evt.getProperties();
                    if (prop != null)
                    {
                        displayEventProperties(prop);
                    }
                }
            }
        }


    }
}