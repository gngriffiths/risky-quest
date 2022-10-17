using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CombatManager : MonoBehaviour
{

    /// <summary>
    /// 
    /// 
    /// 
    /// 
    /// </summary>

    public ScrollingText scrollingText;

    public List<Combat> activeCombats;









    public void StartCombat(int _attackerFaction, int _attackerID, float _atkRoll,int _defenderFaction, int _defenderID, float _defRoll)
    {
        // TODO: show combat stats
        // Check for bonuses



        Unit attacker = null; 

        if (GetPlayer(_attackerFaction))
        {
            attacker = GetPlayer(_attackerFaction).GetUnit(_attackerID);
        }

        Unit defender = null;  

        if (GetPlayer(_defenderFaction))
        {
            defender = GetPlayer(_defenderFaction).GetUnit(_defenderID);
        }

        if (attacker != null && defender != null)
        {

            if (GetCombats().Count == 0)
            {
                GameManager.Instance.ResetCombatTurnTime();
            }

            Combat combat = new Combat(GameManager.Instance.gameTime,attacker, defender);

            GetCombats().Insert(0,combat);

            attacker.SetCommand(Unit_Command.none);
             defender.SetCommand(Unit_Command.none);

            attacker.SetNewDestination(attacker.transform.position);
            defender.SetNewDestination(defender.transform.position);

            //NOTE: placeholder for more complex battle logic
            // current rolls against each other and the loser takes 1 dmg
            // then the attack takes 1 regardless of the outcome

         //   attacker.transform.LookAt(defender.transform.position);
           // defender.transform.LookAt(attacker.transform.position);

          //  attacker.SetCooldown(13.0f);
            attacker.Visuals().StartAttack();
            defender.Visuals().StartAttack();
           // defender.SetCooldown(13.0f);

           
        }



    }

    public void ConcludeCombatTurn(float _atkRoll, float _defRoll)
    {
        Debug.Log("ConcludeCombatTurn ");

        if (GetCombats().Count > 0)
        {

            Combat combat = GetCombats()[GetCombats().Count - 1];

            Unit attacker = combat.attacker;
            Unit defender = combat.defender;

            string line = "Combat: ";
        
            




            //if one of the units no longer exists on the board
            if (attacker == null || defender == null || attacker.gameObject.activeSelf == false || defender.gameObject.activeSelf == false) 
            {
                line += " Attacker is null " + (attacker == null);
                line += " Defender is null " + (attacker == null);
                NewLine(line);
                //remove this combat and then resolve the next one in order
                GetCombats().RemoveAt(GetCombats().Count - 1);

                if (combat.attacker)
                {
                    combat.attacker.Visuals().EndAttack();
                }

                if (combat.defender)
                {
                    combat.defender.Visuals().EndAttack();
                }

                ConcludeCombatTurn(_atkRoll, _defRoll);
                return;
            }


            //TODO: apply bonuses
            float modifiedAttackRoll = _atkRoll;
            float modifiedDefenseRoll = _defRoll;

            //if (attacker.GetControlPoint() && attacker.GetControlPoint().controllerFaction == attacker.faction)
            //{
            //    modifiedAttackRoll += attacker.GetControlPoint().bonusCombatStrength;
            //}

            //if (defender.GetControlPoint() && defender.GetControlPoint().controllerFaction == defender.faction)
            //{
            //    modifiedDefenseRoll += defender.GetControlPoint().bonusCombatStrength;
            //}

            //defender.transform.LookAt(attacker.transform.position);
            combat.defender.Visuals().StartAttack();

            //Check that the attacker is in range
            //the defender still attacks to punish the attacker from trying to hit and run

            float rng = Vector3.Distance(attacker.transform.position, defender.transform.position);

            if (rng <= combat.attacker.range)
            {
              //  attacker.transform.LookAt(defender.transform.position);
                


                line += " Attacker rolls(bonus)[count] "  + Math.Round(_atkRoll, 2) + "(" + Math.Round(modifiedAttackRoll, 2) + ")" + "[" + attacker.Count() + "] \n" ;
                line += " Defender rolls(bonus)[count] " + Math.Round(_defRoll, 2) + "_(" + Math.Round(modifiedDefenseRoll, 2) + ")_" + "[" + defender.Count() + "] \n" ;



                combat.attacker.Visuals().StartAttack();



                if (Mathf.Ceil(modifiedDefenseRoll) + combat.defender.Count() > Mathf.Floor(modifiedAttackRoll) + combat.attacker.Count())
                {
                    combat.attacker.GetBottomFollower().TakeDamage(1);
                }
                else
                {
                    combat.defender.GetBottomFollower().TakeDamage(1);
                }


                
            }

            combat.attacker.GetBottomFollower().TakeDamage(1);

            line += " Attacker remaining [count] " + "[" + attacker.Count() + "] \n";
            line += " Defender remaining [count] "  + "[" + defender.Count() + "] ";


            NewLine(line);


            GetCombats().RemoveAt(GetCombats().Count - 1);

            if (combat.attacker.Count() <= 0 || combat.defender.Count() <= 0)
            {
                if (combat.attacker )
                {
                    combat.attacker.Visuals().EndAttack();
                }

                if (combat.defender)
                {
                    combat.defender.Visuals().EndAttack();
                }


            }
            else
            {
                if (rng <= combat.attacker.range)
                {
                    //if in range
                    //move it to the back of the line
                    GetCombats().Insert(0, combat);
                }
                else 
                {
                    if (combat.attacker)
                    {
                        combat.attacker.Visuals().EndAttack();
                    }

                    if (combat.defender)
                    {
                        combat.defender.Visuals().EndAttack();
                    }
                }
                    
            }

        }
    
    }


    public void NewLine(string _newLine)
    {
        if (scrollingText)
        {
            scrollingText.NewLine(_newLine);
        }
    }


    public PlayerControl GetPlayer(int _faction)
    {

        PlayerControl newPlayer = null;

        PlayerRegistry.ForEach(pObj =>
        {
            if (pObj.Index == _faction)
            {
                newPlayer = pObj.GetComponent<PlayerControl>();
            }


        });

        return newPlayer;
    }


    public List<Combat> GetCombats()
    {
        if (activeCombats == null) 
        {
            activeCombats = new List<Combat>();
        }


        return activeCombats;
    }


}

[System.Serializable]
public class Combat 
{
    public Unit attacker;
    public Unit defender;

    public float combatLength;
    public float timeStamp_combatStart;

    public Combat(float _gameTime,Unit _attacker, Unit _defender)
    {
        attacker = _attacker;
        defender = _defender;
        timeStamp_combatStart = _gameTime;

    }

    public bool CheckIfDone(float _gameTime)
    {
        if (_gameTime - combatLength >= timeStamp_combatStart)
        { return true; }

        return false;
    
    }

}