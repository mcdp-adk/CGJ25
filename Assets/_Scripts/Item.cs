using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace _Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class Item : MonoBehaviour
    {
        // 定义物体可能的状态
        private enum ItemState
        {
            None, // 无状态
            Idle, // 闲置，自己移动
            Dragging, // 被鼠标拖拽
            Focused // 被单击聚焦，停止移动
        }

        [Header("摄像机设置")] [SerializeField] private GameObject _defaultCameraTarget; // 默认摄像机目标

        [Header("物体设置")] [SerializeField] protected Vector2 _selfJumpIntervalRange; // 物体自身移动间隔
        [SerializeField] protected Vector2 _selfVerticalJumpSpeedRange; // 物体自身垂直跳跃速度
        [SerializeField] protected Vector2 _selfHorizontalJumpSpeedRange; // 物体自身水平跳跃速度
        [SerializeField] protected float _selfLookRotationMaxAngle; // 物体自身最大旋转角度
        private float _nextJumpTime; // 下次跳跃时间
        private float _mouseDownTime; // 鼠标按下时间
        private Vector3 _mouseDragOffset; // 鼠标拖拽偏移量

        [Header("进度设置")] [SerializeField] private List<string> _talkTexts; // 对话文本列表
        private int _progressCounter; // 进度计数器

        [Header("组件引用")] private Camera _mainCamera; // 主摄像机引用
        private Rigidbody _rigidbody;

        private ItemState _currentState = ItemState.None; // 当前状态

        #region Unity Lifecycle

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _mainCamera = Camera.main;
            _progressCounter = _talkTexts.Count;
            Debug.Log(_talkTexts.Count);

            _rigidbody.isKinematic = true;
        }

        private void FixedUpdate()
        {
            // 根据当前状态执行不同的逻辑
            switch (_currentState)
            {
                case ItemState.Idle:
                    HandleSelfMovement();
                    break;
                case ItemState.Dragging:
                    HandleDragging();
                    break;
                case ItemState.Focused:
                    break;
            }
        }

        #endregion


        #region 处理鼠标逻辑

        private void OnMouseDown()
        {
            _mouseDownTime = Time.time;

            var mouseWorldPosition = GetMouseWorldPosition(transform.position);
            if (!mouseWorldPosition.HasValue) return;

            _mouseDragOffset = transform.position - mouseWorldPosition.Value;
        }

        private void OnMouseDrag()
        {
            // 按下时间超过阈值，则切换到拖拽状态
            if (Time.time - _mouseDownTime > 0.1f)
            {
                SwitchState(ItemState.Dragging);
            }
        }

        private void OnMouseUp()
        {
            // 根据按下时长判断是单击还是拖拽结束
            if (Time.time - _mouseDownTime < 0.1f) // 单击
            {
                switch (_currentState)
                {
                    // 在闲置和聚焦状态间切换
                    case ItemState.Idle:
                        SwitchState(ItemState.Focused);
                        break;
                    case ItemState.None or ItemState.Focused:
                        SwitchState(ItemState.Idle);
                        break;
                }
            }
            else // 拖拽结束
            {
                // 从拖拽状态切换回闲置状态
                if (_currentState == ItemState.Dragging)
                {
                    SwitchState(ItemState.Idle);
                }
            }
        }

        #endregion


        #region 状态机逻辑

        private void SwitchState(ItemState newState)
        {
            if (_currentState == newState) return;

            _currentState = newState;

            // 根据进入的新状态，执行初始化操作
            switch (newState)
            {
                case ItemState.Idle:
                    _rigidbody.isKinematic = false;
                    // 恢复默认摄像机目标
                    var cameraTarget = GameObject.FindGameObjectWithTag("CameraTarget").transform;
                    cameraTarget.position = _defaultCameraTarget.transform.position;
                    cameraTarget.rotation = _defaultCameraTarget.transform.rotation;

                    UpdateProgress(0, "");
                    break;
                case ItemState.Dragging:
                    _rigidbody.isKinematic = true;
                    break;
                case ItemState.Focused:
                    _rigidbody.isKinematic = true;
                    // 设置摄像机目标为当前物体
                    var camTarget = GameObject.FindGameObjectWithTag("CameraTarget").transform;
                    camTarget.position = transform.position;
                    camTarget.rotation = transform.rotation;

                    // ReSharper disable once UseIndexFromEndExpression
                    UpdateProgress(0, _talkTexts[_talkTexts.Count - _progressCounter]);
                    break;
            }
        }

        private void HandleSelfMovement()
        {
            RandomLookRotation(_selfLookRotationMaxAngle);
            Jump();
        }

        private void HandleDragging()
        {
            var mouseWorldPosition = GetMouseWorldPosition(transform.position);
            if (!mouseWorldPosition.HasValue) return;
            _rigidbody.MovePosition(mouseWorldPosition.Value + _mouseDragOffset);
        }

        #endregion


        #region 类内工具方法

        private void Jump()
        {
            if (Time.time < _nextJumpTime) return;
            _nextJumpTime = Time.time + Random.Range(_selfJumpIntervalRange.x, _selfJumpIntervalRange.y);
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

        private Vector3? GetMouseWorldPosition(Vector3 itemPosition)
        {
            if (!_mainCamera) return null;
            var mousePosition = Input.mousePosition;
            mousePosition.z = _mainCamera.WorldToScreenPoint(itemPosition).z;
            return _mainCamera.ScreenToWorldPoint(mousePosition);
        }

        private void UpdateProgress(int progressToUpdate, string talkText)
        {
            Utilities.SetTalkText(talkText);
            _progressCounter -= progressToUpdate;
        }

        #endregion
    }
}