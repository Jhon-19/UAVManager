#define _CRT_SECURE_NO_WARNINGS
#define _WINSOCK_DEPRECATED_NO_WARNINGS
#include "UAV.h"

using namespace std;

TELE_Management::TELE_Management()
{
    ip = LOCAL_IP;
    //    set send address
    memset(&s_addr_send, 0, sizeof(s_addr_send));
    s_addr_send.sin_family = AF_INET;
    s_addr_send.sin_addr.s_addr = inet_addr(ip);  //trans addr from uint32_t host byte order to network byte order.
    s_addr_send.sin_port = htons(TELE_PORT);          //trans port from uint16_t host byte order to network byte order.
}
TELE_Management::TELE_Management(const char* ip_)
{
    ip = ip_;
    //    set send address
    memset(&s_addr_send, 0, sizeof(s_addr_send));
    s_addr_send.sin_family = AF_INET;
    s_addr_send.sin_addr.s_addr = inet_addr(ip);  //trans addr from uint32_t host byte order to network byte order.
    s_addr_send.sin_port = htons(TELE_PORT);          //trans port from uint16_t host byte order to network byte order.
}
int TELE_Management::teleStart()
{
    //int iResult;
    //WSADATA wsaData;
    //  initialize winsock
    /*iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
    if (iResult != 0) {
        printf("WSAStartup failed with error: %d\n", iResult);
        return 0;
    }*/
    //     create socket
    
    //设置超时
    int timeout = 500;
    sockfd_send = socket(AF_INET, SOCK_DGRAM, 0);  //udp
    if (SOCKET_ERROR == setsockopt(sockfd_send, SOL_SOCKET, SO_RCVTIMEO, (char*)&timeout, sizeof(int))) {
        std::cout << "set recv_timeout error!" << std::endl;
    }
    
    return 1;
}
void TELE_Management::teleClose()
{
    closesocket(sockfd_send);
    //WSACleanup();
}

json TELE_Management::getMeshConfig()
{
    string data = "{\"method\":\"getMeshConfig\"}";
    return tele_socket(data);
}
json TELE_Management::getMeshTopo()
{
    string data = "{\"method\":\"getMeshTopo\"}";
    return tele_socket(data);
}
json TELE_Management::setMeshConfig(json command)
{
    string data = command.dump();
    return tele_socket(data);
}
json TELE_Management::tele_socket(string data)
{
    int i;
    json response;
    int len = sizeof(s_addr_send);
    size_t buffer_len = 2048;
    char* data_recv = new char[buffer_len];
    memset(data_recv, 0, buffer_len);
    i = sendto(sockfd_send, data.c_str(), data.size(), 0, (struct sockaddr*)&s_addr_send, len);
    if (i < 0) {
        printf("send failed!\n");
    }
    else {
        printf("Socket_fd: %d send %s to : %s:%d\n", sockfd_send, data.c_str(),inet_ntoa(s_addr_send.sin_addr), s_addr_send.sin_port);
    }
    i = recvfrom(sockfd_send, data_recv, buffer_len, 0, (struct sockaddr*)&s_addr_send, &len);

    string recv_data = data_recv;
    if (i <= 0)
    {
        //如果超时，就返回空json
        if (i == -1) std::cout << "recv timeout!" << std::endl;
        else printf("recv failed!\n");
        delete[] data_recv;
        return response;
    }
    else
        std::cout << "From tele: " << recv_data << " from:" << inet_ntoa(s_addr_send.sin_addr) << endl;
    delete[] data_recv;
    response = json::parse(recv_data);
    return response;
}