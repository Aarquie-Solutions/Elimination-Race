using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimationHelper : MonoBehaviour
{
    public static string biteAttemptNeck = "Paired_Zombie_BiteAttempt_BiteNeck";
    public static string biteAttemptFromBehind = "Paired_Zombie_BiteAttempt_BiteNeck_FromBehind";

    public static string Death = "Zombie_Idle_Death";

    public static readonly string Starting = "_Start";
    public static readonly string Ending = "_End";
    public static readonly string Looping = "_Loop";
    public static readonly string ElbowOff = "_ElbowOff";

    public static readonly string ZombieAttacker = "_Att";
    public static readonly string HumanVictim = "_Vic";

    public enum AnimationType
    {
        None,
        Paired_Zombie_BiteAttempt_BiteNeck_FromBehind,
        Paired_Zombie_BiteAttempt_BiteNeck,
    }

    public AnimatorPlayer[] models;


    public AnimationType currentAnimationType;
    public AnimationType animationType;

    public RuntimeAnimatorController baseAnimator;
    public string[] allAnimations;

    private void OnValidate()
    {
        if (baseAnimator != null)
            allAnimations = baseAnimator.animationClips.Select(x => x.name).ToArray();
    }

    private void Start()
    {
        Array.ForEach(models, x => x.Init());
        //  PlayAnimation(AnimationType.Paired_Zombie_BiteAttempt_BiteNeck_Start);
    }

    Coroutine coroutine;

    public void Update()
    {
        if (animationType != currentAnimationType)
        {
            switch (animationType)
            {
                case AnimationType.Paired_Zombie_BiteAttempt_BiteNeck:
                    break;
                case AnimationType.Paired_Zombie_BiteAttempt_BiteNeck_FromBehind:
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                    }
                    int num = Random.Range(0, 2);
                    var value = num == 1;
                    $"Randome value: {num}".Log();
                    coroutine = StartCoroutine(PlayBitingSuccessAnimation(value));
                    break;
                default:
                    break;
            }
        }
        currentAnimationType = animationType;
    }

    private void PlayAnimation(string animationName)
    {
        for (int i = 0; i < models.Length; i++)
        {
            var model = models[i];
            if (model.animator == null)
            {
                model.animator = model.model.GetComponent<Animator>();
            }
            if (model.hips == null)
            {
                model.hips = model.animator.GetBoneTransform(HumanBodyBones.Hips);
            }
            model.PlayAnimationWithMode(animationName);
            // model.animator.CrossFade(animationName + (model.isHuman ? humanVictim : zombieAttacker), 0.1f);
        }
    }


    private IEnumerator PlayBitingSuccessAnimation(bool value)
    {
        yield return null;
        PlayAnimation(biteAttemptFromBehind + Starting);
        yield return new WaitForSeconds(models[0].GetCurrentClip().length + 0.1f);
        PlayAnimation(biteAttemptFromBehind + Looping);
        yield return new WaitForSeconds(2f);
        if (value)
        {
            PlayAnimation(biteAttemptFromBehind + Ending);
        }
        else
        {
            PlayAnimation(biteAttemptFromBehind + ElbowOff);
        }
        yield return new WaitForSeconds(models[0].GetCurrentClip().length + 0.2f);
        for (int i = 0; i < models.Length; i++)
        {
            var mo = models[i];
            //mo.ResetLocalPosition();

            mo.animator.CrossFade("Zombie_Idle_Death", 0.1f);
        }
        animationType = AnimationType.None;
        coroutine = null;
    }
}

[Serializable]
public class AnimatorPlayer
{
    public bool isHuman;
    public GameObject model;
    public Animator animator;
    public Transform hips;

    private AnimationClip[] clips;

    [field: SerializeField] public AnimationClip CurrentAnimationClip { get; private set; }

    private string currentAnimationName;

    public AnimatorPlayer(GameObject root, bool isHuman)
    {
        this.model = root;
        this.isHuman = isHuman;
        Init();
    }

    public void Init()
    {
        animator = model.GetComponent<Animator>();

        if (hips == null)
        {
            hips = animator.GetBoneTransform(HumanBodyBones.Hips);
        }
        clips = animator.runtimeAnimatorController.animationClips;
    }

    public void PlayAnimationWithMode(string animationName)
    {
        currentAnimationName = animationName + (isHuman ? AnimationHelper.HumanVictim : AnimationHelper.ZombieAttacker);
        animator.CrossFade(currentAnimationName, 0.1f);
    }

    public void PlayDirectAnimation(string animationName)
    {
        currentAnimationName = animationName;
        animator.CrossFade(currentAnimationName, 0.1f);
    }

    public AnimationClip GetCurrentClip()
    {
        // var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // string clipName = stateInfo.IsName("") ? "Unknown" : GetActiveClipName(stateInfo);

        CurrentAnimationClip = FindClipByName(currentAnimationName);
        if (CurrentAnimationClip != null)
        {
            //   Debug.Log($"Current clip: {CurrentAnimationClip.name}, Length: {CurrentAnimationClip.length}s");
        }
        return CurrentAnimationClip;
    }

    private AnimationClip FindClipByName(string name)
    {
        if (animator == null || string.IsNullOrEmpty(name))
            return null;

        return clips.FirstOrDefault(c => c.name == name);
    }

    private string GetActiveClipName(AnimatorStateInfo stateInfo)
    {
        foreach (var clip in clips)
        {
            if (stateInfo.IsName(clip.name))
            {
                return clip.name;
            }
        }

        return null;
    }

    public void ResetLocalPosition()
    {
        model.transform.localPosition = Vector3.zero;
    }

    public void Die()
    {
        PlayDirectAnimation(AnimationHelper.Death);
    }
}
