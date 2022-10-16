using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public enum bonus_type { power,new_unit,speed}

public class ControlPoint : MonoBehaviour
{
    public bonus_type bonus;
    public float zoneRadius;
    public float bonusCombatStrength;

    public PlayerControl playerA; 
    public PlayerControl player1;
    


    public int controllerFaction;
    public int controllerStrength;




    private MeshRenderer rend;
    public TextMeshPro infoText;

    private List<Unit> units;

    Dictionary<int, int> playerControlStrength;



    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ResetToStart()
    {
        GetUnits().Clear();
        controllerFaction = -1;
        controllerStrength = -1;
        SetVisual();


    }


    public void CheckForControl()
    {


        int factionInControl = -1;

        controllerStrength = 0;


        if (GetUnits().Count > 0)
        {

            PlayerControlStrength().Clear();


            foreach (Unit el in GetUnits())
            {
                if (el.gameObject.activeSelf && el.Count() > 0)
                {
                    if (factionInControl == -1)
                    {
                        factionInControl = el.faction;
                    }


                    if (PlayerControlStrength().ContainsKey(el.faction))
                    {

                        PlayerControlStrength()[el.faction] = PlayerControlStrength()[el.faction] + 1;
                    }
                    else
                    {
                        PlayerControlStrength().Add(el.faction, 1);
                    }

                    if (PlayerControlStrength()[factionInControl] < PlayerControlStrength()[el.faction])
                    {
                        factionInControl = el.faction;
                    }
                }
            }

            if (factionInControl != -1 && PlayerControlStrength()[factionInControl] > 0)
            {

                controllerStrength = PlayerControlStrength()[factionInControl];

                controllerFaction = factionInControl;

                GameManager.Instance.CheckForVictory(this);

            }

        }
        else
        {

            controllerFaction = -1;
            controllerStrength = 0;
            GameManager.Instance.CheckForVictory(this);
        }


       

        



        SetVisual();

    }


    public void SetVisual()
    {

        if (controllerFaction >= 0 && GameManager.rm.PlayerColours.Length > controllerFaction && rend)
        {
            rend.material.color = GameManager.rm.PlayerColours[controllerFaction % GameManager.rm.PlayerColours.Length];
        }
        else { rend.material.color = GameManager.rm.PlayerColours[GameManager.rm.PlayerColours.Length - 1]; }

        if (infoText == null)
        {
            if (transform.childCount > 0)
            { 
                if (transform.GetChild(0).GetComponent<TextMeshPro>())
                { infoText = transform.GetChild(0).GetComponent<TextMeshPro>(); }
            
            }
        }

        if (infoText)
        {
            if (controllerFaction != -1)
            {
                infoText.text = "Faction: " + controllerFaction.ToString() + " \nStrength: " + controllerStrength.ToString();
            }
            else { infoText.text = "Neutral"; }
        }

    }







    public void OnTriggerEnter(Collider other)
    {
        HandleTriggerEnter(other);

    }


    public void OnTriggerExit(Collider other)
    {

        HandleTriggerExit(other);
    }



    public virtual void HandleTriggerEnter(Collider other)
    {
        if (other.GetComponent<Unit>() == null) { return; }

        Debug.Log("Control point HandleTriggerEnter");

        if (GetUnits().Contains(other.GetComponent<Unit>()) == false)
        {
            GetUnits().Add(other.GetComponent<Unit>());
        }
       // other.GetComponent<Unit>().SetCurrentControlPoint(this);

        CheckForControl();
    }
    public virtual void HandleTriggerExit(Collider other)
    {
        if (other.GetComponent<Unit>() == null) { return; }

        Debug.Log("Control point EXIT");

        if (GetUnits().Contains(other.GetComponent<Unit>()))
        {
            GetUnits().Remove(other.GetComponent<Unit>());
        }




        CheckForControl();
    }




    public virtual void CheckForBonus(Unit _unit)
    { 
    
    }





    public List<Unit> GetUnits()
    {
        if (units == null)
        {
            units = new List<Unit>();

        }


        return units;
    }

    public Dictionary<int, int> PlayerControlStrength()
    {
        if (playerControlStrength == null)
        {
            playerControlStrength = new Dictionary<int, int>();
  
        }


        return playerControlStrength;
    }


}
