#include <xc.h>
#include <stdio.h> // library xc.h inclut tous les uC
#include "IO.h"
#include "PWM.h"
#include "Robot.h"
#include "ToolBox.h"
#define PWMPER 40.0
#define MOTEUR_GAUCHE 0
#define MOTEUR_DROIT 1
//unsigned char acceleration = 20;
unsigned char acceleration = 1;

void InitPWM(void) {
    PTCON2bits.PCLKDIV = 0b000; //Divide by 1
    PTPER = 100 * PWMPER; //éPriode en pourcentage
    //éRglage PWM moteur 1 sur hacheur 1
    IOCON1bits.POLH = 1; //High = 1 and active on low =0
    IOCON1bits.POLL = 1; //High = 1
    IOCON1bits.PMOD = 0b01; //Set PWM Mode to Redundant
    FCLCON1 = 0x0003; //éDsactive la gestion des faults
    //Reglage PWM moteur 2 sur hacheur 6
    IOCON6bits.POLH = 1; //High = 1
    IOCON6bits.POLL = 1; //High = 1
    IOCON6bits.PMOD = 0b01; //Set PWM Mode to Redundant
    FCLCON6 = 0x0003; //éDsactive la gestion des faults
    /* Enable PWM Module */
    PTCONbits.PTEN = 1;
}

void PWMSetSpeed(float vitessePourcents, int motor) {

    if (motor == MOTEUR_GAUCHE) {
        if (vitessePourcents > 0) {
            robotState.vitesseGaucheCommandeCourante = vitessePourcents;
            MOTEUR_GAUCHE_L_PWM_ENABLE = 0; //Pilotage de la pin en mode IO
            MOTEUR_GAUCHE_L_IO_OUTPUT = 1; //Mise à1 de la pin
            MOTEUR_GAUCHE_H_PWM_ENABLE = 1; //Pilotage de la pin en mode PWM
            MOTEUR_GAUCHE_DUTY_CYCLE = Abs(robotState.vitesseGaucheCommandeCourante * PWMPER);
        } else {
            robotState.vitesseGaucheCommandeCourante = vitessePourcents;
            MOTEUR_GAUCHE_H_PWM_ENABLE = 0; //Pilotage de la pin en mode IO
            MOTEUR_GAUCHE_H_IO_OUTPUT = 1; //Mise à1 de la pin
            MOTEUR_GAUCHE_L_PWM_ENABLE = 1; //Pilotage de la pin en mode PWM
            MOTEUR_GAUCHE_DUTY_CYCLE = Abs(robotState.vitesseGaucheCommandeCourante * PWMPER);
        }
    } else {
        if (vitessePourcents > 0) {
            robotState.vitesseDroiteCommandeCourante = vitessePourcents;
            MOTEUR_DROIT_L_PWM_ENABLE = 0; //Pilotage de la pin en mode IO
            MOTEUR_DROIT_L_IO_OUTPUT = 1; //Mise à1 de la pin
            MOTEUR_DROIT_H_PWM_ENABLE = 1; //Pilotage de la pin en mode PWM
            MOTEUR_DROIT_DUTY_CYCLE = Abs(robotState.vitesseDroiteCommandeCourante * PWMPER);
        } else {
            robotState.vitesseDroiteCommandeCourante = vitessePourcents;
            MOTEUR_DROIT_H_PWM_ENABLE = 0; //Pilotage de la pin en mode IO
            MOTEUR_DROIT_H_IO_OUTPUT = 1; //Mise à1 de la pin
            MOTEUR_DROIT_L_PWM_ENABLE = 1; //Pilotage de la pin en mode PWM
            MOTEUR_DROIT_DUTY_CYCLE = Abs(robotState.vitesseDroiteCommandeCourante * PWMPER);
        }
    }
}

void PWMSetSpeedConsigne(float vitesseMGConsigne, float vitesseMDConsigne) {//pour donner une vitesse de consigne au robot 
    robotState.vitesseGaucheConsigne = vitesseMGConsigne;
    robotState.vitesseDroiteConsigne = vitesseMDConsigne;
    int t= 1;
}

void PWMUpdateSpeed()//generateur de rampe 
{
    // Cette fonction est ?appele sur timer et permet de suivre des rampes d???acclration
    if (robotState.vitesseDroiteCommandeCourante < robotState.vitesseDroiteConsigne) { //Le robot doit accelerer jusqu'a atteindre la vitesse consigne 
        robotState.vitesseDroiteCommandeCourante = Min(robotState.vitesseDroiteCommandeCourante + acceleration,
                robotState.vitesseDroiteConsigne); // pour rester dans l'intervalle rt ne pas depaser la vitesse consigne
    }
    if (robotState.vitesseDroiteCommandeCourante > robotState.vitesseDroiteConsigne) { //il faut ralentir pour retrouver sa vitesse de consigne 
        robotState.vitesseDroiteCommandeCourante = Max(robotState.vitesseDroiteCommandeCourante - acceleration,
                robotState.vitesseDroiteConsigne); //c'est pour ne jamais depasser la vitesse consigne 
    }
    if (robotState.vitesseDroiteCommandeCourante > 0) {
        MOTEUR_DROIT_L_PWM_ENABLE = 0; //pilotage de la pin en mode IO
        MOTEUR_DROIT_L_IO_OUTPUT = 1; //Mise ?1 de la pin
        MOTEUR_DROIT_H_PWM_ENABLE = 1; //Pilotage de la pin en mode PWM
    } else {
        MOTEUR_DROIT_H_PWM_ENABLE = 0; //pilotage de la pin en mode IO
        MOTEUR_DROIT_H_IO_OUTPUT = 1; //Mise ?1 de la pin
        MOTEUR_DROIT_L_PWM_ENABLE = 1; //Pilotage de la pin en mode PWM
    }
    MOTEUR_DROIT_DUTY_CYCLE = Abs(robotState.vitesseDroiteCommandeCourante) * PWMPER;
    if (robotState.vitesseGaucheCommandeCourante < robotState.vitesseGaucheConsigne) {
        robotState.vitesseGaucheCommandeCourante = Min(robotState.vitesseGaucheCommandeCourante + acceleration,
                robotState.vitesseGaucheConsigne);
    }
    if (robotState.vitesseGaucheCommandeCourante > robotState.vitesseGaucheConsigne) {
        robotState.vitesseGaucheCommandeCourante = Max(robotState.vitesseGaucheCommandeCourante - acceleration,
                robotState.vitesseGaucheConsigne);
    }
    
    if (robotState.vitesseGaucheCommandeCourante > 0) {
        MOTEUR_GAUCHE_L_PWM_ENABLE = 0; //pilotage de la pin en mode IO
        MOTEUR_GAUCHE_L_IO_OUTPUT = 1; //Mise ?1 de la pin
        MOTEUR_GAUCHE_H_PWM_ENABLE = 1; //Pilotage de la pin en mode PWM
    } else {
        MOTEUR_GAUCHE_H_PWM_ENABLE = 0; //pilotage de la pin en mode IO
        MOTEUR_GAUCHE_H_IO_OUTPUT = 1; //Mise ?1 de la pin
        MOTEUR_GAUCHE_L_PWM_ENABLE = 1; //Pilotage de la pin en mode PWM
    }
    MOTEUR_GAUCHE_DUTY_CYCLE = Abs(robotState.vitesseGaucheCommandeCourante) * PWMPER;
}