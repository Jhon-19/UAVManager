#define _CRT_SECURE_NO_WARNINGS
#define _WINSOCK_DEPRECATED_NO_WARNINGS
#include "UAV.h"

#define PI          3.1415926535897932  /* pi */
#define D2R         (PI/180.0)          /* deg to rad */
#define R2D         (180.0/PI)          /* rad to deg */
#define RE_WGS84    6378137.0           /* earth semimajor axis (WGS84) (m) */
#define FE_WGS84    (1.0/298.257223563) /* earth flattening (WGS84) */
#define  POLY24 0x1864cfbu

#define SKIPBITS(b) { LOADBITS(b) numbits -= (b); }

#define LOADBITS(a) \
{\
while ((a) > numbits) \
{ \
if (!size--) break; \
        bitfield = (bitfield << 8) | *(data++); \
        numbits += 8; \
} \
}

/* extract bits from data stream
b = variable to store result, a = number of bits */
#define GETBITS(b, a) \
{ \
        LOADBITS(a) \
        b = (bitfield<<(64-numbits))>>(64-(a)); \
        b = b&((1<<a)-1); \
        numbits -= (a); \
}

/* extract bits from data stream
b = variable to store result, a = number of bits */
#define GETBITSSIGN(b, a) \
{ \
        LOADBITS(a) \
        b = ((__int64)(bitfield<<(64-numbits)))>>(64-(a)); \
    numbits -= (a); \
}

/* extract bits from data stream
b = variable to store result, a = number of bits */
#define GETBITSFACTOR(b, a, c) \
{ \
        LOADBITS(a) \
        b = ((bitfield<<(64-numbits))>>(64-(a)))*(c); \
        numbits -= (a); \
}

/* extract signed floating value from data stream
b = variable to store result, a = number of bits */
#define GETFLOATSIGN(b, a, c) \
{ \
        LOADBITS(a) \
        b = ((double)(((__int64)(bitfield<<(64-numbits)))>>(64-(a))))*(c); \
        numbits -= (a); \
}

/* extract floating value from data stream
b = variable to store result, a = number of bits, c = scale factor */
#define GETFLOAT(b, a, c) \
{ \
        LOADBITS(a) \
        b = ((double)((bitfield<<(sizeof(bitfield)*8-numbits))>>(sizeof(bitfield)*8-(a))))*(c); \
        numbits -= (a); \
}
void crcInit(unsigned long table[256]);
int crcUpdate(unsigned char pUBuffer[], unsigned long len, unsigned char crcChar[]);
int LcResultDecode(unsigned char pUBuffer[], unsigned long nChar, LC_result& stPos);
extern void ecef2pos(const double* r, double* pos);
extern double dot(const double* a, const double* b, int n);
bool outOfChina(double lat, double lon);
double transformLat(double x, double y);
double transformLon(double x, double y);
void transform2Mars(double wgLat, double wgLon, double& mgLat, double& mgLon);

POS_Socket::POS_Socket(int port_, int packet_size_,int managerTeleId, double x, double y)
{
	port = port_;
    packet_size = packet_size_;
    thread_receive_flag = 0;
    //    set receive address
    memset(&s_addr_receive, 0, sizeof(s_addr_receive));
    s_addr_receive.sin_family = AF_INET;
    s_addr_receive.sin_addr.s_addr = htonl(INADDR_ANY);  //trans addr from uint32_t host byte order to network byte order.
    s_addr_receive.sin_port = htons(port);          //trans port from uint16_t host byte order to network byte order.
    double marsLat, marsLon;
    transform2Mars(x, y, marsLat, marsLon);
    positions[std::to_string(managerTeleId)] = { managerTeleId, marsLat, marsLon, 0, 0, 0, 0};
    //positions["2"] = { 2,30.51795, 114.345697, 0, 0, 0, 0 };
    //positions["3"] = { 3,30.54569, 114.315789, 0, 0, 0, 0 };
    //positions["4"] = { 4,30.53478, 114.331024, 0, 0, 0, 0 };
    //positions["5"] = { 5,30.57894, 114.351354, 0, 0, 0, 0 };
    //positions["6"] = { 6,30.55641, 114.359778, 0, 0, 0, 0 };

}
POS_Socket::POS_Socket(int port_, int packet_size_)
{
    port = port_;
    packet_size = packet_size_;
    thread_receive_flag = 0;
    //    set receive address
    memset(&s_addr_receive, 0, sizeof(s_addr_receive));
    s_addr_receive.sin_family = AF_INET;
    s_addr_receive.sin_addr.s_addr = htonl(INADDR_ANY);  //trans addr from uint32_t host byte order to network byte order.
    s_addr_receive.sin_port = htons(port);          //trans port from uint16_t host byte order to network byte order.
}

int POS_Socket::start_collection()
{
    int iResult, fd_temp;
    WSADATA wsaData;
    thread_receive_flag = 1;

    //  initialize winsock
    //iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
    //if (iResult != 0) {
    //    printf("WSAStartup failed with error: %d\n", iResult);
    //    return 1;
    //}
    //     create socket
    sockfd_listen = socket(AF_INET, SOCK_DGRAM, 0);  //udp

    //    set socket opt
    int opt = 1;
    int ttl = 1;
    fd_temp = setsockopt(sockfd_listen, SOL_SOCKET, SO_BROADCAST, (char*)&opt, sizeof(opt));
    if (fd_temp == -1)
    {
        printf("listen fd set broadcast opt error!\n");
        return 0;
    }
    
    //    bind server listening port
    fd_temp = bind(sockfd_listen, (struct sockaddr*)&s_addr_receive, sizeof(s_addr_receive));
    if (fd_temp == -1)
    {
        printf("bind error!\n");
        return 0;
    }
    else
    {
        printf("postion socket addr:%s, port:%d is listening!\n", inet_ntoa(s_addr_receive.sin_addr), ntohs(s_addr_receive.sin_port));
    }
    std::thread t(&POS_Socket::listening_handle,this);
    t.detach();
    return 1;
}

void POS_Socket::listening_handle() {

    int id;
    int serial_id;
    int len;
    unsigned int serial;
    double x, y, z, yaw, pitch, roll;

    LC_result lc_result;
    int i_recvBytes;
    char* data_recv = new char[this->packet_size + HEADERSIZE];
    struct sockaddr_in s_addr_client;
    int client_len = sizeof(s_addr_client);

    while (this->thread_receive_flag)
    {
        memset(data_recv, 0, this->packet_size + HEADERSIZE);

        i_recvBytes = recvfrom(this->sockfd_listen, data_recv, this->packet_size + HEADERSIZE, 0, (struct sockaddr*)&s_addr_client, &client_len);
        if (i_recvBytes <= 0)
        {
            printf("recv failed!\n");
            continue;
        }
        if (sscanf(data_recv, "%2d%2d%10d%4d", &id, &serial_id, &serial, &len) != 4) {
            continue;
        }
        //printf("recvfrom NO.%d, %d B from: %s:%d\n",id, i_recvBytes, inet_ntoa(s_addr_client.sin_addr), s_addr_client.sin_port);
        //历元数
        int msyType = 0;
        unsigned char* pUBuffer = new unsigned char[len];
        memcpy(pUBuffer, data_recv + HEADERSIZE, len);
        for (int i = 0; i < len; i++)
        {
            //标志头
            if (pUBuffer[i] != 0xD3) continue; //RTCM3的标志头
            if (i + 2 >= len) break;

            //检查后一字节是否为空
            int i4Space = pUBuffer[i + 1] >> 2;
            if (i4Space != 0)
            {
                continue;  //不为空的话，是错误的开始
            }

            //读取子帧长度
            int 	i4length;
            i4length = (pUBuffer[i + 1] & 0x03) * 256 + pUBuffer[i + 2];
            //先读出帧号，然后进行检测
            if (i4length > 1023)
            {
                i += 1;
                msyType = -2;
                break;
            }
            if (i + 5 > len) //如果字符串太短，不满足一帧的头的长度，或者，长度异常 退出
            {
                msyType = -2;
                break;
            }

            int i4MessageID;
            i4MessageID = pUBuffer[i + 3] * 16 + (pUBuffer[i + 4] >> 4);

            // printf("XXXXXXXXXXX i4MessageID= %d \n", i4MessageID);
            if (i4MessageID == 1042)
            {
                int ddBreak = 0;
            }

            //对ID进行判断，检查是否符合要求
            if (i4MessageID < 1000 || i4MessageID>5000)
            {
                if (i4MessageID != 63)
                {
                    //消息类型，错误，继续下一个解码
                    //printf("XXXXXXXXXXX   continue1   i4MessageID= %d \n", i4MessageID);
                    continue;
                }
            }

            //对长度进行校验
            if (i + i4length + 6 > len)
            {
                //不足一帧长度，则退回
                msyType = -2;
                break;
            }

            //对字符串进行检校比对
            unsigned char crcChar[3];
            crcUpdate(&pUBuffer[i], i4length + 3, crcChar);
            bool bValid = 1;
            for (int j = 0; j < 3; j++)
            {
                if (crcChar[j] != pUBuffer[i + 3 + i4length + j])
                {
                    bValid = 0;
                    break;
                }
            }
            if (bValid == 0)
            {
                //printf("XXXXXXXXXXX  bValid == 0 i4MessageID= %d \n", i4MessageID);
                msyType == -2;
                continue;
            }
            if (i4MessageID == 3006) {
                LcResultDecode(&pUBuffer[i + 3], len, lc_result);
                double pos[3];
                ecef2pos(lc_result.rGNSS, pos);
                std::cout << std::fixed<< inet_ntoa(s_addr_client.sin_addr)<<": "<<lc_result.rGNSS[0]<<","<< lc_result.rGNSS[1] <<"," << lc_result.rGNSS[2] << std::endl;
                double gpsLat = pos[0] * R2D;
                double gpsLon = pos[1] * R2D;
                z = pos[2];
                transform2Mars(gpsLat, gpsLon, x, y);
                yaw = lc_result.att[2];
                pitch = lc_result.att[1];
                roll = lc_result.att[0];
                positions[std::to_string(id)] = { id, x, y, z, yaw, pitch, roll };
                delete[] pUBuffer;
                break;
            }
        }
  
        

        //printf("recv:\"%s\", %d B from: %s:%d\n", data_recv, i_recvBytes, inet_ntoa(s_addr_client.sin_addr), s_addr_client.sin_port);
    }

    delete[] data_recv;
    //Clear
}
json POS_Socket::getPistions()
{
    return positions;
}
int POS_Socket::stop_collection()
{
    thread_receive_flag = 0;
    closesocket(sockfd_listen);
    //WSACleanup();
    return 1;
}
int LcResultDecode(unsigned char pUBuffer[], unsigned long nChar, LC_result& stPos)
{
    __int64  numbits = 0, bitfield = 0;
    int size = nChar;
    //int old = 0;
    int flag = 0;
    unsigned char* data = pUBuffer;

    //MESSAGEID
    __int64  word64;
    int i4MessageID2;  GETBITS(word64, 12)
        i4MessageID2 = int(word64 & 0xFFF);


    unsigned long lSecond; GETBITS(lSecond, 30);

    stPos.gpsSecond = lSecond / 1000.0;

    __int64 int38;
    GETBITSSIGN(int38, 38);
    stPos.rGNSS[0] = int38 * 0.0001;

    GETBITSSIGN(int38, 38);
    stPos.rGNSS[1] = int38 * 0.0001;

    GETBITSSIGN(int38, 38);
    stPos.rGNSS[2] = int38 * 0.0001;


    GETBITSSIGN(int38, 38);
    stPos.vel[0] = int38 * 0.0001;

    GETBITSSIGN(int38, 38);
    stPos.vel[1] = int38 * 0.0001;

    GETBITSSIGN(int38, 38);
    stPos.vel[2] = int38 * 0.0001;


    GETBITSSIGN(int38, 38);
    stPos.att[0] = int38 * 0.0001;

    GETBITSSIGN(int38, 38);
    stPos.att[1] = int38 * 0.0001;

    GETBITSSIGN(int38, 38);
    stPos.att[2] = int38 * 0.0001;

    //Reference station ID
    // 	int RefID = stPos.rRms;
    // 	word64 = RefID; theOperater.AddBits(word64, 12);

    //	word64 = 0; theOperater.AddBits(word64, 6);   //Reserved for ITRF Realization Year

    unsigned long lrms; GETBITS(lrms, 23);
    GETBITS(stPos.bGpsFlag, 1);

    stPos.rRms = lrms / 1000.0;

    return i4MessageID2;
}



int crcUpdate(unsigned char pUBuffer[], unsigned long len, unsigned char crcChar[])
{
    static unsigned long table[256];    /* Initialized to 0 */

    unsigned long crc = 0;

    if (!table[1])
        crcInit(table);
    int iByte = 0;
    while (len--)
    {
        crc = crc << 8 ^ table[pUBuffer[iByte] ^ (unsigned char)(crc >> 16)];
        iByte++;
    }
    crcChar[0] = (unsigned char)((crc & 0xffffff) >> 16 & 0xFF);
    crcChar[1] = (unsigned char)((crc & 0xffffff) >> 8 & 0xFF);
    crcChar[2] = (unsigned char)((crc & 0xffffff) & 0xFF);

    return 1;
}


void crcInit(unsigned long table[256])
{
    unsigned i, j;
    unsigned long h;    /* CRC of i, where i is a power of 2 */

    table[0] = 0;
    table[1] = h = POLY24;

    for (i = 2; i < 256; i *= 2)
    {
        if ((h <<= 1) & 0x1000000)      /* h <<= 1 (mod poly) */
            h ^= POLY24;
        for (j = 0; j < i; j++)
            table[i + j] = table[j] ^ h;
    }
}

extern void ecef2pos(const double* r, double* pos)
{
    double e2 = FE_WGS84 * (2.0 - FE_WGS84), r2 = dot(r, r, 2), z, zk, v = RE_WGS84, sinp;

    for (z = r[2], zk = 0.0; fabs(z - zk) >= 1E-8;) {
        zk = z;
        sinp = z / sqrt(r2 + z * z);
        v = RE_WGS84 / sqrt(1.0 - e2 * sinp * sinp);
        z = r[2] + v * e2 * sinp;
    }
    pos[0] = r2 > 1E-12 ? atan(z / sqrt(r2)) : (r[2] > 0.0 ? PI / 2.0 : -PI / 2.0);
    pos[1] = r2 > 1E-12 ? atan2(r[1], r[0]) : 0.0;
    pos[2] = sqrt(r2 + z * z) - v;
}
extern double dot(const double* a, const double* b, int n)
{
    double c = 0.0;

    while (--n >= 0) c += a[n] * b[n];
    return c;
}

const double pi = 3.14159265358979324;
const double a = 6378245.0;
const double ee = 0.00669342162296594323;
const  double x_pi = 3.14159265358979324 * 3000.0 / 180.0;

bool outOfChina(double lat, double lon)
{
    if (lon < 72.004 || lon > 137.8347)
        return true;
    if (lat < 0.8293 || lat > 55.8271)
        return true;
    return false;
}

double transformLat(double x, double y)
{
    double ret = -100.0 + 2.0 * x + 3.0 * y + 0.2 * y * y + 0.1 * x * y + 0.2 * sqrt(abs(x));
    ret += (20.0 * sin(6.0 * x * pi) + 20.0 * sin(2.0 * x * pi)) * 2.0 / 3.0;
    ret += (20.0 * sin(y * pi) + 40.0 * sin(y / 3.0 * pi)) * 2.0 / 3.0;
    ret += (160.0 * sin(y / 12.0 * pi) + 320 * sin(y * pi / 30.0)) * 2.0 / 3.0;
    return ret;
}

double transformLon(double x, double y)
{
    double ret = 300.0 + x + 2.0 * y + 0.1 * x * x + 0.1 * x * y + 0.1 * sqrt(abs(x));
    ret += (20.0 * sin(6.0 * x * pi) + 20.0 * sin(2.0 * x * pi)) * 2.0 / 3.0;
    ret += (20.0 * sin(x * pi) + 40.0 * sin(x / 3.0 * pi)) * 2.0 / 3.0;
    ret += (150.0 * sin(x / 12.0 * pi) + 300.0 * sin(x / 30.0 * pi)) * 2.0 / 3.0;
    return ret;
}

/**
 * 地球坐标转换为火星坐标
 * World Geodetic System ==> Mars Geodetic System
 *
 * @param wgLat  地球坐标
 * @param wgLon
 *
 * mglat,mglon 火星坐标
 */
void transform2Mars(double wgLat, double wgLon, double& mgLat, double& mgLon)
{
    if (outOfChina(wgLat, wgLon))
    {
        mgLat = wgLat;
        mgLon = wgLon;
        return;
    }
    double dLat = transformLat(wgLon - 105.0, wgLat - 35.0);
    double dLon = transformLon(wgLon - 105.0, wgLat - 35.0);
    double radLat = wgLat / 180.0 * pi;
    double magic = sin(radLat);
    magic = 1 - ee * magic * magic;
    double sqrtMagic = sqrt(magic);
    dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * sqrtMagic) * pi);
    dLon = (dLon * 180.0) / (a / sqrtMagic * cos(radLat) * pi);
    mgLat = wgLat + dLat;
    mgLon = wgLon + dLon;

}