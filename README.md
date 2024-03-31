# Create react app in docker

Create docker hub repository - publish
```
docker build -t vpd111-api . 
docker run -it --rm -p 5459:8080 --name vpd111_container vpd111-api
docker run -d --restart=always --name vpd111_container -p 5459:8080 vpd111-api
docker ps -a
docker stop vpd111_container
docker rm vpd111_container

docker images --all
docker rmi vpd111-api

docker login
docker tag vpd111-api:latest novakvova/vpd111-api:latest
docker push novakvova/vpd111-api:latest

docker pull novakvova/vpd111-api:latest
docker ps -a
docker run -d --restart=always --name vpd111_container -p 5459:8080 novakvova/vpd111-api


docker pull novakvova/vpd111-api:latest
docker images --all
docker ps -a
docker stop vpd111_container
docker rm vpd111_container
docker run -d --restart=always --name vpd111_container -p 5459:8080 novakvova/vpd111-api
```

```nginx options /etc/nginx/sites-available/default
server {
    server_name   spu111.itstep.click *.spu111.itstep.click;
    location / {
       proxy_pass         http://localhost:5459;
       proxy_http_version 1.1;
       proxy_set_header   Upgrade $http_upgrade;
       proxy_set_header   Connection keep-alive;
       proxy_set_header   Host $host;
       proxy_cache_bypass $http_upgrade;
       proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
       proxy_set_header   X-Forwarded-Proto $scheme;
    }
}

sudo systemctl restart nginx
certbot
```



