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

## Setup       
- docker build . -t operationsmanager    
- docker run --name OperationsManager -p 9020:443 -p 8020:80 -d operationsmanager  