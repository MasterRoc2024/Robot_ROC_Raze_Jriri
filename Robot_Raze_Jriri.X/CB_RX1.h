/* 
 * File:   CB_RX1.h
 * Author: TP-EO-1
 *
 * Created on 2 avril 2024, 11:23
 */

#ifndef CB_RX1_H
#define	CB_RX1_H

void CB_RX1_Add(unsigned char value);
unsigned char CB_RX1_Get(void);
unsigned char CB_RX1_IsDataAvailable(void);
int CB_RX1_GetDataSize(void);
int CB_RX1_GetRemainingSize(void);

#endif	/* CB_RX1_H */

