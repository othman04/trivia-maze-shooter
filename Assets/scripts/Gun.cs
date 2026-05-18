using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    [Header("Gun Settings")]
    public float reloadTime = 1f;
    public float fireRate = 0.15f;
    public int magSize = 20;

    [Header("References")]
    public Bullet bullet; // <-- MODIFIÉ : Référence au SCRIPT 'Bullet', pas GameObject
    public Transform bulletSpawnPoint;
    public GameObject muzzleFlashEffect; // <-- AJOUT : L'effet visuel de tir (particules)

    [Header("Reload Animation Offsets")]
    [SerializeField] private Vector3 reloadRotationOffset = new Vector3(66, 50, 50); 

    private int currentAmmo;
    private bool isReloading = false;
    private float nextTimeToFire = 0f;
    
    private Quaternion initialRotation;
    private Vector3 initialPosition;

    void Start()
    {
        currentAmmo = magSize;
        initialRotation = transform.localRotation;
        initialPosition = transform.localPosition;
    }

    public void Shoot()
{
    if (isReloading || Time.time < nextTimeToFire)
        return;

    if (currentAmmo <= 0)
    {
        StartCoroutine(Reload());
        return;
    }

    nextTimeToFire = Time.time + fireRate;
    currentAmmo--;

    // ================== LOGIQUE CORRIGÉE DU MUZZLE FLASH ==================
    if (muzzleFlashEffect != null && bulletSpawnPoint != null)
    {
        // CORRECTION : On passe "bulletSpawnPoint" en 4ème argument pour définir le PARENT
        GameObject flash = Instantiate(muzzleFlashEffect, bulletSpawnPoint.position, bulletSpawnPoint.rotation, bulletSpawnPoint);
        
        // CORRECTION DE SÉCURITÉ : On force sa position locale à 0 pour l'aligner parfaitement
        flash.transform.localPosition = Vector3.zero;
        flash.transform.localRotation = Quaternion.identity;

        Destroy(flash, 0.1f); 
    }

    // Lancement de la balle (reste inchangé)
    if (bullet != null && bulletSpawnPoint != null)
    {
        Bullet spawnedBullet = Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        spawnedBullet.Launch();
    }
}

    IEnumerator Reload()
    {
        isReloading = true;
        Quaternion targetRotation = Quaternion.Euler(initialRotation.eulerAngles + reloadRotationOffset);
        float halfReload = reloadTime / 2f;
        float t = 0f;

        // Phase 1 : Rotation vers la position de recharge
        while (t < halfReload)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(initialRotation, targetRotation, t / halfReload);
            yield return null;
        }

        t = 0f;
        // Phase 2 : Retour à la position initiale
        while (t < halfReload)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(targetRotation, initialRotation, t / halfReload);
            yield return null;
        }

        currentAmmo = magSize;
        isReloading = false;
    }

    public void TryReload()
    {
        if (isReloading || currentAmmo == magSize)
            return;
        StartCoroutine(Reload());
    }
}