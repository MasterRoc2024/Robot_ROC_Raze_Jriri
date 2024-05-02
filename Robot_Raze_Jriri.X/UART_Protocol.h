/* 
 * File:   UART_Protocol.h
 * Author: TP-EO-1
 *
 * Created on 30 avril 2024, 15:58
 */

#ifndef UART_PROTOCOL_H
#define	UART_PROTOCOL_H


void UartEncodeAndSendMessage(int msgFunction,
        int msgPayloadLength, unsigned char* msgPayload);
void UartProcessDecodedMessage(int function,
        int payloadLength, unsigned char* payload);
unsigned char UartCalculateChecksum(int msgFunction,
        int msgPayloadLength, unsigned char* msgPayload);

#endif	/* UART_PROTOCOL_H */

