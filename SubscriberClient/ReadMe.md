# UM - Subscriber
 
This is sample core java project to implement Universal messaging subscriber.


### Running project:
 1. Download and import the project in eclipse.
 2. Modify required configuration in config.properties file
 3. Run Subscriber.java, connection details will be added to the console.
 4. Whenever a message is recieved from the topic, it will be dispalyed in console


### File details

1. Config.Properties :- 
     Configuration file to store UM details.
  
2. Subscriber.java :- 
      Contains main() function. 

3. SubscriberBase.java :- 
     Contains supporting functions responsible for creating connection and subscribing to  topic.
  
4. MessageProcessor.java :-
   Process() method inside this class is invoked when a message is received.
