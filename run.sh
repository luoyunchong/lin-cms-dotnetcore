SERVER_NAME=lincms-web

#判断是否存在webnotebook容器
docker ps | grep lincms-web &> /dev/null
#如果不存在，则Remove
if [ $? -ne 0 ]
then
    echo "${SERVER_NAME} container not exist continue.. "
else
    echo "remove ${SERVER_NAME} container"
    docker rm ${SERVER_NAME} -f
fi

docker images | grep registry.cn-hangzhou.aliyuncs.com/igeekfan/${SERVER_NAME} &> /dev/null

if [ $? -ne 0 ]
then
    echo "image does not exist , continue..."
else
    echo "image exists !!! remove it"
    docker rmi --force registry.cn-hangzhou.aliyuncs.com/igeekfan/${SERVER_NAME}
fi
#从阿里云拉取刚刚push的镜像
docker pull registry.cn-hangzhou.aliyuncs.com/igeekfan/${SERVER_NAME}

sudo docker run --restart \
unless-stopped \
-p 5011:8080 \
-v /var/www/lin-cms-dotnetcore/wwwroot/:/app/wwwroot:rw \
-v /var/www/lin-cms-dotnetcore/appsettings.Production.json/:/app/appsettings.Production.json:rw \
--privileged=true \
--name ${SERVER_NAME} \
-d registry.cn-hangzhou.aliyuncs.com/igeekfan/${SERVER_NAME}