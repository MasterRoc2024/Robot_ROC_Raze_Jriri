/* 
 * File:   timer.h
 * Author: TP-EO-1
 *
 * Created on 23 octobre 2023, 09:36
 */

#ifndef TIMER_H
#define	TIMER_H
#define CPUCLOCK 40000000

void InitTimer23(void);
void InitTimer1(void);
void InitTimer4(void);
void SetFreqTimer1(float freq);
void SetFreqTimer4(float freq);

extern unsigned long timestamp;

#endif	/* TIMER_H */

