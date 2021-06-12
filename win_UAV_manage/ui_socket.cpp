#define _CRT_SECURE_NO_WARNINGS
#define _WINSOCK_DEPRECATED_NO_WARNINGS
#include "UAV.h"

UI_Socket::UI_Socket(int port_, int packet_size_, const char* managerTeleIp, int managerTeleId, double x, double y)
{
	thread_receive_flag = 0;
    pos_sock = new POS_Socket(port_, packet_size_,managerTeleId, x, y);
    main_tele_manager = new TELE_Management(managerTeleIp);
    //    set receive address
    memset(&s_addr_receive, 0, sizeof(s_addr_receive));
    s_addr_receive.sin_family = AF_INET;
    s_addr_receive.sin_addr.s_addr = htonl(INADDR_ANY);  //trans addr from uint32_t host byte order to network byte order.
    s_addr_receive.sin_port = htons(UI_PORT);          //trans port from uint16_t host byte order to network byte order.
}
UI_Socket::UI_Socket(int port_, int packet_size_, const char* managerTeleIp)
{
    thread_receive_flag = 0;
    pos_sock = new POS_Socket(port_, packet_size_);
    main_tele_manager = new TELE_Management(managerTeleIp);
    //    set receive address
    memset(&s_addr_receive, 0, sizeof(s_addr_receive));
    s_addr_receive.sin_family = AF_INET;
    s_addr_receive.sin_addr.s_addr = htonl(INADDR_ANY);  //trans addr from uint32_t host byte order to network byte order.
    s_addr_receive.sin_port = htons(UI_PORT);          //trans port from uint16_t host byte order to network byte order.
}
int UI_Socket::start_server()
{
	int iResult, fd_temp;
	WSADATA wsaData;

    //  initialize winsock
    iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
    if (iResult != 0) {
        printf("WSAStartup failed with error: %d\n", iResult);
        return 0;
    }
    //���������վsocket
    main_tele_manager->teleStart();
    //�������˻�λ����̬�ɼ��߳�  
    iResult = pos_sock->start_collection();
    if (iResult == 0) {
        printf("pos_sock starting failed!\n");
        return 0;
    }
    //     create socket
    sockfd_listen = socket(AF_INET, SOCK_DGRAM, 0);  //udp

    //    bind server listening port
    fd_temp = bind(sockfd_listen, (struct sockaddr*)&s_addr_receive, sizeof(s_addr_receive));
    if (fd_temp == -1)
    {
        fprintf(stderr, "bind error!\n");
        return 0;
    }
    else
    {
        printf("UI socket addr:%s, port:%d is listening!\n", inet_ntoa(s_addr_receive.sin_addr), ntohs(s_addr_receive.sin_port));
    }
    thread_receive_flag = 1;
    std::thread t(&UI_Socket::listening_handle,this);
    std::cout << "haha" << std::endl;

    t.join();
    return 1;
}
int UI_Socket::stop_server() {
    pos_sock->stop_collection();
    main_tele_manager->teleClose();
    delete main_tele_manager;
    delete pos_sock;
    thread_receive_flag = 0;
    closesocket(sockfd_listen);
    WSACleanup();
    return 1;
}

json UI_Socket::getTopo()
{
    json meshTopo;//��������
    json posTopo;//��������
    json totalTopo;//������
    //�������վ��̨��socket

    //��ȡ��������
    meshTopo = main_tele_manager->getMeshTopo();
    //��������������Ϣ
    //�������Ϊ�գ���������ؿ�json
    if (!meshTopo.is_null()) {
        if (meshTopo["nodeInfos"].size() == 0) return totalTopo;
        for (int i = 0; i < meshTopo["nodeInfos"].size(); i++) {
            Nodeinfo info = meshTopo["nodeInfos"][i];
            if (info.noise == "unknown" || info.name=="") return totalTopo;
            std::vector<std::string> vc;
            split(info.name, vc, '-');
            id_ip[std::to_string(std::stoi(vc.back()))] = info.ip;
            totalTopo["link_serial"].emplace_back(std::stoi(vc.back()));
            for (int j = 0; j < meshTopo["nodeInfos"].size(); j++)
                meshTopo["linkQuality"][i][j] = meshTopo["linkQuality"][i][j] - std::stoi(info.noise);
        }
    }
    else return totalTopo;
    //��ȡ��������
    posTopo = pos_sock->getPistions();
    //��������������Ϣ
    if (!posTopo.is_null()) {
        for (auto& el : posTopo.items()) {
            json temp = { {"id",el.value()[0]},{"latitude",el.value()[1]}, {"longitude",el.value()[2]},
                {"altitude",el.value()[3]}, {"yaw",el.value()[4]}, {"pitch",el.value()[5]}, {"roll",el.value()[6]} };
            totalTopo["node"].emplace_back(temp);
        }
    }
    //�����������Ϊnull������ӿհ���Ϣ
    else totalTopo["node"].emplace_back();
    //����������
    totalTopo["link"] = meshTopo["linkQuality"];  
    totalTopo["method"] = "nodetopo";
    return totalTopo;
}
json UI_Socket::getMeshConfig(int id_) {
    json rawMeshConfig;//ԭʼ��̨json����
    json meshConfig;//���ظ�UI��json����

    //����id_ipӳ����ȡĿ���̨ip
    std::string id = std::to_string(id_);
    std::string ip;
    ip = id_ip.at(id);
    //����Ŀ���̨��socket
    TELE_Management tele_manager(ip.c_str());
    if (!tele_manager.teleStart()) {
        std::cout << "tele_manager starting failed!" << std::endl;
    }
    //��ȡ��̨ԭʼjson����
    rawMeshConfig = tele_manager.getMeshConfig();
    tele_manager.teleClose();
    if (rawMeshConfig.is_null()) return rawMeshConfig;
    //��ԭʼjson���ݵ�ֵ��������
    std::vector<std::string> vc;
    split(rawMeshConfig["meshName"], vc, '-');
    std::string name = vc.back();
    std::string channel = rawMeshConfig["meshFreq"];
    std::string power = rawMeshConfig["meshPower"];
    std::string bandwidth = rawMeshConfig["meshSpan"];
    std::string distance = rawMeshConfig["meshRangeMode"];

    //��дUI��Ҫ��json����
    meshConfig["method"] = "devconfig";
    meshConfig["id"] = id_;
    //meshConfig["id"] = std::stoi(name);
    meshConfig["channel"] = std::stoi(channel);
    meshConfig["power"] = std::stoi(power);
    meshConfig["bandwidth"] = std::stoi(bandwidth);
    meshConfig["distance"] = std::stoi(distance);
    meshConfig["ip"] = rawMeshConfig["ip"];
    meshConfig["mask"] = rawMeshConfig["nwMask"];
    meshConfig["gateway"] = rawMeshConfig["gateway"];
    meshConfig["dns"] = rawMeshConfig["dnsServer"];
    
    return meshConfig;
}
json UI_Socket::setMeshConfig(int id_,json settings) {
    json response;//���ظ�UI�Ľ��
    //ͨ��id�ҵ�ip
    std::string id = std::to_string(id_);
    std::string ip;
    ip = id_ip.at(id);
    //�������̨֮���socket
    TELE_Management tele_manager(ip.c_str());
    if (!tele_manager.teleStart()) {
        std::cout << "tele_manager starting failed!" << std::endl;
    }
    //�ֱ��ÿ�����ö�����һ��
    for (auto& setting : settings.items()) {
        json temp;
        temp["method"] = "setMeshConfig";
        if (setting.key() == "channel")  temp["meshFreq"] = std::to_string((int)setting.value());
        else if (setting.key() == "power")  temp["meshPower"] = std::to_string((int)setting.value());
        else if (setting.key() == "bandwidth")  temp["meshSpan"] = std::to_string((int)setting.value());
        else if (setting.key() == "distance")  temp["meshRangeMode"] = std::to_string((int)setting.value());
        else if (setting.key() == "ip")  temp["ip"] = setting.value();
        else if (setting.key() == "mask")  temp["nwMask"] = setting.value();
        else if (setting.key() == "gateway")  temp["gateway"] = setting.value();
        else if (setting.key() == "dns")  temp["dnsServer"] = setting.value();
        else continue;
        /*else if (setting.key() == "id") {
            std::string id_temp = setting.value();
            temp["meshName"] = "UAV-" + id_temp;
            id_ip[id_temp] = ip;
            id_ip.erase(id);
        }*/
        response = tele_manager.setMeshConfig(temp);
    }
    //�ر�socket
    tele_manager.teleClose();
    return response;
}
void UI_Socket::listening_handle() {

    int i_recvBytes;
    //����UI����Ļ�����
    size_t buffer_len = 2048;
    char* data_recv = new char[buffer_len];
    struct sockaddr_in s_addr_client;
    int client_len = sizeof(s_addr_client);

    while (this->thread_receive_flag)
    {
        json response;//��Ӧ��UI�������Ӧ
        memset(data_recv, 0, buffer_len);

        //����UI����
        i_recvBytes = recvfrom(this->sockfd_listen, data_recv, buffer_len, 0, (struct sockaddr*)&s_addr_client, &client_len);
        if (i_recvBytes <= 0)
        {
            std::cout << i_recvBytes << std::endl;
            printf("UI server recv failed!\n");
            continue;
        }
        //������ת��Ϊstring
        std::string recv_data = data_recv;
        //��stringת��Ϊjson
        json recv_json = json::parse(recv_data);
        std::cout << "From UI: "<<recv_json << std::endl;

        //�����������͵�����ӦAPI
        if (recv_json["method"] == "devconfig") response = getMeshConfig(recv_json["id"]);//��ȡ��̨������Ϣ
        else if (recv_json["method"] == "nodetopo") response = getTopo();//��ȡ���ˣ������������˺���������
        else if (recv_json["method"] == "SetConfig") {//���õ�̨����
            int id = recv_json["id"];
            recv_json.erase("id");
            recv_json.erase("method");
            response = setMeshConfig(id, recv_json);
        }
        if (response.is_null()) continue;
        std::string response_s = response.dump();//����Ӧת��Ϊ�ַ���
        //������Ӧ��UI
        i_recvBytes = sendto(this->sockfd_listen, response_s.c_str(), response_s.size(), 0, (struct sockaddr*)&s_addr_client, client_len);
        if (i_recvBytes <= 0)
        {
            printf("UI server response failed!\n");
        }
        else
            std::cout << "To UI: " << response << std::endl;
    }
    delete[] data_recv;
}
//�ַ����ָ��
void split(const std::string& s, std::vector<std::string>& tokens, char delim = ' ') {
	tokens.clear();
	auto string_find_first_not = [s, delim](size_t pos = 0) -> size_t {
		for (size_t i = pos; i < s.size(); i++) {
			if (s[i] != delim) return i;
		}
		return std::string::npos;
	};
	size_t lastPos = string_find_first_not(0);
	size_t pos = s.find(delim, lastPos);
	while (lastPos != std::string::npos) {
		tokens.emplace_back(s.substr(lastPos, pos - lastPos));
		lastPos = string_find_first_not(pos);
		pos = s.find(delim, lastPos);
	}
}