using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using MyNamespace.tunit;

namespace MyNamespace.databridge
{
    #region builderInterfaces
    public abstract class DefaultBridgeTaskBuilder : BridgeTaskBuilder
    {
        public override BridgeTask GetProduct()
        {
            return _product;
        }

        protected override void BuildDestnation(Component destnationComponent)
        {
            _product.destinationComponent = destnationComponent;
        }

        protected override void BuildParament(object parament)
        {
            _product.parament = parament;
        }

        protected override void BuildOrigin(Component originComponent)
        {
            _product.originComponent = originComponent;
        }

        protected override void DefineBridgeParamentType(BridgeParamentType bridgeParamentType)
        {
            _product.bridgeParamentType = bridgeParamentType;
        }

        protected BridgeTask _product;
    }

    public abstract class BridgeTaskBuilder
    {
        protected abstract void BuildOrigin(Component originComponent);
        protected abstract void BuildParament(object parament);
        protected abstract void DefineBridgeParamentType(BridgeParamentType bridgeParamentType);
        protected abstract void BuildDestnation(Component destnationComponent);
        public abstract BridgeTask GetProduct();
    }

    public struct BridgeTask
    {
        public Component originComponent;
        public object parament;
        public BridgeParamentType bridgeParamentType;
        public Component destinationComponent;
    }
    
    public enum BridgeParamentType
    {
        UnknownAction,
        TUnitNormalAttackTUnit,
        TUnitPoisonAttackTUnit,
        Chara_MousePosition_RayMapPathFinding,
    }
    #endregion

    namespace AllowedParaments
    {
        public class MyPair_Transform_SpriteRenderer
        {
            public MyPair_Transform_SpriteRenderer(Transform t,SpriteRenderer sr)
            {
                transform = t;
                spriteRenderer = sr;
            }
            public Transform transform;
            public SpriteRenderer spriteRenderer;
        }
        
        public class PoisonAttackRequestParament
        {
            public PoisonAttackRequestParament(int indamage, float ingapTime, int inrepeatTimes)
            {
                damage = indamage;
                gapTime = ingapTime;
                repeatTimesRemaining = inrepeatTimes;
            }
            public PoisonAttackRequestParament Fade()
            {
                repeatTimesRemaining -= 1;
                return this;
            }
            public int damage;
            public float gapTime;
            public int repeatTimesRemaining;
        }
    }

    //--------------------MonoBehaviour--------------------//
    public class TDataBridge : MonoBehaviour
    {
        #region UnityFeed
        public MyNamespace.rayMapPathFinding.RayMapPathFinding defaultRayMapPathFindingScript;
        public MyNamespace.spriteSortingOrder.TSpriteSortingOrder defaultSpriteSortingOrderScript;
        #endregion
        public Queue<BridgeTask> bridgeTasks = new Queue<BridgeTask>(0);
        
        public void EnqueueTask(BridgeTask inTask)
        {
            bridgeTasks.Enqueue(inTask);
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            while (bridgeTasks.Count > 0)
            {
                BridgeTask currentTask = bridgeTasks.Dequeue();
                switch(currentTask.bridgeParamentType)
                {
                    case BridgeParamentType.TUnitNormalAttackTUnit:
                        {
                            TUnit dC = currentTask.destinationComponent as TUnit;
                            if (dC.unit.IsDead)
                            { }
                            else
                               dC.unit.BeingAttack((int)currentTask.parament);

                        }
                        break;
                    case BridgeParamentType.TUnitPoisonAttackTUnit:
                        {
                            TUnit dC = currentTask.destinationComponent as TUnit;
                            if (dC.unit.IsDead)
                            { }
                            else
                            {
                                StartCoroutine(
                                    dC.unit.PositionEffect(
                                        bridgeTasks, currentTask.originComponent,
                                        (AllowedParaments.PoisonAttackRequestParament)currentTask.parament,
                                        currentTask.destinationComponent
                                        ));
                            }
                        }
                        break;
                    case BridgeParamentType.Chara_MousePosition_RayMapPathFinding:
                        {
                            rayMapPathFinding.RayMapPathFinding dC = currentTask.destinationComponent as rayMapPathFinding.RayMapPathFinding;
                            dC.Entrance((Vector2)currentTask.parament);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
