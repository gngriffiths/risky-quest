using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visual_Unit : MonoBehaviour
{
    public MeshRenderer primaryRenderer;
    public MeshRenderer selectedRenderer;

    public Transform parent_unitCountIndicators;


    public void SetColors(Color _color)
    {
        if (primaryRenderer) { primaryRenderer.material.color = _color; }

        if (parent_unitCountIndicators == null) { return; }

        foreach (Transform el in parent_unitCountIndicators)
        {
            

            if (el.GetComponent<MeshRenderer>())
            {
                el.GetComponent<MeshRenderer>().material.color = _color;
            }
        }

        
    }

    public void UpdateUnitCount(int _count)
    {
        if (parent_unitCountIndicators == null) { return; }

        foreach (Transform el in parent_unitCountIndicators)
        {
            el.gameObject.SetActive(false);
        }

        if (parent_unitCountIndicators.childCount > _count)
        {
            if (_count > 0)
            {
                parent_unitCountIndicators.GetChild(_count - 1).gameObject.SetActive(true);
                parent_unitCountIndicators.GetChild(_count - 1).localScale = parent_unitCountIndicators.GetChild(0).localScale;
            }
        }
        else
        {
            parent_unitCountIndicators.GetChild(_count - 1).gameObject.SetActive(true);
            parent_unitCountIndicators.GetChild(_count - 1).localScale *= 1 + (0.1f * _count);
        }
  
    }

    public void Select(bool _select)
    {
        if (selectedRenderer)
        {
            if (_select)
            { selectedRenderer.material.color = Color.yellow; }
            else
            { selectedRenderer.material.color = Color.gray; }
        }
    }

}
