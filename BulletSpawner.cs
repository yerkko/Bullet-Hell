using UnityEngine;

namespace BulletHell.Spawners
{
    public class BulletSpawner : MonoBehaviour
    {
        [Header("Bullet movement parameters")] [SerializeField]
        public float m_BulletAcceleration;

        [SerializeField] public float m_BulletCurve;
        [SerializeField] public float m_BulletSpeed      = 1f;
        [SerializeField] public int   m_BulletsPerArrays = 10;
        [SerializeField] public float m_BulletTtl        = 5f;
        [SerializeField] public float m_DefaultAngle;

        [SerializeField] public float m_FireRate = 5f;

        [Header("Spin parameters")] [SerializeField]
        public float m_SpinRate;

        [SerializeField] public float m_MaxSpinRate;
        [SerializeField] public float m_SpinModificator;
        [SerializeField] public bool  m_InvertSpin;

        [Header("Spawner size")] [SerializeField]
        public float m_ObjectHeight;

        [SerializeField] public float m_ObjectWidth;


        [SerializeField] public int         m_PatternArrays = 2;
        private                 GameObject  _bulletRoot;
        private                 float       _shoot;
        private                 bool        _shouldShoot;
        public                  Vector3     m_SpawnPosition = Vector3.zero;


        [Header("Spawn parameters")] [SerializeField]
        public float m_SpreadBetweenArray = 180f;

        [SerializeField] public float m_SpreadWithinArray = 90f;
        [SerializeField] public float m_StartAngle;


        [SerializeField] public float m_BeginSpinSpeed = 1f;

        [SerializeField] public GameObject m_BulletPrefab;


        public void Awake() { }


        public void Start()
        {
            PoolManager.Instance.warmPool(m_BulletPrefab, 100);
        }

        public void Update()
        {
            Draw();
        }

        public void Draw()
        {
            CalculateBullets();

            m_DefaultAngle %= 360f;
            m_DefaultAngle += m_SpinRate        * Time.deltaTime;
            m_SpinRate     += m_SpinModificator * Time.deltaTime;

            if (m_InvertSpin && (m_SpinRate < -m_MaxSpinRate || m_SpinRate > m_MaxSpinRate))
                m_SpinModificator = -m_SpinModificator;

            _shoot += Time.deltaTime;

            if (_shoot >= m_FireRate)
            {
                _shouldShoot = true;
                _shoot       = 0;
            }
            else
            {
                _shouldShoot = false;
            }
        }

        public void CalculateBullets()
        {
            var bulletLength                    = m_BulletsPerArrays - 1;
            if (bulletLength == 0) bulletLength = 1;

            var arrayLength = m_PatternArrays - m_PatternArrays;

            if (arrayLength == 0) arrayLength = 1;

            var arrayAngle  = m_SpreadWithinArray  / bulletLength;
            var bulletAngle = m_SpreadBetweenArray / arrayLength;

            if (!_shouldShoot) return;
            for (var i = 0; i < m_PatternArrays; i++)
            for (var j = 0; j < m_BulletsPerArrays; j++)
                CalculateBullet(i, j, arrayAngle, bulletAngle);
        }


        public void CalculateBullet(int i, int j, float arrayAngle, float bulletAngle)
        {
            var position = transform.position;
            var x1 = position.x + LengthDirX(m_ObjectWidth,
                                             m_DefaultAngle + bulletAngle * i + arrayAngle * j + m_StartAngle);
            var y1 = position.y + LengthDirZ(m_ObjectHeight,
                                             m_DefaultAngle + bulletAngle * i + arrayAngle * j + m_StartAngle);

            var direction = m_DefaultAngle + bulletAngle * i + arrayAngle * j + m_StartAngle;

            m_SpawnPosition.x = x1;
            m_SpawnPosition.y = y1;
            m_SpawnPosition.y = position.y;
            var bullet           = FireBullet(m_SpawnPosition, Quaternion.identity);
            var bullet_component = bullet.GetComponent<Bullet>();
            bullet_component.Launch(x1, y1, direction, m_BulletSpeed, m_BulletAcceleration, m_BulletCurve, m_BulletTtl);
            //_bulletList.Add(bullet_component);
        }


        public GameObject FireBullet(Vector3 position, Quaternion rotation)
        {
            return PoolManager.SpawnObject(m_BulletPrefab, position, rotation);
        }


        private static float LengthDirX(float distance, float angle)
        {
            return distance * Mathf.Cos(Mathf.PI * angle / 180f);
        }

        private static float LengthDirZ(float distance, float angle)
        {
            return distance * Mathf.Sin(Mathf.PI * angle / 180f);
        }
    }
}