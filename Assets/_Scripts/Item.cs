using UnityEngine;
using DG.Tweening;

namespace _Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class Item : MonoBehaviour
    {
        [Header("摄像机设置")] [SerializeField] private GameObject _defaultCameraTarget; // 默认摄像机目标

        [Header("物体设置")] [SerializeField] protected Vector2 _selfJumpIntervalRange; // 物体自身移动间隔
        [SerializeField] protected Vector2 _selfVerticalJumpSpeedRange; // 物体自身垂直跳跃速度
        [SerializeField] protected Vector2 _selfHorizontalJumpSpeedRange; // 物体自身水平跳跃速度
        [SerializeField] protected float _selfLookRotationMaxAngle; // 物体自身最大旋转角度
        private float _nextJumpTime; // 下次跳跃时间
        private float _mouseDownTime; // 鼠标按下时间
        private Vector3 _mouseDragOffset; // 鼠标拖拽偏移量
        private bool _shouldSelfMove; // 是否需要物体自身移动

        [Header("组件引用")] private Rigidbody _rigidbody;

        #region Unity Lifecycle

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = true;
        }

        private void FixedUpdate()
        {
            if (!_shouldSelfMove) return;
            HandleSelfMovement();
        }

        #endregion


        #region 处理鼠标逻辑

        private void OnMouseDown()
        {
            _mouseDownTime = Time.time;

            var mouseWorldPosition = GetMouseWorldPosition(transform.position);
            if (!mouseWorldPosition.HasValue) return;

            // 计算并存储鼠标位置和物体位置之间的偏移量
            _mouseDragOffset = transform.position - mouseWorldPosition.Value;
        }

        private void OnMouseDrag()
        {
            if (Time.time - _mouseDownTime < 0.1f) return;

            _shouldSelfMove = false;
            _rigidbody.isKinematic = true;

            var mouseWorldPosition = GetMouseWorldPosition(transform.position);
            if (!mouseWorldPosition.HasValue) return;
            _rigidbody.MovePosition(mouseWorldPosition.Value + _mouseDragOffset);
        }

        private void OnMouseUp()
        {
            if (Time.time - _mouseDownTime < 0.1f)
            {
                var cameraTarget = GameObject.FindGameObjectWithTag("CameraTarget").transform;

                if (_shouldSelfMove)
                {
                    _shouldSelfMove = false;
                    _rigidbody.isKinematic = true;

                    cameraTarget.transform.position = transform.position;
                    cameraTarget.transform.rotation = transform.rotation;
                }
                else
                {
                    _shouldSelfMove = true;
                    _rigidbody.isKinematic = false;

                    cameraTarget.transform.position = _defaultCameraTarget.transform.position;
                    cameraTarget.transform.rotation = _defaultCameraTarget.transform.rotation;
                }
            }
            else
            {
                _shouldSelfMove = true;
                _rigidbody.isKinematic = false;
            }
        }

        #endregion


        #region 类内逻辑代码

        private void HandleSelfMovement()
        {
            RandomLookRotation(_selfLookRotationMaxAngle);
            Jump();
        }

        #endregion


        #region 类内工具方法

        private void Jump()
        {
            if (Time.time < _nextJumpTime) return;

            // 从区间内随机下一次跳跃的间隔
            _nextJumpTime = Time.time + Random.Range(_selfJumpIntervalRange.x, _selfJumpIntervalRange.y);

            // 从区间内随机跳跃速度
            var verticalJumpSpeed = Random.Range(_selfVerticalJumpSpeedRange.x, _selfVerticalJumpSpeedRange.y);
            var horizontalJumpSpeed = Random.Range(_selfHorizontalJumpSpeedRange.x, _selfHorizontalJumpSpeedRange.y);

            _rigidbody.AddForce(transform.up * verticalJumpSpeed + transform.forward * horizontalJumpSpeed,
                ForceMode.Impulse);
        }

        private void RandomLookRotation(float maxAngle)
        {
            if (Time.time < _nextJumpTime) return;

            var randomAngle = Random.Range(-maxAngle, maxAngle);
            var targetRotation = transform.rotation * Quaternion.Euler(0, randomAngle, 0);
            transform.DORotateQuaternion(targetRotation, 0.2f);
        }

        private static Vector3? GetMouseWorldPosition(Vector3 itemPosition)
        {
            if (!Camera.main) return null;

            var mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.WorldToScreenPoint(itemPosition).z; // 设置鼠标位置的 Z 轴为物体的 Z 轴
            var worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            return worldPosition;
        }

        #endregion
    }
}