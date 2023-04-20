using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private GameLogic localGameLogic;
        [SerializeField] private AudioClip[] effectsClip, musicClips;

        /*===================> List <===============================
         * Element 0 -> For Normal Sound Effects
         * Element 1 -> For Sustained SOund Effects For Player
         * Element 2 -> For BGM
         ===========================================================*/
        [SerializeField] private AudioSource[] soundEffectsSource;

        [Space]
        private byte bgClipIndex = 0;

        private void OnEnable()
        {
            localGameLogic.OnPowerUpCollected += PlaySoundEffect;
            localGameLogic.OnPlayerAction += PlayActionSoundEffect;
        }

        private void OnDisable()
        {
            localGameLogic.OnPowerUpCollected -= PlaySoundEffect;
            localGameLogic.OnPlayerAction -= PlayActionSoundEffect;
        }

        private void Start()
        {
            //Debug.Log($"Clip Length : {soundEffectsSource[2].clip.length}");
            Invoke(nameof(PlayAudioSource2), 0f);                //INvoke BG Music
        }

        private void PlayActionSoundEffect(PlayerAction actionDone, byte status)
        {
            if (status == 0)
            {
                switch (actionDone)
                {
                    case PlayerAction.SpeedBoost:
                    case PlayerAction.Hit:
                        break;

                    case PlayerAction.Dash:
                        {
                            PlayAudioSource0(7);
                            break;
                        }

                    case PlayerAction.Slide:
                        {
                            PlayAudioSource0(6);
                            break;
                        }

                    case PlayerAction.Jump:
                        {
                            PlayAudioSource0(2);
                            break;
                        }

                    default:
                        {
                            Debug.LogError($"No Player Action found of type : {actionDone.ToString()}");
                            break;
                        }
                }
            }
        }

        private void PlaySoundEffect(ObstacleTag detectedTag, int amount)
        {
            switch (detectedTag)
            {
                //Do Nothing
                case ObstacleTag.Score2x:
                case ObstacleTag.Dash:
                case ObstacleTag.Heart:
                case ObstacleTag.HigherJump:
                    {
                        if (amount == 1)
                            PlayAudioSource0(3);

                        break;
                    }

                case ObstacleTag.Coin:
                    {
                        if (amount == 1)
                            PlayAudioSource0(4);

                        break;
                    }

                case ObstacleTag.SpeedBoost:
                    {
                        if (amount == 1)
                            PlayAudioSource0(8);

                        break;
                    }

                case ObstacleTag.Shield:
                    {
                        if (amount == 1)
                            PlayAudioSource0(5);

                        break;
                    }

                default:
                    {
                        Debug.LogError($"Wrong Tag Detected for tag : {detectedTag}");
                        break;
                    }
            }
        }

        //For Click Sounds
        public void PlayAudioSource0(int clipIndex)
        {
            soundEffectsSource[0].clip = effectsClip[clipIndex];
            //soundEffectsSource[0].Play();
            soundEffectsSource[0].PlayOneShot(effectsClip[clipIndex], 1);
        }

        //For EffectsOnPlayer Sounds
        public void PlayAudioSource1(int clipIndex)
        {
            soundEffectsSource[1].clip = effectsClip[clipIndex];
            soundEffectsSource[1].Play();
        }

        //For BGM
        public void PlayAudioSource2()
        {
            soundEffectsSource[2].clip = musicClips[bgClipIndex];
            soundEffectsSource[2].Play();

            bgClipIndex = (byte) Random.Range(0, 2);
            Invoke(nameof(PlayAudioSource2), (musicClips[bgClipIndex].length + 1));
            //Debug.Log($"Clip Length : {musicClips[bgClipIndex].length}");
        }
    }
}
