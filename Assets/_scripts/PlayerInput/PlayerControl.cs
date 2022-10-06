using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

using TMPro;

public enum Unit_Command
{
    none,
    move,attack,attack_move,stop,
    inCombat_attacking,
    inCombat_defending,
    defend,

    split,merge,
    merge_move
}


[RequireComponent(typeof(Selector))]
public class PlayerControl : NetworkBehaviour
{
    public static PlayerControl Local { get; protected set; }

    readonly FixedInput LocalInput = new FixedInput();

    private Selector selector;
    private Visual_IssueOrders visualOrders;



    [SerializeField]
    private Unit_Command pendingCommand;
    [SerializeField]
    private Unit selectedUnit;
    [SerializeField]
    private List<Unit> units;

    public CameraFollow camFollow;


    public int faction;

    private int tracker_idCount;
    private int tracker_unitSelector;

    private float unitSpawnOffset = 0.2f;

    public GameObject prefab_unit;





    // Start is called before the first frame update
    void Start()
    {
        selector = GetComponent<Selector>();
        

    }

    public override void Spawned()
    {
        units = new List<Unit>();

        if (Runner.LocalPlayer)
        {
            Local = this;
         //   faction = PlayerObject.Local.Index;
        }
        if (Object.HasInputAuthority)
        {
            //  Local = this;
            // faction = PlayerObject.Local.Index;

            //faction = GetComponent<PlayerObject>().Index;
            if (GetVisualOrders()) { GetVisualOrders().SetFaction(GameManager.rm.PlayerMaterials[faction]); }
        }


    }

    public void InitUnits(int _unitCount)
    {
       
        if (Runner.IsConnectedToServer)
        {
           return;
        }

        int pID = faction;

        Transform spawnpoint = transform;

        if (GameManager.Instance.parent_spawnPoints != null && GameManager.Instance.parent_spawnPoints.childCount > 0)
        { 
            spawnpoint = GameManager.Instance.parent_spawnPoints.GetChild(0 );

        }

        int count = 0;

        while (count < _unitCount && GetUnits().Count < _unitCount )
        {

            GameObject clone = GetUnitToSpawn();


            units.Add(clone.GetComponent<Unit>());

            Vector3 spawnPos = spawnpoint.position ;
             spawnPos +=  (spawnpoint.right * count * unitSpawnOffset);
             spawnPos -= ((spawnpoint.forward * count * unitSpawnOffset) * (count % 2));

            clone.GetComponent<Unit>().Init(pID, tracker_idCount, GameManager.rm.PlayerMaterials[pID], spawnPos, spawnpoint.rotation);
            clone.GetComponent<Unit>().owner = this;
            clone.GetComponent<Unit>().SetCount(2);

            count++;
        }


    }


    public GameObject GetUnitToSpawn()
    {

        GameObject clone;

        if (GameManager.Instance.GetUnitPool() && GameManager.Instance.GetUnitPool().childCount > 0 && GameManager.Instance.GetUnitPool().GetChild(0).GetComponent<Unit>())
        {
            clone = GameManager.Instance.GetUnitPool().GetChild(0).gameObject;
            clone.transform.parent = null;
        }
        else
        {
            clone = Instantiate(prefab_unit, prefab_unit.transform.position,prefab_unit.transform.rotation);
        }

        tracker_idCount++;

        return clone;
    }




    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_InitUnits(int _faction,int _unitCount)
    {

        faction = _faction;
        int pID = _faction;

        Debug.Log("int pID = GetComponent<PlayerObject>().Index;: " + pID);
        pID = Mathf.Clamp(Mathf.Abs(pID), 0, 100) % 3;

        Transform spawnpoint = transform;
        if (GameManager.Instance.parent_spawnPoints != null && GameManager.Instance.parent_spawnPoints.childCount > 0)
        {
            spawnpoint = GameManager.Instance.parent_spawnPoints.GetChild(pID % (GameManager.Instance.parent_spawnPoints.childCount - 1));

        }

        int count = 0;

        while (count < _unitCount && GetUnits().Count < _unitCount)
        {
            GameObject clone = GetUnitToSpawn();


            units.Add(clone.GetComponent<Unit>());

            Vector3 spawnPos = spawnpoint.position + (spawnpoint.right * count * unitSpawnOffset) - ((spawnpoint.forward * count * unitSpawnOffset) * (count % 2));

            clone.GetComponent<Unit>().Init(pID, units.Count, GameManager.rm.PlayerMaterials[pID], spawnPos, spawnpoint.rotation);
            clone.GetComponent<Unit>().owner = this;
            clone.GetComponent<Unit>().SetCount(2);
            count++;
        }


        if (GetVisualOrders()) { GetVisualOrders().SetFaction(GameManager.rm.PlayerMaterials[pID]); }

    }




    // Update is called once per frame
    void Update()
    {


        //LocalInput.PollDown(KeyCode.E);
        if (Runner.IsServer && GameManager.State.Current == GameState.EGameState.Pregame)
        { 
            LocalInput.PollDown(KeyCode.Space);

        }

        if (Object.HasInputAuthority)
        {
            ListenToInput();
        }

        //   ListenToInput();

    }




    public override void FixedUpdateNetwork()
    
    {
        if (Runner.IsServer)
        {
            if (LocalInput.GetDown(KeyCode.Space))
            {
                GameManager.Instance.Server_StartGame();
                Debug.Log("Starting Game...");
            }
        }



        //GetInput(out NetworkInputPrototype input);

        //Debug.Log(input.IsDown(NetworkInputPrototype.BUTTON_USE));

        if (GameManager.State.Current == GameState.EGameState.Play)
        {
           
        }
       

        if (Runner.IsResimulation == false)
            LocalInput.Clear();
    }


    public void ListenToInput()
    {


   
        if (Input.GetMouseButtonUp(0))
        {
           

        }


        if (Input.GetMouseButtonDown(0))
        {
            LeftClick();
        
        }
        if (Input.GetMouseButtonDown(1))
        {
            RightClick();

        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            tracker_unitSelector++;

            if (tracker_unitSelector > tracker_idCount)
            { tracker_unitSelector = 0; }

            if (GetUnit(tracker_unitSelector) && GetUnit(tracker_unitSelector).gameObject.activeSelf)
            {
                SelectUnit(GetUnit(tracker_unitSelector));
            }




        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            SetPendingCommand(Unit_Command.attack);
        }

         if (Input.GetKeyDown(KeyCode.S))
        {
            SetPendingCommand(Unit_Command.attack_move);
        }

         if (Input.GetKeyDown(KeyCode.D))
        {
            SetPendingCommand(Unit_Command.move);
        }

         if (Input.GetKeyDown(KeyCode.F))
        {
          
        }

         if (Input.GetKeyDown(KeyCode.Q))
        {
            SetPendingCommand(Unit_Command.merge);
        }

         if (Input.GetKeyDown(KeyCode.W))
        {
            if (SelectedUnit())
            {
 
                SetPendingCommand(Unit_Command.none);
            }
            else
            {
                SetPendingCommand(Unit_Command.split);
            }
            
        }


    }


    public void ExecuteCommand(Unit_Command _command)
    {
        if (_command == Unit_Command.stop)
        {
           
        }


        if (_command == Unit_Command.split)
        {
            if (SelectedUnit())
            {
                if (GetVisualOrders()) { GetVisualOrders().SetFaction(GameManager.rm.PlayerMaterials[faction]); }
                RPC_IssueCommand(Unit_Command.split, SelectedUnit().faction, SelectedUnit().id, Vector3.zero);
                //SplitUnit(SelectedUnit());
            }
        }
    }






    public void LeftClick()
    {


        RaycastHit hit;

        LayerMask mask = LayerMask.GetMask(GameConstants.UNIT_LAYER,GameConstants.GROUND_LAYER);

        if (Physics.Raycast(GetSelector().GetRayFromCamera(), out hit, mask))
        {
           

            if (hit.transform.GetComponent<Unit>())
            {
                ClickedOnUnit(hit.transform.GetComponent<Unit>());


            }
            else
            {
                if (SelectedUnit() && SelectedUnit().faction == GetComponent<PlayerObject>().Index)
                {
                    // SelectedUnit().RPC_SetDestination(hit.point);
                    RPC_IssueCommand(Unit_Command.move, SelectedUnit().faction, SelectedUnit().id, hit.point);
                }
            }


        }


        SetPendingCommand(Unit_Command.none);
        DisplaySelectedUnitInfo();

    }


    public void RightClick()
    {


        RaycastHit hit;

        LayerMask mask = LayerMask.GetMask(GameConstants.UNIT_LAYER, GameConstants.GROUND_LAYER);

        if (SelectedUnit() && SelectedUnit().faction == faction && Physics.Raycast(GetSelector().GetRayFromCamera(), out hit,mask) )
        {
            if ( hit.transform.GetComponent<Unit>())
            {

                Unit targetUnit = hit.transform.GetComponent<Unit>();

                if (SelectedUnit().faction != targetUnit.faction)
                {


                    RPC_IssueCommand(Unit_Command.attack, SelectedUnit().faction, SelectedUnit().id, hit.transform.position, targetUnit.faction, targetUnit.id);

                }
                else if (SelectedUnit().faction == targetUnit.faction)
                {

                    if (SelectedUnit().id == targetUnit.id)
                    {


                        RPC_IssueCommand(Unit_Command.split, SelectedUnit().faction, SelectedUnit().id, hit.transform.position, targetUnit.faction, targetUnit.id);

                    }
                    else
                    {
                        RPC_IssueCommand(Unit_Command.merge, SelectedUnit().faction, SelectedUnit().id, hit.transform.position, targetUnit.faction, targetUnit.id);

                    }

                }


            }
            else //if (hit.transform.tag == "ground")
            {
                if (SelectedUnit() && SelectedUnit().faction == faction)
                {
                    RPC_IssueCommand(Unit_Command.move, SelectedUnit().faction, SelectedUnit().id, hit.point);
                }
                    


            }


           


        }


        SetPendingCommand(Unit_Command.none);

        DisplaySelectedUnitInfo();

    }




    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_IssueCommand(Unit_Command _cmd, int _faction,int _unitId, Vector3 _point, int _targetFaction=-1, int _targetUnitId=-1)
    {

        Debug.Log(" RPC_IssueCommand + " + _cmd.ToString() + "Unit: " + _unitId + "   " + _point.ToString());

        if (GetVisualOrders())
        {
            
            GetVisualOrders().OrderIssued(new Vector3(_point.x,0,_point.z),_cmd);
        
        
        }



        if (GetUnits().Count == 0) { return; }





        Unit actingUnit = GetUnit(_unitId);
        Unit targetUnit = null;

        PlayerControl targetPlayer = null;

        if (actingUnit == null || actingUnit.gameObject.activeSelf == false) { return; }

        if (_cmd == Unit_Command.move)
        {

            actingUnit.SetCommand(Unit_Command.move);

            actingUnit.SetNewDestination(_point);

        }
        if (_cmd == Unit_Command.attack_move)
        {
            //actingUnit.SetCommand(Unit_Command.attack);
            actingUnit.SetNewDestination(_point);

        }
        else if (_cmd == Unit_Command.split)
        {
            if (actingUnit)
            {
                SplitUnit(actingUnit);

            }

        }
        else if (_cmd == Unit_Command.merge)
        {

            //targetUnit = GetUnit(_targetUnitId);

            //if (actingUnit && targetUnit)
            //{
            //    MergeUnit(actingUnit, targetUnit);

            //}
            targetUnit = GetUnit(_targetUnitId);

            if (targetUnit && _targetFaction == actingUnit.faction && actingUnit.id != targetUnit.id)
            {
                
                
                

                if (targetUnit)
                {

                    actingUnit.SetCommand(Unit_Command.merge);
                    actingUnit.SetTarget(targetUnit);

                    actingUnit.SetNewDestination(targetUnit.transform.position);
                }


            }

        }
        else if (_cmd == Unit_Command.attack)
        {
            if (_targetFaction != -1 && _targetUnitId != -1)
            {
                targetPlayer = GetPlayer(_targetFaction);



                if (targetPlayer)
                {
                    targetUnit = targetPlayer.GetUnit(_targetUnitId);
                }

                if (targetUnit)
                {

                    actingUnit.SetCommand(Unit_Command.attack);
                    actingUnit.SetTarget(targetUnit);

                    actingUnit.SetNewDestination(targetUnit.transform.position);
                }

                
            }
            
        }
        else
        {

        }


    }



    public void ClickedOnUnit(Unit _unit)
    {
        if (SelectedUnit() == null)
        {
            SelectUnit(_unit);

            if (GetPendingCommand() == Unit_Command.split)
            {
                RPC_IssueCommand(Unit_Command.split, SelectedUnit().faction,SelectedUnit().id, Vector3.zero);
                //SplitUnit(_unit);
                
            }

            


           // return;


        }
        else
        {

            if (GetPendingCommand() == Unit_Command.none)
            {
                SelectUnit(_unit);
               // return;
            }

            else if (GetPendingCommand() == Unit_Command.merge && SelectedUnit())
            {
                //TODO: try merge
                SetPendingCommand(Unit_Command.none);
            }
            else if (GetPendingCommand() == Unit_Command.attack && SelectedUnit())
            {
                //TODO: issue attack order
                SetPendingCommand(Unit_Command.none);
            }
            else
            {
                SelectUnit(_unit);
                //TODO: try merge
                SetPendingCommand(Unit_Command.none);
            }
        }


    }


    public void StartCombat(Unit _attacker, Unit _defender)
    {
        //if (Object.HasInputAuthority)
        if (Runner.IsServer)
        {
            float atkRoll = Random.Range(0, 10.0f);
            float defRoll = Random.Range(0, 10.0f);

            RPC_StartCombat(_attacker.faction, _attacker.id, atkRoll, _defender.faction, _defender.id, defRoll);
        }

    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_StartCombat(int _attackerFaction, int _attackerID, float _atkRoll, int _defenderFaction, int _defenderID, float _defRoll)
    {

        GameManager.combatManager.StartCombat(_attackerFaction, _attackerID, _atkRoll,_defenderFaction, _defenderID, _defRoll);
    }





    public void MergeUnit(Unit _unit, Unit _incomingUnit)
    {

        _unit.UpdateCount(_incomingUnit.unitCount);

        _unit.SetCommand(Unit_Command.none);

        _incomingUnit.De_Init();
    
    }


    public void SplitUnit(Unit _unit)
    {
        if (_unit.unitCount > 1)
        {
            int countChange = Mathf.FloorToInt(_unit.unitCount * 0.5f);

            _unit.unitCount -= countChange;

            GameObject clone = GetUnitToSpawn();

            GetUnits().Add(clone.GetComponent<Unit>());

            Vector3 spawnPos = _unit.transform.position + (Vector3.right * 0.2f);

            clone.GetComponent<Unit>().Init(_unit.faction, tracker_idCount, GameManager.rm.PlayerMaterials[faction], spawnPos, _unit.transform.rotation);
            tracker_idCount++;


            clone.GetComponent<Unit>().NavAgent().Warp(spawnPos);

            
            clone.GetComponent<Unit>().unitCount = countChange;


            clone.GetComponent<Unit>().owner = this;
            clone.GetComponent<Unit>().Update_Visuals();


        }

        SelectUnit(null);
    }









    public TextMeshPro debug_unitDisplay;

    public void DisplaySelectedUnitInfo()
    {
        if (SelectedUnit() == null || SelectedUnit().faction == -1)
        {
            return;
        }

            if (debug_unitDisplay)
        {
            if (SelectedUnit())
            {
                debug_unitDisplay.text = "Faction: " + SelectedUnit().faction + "\niD: " + SelectedUnit().id + "\n#: " + SelectedUnit().unitCount;
                debug_unitDisplay.color = GameManager.rm.PlayerColours[SelectedUnit().faction];



            }
            else { debug_unitDisplay.text = "none"; }
        }
        else
        {
            debug_unitDisplay = GameObject.Find("debugtext").GetComponent<TextMeshPro>();
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

    public Unit GetUnit(int _id)
    {

       
        foreach (Unit el in GetUnits())
        {
            if (el.id == _id)
            { return el; }
        }

        return null;
    }

    public List<Unit> GetUnits()
    {
        if (units == null)  
        { 
            units = new List<Unit>();
            foreach (Unit el in FindObjectsOfType<Unit>())
            {
                if (el.faction == GetComponent<PlayerObject>().Index)
                { units.Add(el); }
            }
        }


        return units;
    }


    public void SelectUnit(Unit _unit)
    {
        if (SelectedUnit() && SelectedUnit().Visuals()  )
        {
            SelectedUnit().Visuals().Select(false);
        }

        selectedUnit = _unit;

        if (GetCameraFollow() && selectedUnit)
        {
            GetCameraFollow().listenerTf = selectedUnit.transform;
        }

        if (SelectedUnit() && SelectedUnit().Visuals())
        {
            SelectedUnit().Visuals().Select(true);
        }

    }

    public Unit SelectedUnit( )
    {
        return selectedUnit;
    }


    public void SetPendingCommand(Unit_Command _command)
    { pendingCommand = _command; }
    public Unit_Command GetPendingCommand()
    { return pendingCommand ; }

    public Selector GetSelector()
    {
        if (selector == null)
        {
            selector = GetComponent<Selector>();
        }
        return selector;
    }

    public Visual_IssueOrders GetVisualOrders()
    {
        if (visualOrders == null)
        {
            visualOrders = GetComponent<Visual_IssueOrders>();
        }
        return visualOrders;
    }


    public CameraFollow GetCameraFollow()
    {
        if (camFollow == null)
        {
            camFollow = FindObjectOfType<CameraFollow>();
        }
        return camFollow;
    }


}
