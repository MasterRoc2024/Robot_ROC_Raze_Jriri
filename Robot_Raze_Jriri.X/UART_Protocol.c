#include <xc.h>
#include "UART_Protocol.h"
#include "UART.h"

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

/*void UartDecodeMessage(unsigned char c) {
    //Fonction prenant en entree un octet et servant a reconstituer les trames
    ...
}

void UartProcessDecodedMessage(int function,
        int payloadLength, unsigned char* payload) {
    //Fonction appelee apres le decodage pour executer l?action
    //correspondant au message recu
    ...
}*/
//*************************************************************************/
//Fonctions correspondant aux messages
//*************************************************************************/
