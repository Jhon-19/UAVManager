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
	//�������վ���������˼·
	//һ����ֻ��ͨ��ģ�飬û���շ����������ֻ�������ݣ��޷����ʹ��о�γ�Ⱥ���̬�ǵ����ݡ���ʱ��������Ҫ����ָ��һ��ID�;�γ�ȣ��������������������ʼ���������վ��λ����Ϣ��
	//UI_Socket ui_sock(8899, 1000, "192.168.0.208", 2, 30.529247, 114.359634);//���˻�ͨ�Ŷ˿ڣ��������������ӵ�ͨѶģ���IP���������ӵĽ��ջ���ID�����վγ�ȣ����վ����
	//39.914453, 116.397201
	//�ڶ����ǰ���ͨ��ģ����շ����������Լ�������γ����Ϣ����˾Ͳ���Ҫ���Ǹ������á�
	UI_Socket ui_sock(9060, 1000, "192.168.0.208");//���˻�ͨ�Ŷ˿ڣ��������������ӵ�ͨѶģ���IP
	ui_sock.start_server();
	return ui_sock.stop_server();
	/*json j;
	j["name"] = "";
	if (j["name"] == "") cout << j << endl;
	return 0;*/
}