#include "json.hpp"
#include <string>
#include <iostream>
#include "UAV.h"
using namespace std;
using nlohmann::json;

json test() {
	json j;
	return j;
}
int main()
{
	//地面管理站有两张设计思路
	//一种是只有通信模块，没有收发机，因此它只能收数据，无法发送带有经纬度和姿态角的数据。此时，我们需要给他指定一个ID和经纬度，这后三个参数是用来初始化地面管理站的位置信息。
	//UI_Socket ui_sock(8899, 1000, "192.168.0.208", 2, 30.529247, 114.359634);//无人机通信端口，包长，电脑连接的通讯模块的IP，电脑连接的接收机的ID，监控站纬度，监控站经度
	//39.914453, 116.397201
	//第二种是包含通信模块和收发机。它能自己产生经纬度信息，因此就不需要我们给他设置。
	UI_Socket ui_sock(9060, 1000, "192.168.0.208");//无人机通信端口，包长，电脑连接的通讯模块的IP
	ui_sock.start_server();
	return ui_sock.stop_server();
	/*json j;
	j["name"] = "";
	if (j["name"] == "") cout << j << endl;
	return 0;*/
}