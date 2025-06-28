using System;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace _Scripts
{
    public abstract class ItemBase : MonoBehaviour
    {
        [Header("物体设置")] [SerializeField] protected Vector2 _selfJumpIntervalRange; // 物体自身移动间隔
        [SerializeField] protected Vector2 _selfVerticalJumpSpeedRange; // 物体自身垂直跳跃速度
        [SerializeField] protected Vector2 _selfHorizontalJumpSpeedRange; // 物体自身水平跳跃速度
        [SerializeField] protected float _selfLookRotationMaxAngle; // 物体自身最大旋转角度
        private float _nextJumpTime; // 下次跳跃时间

        // 组件引用
        private Rigidbody _rigidbody;

        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

            // 初始化下一次跳跃时间
            _nextJumpTime = Time.time + Random.Range(_selfJumpIntervalRange.x, _selfJumpIntervalRange.y);
        }

        protected virtual void Update()
        {
            throw new NotImplementedException(); 
        }

        protected void FixedUpdate()
        {
            HandleSelfMovement();
        }

        #region 子类需实现方法

        public abstract void ItemOnHold();

        protected abstract void HandleSelfMovement();

        #endregion

        #region 类外交互方法

        public virtual void ItemOnHover()
        {
        }

        public virtual void ItemOnClicked()
        {
        }

        #endregion

        #region 类内工具方法

        protected virtual void Jump()
        {
            if (Time.time < _nextJumpTime) return;

            // 从区间内随机下一次跳跃的间隔
            _nextJumpTime = Time.time + Random.Range(_selfJumpIntervalRange.x, _selfJumpIntervalRange.y);

            // 从区间内随机跳跃速度
            var verticalJumpSpeed = Random.Range(_selfVerticalJumpSpeedRange.x, _selfVerticalJumpSpeedRange.y);
            var horizontalJumpSpeed = Random.Range(_selfHorizontalJumpSpeedRange.x, _selfHorizontalJumpSpeedRange.y);

            _rigidbody.AddForce(transform.up * verticalJumpSpeed + transform.forward * horizontalJumpSpeed, ForceMode.Impulse);
        }

        protected virtual void RandomLookRotation(float maxAngle)
        {
            if (Time.time < _nextJumpTime) return;
            
            var randomAngle = Random.Range(-maxAngle, maxAngle);
            var targetRotation = transform.rotation * Quaternion.Euler(0, randomAngle, 0);
            transform.DORotateQuaternion(targetRotation, 0.2f);
        }

        #endregion
    }
}