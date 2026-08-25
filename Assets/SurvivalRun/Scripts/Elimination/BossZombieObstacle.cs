using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace ZombieElimination
{
    public class BossZombieObstacle : ObstacleBase
    {
        public GameObject zombieBoss;
        public Animator zombieBossAnimator;

        public List<OnTriggerEvent> attackTriggers;

        public GameObject leftObject, rightObject;
        public readonly string[] attackAnimations = new string[] { "TwoHandCombo", "StandingTorchMelee" };

        protected override void Awake()
        {
            base.Awake();
            zombieBoss.SetActive(true);
            zombieBossAnimator = zombieBoss.GetComponent<Animator>();

            attackTriggers.Add(leftObject.GetComponent<OnTriggerEvent>());
            attackTriggers.Add(rightObject.GetComponent<OnTriggerEvent>());
        }

        private void Start()
        {
            foreach (var trigger in attackTriggers)
            {
                trigger.OnTriggerEnterEvent += OnAttackTriggerEnter;
            }
            zombieBoss.SetActive(false);
        }

        private void OnAttackTriggerEnter(Collider obj)
        {
            if (obj.TryGetComponent(out Player player))
            {
                StartCoroutine(PlayerHit(player));
            }
        }

        private IEnumerator PlayerHit(Player player)
        {
            $"Player {player.name} was hit by BossZombie".Log();
            player.StartElimination();
            player.Stop();
            yield return null;
            player.EnableRagdoll();
            yield return new WaitForSecondsRealtime(4f);
            player.Die();
        }


        protected override void OnPlayerEntered(Player player)
        {
            if (isTriggerActive)
            {
                return;
            }
            isTriggerActive = true;
            zombieBoss.SetActive(true);
            string attackAnimation = attackAnimations[UnityEngine.Random.Range(0, attackAnimations.Length)];
            if (attackAnimation == "TwoHandCombo")
            {
                leftObject.SetActive(true);
                rightObject.SetActive(true);
            }
            else
            {
                leftObject.SetActive(true);
                rightObject.SetActive(false);
            }
            zombieBossAnimator.CrossFade(attackAnimation, 0.1f);
        }
    }
}
