using FMODUnity;
using log4net.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Transform = UnityEngine.Transform;

public static class WeaponFX
{
    private static float _recoilVisual = 1.2f;

    public static void PlayShootFX(MonoBehaviour weaponScript, Transform shootTransform, Transform turretHead, float timeBetweenShots,
        Vector3 originalWeaponHeadPosition, GameObject projectileShellPrefab, EventReference shootingSfx, ParticleSystem shootBubbleParticles)
    {
        GameAudioManager.Instance.PlaySound(shootingSfx, weaponScript.transform.position);

        if (shootBubbleParticles != null) { shootBubbleParticles.Play(); }

        if (turretHead != null) { weaponScript.StartCoroutine(PlayWeaponRecoilFX(weaponScript.transform, shootTransform, turretHead, timeBetweenShots, originalWeaponHeadPosition)); }

        weaponScript.StartCoroutine(SpawnProjectileShell(turretHead, projectileShellPrefab));
    }

    private static IEnumerator PlayWeaponRecoilFX(Transform transform, Transform shootTransform, Transform turretHead, float timeBetweenShots, Vector3 originalWeaponHeadPosition)
    {
        Vector3 lookDir = (shootTransform.position - transform.position).normalized;
        Vector3 desiredRecoilPosition = turretHead.transform.localPosition + -lookDir * _recoilVisual;

        float elapsedTime = 0;
        float waitTime = timeBetweenShots / 2f;


        //go backwards
        while (elapsedTime < waitTime)
        {
            Vector3 desiredPos = Vector3.Lerp(turretHead.transform.localPosition, desiredRecoilPosition, (elapsedTime / waitTime));

            turretHead.transform.localPosition = desiredPos;

            elapsedTime += Time.deltaTime;

            yield return 0;
        }

        elapsedTime = 0;

        //go forwards
        while (elapsedTime < waitTime)
        {
            Vector3 desiredPos = Vector3.Lerp(turretHead.transform.localPosition, originalWeaponHeadPosition, (elapsedTime / waitTime));

            turretHead.transform.localPosition = desiredPos;

            elapsedTime += Time.deltaTime;

            yield return 0;
        }
    }

    private static IEnumerator SpawnProjectileShell(Transform turretHead, GameObject projectileShellPrefab)
    {
        if (projectileShellPrefab == null) { yield break; }

        GameObject newShell = GameObject.Instantiate(projectileShellPrefab, turretHead.position, Quaternion.identity);

        Rigidbody rb = newShell.GetComponent<Rigidbody>();

        rb.AddForce(turretHead.up * 20f, ForceMode.Impulse);

        yield return new WaitForSeconds(30f);

        rb.isKinematic = true;
    }

    public static ParticleSystem InstantiateBubbleParticles(Transform shootTransform)
    {
        ParticleSystem shootBubbleParticles = GameObject.Instantiate(GameAssetsManager.Instance.ShootBubbleParticles, shootTransform).GetComponent<ParticleSystem>();
        shootBubbleParticles.transform.localPosition = Vector3.zero;
        shootBubbleParticles.transform.localRotation = Quaternion.identity;

        return shootBubbleParticles;
    }
}
