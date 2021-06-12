#include<Ws2tcpip.h>
#pragma comment(lib, "Wsock32.lib")
#include<process.h>
#include<stdio.h>
#include <thread>
#include "json.hpp"

#define HEADERSIZE 18
using nlohmann::json;


class POS_Socket
{
	public:
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