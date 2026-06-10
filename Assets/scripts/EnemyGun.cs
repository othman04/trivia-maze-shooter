using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    [Header("Settings")]
    public float fireRate = 0.5f;        // Cadence de tir du soldat
    
    [Header("References")]
    public Bullet bulletPrefab;          // Glisse ton prefab de balle ici (ton script Bullet mis à jour)
    public Transform bulletSpawnPoint;   // Crée un objet vide au bout du canon du soldat et glisse-le ici
    public GameObject muzzleFlashEffect; // Optionnel : l'effet de fumée/lumière au bout du canon

    private float nextTimeToFire = 0f;

    public void Shoot()
    {
        // Sécurité sur la cadence de tir
        if (Time.time < nextTimeToFire) return;
        
        nextTimeToFire = Time.time + fireRate;

        // 1. Gestion du Muzzle Flash visuel
        if (muzzleFlashEffect != null && bulletSpawnPoint != null)
        {
            GameObject flash = Instantiate(muzzleFlashEffect, bulletSpawnPoint.position, bulletSpawnPoint.rotation, bulletSpawnPoint);
            flash.transform.localPosition = Vector3.zero;
            flash.transform.localRotation = Quaternion.identity;
            Destroy(flash, 0.1f); 
        }

        // 2. Création et lancement de la balle
        if (bulletPrefab != null && bulletSpawnPoint != null)
        {
            Bullet spawnedBullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            
            // Propulse la balle vers l'avant du canon du soldat
            spawnedBullet.Launch(bulletSpawnPoint.forward); 
        }
    }
}