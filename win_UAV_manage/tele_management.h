#pragma once
#include<Ws2tcpip.h>
#pragma comment(lib, "Wsock32.lib")
#include "json.hpp"
#include <string>
#define SOCK_PORT 8866
#define LOCAL_IP "192.168.0.208"
using json = nlohmann::json;
class TELE_Management
{
public:
	TELE_Management();//管理本机连接的电台，电台IP为宏定义LOCAL_IP
	TELE_Management(const char* ip_);//管理自组网中其他电台
	int teleStart();
	json getMeshConfig();
	json getMeshTopo();
	json setMeshConfig(json command);
	
private:
	const char* ip;
	struct sockaddr_in s_addr_send;
	SOCKET sockfd_send = INVALID_SOCKET;
	json tele_socket(std::string data);
	void teleClose();
};