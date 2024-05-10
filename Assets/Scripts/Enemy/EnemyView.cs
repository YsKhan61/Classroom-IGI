﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace StatePattern.Enemy
{
    public class EnemyView : MonoBehaviour
    {
        
        [SerializeField] private EnemyTriggerBehaviour triggerBehaviour;
        [SerializeField] public NavMeshAgent Agent;
        [SerializeField] private SpriteRenderer enemyGraphic;
        [SerializeField] private SpriteRenderer detectableRange;
        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private List<EnemyColor> enemyColors;
        [SerializeField] private GameObject bloodStain;

        private SphereCollider rangeTriggerCollider;

        public EnemyController Controller { get; private set; }

        [Header("Debug Only")]
        [SerializeField, TextArea(5, 5)]
        private string debugMessage;

        private void Start()
        {
            rangeTriggerCollider = GetComponent<SphereCollider>();
            Controller?.InitializeAgent();
        }

        public void SetController(EnemyController controllerToSet)
        {
            Controller = controllerToSet;
            triggerBehaviour.SetController(controllerToSet);        // considering every trigger 
        }

        public void SetTriggerRadius(float radiusToSet)
        {
            SetRangeColliderRadius(radiusToSet);
            SetRangeImageRadius(radiusToSet);
        }

        private void SetRangeColliderRadius(float radiusToSet)
        {
            if (rangeTriggerCollider != null)
                rangeTriggerCollider.radius = radiusToSet;
        }

        private void SetRangeImageRadius(float radiusToSet) => detectableRange.transform.localScale = new Vector3(radiusToSet, radiusToSet, 1);

        public void PlayShootingEffect() => muzzleFlash.Play();

        private void Update() => Controller?.UpdateEnemy();

        /*private void OnTriggerEnter(Collider other)
        {
            if (!other.isTrigger || !other.TryGetComponent(out PlayerView playerView)) return;
            Controller.PlayerEnteredRange(playerView.Controller);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.isTrigger || !other.TryGetComponent(out PlayerView playerView)) return;
            Controller.PlayerExitedRange();
        }*/

        public void Destroy() => StartCoroutine(EnemyDeathSequence());

        private IEnumerator EnemyDeathSequence()
        {
            Controller.ToggleKillOverlay(true);
            Controller.ShakeCamera();

            yield return new WaitForSeconds(0.1f);

            var blood = Instantiate(bloodStain);
            blood.transform.position = transform.position;
            Controller.ToggleKillOverlay(false);

            Destroy(gameObject);
        }

        public void ChangeColor(EnemyColorType colorType) => enemyGraphic.color = enemyColors.Find(item => item.Type == colorType).Color;

        public void SetDefaultColor(EnemyColorType colorType)
        {
            EnemyColor coloToSetAsDefault = new EnemyColor();
            coloToSetAsDefault.Type = EnemyColorType.Default;
            coloToSetAsDefault.Color = enemyColors.Find(item => item.Type == colorType).Color;
            
            enemyColors.Remove(enemyColors.Find(item => item.Type == EnemyColorType.Default));
            enemyColors.Add(coloToSetAsDefault);
        }

        public void LogDebug(string message) => debugMessage = message;

        private void OnDrawGizmos()
        {
            Controller?.DrawGizmos();
        }
    }

    [System.Serializable]
    public struct EnemyColor
    {
        public EnemyColorType Type;
        public Color Color;
    }

    public enum EnemyColorType
    {
        Default,
        Vulnerable,
        Clone
    }
}