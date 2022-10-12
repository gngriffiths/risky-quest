using System.Collections.Generic;
using UnityEngine;

public class Visual_Unit : MonoBehaviour
{


    public MeshRenderer primaryRenderer;
    public MeshRenderer selectedRenderer;

    public MeshFilter carModel;
    public List<Mesh> models;


    public GameObject selectedIndicator;
    public GameObject weaponEffects;

    public GameObject deathEffect;




    public Transform parent_unitCountIndicators;

    //public ParticleSystem attack;

    VehicleVFX vehicleVFX
    {
        get
        {
            {
                if (vehicleVFXCached == null)
                    vehicleVFXCached = GetComponentInChildren<VehicleVFX>();
                return vehicleVFXCached;
            }
        }
    }
    VehicleVFX vehicleVFXCached;

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

    public void SetMaterial(Material _color)
    {
        if (primaryRenderer) { primaryRenderer.material = _color; }

        if (parent_unitCountIndicators == null) { return; }

        foreach (Transform el in parent_unitCountIndicators)
        {


            if (el.GetComponent<MeshRenderer>())
            {
                el.GetComponent<MeshRenderer>().material = _color;
            }
        }


    }


    public void UpdateUnitCount(int _count)
    {
        if (parent_unitCountIndicators == null) { return; }

        int count = 0;

        foreach (Transform el in parent_unitCountIndicators)
        {
            if (count < _count)
            {
                el.localPosition = new Vector3(0, 0, -0.05f * count);
                if (el.GetComponent<MeshRenderer>() )
                {
                    el.GetComponent<MeshRenderer>().material = primaryRenderer.material;
                }
                el.gameObject.SetActive(true);
            }
            else
            {
                el.gameObject.SetActive(false);
            }
            count++;
        }


    }

    public void Select(bool _select)
    {
        //if (selectedRenderer)
        //{
        //    if (_select)
        //    { selectedRenderer.material.color = Color.yellow; }
        //    else
        //    { selectedRenderer.material.color = Color.gray; }
        //}

        if (selectedIndicator)
        {
            selectedIndicator.SetActive(_select); 
        }

    }

    public void Death(Vector3 _pos)
    {
        if (deathEffect)
        {
            deathEffect.SetActive(false);
            deathEffect.transform.parent = null;
            deathEffect.transform.position = _pos;

        }
    }


    public void Spawn()
    {
        if (deathEffect)
        { 
            deathEffect.SetActive(false);
        }
    }



    public void StartAttack()
    {

        if (weaponEffects)
        {
            weaponEffects.SetActive(false);
            weaponEffects.SetActive(true);
        }

      //  vehicleVFX.Shoot(true);

    }

    public void EndAttack()
    {
        if (weaponEffects)
        {
            weaponEffects.SetActive(false);
        }

      //  vehicleVFX.Shoot(false);
    }



    public void StartDefense()
    { }

    public void EndDefense()
    { }


    public void SetModel(int _id)
    {
        if (carModel != null && models != null && models.Count > 0)
        {
            if (models.Count > _id % models.Count)
            { 
                carModel.mesh = models[_id % models.Count];
            }
        }
    }


}
