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



    public void StartCombat(int _attackerFaction, int _attackerID, float _atkRoll,int _defenderFaction, int _defenderID, float _defRoll)
    {
        // TODO: show combat stats
        // Check for bonuses
        


        Unit attacker = GetPlayer(_attackerFaction).GetUnit(_attackerID);
        Unit defender = GetPlayer(_defenderFaction).GetUnit(_defenderID);



        if (attacker != null && defender != null)
        {


            attacker.SetNewDestination(attacker.transform.position);

           //NOTE: placeholder for more complex battle logic
           // current rolls against each other and the loser takes 1 dmg
           // then the attack takes 1 regardless of the outcome
       

            if (Mathf.Ceil(_defRoll) + defender.Count() > Mathf.Floor(_atkRoll) + attacker.Count())
            {
                attacker.UpdateCount(-1);
            }
            else
            {
                defender.UpdateCount(-1);
            }


            attacker.UpdateCount(-1);

            if (attacker.Count() <= 0)
            {
                attacker.De_Init();
            }

            if (defender.Count() <= 0)
            {
                defender.De_Init();
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



}
