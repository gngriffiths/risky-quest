
using UnityEngine;
using TMPro;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : MonoBehaviour
{



    public int unitCount;
    public int faction;
    public int id;

    [Min(1)]
    public int range;

    [Min(1)]
    public int moveSpeed;



    public Unit_Command command;

    private NavMeshAgent navAgent;

    private Visual_Unit visuals;


    public PlayerControl owner;
    private Unit target;

    private float timer_cooldown;


    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();

    }



    public void Init(int _faction, int _id,Color _color,Vector3 _pos,Quaternion _rot)
    {
        faction = _faction;
        id = _id;
        unitCount = _id;

        transform.position = _pos + (transform.right * _id * 0.1f) - ((transform.forward * _id * 0.1f) * (_id % 2));
        transform.rotation = _rot;

        visuals = GetComponent<Visual_Unit>();

        Init_Visuals(_color);

        gameObject.SetActive(true);


    }

    public void Init_Visuals(Color _color)
    {

        //NOTE: currently basic/debug visual references. Intended to be replaced with the final art elements
        if (visuals)
        {
            visuals.UpdateUnitCount(unitCount);
            visuals.SetColors(_color);
        }
        else
        {
            transform.GetChild(0).GetComponent<MeshRenderer>().material.color = _color;
        }


    }

    public void De_Init()
    {
        faction = -1;
        id = -1;
        unitCount = -1;

        transform.parent = GameManager.Instance.GetUnitPool();

        gameObject.SetActive(false);


    }



    public void Update_Visuals( )
    {
        //NOTE: currently basic/debug visual references. Intended to be replaced with the final art elements

        if (visuals)
        {
            visuals.UpdateUnitCount(unitCount);
        }
        else
        {

        }


    }


    // Update is called once per frame
    void FixedUpdate()
    {

       

        if ( GetTarget() && CurrentCooldown() <= 0)
        {
            if (ActiveCommand() == Unit_Command.attack && GetTarget() )
            {


                if (DistanceToDestination() > range)
                {
                    if (GetOwner() && GetOwner().Object.HasInputAuthority)
                    {
                        GetOwner().RPC_IssueCommand(Unit_Command.attack_move, faction, id, GetTarget().transform.position);

                    }

                }
                else
                {
                    if (ActiveCommand() == Unit_Command.attack)
                    {


                        if (GetOwner())
                        {

                            SetCooldown(GameConstants.GCD_UNITACTION);
                            GetOwner().StartCombat(this, GetTarget());

                        }
                    }
                    else if (ActiveCommand() == Unit_Command.merge)
                    { }
                }

            }
        }

        if (unitCount <= 0)
        {
            //  gameObject.SetActive(false);
            return;
        }

        UpdateCooldown(-Time.deltaTime);

    }



    public void SetNewDestination(Vector3 _dest)
    {


        if (NavAgent())
        {
            NavAgent().SetDestination(_dest);
        }

      
    }



    public float DistanceToDestination()
    {
        if (GetTarget())
        {
            return Vector3.Distance(transform.position,GetTarget().transform.position);
        }

        return NavAgent().remainingDistance;
    }





    public Unit GetTarget( )
    { return target; }

    public void SetTarget(Unit _unit)
    { target = _unit; }



    public Unit_Command ActiveCommand( )
    { return command; }

    public void SetCommand(Unit_Command _newCommand)
    { command = _newCommand; }


    public int Count( )
    {return  unitCount ; }

    public void SetCount(int _newCount)
    { 
        
        unitCount = _newCount;
        Update_Visuals();
    }

    public void UpdateCount(int _count)
    { 
        
        unitCount += _count;

        Update_Visuals();
    }


    public void SetCooldown(float _cd)
    { timer_cooldown = _cd; }

    public void UpdateCooldown(float _cd)
    { timer_cooldown += _cd; }

    public float CurrentCooldown()
    { return timer_cooldown ; }


    public PlayerControl GetOwner()
    {
        if (owner == null)
        {
            foreach (PlayerControl el in FindObjectsOfType<PlayerControl>())
            {
                if (el.faction == faction)
                { owner = el; }
            }
        }


        return owner;
    }






    public NavMeshAgent NavAgent()
    {
        if (navAgent == null)
        { navAgent = GetComponent<NavMeshAgent>(); }


        return navAgent;
    }


    public Visual_Unit Visuals()
    { return visuals; }





  



}
