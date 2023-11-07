/* 
 * File:   PWM.h
 * Author: TP-EO-1
 *
 * Created on 23 octobre 2023, 11:25
 */

#ifndef PWM_H
#define	PWM_H
#define PWMPER 40.0

void InitPWM(void);
void PWMSetSpeed(float vitesseMGPourcents, float vitesseMDPourcents);


#endif	/* PWM_H */

