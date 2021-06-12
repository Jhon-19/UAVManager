#include<Ws2tcpip.h>
#pragma comment(lib, "Wsock32.lib")
#include<process.h>
#include<stdio.h>
#include <iostream>
#include <sstream>
#include <thread>
#include <string>
#include "json.hpp"
#define TELE_PORT 8866
#define UI_PORT 10000
#define LOCAL_IP "192.168.0.208"
#define HEADERSIZE 18

using json = nlohmann::json;

void split(const std::string& s, std::vector<std::string>& tokens, char delim);

struct LC_result
{
	int gpsWeek;
	double gpsSecond;
	double rGNSS[3];
	double rRms;
	double vel[3];
	double att[3];
	bool   bGpsFlag;
	double difTime;
	LC_result()
	{
		gpsWeek = 0;
		gpsSecond = 0;
		rRms = 0;
		bGpsFlag = 0;
		difTime = 0;
		for (int i = 0; i < 3; i++)
		{
			rGNSS[i] = 0;
			vel[i] = 0;
			att[i] = 0;
		}
	}
};
struct Nodeinfo {
	std::string name;
	std::string ip;
	std::string noise;
};
//struct Posinfo {
//	std::string id;
//	std::string pos_x;
//	std::string pos_y;
//	std::string pos_z;
//	std::string yaw;
//	std::string pitch;
//	std::string roll;
//};
//inline void to_json(json& j, const Posinfo& info) {
//	j = json{
//		{"id",info.id},
//		{"pos_x",info.pos_x},
//		{"pos_y",info.pos_y},
//		{"pos_z",info.pos_z},
//		{"yaw",info.yaw},
//		{"pitch",info.pitch},
//		{"roll",info.roll}
//	};
//}
//inline void from_json(const json& j, Posinfo& info) {
//	j.at("id").get_to(info.id);
//	j.at("pos_x").get_to(info.pos_x);
//	j.at("pos_y").get_to(info.pos_y);
//	j.at("pos_z").get_to(info.pos_z);
//	j.at("yaw").get_to(info.yaw);
//	j.at("pitch").get_to(info.pitch);
//	j.at("yaw").get_to(info.roll);
//}
inline void to_json(json& j, const Nodeinfo& info) {
	j = json{
		{"name",info.name},
		{"ip",info.ip},
		{"noise",info.noise},
	};
}
inline void from_json(const json& j, Nodeinfo& info) {
	j.at("name").get_to(info.name);
	j.at("ip").get_to(info.ip);
	j.at("noise").get_to(info.noise);
}

class POS_Socket
{
public:
	POS_Socket(int port_, int packet_size_, int managerTeleId, double x, double y);
	POS_Socket(int port_, int packet_size_);
	int start_collection();
	int stop_collection();
	json getPistions();

private:
	int port;
	int packet_size;
	json positions;

	struct sockaddr_in s_addr_receive;
	SOCKET sockfd_listen = INVALID_SOCKET;

	int thread_receive_flag;

	void listening_handle();

};
class TELE_Management
{
public:
	TELE_Management();//管理本机连接的电台，电台IP为宏定义LOCAL_IP
	TELE_Management(const char* ip_);//管理自组网中其他电台
	int teleStart();
	void teleClose();
	json getMeshConfig();
	json getMeshTopo();
	json setMeshConfig(json command);

private:
	const char* ip;
	struct sockaddr_in s_addr_send;
	SOCKET sockfd_send = INVALID_SOCKET;
	json tele_socket(std::string data);
};
class UI_Socket
{
public:
	UI_Socket(int port_, int packet_size_,const char* managerTeleIp);
	UI_Socket(int port_, int packet_size_, const char* managerTeleIp, int managerTeleId, double x, double y);
	int start_server();
	int stop_server();

private:
	json id_ip;
	struct sockaddr_in s_addr_receive;
	SOCKET sockfd_listen = INVALID_SOCKET;
	POS_Socket *pos_sock;
	TELE_Management *main_tele_manager;

	int thread_receive_flag;
	void listening_handle();
	json getTopo();
	json getMeshConfig(int id_);
	json setMeshConfig(int id_, json settings);

};



