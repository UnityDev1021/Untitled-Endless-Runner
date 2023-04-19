using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioClip[] effectsClip;
        [SerializeField] private GameLogic localGameLogic;
        [SerializeField] private AudioSource[] soundEffectsSource;

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        //For Click Sounds
        public void PlaySource0(int clipIndex)
        {
            soundEffectsSource[0].clip = effectsClip[clipIndex];
            soundEffectsSource[0].Play();
        }

        public void PlaySource1(int clipIndex)
        {
            soundEffectsSource[1].clip = effectsClip[clipIndex];
            soundEffectsSource[1].Play();
        }
    }
}
