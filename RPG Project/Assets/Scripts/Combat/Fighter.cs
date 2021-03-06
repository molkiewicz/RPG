﻿using RPG.Core;
using RPG.Resources;
using RPG.Movement;
using RPG.Saving;
using UnityEngine;
namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        //should be in weapon config
        
        [SerializeField] private float timeBetweenAttacks;
        
        [SerializeField] private Transform rightHandTransform = null;
        [SerializeField] private Transform leftHandTransform = null;
        [SerializeField] private Weapon defaultWeapon = null;
        [SerializeField] private string defaultWeaponName = "Unarmed";

        private Mover mover;
        private Health target;
        private ActionScheduler actionScheduler;
        private Animator animator;
        private float timeSinceLastAttack = Mathf.Infinity;
        private Weapon currentWeapon=null;
        private void Awake()
        {
            mover = GetComponent<Mover>();
            actionScheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();
            if (currentWeapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
        }
        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (target == null) return;
            if (target.IsDead()) return;
            if (!GetIsInRange())
            {
                //full speed fraction as 2nd argument
                mover.MoveTo(target.transform.position,1f);
            }
            else
            {
                mover.Cancel();
                AttackBehaviour(); 
            }
        }
        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon = weapon;
            currentWeapon.Spawn(rightHandTransform,leftHandTransform, animator);
        }
        public Health GetTarget()
        {
            return target;
        }
        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                //This will trigger Animation Event Hit()
                TriggerAttack();
                timeSinceLastAttack = 0f;
            }
        }

        private void TriggerAttack()
        {
            animator.ResetTrigger("stopAttack");
            animator.SetTrigger("attack");
        }

        //Animation Event
        void Hit()
        {
            if (target == null) return;
            if (currentWeapon.HasProjectile())
            {
                currentWeapon.LaunchProjectile(rightHandTransform,leftHandTransform,target,gameObject);
            }
            else
            {
                target.TakeDamage(gameObject,currentWeapon.GetDamage());
            }
        }
        void Shoot()
        {
            Hit();
        }
        public void Attack(GameObject combatTarget)
        {
            actionScheduler.StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }
        public void Cancel()
        {
            StopAttack();
            target = null;
            mover.Cancel();
        }

        private void StopAttack()
        {
            animator.ResetTrigger("attack");
            animator.SetTrigger("stopAttack");
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }
        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.GetRange();
        }

        public object CaptureState()
        {
            return currentWeapon.name;
        }

        public void RestoreState(object state)
        {
            string loadedWeapon = (string)state;
            Weapon weapon = UnityEngine.Resources.Load<Weapon>(loadedWeapon);
            EquipWeapon(weapon);
        }
    }
}