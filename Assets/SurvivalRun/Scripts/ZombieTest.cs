using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ZombieAnimations
{
    public bool isPlaying;
    public string animName;
    public AnimationClip clip;
}

public class ZombieTest : MonoBehaviour
{
    public bool validate;
    private Animator animator;
    public List<ZombieAnimations> zombieAnimations = new List<ZombieAnimations>();
    public ZombieAnimations currentAnimation;

    private void OnValidate()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (validate)
        {
            validate = false;
            GetAnimationClips();
        }
        bool found = false;
        foreach (ZombieAnimations zombieAnimation in zombieAnimations)
        {
            if (zombieAnimation.isPlaying && currentAnimation != zombieAnimation && !found)
            {
                found = true;
                currentAnimation = zombieAnimation;
                animator.Play(zombieAnimation.animName);
                $"Playing {zombieAnimation.animName}".Log();
            }
            if (currentAnimation != zombieAnimation)
            {
                zombieAnimation.isPlaying = false;
            }
        }
    }

    private void GetAnimationClips()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        zombieAnimations ??= new List<ZombieAnimations>();
        zombieAnimations.Clear();
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            var zombieAnimation = new ZombieAnimations()
            {
                isPlaying = false,
                animName = clip.name,
                clip = clip
            };
            currentAnimation ??= zombieAnimation;
            zombieAnimations.Add(zombieAnimation);
        }
    }
}
