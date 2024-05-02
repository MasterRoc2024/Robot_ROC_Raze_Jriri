/* 
 * File:   main.c
 * Author: TP-EO-1
 *
 * Created on 16 octobre 2023, 10:49
 */

#include <stdio.h>
#include <stdlib.h>
#include <xc.h>
#include <libpic30.h>
#include "ChipConfig.h"
#include "IO.h"
#include "timer.h"
#include "PWM.h"
#include "ADC.h"
#include "Robot.h"
#include "main.h"
#include "UART.h"
#include "CB_TX1.h"
#include "CB_RX1.h"
#include "UART_Protocol.h"
/*#define STATE_ATTENTE 0
#define STATE_ATTENTE_EN_COURS 1
#define STATE_AVANCE 2
#define STATE_AVANCE_EN_COURS 3
#define STATE_TOURNE_GAUCHE 4
#define STATE_TOURNE_GAUCHE_EN_COURS 5
#define STATE_TOURNE_DROITE 6
#define STATE_TOURNE_DROITE_EN_COURS 7
#define STATE_TOURNE_SUR_PLACE_GAUCHE 8
#define STATE_TOURNE_SUR_PLACE_GAUCHE_EN_COURS 9
#define STATE_TOURNE_SUR_PLACE_DROITE 10
#define STATE_TOURNE_SUR_PLACE_DROITE_EN_COURS 11
#define STATE_ARRET 12
#define STATE_ARRET_EN_COURS 13
#define STATE_RECULE 14
#define STATE_RECULE_EN_COURS 15

#define PAS_D_OBSTACLE 0
#define OBSTACLE_A_GAUCHE 1
#define OBSTACLE_A_DROITE 2
#define OBSTACLE_EN_FACE 3*/

#define MOTEUR_GAUCHE 0
#define MOTEUR_DROIT 1

unsigned char stateRobot;
unsigned char nextStateRobot = 0;
int processReceiveData= 0;

int main(void) {
    /***************************************************************************************************/
    //Initialisation de l?oscillateur
    /****************************************************************************************************/

    InitOscillator();

    /****************************************************************************************************/
    // Configuration des entr�es sorties
    /****************************************************************************************************/
    InitIO();

    InitTimer1();
    InitTimer23();
    InitTimer4();
    InitPWM();
    InitADC1();
    InitUART();
    /*PWMSetSpeed(10, MOTEUR_DROIT);
    PWMSetSpeed(10, MOTEUR_GAUCHE);*/
    LED_BLANCHE = 0;
    LED_BLEUE = 0;
    LED_ORANGE = 0;
    /****************************************************************************************************/
    // Boucle Principale
    /****************************************************************************************************/
    while (1) {
        /*SendMessage((unsigned char*) "Bonjour", 7);
        __delay32(4000000);*/
        /*int i= 0;
        unsigned char payload[] = {'B', 'o', 'n', 'j', 'o', 'u', 'r'};
        int t= sizeof(payload);
        UartEncodeAndSendMessage(128,
            sizeof(payload), payload);
        __delay32(40000000);*/
        /*for (i = 0; i < CB_RX1_GetDataSize(); i++) {
            unsigned char c = CB_RX1_Get();
            SendMessage(&c, 1);
            
        }
        __delay32(10000);*/
        //SendMessageDirect((unsigned char*) "Bonjour", 7);
        //__delay32(4000000);

        //LED_BLANCHE = !LED_BLANCHE;
        //LED_BLEUE = !LED_BLEUE;
        //LED_ORANGE = !LED_ORANGE;
        for (int i = 0; i < CB_RX1_GetDataSize(); i++) {
            unsigned char c = CB_RX1_Get();
            UartDecodeMessage(c);
        }
        
        if (ADCIsConversionFinished() == 1) {           //Si une nouvelle aquisition termin�e || R�cup�ration des r�sultats
            unsigned int * result=  ADCGetResult();
            unsigned int ADCValue0= result[0];
            float volts = ((float) ADCValue0)* 3.3 / 4096 * 3.2;
            robotState.distanceTelemetreGauche = 34 / volts - 5;
            unsigned int ADCValue1= result[1];
            volts = ((float) ADCValue1)* 3.3 / 4096 * 3.2;
            robotState.distanceTelemetreCentre = 34 / volts - 5;
            unsigned int ADCValue2= result[2];
            volts = ((float) ADCValue2)* 3.3 / 4096 * 3.2;
            robotState.distanceTelemetreDroit = 34 / volts - 5;
            //Flag � 0 pour d�tecter la fin de l'aquisition suivante
            ADCClearConversionFinishedFlag();
            //Envoi de trames pour les IR
            unsigned char payloadIR[] = {robotState.distanceTelemetreGauche, robotState.distanceTelemetreCentre, robotState.distanceTelemetreDroit};
            UartEncodeAndSendMessage(0x0030,
            sizeof(payloadIR), payloadIR);
            //Envoi de trames pour les �tats des Leds
            unsigned char payloadLed[2]= {0, LED_BLANCHE};
            UartEncodeAndSendMessage(0x0020,
            sizeof(payloadLed), payloadLed);
            payloadLed[0]= 1;
            payloadLed[1]= LED_BLEUE;
            UartEncodeAndSendMessage(0x0020,
            sizeof(payloadLed), payloadLed);
            payloadLed[0]= 2;
            payloadLed[1]= LED_ORANGE;
            UartEncodeAndSendMessage(0x0020,
            sizeof(payloadLed), payloadLed);
            //Envoi de trame pour les Vitesses courantes des moteurs
            unsigned char payloadMotors[2]= {robotState.vitesseGaucheCommandeCourante, robotState.vitesseDroiteCommandeCourante};
            UartEncodeAndSendMessage(0x0040,
            sizeof(payloadMotors), payloadMotors);
        }
    } // fin main

}


