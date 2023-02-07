# Communication Module  
Contains the Broker and the Broker Producer API that connects with it  

Branch Naming [Here](https://github.com/Aphluentia/Documentation/tree/dev)

Two main folders:  
- *Broker/*: Contains the Kafka Broker definition   
	- The Cluster is composed of 3 zookeepers and 3 kafka brokers  
	- docker-compose generates the cluster available at ports 8005, 8006 and 8007  
	- Makefile to start, stop and remove containers  
	- mock_consumer & mock_producer to simulate&test  
	- Deployed and Accessible at 89.114.83.106, through ports 85, 86 and 86  

- *CommunicationAPI/*: Contains the Kafka Producer definition for allowing communication     
	- 3 Main Endpoint Groups:	
		- Base: Includes Api and Broker heartbeat, cluster details and logs   
		- Pair: Pairing endpoints include new connection, inform accepted connection, disconnect pairing, ping  
		- Topics: broker topic management (listing and deletion)     
	- Dockerfile to start the api in a docker container commsApi at port 8008  
	- Makefile to start, stop and remove container  
	- Deployed at 89.114.83.106 and available at port 88,  [Swagger](http://89.114.83.106/88/docs) (!Unsecure Connection)
