using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visual_IssueOrders : MonoBehaviour
{

    public GameObject move;
    public GameObject attack;
    public GameObject merge;

    [Min(1)]
    public float lifeTime;
    private float timer;

    public Renderer factionRenderer;



    public void SetFaction(Material _mat)
    {
        if (factionRenderer)
        { factionRenderer.material = _mat; }

        if (attack) { SetChildColor(attack.transform,_mat.color); }
        if (move) { SetChildColor(move.transform,_mat.color); }
        if (merge) { SetChildColor(merge.transform,_mat.color); }
    }

    public void SetChildColor(Transform _parent,Color _color)
    {
        foreach (Transform el in _parent)
        {
            if (_parent.childCount > 0)
            {
                SetChildColor(el,_color);
            }

            if (el.GetComponent<MeshRenderer>())
            {
                el.GetComponent<MeshRenderer>().material.color = _color;
            }

        }

    }


    void Update()
    {
        if (timer != -1)
        {

            if (timer < 0)
            {

                timer = -1;
            }
        }
    }

    public void DisableAllOrders()
    {
        if (move)
        { move.SetActive(false); }
        if (attack)
        { attack.SetActive(false); }
        if (merge)
        { merge.SetActive(false); }
    }

    public void OrderIssued(Vector3 _point,Unit_Command _order)
    {
        DisableAllOrders();

        if (move && _order == Unit_Command.move)
        {
            
            move.SetActive(true);
            move.transform.position = _point ;
         
        }
        if (attack && _order == Unit_Command.attack)
        { attack.SetActive(true);
            attack.transform.position = _point;

        }
        if (merge && _order == Unit_Command.merge)
        {
            merge.SetActive(true);
            merge.transform.position = _point;
        }



    }


}
