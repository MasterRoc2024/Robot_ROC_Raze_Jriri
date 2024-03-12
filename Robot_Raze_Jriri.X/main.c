/* 
 * File:   main.c
 * Author: TP-EO-1
 *
 * Created on 16 octobre 2023, 10:49
 */

#include <stdio.h>
#include <stdlib.h>
#include <xc.h>
#include "ChipConfig.h"
#include "IO.h"
#include "timer.h"
#include "PWM.h"
#include "ADC.h"
#include "Robot.h"
#include "main.h"

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
unsigned char nextStateRobot=0;



int main(void) {
    /***************************************************************************************************/
    //Initialisation de l?oscillateur
    /****************************************************************************************************/
    
    InitOscillator();

    /****************************************************************************************************/
    // Configuration des entrées sorties
    /****************************************************************************************************/
    InitIO();
    
    InitTimer1();
    InitTimer23();
    InitTimer4();
    InitPWM();
    InitADC1();
    PWMSetSpeed(10, MOTEUR_DROIT);
    PWMSetSpeed(10, MOTEUR_GAUCHE);
    LED_BLANCHE = 1;
    LED_BLEUE = 1;
    LED_ORANGE = 1;

    /****************************************************************************************************/
    // Boucle Principale
    /****************************************************************************************************/
    while (1) {
        //LED_BLANCHE = !LED_BLANCHE;
        //LED_BLEUE = !LED_BLEUE;
        //LED_ORANGE = !LED_ORANGE;
        
        /*if (ADCIsConversionFinished() == 1) {           //Si une nouvelle aquisition terminée || Récupération des résultats
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
            //Flag à 0 pour détecter la fin de l'aquisition suivante
            ADCClearConversionFinishedFlag();
            
        }*/
    } // fin main
      
}


