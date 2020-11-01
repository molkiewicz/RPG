﻿using UnityEngine;
namespace RPG.Core
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float healthPoints = 100f;
        private bool isDead=false;
        private Animator animator;
        private ActionScheduler actionScheduler;
        public bool IsDead()
        {
            return isDead;
        }
        private void Start()
        {
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
        }
        public void TakeDamage(float damage)
        {
            if (isDead) return;
            healthPoints = Mathf.Max(healthPoints - damage, 0f);
            if (healthPoints == 0f)
            {
                Die();
            }
        }
        private void Die()
        {
            isDead = true;
            animator.SetTrigger("death");
            actionScheduler.CancelCurrentAction();
        }
    }
}