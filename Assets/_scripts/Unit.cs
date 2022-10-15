
using UnityEngine;
using TMPro;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : MonoBehaviour
{

    public Transform chainLink;

    public int unitCount;
    public int faction;
    public int id;


    [Min(1)]
    public int range;
    [Min(1)]
    public int moveSpeed;



    public Unit_Command command;

    private ControlPoint current_controlPoint;


    private NavMeshAgent navAgent;

    private Visual_Unit visuals;

    public PlayerControl owner;

    private Unit target;

    public Unit leader;
    public Unit follower;

    public float followRange=3.5f;

    private float timer_cooldown;




    [SerializeField]
    private float buffCooldown = 5.0f;
    [SerializeField]
    private float timer_buffCooldown;

    [SerializeField]
    private float current_buffPower = 0;
    [SerializeField]
    private float current_buffSpeed = 0;







    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();

    }



    public void Init(int _faction, int _id,Material _color,Vector3 _pos,Quaternion _rot)
    {
        faction = _faction;
        id = _id;

        owner = GameManager.Instance.GetPlayer(faction);

       // unitCount = 1;

        NavAgent().speed = moveSpeed;


        transform.position = _pos;
        transform.rotation = _rot;


        SetCooldown(-1);

        visuals = GetComponent<Visual_Unit>();

        Init_Visuals(_color);

        gameObject.SetActive(true);

        if (owner)
        { 
            transform.parent = owner.transform;

        }

        SetTarget(null);

        if (GetFollower())
        {
            GetFollower().SetLeader(null);
        }

        if (GetLeader())
        {
            GetLeader().SetFollower(null);
        }


        SetLeader(null);
        SetFollower(null);

        SetNewDestination(transform.position);


        transform.localScale = Vector3.one * GameConstants.UNIT_SCALE_MAGNITUDE;

    }

    public void Init_Visuals(Material _color)
    {

        //NOTE: currently basic/debug visual references. Intended to be replaced with the final art elements
        if (visuals)
        {
            Visuals().SetModel(id);

            Visuals().UpdateUnitCount(unitCount);
            Visuals().SetMaterial(_color);

            Visuals().Spawn();
        }
        else
        {
            transform.GetChild(0).GetComponent<MeshRenderer>().material = _color;
        }


    }

    public void De_Init()
    {
        if (GetFollower())
        {
            GetFollower().SetLeader(null);
        }

        if (GetLeader())
        {
            GetLeader().SetFollower(null);
        }

        Visuals().Death(transform.position);

        faction = -1;
        id = -1;

        SetCount(-1);
        SetCooldown(-1);
        command = Unit_Command.none;

        SetNewDestination(transform.position);

        owner = null;



        transform.parent = GameManager.Instance.GetUnitPool();

        gameObject.SetActive(false);


    }



    public void Update_Visuals()
    {
        //NOTE: currently basic/debug visual references. Intended to be replaced with the final art elements

        if (Visuals())
        {
            Visuals().UpdateUnitCount(unitCount);
        }
        else
        {

        }


    }


    // Update is called once per frame
    void FixedUpdate()
    {


        if (GetLeader() != null) 
        {
            if (DistanceTo(GetLeader().GetChainLinkPosition()) > followRange)
            {
                SetNewDestination(GetLeader().GetChainLinkPosition());
            }
            else 
            {
                if (ActiveCommand() == Unit_Command.merge)
                {
                    //GetLeader().
                    GiveBuff(GameConstants.SPEEDBUFF * (1 - (id % 2)), GameConstants.POWERBUFF * (id % 2));
                    timer_buffCooldown = buffCooldown;

                    SetCommand(Unit_Command.none);

                    //if (GetOwner())
                    //{
                    //    //  SetCooldown(GameConstants.GCD_UNITACTION);
                    //    if (GetLeader()  != null && timer_buffCooldown <= 0)
                    //    {
                            
                    //    }

                        
                    //}
                }
            }




        }
        else if ( GetTarget() && CurrentCooldown() <= 0)
        {
           

                if (DistanceToDestination() > range)
                {

                    if (ActiveCommand() == Unit_Command.attack || ActiveCommand() == Unit_Command.merge)
                    {
                        if (GetOwner() && GetOwner().Object.HasInputAuthority)
                        {
                            GetOwner().RPC_IssueCommand(Unit_Command.attack_move, faction, id, GetTarget().transform.position);

                        }
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
                            SetCommand(Unit_Command.none);
                        }
                    }
                else if (ActiveCommand() == Unit_Command.merge)
                {
                    if (GetOwner())
                    {
                        //  SetCooldown(GameConstants.GCD_UNITACTION);
                        if (GetLeader() && GetTopLeader() != this && timer_buffCooldown <= 0)
                        {
                            GetTopLeader().GiveBuff(3 * (id % 2), 1 * (id % 2));
                        }
                        
                        timer_buffCooldown = buffCooldown;

                        SetCommand(Unit_Command.none);
                    }
                }

            }

            




        }

        if (DistanceToDestination() <= range)
        {

           // NavAgent().speed = 0;

        }
       // else { NavAgent().speed = moveSpeed + current_buffSpeed; }



        UpdateCooldown(-Time.deltaTime);

        //decay speed buff over time
       // GiveBuff(-Time.deltaTime,0);




        if (unitCount <= 0)
        {
            //  gameObject.SetActive(false);
            return;
        }

        

    }



    public void TakeDamage(int _dmg)
    {
        UpdateCount(-_dmg);
        


        Update_Visuals();

        if (Count() <= 0 && owner)
        {
            owner.UnitDie(id);
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
        if (other.GetComponent<ControlPoint>() != null) { SetControlPoint(other.GetComponent<ControlPoint>()); }

    }
    public virtual void HandleTriggerExit(Collider other)
    {
        if (other.GetComponent<ControlPoint>() != null && GetControlPoint() == other.GetComponent<ControlPoint>()) { SetControlPoint(null); }
    }




    public void SetControlPoint(ControlPoint _point)
    { current_controlPoint = _point; }


    public ControlPoint GetControlPoint( )
    { return current_controlPoint ; }










    public void SetNewDestination(Vector3 _dest)
    {


        if (NavAgent())
        {
            NavMeshHit hit;

            if (NavMesh.SamplePosition(_dest, out hit, 1f, NavMesh.AllAreas))
            {
                NavAgent().speed = moveSpeed + current_buffSpeed;
                //NavAgent().speed = moveSpeed;
                NavAgent().SetDestination(_dest);
            }


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

    public float DistanceTo(Vector3 _target)
    {
        return Vector3.Distance(transform.position, _target);
    }



    public Unit GetTarget( )
    { return target; }

    public void SetTarget(Unit _unit)
    { target = _unit; }


    public Unit GetTopLeader()
    {
          if (leader == null) { return this; }
        //to avoid possible infinite loops
        if (leader.GetLeader() == this) { return this; }

        return leader.GetLeader();
    }
    public Unit GetBottomFollower()
    {
        if (follower == null) { return this; }
        //to avoid possible infinite loops
        if (follower.GetFollower() == this) { return this; }

        return follower.GetBottomFollower();
    }

    public Unit GetLeader()
    {
        //if (leader == null) { return this; }
        ////to avoid possible infinite loops
        //if (leader.GetLeader() == this) { return this; }

        return leader; 
    }

    public void SetLeader(Unit _unit)
    {
       
        if (_unit != this)
        {
            if (leader && leader.GetFollower() == this) { leader.SetFollower(null); }

            leader = _unit;
            if (leader) { leader.SetFollower(this); }
        }
        else 
        {
            if (leader && leader.GetFollower() == this) { leader.SetFollower(null); }
            leader = null;
        }
        
    }

    public Vector3 GetChainLinkPosition()
    {
        if (chainLink == null)
        { return transform.position; }
        return chainLink.position;
    }


    public Unit GetFollower()
    {
        //if (follower == null) { return this; }
        return follower; 
    
    }

    public void SetFollower(Unit _unit)
    {
        if (_unit != this) { follower = _unit; }
        else
        {
            follower = null;
        }
    }


    public Unit_Command ActiveCommand( )
    { return command; }

    public void SetCommand(Unit_Command _newCommand)
    { command = _newCommand; }


    public void GiveBuff(float _spd,float _power)
    {
        current_buffSpeed += _spd;
        current_buffPower += _power;

        current_buffSpeed = Mathf.Clamp(current_buffSpeed , 0,GameConstants.MAX_SPEEDBUFF);
        current_buffPower = Mathf.Clamp(current_buffPower ,0, GameConstants.MAX_POWERBUFF);

        NavAgent().speed = moveSpeed + current_buffSpeed;

        if (GetLeader())
        { GetLeader().GiveBuff(_spd,_power); }

    }










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
    {
        if (timer_cooldown > 0)
        { timer_cooldown += _cd; }
        if (timer_buffCooldown > 0)
        { timer_buffCooldown += _cd; }


    }

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
    {
        if (visuals == null)
        { visuals = GetComponent<Visual_Unit>(); }

        return visuals;
    }





  



}
