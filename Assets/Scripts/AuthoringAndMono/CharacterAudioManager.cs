using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AxieRescuer
{
    public class RunningAudioManager : MonoBehaviour
    {
        [Header("Running")]
        public AudioClip RunningAudioClip;
        [Range(0, 1)] public float RunningAudioVolume = 1;

        [Header("Handgun Audio")]
        public AudioClip HandgunShootAudioClip;
        [Range(0, 1)] public float HandgunShootVolume = 1;
        public AudioClip HandgunReloadAudioClip;
        [Range(0, 1)] public float HandgunReloadVolume = 1;

        [Header("Autogun Audio")]
        public AudioClip AutogunShootAudioClip;
        [Range(0, 1)] public float AutogunShootVolume = 1;
        public AudioClip AutogunReloadAudioClip;
        [Range(0, 1)] public float AutogunReloadVolume = 1;

        [Header("Showgun Audio")]
        public AudioClip ShotgunShootAudioClip;
        [Range(0, 1)] public float ShotgunShootVolume = 1;
        public AudioClip ShotgunReloadAudioClip;
        [Range(0, 1)] public float ShotgunReloadVolume = 1;

        [Header("Rifle Audio")]
        public AudioClip RifleShootAndReloadAudioClip;
        [Range(0, 1)] public float RifleShootAndReloadVolume = 1;

        [Header("SubmachineGun Audio")]
        public AudioClip SubmachineGunShootAudioClip;
        [Range(0, 1)] public float SubmachineGunShootVolume = 1;
        public AudioClip SubmachineGunReloadAudioClip;
        [Range(0, 1)] public float SubmachinegunReloadVolume = 1;

        [Header("Minigun Audio")]
        public AudioClip MinigunShootAudioClip;
        [Range(0, 1)] public float MinigunShootVolume = 1;
        public AudioClip MinigunReloadAudioClip;
        [Range(0, 1)] public float MinigunReloadVolume = 1;


        private AudioSource gunAudioSource;
        private AudioSource runningAudioSource;

        void Start()
        {
            var runningAudioSourceObject = Instantiate(new GameObject(nameof(runningAudioSource)), transform);
            runningAudioSource = runningAudioSourceObject.AddComponent<AudioSource>();
            runningAudioSource.clip = RunningAudioClip;
            runningAudioSource.loop = true;
            runningAudioSource.playOnAwake = false;
            runningAudioSource.volume = RunningAudioVolume;
            var gunAudioSourceObject = Instantiate(new GameObject(nameof(gunAudioSource)), transform);
            gunAudioSource = gunAudioSourceObject.AddComponent<AudioSource>();
            gunAudioSource.clip = RunningAudioClip;
            gunAudioSource.loop = false;
            gunAudioSource.playOnAwake = false;
        }

        public void Running()
        {
            if (runningAudioSource != null && runningAudioSource.clip != null && !runningAudioSource.isPlaying)
            {
                runningAudioSource.Play();
            }
        }

        public void Idle()
        {
            if (runningAudioSource != null && runningAudioSource.clip != null && runningAudioSource.isPlaying)
            {
                runningAudioSource.Pause();
            }
        }

        public void Handgun_Shoot()
        {
            gunAudioSource.clip = HandgunShootAudioClip;
            gunAudioSource.volume = HandgunShootVolume;
            gunAudioSource.Play();
        }

        public void Handgun_Reload()
        {
            gunAudioSource.clip = HandgunReloadAudioClip;
            gunAudioSource.volume = HandgunReloadVolume;
            gunAudioSource.Play();
        }

        public void Autogun_Shoot()
        {
            gunAudioSource.clip = AutogunShootAudioClip;
            gunAudioSource.volume = AutogunShootVolume;
            gunAudioSource.Play();
        }

        public void Autogun_Reload()
        {
            gunAudioSource.clip = AutogunReloadAudioClip;
            gunAudioSource.volume = AutogunReloadVolume;
            gunAudioSource.Play();
        }

        public void Shotgun_Shoot()
        {
            gunAudioSource.clip = ShotgunShootAudioClip;
            gunAudioSource.volume = ShotgunShootVolume;
            gunAudioSource.Play();
        }

        public void Shotgun_Reload()
        {
            gunAudioSource.clip = ShotgunReloadAudioClip;
            gunAudioSource.volume = ShotgunReloadVolume;
            gunAudioSource.Play();
        }

        public void Rifle_ShootAndReload()
        {
            gunAudioSource.clip = RifleShootAndReloadAudioClip;
            gunAudioSource.volume = RifleShootAndReloadVolume;
            gunAudioSource.Play();
        }

        public void Submachinegun_Shoot()
        {
            gunAudioSource.clip = SubmachineGunShootAudioClip;
            gunAudioSource.volume = SubmachineGunShootVolume;
            gunAudioSource.Play();
        }

        public void Submachinegun_Reload()
        {
            gunAudioSource.clip = SubmachineGunReloadAudioClip;
            gunAudioSource.volume = SubmachinegunReloadVolume;
            gunAudioSource.Play();
        }

        public void Minigun_Shoot()
        {
            gunAudioSource.clip = MinigunShootAudioClip;
            gunAudioSource.volume = MinigunShootVolume;
            gunAudioSource.Play();
        }

        public void Minigun_Reload()
        {
            gunAudioSource.clip= MinigunReloadAudioClip;
            gunAudioSource.volume = MinigunReloadVolume;
            gunAudioSource.Play();
        }
    }
}
