using System;
using System.Collections.Generic;
using com.pcbsys.nirvana.client;

namespace UniversalMessagingClient
{

    /**
     * Base class that contains standard functionality for a nirvana sample app
     */
    public class SubscriberBase : nReconnectHandler, nAsyncExceptionListener
    {

        protected nSession mySession = null;
        protected nSessionAttributes nsa = null;
        private String myLastSessionID;
        private static System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

        protected virtual void processArgs(String[] args) { }

        protected static void processEnvironmentVariables()
        {
            //Process Environment Variables
        }

        /**
         * This method processes a string consisting of one or more comma separated
         * RNAME values and splits them into a a String[]
         *
         * @param realmdetails The RNAME of the Nirvana realm
         */
        protected static String[] parseRealmProperties(String realmdetails)
        {
            String[] rproperties = new String[4];
            int idx = 0;
            String[] st = realmdetails.Split(',');
            for (int x = 0; x < st.Length; x++)
            {
                String someRNAME = (String)st[x];
                rproperties[idx] = someRNAME;
                idx++;
            }
            //Trim the array
            String[] rpropertiesTrimmed = new String[idx];
            System.Array.Copy(rproperties, 0, rpropertiesTrimmed, 0, idx);
            return rpropertiesTrimmed;
        }

        /**
         * This method demonstrates the Nirvana API calls necessary to construct a
         * nirvana session
         *
         * @param realmDetails a String[] containing the possible RNAME values
         */
        protected void constructSession(String[] realmDetails)
        {

            //Create a realm session attributes object from the array of strings
            try
            {
                nsa = new nSessionAttributes(realmDetails, 2);
            }
            catch (Exception)
            {
                Console.WriteLine(
                    "Error creating Session Attributes. Please check your RNAME");
            }

            //Add this class as an asynchronous exception listener
            try
            {
                //Create a session object from the session attributes object, passing this
                //as a reconnect handler class (optional). This will ensure that the reconnection
                //methods will get called by the API.
                mySession = nSessionFactory.create(nsa, this);
                mySession.addAsyncExceptionListener(this);
            }
            catch (nIllegalArgumentException) { }

            //Initialise the Nirvana session. This physically opens the connection to the
            //Nirvana realm, using the specified protocols. If multiple interfaces are supported
            //these will be attempted in weight order (SSL, HTTPS, socket, HTTP).
            try
            {
                nConstants.setClientLogLevel(1);


                mySession.init();


                myLastSessionID = mySession.getId();
                //mySession.updateConnectionListWithServerList();
            }
            //Handle errors
            catch (nSecurityException sec)
            {
                Console.WriteLine("The current user is not authorised to connect to the specified Realm Server");
                Console.WriteLine("Please check the realm acls or contact support");
                Console.WriteLine(sec.StackTrace);
                Environment.Exit(1);
            }
            catch (nRealmUnreachableException)
            {
                Console.WriteLine(
                    "The Nirvana Realm specified by the RNAME value is not reachable.");
                Console.WriteLine(
                    "Please ensure the Realm is running and check your RNAME value.");
                Environment.Exit(1);
            }
            catch (nSessionNotConnectedException)
            {
                Console.WriteLine(
                    "The session object used is not physically connected to the Nirvana Realm.");
                Console.WriteLine(
                    "Please ensure the Realm is up and check your RNAME value.");
                Environment.Exit(1);
            }
            catch (nSessionAlreadyInitialisedException)
            {
                Console.WriteLine("The session object has already been initialised.");
                Console.WriteLine("Please make only one call to the .init() function.");
                Environment.Exit(1);
            }
        }

        /**
         * A callback is received by the API to this method to notify the user of a disconnection
         * from the realm. The method is enforced by the nReconnectHandler interface but is normally optional.
         * It gives the user a chance to log the disconnection or do something else about it.
         *
         * @param anSession The Nirvana session being disconnected
         */
        public void disconnected(nSession anSession)
        {
            try
            {
                Console.WriteLine("You have been disconnected from " + myLastSessionID);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while disconnecting " + ex.Message);
            }
        }

        /**
         * A callback is received by the API to this method to notify the user of a successful reconnection
         * to the realm. The method is enforced by the nReconnectHandler interface but is normally optional.
         * It gives the user a chance to log the reconnection or do something else about it.
         *
         * @param anSession The Nirvana session being reconnected
         */
        public void reconnected(nSession anSession)
        {
            try
            {
                myLastSessionID = mySession.getId();
                Console.WriteLine("You have been Reconnected to " + myLastSessionID);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while reconnecting " + ex.Message);
            }
        }

        /**
         * A callback is received by the API to this method to notify the user that the API is about
         * to attempt reconnecting to the realm. The method is enforced by the nReconnectHandler
         * interface but is normally optional. It allows the user to decide whether further
         * attempts are required or not, whether custom delays should be enforced etc.
         *
         * @param anSession The Nirvana session that will be used to reconnect
         */
        public bool tryAgain(nSession anSession)
        {
            try
            {
                Console.WriteLine("Attempting to reconnect to ");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while trying to reconnect " + ex.Message);
            }
            return true;
        }

        /**
         * A callback is received by the API to this method to notify the user that the an
         * asynchronous exception (in a thread different than the current one) has occured.
         *
         * @param ex The asynchronous exception that was thrown
         */
        public void handleException(nBaseClientException ex)
        {
            Console.WriteLine("An Asynchronous Exception was received from the Nirvana realm.");
        }

        protected static void UsageEnv()
        {
            Console.WriteLine("\n\n(Environment Variables) \n");

            Console.WriteLine("(RNAME) - One or more RNAME entries in the form protocol://host:port");
            Console.WriteLine("   protocol - Can be one of nsp, nhp, nsps, or nhps, where:");
            Console.WriteLine("   nsp - Specifies Nirvana Socket Protocol (nsp)");
            Console.WriteLine("   nhp - Specifies Nirvana HTTP Protocol (nhp)");
            Console.WriteLine("   nsps - Specifies Nirvana Socket Protocol Secure (nsps), i.e. using SSL/TLS");
            Console.WriteLine("   nhps - Specifies Nirvana HTTP Protocol Secure (nhps), i.e. using SSL/TLS");
            Console.WriteLine("   port - The port number of the server");
            Console.WriteLine("\nHint: - For multiple RNAME entries, use comma separated values which will be attempted in connection weight order\n");
            Console.WriteLine("(LOGLEVEL) - This determines how much information the nirvana api will output 0 = verbose 7 = quiet\n");
            Environment.Exit(1);
        }

        protected void displayEventProperties(nEventProperties prop)
        {
            Console.WriteLine("Event Prop : ");
            IEnumerator<String> keys = prop.getKeys();
            keys.Reset();
            while (keys.MoveNext())
            {
                Object key = keys.Current;
                Object value = prop.get(key.ToString());
                if (value is nEventProperties)
                {
                    nEventProperties pvalue = (nEventProperties)value;
                    Console.WriteLine("[" + key + "(event prop)]:");
                    displayEventProperties(pvalue);
                }
                else if (value is nEventProperties[])
                {
                    nEventProperties[] pvalue = (nEventProperties[])value;
                    Console.WriteLine("[" + key + "(event prop[])]:");
                    for (int x = 0; x < pvalue.Length; x++)
                    {
                        displayEventProperties(pvalue[x]);
                    }
                }
                else if (value is String[])
                {
                    String[] pvalue = (String[])value;
                    Console.WriteLine("[" + key + "(String[])]:");
                    for (int x = 0; x < pvalue.Length; x++)
                    {
                        Console.WriteLine("   [" + key + "]:[" + x + "]=" + pvalue[x]);
                    }
                }
                else if (value is long[])
                {
                    long[] pvalue = (long[])value;
                    Console.WriteLine("[" + key + "(long[])]:");
                    for (int x = 0; x < pvalue.Length; x++)
                    {
                        outputKeyVal(key, pvalue[x], x);
                    }
                }
                else if (value is int[])
                {
                    int[] pvalue = (int[])value;
                    Console.WriteLine("[" + key + "(int[])]:");
                    for (int x = 0; x < pvalue.Length; x++)
                    {
                        outputKeyVal(key, pvalue[x], x);
                    }
                }
                else if (value is byte[])
                {
                    byte[] pvalue = (byte[])value;
                    Console.WriteLine("[" + key + "(byte[])]:");
                    for (int x = 0; x < pvalue.Length; x++)
                    {
                        outputKeyVal(key, pvalue[x], x);
                    }
                }
                else if (value is bool[])
                {
                    bool[] pvalue = (bool[])value;
                    Console.WriteLine("[" + key + "(bool[])]:");
                    for (int x = 0; x < pvalue.Length; x++)
                    {
                        outputKeyVal(key, pvalue[x], x);
                    }
                }
                else if (value is double[])
                {
                    double[] pvalue = (double[])value;
                    Console.WriteLine("[" + key + "(double[])]:");
                    for (int x = 0; x < pvalue.Length; x++)
                    {
                        outputKeyVal(key, pvalue[x], x);
                    }
                }
                else
                {
                    outputKeyVal(key, value);
                }
            }
        }

        private void outputKeyVal(object key, object val)
        {
            outputKeyVal(key, val, -1);
        }
        private void outputKeyVal(object key, object val, int idx)
        {
            if (idx == -1)
            {
                Console.WriteLine("   [" + key + "]\t=\t" + val);
            }
            else
            {
                Console.WriteLine("   [" + key + "]:[" + idx + "]\t=\t" + val);
            }
        }

        protected void displayEventAttributes(nEventAttributes attributes)
        {
            if (attributes.getApplicationId() != null)
                Console.WriteLine("Application Id : " + encoding.GetString(attributes.getApplicationId()));
            if (attributes.getCorrelationId() != null)
                Console.WriteLine("Correlation Id : " + encoding.GetString(attributes.getCorrelationId()));
            if (attributes.getMessageId() != null)
                Console.WriteLine("Message Id     : " + encoding.GetString(attributes.getMessageId()));
            if (attributes.getPublisherHost() != null)
                Console.WriteLine("Published from : " + encoding.GetString(attributes.getPublisherHost()));
            if (attributes.getPublisherName() != null)
                Console.WriteLine("Published by   : " + encoding.GetString(attributes.getPublisherName()));
            DateTime date = Jan1st1970.AddMilliseconds(attributes.getTimestamp());
            if (attributes.getTimestamp() != 0)
                Console.WriteLine("Published on   : " + date.ToString());
        }

        private static readonly DateTime Jan1st1970 = new DateTime
            (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }

        //public static void Main(String[] args)
        //{
        //    SubscriberBase subscriberBase = new SubscriberBase();
        //    subscriberBase.constructSession(args[0].Split(','));
        //    subscriberBase.mySession.init();
        //}
    }
}

