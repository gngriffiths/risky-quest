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
            Combat combat = new Combat(GameManager.Instance.gameTime,attacker, defender);

            GetCombats().Insert(0,combat);

            attacker.SetCommand(Unit_Command.inCombat_attacking);
            defender.SetCommand(Unit_Command.inCombat_defending);

            attacker.SetNewDestination(attacker.transform.position);
            defender.SetNewDestination(defender.transform.position);

            //NOTE: placeholder for more complex battle logic
            // current rolls against each other and the loser takes 1 dmg
            // then the attack takes 1 regardless of the outcome

            attacker.transform.LookAt(defender.transform.position);
            defender.transform.LookAt(attacker.transform.position);

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
            Debug.Log("GetCombats().Count > 0) ");
            if (Mathf.Ceil(_defRoll) + combat.defender.Count() > Mathf.Floor(_atkRoll) + combat.attacker.Count())
            {
                combat.attacker.UpdateCount(-1);
            }
            else
            {
                combat.defender.UpdateCount(-1);
            }


            combat.attacker.UpdateCount(-1);

            GetCombats().RemoveAt(GetCombats().Count - 1);

            if (combat.attacker.Count() <= 0 || combat.defender.Count() <= 0)
            {
                if (combat.attacker.Count() <= 0)
                {
                    combat.attacker.De_Init();
                }

                if (combat.defender.Count() <= 0)
                {
                    combat.defender.De_Init();
                }


            }
            else
            {
                //move it to the back of the line
                GetCombats().Insert(0,combat);
            }

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

        combatLength = _attacker.attackTime;
    }

    public bool CheckIfDone(float _gameTime)
    {
        if (_gameTime - combatLength >= timeStamp_combatStart)
        { return true; }

        return false;
    
    }

}