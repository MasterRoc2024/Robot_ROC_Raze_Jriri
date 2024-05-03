#include <xc.h>
#include "UART_Protocol.h"
#include "UART.h"
#include "math.h"
#include "IO.h"
#include "PWM.h"
#include "Robot.h"
#define Waiting 0
#define FunctionMSB 1
#define FunctionLSB 2
#define PayloadLengthMSB 3
#define PayloadLengthLSB 4
#define Payload 5
#define CheckSum 6
#define SET_ROBOT_STATE 0x0051

unsigned char UartCalculateChecksum(int msgFunction,
        int msgPayloadLength, unsigned char* msgPayload) {
    //Fonction prenant entree la trame et sa longueur pour calculer le checksum
    unsigned char checksum = 0;
    checksum ^= 0xFE;
    checksum ^= msgFunction >> 8;
    checksum ^= msgFunction >> 0;
    checksum ^= msgPayloadLength >> 8;
    checksum ^= msgPayloadLength >> 0;
    for (int i = 0;i< msgPayloadLength; i++)
    {
        checksum ^= msgPayload[i];
    }

    return checksum;
}

void UartEncodeAndSendMessage(int msgFunction,
        int msgPayloadLength, unsigned char* msgPayload) {
    //Fonction d?encodage et d?envoi d?un message
    unsigned char checksum= UartCalculateChecksum(msgFunction, 
                                                    msgPayloadLength, msgPayload);
    unsigned char array[msgPayloadLength + 6];
    array[0]= 0xFE;
    array[1] = msgFunction >> 8;
    array[2] = msgFunction >> 0;
    array[3] = msgPayloadLength >> 8;
    array[4] = msgPayloadLength >> 0;
    for(int i= 0; i < msgPayloadLength; i++) {
        array[i + 5] = msgPayload[i];
    }
    array[sizeof(array)-1] = checksum;
    //Envoi du Message
    for (int i = 0; i < sizeof(array); i++) {
        SendMessage(&array[i], 1);
    }
}
int msgDecodedFunction = 0;
int msgDecodedPayloadLength = 0;
unsigned char msgDecodedPayload[128];
int msgDecodedPayloadIndex = 0;
int rcvState= Waiting;
unsigned char calculatedChecksum= 0;

void UartDecodeMessage(unsigned char c) {
    //Fonction prenant en entree un octet et servant a reconstituer les trames
    switch (rcvState)
{
    case Waiting:
        //Etape1
        if (c == 0xFE)
        {
            msgDecodedFunction = 0;
            msgDecodedPayloadIndex = 0;
            msgDecodedPayloadLength = 0;
            rcvState = FunctionMSB;
        }
        break;
    case FunctionMSB:
        msgDecodedFunction += c * (int)pow(2,8);
        //Etape2
        rcvState = FunctionLSB;
        break;
    case FunctionLSB:
        //Etape3
        msgDecodedFunction += c;
        rcvState = PayloadLengthMSB;
        break;
    case PayloadLengthMSB:
        msgDecodedPayloadLength += c * (int)pow(2, 8);
        rcvState = PayloadLengthLSB;
        break;
    case PayloadLengthLSB:
        msgDecodedPayloadLength += c;
        rcvState = Payload;
        break;
    case Payload:
        msgDecodedPayload[msgDecodedPayloadIndex] = c;
        msgDecodedPayloadIndex += 1;
        if (msgDecodedPayloadIndex >= msgDecodedPayloadLength)
        {
            rcvState = CheckSum;
        }
        break;
    case CheckSum:
        calculatedChecksum = 0;
        calculatedChecksum ^= 0xFE;
        calculatedChecksum ^= (msgDecodedFunction >> 8);
        calculatedChecksum ^= (msgDecodedFunction >> 0);
        calculatedChecksum ^= (msgDecodedPayloadLength >> 8);
        calculatedChecksum ^= (msgDecodedPayloadLength >> 0);
        //    msgDecodedPayloadLength;
        for(int i= 0; i < sizeof(msgDecodedPayload); i++)
        {
            calculatedChecksum ^= msgDecodedPayload[i];
        }
        unsigned char receivedChecksum = c;
        if (calculatedChecksum == receivedChecksum)
        {
            UartProcessDecodedMessage(msgDecodedFunction, msgDecodedPayloadLength, msgDecodedPayload);
            rcvState = Waiting;
        }
        else
        {
            rcvState = Waiting;
        }
            
        break;
    default:
        rcvState = Waiting;
        break;
}
}

void UartProcessDecodedMessage(int function,
        int payloadLength, unsigned char* payload) {
    //Fonction appelee apres le decodage pour executer l?action
    //correspondant au message recu
    switch((unsigned char)function) {
        case LED_FUNCTION:
            if (payload[0] == 0x0000) {
                LED_BLANCHE = payload[1];
            }
            if (payload[0] == 0x0001) {
                LED_BLEUE = payload[1];
            }
            if (payload[0] == 0x0002) {
                LED_ORANGE = payload[1];
            }
            break;
        case CONSIGNE_FUNCTION:
            PWMSetSpeedConsigne((float)payload[0],(float)payload[1] );
            break;
        case SET_ROBOT_STATE:
            robotState.stateRobot= payload[0];
            break;
        case SET_ROBOT_MANUAL_CONTROL:
            robotState.autoControlActivated = payload[0];
            break;
        default:
            break;
    }
}
//*************************************************************************/
//Fonctions correspondant aux messages
//*************************************************************************/
