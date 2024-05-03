/* 
 * File:   UART_Protocol.h
 * Author: TP-EO-1
 *
 * Created on 30 avril 2024, 15:58
 */

#ifndef UART_PROTOCOL_H
#define	UART_PROTOCOL_H
#define SET_ROBOT_STATE 0x0051
#define SET_ROBOT_MANUAL_CONTROL 0x0052
#define LED_FUNCTION 0x0020
#define IR_FUNCTION 0x0030
#define CONSIGNE_FUNCTION 0x0040

void UartEncodeAndSendMessage(int msgFunction,
        int msgPayloadLength, unsigned char* msgPayload);
void UartProcessDecodedMessage(int function,
        int payloadLength, unsigned char* payload);
unsigned char UartCalculateChecksum(int msgFunction,
        int msgPayloadLength, unsigned char* msgPayload);

#endif	/* UART_PROTOCOL_H */

