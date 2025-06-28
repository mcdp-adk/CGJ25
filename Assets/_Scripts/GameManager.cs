using UnityEngine;

namespace _Scripts
{
    public class GameManager : MonoBehaviour
    {
        [Header("点击粒子效果")] [SerializeField] private GameObject _clickParticlePrefab;
        [SerializeField] private LayerMask _raycastLayerMask;
        private Camera _camera;

        private static GameManager Instance { get; set; }

        #region Unity Lifecycle

        private void Awake()
        {
            if (Instance == null)
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
            // 检测鼠标左键点击
            if (Input.GetMouseButtonDown(0))
            {
                ParticleOnClick();
            }
        }

        #endregion

        #region Private Methods

        private void ParticleOnClick()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _raycastLayerMask)) return;
            GameObject particle = Instantiate(_clickParticlePrefab, hit.point, Quaternion.Euler(0, 90, 0));
            StartCoroutine(Utilities.DestroyAfterDelay(particle, 1f));
        }

        #endregion
    }
}