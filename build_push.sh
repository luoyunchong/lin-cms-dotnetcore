SERVER_NAME=lincms_api
SERVER_Dockerfile=src/LinCms.Web/Dockerfile
CID=$(docker ps | grep "${SERVER_NAME}" | awk '{print $1}')
IID=$(docker images | grep "${SERVER_NAME}" | awk '{print $3}')

docker rmi ${SERVER_NAME}
docker build -f ${SERVER_Dockerfile} -t "${SERVER_NAME}":latest  .
docker tag ${SERVER_NAME} registry.cn-hangzhou.aliyuncs.com/igeekfan/${SERVER_NAME}
docker push registry.cn-hangzhou.aliyuncs.com/igeekfan/${SERVER_NAME}
docker image prune -f
