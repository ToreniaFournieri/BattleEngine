using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleEngine
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            DateTime startDateTime = DateTime.Now;
            Console.WriteLine("start:" + startDateTime);
            // Battle environment setting
            int numberOfCharacters = 14;
            int battleWavesSets = 1;
            int battleWaves = 1; // one set of battle 

            //BattleWaveSet variables
            double allyAttackMagnificationPerWavesSet = 0.20;
            double allyDefenseMagnificationPerWavesSet = 0.20;

            bool battleEnd = false;
            FuncBattleConditionsText text = null;
            WipeOutCheck wipeOutCheck = null;
            Random r = new Random();

            // System initialize : Do not change them.
            string log = null;
            string finalLog = null;
            string[] logPerWavesSets = new string[battleWavesSets];
            string[] subLogPerWavesSets = new string[battleWavesSets];
            int totalturn = 0; // Static info of total turn
            int currentBattleWaves = 1;
            int allyWinCount = 0;
            int enemyWinCount = 0;
            int drawCount = 0;

            double allyAttackMagnification = 1.0;
            double allyDefenseMagnification = 1.0;

            List<BattleUnit> characters = new List<BattleUnit>();
            BattleUnit.AbilityClass[] abilities = new BattleUnit.AbilityClass[numberOfCharacters];

            //Skill make logic , test: one character per one skill
            SkillsMasterStruct[] skillsMasters = new SkillsMasterStruct[numberOfCharacters + 1];
            List<SkillsMasterStruct> buffMasters = new List<SkillsMasterStruct>();

            //Effect (permament skill) make logic, the effect has two meanings, one is permanent skill, the other is temporary skill (may call buff).
            List<EffectClass> effects = new List<EffectClass>();

            //ally info
            abilities[0] = new BattleUnit.AbilityClass(power: 27, generation: 11, stability: 22, responsiveness: 32, precision: 22, intelligence: 21, luck: 34); //PIG1-
            abilities[1] = new BattleUnit.AbilityClass(power: 29, generation: 13, stability: 22, responsiveness: 24, precision: 22, intelligence: 14, luck: 15); //PIG2-
            abilities[2] = new BattleUnit.AbilityClass(power: 27, generation: 10, stability: 22, responsiveness: 21, precision: 25, intelligence: 21, luck: 34); //PIG3-
            abilities[3] = new BattleUnit.AbilityClass(power: 29, generation: 10, stability: 22, responsiveness: 24, precision: 22, intelligence: 14, luck: 15); //PIG4-
            abilities[4] = new BattleUnit.AbilityClass(power: 27, generation: 10, stability: 22, responsiveness: 32, precision: 22, intelligence: 21, luck: 34); //PIG5-
            abilities[5] = new BattleUnit.AbilityClass(power: 29, generation: 12, stability: 22, responsiveness: 24, precision: 22, intelligence: 30, luck: 15); //PIG6-
            abilities[6] = new BattleUnit.AbilityClass(power: 27, generation: 11, stability: 22, responsiveness: 20, precision: 22, intelligence: 21, luck: 34); //PIG7-
            //enemy info
            abilities[7] = new BattleUnit.AbilityClass(power: 29, generation: 8, stability: 22, responsiveness: 23, precision: 22, intelligence: 14, luck: 15); //ELD1- 
            abilities[8] = new BattleUnit.AbilityClass(power: 27, generation: 13, stability: 22, responsiveness: 31, precision: 22, intelligence: 21, luck: 34); //ELD2-
            abilities[9] = new BattleUnit.AbilityClass(power: 29, generation: 8, stability: 22, responsiveness: 22, precision: 22, intelligence: 14, luck: 15); //ELD3-
            abilities[10] = new BattleUnit.AbilityClass(power: 27, generation: 9, stability: 22, responsiveness: 30, precision: 22, intelligence: 21, luck: 34); //ELD4-
            abilities[11] = new BattleUnit.AbilityClass(power: 29, generation: 8, stability: 22, responsiveness: 25, precision: 22, intelligence: 14, luck: 15); //ELD5-
            abilities[12] = new BattleUnit.AbilityClass(power: 27, generation: 11, stability: 22, responsiveness: 36, precision: 22, intelligence: 21, luck: 34); //ELD6-
            abilities[13] = new BattleUnit.AbilityClass(power: 29, generation: 12, stability: 22, responsiveness: 20, precision: 22, intelligence: 14, luck: 15); //ELD7-

            BattleUnit.OffenseMagnificationClass offenseMagnification =
             new BattleUnit.OffenseMagnificationClass(optimumRangeBonus: 1.2, critical: 1.4, kinetic: 1.0, chemical: 1.0, thermal: 1.4, vsBeast: 1.0, vsCyborg: 2.44, vsDrone: 1.0, vsRobot: 2.2, vsTitan: 1.0);

            BattleUnit.DefenseMagnificationClass defenseMagnification =
                new BattleUnit.DefenseMagnificationClass(critical: 1.0, kinetic: 1.0, chemical: 1.0, thermal: 1.0, vsBeast: 1.0, vsCyborg: 1.0, vsDrone: 1.0, vsRobot: 1.2, vsTitan: 1.0);

            BattleUnit.SkillMagnificationClass.ActionSkillClass skillActionSkillInitial = new BattleUnit.SkillMagnificationClass.ActionSkillClass(move: 1.0, heal: 1.0, counter: 1.0, chain: 1.0, reAttack: 1.0,
                interrupt: 1.0, atBeginning: 1.0, atEnding: 1.0);
            BattleUnit.SkillMagnificationClass.ActionSkillClass skillActionSkillAllDouble = new BattleUnit.SkillMagnificationClass.ActionSkillClass(move: 2.0, heal: 1.0, counter: 2.0, chain: 2.0, reAttack: 2.0,
                interrupt: 2.0, atBeginning: 2.0, atEnding: 2.0);
            BattleUnit.SkillMagnificationClass.ActionSkillClass skillActionSkillAllTriple = new BattleUnit.SkillMagnificationClass.ActionSkillClass(move: 3.0, heal: 1.0, counter: 3.0, chain: 3.0, reAttack: 3.0,
                interrupt: 3.0, atBeginning: 3.0, atEnding: 3.0);

            BattleUnit.SkillMagnificationClass skillMagnificationAllInitial = new BattleUnit.SkillMagnificationClass(offenseEffectPower: skillActionSkillInitial, triggerPossibility: skillActionSkillInitial);
            BattleUnit.SkillMagnificationClass skillMagnificationAllDouble = new BattleUnit.SkillMagnificationClass(offenseEffectPower: skillActionSkillAllDouble, triggerPossibility: skillActionSkillAllDouble);
            BattleUnit.SkillMagnificationClass skillMagnificationOffenseDoubleTriggerTriple = new BattleUnit.SkillMagnificationClass(offenseEffectPower: skillActionSkillAllDouble, triggerPossibility: skillActionSkillAllTriple);

            //Skills
            TriggerBaseClass triggerPossibilityNone = new TriggerBaseClass(possibilityBaseRate: 0.0, possibilityWeight: 0, accumulationReference: ReferenceStatistics.none, accumulationBaseRate: 0.0, accumulationWeight: 0.0);
            TriggerBaseClass triggerPossibilityBasic = new TriggerBaseClass(possibilityBaseRate: 0.149, possibilityWeight: 15, accumulationReference: ReferenceStatistics.none, accumulationBaseRate: 0, accumulationWeight: 0.0);
            TriggerBaseClass triggerPossibilityNormal = new TriggerBaseClass(possibilityBaseRate: 0.122, possibilityWeight: 6, accumulationReference: ReferenceStatistics.none, accumulationBaseRate: 0.0, accumulationWeight: 0.0);
            TriggerBaseClass triggerPossibilityExpert = new TriggerBaseClass(possibilityBaseRate: 0.060, possibilityWeight: 2, accumulationReference: ReferenceStatistics.none, accumulationBaseRate: 0.0, accumulationWeight: 0.0);
            TriggerBaseClass triggerPossibilityMaster = new TriggerBaseClass(possibilityBaseRate: 0.002, possibilityWeight: 1, accumulationReference: ReferenceStatistics.none, accumulationBaseRate: 0.0, accumulationWeight: 0.0);
            TriggerBaseClass triggerPossibility100 = new TriggerBaseClass(possibilityBaseRate: 1.0, possibilityWeight: 0, accumulationReference: ReferenceStatistics.none, accumulationBaseRate: 0.0, accumulationWeight: 0.0);
            TriggerBaseClass triggerAccumulationMiddle = new TriggerBaseClass(possibilityBaseRate: 1.0, possibilityWeight: 0, accumulationReference: ReferenceStatistics.AvoidCount, accumulationBaseRate: 10.0, accumulationWeight: 1.5);
            TriggerBaseClass triggerAccumulationHit = new TriggerBaseClass(possibilityBaseRate: 1.0, possibilityWeight: 0, accumulationReference: ReferenceStatistics.AllTotalBeenHitCount, accumulationBaseRate: 10.0, accumulationWeight: 1.5);

            SkillMagnificationClass magnificationNone = new SkillMagnificationClass(attackTarget: TargetType.none, damage: 1.0, kinetic: 1.0, chemical: 1.0,
            thermal: 1.0, heal: 1.0, numberOfAttacks: 1.0, critical: 1.0, accuracy: 1.0, optimumRangeMin: 1.0, optimumRangeMax: 1.0);
            SkillMagnificationClass magnificationNormal = new SkillMagnificationClass(attackTarget: TargetType.multi, damage: 1.0, kinetic: 1.0, chemical: 1.0,
            thermal: 1.0, heal: 1.0, numberOfAttacks: 1.0, critical: 1.0, accuracy: 1.0, optimumRangeMin: 1.0, optimumRangeMax: 1.0);
            SkillMagnificationClass magnificationHeal20 = new SkillMagnificationClass(attackTarget: TargetType.none, damage: 1.0, kinetic: 1.0, chemical: 1.0,
            thermal: 1.0, heal: 2.0, numberOfAttacks: 1.0, critical: 1.0, accuracy: 1.0, optimumRangeMin: 1.0, optimumRangeMax: 1.0);
            SkillMagnificationClass magnificationHeal40 = new SkillMagnificationClass(attackTarget: TargetType.none, damage: 1.0, kinetic: 1.0, chemical: 1.0,
            thermal: 1.0, heal: 4.0, numberOfAttacks: 1.0, critical: 1.0, accuracy: 1.0, optimumRangeMin: 1.0, optimumRangeMax: 1.0);
            SkillMagnificationClass magnificationSingleD05N05CR05AC05 = new SkillMagnificationClass(attackTarget: TargetType.single, damage: 0.5, kinetic: 1.0, chemical: 1.0,
            thermal: 1.0, heal: 3.0, numberOfAttacks: 0.5, critical: 0.5, accuracy: 0.5, optimumRangeMin: 1.0, optimumRangeMax: 1.0);
            SkillMagnificationClass magnificationMultiD075N05CR05AC05 = new SkillMagnificationClass(attackTarget: TargetType.multi, damage: 0.75, kinetic: 1.0, chemical: 1.0,
            thermal: 1.0, heal: 3.0, numberOfAttacks: 0.5, critical: 0.5, accuracy: 0.5, optimumRangeMin: 1.0, optimumRangeMax: 1.0);
            SkillMagnificationClass magnificationMultiD10N05CR05AC075 = new SkillMagnificationClass(attackTarget: TargetType.multi, damage: 0.75, kinetic: 1.0, chemical: 1.0,
            thermal: 1.0, heal: 3.0, numberOfAttacks: 0.5, critical: 0.5, accuracy: 0.75, optimumRangeMin: 1.0, optimumRangeMax: 1.0);
            SkillMagnificationClass magnificationMultiD10N10CR15AC20 = new SkillMagnificationClass(attackTarget: TargetType.multi, damage: 1.00, kinetic: 1.0, chemical: 1.0,
            thermal: 1.0, heal: 3.0, numberOfAttacks: 1.0, critical: 1.5, accuracy: 2.0, optimumRangeMin: 0.5, optimumRangeMax: 2.0);

            TriggerTargetClass triggerTargetNone = new TriggerTargetClass(actionType: ActionType.none, afterAllMoved: false, counter: false, chain: false, reAttack: false, heal: false, move: false,
             majestyAttackType: AttackType.any, critical: CriticalOrNot.any, whoCrushed: ActorOrTargetUnit.no, onlyWhenBeenHitMoreThanOnce: false, onlyWhenAvoidMoreThanOnce: false);
            TriggerTargetClass triggerTargetDamageControl = new TriggerTargetClass(actionType: ActionType.any, afterAllMoved: false, counter: true, chain: true, reAttack: true, heal: false, move: true,
             majestyAttackType: AttackType.any, critical: CriticalOrNot.any, whoCrushed: ActorOrTargetUnit.no, onlyWhenBeenHitMoreThanOnce: false, onlyWhenAvoidMoreThanOnce: false);
            TriggerTargetClass triggerTargetIndependent = new TriggerTargetClass(actionType: ActionType.any, afterAllMoved: true, counter: false, chain: false, reAttack: false, heal: false, move: false,
             majestyAttackType: AttackType.any, critical: CriticalOrNot.any, whoCrushed: ActorOrTargetUnit.no, onlyWhenBeenHitMoreThanOnce: false, onlyWhenAvoidMoreThanOnce: false);
            TriggerTargetClass triggerTargetCounter = new TriggerTargetClass(actionType: ActionType.any, afterAllMoved: true, counter: true, chain: true, reAttack: true, heal: false, move: true,
             majestyAttackType: AttackType.any, critical: CriticalOrNot.nonCritical, whoCrushed: ActorOrTargetUnit.no, onlyWhenBeenHitMoreThanOnce: true, onlyWhenAvoidMoreThanOnce: false);
            TriggerTargetClass triggerTargetChain = new TriggerTargetClass(actionType: ActionType.counter, afterAllMoved: false, counter: false, chain: true, reAttack: false, heal: false, move: true,
             majestyAttackType: AttackType.any, critical: CriticalOrNot.any, whoCrushed: ActorOrTargetUnit.no, onlyWhenBeenHitMoreThanOnce: false, onlyWhenAvoidMoreThanOnce: false);
            TriggerTargetClass triggerTargetCriticalReAttack = new TriggerTargetClass(actionType: ActionType.any, afterAllMoved: true, counter: false, chain: true, reAttack: false, heal: false, move: true,
             majestyAttackType: AttackType.any, critical: CriticalOrNot.critical, whoCrushed: ActorOrTargetUnit.no, onlyWhenBeenHitMoreThanOnce: false, onlyWhenAvoidMoreThanOnce: false);
            TriggerTargetClass triggerTargetInterrupt = new TriggerTargetClass(actionType: ActionType.interrupt, afterAllMoved: false, counter: true, chain: false, reAttack: false, heal: false, move: false,
             majestyAttackType: AttackType.any, critical: CriticalOrNot.any, whoCrushed: ActorOrTargetUnit.no, onlyWhenBeenHitMoreThanOnce: false, onlyWhenAvoidMoreThanOnce: false);

            BuffTargetParameterClass buffTargetNone = new BuffTargetParameterClass(targetType: TargetType.none, barrierRemaining: 0, defenseMagnification: 1.0, mobilityMagnification: 1.0,
            attackMagnification: 1.0, accuracyMagnification: 1.0, criticalHitRateMagnification: 1.0, numberOfAttackMagnification: 1.0, rangeMinCorrection: 0, rangeMaxCorrection: 0);
            BuffTargetParameterClass buffTargetSelf = new BuffTargetParameterClass(targetType: TargetType.self, barrierRemaining: 0, defenseMagnification: 1.0, mobilityMagnification: 1.0,
           attackMagnification: 1.0, accuracyMagnification: 1.0, criticalHitRateMagnification: 1.0, numberOfAttackMagnification: 1.0, rangeMinCorrection: 0, rangeMaxCorrection: 0);
            BuffTargetParameterClass buffTargetMulti = new BuffTargetParameterClass(targetType: TargetType.multi, barrierRemaining: 0, defenseMagnification: 1.0, mobilityMagnification: 1.0,
           attackMagnification: 1.0, accuracyMagnification: 1.0, criticalHitRateMagnification: 1.0, numberOfAttackMagnification: 1.0, rangeMinCorrection: 0, rangeMaxCorrection: 0);
            BuffTargetParameterClass buffTargetMultiBarrier = new BuffTargetParameterClass(targetType: TargetType.multi, barrierRemaining: 0, defenseMagnification: 1.0, mobilityMagnification: 1.0,
           attackMagnification: 1.0, accuracyMagnification: 1.0, criticalHitRateMagnification: 1.0, numberOfAttackMagnification: 1.0, rangeMinCorrection: 0, rangeMaxCorrection: 0);
            BuffTargetParameterClass buffBarrierDefense12 = new BuffTargetParameterClass(targetType: TargetType.none, barrierRemaining: 20, defenseMagnification: 1.1, mobilityMagnification: 1.0,
           attackMagnification: 1.0, accuracyMagnification: 1.0, criticalHitRateMagnification: 1.0, numberOfAttackMagnification: 1.0, rangeMinCorrection: 0, rangeMaxCorrection: 0);
            BuffTargetParameterClass buffBarrier10 = new BuffTargetParameterClass(targetType: TargetType.none, barrierRemaining: 6, defenseMagnification: 1.05, mobilityMagnification: 1.0,
           attackMagnification: 1.0, accuracyMagnification: 1.0, criticalHitRateMagnification: 1.0, numberOfAttackMagnification: 1.0, rangeMinCorrection: 0, rangeMaxCorrection: 0);

            DebuffTargetParameterClass debuffTargetNone = new DebuffTargetParameterClass(targetType: TargetType.none, barrierRemaining: 0, defenseMagnification: 1.0, mobilityMagnification: 1.0,
            attackMagnification: 1.0, accuracyMagnification: 1.0, criticalHitRateMagnification: 1.0, numberOfAttackMagnification: 1.0);

            skillsMasters[0] = new SkillsMasterStruct(name: SkillName.BarrierCounterAvoidManyTimes, actionType: ActionType.counter, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 3, veiledTurn: 20, ability: Ability.generation,
             triggerBase: triggerAccumulationMiddle, magnification: magnificationNone, triggerTarget: triggerTargetCounter, buffTarget: buffTargetSelf, callingBuffName: SkillName.Buffdefense12,
                 debuffTarget: debuffTargetNone);
            skillsMasters[1] = new SkillsMasterStruct(name: SkillName.CounterNonCriticalAttack, actionType: ActionType.counter, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 6, veiledTurn: 20, ability: Ability.responsiveness,
             triggerBase: triggerPossibilityBasic, magnification: magnificationSingleD05N05CR05AC05,
                 triggerTarget: triggerTargetCounter, buffTarget: buffTargetNone, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone);
            skillsMasters[2] = new SkillsMasterStruct(name: SkillName.ChainAllysCounter, actionType: ActionType.chain, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 6, veiledTurn: 20, ability: Ability.responsiveness,
             triggerBase: triggerPossibilityNormal, magnification: magnificationMultiD075N05CR05AC05, triggerTarget: triggerTargetChain, buffTarget: buffTargetNone, callingBuffName: SkillName.none,
             debuffTarget: debuffTargetNone);
            skillsMasters[3] = new SkillsMasterStruct(name: SkillName.FutureSightShot, actionType: ActionType.move, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 6, veiledTurn: 20, ability: Ability.power,
             triggerBase: triggerPossibilityNormal, magnification: magnificationMultiD10N10CR15AC20, triggerTarget: triggerTargetIndependent, buffTarget: buffTargetNone, callingBuffName: SkillName.none,
             debuffTarget: debuffTargetNone);
            skillsMasters[4] = new SkillsMasterStruct(name: SkillName.ReAttackAfterCritical, actionType: ActionType.reAttack, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 6, veiledTurn: 20, ability: Ability.power,
             triggerBase: triggerPossibilityExpert, magnification: magnificationMultiD10N05CR05AC075, triggerTarget: triggerTargetCriticalReAttack,
             buffTarget: buffTargetNone, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone);
            skillsMasters[5] = new SkillsMasterStruct(name: SkillName.InterruptTargetCounterReduceAccuracy, actionType: ActionType.interrupt, callSkillLogicName: CallSkillLogicName.ReduceAccuracy, isHeal: false, usageCount: 4, veiledTurn: 20, ability: Ability.intelligence,
            triggerBase: triggerPossibilityMaster, magnification: magnificationNone, triggerTarget: triggerTargetInterrupt, buffTarget: buffTargetNone, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone);
            // interrupt skill needs coding..
            skillsMasters[6] = new SkillsMasterStruct(name: SkillName.ShiledHealAll, actionType: ActionType.move, callSkillLogicName: CallSkillLogicName.ShieldHealMulti, isHeal: false, usageCount: 1, veiledTurn: 20, ability: Ability.generation,
             triggerBase: triggerPossibility100, magnification: magnificationNone, triggerTarget: triggerTargetDamageControl, buffTarget: buffTargetNone, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone);

            skillsMasters[11] = new SkillsMasterStruct(name: SkillName.ShiledHealplusSingle, actionType: ActionType.move, callSkillLogicName: CallSkillLogicName.ShieldHealSingle, isHeal: true, usageCount: 2, veiledTurn: 20, ability: Ability.generation,
             triggerBase: triggerPossibility100, magnification: magnificationHeal40, triggerTarget: triggerTargetDamageControl, buffTarget: buffTargetNone, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone);
            skillsMasters[12] = new SkillsMasterStruct(name: SkillName.ShiledHealSingle, actionType: ActionType.move, callSkillLogicName: CallSkillLogicName.ShieldHealSingle, isHeal: true, usageCount: 3, veiledTurn: 20, ability: Ability.generation,
             triggerBase: triggerPossibility100, magnification: magnificationHeal20, triggerTarget: triggerTargetDamageControl, buffTarget: buffTargetNone, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone);
            skillsMasters[13] = new SkillsMasterStruct(name: SkillName.BarrierAll, actionType: ActionType.atBeginning, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 1, veiledTurn: 20, ability: Ability.none,
             triggerBase: triggerPossibility100, magnification: magnificationNone, triggerTarget: triggerTargetDamageControl, buffTarget: buffTargetMultiBarrier, callingBuffName: SkillName.Buffbarrier10, debuffTarget: debuffTargetNone);

            // Special Normal attack skill
            skillsMasters[14] = new SkillsMasterStruct(name: SkillName.normalAttack, actionType: ActionType.none, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 1000, veiledTurn: 20, ability: Ability.none,
             triggerBase: triggerPossibility100, magnification: magnificationNormal, triggerTarget: triggerTargetNone, buffTarget: buffTargetNone, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone);

            //Buff
            buffMasters.Add(new SkillsMasterStruct(name: SkillName.Buffdefense12, actionType: ActionType.none, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 0, veiledTurn: 5, ability: Ability.none,
            triggerBase: triggerPossibilityNone, magnification: magnificationNone, triggerTarget: triggerTargetNone, buffTarget: buffBarrierDefense12, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone));
            buffMasters.Add(new SkillsMasterStruct(name: SkillName.Buffbarrier10, actionType: ActionType.none, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 0, veiledTurn: 5, ability: Ability.none,
            triggerBase: triggerPossibilityNone, magnification: magnificationNone, triggerTarget: triggerTargetNone, buffTarget: buffBarrier10, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone));


            //------------------------Battle Main Engine------------------------
            //Logic: Number of Battle
            for (int battleWavesSet = 1; battleWavesSet <= battleWavesSets; battleWavesSet++)
            {
                //set up set info
                List<StatisticsReporterFirstBloodClass> statisticsReporterFirstBlood = new List<StatisticsReporterFirstBloodClass>();
                WhichWin[] statisticsReporterWhichWins = new WhichWin[battleWaves];

                if (battleWavesSet > 1) //  magnification per wave set
                { allyAttackMagnification *= (1 + allyAttackMagnificationPerWavesSet); allyDefenseMagnification *= (1 + allyDefenseMagnificationPerWavesSet); }

                BattleUnit.CombatClass[] combats = new BattleUnit.CombatClass[numberOfCharacters];
                //Ally info
                combats[0] = new BattleUnit.CombatClass(shiledCurrent: 5000, shiledMax: 5000, hitPointCurrent: 10000, hitPointMax: 10000,
                 attack: (int)(542 * allyAttackMagnification), kineticAttackRatio: 0.5, chemicalAttackRatio: 0.1, thermalAttackRatio: 0.4, criticalHit: 30,
                numberOfAttacks: 20, minRange: 5, maxRange: 6, accuracy: 28344, mobility: 1321, deffense: (int)(700 * allyDefenseMagnification));
                combats[1] = new BattleUnit.CombatClass(shiledCurrent: 5000, shiledMax: 5000, hitPointCurrent: 13000, hitPointMax: 13000,
                 attack: (int)(682 * allyAttackMagnification), kineticAttackRatio: 0.7, chemicalAttackRatio: 0.0, thermalAttackRatio: 0.3, criticalHit: 30,
                numberOfAttacks: 14, minRange: 3, maxRange: 3, accuracy: 1344, mobility: 1321, deffense: (int)(600 * allyDefenseMagnification));
                combats[2] = new BattleUnit.CombatClass(shiledCurrent: 5000, shiledMax: 5000, hitPointCurrent: 9800, hitPointMax: 9800,
                 attack: (int)(699 * allyAttackMagnification), kineticAttackRatio: 0.8, chemicalAttackRatio: 0.1, thermalAttackRatio: 0.1, criticalHit: 30,
                numberOfAttacks: 20, minRange: 3, maxRange: 4, accuracy: 1344, mobility: 1321, deffense: (int)(700 * allyDefenseMagnification));
                combats[3] = new BattleUnit.CombatClass(shiledCurrent: 5000, shiledMax: 5000, hitPointCurrent: 11200, hitPointMax: 11200,
                 attack: (int)(832 * allyAttackMagnification), kineticAttackRatio: 1.0, chemicalAttackRatio: 0.0, thermalAttackRatio: 0.0, criticalHit: 30,
                numberOfAttacks: 9, minRange: 3, maxRange: 4, accuracy: 1344, mobility: 1300, deffense: (int)(600 * allyDefenseMagnification));
                combats[4] = new BattleUnit.CombatClass(shiledCurrent: 5000, shiledMax: 5000, hitPointCurrent: 6000, hitPointMax: 6000,
                 attack: (int)(592 * allyAttackMagnification), kineticAttackRatio: 0.6, chemicalAttackRatio: 0.4, thermalAttackRatio: 0.0, criticalHit: 30,
                numberOfAttacks: 10, minRange: 3, maxRange: 4, accuracy: 1344, mobility: 1300, deffense: (int)(700 * allyDefenseMagnification));
                combats[5] = new BattleUnit.CombatClass(shiledCurrent: 4000, shiledMax: 4000, hitPointCurrent: 7800, hitPointMax: 7800,
                 attack: (int)(688 * allyAttackMagnification), kineticAttackRatio: 0.8, chemicalAttackRatio: 0.1, thermalAttackRatio: 0.1, criticalHit: 30,
                numberOfAttacks: 8, minRange: 3, maxRange: 4, accuracy: 1344, mobility: 1321, deffense: (int)(700 * allyDefenseMagnification));
                combats[6] = new BattleUnit.CombatClass(shiledCurrent: 7000, shiledMax: 7000, hitPointCurrent: 6190, hitPointMax: 6190,
                 attack: (int)(642 * allyAttackMagnification), kineticAttackRatio: 0.3, chemicalAttackRatio: 0.7, thermalAttackRatio: 0.0, criticalHit: 30,
                numberOfAttacks: 6, minRange: 3, maxRange: 4, accuracy: 1344, mobility: 1300, deffense: (int)(700 * allyDefenseMagnification));
                //Enemy info
                combats[7] = new BattleUnit.CombatClass(shiledCurrent: 7000, shiledMax: 7000, hitPointCurrent: 12300, hitPointMax: 12300,
                 attack: 800, kineticAttackRatio: 0.0, chemicalAttackRatio: 0.1, thermalAttackRatio: 0.9, criticalHit: 30,
                 numberOfAttacks: 12, minRange: 3, maxRange: 7, accuracy: 1344, mobility: 1200, deffense: 700);
                combats[8] = new BattleUnit.CombatClass(shiledCurrent: 6000, shiledMax: 6000, hitPointCurrent: 13400, hitPointMax: 13400,
                 attack: 880, kineticAttackRatio: 0.0, chemicalAttackRatio: 0.0, thermalAttackRatio: 1.0, criticalHit: 30,
                numberOfAttacks: 11, minRange: 3, maxRange: 7, accuracy: 1344, mobility: 1321, deffense: 700);
                combats[9] = new BattleUnit.CombatClass(shiledCurrent: 6000, shiledMax: 6000, hitPointCurrent: 13100, hitPointMax: 13100,
                 attack: 482, kineticAttackRatio: 1.0, chemicalAttackRatio: 0.0, thermalAttackRatio: 0.0, criticalHit: 30,
                numberOfAttacks: 20, minRange: 3, maxRange: 7, accuracy: 4344, mobility: 1300, deffense: 700);
                combats[10] = new BattleUnit.CombatClass(shiledCurrent: 5000, shiledMax: 5000, hitPointCurrent: 9840, hitPointMax: 9840,
                 attack: 742, kineticAttackRatio: 1.0, chemicalAttackRatio: 0.0, thermalAttackRatio: 0.0, criticalHit: 30,
                numberOfAttacks: 5, minRange: 3, maxRange: 7, accuracy: 1344, mobility: 1300, deffense: 700);
                combats[11] = new BattleUnit.CombatClass(shiledCurrent: 5000, shiledMax: 5000, hitPointCurrent: 7640, hitPointMax: 7640,
                 attack: 732, kineticAttackRatio: 1.0, chemicalAttackRatio: 0.0, thermalAttackRatio: 0.0, criticalHit: 30,
                numberOfAttacks: 3, minRange: 3, maxRange: 7, accuracy: 1344, mobility: 1321, deffense: 700);
                combats[12] = new BattleUnit.CombatClass(shiledCurrent: 4500, shiledMax: 4500, hitPointCurrent: 5600, hitPointMax: 5600,
                 attack: 712, kineticAttackRatio: 1.0, chemicalAttackRatio: 0.0, thermalAttackRatio: 0.0, criticalHit: 30,
                numberOfAttacks: 7, minRange: 3, maxRange: 7, accuracy: 1344, mobility: 1300, deffense: 700);
                combats[13] = new BattleUnit.CombatClass(shiledCurrent: 7500, shiledMax: 7500, hitPointCurrent: 6210, hitPointMax: 6210,
                 attack: 682, kineticAttackRatio: 0.8, chemicalAttackRatio: 0.0, thermalAttackRatio: 0.2, criticalHit: 30,
                numberOfAttacks: 6, minRange: 3, maxRange: 7, accuracy: 1344, mobility: 1210, deffense: 700);

                BattleUnit.FeatureClass featureNormal;
                BattleUnit.FeatureClass featureMedic;

                for (int i = 0; i <= 5; i++)
                {
                    featureNormal = new BattleUnit.FeatureClass(absorbInitial: 0.0, damageControlAssist: false, hateInitial: 10, hateMagnificationPerTurn: 0.666);
                    characters.Add(new BattleUnit(uniqueID: i, name: "PIG" + (i + 1).ToString() + "-" + skillsMasters[i].Name.ToString().Substring(0, 7), affiliation: Affiliation.ally, unitType: UnitType.robot,
                     ability: abilities[i], combat: combats[i], feature: featureNormal,
                         offenseMagnification: offenseMagnification, defenseMagnification: defenseMagnification, skillMagnification: skillMagnificationOffenseDoubleTriggerTriple));
                }
                //Medic only  number 6
                for (int i = 6; i <= 6; i++)
                {
                    featureMedic = new BattleUnit.FeatureClass(absorbInitial: 0.0, damageControlAssist: true, hateInitial: 0, hateMagnificationPerTurn: 0.500);
                    characters.Add(new BattleUnit(uniqueID: i, name: "PIG" + (i + 1).ToString() + "-MedicHe", affiliation: Affiliation.ally, unitType: UnitType.robot,
                     ability: abilities[i], combat: combats[i], feature: featureMedic,
                         offenseMagnification: offenseMagnification, defenseMagnification: defenseMagnification, skillMagnification: skillMagnificationOffenseDoubleTriggerTriple));
                }
                for (int i = 7; i <= 12; i++)
                {
                    featureNormal = new BattleUnit.FeatureClass(absorbInitial: 0.0, damageControlAssist: false, hateInitial: 10, hateMagnificationPerTurn: 0.666);
                    // pigs skill has 8 so  skillsMasters [i - 6 -1 ] collect
                    characters.Add(new BattleUnit(uniqueID: i, name: "ELD" + (i - 6).ToString() + "-" + skillsMasters[i - 7].Name.ToString().Substring(0, 7), affiliation: Affiliation.enemy, unitType: UnitType.cyborg,
                     ability: abilities[i], combat: combats[i], feature: featureNormal,
                    offenseMagnification: offenseMagnification, defenseMagnification: defenseMagnification, skillMagnification: skillMagnificationOffenseDoubleTriggerTriple));
                }
                //Medic only  number 13
                for (int i = 13; i <= 13; i++)
                {
                    featureMedic = new BattleUnit.FeatureClass(absorbInitial: 0.0, damageControlAssist: true, hateInitial: 0, hateMagnificationPerTurn: 0.500);
                    characters.Add(new BattleUnit(uniqueID: i, name: "ELD" + (i - 6).ToString() + "-MedicHe", affiliation: Affiliation.enemy, unitType: UnitType.cyborg,
                     ability: abilities[i], combat: combats[i], feature: featureMedic,
                    offenseMagnification: offenseMagnification, defenseMagnification: defenseMagnification, skillMagnification: skillMagnificationOffenseDoubleTriggerTriple));
                }

                allyWinCount = 0; enemyWinCount = 0; drawCount = 0; // Initialize

                for (int battleWave = 1; battleWave <= battleWaves; battleWave++)
                {
                    bool allyFirstBlood = false;
                    bool enemyFirstBlood = false; // Set up Phase
                    statisticsReporterFirstBlood.Add(new StatisticsReporterFirstBloodClass(battleWave: battleWave));

                    for (int i = 0; i < numberOfCharacters; i++) //Shiled, HitPoint initialize
                    {
                        characters[i].Combat.ShiledCurrent = characters[i].Combat.ShiledMax;
                        characters[i].Combat.HitPointCurrent = characters[i].Combat.HitPointMax;
                        characters[i].Deterioration = 0.0; //Deterioration initialize to 0.0
                        characters[i].Buff.InitializeBuff(); //Buff initialize
                        characters[i].Buff.BarrierRemaining = 0; //Barrier initialize
                        characters[i].Feature.InitializeFeature(); //Feature initialize
                    }
                    EffectInitialize(effects: effects, skillsMasters: skillsMasters, characters: characters); //Effect/Buff initialize

                    if (battleWave == battleWaves && battleWavesSet == battleWavesSets) // only last battle display inside.
                    {
                        foreach (EffectClass effect in effects)
                        {
                            Console.WriteLine(effect.Character.Name + " has a skill: " + effect.Skill.Name + ". [possibility:" + (double)((int)(effect.TriggeredPossibility * 1000) / 10.0) + "%]"
                            + " Left:" + effect.UsageCount + " Offense Magnification:" + effect.OffenseEffectMagnification + " " + effect.Character.Buff.DefenseMagnification);
                        }
                    }
                    // _/_/_/_/_/_/ Effect setting end _/_/_/_/_/_/

                    battleEnd = false;
                    currentBattleWaves = battleWave;

                    while (battleEnd == false)
                    {
                        for (int turn = 1; turn <= 20; turn++)
                        {
                            //------------------------Header Phase------------------------
                            //Battle End check
                            if (battleEnd == true) { continue; } //continue turn.

                            //Effect/Buff initialize and Set again
                            for (int i = 0; i < numberOfCharacters; i++) { characters[i].Buff.InitializeBuff(); }
                            foreach (EffectClass effect in effects) { effect.BuffToCharacter(currentTurn: turn); }

                            //Battle conditions output
                            log += "------------------------------------\n";
                            text = new FuncBattleConditionsText(currentTurn: turn, currentBattleWaves: currentBattleWaves, characters: characters);
                            log += text.Text();

                            //------------------------Action order routine------------------------
                            //Only alive character should be action.
                            List<BattleUnit> aliveCharacters = characters.FindAll(character1 => character1.Combat.HitPointCurrent > 0);

                            //Action order decision for each turn
                            Stack<OrderClass> orders = new Stack<OrderClass>();
                            List<OrderClass> orderForSort = new List<OrderClass>();
                            for (int i = 0; i <= aliveCharacters.Count - 1; i++)
                            {
                                List<EffectClass> effectList = effects.FindAll((obj) => obj.Character == aliveCharacters[i] && obj.Skill.ActionType == ActionType.move
                                && obj.UsageCount > 0 && obj.VeiledFromTurn <= turn && obj.VeiledToTurn >= turn);

                                // Add normal attack skills
                                EffectClass normalAttackEffect = new EffectClass(character: aliveCharacters[i], skill: skillsMasters[14], actionType: ActionType.normalAttack, offenseEffectMagnification: 1.0,
                                triggeredPossibility: 1.0, isDamageControlAssistAble: false, usageCount: 1000, veiledFromTurn: 1, veiledToTurn: 20);
                                effectList.Add(normalAttackEffect);
                                orderForSort.Add(new OrderClass(actor: aliveCharacters[i], actionType: ActionType.move, skillEffectProposed: effectList,
                                actionSpeed: (aliveCharacters[i].Ability.Responsiveness * r.Next(40 + aliveCharacters[i].Ability.Luck, 100)), individualTargetID: -1, isDamageControlAssist: false));
                            }
                            orderForSort.Sort((OrderClass x, OrderClass y) => x.ActionSpeed - y.ActionSpeed);
                            for (int i = 0; i < orderForSort.Count; i++) { orders.Push(orderForSort[i]); }

                            //------------------------Action phase------------------------
                            //Action for each character by action order.

                            // _/_/_/_/_/_/_/_/ At Beginning Skill _/_/_/_/_/_/_/_/_/_/
                            Stack<OrderClass> skillTriggerPossibilityCheck = SkillTriggerPossibilityCheck(actor: null, effects: effects, characters: characters, attackerOrder: null,
                             orders: orders, actionType: ActionType.atBeginning, shouldHeal: false, isDamageControlAssist: false, battleResult: null, individualTargetID: 0, turn: turn, r: r);

                            while (skillTriggerPossibilityCheck != null && skillTriggerPossibilityCheck.Count > 0) { orders.Push(skillTriggerPossibilityCheck.Pop()); }
                            while (orders.Any()) // loop until order is null.
                            {
                                OrderClass order = orders.Pop();
                                if (order.Actor == null) { continue; } // attacker alive check, if crushed, continue.
                                if (order.Actor.Combat.HitPointCurrent <= 0) { continue; }

                                //[[ SKILLS CHECK ]] Interrupt skills triger.

                                //------------------------Indivisual attacker's move phase------------------------

                                //------------------------Attacker's action dicision phase------------------------
                                // BasicAttackFunction basicAttack;
                                BattleResultClass battleResult = new BattleResultClass();
                                (string log, BattleResultClass battleResult) result; //BasicAttackFunction basicAttack;

                                //[[ SKILLS CHECK ]] Move skills triger.
                                order.SkillDecision(characters: characters);

                                //effect spend.
                                if (order.SkillEffectChosen != null)
                                {
                                    EffectClass spendEffect = effects.Find((obj) => obj.Character == order.Actor && obj.Skill.Name == order.SkillEffectChosen.Skill.Name);
                                    if (spendEffect != null) // normal attack is null
                                    {
                                        spendEffect.UsageCount -= 1;
                                        spendEffect.SpentCount += 1;
                                        spendEffect.NextAccumulationCount += (int)(spendEffect.Skill.TriggerBase.AccumulationBaseRate * spendEffect.Skill.TriggerBase.AccumulationWeight);
                                    }
                                }

                                log += BuffDebuffFunction(order: order, characters: characters, effects: effects, buffMasters: buffMasters, turn: turn); //Buff, debuff action
                                log += SkillLogicDispatcher(order: order, characters: characters, r: r); // SkillLogic action include damage control assist.
                                result = SkillMoveFunction(order: order, characters: characters, r: r); // offense action
                                log += result.log;
                                battleResult = result.battleResult;

                                if (order.IsDamageControlAssist) //only when Damage Control Assist
                                {
                                    List<OrderClass> deleteOneActionOrderrIfHave = orders.ToList();
                                    OrderClass deleteOneActionOrderRaw = deleteOneActionOrderrIfHave.FindLast(obj => obj.Actor == order.Actor && obj.ActionType == ActionType.move);
                                    OrderClass deleteOneActionOrder = null;
                                    foreach (EffectClass effect in deleteOneActionOrderRaw.SkillEffectProposed)
                                    { if (effect.Skill.Name == SkillName.normalAttack) { deleteOneActionOrder = deleteOneActionOrderRaw; } }
                                    deleteOneActionOrderrIfHave.Remove(deleteOneActionOrder);
                                    deleteOneActionOrderrIfHave.Reverse();
                                    if (deleteOneActionOrder != null) //clear stack and input again
                                    { orders.Clear(); foreach (OrderClass data in deleteOneActionOrderrIfHave) { orders.Push(data); } }
                                }
                                //Only when first kill happend, insert statistics reporter for first blood per side.
                                if (order.Actor.Affiliation == Affiliation.ally && allyFirstBlood == false && battleResult.NumberOfCrushed >= 1) //When ally first blood 
                                {
                                    string es = null;
                                    if (battleResult.NumberOfCrushed != 1) { es = "es"; }
                                    string t = "Turn:" + turn + " " + order.Actor.Name + "'s " + order.SkillEffectChosen.Skill.Name + ". first blood! total dealt damage:" + battleResult.TotalDeltDamage.WithComma() + " " + battleResult.NumberOfCrushed.WithComma() + " crush" + es + ".";
                                    StatisticsReporterFirstBloodClass setStatisticsReporterFirstBlood = statisticsReporterFirstBlood.FindLast((obj) => obj.BattleWave == battleWave);
                                    setStatisticsReporterFirstBlood.AllyCharacterName = order.Actor.Name;
                                    setStatisticsReporterFirstBlood.AllyActionType = order.ActionType;
                                    setStatisticsReporterFirstBlood.AllyHappenedTurn = turn;
                                    setStatisticsReporterFirstBlood.AllyCrushedCount = battleResult.NumberOfCrushed;
                                    setStatisticsReporterFirstBlood.AllyTotalDealtDamage = battleResult.TotalDeltDamage;
                                    setStatisticsReporterFirstBlood.AllyContentText = t;
                                    allyFirstBlood = true;

                                }
                                else if (order.Actor.Affiliation == Affiliation.enemy && enemyFirstBlood == false && battleResult.NumberOfCrushed >= 1) //When enemy first blood 
                                {
                                    string es = null;
                                    if (battleResult.NumberOfCrushed != 1) { es = "es"; }
                                    string t = "Turn:" + turn + " " + order.Actor.Name + "'s " + order.SkillEffectChosen.Skill.Name + ". first blood! total dealt damage:" + battleResult.TotalDeltDamage.WithComma() + " " + battleResult.NumberOfCrushed.WithComma() + " crush" + es + ".";
                                    StatisticsReporterFirstBloodClass setStatisticsReporterFirstBlood = statisticsReporterFirstBlood.FindLast((obj) => obj.BattleWave == battleWave);
                                    setStatisticsReporterFirstBlood.EnemyCharacterName = order.Actor.Name;
                                    setStatisticsReporterFirstBlood.EnemyActionType = order.ActionType;
                                    setStatisticsReporterFirstBlood.EnemyHappenedTurn = turn;
                                    setStatisticsReporterFirstBlood.EnemyCrushedCount = battleResult.NumberOfCrushed;
                                    setStatisticsReporterFirstBlood.EnemyTotalDealtDamage = battleResult.TotalDeltDamage;
                                    setStatisticsReporterFirstBlood.EnemyContentText = t;
                                    enemyFirstBlood = true;
                                }

                                if (battleEnd == false)
                                {
                                    battleEnd = battleResult.BattleEnd;
                                    if (battleResult.IsAllyWin == true) { allyWinCount++; statisticsReporterWhichWins[battleWave - 1] = WhichWin.allyWin; }
                                    if (battleResult.IsEnemyWin) { enemyWinCount++; statisticsReporterWhichWins[battleWave - 1] = WhichWin.enemyWin; }
                                    if (battleResult.IsDraw) { drawCount++; statisticsReporterWhichWins[battleWave - 1] = WhichWin.Draw; }
                                }

                                //[[ SKILLS CHECK ]] Counter skills triger.
                                skillTriggerPossibilityCheck = SkillTriggerPossibilityCheck(actor: null, effects: effects, characters: characters,
                                 attackerOrder: order, orders: orders, actionType: ActionType.counter, shouldHeal: false, isDamageControlAssist: false,
                                    battleResult: battleResult, individualTargetID: order.Actor.UniqueID, turn: turn, r: r);
                                while (skillTriggerPossibilityCheck != null && skillTriggerPossibilityCheck.Count > 0) { orders.Push(skillTriggerPossibilityCheck.Pop()); }

                                //[[ SKILLS CHECK ]] Chain skills trigger.
                                skillTriggerPossibilityCheck = SkillTriggerPossibilityCheck(actor: null, effects: effects, characters: characters,
                                 attackerOrder: order, orders: orders, actionType: ActionType.chain, shouldHeal: false, isDamageControlAssist: false,
                                  battleResult: battleResult, individualTargetID: order.Actor.UniqueID, turn: turn, r: r);
                                while (skillTriggerPossibilityCheck != null && skillTriggerPossibilityCheck.Count > 0) { orders.Push(skillTriggerPossibilityCheck.Pop()); }

                                //[[ SKILLS CHECK ]] ReAttack skills trigger.
                                skillTriggerPossibilityCheck = SkillTriggerPossibilityCheck(actor: null, effects: effects, characters: characters,
                                 attackerOrder: order, orders: orders, actionType: ActionType.reAttack, shouldHeal: false, isDamageControlAssist: false,
                                battleResult: battleResult, individualTargetID: order.Actor.UniqueID, turn: turn, r: r);
                                while (skillTriggerPossibilityCheck != null && skillTriggerPossibilityCheck.Count > 0) { orders.Push(skillTriggerPossibilityCheck.Pop()); }

                                //[[ SKILLS CHECK ]] Damage Control Assist trigger. Note: ActionType independent so ActionType.any!
                                skillTriggerPossibilityCheck = SkillTriggerPossibilityCheck(actor: null, effects: effects, characters: characters,
                                 attackerOrder: order, orders: orders, actionType: ActionType.any, shouldHeal: true, isDamageControlAssist: true,
                                battleResult: battleResult, individualTargetID: order.Actor.UniqueID, turn: turn, r: r);
                                while (skillTriggerPossibilityCheck != null && skillTriggerPossibilityCheck.Count > 0) { orders.Push(skillTriggerPossibilityCheck.Pop()); }

                            }  // Until all Characters act.

                            //------------------------Footer phase------------------------
                            //Heal Shiled by generation %
                            ShiledHealFunction shiledHeal = new ShiledHealFunction(characters: characters);
                            log += shiledHeal.Log;
                            CalculationHateMagnificationPerTurnFunction camlHate = new CalculationHateMagnificationPerTurnFunction(characters: characters);
                            log += camlHate.Log;
                            foreach (BattleUnit character in characters) { if (character.IsCrushedJustNow) { character.IsCrushedJustNow = false; } } // reset IsCrushedJustNow flag

                            //Check wipe out and should continue the battle
                            if (battleEnd == false)
                            {
                                wipeOutCheck = new WipeOutCheck(characters);
                                if (wipeOutCheck.IsAllyWin) { allyWinCount++; }
                                if (wipeOutCheck.IsEnemyWin) { enemyWinCount++; }
                                if (wipeOutCheck.IsDraw) { drawCount++; }
                                battleEnd = wipeOutCheck.BatleEnd;
                            }
                            totalturn = turn; // If battle end, set total turn for show end log.

                        } //turn

                        //------------------------Statistics phase------------------------

                        //time over NEED statistics reporter and FIX BUG "no count"!
                        //Check wipe out and should continue the battle
                        if (battleEnd == false) { drawCount++; battleEnd = true; log += "Time over. \n"; }

                    } //battleEnd
                    // only final battle log display.
                    finalLog = log;
                    log = null;

                } //Battle waves

                subLogPerWavesSets[battleWavesSet - 1] += "[Set:" + battleWavesSet + "] Battle count:" + (allyWinCount + enemyWinCount + drawCount) + " Win:" + (allyWinCount) + " lost:" + (enemyWinCount)
                    + " Win Ratio:" + (int)((double)allyWinCount / (double)(allyWinCount + enemyWinCount + drawCount) * 100)
                    + "% Ally:[Attack x" + Math.Round(allyAttackMagnification, 2) + "] [Defense x" + Math.Round(allyDefenseMagnification, 2) + "] \n";
                //statistics reporter open
                for (int battleWave = 1; battleWave <= battleWaves; battleWave++)
                {
                    StatisticsReporterFirstBloodClass setStatisticsReporterFirstBlood = statisticsReporterFirstBlood.FindLast((obj) => obj.BattleWave == battleWave);
                    setStatisticsReporterFirstBlood.WhichWin = statisticsReporterWhichWins[battleWave - 1];
                }
                var statisticsQueryAlly = statisticsReporterFirstBlood.Where(x => x.WhichWin == WhichWin.allyWin)
                    .GroupBy(x => x.AllyCharacterName).Select(x => new { Subj = x.Key, Count = x.Count() }).OrderByDescending(x => x.Count);
                var statisticsQueryEnemy = statisticsReporterFirstBlood.Where(x => x.WhichWin == WhichWin.enemyWin)
                    .GroupBy(x => x.EnemyCharacterName).Select(x => new { Subj = x.Key, Count = x.Count() }).OrderByDescending(x => x.Count);

                logPerWavesSets[battleWavesSet - 1] += "Ally info: MVP(times)\n";
                foreach (var group in statisticsQueryAlly) { logPerWavesSets[battleWavesSet - 1] += new string(' ', 2) + group.Subj + " (" + group.Count + ")."; }
                logPerWavesSets[battleWavesSet - 1] += "\n";
                if (statisticsReporterFirstBlood.FindAll((obj) => obj.WhichWin == WhichWin.allyWin).Any())
                {
                    StatisticsReporterFirstBloodClass bestFirstBloodAlly = statisticsReporterFirstBlood.FindAll((obj) => obj.WhichWin == WhichWin.allyWin).OrderByDescending(obj => obj.AllyTotalDealtDamage).First();
                    logPerWavesSets[battleWavesSet - 1] += "[Best shot] Waves: " + bestFirstBloodAlly.BattleWave + " " + bestFirstBloodAlly.AllyContentText + "\n";
                }
                logPerWavesSets[battleWavesSet - 1] += "Enemy info: MVP(times) \n";
                foreach (var group in statisticsQueryEnemy) { logPerWavesSets[battleWavesSet - 1] += new string(' ', 2) + group.Subj + " (" + group.Count + ")."; }
                logPerWavesSets[battleWavesSet - 1] += "\n";
                if (statisticsReporterFirstBlood.FindAll((obj) => obj.WhichWin == WhichWin.enemyWin).Any())
                {
                    StatisticsReporterFirstBloodClass bestFirstBloodEnemy = statisticsReporterFirstBlood.FindAll((obj) => obj.WhichWin == WhichWin.enemyWin).OrderByDescending(obj => obj.EnemyTotalDealtDamage).First();
                    logPerWavesSets[battleWavesSet - 1] += "[Best shot] Waves: " + bestFirstBloodEnemy.BattleWave + " " + bestFirstBloodEnemy.EnemyContentText + "\n";
                }
                //Characters Statistics Collection
                foreach (BattleUnit character in characters) { character.StatisticsCollection.Avarage(battleWaves: battleWaves); } // Avarage Calculation
                logPerWavesSets[battleWavesSet - 1] += "Avarage (critical):\n";
                foreach (BattleUnit character in characters) { logPerWavesSets[battleWavesSet - 1] += new string(' ', 1) + character.Name + " " + character.StatisticsCollection.AllCriticalRatio() + "\n"; }
                logPerWavesSets[battleWavesSet - 1] += "Avarage Skill:\n";
                foreach (BattleUnit character in characters) { logPerWavesSets[battleWavesSet - 1] += character.Name + " " + character.StatisticsCollection.Skill() + "\n"; }
                logPerWavesSets[battleWavesSet - 1] += "------------------------------------\n";


            } // Battle waves set

            //Battle is over.
            finalLog += "------------------------------------\n";
            finalLog += "Battle is over. ";
            text = new FuncBattleConditionsText(currentTurn: totalturn, currentBattleWaves: currentBattleWaves, characters: characters);
            finalLog += text.Text();

            for (int i = 0; i < battleWavesSets; i++) { finalLog += logPerWavesSets[i]; }
            finalLog += " Ally attack magnification per waves set: x" + (1 + allyAttackMagnificationPerWavesSet) + "\n";
            finalLog += " Ally defense magnification per waves set: x" + (1 + allyDefenseMagnificationPerWavesSet) + "\n";
            for (int i = 0; i < battleWavesSets; i++) { finalLog += subLogPerWavesSets[i]; }

            DateTime finishDateTime = DateTime.Now;
            TimeSpan processedTimeSpan = finishDateTime - startDateTime;
            finalLog += "finished:" + finishDateTime + " processed time:" + processedTimeSpan + "\n";

            Console.WriteLine(finalLog);
        }

        // Skill check method
        public static Stack<OrderClass> SkillTriggerPossibilityCheck(BattleUnit actor, List<EffectClass> effects, List<BattleUnit> characters, OrderClass attackerOrder,
        Stack<OrderClass> orders, ActionType actionType, bool shouldHeal, bool isDamageControlAssist,
            BattleResultClass battleResult, int individualTargetID, int turn, Random r)
        {
            if (attackerOrder != null && attackerOrder.IsDamageControlAssist) { return null; } //If previous move is Damage Control Assist, no counter, reattack, chain and Damage control assist is triggered.
            List<EffectClass> rawActionTypeEffects;
            if (isDamageControlAssist) // Damage control assist is ActionType independent
            {
                rawActionTypeEffects = effects.FindAll((obj) => obj.UsageCount > 0 && obj.Character.Combat.HitPointCurrent > 0 &&
                obj.Skill.IsHeal == shouldHeal && obj.VeiledFromTurn <= turn && obj.VeiledToTurn >= turn);
            }
            else
            {
                if (shouldHeal) //if heal has, be selected.
                {
                    rawActionTypeEffects = effects.FindAll((obj) => obj.ActionType == actionType && obj.UsageCount > 0 && obj.Character.Combat.HitPointCurrent > 0 &&
                    obj.Skill.IsHeal == shouldHeal && obj.VeiledFromTurn <= turn && obj.VeiledToTurn >= turn);
                    if (!rawActionTypeEffects.Any()) //if no heal skill left, other move skill should be selected.
                    {
                        rawActionTypeEffects = effects.FindAll((obj) => obj.ActionType == actionType && obj.UsageCount > 0 && obj.Character.Combat.HitPointCurrent > 0 &&
                        obj.Skill.IsHeal == !shouldHeal && obj.VeiledFromTurn <= turn && obj.VeiledToTurn >= turn);
                    }
                }
                else // should not heal, so find other move skill.
                {
                    rawActionTypeEffects = effects.FindAll((obj) => obj.ActionType == actionType && obj.UsageCount > 0 && obj.Character.Combat.HitPointCurrent > 0 &&
                    obj.Skill.IsHeal == shouldHeal && obj.VeiledFromTurn <= turn && obj.VeiledToTurn >= turn);
                }
            }
            List<EffectClass> matchedActionTypeEffects = new List<EffectClass>();
            Affiliation counterAffiliation = Affiliation.ally;
            if (attackerOrder != null) //Memo: at Beginning and move skills, attackOrder is null.
            { if (attackerOrder.Actor.Affiliation == Affiliation.ally) { counterAffiliation = Affiliation.enemy; } else { counterAffiliation = Affiliation.ally; } }

            Affiliation affiliationWhoWillAct = Affiliation.none;
            switch (actionType) // Get actionType dependent condition before calculation.
            {
                case ActionType.move: //Normal moveskill logic: only actor should trigger moveskill.
                    matchedActionTypeEffects = rawActionTypeEffects.FindAll(obj => obj.Character == actor);
                    break;
                case ActionType.counter:
                    if (attackerOrder.Actor.Affiliation == Affiliation.ally) { affiliationWhoWillAct = Affiliation.enemy; }
                    else if (attackerOrder.Actor.Affiliation == Affiliation.enemy) { affiliationWhoWillAct = Affiliation.ally; }
                    matchedActionTypeEffects = rawActionTypeEffects.FindAll((obj) => obj.Character.Affiliation == affiliationWhoWillAct);
                    break;
                case ActionType.chain:
                    matchedActionTypeEffects = rawActionTypeEffects.FindAll((obj) => obj.Character.Affiliation == attackerOrder.Actor.Affiliation && obj.Character != attackerOrder.Actor);
                    break;
                case ActionType.reAttack:
                    matchedActionTypeEffects = rawActionTypeEffects.FindAll((obj) => obj.Character == attackerOrder.Actor);
                    break;
                case ActionType.atBeginning:
                    matchedActionTypeEffects = rawActionTypeEffects;
                    break;
                case ActionType.any: //[Damage Control Assist skill logic]. ActionType independent so DCA is in ActionType.any.
                    if (isDamageControlAssist) // Damage Control Assist skill logic
                    {
                        // Actor's affiliation character is dead just now?
                        List<BattleUnit> crushedJustNowCounterAffiliationCharacter = characters.FindAll(obj => obj.IsCrushedJustNow == true && obj.Affiliation == counterAffiliation);
                        if (crushedJustNowCounterAffiliationCharacter.Count > 0) // Damage Control Assist required!
                        {
                            matchedActionTypeEffects = rawActionTypeEffects.FindAll(obj => obj.Character.Affiliation == counterAffiliation && obj.Character.Feature.DamageControlAssist == true);
                            Console.WriteLine(turn + " DMC needed! :" + attackerOrder.Actor.Name + " with " + attackerOrder.SkillEffectChosen.Skill.Name + " " + matchedActionTypeEffects.Count);
                        }

                        // incase of friendly fired.
                        List<BattleUnit> crushedJustNowbyFriendlyFiredCharacter = characters.FindAll(obj => obj.IsCrushedJustNow == true && obj.Affiliation == attackerOrder.Actor.Affiliation);
                        if (crushedJustNowbyFriendlyFiredCharacter.Count > 0)
                        { matchedActionTypeEffects = rawActionTypeEffects.FindAll(obj => obj.Character.Affiliation == attackerOrder.Actor.Affiliation && obj.Character.Feature.DamageControlAssist == true); }
                    }
                    break;
                default:
                    matchedActionTypeEffects = new List<EffectClass>();
                    break;
            }

            //push order from slow character's effect to fast character's effect. It means pop from fast character's effect to slow character's effect.
            matchedActionTypeEffects.Sort((EffectClass x, EffectClass y) => y.Character.Ability.Responsiveness - x.Character.Ability.Responsiveness);

            List<EffectClass> validEffects = new List<EffectClass>();
            foreach (EffectClass effect in matchedActionTypeEffects)
            {
                EffectClass filteredEffect = new EffectClass(character: effect.Character, skill: effect.Skill, actionType: effect.ActionType,
                offenseEffectMagnification: effect.OffenseEffectMagnification, triggeredPossibility: effect.TriggeredPossibility, isDamageControlAssistAble: effect.IsDamageControlAssistAble, usageCount: effect.UsageCount,
                 veiledFromTurn: effect.VeiledFromTurn, veiledToTurn: effect.VeiledToTurn);
                if (effect.Skill.TriggerTarget.ActionType != ActionType.any) { if ((effect.Skill.TriggerTarget.ActionType == attackerOrder.ActionType) == false) { continue; } } // Trigger condition check
                if (effect.Skill.ActionType != ActionType.move && effect.Skill.TriggerTarget.AfterAllMoved == false) // check normal Attack left, except move skill. (move skill is itself so check this logic make no sense )
                {
                    List<OrderClass> checkOrders = orders.ToList();
                    if (checkOrders.FindLast((obj) => obj.Actor == effect.Character && obj.SkillEffectChosen.Skill.Name == SkillName.normalAttack) == null) { continue; }// no normalAttack left, whitch means no action.   
                }

                if (effect.Skill.ActionType == ActionType.move && effect.IsDamageControlAssistAble) //Damage Control Assist Special Logic....
                {
                    // check normal Attack left, except move skill. (move skill is itself so check this logic make no sense )
                    if (effect.Skill.TriggerTarget.AfterAllMoved == false)
                    {
                        List<OrderClass> checkActionOrders = orders.ToList();
                        if (checkActionOrders.FindLast((obj) => obj.Actor == effect.Character && obj.ActionType == ActionType.move) == null)
                        { continue; }// no normalAttack left, whitch means no action.   
                    }
                }

                if (attackerOrder != null) // only attackOrder exist, check
                {
                    if (effect.Skill.TriggerTarget.Counter == false && attackerOrder.ActionType == ActionType.counter) { continue; } // counter reaction
                    if (effect.Skill.TriggerTarget.Chain == false && attackerOrder.ActionType == ActionType.chain) { continue; } // chain reaction
                    if (effect.Skill.TriggerTarget.ReAttack == false && attackerOrder.ActionType == ActionType.reAttack) { continue; } // reAttack reaction
                    if (effect.Skill.TriggerTarget.Move == false && attackerOrder.ActionType == ActionType.move) { continue; } // move skill reaction
                }

                // AttackType MajestyAttackType NO IMPLEMENTATION.

                if (effect.Skill.TriggerTarget.Critical != CriticalOrNot.any)
                {
                    if (effect.Skill.TriggerTarget.Critical == CriticalOrNot.critical && battleResult.CriticalOrNot == CriticalOrNot.nonCritical) { continue; } // non critical but only when critical triggers
                    if (effect.Skill.TriggerTarget.Critical == CriticalOrNot.nonCritical && battleResult.CriticalOrNot == CriticalOrNot.critical) { continue; } // critical but only when non critical triggers
                }

                //ActorOrTargetUnit WhoCrushed   NO IMPLEMENTATION.

                if (effect.Skill.TriggerTarget.OnlyWhenBeenHitMoreThanOnce) // Been hit trigger check.
                {
                    BattleUnit checkCharacter = battleResult.HitMoreThanOnceCharacters.Find((obj) => obj == effect.Character);
                    if (checkCharacter == null) { continue; }//this means not hit, so skill should not be triggered.
                }

                if (effect.Skill.TriggerTarget.OnlyWhenAvoidMoreThanOnce) // Been avoid trigger check.
                {
                    BattleUnit checkCharacter = battleResult.AvoidMoreThanOnceCharacters.Find((obj) => obj == effect.Character);
                    if (checkCharacter != null) { continue; }//this means not hit, so skill should not be triggered.
                }

                switch (effect.Skill.TriggerBase.AccumulationReference) //Trigger Accumulation check
                {
                    case ReferenceStatistics.none: break;
                    case ReferenceStatistics.AvoidCount: if (effect.Character.StatisticsCollection.AvoidCount < effect.NextAccumulationCount) { continue; } break;
                    case ReferenceStatistics.AllHitCount: if (effect.Character.StatisticsCollection.AllHitCount < effect.NextAccumulationCount) { continue; } break;
                    case ReferenceStatistics.AllTotalBeenHitCount: if (effect.Character.StatisticsCollection.AllTotalBeenHitCount < effect.NextAccumulationCount) { continue; } break;
                    case ReferenceStatistics.CriticalBeenHitCount: if (effect.Character.StatisticsCollection.CriticalBeenHitCount < effect.NextAccumulationCount) { continue; } break;
                    case ReferenceStatistics.CriticalHitCount: if (effect.Character.StatisticsCollection.CriticalHitCount < effect.NextAccumulationCount) { continue; } break;
                    case ReferenceStatistics.SkillBeenHitCount: if (effect.Character.StatisticsCollection.SkillBeenHitCount < effect.NextAccumulationCount) { continue; } break;
                    case ReferenceStatistics.SkillHitCount: if (effect.Character.StatisticsCollection.SkillHitCount < effect.NextAccumulationCount) { continue; } break;
                    default: break;
                }

                double possibility = (double)(r.Next(0, 1000)) / 1000.0; //TriggerPossibility Check
                if (effect.TriggeredPossibility >= possibility) { validEffects.Add(effect); }
            }

            //console
            if (isDamageControlAssist && validEffects.Count > 0) { Console.WriteLine(" DMC validEffect:" + validEffects.Count + " " + validEffects[0].Character.Name + " " + validEffects[0].Skill.Name); }

            //set order  grouped by actors
            List<EffectClass> validEffectsPerActor;
            OrderClass skillsByOrder;
            Stack<OrderClass> skillsByOrderStack = new Stack<OrderClass>();
            foreach (BattleUnit character in characters)
            {
                validEffectsPerActor = validEffects.FindAll(obj => obj.Character == character);
                if (validEffectsPerActor.Count >= 1)
                {
                    skillsByOrder = new OrderClass(actor: character, actionType: actionType, skillEffectProposed: validEffectsPerActor, actionSpeed: 0,
                     individualTargetID: individualTargetID, isDamageControlAssist: isDamageControlAssist);
                    skillsByOrderStack.Push(skillsByOrder);
                }
            }
            if (skillsByOrderStack.Count > 0) { return skillsByOrderStack; }
            return null;
        }

        // Skills Possibility Rate calculation
        public static double TriggerPossibilityRate(SkillsMasterStruct skillsMaster, BattleUnit character)
        {
            double abilityValue = 0.0;
            double magnificationRate = 1.0;
            switch (skillsMaster.Ability)
            {
                case Ability.power: abilityValue = (double)character.Ability.Power; break;
                case Ability.generation: abilityValue = (double)character.Ability.Generation; break;
                case Ability.responsiveness: abilityValue = (double)character.Ability.Responsiveness; break;
                case Ability.intelligence: abilityValue = (double)character.Ability.Intelligence; break;
                case Ability.precision: abilityValue = (double)character.Ability.Precision; break;
                case Ability.luck: abilityValue = (double)character.Ability.Luck; break;
                case Ability.none: abilityValue = 0.0; break;
                default: break;
            }

            switch (skillsMaster.ActionType)
            {
                case ActionType.counter: magnificationRate = character.SkillMagnification.TriggerPossibility.Counter; break;
                case ActionType.chain: magnificationRate = character.SkillMagnification.TriggerPossibility.Chain; break;
                case ActionType.reAttack: magnificationRate = character.SkillMagnification.TriggerPossibility.ReAttack; break;
                case ActionType.move: magnificationRate = character.SkillMagnification.TriggerPossibility.Move; break;
                case ActionType.interrupt: magnificationRate = character.SkillMagnification.TriggerPossibility.Interrupt; break;
                case ActionType.atBeginning: magnificationRate = character.SkillMagnification.TriggerPossibility.AtBeginning; break;
                case ActionType.atEnding: magnificationRate = character.SkillMagnification.TriggerPossibility.AtEnding; break;
                default: break;
            }

            double value = Math.Pow(((skillsMaster.TriggerBase.PossibilityBaseRate + abilityValue /
                     (100.0 + abilityValue * 2 * skillsMaster.TriggerBase.PossibilityWeight)) * 100), 3.0) / 40000 * magnificationRate;
            if (value > 1.0) { value = 1.0; }
            return value;
        }

        // Skills offenseEffectMagnification calculation
        public static double EffectPower(SkillsMasterStruct skillsMaster, BattleUnit character)
        {
            double magnificationRate = 1.0;
            switch (skillsMaster.ActionType)
            {
                case ActionType.counter: magnificationRate = character.SkillMagnification.OffenseEffectPower.Counter; break;
                case ActionType.chain: magnificationRate = character.SkillMagnification.OffenseEffectPower.Chain; break;
                case ActionType.reAttack: magnificationRate = character.SkillMagnification.OffenseEffectPower.ReAttack; break;
                case ActionType.move: magnificationRate = character.SkillMagnification.OffenseEffectPower.Move; break;
                case ActionType.interrupt: magnificationRate = character.SkillMagnification.OffenseEffectPower.Interrupt; break;
                case ActionType.atBeginning: magnificationRate = character.SkillMagnification.OffenseEffectPower.AtBeginning; break;
                case ActionType.atEnding: magnificationRate = character.SkillMagnification.OffenseEffectPower.AtEnding; break;
                default: break;
            }
            return 1.0 * magnificationRate;
        }

        public static void EffectInitialize(List<EffectClass> effects, SkillsMasterStruct[] skillsMasters, List<BattleUnit> characters)
        {
            double[] TriggerPossibility = new double[characters.Count];
            double[] OffenseEffectMagnification = new double[characters.Count];
            //Effect/Buff initialize
            effects.Clear();
            //for (int i = effects.Count - 1; i >= 0; i--) { effects.RemoveAt(i); }

            for (int i = 0; i < 7; i++)
            {
                TriggerPossibility[i] = TriggerPossibilityRate(skillsMaster: skillsMasters[i], character: characters[i]);
                OffenseEffectMagnification[i] = EffectPower(skillsMaster: skillsMasters[i], character: characters[i]);
                EffectClass effect = new EffectClass(character: characters[i], skill: skillsMasters[i], actionType: skillsMasters[i].ActionType,
                 offenseEffectMagnification: OffenseEffectMagnification[i],
                triggeredPossibility: TriggerPossibility[i], isDamageControlAssistAble: false, usageCount: skillsMasters[i].UsageCount, veiledFromTurn: 1, veiledToTurn: 20);
                effects.Add(effect);
            }
            for (int i = 7; i < 14; i++)
            {
                TriggerPossibility[i] = TriggerPossibilityRate(skillsMaster: skillsMasters[i - 7], character: characters[i]);
                OffenseEffectMagnification[i] = EffectPower(skillsMaster: skillsMasters[i - 7], character: characters[i]);
                EffectClass effect = new EffectClass(character: characters[i], skill: skillsMasters[i - 7], actionType: skillsMasters[i - 7].ActionType,
                 offenseEffectMagnification: OffenseEffectMagnification[i],
                triggeredPossibility: TriggerPossibility[i], isDamageControlAssistAble: false, usageCount: skillsMasters[i - 7].UsageCount, veiledFromTurn: 1, veiledToTurn: 20);
                effects.Add(effect);
            }

            //Special add Eld4 to counter skill
            EffectClass secondEffect = new EffectClass(character: characters[10], skill: skillsMasters[1], actionType: skillsMasters[1].ActionType,
 offenseEffectMagnification: OffenseEffectMagnification[10],
triggeredPossibility: TriggerPossibilityRate(skillsMaster: skillsMasters[1], character: characters[10]), isDamageControlAssistAble: false, usageCount: skillsMasters[1].UsageCount, veiledFromTurn: 1, veiledToTurn: 20);
            effects.Add(secondEffect);

            //Special add to PIG7 and ELD7 to ShiledHealAll, ShiledHealSingle and ShiledHealPlusSingle
            //ShiledHealAll
            secondEffect = new EffectClass(character: characters[6], skill: skillsMasters[13], actionType: skillsMasters[13].ActionType,
 offenseEffectMagnification: OffenseEffectMagnification[6],
triggeredPossibility: TriggerPossibilityRate(skillsMaster: skillsMasters[13], character: characters[6]), isDamageControlAssistAble: true, usageCount: skillsMasters[13].UsageCount, veiledFromTurn: 1, veiledToTurn: 20);
            effects.Add(secondEffect);
            secondEffect = new EffectClass(character: characters[13], skill: skillsMasters[13], actionType: skillsMasters[13].ActionType,
offenseEffectMagnification: OffenseEffectMagnification[13],
triggeredPossibility: TriggerPossibilityRate(skillsMaster: skillsMasters[13], character: characters[13]), isDamageControlAssistAble: true, usageCount: skillsMasters[13].UsageCount, veiledFromTurn: 1, veiledToTurn: 20);
            effects.Add(secondEffect);
            //ShiledHealSingle
            secondEffect = new EffectClass(character: characters[6], skill: skillsMasters[12], actionType: skillsMasters[12].ActionType,
 offenseEffectMagnification: OffenseEffectMagnification[6],
triggeredPossibility: TriggerPossibilityRate(skillsMaster: skillsMasters[12], character: characters[6]), isDamageControlAssistAble: true, usageCount: skillsMasters[12].UsageCount, veiledFromTurn: 1, veiledToTurn: 20);
            effects.Add(secondEffect);
            secondEffect = new EffectClass(character: characters[13], skill: skillsMasters[12], actionType: skillsMasters[12].ActionType,
offenseEffectMagnification: OffenseEffectMagnification[12],
triggeredPossibility: TriggerPossibilityRate(skillsMaster: skillsMasters[12], character: characters[13]), isDamageControlAssistAble: true, usageCount: skillsMasters[12].UsageCount, veiledFromTurn: 1, veiledToTurn: 20);
            effects.Add(secondEffect);
            //ShiledHealPlusSingle
            secondEffect = new EffectClass(character: characters[6], skill: skillsMasters[11], actionType: skillsMasters[11].ActionType,
offenseEffectMagnification: OffenseEffectMagnification[6],
triggeredPossibility: TriggerPossibilityRate(skillsMaster: skillsMasters[11], character: characters[6]), isDamageControlAssistAble: true, usageCount: skillsMasters[11].UsageCount, veiledFromTurn: 1, veiledToTurn: 20);
            effects.Add(secondEffect);
            secondEffect = new EffectClass(character: characters[13], skill: skillsMasters[11], actionType: skillsMasters[12].ActionType,
offenseEffectMagnification: OffenseEffectMagnification[12],
triggeredPossibility: TriggerPossibilityRate(skillsMaster: skillsMasters[11], character: characters[13]), isDamageControlAssistAble: true, usageCount: skillsMasters[11].UsageCount, veiledFromTurn: 1, veiledToTurn: 20);
            effects.Add(secondEffect);
            foreach (EffectClass effect in effects) { effect.BuffToCharacter(currentTurn: 1); }
        }

        // Buff logic
        public static string BuffDebuffFunction(OrderClass order, List<BattleUnit> characters, List<EffectClass> effects, List<SkillsMasterStruct> buffMasters, int turn)
        {
            SkillsMasterStruct addingBuff;
            List<EffectClass> addingEffect = new List<EffectClass>();
            string log = null;
            if (order.SkillEffectChosen == null) { return log; } // no effect exist, so no buff/ debuff happened
            switch (order.SkillEffectChosen.Skill.BuffTarget.TargetType)
            {
                case TargetType.self: //Buff self
                    addingBuff = buffMasters.FindLast((obj) => obj.Name == order.SkillEffectChosen.Skill.CallingBuffName);
                    addingEffect.Add(new EffectClass(character: order.Actor, skill: addingBuff, actionType: ActionType.none,
                    offenseEffectMagnification: 1.0, triggeredPossibility: 0.0, isDamageControlAssistAble: false, usageCount: addingBuff.UsageCount,
                       veiledFromTurn: turn, veiledToTurn: (turn + addingBuff.VeiledTurn)));
                    effects.Add(addingEffect[0]);
                    addingEffect[0].BuffToCharacter(currentTurn: turn);
                    order.Actor.Buff.AddBarrier(addingEffect[0].Skill.BuffTarget.BarrierRemaining);

                    string triggerPossibilityText = null;
                    if (order.SkillEffectChosen.TriggeredPossibility < 1.0) { triggerPossibilityText = "(Trigger Possibility: " + (double)((int)(order.SkillEffectChosen.TriggeredPossibility * 1000) / 10.0) + "%) "; }
                    string accumulationText = null;
                    if (order.SkillEffectChosen.NextAccumulationCount > 0)
                    {
                        double count = 0.0;
                        switch (order.SkillEffectChosen.Skill.TriggerBase.AccumulationReference)
                        {
                            case ReferenceStatistics.none: break;
                            case ReferenceStatistics.AvoidCount: count = order.Actor.StatisticsCollection.AvoidCount; break;
                            case ReferenceStatistics.AllHitCount: count = order.Actor.StatisticsCollection.AllHitCount; break;
                            case ReferenceStatistics.AllTotalBeenHitCount: count = order.Actor.StatisticsCollection.AllTotalBeenHitCount; break;
                            case ReferenceStatistics.CriticalBeenHitCount: count = order.Actor.StatisticsCollection.CriticalBeenHitCount; break;
                            case ReferenceStatistics.CriticalHitCount: count = order.Actor.StatisticsCollection.CriticalHitCount; break;
                            case ReferenceStatistics.SkillBeenHitCount: count = order.Actor.StatisticsCollection.SkillBeenHitCount; break;
                            case ReferenceStatistics.SkillHitCount: count = order.Actor.StatisticsCollection.SkillHitCount; break;
                            default: break;
                        }
                        string nextText = null;
                        if (order.SkillEffectChosen.UsageCount > 0) { nextText = " next trigger: " + order.SkillEffectChosen.NextAccumulationCount; } else { nextText = " no usuage count left."; }
                        accumulationText = "(Accumulation " + order.SkillEffectChosen.Skill.TriggerBase.AccumulationReference.ToString() + ": " + count + nextText + ")";
                    }
                    log += new string(' ', 2) + order.Actor.Name + "'s " + order.SkillEffectChosen.Skill.Name + "! " + triggerPossibilityText + accumulationText + "\n"
                              + new string(' ', 3) + order.Actor.Name + " gets " + addingBuff.Name + " which will last " + addingBuff.VeiledTurn + " turns. " + "\n";
                    // Belows: It's a temporary message, only for deffense magnification.
                    if (addingBuff.BuffTarget.DefenseMagnification > 1.0) { log += new string(' ', 4) + "[Defense: " + order.Actor.Buff.DefenseMagnification + " (x" + addingBuff.BuffTarget.DefenseMagnification + ")] "; }
                    if (addingEffect[0].Skill.BuffTarget.BarrierRemaining > 0) { log += "[Barrier:" + order.Actor.Buff.BarrierRemaining + " (+" + addingEffect[0].Skill.BuffTarget.BarrierRemaining + ")] "; }
                    log += "\n";
                    break;
                case TargetType.multi: //Buff attacker's side all
                    addingBuff = buffMasters.FindLast((obj) => obj.Name == order.SkillEffectChosen.Skill.CallingBuffName);
                    List<BattleUnit> buffTargetCharacters = characters.FindAll(character1 => character1.Affiliation == order.Actor.Affiliation && character1.Combat.HitPointCurrent > 0);
                    log += new string(' ', 2) + order.Actor.Name + "'s " + order.SkillEffectChosen.Skill.Name + "! (Trigger Possibility:" + (double)((int)(order.SkillEffectChosen.TriggeredPossibility * 1000) / 10.0) + "%) \n";

                    for (int i = 0; i < buffTargetCharacters.Count; i++)
                    {
                        addingEffect.Add(new EffectClass(character: buffTargetCharacters[i], skill: addingBuff, actionType: ActionType.none,
                        offenseEffectMagnification: 1.0, triggeredPossibility: 0.0, isDamageControlAssistAble: false, usageCount: addingBuff.UsageCount,
                        veiledFromTurn: turn, veiledToTurn: (turn + addingBuff.VeiledTurn)));
                        effects.Add(addingEffect[i]);

                        addingEffect[i].BuffToCharacter(currentTurn: turn);
                        buffTargetCharacters[i].Buff.AddBarrier(addingEffect[i].Skill.BuffTarget.BarrierRemaining);

                        log += new string(' ', 3) + buffTargetCharacters[i].Name + " gets " + addingBuff.Name + " which will last " + addingBuff.VeiledTurn + " turns.";
                        if (addingBuff.BuffTarget.DefenseMagnification > 1.0)
                        { log += new string(' ', 4) + "[Defense: " + buffTargetCharacters[i].Buff.DefenseMagnification + " (x" + addingBuff.BuffTarget.DefenseMagnification + ")] "; }
                        if (addingEffect[i].Skill.BuffTarget.BarrierRemaining > 0)
                        { log += " [Barrier: " + buffTargetCharacters[i].Buff.BarrierRemaining + " (+" + addingEffect[i].Skill.BuffTarget.BarrierRemaining + ")] \n"; }
                    }
                    log += "\n";
                    break;
                case TargetType.none:
                    break;
                default:
                    break;
            }

            if (order.SkillEffectChosen.Skill.DebuffTarget.TargetType != TargetType.none)
            {
                //Debuff exist
            }
            return log;
        }

        public static string SkillLogicDispatcher(OrderClass order, List<BattleUnit> characters, Random r)
        {
            string log = null;
            // check call skill 
            if (order.SkillEffectChosen.Skill.CallSkillLogicName == CallSkillLogicName.none) { return null; }
            SkillLogicShieldHealClass healMulti;
            switch (order.SkillEffectChosen.Skill.CallSkillLogicName)
            {
                case CallSkillLogicName.ShieldHealMulti:
                    healMulti = new SkillLogicShieldHealClass(order: order, characters: characters, isMulti: true, r: r);
                    log += healMulti.Log;
                    break;
                case CallSkillLogicName.ShieldHealSingle:
                    healMulti = new SkillLogicShieldHealClass(order: order, characters: characters, isMulti: false, r: r);
                    log += healMulti.Log;
                    break;
                default:
                    break;
            }
            return log;
        }

        public static (string, BattleResultClass) SkillMoveFunction(OrderClass order, List<BattleUnit> characters, Random r)
        {
            BasicAttackFunction basicAttack;
            BattleResultClass battleResult = new BattleResultClass();
            string log = null;

            switch (order.SkillEffectChosen.Skill.Magnification.AttackTarget)
            {
                case TargetType.self:
                    break;
                case TargetType.none:
                    break;
                default:
                    basicAttack = new BasicAttackFunction(order: order, characters: characters, r: r);
                    battleResult = basicAttack.BattleResult;
                    log += basicAttack.Log;
                    break;
            }
            return (log, battleResult);
        }

    } //End of MainClass

    //Check wipe out and should continue the battle
    public class WipeOutCheck
    {
        public WipeOutCheck(List<BattleUnit> characters)
        {
            List<BattleUnit> totalAllys = characters.FindAll(character => character.Affiliation == Affiliation.ally);
            List<BattleUnit> totalEnemys = characters.FindAll(character => character.Affiliation == Affiliation.enemy);
            List<BattleUnit> crushedAllys = characters.FindAll(character => character.Affiliation == Affiliation.ally && character.Combat.HitPointCurrent == 0);
            List<BattleUnit> crushedEnemys = characters.FindAll(character => character.Affiliation == Affiliation.enemy && character.Combat.HitPointCurrent == 0);
            IsDraw = false; IsEnemyWin = false; IsAllyWin = false; BatleEnd = false;
            if (totalAllys.Count == crushedAllys.Count && totalEnemys.Count == crushedEnemys.Count) { IsDraw = true; BatleEnd = true; }//Draw
            else if (totalAllys.Count == crushedAllys.Count) { IsEnemyWin = true; BatleEnd = true; }  //Ally is wiped out
            else if (totalEnemys.Count == crushedEnemys.Count) { IsAllyWin = true; BatleEnd = true; }//Enemy is wiped out
        }
        public bool BatleEnd { get; set; }
        public bool IsDraw { get; set; }
        public bool IsAllyWin { get; set; }
        public bool IsEnemyWin { get; set; }
    }

    //Function, dhisplay battle condition text
    public class FuncBattleConditionsText
    {
        public FuncBattleConditionsText(int currentTurn, int currentBattleWaves, List<BattleUnit> characters)
        { this.CurrentTurn = currentTurn; this.Characters = characters; this.CurrentBattleWaves = currentBattleWaves; }

        public string Text()
        {
            string text = null;
            text += "Turn " + CurrentTurn + " of " + "wave " + "" + CurrentBattleWaves + "\n";

            // get each affiliation unit list.
            List<BattleUnit> allys = Characters.FindAll(x => x.Affiliation == Affiliation.ally);
            allys.Sort((x, y) => x.UniqueID - y.UniqueID);
            List<BattleUnit> enemys = Characters.FindAll(x => x.Affiliation == Affiliation.enemy);
            enemys.Sort((x, y) => x.UniqueID - y.UniqueID);

            //Display allys info
            for (int j = 0; j < 2; j++)
            {
                List<BattleUnit> affiliationCharacters;
                if (j == 0) { text += "Ally: \n"; affiliationCharacters = allys; } //Display allys info
                else { text += "Enemy: \n"; affiliationCharacters = enemys; } //Display enemys info

                for (int i = 0; i < affiliationCharacters.Count; i++)
                {
                    int shiledSpace = (9 - affiliationCharacters[i].Combat.ShiledCurrent.WithComma().Length);
                    if (shiledSpace <= 0) { shiledSpace = 1; }
                    int hPSpace = (9 - affiliationCharacters[i].Combat.HitPointCurrent.WithComma().Length);
                    if (hPSpace <= 0) { hPSpace = 1; }
                    int shiledPercentSpace = (3 - Math.Round(((double)affiliationCharacters[i].Combat.ShiledCurrent / (double)affiliationCharacters[i].Combat.ShiledMax * 100), 0).WithComma().Length);
                    int hPPercentSpace = (3 - Math.Round(((double)affiliationCharacters[i].Combat.HitPointCurrent / (double)affiliationCharacters[i].Combat.HitPointMax * 100), 0).WithComma().Length);

                    string barrierText = null;
                    if (affiliationCharacters[i].Buff.BarrierRemaining > 0) { barrierText = "[Barrier:" + affiliationCharacters[i].Buff.BarrierRemaining + "] "; }
                    string deteriorationText = null;
                    if ((int)(affiliationCharacters[i].Deterioration * 100) >= 1) { deteriorationText = "(Deterioration:" + (int)(affiliationCharacters[i].Deterioration * 100) + "%) "; }
                    string defenseText = null;
                    if (affiliationCharacters[i].Buff.DefenseMagnification > 1.01) { defenseText = "[Defense: x" + affiliationCharacters[i].Buff.DefenseMagnification + "]"; }

                    text += new string(' ', 1) + "#" + i + " " + affiliationCharacters[i].Name
                        + " Shiled:" + new string(' ', shiledSpace) + affiliationCharacters[i].Combat.ShiledCurrent.WithComma() + " ("
                        + new string(' ', shiledPercentSpace) + Math.Round(((double)affiliationCharacters[i].Combat.ShiledCurrent / (double)affiliationCharacters[i].Combat.ShiledMax * 100), 0) + "%)"
                        + " HP:" + new string(' ', hPSpace) + affiliationCharacters[i].Combat.HitPointCurrent.WithComma() + " ("
                        + new string(' ', hPPercentSpace) + Math.Round(((double)affiliationCharacters[i].Combat.HitPointCurrent / (double)affiliationCharacters[i].Combat.HitPointMax * 100), 0) + "%) "
                        + barrierText + deteriorationText + defenseText
                        + "\n";
                }
            }

            text += "------------------------------------\n";
            return text;
        }
        private int CurrentTurn { get; }
        private int CurrentBattleWaves { get; }
        private List<BattleUnit> Characters { get; }
    }

    //Enum: Affiliation (enemy or ally) difinition 
    public enum Affiliation { ally, enemy, none }
    //Enum: Unit type difinition (for vs check, deal additional bonus or delt additional reduction)
    public enum UnitType { beast, cyborg, drone, robot, titan }
    public enum Ability { power, generation, stability, responsiveness, precision, intelligence, luck, none }
    public enum ActionType { normalAttack, move, counter, chain, reAttack, interrupt, buff, atBeginning, atEnding, any, none }
    public enum AttackType { kinetic, chemical, thermal, any }
    public enum TargetType { self, single, multi, none }
    public enum CriticalOrNot { critical, nonCritical, any }
    public enum ActorOrTargetUnit { actorUnit, targetUnit, no }
    public enum WhichWin { allyWin, enemyWin, Draw }
    public enum SkillName
    {
        none, normalAttack, BarrierCounterAvoidManyTimes, CounterNonCriticalAttack, ChainAllysCounter, FutureSightShot, ReAttackAfterCritical,
        InterruptTargetCounterReduceAccuracy, BarrierAll, Buffdefense12, Buffbarrier10,
        ShiledHealSingle, ShiledHealplusSingle, ShiledHealAll
    }
    public enum ReferenceStatistics { none, AllHitCount, CriticalHitCount, SkillHitCount, AllTotalBeenHitCount, CriticalBeenHitCount, SkillBeenHitCount, AvoidCount }
    public enum CallSkillLogicName { none, ShieldHealSingle, ShieldHealMulti, ReduceAccuracy }

    //Unit, ally and enemy difinitions in craft mode
    public class BattleUnit
    {
        public BattleUnit(int uniqueID, string name, Affiliation affiliation, UnitType unitType, AbilityClass ability, CombatClass combat, FeatureClass feature,
         OffenseMagnificationClass offenseMagnification, DefenseMagnificationClass defenseMagnification, SkillMagnificationClass skillMagnification)
        {
            this.UniqueID = uniqueID; this.Name = name; this.Affiliation = affiliation; this.UnitType = unitType; this.Ability = ability; this.Combat = combat;
            this.Feature = feature; this.OffenseMagnification = offenseMagnification; this.DefenseMagnification = defenseMagnification; this.SkillMagnification = skillMagnification;
            //Initialize 
            this.Deterioration = 0.0;
            this.Buff = new BuffClass();
            this.IsOptimumTarget = false;
            this.IsBarrierBrokenJustNow = false;
            this.IsBarrierExistJustBefore = false;
            this.IsCrushedJustNow = false;
            this.IsAvoidMoreThanOnce = false;
            this.StatisticsCollection = new StatisticsCollectionClass();
        }


        //Basic status, which is passed by outside of battle module.
        public class AbilityClass
        {
            public AbilityClass(int power, int generation, int stability, int responsiveness, int precision, int intelligence, int luck)
            {
                this.Power = power; this.Generation = generation; this.Stability = stability; this.Responsiveness = responsiveness;
                this.Precision = precision; this.Intelligence = intelligence; this.Luck = luck;
            }
            public int Power { get; }
            public int Generation { get; }
            public int Stability { get; }
            public int Responsiveness { get; }
            public int Precision { get; }
            public int Intelligence { get; }
            public int Luck { get; }
        }

        public class BuffClass
        {
            public BuffClass()
            {
                this.BarrierRemaining = 0;
                this.DefenseMagnification = 1.0;
                this.MobilityMagnification = 1.0;
                this.AttackMagnification = 1.0;
                this.AccuracyMagnification = 1.0;
                this.CriticalHitRateMagnification = 1.0;
                this.NumberOfAttackMagnification = 1.0;
                this.RangeMinCorrection = 0;
                this.RangeMaxCorrection = 0;
            }

            public void InitializeBuff()
            {
                this.DefenseMagnification = 1.0;
                this.MobilityMagnification = 1.0;
                this.AttackMagnification = 1.0;
                this.AccuracyMagnification = 1.0;
                this.CriticalHitRateMagnification = 1.0;
                this.NumberOfAttackMagnification = 1.0;
                this.RangeMinCorrection = 0;
                this.RangeMaxCorrection = 0;
            }

            public void AddBarrier(int addBarrierCount) { this.BarrierRemaining += addBarrierCount; }
            // take 1 barrier. true=barrier has, false= no barrier anymore
            public bool RemoveBarrier() { if (this.BarrierRemaining > 0) { this.BarrierRemaining--; return true; } else { this.BarrierRemaining = 0; return false; } }
            public int BarrierRemaining { get; set; }
            public double DefenseMagnification { get; set; }
            public double MobilityMagnification { get; set; }
            public double AttackMagnification { get; set; }
            public double AccuracyMagnification { get; set; }
            public double CriticalHitRateMagnification { get; set; }
            public double NumberOfAttackMagnification { get; set; }
            public int RangeMinCorrection { get; set; }
            public int RangeMaxCorrection { get; set; }
        }

        //Combat Status, the status whitch has been calculated by parts.
        public class CombatClass
        {
            public CombatClass(int shiledCurrent, int shiledMax, int hitPointCurrent, int hitPointMax, int attack, double kineticAttackRatio, double chemicalAttackRatio,
             double thermalAttackRatio, int criticalHit, int numberOfAttacks, int minRange, int maxRange, int accuracy, int mobility, int deffense)
            {
                this.ShiledCurrent = shiledCurrent; this.ShiledMax = shiledMax; this.HitPointCurrent = hitPointCurrent; this.HitPointMax = hitPointMax; this.Attack = attack;
                this.KineticAttackRatio = kineticAttackRatio; this.ChemicalAttackRatio = chemicalAttackRatio; this.ThermalAttackRatio = thermalAttackRatio; this.CriticalHit = criticalHit;
                this.NumberOfAttacks = numberOfAttacks; this.MinRange = minRange; this.MaxRange = maxRange; this.Accuracy = accuracy; this.Mobility = mobility; this.Deffense = deffense;
            }
            public int ShiledCurrent { get; set; }
            public int ShiledMax { get; set; }
            public int HitPointCurrent { get; set; }
            public int HitPointMax { get; set; }
            public int Attack { get; set; }
            public double KineticAttackRatio { get; set; }
            public double ChemicalAttackRatio { get; set; }
            public double ThermalAttackRatio { get; set; }
            public int CriticalHit { get; set; }
            public int NumberOfAttacks { get; set; }
            public int MinRange { get; }
            public int MaxRange { get; }
            public int Accuracy { get; set; }
            public int Mobility { get; set; }
            public int Deffense { get; set; }
        }

        public class FeatureClass
        {
            public FeatureClass(double absorbInitial, bool damageControlAssist, double hateInitial, double hateMagnificationPerTurn)
            {
                this.AbsorbRatioCurrent = absorbInitial; this.AbsorbRatioInitial = absorbInitial; this.DamageControlAssist = damageControlAssist;
                this.HateInitial = hateInitial; this.HateCurrent = hateInitial; this.HateMagnificationPerTurn = hateMagnificationPerTurn;
            }
            // absorb level should i call?
            // int absorbLevel  1= (3 * absorbLevel)% of attack and total (9 + 3* absorbLevel)% of max shiled heal  etc...
            public void InitializeFeature() { this.HateCurrent = HateInitial; }
            public double AbsorbRatioInitial { get; }
            public double AbsorbRatioCurrent { get; set; }
            public double AbsorbMaxRatioInitial { get; }
            public double AbsorbMaxRatioCurrent { get; set; }
            public bool DamageControlAssist { get; }
            public double HateInitial { get; }
            public double HateCurrent { get; set; }
            public double HateMagnificationPerTurn { get; }
        }

        //Basic Magnification, offense and defense is acturally same.
        public class MagnificationClass
        {
            public MagnificationClass(double critical, double kinetic, double chemical, double thermal, double vsBeast, double vsCyborg, double vsDrone, double vsRobot, double vsTitan)
            {
                this.Critical = critical; this.Kinetic = kinetic; this.Chemical = chemical; this.Thermal = thermal; this.VsBeast = vsBeast;
                this.VsCyborg = vsCyborg; this.VsDrone = vsDrone; this.VsRobot = vsRobot; this.VsTitan = vsTitan;
            }
            public double Critical { get; }
            public double Kinetic { get; }
            public double Chemical { get; }
            public double Thermal { get; }
            public double VsBeast { get; }
            public double VsCyborg { get; }
            public double VsDrone { get; }
            public double VsRobot { get; }
            public double VsTitan { get; }
        }

        //Offense magnification
        public class OffenseMagnificationClass : MagnificationClass
        {
            public OffenseMagnificationClass(double optimumRangeBonus, double critical, double kinetic, double chemical, double thermal, double vsBeast, double vsCyborg, double vsDrone, double vsRobot, double vsTitan)
            : base(critical, kinetic, chemical, thermal, vsBeast, vsCyborg, vsDrone, vsRobot, vsTitan)
            { this.OptimumRangeBonus = optimumRangeBonus; }
            public double OptimumRangeBonus { get; }
        }

        //Defense magnification
        public class DefenseMagnificationClass : MagnificationClass
        {
            public DefenseMagnificationClass(double critical, double kinetic, double chemical, double thermal, double vsBeast, double vsCyborg, double vsDrone, double vsRobot, double vsTitan)
             : base(critical, kinetic, chemical, thermal, vsBeast, vsCyborg, vsDrone, vsRobot, vsTitan) { }
        }

        public class SkillMagnificationClass
        {
            public SkillMagnificationClass(ActionSkillClass offenseEffectPower, ActionSkillClass triggerPossibility)
            { this.OffenseEffectPower = offenseEffectPower; this.TriggerPossibility = triggerPossibility; }

            public class ActionSkillClass
            {
                public ActionSkillClass(double move, double heal, double counter, double chain, double reAttack, double interrupt, double atBeginning, double atEnding)
                {
                    this.Move = move; this.Heal = heal; this.Counter = counter; this.Chain = chain; this.ReAttack = reAttack;
                    this.Interrupt = interrupt; this.AtBeginning = atBeginning; this.AtEnding = atEnding;
                }
                public double Move { get; }
                public double Heal { get; }
                public double Counter { get; }
                public double Chain { get; }
                public double ReAttack { get; }
                public double Interrupt { get; }
                public double AtBeginning { get; }
                public double AtEnding { get; }

            }

            public ActionSkillClass OffenseEffectPower { get; }
            public ActionSkillClass TriggerPossibility { get; }
        }

        public class StatisticsCollectionClass
        {
            public StatisticsCollectionClass() { this.Initialise(); }

            public void Initialise()
            {
                this.NumberOfCrushed = 0;
                this.AllActivatedCount = 0;
                this.AllHitCount = 0;
                this.AllTotalDealtDamage = 0;
                this.AllTotalBeTakenDamage = 0;
                this.CriticalActivatedCount = 0;
                this.CriticalHitCount = 0;
                this.CriticalTotalDealtDamage = 0;
                this.CriticalTotalBeTakenDamage = 0;
                this.SkillActivatedCount = 0;
                this.SkillHitCount = 0;
                this.SkillTotalDealtDamage = 0;
                this.SkillTotalBeTakenDamage = 0;
            }

            public void Avarage(int battleWaves)
            {
                this.NumberOfCrushed /= battleWaves;
                this.AllActivatedCount /= battleWaves;
                this.AllHitCount /= battleWaves;
                this.AllTotalDealtDamage /= battleWaves;
                this.AllTotalBeTakenDamage /= battleWaves;
                this.CriticalActivatedCount /= battleWaves;
                this.CriticalHitCount /= battleWaves;
                this.CriticalTotalDealtDamage /= battleWaves;
                this.CriticalTotalBeTakenDamage /= battleWaves;
                this.SkillActivatedCount /= battleWaves;
                this.SkillHitCount /= battleWaves;
                this.SkillTotalDealtDamage /= battleWaves;
                this.SkillTotalBeTakenDamage /= battleWaves;
            }
            public string AllCriticalRatio()
            {
                int countSpace = (3 - Math.Round(this.AllActivatedCount, 0).ToString().Length); if (countSpace <= 0) { countSpace = 0; }
                int criticalCountRateSpace = (3 - Math.Round((this.CriticalActivatedCount / this.AllActivatedCount * 100), 1).WithComma().Length); if (criticalCountRateSpace <= 0) { criticalCountRateSpace = 0; }
                int hitSpace = (5 - Math.Round(this.AllHitCount, 0).WithComma().Length); if (hitSpace <= 0) { hitSpace = 0; }
                int criticalHitRateSpace = (3 - Math.Round((this.CriticalHitCount / this.AllHitCount * 100), 1).WithComma().Length); if (criticalHitRateSpace <= 0) { criticalHitRateSpace = 0; }
                int totalDamageSpace = (8 - Math.Round(this.AllTotalDealtDamage, 1).WithComma().Length); if (totalDamageSpace <= 0) { totalDamageSpace = 0; }
                int criticalTotalDamageRateSpace = (3 - Math.Round((this.CriticalTotalDealtDamage / this.AllTotalDealtDamage * 100), 1).WithComma().Length); if (criticalTotalDamageRateSpace <= 0) { criticalTotalDamageRateSpace = 0; }
                int beTakenDamageSpace = (8 - Math.Round(this.AllTotalBeTakenDamage, 1).WithComma().Length); if (beTakenDamageSpace <= 0) { beTakenDamageSpace = 0; }
                int criticalBeTakenDamageRateSpace = (3 - Math.Round((this.CriticalTotalBeTakenDamage / this.AllTotalBeTakenDamage * 100), 1).WithComma().Length); if (criticalBeTakenDamageRateSpace <= 0) { criticalBeTakenDamageRateSpace = 0; }
                int crushedRateSpace = (3 - Math.Round((this.NumberOfCrushed) * 100, 0).WithComma().Length); if (crushedRateSpace <= 0) { crushedRateSpace = 0; }

                return "Attacks:" + new string(' ', countSpace) + Math.Round(this.AllActivatedCount, 0) + " (" + new string(' ',
                 criticalCountRateSpace) + Math.Round((this.CriticalActivatedCount / this.AllActivatedCount * 100), 1).WithComma() + "%) Hit:"
                + new string(' ', hitSpace) + Math.Round(this.AllHitCount, 0) + " (" + new string(' ', criticalHitRateSpace) + Math.Round((this.CriticalHitCount / this.AllHitCount * 100), 1).WithComma() + "%) Damage:"
                   + new string(' ', totalDamageSpace) + Math.Round(this.AllTotalDealtDamage, 1).WithComma() + " ("
                + new string(' ', criticalTotalDamageRateSpace) + Math.Round((this.CriticalTotalDealtDamage / this.AllTotalDealtDamage * 100), 1).WithComma()
                 + "%) BeTaken: "
                + new string(' ', beTakenDamageSpace) + Math.Round(this.AllTotalBeTakenDamage, 1).WithComma() + " ("
                + new string(' ', criticalBeTakenDamageRateSpace) + Math.Round((this.CriticalTotalBeTakenDamage / this.AllTotalBeTakenDamage * 100), 1).WithComma() + "%)"
                + " Crushed:" + new string(' ', crushedRateSpace) + Math.Round((this.NumberOfCrushed) * 100, 0).WithComma() + "%)";
            }

            public string Skill()
            { return "[Skill] Count: " + this.SkillActivatedCount + " Hit: " + this.SkillHitCount + " Damage: " + this.SkillTotalDealtDamage + " (BeTakenDamage: " + this.SkillTotalBeTakenDamage + ")"; }

            public double NumberOfCrushed { get; set; }
            public double AllActivatedCount { get; set; }
            public double AllHitCount { get; set; }
            public double AllTotalDealtDamage { get; set; }
            public double AllTotalBeenHitCount { get; set; }
            public double AllTotalBeTakenDamage { get; set; }
            public double CriticalActivatedCount { get; set; }
            public double CriticalHitCount { get; set; }
            public double CriticalTotalDealtDamage { get; set; }
            public double CriticalBeenHitCount { get; set; }
            public double CriticalTotalBeTakenDamage { get; set; }
            public double SkillHitCount { get; set; }
            public double SkillTotalDealtDamage { get; set; }
            public double SkillBeenHitCount { get; set; }
            public double SkillTotalBeTakenDamage { get; set; }
            public double SkillActivatedCount { get; set; }
            public double AvoidCount { get; set; }
        }

        public enum Conditions { current, max }
        public int UniqueID { get; }
        public string Name { get; }
        public Affiliation Affiliation { get; }
        public UnitType UnitType { get; }
        public AbilityClass Ability { get; }
        public BuffClass Buff { get; set; }
        public CombatClass Combat { get; }
        public FeatureClass Feature { get; }
        public OffenseMagnificationClass OffenseMagnification { get; }
        public DefenseMagnificationClass DefenseMagnification { get; }
        public SkillMagnificationClass SkillMagnification { get; }
        public bool IsOptimumTarget { get; set; }
        public bool IsBarrierExistJustBefore { get; set; }
        public bool IsBarrierBrokenJustNow { get; set; }
        public bool IsCrushedJustNow { get; set; }
        public bool IsAvoidMoreThanOnce { get; set; }
        public double Deterioration { get; set; }
        public StatisticsCollectionClass StatisticsCollection { get; set; }
    }

    public class EffectClass
    {
        public EffectClass(BattleUnit character, SkillsMasterStruct skill, ActionType actionType, double offenseEffectMagnification, double triggeredPossibility, bool isDamageControlAssistAble, int usageCount,
        int veiledFromTurn, int veiledToTurn)
        {
            this.Character = character; this.Skill = skill; this.ActionType = actionType; this.OffenseEffectMagnification = offenseEffectMagnification; this.TriggeredPossibility = triggeredPossibility;
            this.IsDamageControlAssistAble = isDamageControlAssistAble; this.UsageCount = usageCount; this.VeiledFromTurn = veiledFromTurn; this.VeiledToTurn = veiledToTurn;
            this.SpentCount = 0;
            this.NextAccumulationCount = (int)skill.TriggerBase.AccumulationBaseRate;
        }

        public void BuffToCharacter(int currentTurn)
        {
            if (currentTurn <= VeiledToTurn && currentTurn >= VeiledFromTurn)
            {
                Character.Buff.DefenseMagnification *= Skill.BuffTarget.DefenseMagnification;
                Character.Buff.MobilityMagnification *= Skill.BuffTarget.MobilityMagnification;
                Character.Buff.AttackMagnification *= Skill.BuffTarget.AttackMagnification;
                Character.Buff.AccuracyMagnification *= Skill.BuffTarget.AccuracyMagnification;
                Character.Buff.CriticalHitRateMagnification *= Skill.BuffTarget.CriticalHitRateMagnification;
                Character.Buff.NumberOfAttackMagnification *= Skill.BuffTarget.CriticalHitRateMagnification;
                Character.Buff.RangeMinCorrection += Skill.BuffTarget.RangeMinCorrection;
                Character.Buff.RangeMaxCorrection += Skill.BuffTarget.RangeMaxCorrection;
            }
        }

        public BattleUnit Character { get; }
        public SkillsMasterStruct Skill { get; }
        public ActionType ActionType { get; }
        public double OffenseEffectMagnification { get; }
        public double TriggeredPossibility { get; }
        public bool IsDamageControlAssistAble { get; }
        public int UsageCount { get; set; }
        public int SpentCount { get; set; }
        public int NextAccumulationCount { get; set; }
        public int VeiledFromTurn { get; }
        public int VeiledToTurn { get; }
    }

    // STRUCT SEGUMENT

    //Report for struct
    public class StatisticsReporterFirstBloodClass
    {
        public StatisticsReporterFirstBloodClass(int battleWave)
        {
            this.BattleWave = battleWave;
            this.AllyCharacterName = "none";
            this.AllyActionType = ActionType.none;
            this.AllyHappenedTurn = 0;
            this.AllyCrushedCount = 0;
            this.AllyTotalDealtDamage = 0;
            this.AllyContentText = "No first Blood.";
            this.EnemyCharacterName = "none";
            this.EnemyActionType = ActionType.none;
            this.EnemyHappenedTurn = 0;
            this.EnemyCrushedCount = 0;
            this.EnemyTotalDealtDamage = 0;
            this.EnemyContentText = "No first Blood.";
            this.WhichWin = WhichWin.Draw;
        }
        public int BattleWave { get; set; }
        public string AllyCharacterName { get; set; }
        public ActionType AllyActionType { get; set; }
        public int AllyHappenedTurn { get; set; }
        public int AllyCrushedCount { get; set; }
        public int AllyTotalDealtDamage { get; set; }
        public string AllyContentText { get; set; }
        public string EnemyCharacterName { get; set; }
        public ActionType EnemyActionType { get; set; }
        public int EnemyHappenedTurn { get; set; }
        public int EnemyCrushedCount { get; set; }
        public int EnemyTotalDealtDamage { get; set; }
        public string EnemyContentText { get; set; }
        public WhichWin WhichWin { get; set; }
    }

    //Action order class
    public class OrderClass
    {
        public OrderClass(BattleUnit actor, ActionType actionType, List<EffectClass> skillEffectProposed, int actionSpeed, int individualTargetID, bool isDamageControlAssist)
        {
            this.Actor = actor; this.ActionType = actionType; this.SkillEffectProposed = skillEffectProposed;
            this.ActionSpeed = actionSpeed; this.IndividualTargetID = individualTargetID; this.IsDamageControlAssist = isDamageControlAssist;
            // By default, first list of SkillEffectProposed is selected if has.
            // You need override others if you want to change it.
            if (skillEffectProposed.Count >= 1) { this.SkillEffectChosen = skillEffectProposed[0]; }
            else { Console.WriteLine(" skill Effect proposed is null!!"); }
        }

        // Skill decision, decide best skill in this timming. healAll or healSingle or just do nothing, which move skill should use .
        public void SkillDecision(List<BattleUnit> characters)
        {
            if (SkillEffectProposed != null) // skill effect proposed valid check
            {
                List<EffectClass> validEffects = new List<EffectClass>();
                foreach (EffectClass effect in SkillEffectProposed) { if (effect.UsageCount > 0) { validEffects.Add(effect); } }

                if (validEffects.Count == 0) { Console.WriteLine(" no valid skill exist" + this.Actor.Name + " " + this.ActionType); }
                else if (validEffects.Count >= 1)// in case more than 2 skills proposed.
                {
                    List<BattleUnit> healTargets;
                    List<EffectClass> fillteredEffectList = null;
                    //(1)Damage control assist is requred?
                    if (IsDamageControlAssist)
                    {
                        healTargets = characters.ToList().FindAll((obj) => obj.Affiliation == this.Actor.Affiliation && obj.Combat.HitPointCurrent == 0 && obj.IsCrushedJustNow == true);
                        healTargets.Sort((x, y) => y.Combat.HitPointCurrent - x.Combat.HitPointCurrent);
                    }
                    else // non Damage control assisted.
                    {
                        //(2)heal expected?
                        healTargets = characters.ToList().FindAll((obj) => obj.Combat.ShiledCurrent == 0 && obj.Affiliation == this.Actor.Affiliation
                                     && obj.Combat.HitPointCurrent > 0);
                        healTargets.Sort((x, y) => y.Combat.HitPointCurrent - x.Combat.HitPointCurrent);
                    }

                    //If (1)damage control assisted or (2)heal is expected, check skill proposed.
                    // 0 shiled and low HitPoint character should heal first
                    if (healTargets.Count >= 2)
                    { //Multi heal reccomended
                        fillteredEffectList = validEffects.FindAll(obj => obj.Skill.CallSkillLogicName == CallSkillLogicName.ShieldHealMulti);
                        //Something wise way to chose best skill.

                        // multi - ShieldHealMulti is not expected now..
                        if (fillteredEffectList.Count > 0)
                        {
                            this.SkillEffectChosen = fillteredEffectList.Last();
                            this.IndividualTargetID = healTargets.First().UniqueID;  // get heal target unique ID, 
                        }// multi heal exist
                        else
                        {
                            // find single 
                            fillteredEffectList.Clear();
                            fillteredEffectList = validEffects.FindAll(obj => obj.Skill.CallSkillLogicName == CallSkillLogicName.ShieldHealSingle);
                            //Something wise way to chose best skill.
                            if (fillteredEffectList.Count > 0)
                            {
                                this.SkillEffectChosen = fillteredEffectList.Last();
                                this.IndividualTargetID = healTargets.First().UniqueID;  // get heal target unique ID, 
                            }// single heal exist
                        }
                    }
                    else if (healTargets.Count == 1)
                    { //Single heal reccomended
                        fillteredEffectList = validEffects.FindAll(obj => obj.Skill.CallSkillLogicName == CallSkillLogicName.ShieldHealSingle);
                        //Something wise way to chose best skill.
                        // multi - ShieldHealMulti is not expected now..
                        if (fillteredEffectList.Count > 0)
                        {
                            this.SkillEffectChosen = fillteredEffectList.Last();
                            this.IndividualTargetID = healTargets.First().UniqueID;  // get heal target unique ID, 
                        }// multi heal exist
                        else
                        {
                            // find single 
                            fillteredEffectList.Clear();
                            fillteredEffectList = validEffects.FindAll(obj => obj.Skill.CallSkillLogicName == CallSkillLogicName.ShieldHealMulti);
                            //Something wise way to chose best skill.
                            if (fillteredEffectList.Count > 0)
                            {
                                this.SkillEffectChosen = fillteredEffectList.Last();
                                this.IndividualTargetID = healTargets.First().UniqueID;  // get heal target unique ID, 
                            }// single heal exist
                        }
                    }

                    if (fillteredEffectList == null)
                    {
                        // find non heal skill or valid heal target.
                        //(3)other skill ?
                        fillteredEffectList = validEffects;
                        //Something wise way to chose best skill.
                        if (fillteredEffectList != null) { this.SkillEffectChosen = fillteredEffectList.Last(); }// single heal exist
                        else { Console.WriteLine("unexpected message in SkillDecision."); }// no skill proposed. something wrong!
                    }
                }
            }
        }

        public BattleUnit Actor;
        public ActionType ActionType;
        public List<EffectClass> SkillEffectProposed;
        public EffectClass SkillEffectChosen { get; set; }
        public int ActionSpeed;
        public int IndividualTargetID;
        public bool IsDamageControlAssist;
    }

    // FUNCTION SEGUMENT
    public class BasicAttackFunction
    {
        public BasicAttackFunction(OrderClass order, List<BattleUnit> characters, Random r)
        {
            this.BattleResult = new BattleResultClass();
            int totalDealtDamageSum = 0;
            // Target control
            Affiliation toTargetAffiliation;
            if (order.Actor.Affiliation == Affiliation.ally) { toTargetAffiliation = Affiliation.enemy; } //ally's move, so target should be enemy, without confusion.
            else { toTargetAffiliation = Affiliation.ally; } //enemy's move, so target should be ally, without confusuion.

            // Initialize battle environment: Hit, Failed Hit, Damage for each opponent.
            int numberOfHitTotal = 0;
            int numberOfSuccessAttacks = 0;
            List<BattleUnit> opponents = characters.FindAll(character1 => character1.Affiliation == toTargetAffiliation && character1.Combat.HitPointCurrent > 0);
            // reset IsCrushedJustNow flag
            foreach (BattleUnit character in characters) { if (character.IsCrushedJustNow) { character.IsCrushedJustNow = false; } }

            // no enemy anymore.
            bool invalidAction = false || opponents.Count == 0;

            //Ally alive list
            List<BattleUnit> aliveAttackerSide = characters.FindAll(character1 => character1.Affiliation == order.Actor.Affiliation && character1.Combat.HitPointCurrent > 0);
            int aliveAttackerIndex = aliveAttackerSide.IndexOf(order.Actor);
            //int aliveAttackerIndex = Array.IndexOf(aliveAttackerSide, attacker);

            if (aliveAttackerIndex == -1) { invalidAction = true; }// in case attacker is dead.

            if (invalidAction == false)
            {
                int[] totalDealtDamages = new int[characters.Count];
                int[] totalIndivisualHits = new int[characters.Count];
                bool isNeedCreateTargetPossibilityBox = true;
                List<int> targetPossibilityBox = new List<int>();
                //int[] targetPossibilityBox = new int[200];
                int totalTickets = 0;

                // Skill effect magnification set in case order.SkillEffectChosen is null
                double skillOffenseEffectMagnification = 1.0;
                double skillMagnificationDamage = 1.0;
                double skillMagnificationKinetic = 1.0;
                double skillMagnificationChemical = 1.0;
                double skillMagnificationThermal = 1.0;
                double skillMagnificationAccuracy = 1.0;
                double skillMagnificationCritical = 1.0;
                double skillMagnificationOptimumRangeMin = 1.0;
                double skillMagnificationOptimumRangeMax = 1.0;
                double skillMagnificationNumberOfAttacks = 1.0;

                if (order.SkillEffectChosen != null)
                {
                    skillOffenseEffectMagnification = order.SkillEffectChosen.OffenseEffectMagnification;
                    skillMagnificationDamage = order.SkillEffectChosen.Skill.Magnification.Damage;
                    skillMagnificationKinetic = order.SkillEffectChosen.Skill.Magnification.Kinetic;
                    skillMagnificationChemical = order.SkillEffectChosen.Skill.Magnification.Chemical;
                    skillMagnificationThermal = order.SkillEffectChosen.Skill.Magnification.Thermal;
                    skillMagnificationAccuracy = order.SkillEffectChosen.Skill.Magnification.Accuracy;
                    skillMagnificationCritical = order.SkillEffectChosen.Skill.Magnification.Critical;
                    skillMagnificationNumberOfAttacks = order.SkillEffectChosen.Skill.Magnification.NumberOfAttacks;
                    skillMagnificationOptimumRangeMin = order.SkillEffectChosen.Skill.Magnification.OptimumRangeMin;
                    skillMagnificationOptimumRangeMax = order.SkillEffectChosen.Skill.Magnification.OptimumRangeMax;
                }

                //Critical control
                int criticalReduction = 0;
                if (order.Actor.Combat.CriticalHit >= r.Next(0, 100)) //Critical hit!
                {
                    criticalReduction = 50;
                    this.BattleResult.CriticalOrNot = CriticalOrNot.critical;
                }

                //Dacay difinition
                double decayAccuracy = 0.55 + 0.01 * order.Actor.Ability.Precision;
                double decayDamage = 0.55 + 0.01 * order.Actor.Ability.Power;

                //Decay Cap contorol 
                if (decayAccuracy > 0.99) { decayAccuracy = 0.99; }
                if (decayDamage > 0.99) { decayDamage = 0.99; }

                // Minimum range - ally's column , Max range - ally's column
                int minTargetOptimumRange = (int)(order.Actor.Combat.MinRange * skillMagnificationOptimumRangeMin) - aliveAttackerIndex;
                int maxTargetOptimumRange = (int)(order.Actor.Combat.MaxRange * skillMagnificationOptimumRangeMax) - aliveAttackerIndex;

                //Initialize Is Barrier broken just now flag and is Avoid flag.
                for (int i = 0; i < characters.Count; i++)
                {
                    if (characters[i].IsBarrierBrokenJustNow) { characters[i].IsBarrierBrokenJustNow = false; }
                    if (characters[i].Buff.BarrierRemaining > 0)
                    { if (characters[i].IsBarrierExistJustBefore == false) { characters[i].IsBarrierExistJustBefore = true; } }
                    else { if (characters[i].IsBarrierExistJustBefore) { characters[i].IsBarrierExistJustBefore = false; } }
                    if (characters[i].IsAvoidMoreThanOnce) { characters[i].IsAvoidMoreThanOnce = false; }
                }

                // Indivisual attack routine

                for (int numberOfAttacks = 1; numberOfAttacks <= (int)(order.Actor.Combat.NumberOfAttacks * skillMagnificationNumberOfAttacks); numberOfAttacks++)
                {

                    //Target individual oppornet per each number of attack.
                    int tickets = 0;
                    List<BattleUnit> survivaledOpponents = opponents.FindAll(character1 => character1.Combat.HitPointCurrent > 0);

                    if (survivaledOpponents.Count == 0) { continue; }//enemy all wipe out
                    BattleUnit toTarget = null;
                    if (order.SkillEffectChosen.Skill.Magnification.AttackTarget == TargetType.multi)
                    {
                        int targetColumn = 0;
                        if (isNeedCreateTargetPossibilityBox)
                        {
                            targetPossibilityBox.Clear(); // initialize targetPossibilityBox
                            double basicTargetRatio = 2.0 / 3.0;     //Tiar 1: Basic Target ratio
                            double optimumTargetRatio = 1.0 / 3.0;     //Tiar 2: Optinum Target ratio
                            double optimumTargetBonus = 0;
                            for (int opponent = 1; opponent <= survivaledOpponents.Count; opponent++)
                            {
                                if (opponent >= minTargetOptimumRange && opponent <= maxTargetOptimumRange)
                                {
                                    optimumTargetBonus = optimumTargetRatio / (1 + maxTargetOptimumRange - minTargetOptimumRange);
                                    survivaledOpponents[opponent - 1].IsOptimumTarget = true;
                                }
                                else
                                { survivaledOpponents[opponent - 1].IsOptimumTarget = false; }
                                int targetPossibilityTicket = (int)((basicTargetRatio / Math.Pow(2.0, opponent) + optimumTargetBonus) * 50);

                                targetPossibilityTicket += (int)(survivaledOpponents[opponent - 1].Feature.HateCurrent);// add Hate value 
                                if (targetPossibilityTicket == 0) { targetPossibilityTicket = 1; }//at leaset one chance to hit.

                                //Put tickets into Box with opponent column number (expected: column recalculated when they crushed)
                                for (int ticket = tickets; ticket <= targetPossibilityTicket + tickets; ticket++)
                                { targetPossibilityBox.Add(opponent - 1); }
                                tickets += targetPossibilityTicket;
                            }
                        }
                        else { tickets = totalTickets; }// get previous total tickets

                        int index = r.Next(0, tickets);
                        targetColumn = targetPossibilityBox[index];
                        totalTickets = tickets;
                        toTarget = survivaledOpponents[targetColumn];

                    }
                    else if (order.SkillEffectChosen.Skill.Magnification.AttackTarget == TargetType.single)// if individual target exist, choose he/she.
                    { toTarget = opponents.Find(character1 => character1.UniqueID == order.IndividualTargetID); }
                    else { Console.WriteLine("unexpected. basic attack function, targetType is not single nor multi."); }

                    isNeedCreateTargetPossibilityBox = false;

                    if (toTarget != null && toTarget.Combat.HitPointCurrent > 0)
                    {
                        //Hit control
                        //Logic of Hit: accuracy / mobility * (1- fatigue) * Decay(0.8) %
                        double hitPossibility = (double)order.Actor.Combat.Accuracy * skillMagnificationAccuracy / (double)toTarget.Combat.Mobility / (1.00 - toTarget.Deterioration)
                         * (Math.Pow(decayAccuracy, numberOfSuccessAttacks) / decayAccuracy);

                        //judge
                        if (hitPossibility <= r.NextDouble()) //Failed!
                        {
                            if (toTarget.IsAvoidMoreThanOnce == false) { toTarget.IsAvoidMoreThanOnce = true; }
                            toTarget.StatisticsCollection.AvoidCount++;
                        }
                        else //success!
                        {
                            numberOfSuccessAttacks++;
                            totalIndivisualHits[toTarget.UniqueID]++;

                            double criticalMagnification = 1.0;
                            // critical magnification calculation
                            if (criticalReduction > 0) { criticalMagnification = order.Actor.OffenseMagnification.Critical * toTarget.DefenseMagnification.Critical * skillMagnificationCritical; } //critical

                            //Physical Attack damage calculation
                            double attackDamage = (double)order.Actor.Combat.Attack * r.Next(40 + order.Actor.Ability.Luck, 100) / 100
                            - (double)toTarget.Combat.Deffense * (1.00 - toTarget.Deterioration) * r.Next(40 + toTarget.Ability.Luck - criticalReduction, 100 - criticalReduction) / 100;
                            if (attackDamage < 0) { attackDamage = 1; }

                            //vs Magnification offense
                            double vsOffenseMagnification = 1.0;
                            switch (toTarget.UnitType)
                            {
                                case (UnitType.beast): vsOffenseMagnification = order.Actor.OffenseMagnification.VsBeast; break;
                                case (UnitType.cyborg): vsOffenseMagnification = order.Actor.OffenseMagnification.VsCyborg; break;
                                case (UnitType.drone): vsOffenseMagnification = order.Actor.OffenseMagnification.VsDrone; break;
                                case (UnitType.robot): vsOffenseMagnification = order.Actor.OffenseMagnification.VsRobot; break;
                                case (UnitType.titan): vsOffenseMagnification = order.Actor.OffenseMagnification.VsTitan; break;
                                default: break;
                            }

                            //vs Magnification offense
                            double vsDeffenseMagnification = 1.0;
                            switch (order.Actor.UnitType)
                            {
                                case (UnitType.beast): vsDeffenseMagnification = toTarget.DefenseMagnification.VsBeast; break;
                                case (UnitType.cyborg): vsDeffenseMagnification = toTarget.DefenseMagnification.VsCyborg; break;
                                case (UnitType.drone): vsDeffenseMagnification = toTarget.DefenseMagnification.VsDrone; break;
                                case (UnitType.robot): vsDeffenseMagnification = toTarget.DefenseMagnification.VsRobot; break;
                                case (UnitType.titan): vsDeffenseMagnification = toTarget.DefenseMagnification.VsTitan; break;
                                default: break;
                            }

                            //Damage type Magnification 
                            double damageTypeMagnification =
                             order.Actor.Combat.KineticAttackRatio * order.Actor.OffenseMagnification.Kinetic * toTarget.DefenseMagnification.Kinetic * skillMagnificationKinetic
                             + order.Actor.Combat.ChemicalAttackRatio * order.Actor.OffenseMagnification.Chemical * toTarget.DefenseMagnification.Chemical * skillMagnificationChemical
                             + order.Actor.Combat.ThermalAttackRatio * order.Actor.OffenseMagnification.Thermal * toTarget.DefenseMagnification.Thermal * skillMagnificationThermal;

                            double optimumRangeBonus = 1.0;
                            if (toTarget.IsOptimumTarget) { optimumRangeBonus = order.Actor.OffenseMagnification.OptimumRangeBonus; } //Consider optimum range bonus.
                            double barrierReduction = 1.0;

                            if (toTarget.Buff.RemoveBarrier()) // Barrier check, true: barrier has, false no barrier.
                            {
                                barrierReduction = 1.0 / 3.0;
                                if (toTarget.Buff.BarrierRemaining <= 0) { toTarget.IsBarrierBrokenJustNow = true; }
                            }
                            else // if barrier is broken within this action, broken check.
                            { if (toTarget.IsBarrierExistJustBefore && toTarget.IsBarrierBrokenJustNow == false) { toTarget.IsBarrierBrokenJustNow = true; } }

                            double buffDamageMagnification = order.Actor.Buff.AttackMagnification / toTarget.Buff.DefenseMagnification; // Buff damage reduction
                            int dealtDamage = (int)((attackDamage) * damageTypeMagnification * vsOffenseMagnification * vsDeffenseMagnification
                            * criticalMagnification * optimumRangeBonus * skillOffenseEffectMagnification * skillMagnificationDamage * buffDamageMagnification * (Math.Pow(decayDamage, numberOfSuccessAttacks) / decayDamage) * barrierReduction);
                            if (dealtDamage <= 0) { dealtDamage = 1; }
                            totalDealtDamageSum += dealtDamage;

                            //Damage calclations 1st shiled, and 2nd hitPoint.
                            if (toTarget.Combat.ShiledCurrent >= dealtDamage) { toTarget.Combat.ShiledCurrent -= dealtDamage; }// Only shiled damaged
                            else // shiled break
                            {
                                int remainsDamage = dealtDamage - toTarget.Combat.ShiledCurrent;
                                toTarget.Combat.ShiledCurrent = 0;
                                if (toTarget.Combat.HitPointCurrent > remainsDamage) // hitPoint damage
                                {
                                    toTarget.Combat.HitPointCurrent -= remainsDamage;
                                    double deteriorationStabilityReduction = 1.0 - 0.01 * toTarget.Ability.Stability;
                                    toTarget.Deterioration += (1 - toTarget.Deterioration) * deteriorationStabilityReduction * 0.01;
                                }
                                else //Crushed!
                                {
                                    toTarget.Combat.HitPointCurrent = 0;
                                    toTarget.IsCrushedJustNow = true;
                                    isNeedCreateTargetPossibilityBox = true;
                                    BattleResult.NumberOfCrushed++;
                                    toTarget.StatisticsCollection.NumberOfCrushed++;
                                }
                            }
                            totalDealtDamages[toTarget.UniqueID] += dealtDamage;
                        }
                    }
                    numberOfHitTotal++;
                }
                //Absorb calculation HERE!!

                // Hate Managiment
                double criticalHateAdd = 0; if (criticalReduction > 0) { criticalHateAdd = 30; }
                double skillHateAdd = 0; if (order.SkillEffectChosen != null) { if (order.SkillEffectChosen.Skill.Name != SkillName.normalAttack) { skillHateAdd = 50; } }
                double crushedHateAdd = 0;
                for (int fTargetColumn = 0; fTargetColumn <= opponents.Count - 1; fTargetColumn++)
                { if (opponents[fTargetColumn].Combat.HitPointCurrent == 0) { crushedHateAdd += 100; } }
                order.Actor.Feature.HateCurrent = (numberOfHitTotal / 3.0) + criticalHateAdd + skillHateAdd + crushedHateAdd;

                //Statistics Collection
                for (int toTargetUniqueID = 0; toTargetUniqueID < totalDealtDamages.Length; toTargetUniqueID++)
                {
                    characters[toTargetUniqueID].StatisticsCollection.AllTotalBeTakenDamage += totalDealtDamages[toTargetUniqueID];
                    if (totalDealtDamages[toTargetUniqueID] > 0)
                    {
                        if (criticalReduction > 0)
                        {
                            characters[toTargetUniqueID].StatisticsCollection.CriticalTotalBeTakenDamage += totalDealtDamages[toTargetUniqueID];
                            characters[toTargetUniqueID].StatisticsCollection.CriticalBeenHitCount++;
                        }
                        if (order.SkillEffectChosen != null)
                        {
                            if (order.SkillEffectChosen.Skill.Name != SkillName.normalAttack)
                            {
                                characters[toTargetUniqueID].StatisticsCollection.SkillTotalBeTakenDamage += totalDealtDamages[toTargetUniqueID];
                                characters[toTargetUniqueID].StatisticsCollection.SkillBeenHitCount++;
                            }
                        }
                        if (totalDealtDamages[toTargetUniqueID] > 0) // Get information of character is hit by attack.
                        {
                            BattleResult.HitMoreThanOnceCharacters.Add(characters[toTargetUniqueID]);
                            characters[toTargetUniqueID].StatisticsCollection.AllHitCount++;
                        }
                    }
                    if (characters[toTargetUniqueID].IsAvoidMoreThanOnce) { BattleResult.AvoidMoreThanOnceCharacters.Add(characters[toTargetUniqueID]); } // Set avoidMoreThanOnce characters

                }
                order.Actor.StatisticsCollection.AllActivatedCount++;
                order.Actor.StatisticsCollection.AllTotalDealtDamage += totalDealtDamageSum;
                order.Actor.StatisticsCollection.AllHitCount += numberOfSuccessAttacks;
                if (criticalReduction > 0)
                {
                    order.Actor.StatisticsCollection.CriticalActivatedCount++;
                    order.Actor.StatisticsCollection.CriticalHitCount += numberOfSuccessAttacks;
                    order.Actor.StatisticsCollection.CriticalTotalDealtDamage += totalDealtDamageSum;
                }

                if (order.SkillEffectChosen != null)
                {
                    if (order.SkillEffectChosen.Skill.Name != SkillName.normalAttack)
                    {
                        order.Actor.StatisticsCollection.SkillActivatedCount++;
                        order.Actor.StatisticsCollection.SkillHitCount += numberOfSuccessAttacks;
                        order.Actor.StatisticsCollection.SkillTotalDealtDamage += totalDealtDamageSum;
                    }
                }

                string criticalWords = null; if (criticalReduction > 0) { criticalWords = "critical "; }//critical word.
                string skillTriggerPossibility = null; //if moveskill, show possibility
                if (order.SkillEffectChosen != null)
                {
                    if (order.SkillEffectChosen.Skill.Name != SkillName.normalAttack)
                    { skillTriggerPossibility = " (Trigger Possibility: " + (int)(order.SkillEffectChosen.TriggeredPossibility * 1000.0) / 10.0 + "% left:" + order.SkillEffectChosen.UsageCount + ")"; }
                }
                string sNumberofAttacks = null;
                if (order.Actor.Combat.NumberOfAttacks != 1) { sNumberofAttacks = "s"; }
                string snumberOfSuccessAttacks = null;
                if (numberOfSuccessAttacks != 1) { snumberOfSuccessAttacks = "s"; }

                string skillName = "unknown skill";
                if (order.SkillEffectChosen != null) { skillName = order.SkillEffectChosen.Skill.Name.ToString(); }

                Log += new string(' ', 2) + order.Actor.Name + "'s " + criticalWords + skillName + skillTriggerPossibility + " "
                + order.Actor.Combat.NumberOfAttacks + "time" + sNumberofAttacks +
                 " total hit" + snumberOfSuccessAttacks + ":" + numberOfSuccessAttacks + " Speed:" + order.ActionSpeed + "\n"
                + "   Attack:" + (order.Actor.Combat.Attack)
                + " (Kinetic:" + (order.Actor.Combat.KineticAttackRatio * 100)
                 + "% Chemical:" + (order.Actor.Combat.ChemicalAttackRatio * 100)
                 + "% Thermal:" + (order.Actor.Combat.ThermalAttackRatio * 100) + "%) \n";

                for (int fTargetColumn = 0; fTargetColumn <= opponents.Count - 1; fTargetColumn++)
                {
                    string crushed = null;
                    if (opponents[fTargetColumn].Combat.HitPointCurrent == 0) { crushed = " crushed!"; }
                    if (totalIndivisualHits[opponents[fTargetColumn].UniqueID] >= 1)
                    {
                        string optimumRangeWords = null;
                        if (opponents[fTargetColumn].IsOptimumTarget) { optimumRangeWords = " [optimum range]"; }

                        string barrierWords = null;
                        if (opponents[fTargetColumn].IsBarrierBrokenJustNow) { barrierWords = " [Barrier Broken]"; }
                        else if (opponents[fTargetColumn].Buff.BarrierRemaining > 0) { barrierWords = " [Barriered(" + opponents[fTargetColumn].Buff.BarrierRemaining + ")]"; }

                        string s = null;
                        if (totalIndivisualHits[opponents[fTargetColumn].UniqueID] != 1) { s = "s"; }
                        int shiledPercentSpace = (3 - Math.Round(((double)opponents[fTargetColumn].Combat.ShiledCurrent / (double)opponents[fTargetColumn].Combat.ShiledMax * 100), 0).WithComma().Length);
                        int hPPercentSpace = (3 - Math.Round(((double)opponents[fTargetColumn].Combat.HitPointCurrent / (double)opponents[fTargetColumn].Combat.HitPointMax * 100), 0).WithComma().Length);
                        int damageSpace = (6 - totalDealtDamages[opponents[fTargetColumn].UniqueID].WithComma().Length);

                        Log += new string(' ', 4) + opponents[fTargetColumn].Name
                            + " (Sh" + new string(' ', shiledPercentSpace) + Math.Round(((double)opponents[fTargetColumn].Combat.ShiledCurrent / (double)opponents[fTargetColumn].Combat.ShiledMax * 100), 0).WithComma()
                        + "% HP" + new string(' ', hPPercentSpace) + Math.Round(((double)opponents[fTargetColumn].Combat.HitPointCurrent / (double)opponents[fTargetColumn].Combat.HitPointMax * 100), 0).WithComma()
                         + "%)" + " gets " + new string(' ', damageSpace) + totalDealtDamages[opponents[fTargetColumn].UniqueID].WithComma() + " damage" + crushed + " Hit" + s + ":"
                        + totalIndivisualHits[opponents[fTargetColumn].UniqueID] + barrierWords + optimumRangeWords + " \n";

                        if (opponents[fTargetColumn].IsOptimumTarget) { opponents[fTargetColumn].IsOptimumTarget = false; } //clear IsOptimumTarget to false
                    }

                    //Check wipe out and should continue the battle
                    WipeOutCheck = new WipeOutCheck(characters);
                    BattleResult.BattleEnd = WipeOutCheck.BatleEnd;
                    BattleResult.IsAllyWin = WipeOutCheck.IsAllyWin;
                    BattleResult.IsEnemyWin = WipeOutCheck.IsEnemyWin;
                    BattleResult.IsDraw = WipeOutCheck.IsDraw;
                    BattleResult.TotalDeltDamage = totalDealtDamageSum;
                } //fTargetColumn
            }
        }

        public BattleResultClass BattleResult { get; }
        public WipeOutCheck WipeOutCheck { get; set; }
        public string Log { get; }
    }

    public class ShiledHealFunction
    {
        public ShiledHealFunction(List<BattleUnit> characters)
        {
            for (int i = 0; i <= characters.Count - 1; i++)
            {
                if (characters[i].Combat.HitPointCurrent > 0)
                { // Only when character is not crushed.
                    int shiledHealAmount = (int)((double)characters[i].Combat.ShiledMax * (double)characters[i].Ability.Generation / 100.0);
                    if ((characters[i].Combat.ShiledMax - characters[i].Combat.ShiledCurrent) <= shiledHealAmount)
                    { // can heal max
                        characters[i].Combat.ShiledCurrent = characters[i].Combat.ShiledMax;
                        Log += characters[i].Name + " heals all shiled." +
                         " Shiled:" + characters[i].Combat.ShiledCurrent + " (" + (int)((double)characters[i].Combat.ShiledCurrent / (double)characters[i].Combat.ShiledMax * 100) + "%) \n";
                    }
                    else
                    {
                        characters[i].Combat.ShiledCurrent += shiledHealAmount;
                        Log += characters[i].Name + " heals " + shiledHealAmount +
                         " Shiled:" + characters[i].Combat.ShiledCurrent + " (" + (int)((double)characters[i].Combat.ShiledCurrent / (double)characters[i].Combat.ShiledMax * 100) + "%) \n";
                    }
                }
            }
        }
        public string Log { get; }
    }

    public class CalculationHateMagnificationPerTurnFunction
    {
        public CalculationHateMagnificationPerTurnFunction(List<BattleUnit> characters)
        { foreach (BattleUnit character in characters) { character.Feature.HateCurrent *= character.Feature.HateMagnificationPerTurn; } }
        public string Log { get; }
    }

    public class TriggerBaseClass
    {
        public TriggerBaseClass(double possibilityBaseRate, double possibilityWeight, ReferenceStatistics accumulationReference, double accumulationBaseRate, double accumulationWeight)
        {
            this.PossibilityBaseRate = possibilityBaseRate; this.PossibilityWeight = possibilityWeight; this.AccumulationReference = accumulationReference;
            this.AccumulationBaseRate = accumulationBaseRate; this.AccumulationWeight = accumulationWeight;
        }

        public double PossibilityBaseRate { get; }
        public double PossibilityWeight { get; }
        public ReferenceStatistics AccumulationReference { get; }
        public double AccumulationBaseRate { get; }
        public double AccumulationWeight { get; }
    }

    public class SkillMagnificationClass
    {
        public SkillMagnificationClass(TargetType attackTarget, double damage, double kinetic,
        double chemical, double thermal, double heal, double numberOfAttacks, double critical, double accuracy,
             double optimumRangeMin, double optimumRangeMax)
        {
            this.AttackTarget = attackTarget; this.Damage = damage; this.Kinetic = kinetic; this.Chemical = chemical; this.Thermal = thermal; this.Heal = heal;
            this.NumberOfAttacks = numberOfAttacks; this.Critical = critical; this.Accuracy = accuracy; this.OptimumRangeMin = optimumRangeMin; this.OptimumRangeMax = optimumRangeMax;
        }

        public TargetType AttackTarget { get; }
        public double Damage { get; }
        public double Kinetic { get; }
        public double Chemical { get; }
        public double Thermal { get; }
        public double Heal { get; }
        public double NumberOfAttacks { get; }
        public double Critical { get; }
        public double Accuracy { get; }
        public double OptimumRangeMin { get; }
        public double OptimumRangeMax { get; }
    }

    public class TriggerTargetClass
    {
        public TriggerTargetClass(ActionType actionType, bool afterAllMoved, bool counter, bool chain,
         bool reAttack, bool heal, bool move, AttackType majestyAttackType, CriticalOrNot critical, ActorOrTargetUnit whoCrushed, bool onlyWhenBeenHitMoreThanOnce, bool onlyWhenAvoidMoreThanOnce)
        {
            this.ActionType = actionType; this.AfterAllMoved = afterAllMoved; this.Counter = counter; this.Chain = chain; this.ReAttack = reAttack; this.Heal = Heal;
            this.Move = move; this.MajestyAttackType = majestyAttackType; this.Critical = critical; this.WhoCrushed = whoCrushed;
            this.OnlyWhenBeenHitMoreThanOnce = onlyWhenBeenHitMoreThanOnce; this.OnlyWhenAvoidMoreThanOnce = onlyWhenAvoidMoreThanOnce;
        }

        public ActionType ActionType { get; }
        public bool AfterAllMoved { get; }
        public bool Counter { get; }
        public bool Chain { get; }
        public bool ReAttack { get; }
        public bool Heal { get; }
        public bool Move { get; }
        public AttackType MajestyAttackType { get; }
        public CriticalOrNot Critical { get; }
        public ActorOrTargetUnit WhoCrushed { get; }
        public bool OnlyWhenBeenHitMoreThanOnce { get; }
        public bool OnlyWhenAvoidMoreThanOnce { get; }
    }

    public class BuffTargetParameterClass
    {
        public BuffTargetParameterClass(TargetType targetType, int barrierRemaining, double defenseMagnification, double mobilityMagnification, double attackMagnification,
         double accuracyMagnification, double criticalHitRateMagnification, double numberOfAttackMagnification, int rangeMinCorrection, int rangeMaxCorrection)
        {
            this.TargetType = targetType; this.BarrierRemaining = barrierRemaining; this.DefenseMagnification = defenseMagnification; this.MobilityMagnification = mobilityMagnification;
            this.AttackMagnification = attackMagnification; this.AccuracyMagnification = accuracyMagnification; this.CriticalHitRateMagnification = criticalHitRateMagnification;
            this.NumberOfAttackMagnification = numberOfAttackMagnification; this.RangeMinCorrection = rangeMinCorrection; this.RangeMaxCorrection = rangeMaxCorrection;
        }

        public TargetType TargetType { get; }
        public int BarrierRemaining { get; }
        public double DefenseMagnification { get; }
        public double MobilityMagnification { get; }
        public double AttackMagnification { get; }
        public double AccuracyMagnification { get; }
        public double CriticalHitRateMagnification { get; }
        public double NumberOfAttackMagnification { get; }
        public int RangeMinCorrection { get; }
        public int RangeMaxCorrection { get; }
    }

    public class DebuffTargetParameterClass
    {
        public DebuffTargetParameterClass(TargetType targetType, double barrierRemaining, double defenseMagnification, double mobilityMagnification, double attackMagnification,
         double accuracyMagnification, double criticalHitRateMagnification, double numberOfAttackMagnification)
        {
            this.TargetType = targetType; this.BarrierRemaining = barrierRemaining; this.DefenseMagnification = defenseMagnification; this.MobilityMagnification = mobilityMagnification;
            this.AttackMagnification = attackMagnification; this.AccuracyMagnification = accuracyMagnification; this.CriticalHitRateMagnification = criticalHitRateMagnification;
            this.NumberOfAttackMagnification = numberOfAttackMagnification;
        }

        public TargetType TargetType { get; }
        public double BarrierRemaining { get; }
        public double DefenseMagnification { get; }
        public double MobilityMagnification { get; }
        public double AttackMagnification { get; }
        public double AccuracyMagnification { get; }
        public double CriticalHitRateMagnification { get; }
        public double NumberOfAttackMagnification { get; }
    }



    // SkillsMaster should be more Global class..
    public struct SkillsMasterStruct
    {
        public SkillsMasterStruct(SkillName name, ActionType actionType, CallSkillLogicName callSkillLogicName, bool isHeal, int usageCount, int veiledTurn, Ability ability, TriggerBaseClass triggerBase,
          SkillMagnificationClass magnification, TriggerTargetClass triggerTarget, BuffTargetParameterClass buffTarget, SkillName callingBuffName, DebuffTargetParameterClass debuffTarget)
        {
            this.Name = name; this.ActionType = actionType; this.CallSkillLogicName = callSkillLogicName; this.IsHeal = isHeal; this.UsageCount = usageCount; this.VeiledTurn = veiledTurn;
            this.Ability = ability; this.TriggerBase = triggerBase; this.Magnification = magnification; this.TriggerTarget = triggerTarget;
            this.BuffTarget = buffTarget; this.CallingBuffName = callingBuffName; this.DebuffTarget = debuffTarget;
        }

        public SkillName Name { get; }
        public ActionType ActionType { get; }
        public CallSkillLogicName CallSkillLogicName { get; }
        public bool IsHeal { get; }
        public int UsageCount { get; }
        public int VeiledTurn { get; }
        public Ability Ability { get; }
        public TriggerBaseClass TriggerBase { get; }
        public SkillMagnificationClass Magnification { get; }
        public TriggerTargetClass TriggerTarget { get; }
        public BuffTargetParameterClass BuffTarget { get; }
        public SkillName CallingBuffName { get; }
        public DebuffTargetParameterClass DebuffTarget { get; }
    }

    public class BattleResultClass
    {
        public BattleResultClass()
        {
            this.BattleEnd = false;
            this.IsAllyWin = false;
            this.IsEnemyWin = false;
            this.IsDraw = false;
            this.NumberOfCrushed = 0;
            this.TotalDeltDamage = 0;
            this.CriticalOrNot = CriticalOrNot.any;
            this.HitMoreThanOnceCharacters = new List<BattleUnit>();
            this.AvoidMoreThanOnceCharacters = new List<BattleUnit>();
        }

        public bool BattleEnd;
        public bool IsAllyWin;
        public bool IsEnemyWin;
        public bool IsDraw;
        public int NumberOfCrushed;
        public int TotalDeltDamage;
        public CriticalOrNot CriticalOrNot;
        public List<BattleUnit> HitMoreThanOnceCharacters;
        public List<BattleUnit> AvoidMoreThanOnceCharacters;
    }

    public static class ObjectExtensions { public static string WithComma(this object self) { return string.Format("{0:#,##0}", self); } } //With comma override Object

    // [[Skill logic ]]
    public class SkillLogicShieldHealClass
    {
        // heal shiled all actor's affiliation characters.
        public SkillLogicShieldHealClass(OrderClass order, List<BattleUnit> characters, bool isMulti, Random r)
        {
            string damageControlAssistText = null;
            if (order.IsDamageControlAssist) { damageControlAssistText = "[damage control assist] "; }
            Log += new string(' ', 2) + order.Actor.Name + " " + damageControlAssistText + order.SkillEffectChosen.Skill.Name + " (Left:" + order.SkillEffectChosen.UsageCount + ") \n";
            double healBase = order.Actor.Ability.Generation * order.SkillEffectChosen.Skill.Magnification.Heal * 10.0;

            // heal only same affiliation
            List<BattleUnit> healingCharacters = new List<BattleUnit>();
            if (isMulti)
            {
                if (order.IsDamageControlAssist) // include dead character
                { healingCharacters = characters.Where((arg) => arg.Affiliation == order.Actor.Affiliation).ToList(); }
                else { healingCharacters = characters.Where((arg) => arg.Affiliation == order.Actor.Affiliation && arg.Combat.HitPointCurrent > 0).ToList(); }
            }
            else
            { // heal single find who should heal.
                List<BattleUnit> healingCharactersLaw;
                BattleUnit healingCharacter;
                if (order.IsDamageControlAssist)//set crushed character.
                {
                    healingCharacter = characters.FindLast(obj => obj.UniqueID == order.IndividualTargetID);
                    if (healingCharacter != null) { healingCharacters.Add(healingCharacter); }
                }
                else
                {
                    healingCharactersLaw = characters.Where((arg) => arg.Affiliation == order.Actor.Affiliation && arg.Combat.HitPointCurrent > 0 && arg.Combat.ShiledCurrent == 0).ToList();
                    if (healingCharactersLaw.Count > 0) //No shiled , lowest ratio HP
                    {
                        healingCharactersLaw.Sort((x, y) => (x.Combat.HitPointCurrent / x.Combat.HitPointMax) - (y.Combat.HitPointCurrent / y.Combat.HitPointMax));
                        healingCharacter = healingCharactersLaw.First();
                        if (healingCharacter != null) { healingCharacters.Add(healingCharacter); }
                    }
                    else //All at leaset more than 1 shiled, then lowest ratio shiled.
                    {
                        healingCharactersLaw = characters.Where((arg) => arg.Affiliation == order.Actor.Affiliation && arg.Combat.HitPointCurrent > 0).ToList();
                        healingCharactersLaw.Sort((x, y) => (x.Combat.ShiledCurrent / x.Combat.ShiledMax) - (y.Combat.ShiledCurrent / y.Combat.ShiledMax));
                        healingCharacter = healingCharactersLaw.First();
                        if (healingCharacter != null) { healingCharacters.Add(healingCharacter); }
                    }
                }
            }
            foreach (BattleUnit character in healingCharacters)
            {
                double healValue = healBase * character.Ability.Generation * r.Next(40 + order.Actor.Ability.Luck, 100) / 100;
                character.Combat.ShiledCurrent += (int)healValue;
                if (order.IsDamageControlAssist && character.Combat.HitPointCurrent == 0) //Damage controled, then heal armor only 1%
                { character.Combat.HitPointCurrent = (int)(character.Combat.HitPointMax * 0.01); }

                int shiledPercentSpace = (3 - Math.Round(((double)character.Combat.ShiledCurrent / (double)character.Combat.ShiledMax * 100), 0).WithComma().Length);
                int hPPercentSpace = (3 - Math.Round(((double)character.Combat.HitPointCurrent / (double)character.Combat.HitPointMax * 100), 0).WithComma().Length);
                int healSpace = (6 - healValue.WithComma().Length);

                // check overflow of shiled current.
                if (character.Combat.ShiledCurrent > character.Combat.ShiledMax) { character.Combat.ShiledCurrent = character.Combat.ShiledMax; }
                Log += new string(' ', 4) + character.Name + " (Sh" + new string(' ', shiledPercentSpace) + Math.Round(((double)character.Combat.ShiledCurrent
                        / (double)character.Combat.ShiledMax * 100), 0).WithComma() + "% HP" + new string(' ', hPPercentSpace) + Math.Round(((double)character.Combat.HitPointCurrent
                        / (double)character.Combat.HitPointMax * 100), 0).WithComma() + "%)" + " is healed its shiled by " + (int)healValue + "\n";
            }
            foreach (BattleUnit character in characters) { if (character.IsCrushedJustNow) { character.IsCrushedJustNow = false; } } // reset IsCrushedJustNow flag

            // hate control
            int hateAdd = 30; if (isMulti) { hateAdd = 50; }
            order.Actor.Feature.HateCurrent += hateAdd;
        }
        public string Log { get; }
    }
}