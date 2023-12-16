start:
	docker-compose up

stop:
	docker stop zoo1 zoo2 zoo3 kafka1 kafka2 kafka3

remove:
	docker rm zoo1 zoo2 zoo3 kafka1 kafka2 kafka3
