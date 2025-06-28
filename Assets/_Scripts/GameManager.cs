using UnityEngine;

namespace _Scripts
{
    public class GameManager : MonoBehaviour
    {
        [Header("点击粒子效果")] [SerializeField] private GameObject _clickParticlePrefab;
        [SerializeField] private LayerMask _raycastLayerMask;

        [Header("引用")] private Camera _camera;
        private Ray _cameraRay;
        private RaycastHit _cameraRayHit;
        private GameObject _previousHitObject;

        private static GameManager Instance { get; set; }

        #region Unity Lifecycle

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            _camera = Camera.main;
        }

        private void Update()
        {
            HandleCameraRay();
        }

        #endregion

        #region Private Methods

        private void HandleCameraRay()
        {
            // 更新摄像机射线
            _cameraRay = _camera.ScreenPointToRay(Input.mousePosition);

            // 如果射线没有击中任何物体
            if (!Physics.Raycast(_cameraRay, out _cameraRayHit, Mathf.Infinity, _raycastLayerMask)) return;

            // 检测鼠标左键点击
            if (Input.GetMouseButtonDown(0)) ParticleOnClick();
        }

        private void ParticleOnClick()
        {
            var particle = Instantiate(_clickParticlePrefab, _cameraRayHit.point, Quaternion.Euler(0, 90, 0));
            StartCoroutine(Utilities.DestroyAfterDelay(particle, 1f));
        }

        #endregion
    }
}