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
    InitPWM();
    InitADC1();
    
    LED_BLANCHE = 1;
    LED_BLEUE = 1;
    LED_ORANGE = 1;
    //PWMSetSpeed(20.0, 20.0);
    //PWMSetSpeedConsigne(20, 20);

    /****************************************************************************************************/
    // Boucle Principale
    /****************************************************************************************************/
    while (1) {
        //LED_BLANCHE = !LED_BLANCHE;
        //LED_BLEUE = !LED_BLEUE;
        //LED_ORANGE = !LED_ORANGE;
        
        if (ADCIsConversionFinished() == 1) {           //Si une nouvelle aquisition terminée || Récupération des résultats
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
            
        }
    } // fin main
}

