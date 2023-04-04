# UM - .NET client
 
This is sample C# console application to implement Universal messaging subscriber.


### Running project:
 1. Download and import the project in eclipse.
 2. Modify required configuration in app.config file
 3. Run Subscriber.cs, connection details will be added to the console.
 4. Whenever a message is recieved from the topic, it will be dispalyed in console


### File details

1. App.config :- 
     Configuration file to store UM details.
  
2. Subscriber.cs :- 
      Contains main() function. 

3. SubscriberBase.cs :- 
     Contains supporting functions responsible for creating connection and subscribing to topic.
  
4. MessageProcessor.cs :-
   Process() method inside this class is invoked when a message is received.
