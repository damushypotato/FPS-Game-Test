using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Camera cam;

    public float damage;
    public float range;
    public float fireRate;
    public float impactForce;
    public bool auto;
    public bool canShoot = true;
    public LayerMask hittable;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    private void Awake()
    {
        //muzzleFlash = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (PauseMenu.GameIsPaused) return;

        if (canShoot)
        {
            if (auto)
            {
                if (Input.GetButton("Fire1"))
                {
                    Shoot();
                }
            }
            else
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    Shoot();
                }
            }
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range, hittable))
        {
            Debug.Log(hit.transform.name);

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForceAtPosition(-hit.normal * impactForce, hit.point);
            }

            Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }

        muzzleFlash.Play();

        canShoot = false;
        Invoke(nameof(EnableShoot), fireRate);
    }

    void EnableShoot()
    {
        canShoot = true;
    }
}
