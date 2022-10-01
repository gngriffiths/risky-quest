using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPoint : MonoBehaviour
{
    public float zoneRadius;
    public PlayerControl playerA; 
    public PlayerControl player1;
    


    public int controllerFaction;

    public int control_A;
    public int control_1;

    private MeshRenderer rend;

    private List<Unit> units;

    Dictionary<PlayerControl, int> playerControlStrength =
                     new Dictionary<PlayerControl, int>();



    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   

    public void CheckForControl()
    {
        controllerFaction = -1;

        int newA = 0;
        int new1 = 0;

        foreach (Unit el in GetUnits())
        {
            if (Vector3.Distance(transform.position, el.transform.position) <= zoneRadius)
            {
                if (el.faction == playerA.faction)
                {
                    newA += 1;// other.GetComponent<Unit>().Count();
                }
                if (el.faction == player1.faction)
                {
                    new1 += 1;// other.GetComponent<Unit>().Count();
                }
            }
        }

        control_A = newA;
        control_1 = new1;

        if (control_A > control_1)
        { controllerFaction = playerA.faction; }
        else if (control_1 > control_A)
        { controllerFaction = player1.faction; }


        if (controllerFaction >= 0 && GameManager.rm.PlayerColours.Length > controllerFaction && rend)
        {
            rend.material.color = GameManager.rm.PlayerColours[controllerFaction % GameManager.rm.PlayerColours.Length];
        }
    
    }


    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");

        if (playerA == null || player1 == null) 
        {
            FindPlayers();
        }

        if (playerA == null || player1 == null) { return; }


        if (other.GetComponent<Unit>() == null) { return; }

        if(GetUnits().Contains(other.GetComponent<Unit>())) { return; }

        GetUnits().Add(other.GetComponent<Unit>());

        if (other.GetComponent<Unit>().faction == playerA.faction)
        {
            control_A += 1;// other.GetComponent<Unit>().Count();
        }
        if (other.GetComponent<Unit>().faction == player1.faction)
        {
            control_1 += 1;// other.GetComponent<Unit>().Count();
        }
        CheckForControl();
    }
    public void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit");

        if (playerA == null || player1 == null)
        {
            FindPlayers();
        }

        if (playerA == null || player1 == null) { return; }
        if (other.GetComponent<Unit>() == null) { return; }

        if (GetUnits().Contains(other.GetComponent<Unit>()) == false) { return; }


        if (other.GetComponent<Unit>().faction == playerA.faction)
        {
            control_A -= 1;// other.GetComponent<Unit>().Count();
        }
        if (other.GetComponent<Unit>().faction == player1.faction)
        {
            control_1 -= 1;// other.GetComponent<Unit>().Count();
        }

        GetUnits().Remove(other.GetComponent<Unit>());

        CheckForControl();
    }


    public void FindPlayers()
    {
        PlayerControl firstPlayer = null;
        PlayerControl secondPlayer = null; 

        PlayerRegistry.ForEach(pObj =>
        {
            if (firstPlayer == null)
            {
                firstPlayer = pObj.GetComponent<PlayerControl>();
            }
            else
            {
                if (pObj.Index < firstPlayer.faction)
                {
                    secondPlayer = firstPlayer;
                    firstPlayer = pObj.GetComponent<PlayerControl>();
                }
            }
            


        });

        player1 = firstPlayer;
        playerA = secondPlayer;



    }


    public List<Unit> GetUnits()
    {
        if (units == null)
        {
            units = new List<Unit>();
            foreach (Unit el in FindObjectsOfType<Unit>())
            {
                if (Vector3.Distance(el.transform.position,transform.position) <= zoneRadius)
                { units.Add(el); }
            }
        }


        return units;
    }

}
